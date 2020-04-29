using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CC.Net.Collections;
using CC.Net.Db;
using CC.Net.Extensions;
using CC.Net.Hubs;
using CC.Net.Services.Courses;
using CC.Net.Services.Languages;
using CC.Net.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CC.Net.Services
{
    public partial class ProcessService: BackgroundService
    {
        private ILogger<ProcessService> _logger;
        private readonly IServiceScopeFactory _serviceProvider;

        public static readonly string ContainerName = "automatestWorker";

        public ProcessService(ILogger<ProcessService> logger, IServiceScopeFactory serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                _logger.LogInformation("ProcessService is starting.");

                var dockerPurge = ProcessUtils.Popen($"docker rm -f {ContainerName}");
                var dockerStart = ProcessUtils.Popen($"docker run -di --name {ContainerName} automatest/all");

                // TODO: configurable period
                while (stoppingToken.IsCancellationRequested == false)
                {
                    await DoWork();
                    await Task.Delay(5 * 1000, stoppingToken);
                }
            });
        }


        private async Task DoWork()
        {
            _logger.LogInformation("checking db");
            using (var scope = _serviceProvider.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var dbService = provider.GetService<DbService>();

                var cursor = await dbService.Data
                    .FindAsync(i => i.Result.Status == ProcessStatus.InQueue.Value);

                var items = await cursor
                    .ToListAsync();

                _logger.LogInformation($"Found {items.Count} items to process");

                foreach (var item in items)
                {
                    if (item.Action == "solve")
                    {
                        var processItem = new ProcessItem(
                            provider.GetService<ILogger<ProcessItem>>(),
                            provider.GetService<CourseService>(),
                            provider.GetService<LanguageService>(),
                            provider.GetService<IdService>(),
                            provider.GetService<IHubContext<LiveHub>>(),
                            provider.GetService<CompareService>(),
                            item
                        );

                        _logger.LogInformation($"Processing item {item?.Id} {item?.User} {item?.CourseName}-{item?.CourseYear} {item?.Problem}");
                        await processItem.Solve();
                    }
                }
            }
        }
    }
}
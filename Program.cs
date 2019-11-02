using System.IO;
using CC.Net.Collections;
using CC.Net.Entities;
using CC.Net.Services.Courses;
using CC.Net.Services.Languages;
using CC.Net.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TypeLite;

namespace CC.Net
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--generate")
            {
                Directory.CreateDirectory("_client/src/models/");
                File.WriteAllText(
                    "_client/src/models/DataModel.d.ts",
                    TypeScript.Definitions()
                        .For<User>()
                        .For<Language>()
                        .For<Course>()
                        .For<CcData>()
                        .For<TableRequest>()
                        .For<TableResponse>()
                        .WithModuleNameFormatter((moduleName) => "")
                        .WithTypeFormatter((type, f) => "I" + ((TypeLite.TsModels.TsClass)type).Name)
                        .Generate());
                System.Environment.Exit(0);
            }

            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.secret.json", optional: true)
            .AddCommandLine(args)
            .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<Startup>();
        }
    }
}
;
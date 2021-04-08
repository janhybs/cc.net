using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cc.Net;
using Cc.Net.Apis;
using CC.Net.Collections;
using CC.Net.Db;
using CC.Net.Entities;
using CC.Net.Services;
using Cc.Net.Utils;
using CC.Net.Services.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CC.Net.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class NotificationsController: ControllerBase
    {

        private readonly UserService _userService;
        private readonly DbService _dbService;
        
        public NotificationsController(UserService userService, DbService dbService)
        {
            _userService = userService;
            _dbService = dbService;
        }
        
        [HttpGet("notifications-get")]
        [ProducesResponseType(typeof(ApiListResponse<CcEvent>), StatusCodes.Status200OK)]
        public async Task<IActionResult> NotificationsGet(string objectId, string path)
        {
            var user = _userService.CurrentUser;
            var minDate = DateTime.Now.Subtract(TimeSpan.FromDays(30));
            var minId = new ObjectId(minDate, 0, 0, 0);
            var notifications = await _dbService.Events
                .Find(i => i.Id > minId && i.Reciever == user.Id)
                .SortByDescending(i => i.IsNew)
                .SortByDescending(i => i.Id)
                .Limit(30)
                .ToListAsync();

            var result = new ApiListResponse<CcEvent>(notifications);

            return Ok(result);
        }
    }
}
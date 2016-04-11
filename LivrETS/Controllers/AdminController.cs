/*
LivrETS - Centralized system that manages selling of pre-owned ETS goods.
Copyright (C) 2016  TribuTerre

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using LivrETS.Models;
using LivrETS.Services;
using LivrETS.ViewModels.Account;
using System.Collections.Generic;
using Newtonsoft.Json;
using LivrETS.ViewModels.Admin;

namespace LivrETS.Controllers
{
    [Authorize(Policy = "AdministrationRights")]
    public class AdminController : Controller
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public AdminController(
            ILoggerFactory loggerFactory,
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager
            )
        {
            _logger = loggerFactory.CreateLogger<AdminController>();
            _db = db;
            _userManager = userManager;
        }
        
        /// <summary>
        /// GET /Admin/ManageUsers 
        /// </summary>
        [HttpGet]
        public IActionResult ManageUsers()
        {
            ViewBag.users = (
                from user in _db.Users
                orderby user.LastName ascending
                select user
            ).ToList();
            ViewBag.roles = _db.Roles.ToList();
            return View();
        }
        
        /// <summary>
        /// PUT: /Admin/ChangeUserRole
        /// </summary>
        /// <param name="model">
        ///     UserId: The id is needed to change the role of that user.
        ///     NewRole: The name of the new role. If "User" is passed, then all roles are removed.
        /// </param>
        /// <returns>200 if OK. 400 otherwise.</returns>
        [HttpPut]
        public async Task<IActionResult> ChangeUserRole([FromBody] AjaxUsersViewModel model)
        {
            _logger.LogInformation("Role Change Requested");
            var user = await _userManager.FindByIdAsync(model.UserId);
            
            if (user == null)
            {
                return new HttpStatusCodeResult(400);
            }
            
            // Remove from all roles, just in case.
            await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));
            
            if (model.NewRole != null)  // If null, user is a simple user of the system. 
            {
                await _userManager.AddToRoleAsync(user, model.NewRole);
            }
            
            _logger.LogInformation("Role Change Successful!");
            return new HttpOkResult();
        }
        
        /// <summary>
        /// Deletes a user from the system.
        /// </summary>
        /// <param name="model">
        ///     UserId: The id of the user to delete.
        /// </param>
        /// <returns>200 if deleted</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] AjaxUsersViewModel model)
        {
            _logger.LogInformation("Delete User Requested");
            var user = await _userManager.FindByIdAsync(model.UserId);
            
            if (user == null)
            {
                return new HttpOkResult();
            }
            
            await UnregisterAsync(user);
            _logger.LogInformation("User deleted successfully!");
            return new HttpOkResult();
        }
        
        /// <summary>
        /// Deletes multiple users.
        /// </summary>
        /// <param name="model">
        ///     UserIds: All of the ids of users to delete separated by commas.
        /// </param>
        /// <returns>200 if OK.</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteUsers([FromBody] AjaxUsersViewModel model)
        {
            var ids = model.UserIds.Split(',');
            
            foreach (var id in ids)
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user == null) {
                    continue;
                }
                
                await UnregisterAsync(user);
            }
            
            return new HttpOkResult();
        }
        
        /// <summary>
        /// Deletes a user completely from the system.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <returns>true if deleted. false otherwise.</returns>
        private async Task<bool> UnregisterAsync(ApplicationUser user)
        {
            var logins = user.Logins;
            var roles = await _userManager.GetRolesAsync(user);
            
            foreach (var login in logins)
            {
                await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            }
            
            if (roles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, roles);
            }

            await _userManager.DeleteAsync(user);            
            return true;
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerMeasure.Models;
using PowerMeasure.Models.DTO;
using PowerMeasure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserInterface _userService;
        private ITokenHandlerService _tokenService;

        public UserController(IUserInterface userService, ITokenHandlerService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("createUser")]
        public Task<Users> createUser([FromBody] AddUserRequest user)
        {
            return _userService.addUser(user);
        }
        [HttpGet("users")]
        [Authorize(Roles = "admin")]
        public Task<IEnumerable<Users>> getAllUsers()
        {
            return _userService.getUsers();
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginRequest loginInfo)
        {
            var user = await _userService.authenticateUser(loginInfo.EmailAddress, loginInfo.Password);
            if ( user != null)
            {
                var token = await _tokenService.CreateTokenAsync(user);
                return Ok(new { Token = token });
            }

            return BadRequest("Username or Password wrong");
        }

        [HttpGet("get-users-count")]
        public async Task<int> getUsersCount() {
            return await _userService.getUsersCount();
        }

        [HttpGet("get-admin-count")]
        public async Task<int> getAdminCount()
        {
            return await _userService.getAdminCount();
        }
    }
}

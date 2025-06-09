using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ToDoApp.Application.ActionFilters;
using ToDoApp.Application.Dtos.UserModel;
using ToDoApp.Application.Helpers;
using ToDoApp.Application.Services;
using ToDoApp.Application.Services.GoogleCredentialService;
using ToDoApp.Domains.Entities;

namespace ToDoApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    //[TypeFilter(typeof(LogFilter), Arguments = [LogLevel.Warning])]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IGoogleCredentialService _googleCredentialService;
        private readonly IMemoryCache _memoryCache;

        public UserController(
            IUserService userService, 
            ILogger<CourseController> logger,
            IGoogleCredentialService googleCredentialService,
            IMemoryCache memoryCache)
        {
            _userService = userService;
            _googleCredentialService = googleCredentialService;
            _memoryCache = memoryCache;
        }

        [HttpPost("/register")]
        public IActionResult Register(UserRegisterModel user)
        {
            var userId = _userService.Register(user);
            if(userId == -1)
            {
                return BadRequest("Error when register new user");
            }
            return Ok(userId);
        }

        [HttpPost("/login")]
        public IActionResult Login(UserLoginModel loginModel)
        {
            var user = _userService.Login(loginModel);

            if (user == null)
            {
                return BadRequest("Username or password are wrong");
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            
            return Ok("Login successfully");
        }

        [HttpPost("/login-cookies")]
        public async Task<IActionResult> LoginCookies(UserLoginModel loginModel)
        {
            var user = _userService.Login(loginModel);

            if (user == null)
            {
                return BadRequest("Username or password are wrong");
            }

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Email, user.EmailAddress),
                new (ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            return Ok("Login successfully");
        }
        
        [HttpPost("/login-google")]
        public async Task<IActionResult> LoginGoogle(GoogleLoginModel model)
        {
            var payload = await _googleCredentialService.VerifyCredential(model.Credential);

            // Todo: Register user if not exists
            var user = _userService.GetOrCreateUserFromGoogle(payload);
            // Todo: Generate JWT Token
            var jwtToken = _userService.GenerateJwt(user);
            Console.WriteLine(jwtToken);
            return Ok(jwtToken);
        }

        [HttpPost("/login-jwt")]
        public async Task<IActionResult> LoginJwt(UserLoginModel loginModel)
        {
            var user = _userService.Login(loginModel);

            if (user == null)
            {
                return BadRequest("Username or password are wrong");
            }

            // Delete old rt
            _userService.DeleteOldRefreshToken(user.Id);

            var accessToken = _userService.GenerateJwt(user);
            var refreshToken = _userService.GenerateRefreshToken(user.Id);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // USe Secure cookies in production
                Expires = DateTime.UtcNow.AddDays(7)
            };

            HttpContext.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

            return Ok(accessToken);
        }

        [HttpPost("/refresh-token")]
        public IActionResult RefreshToken()
        {
            var isExist = HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

            if (!isExist)
            {
                return Unauthorized("Refresh token is not found");
            }

            var user = _userService.GetUserByRefreshToken(refreshToken!);

            if(user == null)
            {
                return Unauthorized("Refresh token is not found");
            }

            // Delete old rt
            _userService.DeleteOldRefreshToken(user.Id);

            // Generate new at and rt
            var accessToken = _userService.GenerateJwt(user);
            var newRefreshToken = _userService.GenerateRefreshToken(user.Id);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // USe Secure cookies in production
                Expires = DateTime.UtcNow.AddDays(7)
            };

            HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken, cookieOptions);

            return Ok(accessToken);
        }

        [HttpPost("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            // check token
            var hasToken = HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if (!hasToken || string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("Refresh token missing.");
            }
            
            // get user
            var user = _userService.GetUserByRefreshToken(refreshToken!);
            if (user == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            _userService.DeleteOldRefreshToken(user.Id);

            // delete rt in cookies
            HttpContext.Response.Cookies.Append("refreshToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(-1)
            });
            return Ok();
        }

        // Cache build in .NET, time to live
        // Set user to InActive
        // Delete rt
        // User still keep at, assum expire time is 15 min

        // Ban user's access token
        // Create a authorize filter, cache contains black list at
        // Filter check if user's userId exist in the cache
        // Yes => return Unauthorize
        // No => continue processing
        [HttpPost("/revoke")]
        public IActionResult Revoke(int userId)
        {
            // Delete old rt
            _userService.DeleteOldRefreshToken(userId);
            _memoryCache.Set($"BLACK_LIST:{userId}", true, TimeSpan.FromMinutes(15));
            return Ok();
        }

        [HttpPost("/logout-cookies")]
        public async Task<IActionResult> LogoutCookies()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
    }
}

using DL_LeaveManagementSystem.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace LeaveManagementSystem.Controllers.API
{
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly ILeaveRepository _repo;

        public AuthApiController(ILeaveRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                    return BadRequest(new { success = false, message = "Email and password required" });

                var user = await _repo.GetUserByEmail(request.Email, request.Password);

                if (user == null)
                    return Ok(new { success = false, message = "Invalid email or password" });


                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetInt32("Role", user.Role);


                var redirectUrl = user.Role == 2 ? "/Manager/Dashboard" : user.Role == 3 ? "/HR/Dashboard" : "/Employee/Dashboard";

                return Ok(new { success = true, role = user.Role, redirectUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Login error: " + ex.Message);
                return Ok(new { success = false, message = "Something went wrong. Please try again." });
            }
            
        }


        [HttpGet]
        public IActionResult Logout()
        {
            var userName = HttpContext.Session.GetString("UserId");
            Log.Information($"User Logged Out. UserName: {userName}");

            HttpContext.Session.Clear();
            return Ok(new { success = true, redirectUrl = "/Account/Login" });
        }
    }


    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}

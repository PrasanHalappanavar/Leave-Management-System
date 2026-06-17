using DL_LeaveManagementSystem;
using DL_LeaveManagementSystem.Abstract;
using DL_LeaveManagementSystem.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers.API
{
    [Route("api/hr")]
    [ApiController]
    public class HRApiController : ControllerBase
    {
        private readonly ILeaveRepository _repo;
        private readonly EmailHelper _email;

        public HRApiController(ILeaveRepository repo, EmailHelper email)
        {
            _repo = repo;
            _email = email;
        }

        private bool IsHR() => HttpContext.Session.GetInt32("UserId") > 0
                             && HttpContext.Session.GetInt32("Role") == 3;



        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                if (!IsHR())
                    return Unauthorized(new { message = "Unauthorized" });
                var data = await _repo.GetHRDashboard();
                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HR Dashboard error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load dashboard." });
            }
        }


        [HttpGet("allrequests")]
        public async Task<IActionResult> AllRequests()
        {
            try
            {
                if (!IsHR()) return Unauthorized(new { message = "Unauthorized" });
                var data = await _repo.GetAllRequests();
                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AllRequests error in HR Api: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load all requests." });
            }
            
        }




        [HttpPost("adduser")]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequest request)
        {
            if (!IsHR()) return
                Unauthorized(new { message = "Unauthorized" });

            var exists = await _repo.GetUserByEmail(request.Email, "");
            if (exists != null)
                return Ok(new { success = false, message = "Email already exists" });

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = HashHelper.HashPassword(request.Password),
                Role = request.Role,
                DepartmentId = request.DepartmentId,
                AvailableLeaveDays = request.Role == 1 ? 22 : 0,
                CreatedAt = DateTime.Now
            };

            await _repo.AddUser(user);


            // send welcome email after user saved successfully
            try
            {
                var roleText = request.Role == 1 ? "Employee" : "Manager";

                var body = $@"
                <div style='font-family:Segoe UI,sans-serif;max-width:500px;margin:0 auto;'>
                    <div style='background:#2c3e50;padding:24px;border-radius:8px 8px 0 0;'>
                        <h2 style='color:#fff;margin:0;'>Leave Management System</h2>
                    </div>
                    <div style='background:#f9f9f9;padding:28px;border-radius:0 0 8px 8px;'>
                        <p style='font-size:15px;color:#333;'>
                            Hi <strong>{request.FullName}</strong>,
                        </p>
                        <p style='font-size:14px;color:#555;'>
                            Your account has been created by HR.
                            You can now log in to the Leave Management System.
                        </p>
                        <div style='background:#fff;border-radius:6px;padding:16px;
                                    border-left:4px solid #3498db;margin:20px 0;'>
                            <p style='margin:0 0 8px;font-size:13px;color:#888;'>
                                Your login details
                            </p>
                            <p style='margin:4px 0;font-size:14px;'>
                                <strong>Email:</strong> {request.Email}
                            </p>
                            <p style='margin:4px 0;font-size:14px;'>
                                <strong>Password:</strong> {request.Password}
                            </p>
                            <p style='margin:4px 0;font-size:14px;'>
                                <strong>Role:</strong> {roleText}
                            </p>
                        </div>
                        <p style='font-size:13px;color:#888;'>
                            Please change your password after first login.
                        </p>
                    </div>
                </div>";

                _email.SendEmail(
                    request.Email,
                    "Welcome to Leave Management System",
                    body
                );

                return Ok(new { success = true, message = "User added and welcome email sent." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email error: " + ex.Message);
                return Ok(new { success = true, message = "User added but email could not be sent." });
            }

            return Ok(new { success = true, message = "User added successfully" });
        }





        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                if (!IsHR()) return Unauthorized(new { message = "Unauthorized" });
                var managers = await _repo.GetManagers();
                return Ok(managers);
            }
            catch (Exception ex) {
                Console.WriteLine("GetUsers error in HR Api: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load users." });
            }
            
        }


        [HttpGet("leavesummary")]
        public async Task<IActionResult> LeaveSummary()
        {
            try
            {
                if (!IsHR()) return Unauthorized(new { message = "Unauthorized" });
                var data = await _repo.GetLeaveSummaryAsync();
                return Ok(data);
            }
            catch (Exception ex) {
                Console.WriteLine("LeaveSummary error in HR Api: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load leavesummary." });
            }
        }
    }

    public class AddUserRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Role { get; set; }
        public int DepartmentId { get; set; }
    }
}

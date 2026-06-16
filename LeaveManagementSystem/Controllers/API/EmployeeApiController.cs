using DL_LeaveManagementSystem.Abstract;
using DL_LeaveManagementSystem.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers.API
{
    [Route("api/employee")]
    [ApiController]
    public class EmployeeApiController : ControllerBase
    {
        private readonly ILeaveRepository _repo;

        public EmployeeApiController(ILeaveRepository repo)
        {
            _repo = repo;
        }

        private int GetUserId() => HttpContext.Session.GetInt32("UserId") ?? 0;

        private bool IsEmployee() => GetUserId() > 0 && HttpContext.Session.GetInt32("Role") == 1;



        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                if (!IsEmployee())
                    return Unauthorized(new { message = "Unauthorized" });

                var data = await _repo.GetEmployeeDashboard(GetUserId());
                return Ok(data);
            }
            catch (Exception ex) {
                Console.WriteLine("Employee Dashboard error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load dashboard." });
            }
        }



        [HttpGet("leavetypes")]
        public async Task<IActionResult> GetLeaveTypes()
        {
            try
            {
                if (!IsEmployee())
                    return Unauthorized(new { message = "Unauthorized" });

                var types = await _repo.GetLeaveTypes();
                if (types == null || types.Count == 0)
                    return Ok(new List<object>());

                return Ok(types);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLeaveTypes error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load leave types." });
            }
        }



        [HttpGet("managers")]
        public async Task<IActionResult> GetManagers()
        {
            try
            {
                if (!IsEmployee())
                    return Unauthorized(new { message = "Unauthorized" });

                var managers = await _repo.GetManagers();

                if (managers == null || managers.Count == 0)
                    return Ok(new List<object>());

                return Ok(managers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetManagers error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load managers." });
            }
        }



        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromBody] ApplyLeaveRequest request)
        {
            try
            {
                if (!IsEmployee())
                    return Unauthorized(new { message = "Unauthorized" });

                if (request.FromDate == default || request.ToDate == default)
                    return Ok(new { success = false, message = "Please select valid dates." });

                if (request.ToDate < request.FromDate)
                    return Ok(new { success = false, message = "To Date cannot be before From Date." });

                if (string.IsNullOrWhiteSpace(request.Reason))
                    return Ok(new { success = false, message = "Please enter a reason." });

                var result = await _repo.ApplyLeave(
                    GetUserId(),
                    request.LeaveTypeId,
                    request.ManagerId,
                    request.FromDate,
                    request.ToDate,
                    request.Reason
                );

                if (result == "OVERLAP")
                    return Ok(new { success = false, message = "You already have a leave on these dates" });

                if (result == "INSUFFICIENT")
                    return Ok(new { success = false, message = "You do not have enough leave days available."});

                if (result == "SUCCESS")
                    return Ok(new { success = true, message = "Leave applied successfully" });

                return Ok(new { success = false, message = "Something went wrong" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Apply error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to apply leave. Please try again." });
            }
            
        }



        [HttpGet("myrequests")]
        public async Task<IActionResult> MyRequests()
        {
            try
            {
                if (!IsEmployee())
                    return Unauthorized(new { message = "Unauthorized" });

                await _repo.ExpireLeaves();

                var requests = await _repo.GetMyRequests(GetUserId());
                return Ok(requests ?? new List<MyLeaveRequest>());
            }
            catch (Exception ex)
            {
                Console.WriteLine("MyRequests error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load requests." });
            }
        }



        [HttpPost("cancel/{requestId}")]
        public async Task<IActionResult> Cancel(int requestId)
        {
            try
            {
                if (!IsEmployee())
                    return Unauthorized(new { message = "Unauthorized" });

                if (requestId <= 0)
                    return Ok(new { success = false, message = "Invalid request." });

                var result = await _repo.CancelLeave(requestId, GetUserId());

                if (!result)
                    return Ok(new { success = false, message = "Cannot cancel this request. It may already be processed." });

                return Ok(new { success = true, message = "Request cancelled successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cancel error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to cancel request. Please try again." });
            }
        }
    }



    public class ApplyLeaveRequest
    {
        public int LeaveTypeId { get; set; }
        public int ManagerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Reason { get; set; }
    }
}

using DL_LeaveManagementSystem.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Controllers.API
{
    [Route("api/manager")]
    [ApiController]
    public class ManagerApiController : ControllerBase
    {
        private readonly ILeaveRepository _repo;

        public ManagerApiController(ILeaveRepository repo)
        {
            _repo = repo;
        }


        private int GetManagerId() => HttpContext.Session.GetInt32("UserId") ?? 0;

        private bool IsManager() => GetManagerId() > 0 && HttpContext.Session.GetInt32("Role") == 2;



        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                if (!IsManager())
                    return Unauthorized(new { message = "Unauthorized" });

                await _repo.ExpireLeaves();

                var data = await _repo.GetManagerDashboard(GetManagerId());
                return Ok(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Manager Dashboard error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load dashboard." });
            }
            
        }



        [HttpGet("pending")]
        public async Task<IActionResult> PendingRequests()
        {
            try
            {
                if (!IsManager())
                    return Unauthorized(new { message = "Unauthorized" });

                await _repo.ExpireLeaves();

                var requests = await _repo.GetPendingRequests(GetManagerId());
                return Ok(requests);
            }
            catch (Exception ex) {
                Console.WriteLine("PendingRequests error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load Pending Requests." });
            }
        }



        [HttpPost("updatestatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                if (!IsManager()) return Unauthorized(new { message = "Unauthorized" });

                var result = await _repo.UpdateLeaveStatus(
                    request.RequestId,
                    request.Status,
                    GetManagerId(),
                    request.Remarks
                );

                if (result == "SUCCESS")
                    return Ok(new { success = true, message = $"Leave {request.Status} successfully" });

                return Ok(new { success = false, message = "Something went wrong" });
            }
            catch (Exception ex) {
                Console.WriteLine("Update Status error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to Update Status." });
            }
        }



        [HttpGet("history")]
        public async Task<IActionResult> LeaveHistory()
        {
            try
            {
                if (!IsManager())
                    return Unauthorized(new { message = "Unauthorized" });

                var history = await _repo.GetLeaveHistory(GetManagerId());
                return Ok(history);
            }
            catch (Exception ex) {
                Console.WriteLine("Leave History error: " + ex.Message);
                return Ok(new { success = false, message = "Failed to load Leave History." });
            }
        }
    }



    public class UpdateStatusRequest
    {
        public int RequestId { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}

using DL_LeaveManagementSystem.Abstract;
using DL_LeaveManagementSystem.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_LeaveManagementSystem.Repository
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly LMS_DbContext _context;
        private readonly CacheService _cache;

        public LeaveRepository(LMS_DbContext context, CacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<User> GetUserByEmail(string email, string password)
        {
            try
            {
                var hashedPassword = HashHelper.HashPassword(password);

                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetUserByEmail error: " + ex.Message);
                return null;
            }
        }

        public async Task<User> GetUserById(int userId)
        {
            try
            {
                return await _context.Users.Include(u => u.Department).FirstOrDefaultAsync(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetUserById error: " + ex.Message);
                return null;
            }
        }

        public async Task<List<LeaveType>> GetLeaveTypes()
        {
            try
            {
                const string cacheKey = "leave_types";
                var cached = _cache.Get<List<LeaveType>>(cacheKey);
                if (cached != null)
                    return cached;

                var types = await _context.LeaveTypes.ToListAsync();

                _cache.Set(cacheKey, types, 60);

                return types;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLeaveTypes error: " + ex.Message);
                return new List<LeaveType>();
            }
        }

        public async Task<string> ApplyLeave(int userId, int leaveTypeId, int managerId, DateTime fromDate, DateTime toDate, string reason)
        {
            try
            {
                var result = await _context.Database
                    .SqlQueryRaw<string>(
                        "EXEC sp_ApplyLeave @UserId, @LeaveTypeId, @ManagerId, @FromDate, @ToDate, @Reason",
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@LeaveTypeId", leaveTypeId),
                        new SqlParameter("@ManagerId", managerId),
                        new SqlParameter("@FromDate", fromDate),
                        new SqlParameter("@ToDate", toDate),
                        new SqlParameter("@Reason", reason)
                    ).ToListAsync();

                return result.FirstOrDefault() ?? "ERROR";
            }
            catch (Exception ex)
            {
                Console.WriteLine("ApplyLeave error: " + ex.Message);
                return "ERROR";
            }
        }

        public async Task<List<MyLeaveRequest>> GetMyRequests(int userId)
        {
            try
            {
                return await _context.Database
                    .SqlQueryRaw<MyLeaveRequest>(
                        "EXEC sp_GetMyRequests @UserId",
                        new SqlParameter("@UserId", userId)
                    ).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetMyRequests error: " + ex.Message);
                return new List<MyLeaveRequest>();
            }
        }

        public async Task<bool> CancelLeave(int requestId, int userId)
        {
            try
            {
                var request = await _context.LeaveRequests.FirstOrDefaultAsync(r => r.RequestId == requestId && r.UserId == userId && r.Status == "Pending");

                if (request == null)
                    return false;

                request.Status = "Cancelled";

                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.AvailableLeaveDays += request.TotalDays;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("CancelLeave error: " + ex.Message);
                return false;
            }
        }

        public async Task<List<PendingRequest>> GetPendingRequests(int managerId)
        {
            try
            {
                return await _context.Database
                    .SqlQueryRaw<PendingRequest>(
                        "EXEC sp_GetPendingRequests @ManagerId",
                        new SqlParameter("@ManagerId", managerId)
                    ).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetPendingRequests error: " + ex.Message);
                return new List<PendingRequest>();
            }
        }

        public async Task<string> UpdateLeaveStatus(int requestId, string status, int managerId, string remarks)
        {
            try
            {
                var result = await _context.Database
                    .SqlQueryRaw<string>(
                        "EXEC sp_UpdateLeaveStatus @RequestId, @Status, @ManagerId, @Remarks",
                        new SqlParameter("@RequestId", requestId),
                        new SqlParameter("@Status", status),
                        new SqlParameter("@ManagerId", managerId),
                        new SqlParameter("@Remarks", remarks ?? "")
                    ).ToListAsync();

                return result.FirstOrDefault() ?? "ERROR";
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateLeaveStatus error: " + ex.Message);
                return "ERROR";
            }
        }

        public async Task<object> GetEmployeeDashboard(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                var total = await _context.LeaveRequests.CountAsync(r => r.UserId == userId);
                var pending = await _context.LeaveRequests.CountAsync(r => r.UserId == userId && r.Status == "Pending");
                var approved = await _context.LeaveRequests.CountAsync(r => r.UserId == userId && r.Status == "Approved");

                return new
                {
                    user.AvailableLeaveDays,
                    TotalRequests = total,
                    PendingRequests = pending,
                    ApprovedRequests = approved
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetEmployeeDashboard error: " + ex.Message);
                return new
                {
                    AvailableLeaveDays = 0,
                    TotalRequests = 0,
                    PendingRequests = 0,
                    ApprovedRequests = 0
                };
            }
        }

        public async Task<object> GetManagerDashboard(int managerId)
        {
            try
            {
                var pending = await _context.LeaveRequests.CountAsync(r => r.ManagerId == managerId && r.Status == "Pending");
                var approved = await _context.LeaveRequests.CountAsync(r => r.ManagerId == managerId && r.Status == "Approved");
                var rejected = await _context.LeaveRequests.CountAsync(r => r.ManagerId == managerId && r.Status == "Rejected");

                return new
                {
                    PendingRequests = pending,
                    ApprovedRequests = approved,
                    RejectedRequests = rejected
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetManagerDashboard error: " + ex.Message);
                return new
                {
                    PendingRequests = 0,
                    ApprovedRequests = 0,
                    RejectedRequests = 0
                };
            }
        }

        public async Task<object> GetLeaveHistory(int managerId)
        {
            try
            {
                var result = await _context.Database
                    .SqlQueryRaw<LeaveHistoryResult>(
                        "EXEC sp_GetLeaveHistory @ManagerId",
                        new SqlParameter("@ManagerId", managerId)
                    ).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLeaveHistory error: " + ex.Message);
                return new List<LeaveHistoryResult>();
            }
        }

        public async Task<List<User>> GetManagers()
        {
            try
            {
                const string cacheKey = "managers_list";

                var cached = _cache.Get<List<User>>(cacheKey);
                if (cached != null)
                    return cached;

                var managers = await _context.Users.Where(u => u.Role == 2)
                    .Select(u => new User
                    {
                        UserId = u.UserId,
                        FullName = u.FullName
                    })
                    .ToListAsync();

                _cache.Set(cacheKey, managers, 30);

                return managers;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetManagers error: " + ex.Message);
                return new List<User>();
            }
        }

        public async Task<object> GetAllRequests()
        {
            try
            {
                var result = await _context.Database
                    .SqlQueryRaw<AllRequestResult>("EXEC sp_GetAllRequests")
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetAllRequests error: " + ex.Message);
                return new List<AllRequestResult>();
            }
        }

        public async Task<bool> AddUser(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                if (user.Role == 2)
                    _cache.Remove("managers_list");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddUser error: " + ex.Message);
                return false;
            }
        }

        public async Task<object> GetHRDashboard()
        {
            try
            {
                var totalEmployees = await _context.Users.CountAsync(u => u.Role == 1);
                var totalManagers = await _context.Users.CountAsync(u => u.Role == 2);
                var totalPending = await _context.LeaveRequests.CountAsync(r => r.Status == "Pending");

                return new
                {
                    TotalEmployees = totalEmployees,
                    TotalManagers = totalManagers,
                    PendingRequests = totalPending
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetHRDashboard error: " + ex.Message);
                return new
                {
                    TotalEmployees = 0,
                    TotalManagers = 0,
                    PendingRequests = 0
                };
            }
        }

        public async Task<List<LeaveSummaryResult>> GetLeaveSummaryAsync()
        {
            try
            {
                return await _context.Database
                    .SqlQueryRaw<LeaveSummaryResult>("EXEC sp_GetLeaveSummary")
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLeaveSummaryAsync error: " + ex.Message);
                return new List<LeaveSummaryResult>();
            }
        }



        public async Task ExpireLeaves()
        {
            try
            {
                await _context.Database
                    .ExecuteSqlRawAsync("EXEC sp_ExpireLeaves");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExpireLeaves error: " + ex.Message);
            }
        }
    }
}

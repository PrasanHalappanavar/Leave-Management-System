using DL_LeaveManagementSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_LeaveManagementSystem.Abstract
{
    public interface ILeaveRepository
    {
        Task<User> GetUserByEmail(string email, string password);
        Task<User> GetUserById(int userId);
        Task<List<LeaveType>> GetLeaveTypes();
        Task<string> ApplyLeave(int userId, int leaveTypeId, int managerId, DateTime fromDate, DateTime toDate, string reason);
        Task<List<MyLeaveRequest>> GetMyRequests(int userId);
        Task<bool> CancelLeave(int requestId, int userId);
        Task<List<PendingRequest>> GetPendingRequests(int managerId);
        Task<string> UpdateLeaveStatus(int requestId, string status, int managerId, string remarks);
        Task<object> GetEmployeeDashboard(int userId);
        Task<object> GetManagerDashboard(int managerId);
        Task<object> GetLeaveHistory(int managerId);

        Task<List<User>> GetManagers();
        Task<object> GetAllRequests();
        Task<bool> AddUser(User user);
        Task<object> GetHRDashboard();

        Task<List<LeaveSummaryResult>> GetLeaveSummaryAsync();
        Task ExpireLeaves();

    }
}

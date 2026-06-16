using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_LeaveManagementSystem.Model
{
    [Table("LeaveRequests")]
    public class LeaveRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int LeaveTypeId { get; set; }

        [Required]
        public int ManagerId { get; set; }

        [Required]
        public DateOnly FromDate { get; set; }

        [Required]
        public DateOnly ToDate { get; set; }

        [Required]
        public int TotalDays { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public int? ApprovedBy { get; set; }

        [MaxLength(500)]
        public string? RemarksFromManager { get; set; }

        public DateTime AppliedOn { get; set; } = DateTime.Now;

        public DateTime? ManagerActionDate { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [ForeignKey("LeaveTypeId")]
        public LeaveType LeaveType { get; set; }

        [ForeignKey("ManagerId")]
        public User? AssignedManager { get; set; }

        [ForeignKey("ApprovedBy")]
        public User? Manager { get; set; }
    }


    // maps result from sp_GetMyRequests
    public class MyLeaveRequest
    {
        public int RequestId { get; set; }
        public string LeaveTypeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime AppliedOn { get; set; }
        public string? RemarksFromManager { get; set; }
        public DateTime? ManagerActionDate { get; set; }
    }

    // maps result from sp_GetPendingRequests
    public class PendingRequest
    {
        public int RequestId { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveTypeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public DateTime AppliedOn { get; set; }
        public string Status { get; set; }
    }




    // maps sp_GetLeaveHistory result
    public class LeaveHistoryResult
    {
        public int RequestId { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveTypeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string? RemarksFromManager { get; set; }
        public DateTime? ManagerActionDate { get; set; }
    }

    // maps sp_GetAllRequests result
    public class AllRequestResult
    {
        public int RequestId { get; set; }
        public string EmployeeName { get; set; }
        public string ManagerName { get; set; }
        public string LeaveTypeName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int TotalDays { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime AppliedOn { get; set; }
        public string? RemarksFromManager { get; set; }
        public DateTime? ManagerActionDate { get; set; }
    }



    // maps result from sp_GetLeaveSummary
    public class LeaveSummaryResult
    {
        public int UserId { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public int AvailableLeaveDays { get; set; }
        public int TotalRequests { get; set; }
        public int ApprovedDays { get; set; }
        public int PendingCount { get; set; }
        public int RejectedCount { get; set; }
        public int CancelledCount { get; set; }
    }
}

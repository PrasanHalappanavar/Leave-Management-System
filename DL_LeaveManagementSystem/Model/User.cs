using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_LeaveManagementSystem.Model
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int Role { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int AvailableLeaveDays { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("DepartmentId")]
        public Department Department { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_LeaveManagementSystem.Model
{
    [Table("LeaveTypes")]
    public class LeaveType
    {
        [Key]
        public int LeaveTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; }

        [Required]
        public int MaxDaysAllowed { get; set; }

        [Required]
        public bool IsPaid { get; set; }
    }
}

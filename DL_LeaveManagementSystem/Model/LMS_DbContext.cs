using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_LeaveManagementSystem.Model
{
    public class LMS_DbContext : DbContext
    {
        public LMS_DbContext(DbContextOptions<LMS_DbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<LeaveType> LeaveTypes { get; set; }

        public DbSet<LeaveRequest> LeaveRequests { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UserId → no cascade because multiple FK paths exist
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // ManagerId → no cascade
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(r => r.AssignedManager)
                .WithMany()
                .HasForeignKey(r => r.ManagerId)
                .OnDelete(DeleteBehavior.NoAction);

            // ApprovedBy → no cascade
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(r => r.Manager)
                .WithMany()
                .HasForeignKey(r => r.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

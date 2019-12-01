using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VollyV3.Data;
using VollyV3.GlobalConstants;
using VollyV3.Models.Users;

namespace VollyV3.Models
{
    public class ApplicationDbContext : IdentityDbContext<VollyV3User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationAdministratorUser> OrganizationAdministratorUsers { get; set; }
        public DbSet<VolunteerUser> VolunteerUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cause> Causes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<OpportunityImage> OpportunityImages { get; set; }
        public DbSet<Occurrence> Occurrences { get; set; }
        public DbSet<ApplicationOccurrence> ApplicationsOccurrence { get; set; }
        //public DbSet<Company> Companies { get; set; }
        //public DbSet<VolunteerHours> VolunteerHours { get; set; }
        public DbSet<Community> Communities { get; set; }
        //public DbSet<UserCause> UserCauses { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationOccurrence>()
                .HasKey(k => new { k.ApplicationId, k.OccurrenceId });
            builder.Entity<ApplicationOccurrence>()
                .HasOne(ao => ao.Application)
                .WithMany(ao => ao.Occurrences)
                .HasForeignKey(ao => ao.ApplicationId);
            builder.Entity<ApplicationOccurrence>()
                .HasOne(ao => ao.Occurrence)
                .WithMany(ao => ao.Applications)
                .HasForeignKey(ao => ao.OccurrenceId);
            //builder.Entity<Company>()
            //    .HasIndex(c => c.CompanyCode)
            //    .IsUnique();
            //builder.Entity<UserCause>()
            //   .HasKey(u => new { u.UserId, u.CauseId });
            //builder.Entity<UserCause>()
            //    .HasOne(uc => uc.User)
            //    .WithMany(uc => uc.Causes)
            //    .HasForeignKey(uc => uc.UserId);
            //builder.Entity<UserCause>()
            //    .HasOne(uc => uc.Cause)
            //    .WithMany(uc => uc.Users)
            //    .HasForeignKey(uc => uc.CauseId);
            builder.Entity<OrganizationAdministratorUser>()
                .HasKey(u => u.UserId);
            builder.Entity<VolunteerUser>()
                .HasKey(x => x.UserId);

            builder.Entity<Organization>()
                .HasKey(o => o.Id);
        }
    }
}

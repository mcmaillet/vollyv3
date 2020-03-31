using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VollyV3.Data;
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
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cause> Causes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<OpportunityImage> OpportunityImages { get; set; }
        public DbSet<Occurrence> Occurrences { get; set; }
        public DbSet<OccurrenceApplication> OccurrenceApplications { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OccurrenceApplication>()
                .HasKey(k => new { k.OccurrenceId, k.ApplicationId });
            builder.Entity<OccurrenceApplication>()
                .HasOne(ao => ao.Application)
                .WithMany(ao => ao.Occurrences)
                .HasForeignKey(ao => ao.ApplicationId);
            builder.Entity<OccurrenceApplication>()
                .HasOne(ao => ao.Occurrence)
                .WithMany(ao => ao.Applications)
                .HasForeignKey(ao => ao.OccurrenceId);

            builder.Entity<OrganizationAdministratorUser>()
                .HasKey(x => new { x.UserId, x.OrganizationId });

            builder.Entity<Opportunity>()
                .HasOne(x => x.CreatedBy)
                .WithMany(x => x.Opportunities)
                .HasForeignKey(x => new { x.CreatedByUserId, x.CreatedByOrganizationId });

            builder.Entity<OrganizationAdministratorUser>()
                .HasMany(x => x.Opportunities)
                .WithOne(x => x.CreatedBy)
                .HasForeignKey(x => new { x.CreatedByUserId, x.CreatedByOrganizationId });
        }
    }
}

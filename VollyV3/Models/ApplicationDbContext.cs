﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        //public DbSet<Company> Companies { get; set; }
        //public DbSet<VolunteerHours> VolunteerHours { get; set; }
        //public DbSet<UserCause> UserCauses { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<OccurrenceApplication>()
                .HasKey(k => new {  k.OccurrenceId, k.ApplicationId });
            builder.Entity<OccurrenceApplication>()
                .HasOne(ao => ao.Application)
                .WithMany(ao => ao.Occurrences)
                .HasForeignKey(ao => ao.ApplicationId);
            builder.Entity<OccurrenceApplication>()
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
                .HasKey(x => new { x.UserId, x.OrganizationId });

            builder.Entity<Organization>()
                .HasKey(o => o.Id);
        }
    }
}

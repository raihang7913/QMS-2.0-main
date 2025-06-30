using Microsoft.EntityFrameworkCore;
using OJT_WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace QMS.Datas
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) :base(options)
        { }
        public DbSet<Models.DOCREQ> DOCREQS { get; set; }
        public DbSet<Models.DOCREQUSER> DOCREQUSERs { get; set; }
        public DbSet<EMPL> EMPL { get; set; }
        public DbSet<Models.EngineeringPortal_CCR> EngineeringPortal_CCRs { get; set; }
        public DbSet<Models.EngineeringPortal_CCRP1Drawing> EngineeringPortal_CCRP1Drawings { get; set; }
        public DbSet<Models.EngineeringPortal_CCRP1Standard> EngineeringPortal_CCRP1Standards { get; set; }
        public DbSet<Models.EngineeringPortal_CCRP2GAP> EngineeringPortal_CCRP2GAPs { get; set; }
        public DbSet<Models.EngineeringPortal_CCRP3GAP> EngineeringPortal_CCRP3GAPs { get; set; }
        public DbSet<Models.EngineeringPortal_CCRP3Review> EngineeringPortal_CCRP3Reviews { get; set; }
        public DbSet<Models.EngineeringPortal_CCRP3SpesificationImpact> EngineeringPortal_CCRP3SpesificationImpacts { get; set; }
        public DbSet<Models.EngineeringPortal_CCRP4Verification> EngineeringPortal_CCRP4Verifications { get; set; }
        public DbSet<Models.EngineeringPortal_CCRP4VerificationFooter> EngineeringPortal_CCRP4VerificationFooters { get; set; }
    }
}

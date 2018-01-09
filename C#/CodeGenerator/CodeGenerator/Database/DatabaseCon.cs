namespace CodeGenerator.Database
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DatabaseCon : DbContext
    {
        public DatabaseCon()
            : base("name=Database")
        {
        }

        public virtual DbSet<application> applications { get; set; }
        public virtual DbSet<device_type> device_type { get; set; }
        public virtual DbSet<error_code> error_code { get; set; }
        public virtual DbSet<language> languages { get; set; }
        public virtual DbSet<severity> severities { get; set; }
        public virtual DbSet<version> versions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<application>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<device_type>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<error_code>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<language>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<severity>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<severity>()
                .HasMany(e => e.error_code)
                .WithRequired(e => e.severity)
                .HasForeignKey(e => e.default_severity_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<version>()
                .Property(e => e.timestamp)
                .IsFixedLength();
        }
    }
}

namespace EFLayer.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class RadioPlayer : DbContext
    {
        public RadioPlayer()
            : base("name=RadioPlayer")
        {
        }

        public virtual DbSet<tbl_collection> tbl_collection { get; set; }
        public virtual DbSet<tbl_collectionmap> tbl_collectionmap { get; set; }
        public virtual DbSet<tbl_format> tbl_format { get; set; }
        public virtual DbSet<tbl_genre> tbl_genre { get; set; }
        public virtual DbSet<tbl_genremap> tbl_genremap { get; set; }
        public virtual DbSet<tbl_mood> tbl_mood { get; set; }
        public virtual DbSet<tbl_moodmap> tbl_moodmap { get; set; }
        public virtual DbSet<tbl_play_history> tbl_play_history { get; set; }
        public virtual DbSet<tbl_playlist_header> tbl_playlist_header { get; set; }
        public virtual DbSet<tbl_playlist_lines> tbl_playlist_lines { get; set; }
        public virtual DbSet<tbl_track> tbl_track { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_collection>()
                .HasMany(e => e.tbl_collectionmap)
                .WithRequired(e => e.tbl_collection)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbl_format>()
                .HasMany(e => e.tbl_track)
                .WithRequired(e => e.tbl_format)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbl_genre>()
                .Property(e => e.description)
                .IsUnicode(false);

            modelBuilder.Entity<tbl_genre>()
                .HasMany(e => e.tbl_genremap)
                .WithRequired(e => e.tbl_genre)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbl_mood>()
                .HasMany(e => e.tbl_moodmap)
                .WithRequired(e => e.tbl_mood)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbl_playlist_header>()
                .HasMany(e => e.tbl_playlist_lines)
                .WithRequired(e => e.tbl_playlist_header)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbl_track>()
                .HasMany(e => e.tbl_collectionmap)
                .WithRequired(e => e.tbl_track)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbl_track>()
                .HasMany(e => e.tbl_genremap)
                .WithRequired(e => e.tbl_track)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbl_track>()
                .HasMany(e => e.tbl_moodmap)
                .WithRequired(e => e.tbl_track)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbl_track>()
                .HasMany(e => e.tbl_play_history)
                .WithRequired(e => e.tbl_track)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tbl_track>()
                .HasMany(e => e.tbl_playlist_lines)
                .WithRequired(e => e.tbl_track)
                .WillCascadeOnDelete(false);
        }
    }
}

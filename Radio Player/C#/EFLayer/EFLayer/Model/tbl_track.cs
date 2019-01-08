namespace EFLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_track
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_track()
        {
            tbl_collectionmap = new HashSet<tbl_collectionmap>();
            tbl_genremap = new HashSet<tbl_genremap>();
            tbl_moodmap = new HashSet<tbl_moodmap>();
            tbl_play_history = new HashSet<tbl_play_history>();
            tbl_playlist_lines = new HashSet<tbl_playlist_lines>();
        }

        [Key]
        public int track_id { get; set; }

        [Required]
        [StringLength(50)]
        public string title { get; set; }

        [StringLength(50)]
        public string subtitle { get; set; }

        [StringLength(256)]
        public string artists { get; set; }

        [StringLength(50)]
        public string bitrate { get; set; }

        [StringLength(50)]
        public string publisher { get; set; }

        public bool parental_advisory { get; set; }

        [Required]
        [StringLength(256)]
        public string folder_path { get; set; }

        [Required]
        [StringLength(50)]
        public string file_name { get; set; }

        [StringLength(256)]
        public string composers { get; set; }

        public int duration { get; set; }

        public int format_id { get; set; }

        public long filesize { get; set; }

        public DateTime? date_recorded { get; set; }

        public DateTime? date_released { get; set; }

        public int likes { get; set; }

        public int dislikes { get; set; }

        [StringLength(256)]
        public string keywords { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_collectionmap> tbl_collectionmap { get; set; }

        public virtual tbl_format tbl_format { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_genremap> tbl_genremap { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_moodmap> tbl_moodmap { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_play_history> tbl_play_history { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_playlist_lines> tbl_playlist_lines { get; set; }
    }
}

namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class bbc_rss_wind_direction
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public bbc_rss_wind_direction()
        {
            rss_feed_result = new HashSet<rss_feed_result>();
        }

        [Key]
        public int bbc_rss_wind_direction_id { get; set; }

        [Required]
        [StringLength(50)]
        public string description { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public bool is_active { get; set; }

        public int degrees { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rss_feed_result> rss_feed_result { get; set; }
    }
}

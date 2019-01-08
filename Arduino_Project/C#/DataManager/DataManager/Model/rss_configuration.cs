namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class rss_configuration
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public rss_configuration()
        {
            rss_feed_result = new HashSet<rss_feed_result>();
        }

        [Key]
        public int rss_configuration_id { get; set; }

        [Required]
        [StringLength(50)]
        public string description { get; set; }

        public int version { get; set; }

        public bool is_active { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public DateTime last_update { get; set; }

        public double update_frequency { get; set; }

        public int update_period_id { get; set; }

        public int device_id { get; set; }

        [Required]
        [StringLength(128)]
        public string rss_url { get; set; }

        public int? rss_configuration_type_id { get; set; }

        public virtual device device { get; set; }

        public virtual rss_configuration_type rss_configuration_type { get; set; }

        public virtual update_period update_period { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rss_feed_result> rss_feed_result { get; set; }
    }
}

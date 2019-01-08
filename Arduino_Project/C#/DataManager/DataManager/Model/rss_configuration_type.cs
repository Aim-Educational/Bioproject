namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class rss_configuration_type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public rss_configuration_type()
        {
            rss_configuration = new HashSet<rss_configuration>();
        }

        [Key]
        public int rss_configuration_type_id { get; set; }

        [Required]
        [StringLength(50)]
        public string description { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public int version { get; set; }

        public bool is_active { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rss_configuration> rss_configuration { get; set; }
    }
}

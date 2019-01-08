namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("unit")]
    public partial class unit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public unit()
        {
            device_type = new HashSet<device_type>();
        }

        [Key]
        public int unit_id { get; set; }

        [Required]
        [StringLength(50)]
        public string description { get; set; }

        [Required]
        [StringLength(50)]
        public string abbreviation { get; set; }

        public bool is_active { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<device_type> device_type { get; set; }
    }
}

namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class device_type
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public device_type()
        {
            devices = new HashSet<device>();
        }

        [Key]
        public int device_type_id { get; set; }

        public int unit_id { get; set; }

        [Required]
        [StringLength(50)]
        public string description { get; set; }

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
        public virtual ICollection<device> devices { get; set; }

        public virtual unit unit { get; set; }
    }
}

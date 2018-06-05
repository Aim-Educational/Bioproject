namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class device_address
    {
        [Key]
        public int device_address_id { get; set; }

        public int device_address_type_id { get; set; }

        public int device_id { get; set; }

        public int? pin_number { get; set; }

        [StringLength(50)]
        public string ip_address { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public bool is_active { get; set; }

        public virtual device device { get; set; }

        public virtual device_address_type device_address_type { get; set; }
    }
}

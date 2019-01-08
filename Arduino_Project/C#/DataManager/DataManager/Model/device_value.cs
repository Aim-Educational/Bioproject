namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class device_value
    {
        [Key]
        public int device_value_id { get; set; }

        public int device_id { get; set; }

        public double value { get; set; }

        public DateTime datetime { get; set; }

        public bool response_recieved { get; set; }

        public bool is_active { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        [Required]
        [StringLength(255)]
        public string extra_data { get; set; }

        public virtual device device { get; set; }
    }
}

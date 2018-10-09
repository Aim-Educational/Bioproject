namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("alarm")]
    public partial class alarm
    {
        [Key]
        public int alarm_id { get; set; }

        public int alarm_type_id { get; set; }

        public double value { get; set; }

        public int group_type_id { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public int version { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public bool is_active { get; set; }

        public int device_id { get; set; }

        public virtual alarm_type alarm_type { get; set; }

        public virtual device device { get; set; }

        public virtual group_type group_type { get; set; }
    }
}

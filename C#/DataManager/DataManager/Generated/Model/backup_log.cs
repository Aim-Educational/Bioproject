namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class backup_log
    {
        [Key]
        public int backup_log_id { get; set; }

        [Required]
        [StringLength(255)]
        public string filename { get; set; }

        public DateTime datetime { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public bool is_active { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public int version { get; set; }
    }
}

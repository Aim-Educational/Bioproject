namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class database_config
    {
        [Key]
        public int database_config_id { get; set; }

        [Required]
        [StringLength(255)]
        public string database_backup_directory { get; set; }

        public int version { get; set; }
        public bool is_active { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }
    }
}

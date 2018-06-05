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
        public int id { get; set; }

        [Required]
        [StringLength(255)]
        public string database_backup_directory { get; set; }
    }
}

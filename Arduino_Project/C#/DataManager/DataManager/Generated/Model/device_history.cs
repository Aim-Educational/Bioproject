namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class device_history
    {
        [Key]
        public int device_history_id { get; set; }

        public int device_id { get; set; }

        public int device_history_action_id { get; set; }

        public int supplier_id { get; set; }

        public DateTime datetime { get; set; }

        public bool is_active { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public virtual device device { get; set; }

        public virtual device_history_action device_history_action { get; set; }

        public virtual supplier supplier { get; set; }
    }
}

namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class application_log
    {
        [Key]
        public int application_log_id { get; set; }

        public int message_type_id { get; set; }

        [Required]
        [StringLength(512)]
        public string message { get; set; }

        public DateTime datetime { get; set; }

        public int application_id { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public bool is_active { get; set; }

        public virtual application application { get; set; }

        public virtual message_type message_type { get; set; }
    }
}

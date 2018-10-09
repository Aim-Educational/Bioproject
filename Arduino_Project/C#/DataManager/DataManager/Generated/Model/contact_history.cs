namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class contact_history
    {
        [Key]
        public int contact_history_id { get; set; }

        public int history_event_id { get; set; }

        public int contact_id { get; set; }

        public DateTime date_and_time { get; set; }

        public bool is_active { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public virtual contact contact { get; set; }

        public virtual history_event history_event { get; set; }
    }
}

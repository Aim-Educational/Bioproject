namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class rss_error
    {
        [Key]
        public int rss_error_id { get; set; }

        public DateTime date_and_time { get; set; }

        [Required]
        public string data { get; set; }

        [Required]
        [StringLength(256)]
        public string message { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public bool is_active { get; set; }
    }
}

namespace CodeGenerator.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class error_code
    {
        [Key]
        public int error_code_id { get; set; }

        [Column("error_code")]
        [Required]
        [StringLength(50)]
        public string error_code1 { get; set; }

        [Required]
        [StringLength(50)]
        public string error_code_mneumonic { get; set; }

        [Required]
        [StringLength(255)]
        public string narrative { get; set; }

        public byte application_ids { get; set; }

        public byte device_ids { get; set; }

        public int default_severity_id { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public virtual severity severity { get; set; }
    }
}

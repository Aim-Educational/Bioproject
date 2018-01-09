namespace CodeGenerator.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("version")]
    public partial class version
    {
        [Key]
        public int version_id { get; set; }

        public DateTime date_generated { get; set; }

        [Required]
        [StringLength(50)]
        public string version_string { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }
    }
}

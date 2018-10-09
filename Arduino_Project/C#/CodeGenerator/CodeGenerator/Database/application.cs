namespace CodeGenerator.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("application")]
    public partial class application
    {
        [Key]
        public int application_id { get; set; }

        [Required]
        [StringLength(50)]
        public string description { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public byte bit_index { get; set; }

        [Required]
        [StringLength(255)]
        public string path_to_output_file { get; set; }
    }
}

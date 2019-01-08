namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class group_action
    {
        [Key]
        public int group_action_id { get; set; }

        public int action_type_id { get; set; }

        public int group_type_id { get; set; }

        public bool is_active { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public virtual action_type action_type { get; set; }

        public virtual group_type group_type { get; set; }
    }
}

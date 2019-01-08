namespace EFLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_device_config
    {
        [Key]
        public int device_config_id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(256)]
        public string path_to_root { get; set; }

        [Required]
        [StringLength(50)]
        public string network_user { get; set; }

        [Required]
        [StringLength(50)]
        public string network_pass { get; set; }
    }
}

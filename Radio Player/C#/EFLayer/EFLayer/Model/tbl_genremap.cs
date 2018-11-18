namespace EFLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_genremap
    {
        [Key]
        public int genremap_id { get; set; }

        public int genre_id { get; set; }

        public int track_id { get; set; }

        public virtual tbl_genre tbl_genre { get; set; }

        public virtual tbl_track tbl_track { get; set; }
    }
}

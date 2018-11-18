namespace EFLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_moodmap
    {
        [Key]
        public int moodmap_id { get; set; }

        public int track_id { get; set; }

        public int mood_id { get; set; }

        public virtual tbl_mood tbl_mood { get; set; }

        public virtual tbl_track tbl_track { get; set; }
    }
}

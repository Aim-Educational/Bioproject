namespace EFLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_play_history
    {
        [Key]
        public int play_history_id { get; set; }

        public int track_id { get; set; }

        public DateTime date_played { get; set; }

        public virtual tbl_track tbl_track { get; set; }
    }
}

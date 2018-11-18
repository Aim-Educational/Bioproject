namespace EFLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_playlist_lines
    {
        [Key]
        public int playlist_lines_id { get; set; }

        public int playlist_header_id { get; set; }

        public int track_id { get; set; }

        public int sequence_index { get; set; }

        public virtual tbl_playlist_header tbl_playlist_header { get; set; }

        public virtual tbl_track tbl_track { get; set; }
    }
}

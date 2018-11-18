namespace EFLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_collectionmap
    {
        [Key]
        public int collectionmap_id { get; set; }

        public int sequence_index { get; set; }

        public int track_id { get; set; }

        public int collection_id { get; set; }

        public virtual tbl_collection tbl_collection { get; set; }

        public virtual tbl_track tbl_track { get; set; }
    }
}

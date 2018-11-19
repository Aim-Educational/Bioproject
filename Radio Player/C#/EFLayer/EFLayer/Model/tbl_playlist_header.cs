namespace EFLayer.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_playlist_header
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_playlist_header()
        {
            tbl_playlist_lines = new HashSet<tbl_playlist_lines>();
        }

        [Key]
        public int playlist_header_id { get; set; }

        [Required]
        [StringLength(50)]
        public string description { get; set; }

        public int likes { get; set; }

        public int dislikes { get; set; }

        [Required]
        [StringLength(50)]
        public string creator { get; set; }

        public bool play_random { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_playlist_lines> tbl_playlist_lines { get; set; }
    }
}

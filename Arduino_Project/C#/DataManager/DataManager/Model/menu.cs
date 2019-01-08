namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("menu")]
    public partial class menu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public menu()
        {
            menu1 = new HashSet<menu>();
        }

        [Key]
        public int menu_id { get; set; }

        public int parent_menu_id { get; set; }

        [Required]
        [StringLength(50)]
        public string description { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public int version { get; set; }

        public bool is_active { get; set; }

        [Required]
        [StringLength(150)]
        public string search_provider { get; set; }

        [Required]
        [StringLength(150)]
        public string editor { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<menu> menu1 { get; set; }

        public virtual menu menu2 { get; set; }
    }
}

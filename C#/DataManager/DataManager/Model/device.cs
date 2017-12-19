namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("device")]
    public partial class device
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public device()
        {
            action_level = new HashSet<action_level>();
            device1 = new HashSet<device>();
            device_history_action = new HashSet<device_history_action>();
            device_history = new HashSet<device_history>();
            device_url = new HashSet<device_url>();
            device_value = new HashSet<device_value>();
            rss_configuration = new HashSet<rss_configuration>();
        }

        [Key]
        public int device_id { get; set; }

        [Required]
        [StringLength(50)]
        public string name { get; set; }

        [Required]
        [StringLength(50)]
        public string description { get; set; }

        public int device_type_id { get; set; }

        [Required]
        [StringLength(50)]
        public string location { get; set; }

        public double min_value { get; set; }

        public double max_value { get; set; }

        public double accuracy { get; set; }

        [Required]
        [StringLength(50)]
        public string serial_number { get; set; }

        [Column(TypeName = "money")]
        public decimal cost { get; set; }

        public int reliability { get; set; }

        public int strikes { get; set; }

        public bool is_active { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public int parent_device_id { get; set; }

        public bool is_allowed_for_use { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<action_level> action_level { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<device> device1 { get; set; }

        public virtual device device2 { get; set; }

        public virtual device_type device_type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<device_history_action> device_history_action { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<device_history> device_history { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<device_url> device_url { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<device_value> device_value { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<rss_configuration> rss_configuration { get; set; }
    }
}

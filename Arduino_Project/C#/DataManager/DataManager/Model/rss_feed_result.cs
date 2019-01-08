namespace DataManager.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class rss_feed_result
    {
        [Key]
        public int rss_feed_result_id { get; set; }

        public int source_id { get; set; }

        public DateTime date_and_time_request { get; set; }

        [Required]
        public string raw_data { get; set; }

        public int wind_direction_id { get; set; }

        public int visibility_id { get; set; }

        public double wind_speed { get; set; }

        public double? observed_temperature { get; set; }

        public int pressure_value { get; set; }

        public int pressure_change_id { get; set; }

        public int humidity { get; set; }

        public int version { get; set; }

        [Column(TypeName = "timestamp")]
        [MaxLength(8)]
        [Timestamp]
        public byte[] timestamp { get; set; }

        public bool is_active { get; set; }

        [Required]
        [StringLength(50)]
        public string comment { get; set; }

        public DateTime date_and_time_data { get; set; }

        public double? forecast_min_temperature { get; set; }

        public double? forecast_max_temperature { get; set; }

        public int forecast_uv_risk { get; set; }

        public int forecast_pollution_id { get; set; }

        public TimeSpan forecast_sunrise_time { get; set; }

        public TimeSpan forecast_sunset_time { get; set; }

        public int cloud_coverage_id { get; set; }

        public virtual bbc_rss_barometric_change bbc_rss_barometric_change { get; set; }

        public virtual bbc_rss_cloud_coverage bbc_rss_cloud_coverage { get; set; }

        public virtual bbc_rss_pollution bbc_rss_pollution { get; set; }

        public virtual bbc_rss_visibility bbc_rss_visibility { get; set; }

        public virtual bbc_rss_wind_direction bbc_rss_wind_direction { get; set; }

        public virtual rss_configuration rss_configuration { get; set; }
    }
}

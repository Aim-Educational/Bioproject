namespace DataManager.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PlanningContext : DbContext
    {
        public PlanningContext()
            : base("name=planning")
        {
        }

        public virtual DbSet<action_level> action_level { get; set; }
        public virtual DbSet<action_type> action_type { get; set; }
        public virtual DbSet<alarm> alarms { get; set; }
        public virtual DbSet<alarm_type> alarm_type { get; set; }
        public virtual DbSet<application> applications { get; set; }
        public virtual DbSet<application_log> application_log { get; set; }
        public virtual DbSet<backup_log> backup_log { get; set; }
        public virtual DbSet<bbc_rss_barometric_change> bbc_rss_barometric_change { get; set; }
        public virtual DbSet<bbc_rss_cloud_coverage> bbc_rss_cloud_coverage { get; set; }
        public virtual DbSet<bbc_rss_pollution> bbc_rss_pollution { get; set; }
        public virtual DbSet<bbc_rss_visibility> bbc_rss_visibility { get; set; }
        public virtual DbSet<bbc_rss_wind_direction> bbc_rss_wind_direction { get; set; }
        public virtual DbSet<contact> contacts { get; set; }
        public virtual DbSet<contact_email> contact_email { get; set; }
        public virtual DbSet<contact_history> contact_history { get; set; }
        public virtual DbSet<contact_telephone> contact_telephone { get; set; }
        public virtual DbSet<contact_type> contact_type { get; set; }
        public virtual DbSet<database_config> database_config { get; set; }
        public virtual DbSet<device> devices { get; set; }
        public virtual DbSet<device_address> device_address { get; set; }
        public virtual DbSet<device_address_type> device_address_type { get; set; }
        public virtual DbSet<device_history> device_history { get; set; }
        public virtual DbSet<device_history_action> device_history_action { get; set; }
        public virtual DbSet<device_type> device_type { get; set; }
        public virtual DbSet<device_url> device_url { get; set; }
        public virtual DbSet<device_value> device_value { get; set; }
        public virtual DbSet<group_action> group_action { get; set; }
        public virtual DbSet<group_member> group_member { get; set; }
        public virtual DbSet<group_type> group_type { get; set; }
        public virtual DbSet<history_event> history_event { get; set; }
        public virtual DbSet<menu> menus { get; set; }
        public virtual DbSet<message_type> message_type { get; set; }
        public virtual DbSet<rss_configuration> rss_configuration { get; set; }
        public virtual DbSet<rss_configuration_type> rss_configuration_type { get; set; }
        public virtual DbSet<rss_error> rss_error { get; set; }
        public virtual DbSet<rss_feed_result> rss_feed_result { get; set; }
        public virtual DbSet<supplier> suppliers { get; set; }
        public virtual DbSet<unit> units { get; set; }
        public virtual DbSet<update_period> update_period { get; set; }
        public virtual DbSet<user> users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<action_level>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<action_type>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<action_type>()
                .HasMany(e => e.action_level)
                .WithRequired(e => e.action_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<action_type>()
                .HasMany(e => e.group_action)
                .WithRequired(e => e.action_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<alarm>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<alarm_type>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<alarm_type>()
                .HasMany(e => e.alarms)
                .WithRequired(e => e.alarm_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<application>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<application>()
                .HasMany(e => e.application_log)
                .WithRequired(e => e.application)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<application_log>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<backup_log>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<bbc_rss_barometric_change>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<bbc_rss_barometric_change>()
                .HasMany(e => e.rss_feed_result)
                .WithRequired(e => e.bbc_rss_barometric_change)
                .HasForeignKey(e => e.pressure_change_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<bbc_rss_cloud_coverage>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<bbc_rss_cloud_coverage>()
                .HasMany(e => e.rss_feed_result)
                .WithRequired(e => e.bbc_rss_cloud_coverage)
                .HasForeignKey(e => e.cloud_coverage_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<bbc_rss_pollution>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<bbc_rss_pollution>()
                .HasMany(e => e.rss_feed_result)
                .WithRequired(e => e.bbc_rss_pollution)
                .HasForeignKey(e => e.forecast_pollution_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<bbc_rss_visibility>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<bbc_rss_visibility>()
                .HasMany(e => e.rss_feed_result)
                .WithRequired(e => e.bbc_rss_visibility)
                .HasForeignKey(e => e.visibility_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<bbc_rss_wind_direction>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<bbc_rss_wind_direction>()
                .HasMany(e => e.rss_feed_result)
                .WithRequired(e => e.bbc_rss_wind_direction)
                .HasForeignKey(e => e.wind_direction_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<contact>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<contact>()
                .HasMany(e => e.contact_email)
                .WithRequired(e => e.contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<contact>()
                .HasMany(e => e.contact_history)
                .WithRequired(e => e.contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<contact>()
                .HasMany(e => e.contact_telephone)
                .WithRequired(e => e.contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<contact>()
                .HasMany(e => e.group_member)
                .WithRequired(e => e.contact)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<contact_email>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<contact_history>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<contact_telephone>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<contact_type>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<contact_type>()
                .HasMany(e => e.contacts)
                .WithRequired(e => e.contact_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<database_config>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<device>()
                .Property(e => e.cost)
                .HasPrecision(19, 4);

            modelBuilder.Entity<device>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<device>()
                .HasMany(e => e.action_level)
                .WithRequired(e => e.device)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device>()
                .HasMany(e => e.alarms)
                .WithRequired(e => e.device)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device>()
                .HasMany(e => e.device_address)
                .WithRequired(e => e.device)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device>()
                .HasMany(e => e.device1)
                .WithOptional(e => e.device2)
                .HasForeignKey(e => e.parent_device_id);

            modelBuilder.Entity<device>()
                .HasMany(e => e.device_history_action)
                .WithRequired(e => e.device)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device>()
                .HasMany(e => e.device_history)
                .WithRequired(e => e.device)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device>()
                .HasMany(e => e.device_url)
                .WithRequired(e => e.device)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device>()
                .HasMany(e => e.device_value)
                .WithRequired(e => e.device)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device>()
                .HasMany(e => e.rss_configuration)
                .WithRequired(e => e.device)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device_address>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<device_address_type>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<device_address_type>()
                .HasMany(e => e.device_address)
                .WithRequired(e => e.device_address_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device_history>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<device_history_action>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<device_history_action>()
                .HasMany(e => e.device_history)
                .WithRequired(e => e.device_history_action)
                .HasForeignKey(e => e.device_history_action_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device_type>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<device_type>()
                .HasMany(e => e.devices)
                .WithRequired(e => e.device_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<device_url>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<device_value>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<group_action>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<group_member>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<group_type>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<group_type>()
                .HasMany(e => e.alarms)
                .WithRequired(e => e.group_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<group_type>()
                .HasMany(e => e.group_action)
                .WithRequired(e => e.group_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<group_type>()
                .HasMany(e => e.group_member)
                .WithRequired(e => e.group_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<group_type>()
                .HasMany(e => e.group_type1)
                .WithRequired(e => e.group_type2)
                .HasForeignKey(e => e.parent_group_type_id);

            modelBuilder.Entity<history_event>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<history_event>()
                .HasMany(e => e.contact_history)
                .WithRequired(e => e.history_event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<menu>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<menu>()
                .HasMany(e => e.menu1)
                .WithRequired(e => e.menu2)
                .HasForeignKey(e => e.parent_menu_id);

            modelBuilder.Entity<message_type>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<message_type>()
                .HasMany(e => e.application_log)
                .WithRequired(e => e.message_type)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<rss_configuration>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<rss_configuration>()
                .HasMany(e => e.rss_feed_result)
                .WithRequired(e => e.rss_configuration)
                .HasForeignKey(e => e.source_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<rss_configuration_type>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<rss_error>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<rss_feed_result>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<supplier>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<supplier>()
                .HasMany(e => e.contacts)
                .WithRequired(e => e.supplier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<supplier>()
                .HasMany(e => e.device_history)
                .WithRequired(e => e.supplier)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<unit>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<unit>()
                .HasMany(e => e.device_type)
                .WithRequired(e => e.unit)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<update_period>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<update_period>()
                .HasMany(e => e.rss_configuration)
                .WithRequired(e => e.update_period)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<user>()
                .Property(e => e.timestamp)
                .IsFixedLength();

            modelBuilder.Entity<user>()
                .HasMany(e => e.contacts)
                .WithRequired(e => e.user)
                .WillCascadeOnDelete(false);
        }
    }
}

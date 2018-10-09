using System;
using System.Linq;
using System.Windows.Forms;
using DataManager.Model;

namespace DataUserInterface.Forms
{
    public enum EnumSearchFormType
    {
        ActionLevel,
        ActionType,
        Alarm,
        AlarmType,
        Application,
        ApplicationLog,
        BackupLog,
        BBCRSSBarometricChange,
        BBCRSSGeneral,
        BBCRSSVisibility,
        Contact,
        ContactEmail,
        ContactHistory,
        ContactTelephone,
        ContactType,
        DatabaseConfig,
        Device,
        DeviceAddress,
        DeviceAddressType,
        DeviceHistory,
        DeviceHistoryAction,
        DeviceType,
        DeviceURL,
        DeviceValue,
        GroupAction,
        GroupMember,
        GroupType,
        HistoryEvent,
        MessageType,
        RSSConfiguration,
        Supplier,
        Unit,
        UpdatePeriod,
        User
    }

    public partial class SearchForm : Form
    {
        void populateSearchResultsForType(EnumSearchFormType type)
        {
            using (var db = new PlanningContext())
            {
                switch (type)
                {
                    case EnumSearchFormType.ActionLevel:
                        var ActionLevelQuery = from value in db.action_level
                                               orderby value.comment
                                               select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in ActionLevelQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.action_level_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.action_level_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.ActionType:
                        var ActionTypeQuery = from value in db.action_type
                                              orderby value.description
                                              select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in ActionTypeQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.action_type_id),
                                    value.description
                                }
                            );
                            item.Tag = value.action_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.Alarm:
                        var AlarmQuery = from value in db.alarms
                                         orderby value.comment
                                         select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in AlarmQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.alarm_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.alarm_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.AlarmType:
                        var AlarmTypeQuery = from value in db.alarm_type
                                             orderby value.description
                                             select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in AlarmTypeQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.alarm_type_id),
                                    value.description
                                }
                            );
                            item.Tag = value.alarm_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.Application:
                        var ApplicationQuery = from value in db.applications
                                               orderby value.name
                                               select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Name");
                        foreach (var value in ApplicationQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.application_id),
                                    value.name
                                }
                            );
                            item.Tag = value.application_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.ApplicationLog:
                        var ApplicationLogQuery = from value in db.application_log
                                                  orderby value.message
                                                  select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Message");
                        foreach (var value in ApplicationLogQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.application_log_id),
                                    value.message
                                }
                            );
                            item.Tag = value.application_log_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.BackupLog:
                        var BackupLogQuery = from value in db.backup_log
                                             orderby value.comment
                                             select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in BackupLogQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.backup_log_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.backup_log_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.BBCRSSBarometricChange:
                        var BBCRSSBarometricChangeQuery = from value in db.bbc_rss_barometric_change
                                                          orderby value.description
                                                          select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in BBCRSSBarometricChangeQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.bbc_rss_barometric_change_id),
                                    value.description
                                }
                            );
                            item.Tag = value.bbc_rss_barometric_change_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.BBCRSSGeneral:
                        var BBCRSSGeneralQuery = from value in db.bbc_rss_general
                                                 orderby value.description
                                                 select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in BBCRSSGeneralQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.bbc_rss_general_id),
                                    value.description
                                }
                            );
                            item.Tag = value.bbc_rss_general_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.BBCRSSVisibility:
                        var BBCRSSVisibilityQuery = from value in db.bbc_rss_visibility
                                                    orderby value.description
                                                    select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in BBCRSSVisibilityQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.bbc_rss_visibility_id),
                                    value.description
                                }
                            );
                            item.Tag = value.bbc_rss_visibility_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.Contact:
                        var ContactQuery = from value in db.contacts
                                           orderby value.comment
                                           select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in ContactQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.contact_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.contact_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.ContactEmail:
                        var ContactEmailQuery = from value in db.contact_email
                                                orderby value.comment
                                                select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in ContactEmailQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.contact_email_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.contact_email_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.ContactHistory:
                        var ContactHistoryQuery = from value in db.contact_history
                                                  orderby value.comment
                                                  select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in ContactHistoryQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.contact_history_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.contact_history_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.ContactTelephone:
                        var ContactTelephoneQuery = from value in db.contact_telephone
                                                    orderby value.comment
                                                    select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in ContactTelephoneQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.contact_telephone_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.contact_telephone_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.ContactType:
                        var ContactTypeQuery = from value in db.contact_type
                                               orderby value.description
                                               select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in ContactTypeQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.contact_type_id),
                                    value.description
                                }
                            );
                            item.Tag = value.contact_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.DatabaseConfig:
                        var DatabaseConfigQuery = from value in db.database_config
                                                  orderby value.database_backup_directory
                                                  select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("DatabaseBackupDirectory");
                        foreach (var value in DatabaseConfigQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.database_config_id),
                                    value.database_backup_directory
                                }
                            );
                            item.Tag = value.database_config_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.Device:
                        var DeviceQuery = from value in db.devices
                                          orderby value.name
                                          select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Name");
                        foreach (var value in DeviceQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.device_id),
                                    value.name
                                }
                            );
                            item.Tag = value.device_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.DeviceAddress:
                        var DeviceAddressQuery = from value in db.device_address
                                                 orderby value.comment
                                                 select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in DeviceAddressQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.device_address_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.device_address_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.DeviceAddressType:
                        var DeviceAddressTypeQuery = from value in db.device_address_type
                                                     orderby value.description
                                                     select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in DeviceAddressTypeQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.device_address_type_id),
                                    value.description
                                }
                            );
                            item.Tag = value.device_address_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.DeviceHistory:
                        var DeviceHistoryQuery = from value in db.device_history
                                                 orderby value.comment
                                                 select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in DeviceHistoryQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.device_history_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.device_history_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.DeviceHistoryAction:
                        var DeviceHistoryActionQuery = from value in db.device_history_action
                                                       orderby value.description
                                                       select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in DeviceHistoryActionQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.device_history_action1),
                                    value.description
                                }
                            );
                            item.Tag = value.device_history_action1;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.DeviceType:
                        var DeviceTypeQuery = from value in db.device_type
                                              orderby value.description
                                              select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in DeviceTypeQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.device_type_id),
                                    value.description
                                }
                            );
                            item.Tag = value.device_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.DeviceURL:
                        var DeviceURLQuery = from value in db.device_url
                                             orderby value.comment
                                             select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in DeviceURLQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.device_url_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.device_url_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.DeviceValue:
                        var DeviceValueQuery = from value in db.device_value
                                               orderby value.comment
                                               select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in DeviceValueQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.device_value_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.device_value_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.GroupAction:
                        var GroupActionQuery = from value in db.group_action
                                               orderby value.comment
                                               select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in GroupActionQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.group_action_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.group_action_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.GroupMember:
                        var GroupMemberQuery = from value in db.group_member
                                               orderby value.comment
                                               select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in GroupMemberQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.group_member_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.group_member_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.GroupType:
                        var GroupTypeQuery = from value in db.group_type
                                             orderby value.name
                                             select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Name");
                        foreach (var value in GroupTypeQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.group_type_id),
                                    value.name
                                }
                            );
                            item.Tag = value.group_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.HistoryEvent:
                        var HistoryEventQuery = from value in db.history_event
                                                orderby value.description
                                                select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in HistoryEventQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.history_event_id),
                                    value.description
                                }
                            );
                            item.Tag = value.history_event_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.MessageType:
                        var MessageTypeQuery = from value in db.message_type
                                               orderby value.description
                                               select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in MessageTypeQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.message_type_id),
                                    value.description
                                }
                            );
                            item.Tag = value.message_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.RSSConfiguration:
                        var RSSConfigurationQuery = from value in db.rss_configuration
                                                    orderby value.description
                                                    select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in RSSConfigurationQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.rss_configuration_id),
                                    value.description
                                }
                            );
                            item.Tag = value.rss_configuration_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.Supplier:
                        var SupplierQuery = from value in db.suppliers
                                            orderby value.name
                                            select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Name");
                        foreach (var value in SupplierQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.supplier_id),
                                    value.name
                                }
                            );
                            item.Tag = value.supplier_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.Unit:
                        var UnitQuery = from value in db.units
                                        orderby value.description
                                        select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in UnitQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.unit_id),
                                    value.description
                                }
                            );
                            item.Tag = value.unit_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.UpdatePeriod:
                        var UpdatePeriodQuery = from value in db.update_period
                                                orderby value.description
                                                select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var value in UpdatePeriodQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.update_period_id),
                                    value.description
                                }
                            );
                            item.Tag = value.update_period_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;
                    case EnumSearchFormType.User:
                        var UserQuery = from value in db.users
                                        orderby value.comment
                                        select value;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var value in UserQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(value.user_id),
                                    value.comment
                                }
                            );
                            item.Tag = value.user_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;


                    default:
                        throw new NotImplementedException($"No handler for type: {type}");
                }
            }
        }

        void openEditorByType(EnumSearchFormType type, EnumEditorMode mode, int id = -1)
        {
            Form form;
            switch (type)
            {

                case EnumSearchFormType.ActionLevel:
                    form = new FormActionLevelEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.ActionType:
                    form = new FormActionTypeEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.Alarm:
                    form = new FormAlarmEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.AlarmType:
                    form = new FormAlarmTypeEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.Application:
                    form = new FormApplicationEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.ApplicationLog:
                    form = new FormApplicationLogEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.BackupLog:
                    form = new FormBackupLogEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.BBCRSSBarometricChange:
                    form = new FormBBCRSSBarometricChangeEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.BBCRSSGeneral:
                    form = new FormBBCRSSGeneralEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.BBCRSSVisibility:
                    form = new FormBBCRSSVisibilityEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.Contact:
                    form = new FormContactEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.ContactEmail:
                    form = new FormContactEmailEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.ContactHistory:
                    form = new FormContactHistoryEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.ContactTelephone:
                    form = new FormContactTelephoneEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.ContactType:
                    form = new FormContactTypeEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.DatabaseConfig:
                    form = new FormDatabaseConfigEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.Device:
                    form = new FormDeviceEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.DeviceAddress:
                    form = new FormDeviceAddressEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.DeviceAddressType:
                    form = new FormDeviceAddressTypeEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.DeviceHistory:
                    form = new FormDeviceHistoryEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.DeviceHistoryAction:
                    form = new FormDeviceHistoryActionEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.DeviceType:
                    form = new FormDeviceTypeEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.DeviceURL:
                    form = new FormDeviceURLEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.DeviceValue:
                    form = new FormDeviceValueEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.GroupAction:
                    form = new FormGroupActionEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.GroupMember:
                    form = new FormGroupMemberEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.GroupType:
                    form = new FormGroupTypeEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.HistoryEvent:
                    form = new FormHistoryEventEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.MessageType:
                    form = new FormMessageTypeEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.RSSConfiguration:
                    form = new FormRSSConfigurationEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.Supplier:
                    form = new FormSupplierEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.Unit:
                    form = new FormUnitEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.UpdatePeriod:
                    form = new FormUpdatePeriodEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.User:
                    form = new FormUserEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;


                default:
                    throw new NotImplementedException($"No editor for type: {type}");
            }
        }
    }
}


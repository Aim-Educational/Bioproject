using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DataManager.Model;

namespace DataUserInterface.Forms
{
    public enum EnumSearchFormType
    {
        ActionLevel             = 0,
        ActionType              = 1,
        Alarm                   = 2,
        AlarmType               = 3,
        Application             = 4,
        ApplicationLog          = 5,
        BackupLog               = 6,
        BBCRSSBarometricChange  = 7,
        BBCRSSGeneral           = 8,
        BBCRSSVisibility        = 9,
        Contact                 = 10,
        ContactEmail            = 11,
        ContactHistory          = 12,
        ContactTelephone        = 13,
        ContactType             = 14,
        DatabaseConfig          = 15,
        Device                  = 16,
        DeviceAddress           = 17,
        DeviceAddressType       = 18,
        DeviceHistory           = 19,
        DeviceHistoryAction     = 20,
        DeviceType              = 21,
        DeviceURL               = 22,
        DeviceValue             = 23,
        GroupAction             = 24,
        GroupMember             = 25,
        GroupType               = 26,
        HistoryEvent            = 27,
        MessageType             = 28,
        RSSConfiguration        = 29,
        Supplier                = 30,
        Unit                    = 31,
        UpdatePeriod            = 32,
        User                    = 33
    }

    public partial class SearchForm : Form
    {
        public EnumSearchFormType type { private set; get; }

        public SearchForm(EnumSearchFormType type)
        {
            InitializeComponent();
            this.SetListData(type);
        }

        /// <summary>
        /// Sets the data of the list.
        /// 
        /// Notes:
        ///  This function also clears the list.
        /// </summary>
        /// <param name="dataType">The type of data to add to the list.</param>
        void SetListData(EnumSearchFormType dataType)
        {
            this.type = dataType;

            this.list.Clear();
            using (var db = new PlanningContext())
            {
                switch (dataType)
                {
                    case EnumSearchFormType.ActionLevel:
                        var alQuery = from action in db.action_level
                                    orderby action.comment
                                    select action;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var action in alQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(action.action_level_id),
                                    action.comment
                                }
                            );
                            item.Tag = action.action_level_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.ActionType:
                        var atQuery = from val in db.action_type
                                      orderby val.description
                                      select val;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var val in atQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(val.action_type_id),
                                    val.description
                                }
                            );
                            item.Tag = val.action_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.ApplicationLog:
                        var appLogQuery = from val in db.application_log
                                          orderby val.message_type_id
                                          select val;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Message");
                        foreach (var val in appLogQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(val.application_log_id),
                                    val.message
                                }
                            );
                            item.Tag = val.application_log_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.Alarm:
                        var alarmQuery = from val in db.alarms
                                         orderby val.comment
                                         select val;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Comment");
                        foreach (var val in alarmQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(val.alarm_id),
                                    val.comment
                                }
                            );
                            item.Tag = val.alarm_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.AlarmType:
                        var alarmTQuery = from val in db.action_type
                                         orderby val.description
                                         select val;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var val in alarmTQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(val.action_type_id),
                                    val.description
                                }
                            );
                            item.Tag = val.action_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.Application:
                        var appQuery = from val in db.applications
                                       orderby val.name
                                       select val;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Name");
                        foreach (var val in appQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(val.application_id),
                                    val.name
                                }
                            );
                            item.Tag = val.application_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.BackupLog:
                        var backQuery = from val in db.backup_log
                                        orderby val.filename
                                        select val;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Filename");
                        foreach (var val in backQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(val.backup_log_id),
                                    val.filename
                                }
                            );
                            item.Tag = val.backup_log_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.MessageType:
                        var messageTQuery = from val in db.message_type
                                            orderby val.description
                                            select val;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Description");
                        foreach (var val in messageTQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(val.message_type_id),
                                    val.description
                                }
                            );
                            item.Tag = val.message_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.GroupType:
                        var gtQuery = from val in db.group_type
                                      orderby val.name
                                      select val;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Name");
                        foreach (var val in gtQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(val.group_type_id),
                                    val.name
                                }
                            );
                            item.Tag = val.group_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.Device:
                        var query = from dev in db.devices
                                    orderby dev.name
                                    select dev;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Name");
                        foreach (var dev in query)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(dev.device_id),
                                    dev.name
                                }
                            );
                            item.Tag = dev.device_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.DeviceType:
                        var dtQuery = from type in db.device_type
                                      orderby type.description
                                      select type;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Name");
                        foreach (var type in dtQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(type.device_type_id),
                                    type.description
                                }
                            );
                            item.Tag = type.device_type_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    case EnumSearchFormType.User:
                        var userQuery = from val in db.users
                                        orderby val.username
                                        select val;

                        this.list.Columns.Add("ID");
                        this.list.Columns.Add("Username");
                        this.list.Columns.Add("Forename");
                        this.list.Columns.Add("Surname");
                        foreach (var val in userQuery)
                        {
                            var item = new ListViewItem(
                                new string[]
                                {
                                    Convert.ToString(val.user_id),
                                    val.username,
                                    val.forename,
                                    val.surname
                                }
                            );
                            item.Tag = val.user_id;
                            this.list.Items.Add(item);
                        }

                        this.list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        break;

                    default:
                        throw new NotImplementedException($"Type: {dataType}");
                }
            }
        }

        private void list_DoubleClick(object sender, EventArgs e)
        {
            var selected = this.list.SelectedItems;
            if (selected.Count == 0)
                return;

            // The tag for each item is always it's ID.
            var id = (int)selected[0].Tag;
            this.openEditorByType(this.type, EnumEditorMode.Modify, id);
        }

        // New entry button
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            this.openEditorByType(this.type, EnumEditorMode.Create);
        }
    }
}

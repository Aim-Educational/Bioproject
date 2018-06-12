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

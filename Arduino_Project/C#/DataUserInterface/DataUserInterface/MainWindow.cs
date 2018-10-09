using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DataUserInterface.Forms;

namespace DataUserInterface
{
    public partial class MainWindow : Form
    {
        private int childFormNumber = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region Help Menu Items
        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var childForm = new SearchForm(EnumSearchFormType.Device);
            childForm.MdiParent = this;
            childForm.Show();
        }
        #endregion

        #region File Menu Items
        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Edit Menu Items
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        #endregion

        #region View Menu Items
        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }
        #endregion

        #region Window Menu Items
        private void ShowNewForm(object sender, EventArgs e)
        {
            var testWindow = new FormDeviceEditor(EnumEditorMode.Create);
            testWindow.MdiParent = this;
            testWindow.Show();
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }
        #endregion

        private void showSearchForm(EnumSearchFormType type)
        {
            var form = new SearchForm(type);
            form.MdiParent = this;
            form.Show();
        }

        private void actionLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.ActionLevel);
        }

        private void actionTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.ActionType);
        }

        private void alarmTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.AlarmType);
        }

        private void applicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.Application);
        }

        private void applicationLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.ApplicationLog);
        }

        private void backupLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.BackupLog);
        }

        private void bBCBarometricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.BBCRSSBarometricChange);
        }

        private void bBCGeneralToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.BBCRSSGeneral);
        }

        private void bBCVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.BBCRSSVisibility);
        }

        private void configurationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.RSSConfiguration);
        }

        private void contactTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.ContactType);
        }

        private void databaseConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.DatabaseConfig);
        }

        private void deviceAddressTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.DeviceAddressType);
        }

        private void deviceHistoryActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.DeviceHistoryAction);
        }

        private void deviceTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.DeviceType);
        }

        private void deviceValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.DeviceValue);
        }

        private void groupTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.GroupType);
        }

        private void historyEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.HistoryEvent);
        }

        private void messageTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.MessageType);
        }

        private void uniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.Unit);
        }

        private void updatePeriodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.UpdatePeriod);
        }

        private void deviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.Device);
        }

        private void alarmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.Alarm);
        }

        private void contactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.Contact);
        }

        private void deviceAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.DeviceAddress);
        }

        private void groupActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.GroupAction);
        }

        private void groupMemberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.GroupMember);
        }

        private void supplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.Supplier);
        }

        private void userToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.showSearchForm(EnumSearchFormType.User);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var form = new FormReport1();
            form.MdiParent = this;
            form.Show();
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            var form = new DeviceValueGenerator();
            form.MdiParent = this;
            form.Show();
        }
    }
}

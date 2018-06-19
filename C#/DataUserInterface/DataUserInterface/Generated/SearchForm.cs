using System;
using System.Windows.Forms;


namespace DataUserInterface.Forms
{
    public partial class SearchForm : Form
    {
        void openEditorByType(EnumSearchFormType type, EnumEditorMode mode, int id = -1)
        {
            Form form;
            switch(this.type)
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

                case EnumSearchFormType.DeviceUrl:
                    form = new FormDeviceUrlEditor(mode, id);
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
                    throw new NotImplementedException($"No editor for type: {this.type}");
            }
        }
    }
}


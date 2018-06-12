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
                case EnumSearchFormType.Device:
                    form = new FormDeviceEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                case EnumSearchFormType.DeviceType:
                    form = new FormDeviceTypeEditor(mode, id);
                    form.MdiParent = this.MdiParent;
                    form.Show();
                    break;

                default:
                    throw new NotImplementedException($"No editor for type: {this.type}");
            }
        }
    }
}


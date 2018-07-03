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
            this.populateSearchResultsForType(dataType);
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

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using EFLayer.Model;

namespace Maintainer.Controls
{
    /// <summary>
    /// Interaction logic for EditorMood.xaml
    /// </summary>
    public partial class EditorMood : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool _isCreateMode;

        public bool isDataDirty
        {
            get => true;
        }

        public EditorMood(MainInterface mainInterface)
        {
            InitializeComponent();
            this._mainInterface = mainInterface;
        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var mood = selectedItem as tbl_mood;
            if (mood == null)
                throw new ArgumentException("Invalid type. Expected a tbl_mood.", "selectedItem");

            this.id.Text = $"{mood.mood_id}";
            this.description.Text = mood.description;

            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new RadioPlayer())
            {
                tbl_mood mood = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.id.Text);
                    mood = db.tbl_mood.Where(m => m.mood_id == id).FirstOrDefault();
                    if (mood == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.id.Text}' does not exist.";
                        return;
                    }
                }
                else
                    mood = new tbl_mood();

                mood.description = this.description.Text;

                if (this._isCreateMode)
                    db.tbl_mood.Add(mood);
                db.SaveChanges();
            }
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.id.Text = "(CREATE NEW)";
            this.description.Text = "";
        }
    }
}

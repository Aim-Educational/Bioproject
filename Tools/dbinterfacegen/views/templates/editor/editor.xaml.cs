$PLACEHOLDERS
    $MODEL_NAMESPACE
    $PROVIDER_NAMESPACE
    $EDITOR_NAMESPACE
    $EDITOR_NAME
    $PK_NAME
    $CONTEXT_NAME
    $OBJECT_TYPE
    $TABLE_NAME
    $CTOR_CODE
    $CREATE_CODE
    $LOAD_CODE
    $SAVE_CODE
$END
$FINISH_CONFIG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using $MODEL_NAMESPACE;
using $PROVIDER_NAMESPACE;

namespace $EDITOR_NAMESPACE
{
    /// <summary>
    /// Interaction logic for $EDITOR_NAME.xaml
    /// </summary>
    public partial class $EDITOR_NAME : UserControl, IEditor
    {
        private MainInterface _mainInterface;
        private bool          _isCreateMode;
        public  bool          isDataDirty => true;

        public $EDITOR_NAME(MainInterface mi)
        {
            InitializeComponent();
            this._mainInterface = mi;

            $CTOR_CODE
        }

        public void flagAsCreateMode()
        {
            this._isCreateMode = true;

            this.$PK_NAME.Text = "(CREATE NEW)";
            $CREATE_CODE
        }

        public RefreshSearchList onSearchAction(object selectedItem)
        {
            var saveResult = MessageBoxResult.No;
            this._mainInterface.protectedExecute(() => saveResult = this.askToSave());
            if (saveResult == MessageBoxResult.Cancel)
                return RefreshSearchList.no;

            this._isCreateMode = false;
            var data = selectedItem as $OBJECT_TYPE;
            if (data == null)
                throw new ArgumentException("Invalid type. Expected a $OBJECT_TYPE.", "selectedItem");

            $LOAD_CODE

            return (saveResult == MessageBoxResult.Yes) ? RefreshSearchList.yes : RefreshSearchList.no;
        }

        public void saveChanges()
        {
            using (var db = new $CONTEXT_NAME())
            {
                $OBJECT_TYPE data = null;

                if (!this._isCreateMode)
                {
                    var id = Convert.ToInt32(this.$PK_NAME.Text);
                    data = db.$TABLE_NAME.Where(d => d.$PK_NAME == id).FirstOrDefault();
                    if (data == null)
                    {
                        this._mainInterface.statusText = $"ERROR: ID '{this.$PK_NAME.Text}' does not exist.";
                        return;
                    }
                }
                else
                    data = new $OBJECT_TYPE();

                $SAVE_CODE

                if (this._isCreateMode)
                    db.$TABLE_NAME.Add(data);
                db.SaveChanges();
            }
        }
    }
}

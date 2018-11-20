$PLACEHOLDERS
    $MODEL_NAMESPACE        The namespace that contains the EF model.
    $PROVIDER_NAMESPACE     The namespace of the provider.
    $PROVIDER_NAME          The name of the provider.
    $TABLE_NAME             The name of the TableObject.
    $OBJECT_TYPE
    $PRIMARY_KEY            The name of the primary key for the TableObject.
    $TABLE_DISPLAY_NAME     The variable to use as the display name.
    $COLUMN_DEFINITIONS     Column definitions for the data grid.
    $CACHE_CODE             Code to cache certain values.
    $CONTEXTNAME
$END
$FINISH_CONFIG
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Aim.DatabaseInterface.Interfaces;
using $MODEL_NAMESPACE;

namespace $PROVIDER_NAMESPACE
{
    public class $PROVIDER_NAME : ISearchProvider
    {
        public bool areSameItems(object item1, object item2)
        {
            return (item1 as $OBJECT_TYPE).$PRIMARY_KEY == (item2 as $OBJECT_TYPE).$PRIMARY_KEY;
        }

        public void deleteItem(object item)
        {
            if(item == null)
                return;

            $OBJECT_TYPE data = item as $OBJECT_TYPE;
            if(data == null)
                return;

            using (var db = new $CONTEXTNAME())
            {
                db.$TABLE_NAME.Remove(db.$TABLE_NAME.Where(d => d.$PRIMARY_KEY == data.$PRIMARY_KEY).First());
                db.SaveChanges();
            }
        }

        public string getDisplayStringForItem(object item)
        {
            return $"{(item as $OBJECT_TYPE).$TABLE_DISPLAY_NAME}";
        }

        public void populateDataGrid(DataGrid grid)
        {
            using (var db = new $CONTEXTNAME())
            {
                grid.Columns.Add(new DataGridTextColumn()
                {
                    Header = "ID",
                    Binding = new Binding(nameof($OBJECT_TYPE.$PRIMARY_KEY))
                });
                $COLUMN_DEFINITIONS

                foreach (var data in db.$TABLE_NAME)
                {
                    grid.Items.Add(data);

                    // Cache some of the data we need
                    $CACHE_CODE
                }
            }
        }
    }
}

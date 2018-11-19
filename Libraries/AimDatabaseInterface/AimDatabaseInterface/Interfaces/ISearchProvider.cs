using System;
using System.Windows.Controls;

namespace Aim.DatabaseInterface.Interfaces
{
    /// <summary>
    /// An interface used by classes that provide information about a specific type of database object.
    /// 
    /// This class is the lifeblood of the <see cref="Aim.DatabaseInterface.Controls.SearchControl"/> as without this interface,
    /// it wouldn't have any ability to display data.
    /// </summary>
    public interface ISearchProvider
    {
        /// <summary>
        /// Called whenever the <see cref="Aim.DatabaseInterface.Controls.SearchControl"/> needs to update
        /// it's data grid to reflect the data this provider provides information on.
        /// 
        /// In most cases, this is simply going over all the data in a table from the database and adding them into
        /// the <paramref name="grid"/>.
        /// 
        /// The <paramref name="grid"/> will be emptied before this function is called.
        /// 
        /// The first thing that should be done is to modify <see cref="System.Windows.Controls.DataGrid.Columns"/> to
        /// create the columns+headers for the grid, as well as the setup data bindings if needed. After that, populate
        /// <see cref="System.Windows.Controls.DataGrid.Items"/> with all of the data needed.
        /// 
        /// PLEASE NOTE, the items added into this grid should be a database object (e.g. from an EF model). This is 
        /// required for most functionality to work.
        /// </summary>
        /// <param name="grid">The DataGrid to populate</param>
        void populateDataGrid(DataGrid grid);

        /// <summary>
        /// Instructs the provider to delete the specified <paramref name="item"/> from it's data/database/table/whatever.
        /// </summary>
        /// <param name="item">
        ///     The item to delete.
        ///     
        ///     This item will be one of the same items added to the data grid via the <see cref="populateDataGrid(DataGrid)"/> function.
        /// </param>
        void deleteItem(Object item);

        /// <summary>
        /// In some cases an object will need to be represented as a string outside of the data grid, so
        /// this function exists to provide a 'display name' for the object.
        /// </summary>
        /// <param name="item">
        ///     The item to get the display name for.
        ///     
        ///     This item will be one of the same items added to the data grid via the <see cref="populateDataGrid(DataGrid)"/> function.
        /// </param>
        /// <returns>The display name for <paramref name="item"/></returns>
        string getDisplayStringForItem(Object item);

        /// <summary>
        /// When working with databases, there will be times where you may have two different
        /// objects referencing the same data in a database (e.g. a cached version which has been modified, and a fresh version
        /// straight from the database). Performing a simple comparison in this case wouldn't work as desired, so this function exists
        /// to provide the functionality to compare two different objects and test them for equality.
        /// 
        /// In most cases, this will be as simple as comparing the primary keys of the two objects.
        /// </summary>
        /// <param name="item1">The first item to compare</param>
        /// <param name="item2">The second item to compare</param>
        /// <returns>Whether the items reference the same data or not</returns>
        bool areSameItems(Object item1, Object item2);
    }
}

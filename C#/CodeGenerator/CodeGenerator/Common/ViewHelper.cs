using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using CodeGenerator.Database;

namespace CodeGenerator.Common
{
    /// <summary>
    /// Contains helper functions/variables for views.
    /// </summary>
    public static class ViewHelper
    {
        /// <summary>
        /// The standard width for the 0th column of a view.
        /// 
        /// This exists so any user controls made for views, and all other views, can be designed in a consistent way.
        /// </summary>
        public static GridLength View_Column_0_Width
        {
            private set { }
            get { return new GridLength(80); }
        }

        /// <summary>
        /// The standard width for the 1st column of a view.
        /// </summary>
        public static GridLength View_Column_1_Width
        {
            private set {}
            get { return new GridLength(140, GridUnitType.Star); }
        }

        /// <summary>
        /// The standard width for the 2nd column of a view.
        /// </summary>
        public static GridLength View_Column_2_Width
        {
            private set { }
            get { return new GridLength(25); }
        }

        /// <summary>
        /// The standard height for a row of a view.
        /// 
        /// This exists so any user controls made for views, and all other views, can be designed in a consistent way.
        /// </summary>
        public static GridLength View_Row_Height
        {
            private set { }
            get { return new GridLength(25); }
        }

        /// <summary>
        /// Populates a drop down box (combobox) with the descriptions of `T` from the database.
        /// 
        /// For example, if `T` were `device_type`, then the drop down would contain all of the descriptions/names of 
        /// all the entries in the device_type table.
        /// </summary>
        /// <typeparam name="T">The database type to use</typeparam>
        /// <param name="db">The database connection</param>
        /// <param name="dropDown">The dropdown to populate</param>
        /// <param name="list">This list will be filled with all of the records from the table</param>
        /// <param name="additionalProcessing">
        ///     This function will be called with each record of `T` from the database, in case any additional action is required.
        ///     
        ///     The records are passed in order of description.
        ///     
        ///     This alleviates the need to then create another foreach right after this function, just to do some additional action.
        /// </param>
        public static void populateDropDownWithT<T>(this DatabaseCon db, ComboBox dropDown, out List<T> list, Action<T> additionalProcessing = null) where T : class
        {
            list = db._getSetFromT<T>().ToList();

            foreach(var value in list.OrderBy(v => { dynamic dyV = v; return dyV.description; }))
            {
                dynamic dyValue = value;
                dropDown.Items.Add(dyValue.description);

                additionalProcessing?.Invoke(value);
            }
        }
    }
}

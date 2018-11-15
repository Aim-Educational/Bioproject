using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Maintainer.Interfaces
{
    public interface IEditor : ISearchAction
    {
        void flagAsCreateMode();
        void saveChanges();
        bool isDataDirty { get; }
    }

    public static class IEditorHelper
    {
        public static MessageBoxResult askToSave(this IEditor me)
        {
            if(me == null)
                return MessageBoxResult.No;

            if(me.isDataDirty)
            {
                var result = MessageBox.Show("Do you want to save your changes before they're lost?", "Are you sure?",
                                             MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if(result == MessageBoxResult.Cancel || result == MessageBoxResult.No)
                    return result;

                me.saveChanges();
                return MessageBoxResult.Yes;
            }

            return MessageBoxResult.No;
        }
    }
}

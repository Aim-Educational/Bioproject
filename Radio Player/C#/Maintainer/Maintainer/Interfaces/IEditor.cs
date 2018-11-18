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

        public static void getMappings<MapT, MapValueT>(out List<MapT> toUnmap,
                                                        out List<MapValueT> toMap,
                                                        IEnumerable<Object> cachedItems,
                                                        ICollection<MapT> mapSet,
                                                        Func<MapT, MapValueT, bool> isSameTest) where MapT : class where MapValueT : class
        {
            toUnmap = new List<MapT>();
            toMap = cachedItems.Select(obj => obj as MapValueT).ToList();
            foreach (var mapping in mapSet)
            {
                bool shouldUnmap = true;
                foreach (var selectedMapValue in cachedItems.Select(obj => obj as MapValueT))
                {
                    if (isSameTest(mapping, selectedMapValue))
                    {
                        toMap.Remove(selectedMapValue);
                        shouldUnmap = false;
                        break;
                    }
                }

                if (shouldUnmap)
                    toUnmap.Add(mapping);
            }
        }
    }
}

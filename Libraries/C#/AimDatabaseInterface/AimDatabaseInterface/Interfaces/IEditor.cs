using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Aim.DatabaseInterface.Interfaces
{
    /// <summary>
    /// This interface describes a class used as an editor.
    /// 
    /// IEditors are used with the <see cref="Aim.DatabaseInterface.Windows.MainInterface"/> control to provide
    /// functionality for modifying the data provided by an <see cref="ISearchProvider"/>
    /// 
    /// The way an editor gets it's data is by implementing the <see cref="ISearchAction.onSearchAction(object)"/> function,
    /// which is where it's data is provided.
    /// </summary>
    public interface IEditor : ISearchAction
    {
        /// <summary>
        /// This function is used to flag to the editor that it is creating a new piece of data, rather
        /// than modifying an existing one.
        /// </summary>
        void flagAsCreateMode();

        /// <summary>
        /// This function is used to take the information from the editor, and update/create a new piece of data in a database/table/whatever.
        /// 
        /// <seealso cref="IEditorHelper.askToSave(IEditor)"/>
        /// </summary>
        void saveChanges();

        /// <summary>
        /// A property which flags whether the editor's data is dirty (has been modified/differs from the one in the database/cached data/whatever).
        /// 
        /// This is used in several places, namely whether the <see cref="IEditorHelper.askToSave(IEditor)"/> function should display
        /// a save dialog before overriding the editor's data.
        /// 
        /// It is a valid option to always return true, but may be annoying for users since the save dialog will always appear.
        /// </summary>
        bool isDataDirty { get; }
    }

    /// <summary>
    /// Provides extensions/helper function for an <see cref="IEditor"/>
    /// </summary>
    public static class IEditorHelper
    {
        /// <summary>
        /// Displays a save dialog to the user.
        /// 
        /// If the user presses 'yes', then <paramref name="me"/>'s <see cref="IEditor.saveChanges"/> function is called.
        /// 
        /// If <paramref name="me"/>'s <see cref="IEditor.isDataDirty"/> flag returns false, then no dialog is shown.
        /// 
        /// If the user presses 'cancel' then <paramref name="me"/> should abort the saving.
        /// </summary>
        /// <param name="me">The editor asking to the save.</param>
        /// <returns>
        ///     Either <see cref="System.Windows.MessageBoxResult.Yes"/>, <see cref="System.Windows.MessageBoxResult.No"/>,
        ///     or <see cref="System.Windows.MessageBoxResult.Cancel"/>
        /// </returns>
        public static MessageBoxResult askToSave(this IEditor me)
        {
            if (me == null)
                return MessageBoxResult.No;

            if (me.isDataDirty)
            {
                var result = MessageBox.Show("Do you want to save your changes before they're lost?", "Are you sure?",
                                             MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Cancel || result == MessageBoxResult.No)
                    return result;

                me.saveChanges();
                return MessageBoxResult.Yes;
            }

            return MessageBoxResult.No;
        }

        /// <summary>
        /// A very specialised and niche function.
        /// 
        /// Imagine you have a database with 3 tables, tbl_song, tbl_playlist, and tbl_playlistmap.
        /// 
        /// tbl_song describes a song, tbl_playlist contains general information about a playlist, and tbl_playlistmap
        /// contains the mappings between a song and a playlist (in other words, what songs belong to what playlists).
        /// 
        /// tbl_playlistmap would be the <typeparamref name="MapT"/>, tbl_song would be the <typeparamref name="MapValueT"/>,
        /// and tbl_playlist would be used to create the <paramref name="mapSet"/>.
        /// 
        /// When using something such as the <see cref="Aim.DatabaseInterface.Controls.ListboxSelectorEditorControl"/> for an easy way
        /// to edit which tbl_songs belong to a tbl_playlist, there are 3 things that need to be updated in the database.
        /// 
        /// 1. Which of the tbl_songs need to have a mapping(tbl_playlistmap) created for them (since they were added to the list).
        /// 
        /// 2. Which of the tbl_songs need to have their mappings removed (since they were removed off the list).
        /// 
        /// 3. Which of the tbl_songs can have their mappings left alone (since they were already on the list).
        /// 
        /// This function will essentially figure these things out for you, and populate 2 lists of which mappings need to be made and
        /// removed.
        /// </summary>
        /// <typeparam name="MapT">The type of the mapping object. Using the example this would be tbl_playlistmap.</typeparam>
        /// <typeparam name="MapValueT">The type of the value you're mapping. Using the example this would be tbl_song.</typeparam>
        /// <param name="toUnmap">
        ///     A list that is populated with all the mappings that should be removed.
        ///     
        ///     These values come directly from <paramref name="mapSet"/>.
        /// </param>
        /// <param name="toMap">
        ///     A list that is populated with all the values that need to have a mapping created for them.
        ///     
        ///     These values come directly from <paramref name="items"/>.
        /// </param>
        /// <param name="items">
        ///     The items that must be mapped.
        ///     
        ///     This is likely gotten from a control such as <see cref="Aim.DatabaseInterface.Controls.ListboxSelectorEditorControl"/>.
        /// </param>
        /// <param name="mapSet">
        ///     All of the mappings related to some other object.
        ///     
        ///     Using the example, this would be all of the tbl_playlistmaps that reference a certain
        ///     tbl_playlist.
        /// </param>
        /// <param name="hasMapping">
        ///     This function need to take in a Map, and a Value, and return whether the Map is a mapping
        ///     for the given Value.
        ///     
        ///     Using the example, this would test to see whether a specific tbl_playlistmap is a mapping for a specific tbl_song.
        /// </param>
        public static void getMappings<MapT, MapValueT>(out List<MapT>              toUnmap,
                                                        out List<MapValueT>         toMap,
                                                        IEnumerable<MapValueT>      items,
                                                        ICollection<MapT>           mapSet,
                                                        Func<MapT, MapValueT, bool> hasMapping) where MapT : class where MapValueT : class
        {
            toUnmap = new List<MapT>();
            toMap = items.Select(obj => obj as MapValueT).ToList();
            foreach (var mapping in mapSet)
            {
                bool shouldUnmap = true;
                foreach (var selectedMapValue in items)
                {
                    if (hasMapping(mapping, selectedMapValue))
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

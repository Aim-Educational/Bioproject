using System;

namespace Aim.DatabaseInterface.Interfaces
{
    /// <summary>
    /// An enum used by <see cref="ISearchAction.onSearchAction(object)"/>.
    /// </summary>
    public enum RefreshSearchList
    {
        /// <summary>
        /// Informs the <see cref="Aim.DatabaseInterface.Controls.SearchControl"/> that it should refresh it's data.
        /// </summary>
        yes,

        /// <summary>
        /// Informs the <see cref="Aim.DatabaseInterface.Controls.SearchControl"/> that it should refresh it's data.
        /// </summary>
        no
    }

    /// <summary>
    /// An interface which is used by the <see cref="Aim.DatabaseInterface.Controls.SearchControl"/> to perform
    /// an action when an item has been selected(double clicked).
    /// </summary>
    public interface ISearchAction
    {
        /// <summary>
        /// Performs an action on the selected item.
        /// </summary>
        /// <param name="selectedItem">
        ///     The item that was selected, the actual type of this item depends on what item type
        ///     the <see cref="ISearchProvider"/> populated the <see cref="Aim.DatabaseInterface.Controls.SearchControl"/> with.
        ///     
        ///     In general, this function should throw an <see cref="System.ArgumentException"/> if <paramref name="selectedItem"/> is
        ///     of a type that isn't supported.
        ///     
        ///     <see cref="System.ArgumentNullException"/> should be thrown if <paramref name="selectedItem"/> is null.
        /// </param>
        /// <returns>Whether the <see cref="Aim.DatabaseInterface.Controls.SearchControl"/> should refresh it's data.</returns>
        RefreshSearchList onSearchAction(Object selectedItem);
    }
}

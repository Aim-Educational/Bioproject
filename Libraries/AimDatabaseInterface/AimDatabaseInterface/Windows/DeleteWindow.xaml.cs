using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Aim.DatabaseInterface.Interfaces;

namespace Aim.DatabaseInterface.Windows
{
    /// <summary>
    /// A dialog window used to confirm to the user that they want to delete a certain item.
    /// </summary>
    public partial class DeleteBox : Window
    {
        public DeleteBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows a new deletion dialog box.
        /// 
        /// This function doesn't actually do any form of deletion, that is up to the caller.
        /// </summary>
        /// <param name="editor">
        ///     An editor which can display to the user what data is inside of the given <paramref name="value"/>.
        ///     
        ///     This allows the user to visibly see what item they are trying to delete.
        /// </param>
        /// <param name="value">
        ///     The value to be deleted, this is simply passed to the given <paramref name="editor"/> and isn't
        ///     modified by this class
        /// </param>
        /// <returns>Whether the user wants to delete the item or not. So <see cref="MessageBoxResult.Yes"/> or <see cref="MessageBoxResult.No"/></returns>
        public static MessageBoxResult Show(IEditor editor, object value)
        {
            if(editor == null)
                throw new ArgumentNullException("editor");

            var window = new DeleteBox();
            window.content.Content = editor;
            window.content.IsEnabled = false;
            window.ShowActivated = true;
            editor.onSearchAction(value);

            return ((bool)window.ShowDialog()) ? MessageBoxResult.Yes : MessageBoxResult.No;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}

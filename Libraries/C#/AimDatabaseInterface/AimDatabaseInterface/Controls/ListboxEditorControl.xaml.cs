using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Aim.DatabaseInterface.Controls
{
    /// <summary>
    /// A rather simple editor control that simply lets a user create a list of strings.
    /// 
    /// The items can be accessed using <see cref="ListboxEditorControl.list"/>
    /// </summary>
    public partial class ListboxEditorControl : UserControl
    {
        public ListboxEditorControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Clears both the input text box, as well as the value list.
        /// </summary>
        public void clear()
        {
            this.inputBox.Text = "";
            this.list.Items.Clear();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(this.inputBox.Text))
                return;

            // TODO: Make this customisable
            if (this.inputBox.Text.Contains(","))
            {
                MessageBox.Show("Input string cannot contain a comma.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            this.list.Items.Add(this.inputBox.Text);
            this.inputBox.Text = "";
        }

        private void inputBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                this.btnAdd_Click(sender, null);
            }
        }

        private void list_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && this.list.SelectedIndex != -1)
            {
                e.Handled = true;

                this.list.Items.RemoveAt(this.list.SelectedIndex);
            }
        }
    }
}

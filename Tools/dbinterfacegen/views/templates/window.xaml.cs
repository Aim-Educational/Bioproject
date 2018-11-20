$PLACEHOLDERS
    $WINDOW_NAMESPACE
    $CONTROL_NAMESPACE
    $PROVIDER_NAMESPACE
    $BUTTONS
$END
$FINISH_CONFIG
using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using $CONTROL_NAMESPACE;
using $PROVIDER_NAMESPACE;

namespace $WINDOW_NAMESPACE
{
    /// <summary>
    /// Interaction logic for InterfaceWindow.xaml
    /// </summary>
    public partial class InterfaceWindow : Window
    {
        private MainInterface _mainInterface;

        public InterfaceWindow()
        {
            InitializeComponent();

            this._mainInterface = new MainInterface();
            this.content.Content = this._mainInterface;

            $BUTTONS

            var button = new Button();
            button.Content = "[New Window]";
            button.Click += (sender, e) => (new InterfaceWindow()).Show();
            this.toolbar.Items.Add(button);
        }

        private void addEditorButton(string name, ISearchProvider provider, UserControl editor)
        {
            var button = new Button();
            button.Content = name;
            button.Click += (sender, e) => this._mainInterface.openEditor(provider, editor);
            this.toolbar.Items.Add(button);
        }
    }
}

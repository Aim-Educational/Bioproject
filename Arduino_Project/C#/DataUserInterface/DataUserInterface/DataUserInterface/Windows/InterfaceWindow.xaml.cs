using System.Windows;
using System.Windows.Controls;
using Aim.DatabaseInterface.Interfaces;
using Aim.DatabaseInterface.Windows;
using DataUserInterface.Editors;
using DataUserInterface.SearchProviders;

namespace DataUserInterface.Windows
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

            this.addEditorButton("action_level",  new SearchProvideraction_level(), new Editoraction_level(this._mainInterface));
this.addEditorButton("action_type",  new SearchProvideraction_type(), new Editoraction_type(this._mainInterface));
this.addEditorButton("alarm",  new SearchProvideralarm(), new Editoralarm(this._mainInterface));
this.addEditorButton("alarm_type",  new SearchProvideralarm_type(), new Editoralarm_type(this._mainInterface));
this.addEditorButton("application",  new SearchProviderapplication(), new Editorapplication(this._mainInterface));
this.addEditorButton("application_log",  new SearchProviderapplication_log(), new Editorapplication_log(this._mainInterface));
this.addEditorButton("backup_log",  new SearchProviderbackup_log(), new Editorbackup_log(this._mainInterface));
this.addEditorButton("bbc_rss_barometric_change",  new SearchProviderbbc_rss_barometric_change(), new Editorbbc_rss_barometric_change(this._mainInterface));
this.addEditorButton("bbc_rss_general",  new SearchProviderbbc_rss_general(), new Editorbbc_rss_general(this._mainInterface));
this.addEditorButton("bbc_rss_visibility",  new SearchProviderbbc_rss_visibility(), new Editorbbc_rss_visibility(this._mainInterface));
this.addEditorButton("contact",  new SearchProvidercontact(), new Editorcontact(this._mainInterface));
this.addEditorButton("contact_email",  new SearchProvidercontact_email(), new Editorcontact_email(this._mainInterface));
this.addEditorButton("contact_history",  new SearchProvidercontact_history(), new Editorcontact_history(this._mainInterface));
this.addEditorButton("contact_telephone",  new SearchProvidercontact_telephone(), new Editorcontact_telephone(this._mainInterface));
this.addEditorButton("contact_type",  new SearchProvidercontact_type(), new Editorcontact_type(this._mainInterface));
this.addEditorButton("database_config",  new SearchProviderdatabase_config(), new Editordatabase_config(this._mainInterface));
this.addEditorButton("device",  new SearchProviderdevice(), new Editordevice(this._mainInterface));
this.addEditorButton("device_address",  new SearchProviderdevice_address(), new Editordevice_address(this._mainInterface));
this.addEditorButton("device_address_type",  new SearchProviderdevice_address_type(), new Editordevice_address_type(this._mainInterface));
this.addEditorButton("device_history",  new SearchProviderdevice_history(), new Editordevice_history(this._mainInterface));
this.addEditorButton("device_history_action",  new SearchProviderdevice_history_action(), new Editordevice_history_action(this._mainInterface));
this.addEditorButton("device_type",  new SearchProviderdevice_type(), new Editordevice_type(this._mainInterface));
this.addEditorButton("device_url",  new SearchProviderdevice_url(), new Editordevice_url(this._mainInterface));
this.addEditorButton("device_value",  new SearchProviderdevice_value(), new Editordevice_value(this._mainInterface));
this.addEditorButton("group_action",  new SearchProvidergroup_action(), new Editorgroup_action(this._mainInterface));
this.addEditorButton("group_member",  new SearchProvidergroup_member(), new Editorgroup_member(this._mainInterface));
this.addEditorButton("group_type",  new SearchProvidergroup_type(), new Editorgroup_type(this._mainInterface));
this.addEditorButton("history_event",  new SearchProviderhistory_event(), new Editorhistory_event(this._mainInterface));
this.addEditorButton("menu",  new SearchProvidermenu(), new Editormenu(this._mainInterface));
this.addEditorButton("message_type",  new SearchProvidermessage_type(), new Editormessage_type(this._mainInterface));
this.addEditorButton("rss_configuration",  new SearchProviderrss_configuration(), new Editorrss_configuration(this._mainInterface));
this.addEditorButton("supplier",  new SearchProvidersupplier(), new Editorsupplier(this._mainInterface));
this.addEditorButton("unit",  new SearchProviderunit(), new Editorunit(this._mainInterface));
this.addEditorButton("update_period",  new SearchProviderupdate_period(), new Editorupdate_period(this._mainInterface));
this.addEditorButton("user",  new SearchProvideruser(), new Editoruser(this._mainInterface));


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

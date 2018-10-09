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

using EFLayer.Model;
using System.Data.Entity;

namespace Maintainer.Forms
{
    /// <summary>
    /// Interaction logic for MoodEditor.xaml
    /// </summary>
    public partial class EditorMood : Window, IEditor
    {
        public EditorType type { get; set; }
        public int objectID { get; set; }

        public EditorMood(EditorType type, int id = -1)
        {
            InitializeComponent();

            this.type = type;
            this.objectID = id;
            this.Title += $" - {Convert.ToString(type)}";

            if (id != -1)
            {
                using (var db = new RadioPlayer())
                {
                    if(db.tbl_mood.Where(m => m.mood_id == id).Count() == 0)
                    {
                        MessageBox.Show($"No object with the ID of {id} exists.",
                                         "Not found",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Error);
                        this.Close();
                        return;
                    }
                }

                this.onUpdateUI();
            }
        }

        private void button_apply_Click(object sender, RoutedEventArgs e)
        {
            switch(this.type)
            {
                case EditorType.Create:
                    this.onCreate();
                    break;

                case EditorType.Update:
                    this.onUpdate();
                    break;

                case EditorType.Delete:
                    this.onDelete();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public void onUpdateUI()
        {
            using (var db = new RadioPlayer())
            {
                var mood = db.tbl_mood.Where(m => m.mood_id == this.objectID).First();
                this.textbox_id.Text = Convert.ToString(mood.mood_id);
                this.textbox_description.Text = mood.description;
            }
        }

        public void onCreate()
        {
            using (var db = new RadioPlayer())
            {
                if (this.doesDescriptionExist(db))
                    return;

                var mood = new tbl_mood();
                mood.description = this.textbox_description.Text;

                db.tbl_mood.Add(mood);
                db.SaveChanges();
                this.textbox_id.Text = $"{mood.mood_id}";
                this.type = EditorType.Update;
                this.objectID = mood.mood_id;
            }
        }

        public void onUpdate()
        {
            using (var db = new RadioPlayer())
            {
                var mood = db.tbl_mood.Where(m => m.mood_id == this.objectID).First();

                if(mood.description != this.textbox_description.Text
                && !this.doesDescriptionExist(db))
                {
                    mood.description = this.textbox_description.Text;
                    db.SaveChanges();
                }
            }
        }

        public void onDelete()
        {
            using (var db = new RadioPlayer())
            {
                db.tbl_mood.Remove(db.tbl_mood.Where(m => m.mood_id == this.objectID).First());
                db.SaveChanges();
                this.Close();
            }
        }

        bool doesDescriptionExist(RadioPlayer db)
        {
            if (db.tbl_mood.Where(m => m.description.ToLower() == this.textbox_description.Text.ToLower()).Count() > 0)
            {
                MessageBox.Show($"The mood '{this.textbox_description.Text}' already exists.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                return true;
            }

            return false;
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

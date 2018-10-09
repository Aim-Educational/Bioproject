using DataManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataUserInterface.Forms
{
    public partial class DeviceValueGenerator : Form
    {
        public DeviceValueGenerator()
        {
            InitializeComponent();

            FormHelper.unlimitNumericBox(this.numericAmount, AllowDecimals.no);
            FormHelper.unlimitNumericBox(this.numericMax, AllowDecimals.no);
            FormHelper.unlimitNumericBox(this.numericMin, AllowDecimals.no);
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            this.progressBar.Maximum = Convert.ToInt32(this.numericAmount.Value);
            this.progressBar.Value = 0;

            using (var db = new PlanningContext())
            {
                Random rng = new Random();
                var devices = db.devices.ToArray();

                for (int i = 0; i < Convert.ToInt32(this.numericAmount.Value); i++)
                {
                    device_value value = new device_value();
                    value.comment = "Auto-generated";
                    value.value = rng.Next(Convert.ToInt32(this.numericMin.Value), Convert.ToInt32(this.numericMax.Value));
                    value.device = devices[rng.Next(0, devices.Length)];
                    value.extra_data = "N/A";

                    var timespan = this.dateTimeEnd.Value - this.dateTimeStart.Value;
                    var randomTime = new TimeSpan(0, rng.Next(0, (int)timespan.TotalMinutes), 0);
                    value.datetime = this.dateTimeStart.Value + randomTime;

                    this.progressBar.PerformStep();
                    this.progressBar.Invalidate();
                    this.progressBar.Update();
                    db.device_value.Add(value);
                }

                db.SaveChanges();
            }
        }
    }
}

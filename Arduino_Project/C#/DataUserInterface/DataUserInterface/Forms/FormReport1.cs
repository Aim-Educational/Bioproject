using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DataManager.Model;
using System.Windows.Forms.DataVisualization.Charting;

namespace DataUserInterface.Forms
{
    enum MoveDirection
    {
        up,
        down
    }

    public partial class FormReport1 : Form
    {
        private bool _skipCheckedItems { get; set; }

        private string[] _blacklistedTypes =
        {
            "Kagi",
            "PointAndFigure",
            "Renko",
            "StackedArea",
            "StackedArea100",
            "StackedBar100",
            "StackedColumn100",
            "ThreeLineBreak"
        };

        private void populateList()
        {
            this.listDevices.Items.Clear();
            using (var db = new PlanningContext())
            {
                var query = from dev in db.devices
                            orderby dev.name
                            select dev;

                // WORKAROUND: Adding a new item to the list will modify it's 'Checked' state, which calls
                //             the ItemChecked event when we're not expecting it, so the _skipCheckedItems
                //             flag forces the event to do nothing.
                this._skipCheckedItems = true;
                foreach (var dev in query)
                {
                    this.listDevices.Items.Add(new ListViewItem(
                        new string[]
                        {
                            Convert.ToString(dev.device_id),
                            dev.name
                        }
                    )).Tag = dev.device_id;
                }
                this.listDevices.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                this._skipCheckedItems = false;
            }
        }

        private void populateChartDropdown()
        {
            foreach (var typeName in Enum.GetNames(typeof(SeriesChartType))
                                         .Where(name => !this._blacklistedTypes.Contains(name))
                                         .OrderBy(name => name))
            {
                this.dropdownChartType.Items.Add(typeName);
            }
        }

        public FormReport1()
        {
            InitializeComponent();

            this.populateList();
            this.populateChartDropdown();
        }

        private void FormReport1_Load(object sender, EventArgs e)
        {
        }

        private void FormReport1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                return;

            this.tabView.Size = this.Size;
            this.listDevices.Size = new Size(this.tabPage1.Width / 2, this.tabPage1.Height);
            this.chartIndividual.Size = this.tabPage2.Size;

            /*
            // The tab that chartStacked is in will have more than one control, so some extra stuff is needed
            // to resize it.
            var chartPos = this.chartStacked.Location;
            var tabPos = this.tabPage3.Location;
            var localPos = new Point(chartPos.X - tabPos.X, chartPos.Y - tabPos.Y);
            this.chartStacked.Size = new Size(this.tabPage3.Size.Width - localPos.X, this.tabPage3.Size.Height - (localPos.Y + this.chartStacked.Margin.Top));
            */
        }

        private void ItemChecked_ChartTabIndividual(int deviceID, ItemCheckedEventArgs e)
        {
            using (var db = new PlanningContext())
            {
                var dev = db.devices.Single(d => d.device_id == deviceID);

                // If it were unchecked, remove the previous data.
                if (!e.Item.Checked)
                {
                    this.chartIndividual.Series.Remove(this.chartIndividual.Series.Single(s => s.ChartArea == dev.name));
                    this.chartIndividual.ChartAreas.Remove(this.chartIndividual.ChartAreas[dev.name]);
                    return;
                }

                // Ignore devices with no values.
                // 'dev.device_value' is a list of device_values that use this particular device ('dev').
                if (dev.device_value.Count == 0)
                    return;

                // Make a new area and series for the device
                var area = new ChartArea(dev.name);
                area.Position.Auto = true;
                area.InnerPlotPosition.Auto = true;
                area.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
                this.chartIndividual.ChartAreas.Add(area);

                var series = new Series();
                series.ChartArea = dev.name;
                series.ChartType = (dev.device_value.Count == 1) ? SeriesChartType.Column : SeriesChartType.Line;
                series.XValueType = ChartValueType.DateTime;
                series.YValueType = ChartValueType.Double;
                series.LegendText = dev.name;
                this.chartIndividual.Series.Add(series);

                // Add all of the device's values on
                foreach (var value in dev.device_value.OrderBy(v => v.datetime))
                    series.Points.AddXY(value.datetime, value.value);
                area.RecalculateAxesScale();

                // Force the chart to redraw
                this.chartIndividual.Invalidate();
            }
        }

        private void ItemChecked_ChartTabStacked(int deviceID, ItemCheckedEventArgs e)
        {
            using (var db = new PlanningContext())
            {
                var dev = db.devices.Single(d => d.device_id == deviceID);

                // If it were unchecked, remove the previous data.
                if (!e.Item.Checked)
                {
                    this.chartStacked.Series.Remove(this.chartStacked.Series.Single(s => (string)s.Tag == dev.name));
                    return;
                }

                // Ignore devices with no values.
                // 'dev.device_value' is a list of device_values that use this particular device ('dev').
                if (dev.device_value.Count == 0)
                    return;

                // Make a new series for the device
#error FIX ME
                var chartTypeName = this.dropdownChartType.SelectedText;
                var chartTypeExists = (chartTypeName == null) ? false : Enum.IsDefined(typeof(SeriesChartType), chartTypeName);

                var series = new Series();
                series.Tag = dev.name;
                series.ChartArea = "Default";
                series.ChartType = (chartTypeExists) ? (SeriesChartType)Enum.Parse(typeof(SeriesChartType), chartTypeName) : SeriesChartType.Line;
                series.XValueType = ChartValueType.DateTime;
                series.YValueType = ChartValueType.Double;
                series.LegendText = dev.name;
                this.chartStacked.Series.Add(series);

                // Add all of the device's values on
                foreach (var value in dev.device_value.OrderBy(v => v.datetime))
                    series.Points.AddXY(value.datetime, value.value);
                this.chartStacked.ChartAreas["Default"].RecalculateAxesScale();

                // Force the chart to redraw
                this.chartStacked.Invalidate();
            }
        }

        private void moveSelectedItems(MoveDirection direction)
        {
            var items = this.listDevices.Items;
            var selected = this.listDevices.SelectedIndices;

            var dummyItem = new ListViewItem();
            this._skipCheckedItems = true;
            for (int i = 0; i < selected.Count; i++)
            {
                var listIndex = selected[i];
                if (   (direction == MoveDirection.up && listIndex == 0) 
                    || (direction == MoveDirection.down && listIndex == this.listDevices.Items.Count - 1))
                    continue;

                var current = this.listDevices.Items[listIndex];
                this.listDevices.Items[listIndex] = dummyItem;

                if (direction == MoveDirection.up)
                {
                    var above = this.listDevices.Items[listIndex - 1];
                    this.listDevices.Items[listIndex - 1] = current;
                    this.listDevices.Items[listIndex] = above;
                }
                else
                {
                    var below = this.listDevices.Items[listIndex + 1];
                    this.listDevices.Items[listIndex + 1] = current;
                    this.listDevices.Items[listIndex] = below;
                }
            }
            this._skipCheckedItems = false;
            this.remakeCharts();
        }

        private void remakeCharts()
        {
            this.chartStacked.Series.Clear();
            this.chartIndividual.Series.Clear();
            this.chartIndividual.ChartAreas.Clear();

            for(int i = 0; i < this.listDevices.Items.Count; i++)
            {
                var item = this.listDevices.Items[i];
                var e = new ItemCheckedEventArgs(item);

                if (!item.Checked)
                    continue;

                this.ItemChecked_ChartTabIndividual((int)item.Tag, e);
                this.ItemChecked_ChartTabStacked((int)item.Tag, e);
            }
        }

        private void listDevices_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // Workaround an issue that comes up when adding in items.
            if (this._skipCheckedItems)
                return;

            var item = e.Item;
            var deviceID = (int)item.Tag;

            this.ItemChecked_ChartTabIndividual(deviceID, e);
            this.ItemChecked_ChartTabStacked(deviceID, e);
        }

        private void dropdownChartType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var index = this.dropdownChartType.SelectedIndex;
            var value = this.dropdownChartType.Items[index] as string;

            // All the values inside the drop down are valid names for the enum
            var enumValue = (SeriesChartType)Enum.Parse(typeof(SeriesChartType), value);

            foreach(var series in this.chartStacked.Series)
                series.ChartType = enumValue;
        }

        // Move selection up by 1
        private void button1_Click(object sender, EventArgs e)
        {
            this.moveSelectedItems(MoveDirection.up);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.moveSelectedItems(MoveDirection.down);
        }
    }
}

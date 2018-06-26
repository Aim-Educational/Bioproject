using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataUserInterface.Forms
{
    enum AllowDecimals
    {
        yes,
        no
    }

    static class FormHelper
    {
        public static void unlimitNumericBox(NumericUpDown numberBox, AllowDecimals decimals = AllowDecimals.yes)
        {
            if (numberBox == null)
                throw new ArgumentNullException("numberBox");

            numberBox.Minimum = decimal.MinValue;
            numberBox.Maximum = decimal.MaxValue;

            if(decimals == AllowDecimals.yes)
                numberBox.DecimalPlaces = 5;
        }

        public static void selectAllText(NumericUpDown numberBox)
        {
            if (numberBox == null)
                throw new ArgumentNullException("numberBox");

            numberBox.Select(0, numberBox.Text.Length);
        }

        public static void disableControl(Object control)
        {
            var type = control.GetType();

            if (type == typeof(Button))
                (control as Button).Enabled = false;
            else if (type == typeof(NumericUpDown))
                (control as NumericUpDown).Enabled = false;
            else if (type == typeof(TextBox))
                (control as TextBox).Enabled = false;
            else if (type == typeof(ComboBox))
                (control as ComboBox).Enabled = false;
            else if (type == typeof(Label))
                return;
            else
                throw new NotImplementedException(control.GetType().ToString());
        }
    }
}

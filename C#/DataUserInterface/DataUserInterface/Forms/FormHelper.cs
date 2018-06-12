using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataUserInterface.Forms
{
    static class FormHelper
    {
        public static void unlimitNumericBox(NumericUpDown numberBox)
        {
            if (numberBox == null)
                throw new ArgumentNullException("numberBox");

            numberBox.Minimum = decimal.MinValue;
            numberBox.Maximum = decimal.MaxValue;
        }

        public static void selectAllText(NumericUpDown numberBox)
        {
            if (numberBox == null)
                throw new ArgumentNullException("numberBox");

            numberBox.Select(0, numberBox.Text.Length);
        }
    }
}

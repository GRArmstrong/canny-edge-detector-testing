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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Canny_Edge_Detector
{
    /// <summary>
    /// Interaction logic for SliderCombo.xaml
    /// </summary>
    public partial class SliderCombo : UserControl
    {
        public int Value { get { return (int)slValue.Value; } }

        public event EventHandler<EventArgs> ValueUpdated;

        public SliderCombo()
        {
            InitializeComponent();
        }

        public void Setup(string name, double min, double max)
        {
            tbLabel.Text = name;
            slValue.Minimum = min;
            slValue.Maximum = max;
            tbMin.Text = min.ToString();
            tbMax.Text = max.ToString();
            txtValue.Text = slValue.Value.ToString("F0");

            slValue.ValueChanged += slValue_ValueChanged;
        }

        private void slValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            txtValue.Text = slValue.Value.ToString("F0");
            ValueUpdated(this, new EventArgs());
        }

        private void txtValue_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                double val = double.Parse(txtValue.Text);
                if (val < slValue.Minimum || val > slValue.Maximum)
                {
                    throw new Exception("Text value outside range");
                }
                else
                {
                    slValue.Value = val;
                }
            }
            catch (Exception)
            {
                txtValue.Text = slValue.Value.ToString("F0");
            }
            ValueUpdated(this, new EventArgs());
        }
    }
}

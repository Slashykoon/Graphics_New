using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Graphics_New
{
    public partial class CurveDetail : UserControl
    {
        public event EventHandler<CurveCheckedEventArgs> CurveCheckedChanged;
        public event EventHandler<CurveCheckedEventArgs> CurveColorChanged;
        public CurveDetail(String Name, Color col)
        {
            InitializeComponent();
            cb_CurveTitle.Text = Name;
            btn_color.BackColor = col;
        }

        private void btn_color_Click(object sender, EventArgs e)
        {
            dialog_curvecolor.ShowDialog();
            btn_color.BackColor = dialog_curvecolor.Color;

        }

        private void cb_CurveTitle_CheckedChanged(object sender, EventArgs e)
        {
            CurveCheckedChanged?.Invoke(this, new CurveCheckedEventArgs
            {
                IsChecked = cb_CurveTitle.Checked,
                CurveName = cb_CurveTitle.Text,
                CurveColor = btn_color.BackColor
            });
        }


        public class CurveCheckedEventArgs : EventArgs
        {
            public bool IsChecked { get; set; }
            public string CurveName { get; set; }
            public Color CurveColor { get; set; }
        }

        private void btn_color_BackColorChanged(object sender, EventArgs e)
        {
            CurveColorChanged?.Invoke(this, new CurveCheckedEventArgs
            {
                IsChecked = cb_CurveTitle.Checked,
                CurveName = cb_CurveTitle.Text,
                CurveColor = btn_color.BackColor
            });
        }
    }
}

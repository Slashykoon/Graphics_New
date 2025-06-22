using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Graphics_New
{
    public class Signal : Curve
    {
        public string Name { get; set; } = "DefaultName";
        private Color _Col;

        public ScottPlot.Color ScottCol { get ; set; }
        private String _sColor = "0,0,0";

        public String sColor
        {
            get => _sColor;
            set
            {
                _sColor = value;
               
            }
            
        }
        public Color Col
        {
            get => _Col;
            set
            {
                _Col = value;

            }

        }

        public bool IsDetector { get; set; } = false;
        public int Index { get; set; } = -1; // Index of the signal in the record
        public string Unit { get; set; } = ""; // Default unit for the signal
        public List<float> XPoints { get; set; } = new List<float>();
        public List<float> YPoints { get; set; } = new List<float>();

        public Signal(int idx, string sName, string sColor, Boolean IsDetect) : base()
        {
            Name = sName;
           
            if (sColor != null && sColor != string.Empty)
            {
                this.sColor = sColor; // Color in RGB format as a string
                Col = GetFormatColor(); // Convert to Color object
            }
            else
            {
                this.sColor = "0,0,0"; // Default color in RGB format
                Col = Color.Black; // Default color
            }
            IsDetector = IsDetect;
            Index = idx; // Set the index of the signal
         

        }
        public ScottPlot.Color Convert_SysColToScottCol(Color c)
        {
            ScottPlot.Color tmp_Color = new ScottPlot.Color(c);
            return tmp_Color;
        }

        public Color GetFormatColor()
        {
            string[] rgb;
            if (sColor != null)
            {
                rgb = sColor.Split(',');
                Color c = Color.FromArgb(
                    int.Parse(rgb[0]),
                    int.Parse(rgb[1]),
                    int.Parse(rgb[2])
                );
                return c;
            }
            else
            {
                return Color.Black; // Default color if not specified
            }


        }
    }

}










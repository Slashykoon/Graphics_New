using ScottPlot.AxisPanels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScottPlot.Plottables;
using System.Drawing;
using ScottPlot;

namespace Graphics_New
{
    public class Curve 
    {
        public DataLogger CurveLogger { get; set; }
        public ScottPlot.Plot CurveLoaded { get; set; } 
        
        public ScottPlot.IYAxis YAxis;
        //public Boolean IsSelected = false;
        public Curve()
        {
            CurveLogger = new DataLogger();
            CurveLoaded = new ScottPlot.Plot();
            //CurveLogger.ViewSlide(100);
        }

     
        public void Clear()
        {
            CurveLogger.Clear();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using ScottPlot.AxisPanels;
using ScottPlot;
using Snap7;
using Microsoft.VisualBasic.Devices;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Graphics_New.CurveDetail;
using System.Windows.Forms.VisualStyles;

namespace Graphics_New
{
    public class Curve_Manager
    {
        private bool isRunning = false;
        private Thread refreshThread;
        private object plotLock = new object();

      

        // Create FormsPlot
        ScottPlot.WinForms.FormsPlot formsPlot = new ScottPlot.WinForms.FormsPlot
        {
            Dock = DockStyle.Fill
        };
        public Curve_Manager()
        {
            InitFormPlot();
        }
        private void CurveDetail_CurveColorChanged(object sender, CurveCheckedEventArgs e)
        {
            Signal s = Data.GetSignalByName(Data.CurrentRun, Data.CurrentRecord, e.CurveName);
            s.CurveLogger.Color=s.Convert_SysColToScottCol(e.CurveColor);
        }
        private void CurveDetail_CurveCheckedChanged(object sender, CurveCheckedEventArgs e)
        {
            // Handle the event
            Tools.LogToFile($"Curve: {e.CurveName}, Checked: {e.IsChecked}, Color: {e.CurveColor}");
          

            foreach (Signal sig in Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals())
            {
                if (sig.Name == e.CurveName)
                {
                    sig.CurveLogger.IsVisible = e.IsChecked;
                    if (sig.CurveLogger.Axes.YAxis != null)
                    {
                        sig.CurveLogger.Axes.YAxis.IsVisible = e.IsChecked;
                    }
                }
            }
                    // Perform action when checked (e.g., enable plotting)
                    // MessageBox.Show($"Curve {e.CurveName} is now checked with color {e.CurveColor}");
        }
        public void InitFormPlot()
        {
            // disable interactivity by default
            formsPlot.UserInputProcessor.Disable();
        }
       public void AttachCurves()
        {
            foreach (Signal sig in Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals())
            {
                //I want here to attach sig.CurveLogger to formsplot 
               sig.CurveLogger= formsPlot.Plot.Add.DataLogger();
            }
            Configure_Axes();
            ApplyScaleThight();
        }


        public void Configure_Axes()
        {
            // Ensure at least one left and one right axis as defaults (optional)
            var defaultLeftAxis = formsPlot.Plot.Axes.Left;
            var defaultRightAxis = formsPlot.Plot.Axes.Right;
            formsPlot.Plot.Axes.Remove(defaultLeftAxis);
            formsPlot.Plot.Axes.Remove(defaultRightAxis);
            foreach (Signal sig in Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals())
            {
                if(sig.IsDetector)
                {
                    sig.YAxis = formsPlot.Plot.Axes.AddLeftAxis();
                    sig.CurveLogger.Axes.YAxis = sig.YAxis;
                    sig.YAxis.Label.Text = sig.Name;
                    sig.YAxis.Label.ForeColor = sig.Convert_SysColToScottCol(sig.Col);
                    sig.YAxis.IsVisible = true;

                }
                else
                {
                    sig.YAxis = formsPlot.Plot.Axes.AddRightAxis();
                    sig.CurveLogger.Axes.YAxis = sig.YAxis;
               
                    sig.YAxis.Label.Text = sig.Name;
                    sig.YAxis.Label.ForeColor = sig.Convert_SysColToScottCol(sig.Col);
                    sig.YAxis.IsVisible = false;
                }
            }
            // Refresh the plot to render the new axes
            //formsPlot.Refresh();
        }

        private void ApplyManualScale()
        {
            foreach (Signal sig in Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals())
            {
                formsPlot.Plot.Axes.SetLimits(0, 50, -20, 20, formsPlot.Plot.Axes.Bottom, sig.YAxis);
            }
            //formsPlot.Plot.Axes.SetLimits(0, 50, -20_000, 20_000, formsPlot.Plot.Axes.Bottom, YAxis2);
            formsPlot.Refresh();
        }

        private void ApplyAutoscale()
        {
            formsPlot.Plot.Axes.Margins();
            formsPlot.Plot.Axes.AutoScale();
            formsPlot.Refresh();
        }

        private void ApplyScaleThight()
        {
            //formsPlot.Plot.Axes.Margins(0, 0);
            //formsPlot.Refresh();
        }

        private void ApplyAutoScaleWithMargin()
        {
            formsPlot.Plot.Axes.Margins(1, 1);
            formsPlot.Refresh();
        }


        public void AttachFormPlot(TableLayoutPanel tlp_MainGraphics)
        {
            tlp_MainGraphics.Controls.Add(formsPlot, 0, 0);
        }


        public void StartRefreshPlot()
        {
            isRunning = true;
            refreshThread = new Thread(RefreshPlotLoop)
            {
                IsBackground = true
            };
            refreshThread.Start();
        }
        private void RefreshPlotLoop()
        {
            while (isRunning)
            {
                lock (plotLock)
                {
                    // Use Invoke to update UI on the correct thread
                    if (formsPlot.InvokeRequired)
                    {
                        formsPlot.Invoke((MethodInvoker)delegate
                        {
                            formsPlot.Refresh();
                        });
                    }
                    else
                    {
                        formsPlot.Refresh();
                    }
                }
                Thread.Sleep(300);
            }
        }

        public void CloseThread()
        {
            isRunning = false;
            if (refreshThread != null && refreshThread.IsAlive)
            {
                //refreshThread.Join();
            }
        }

        public void RefreshCurveSelector(TableLayoutPanel tlp_CurveDetails)
        {
            foreach (Signal sig in Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals())
            {
                FillCurveDetails(sig.Name, sig.GetFormatColor(), tlp_CurveDetails);
            }
        }


        public void FillCurveDetails(string CurveName, System.Drawing.Color col, TableLayoutPanel tlp_CurveDetails)
        {
            CurveDetail CurveDetail_Instance = new(CurveName, col)
            {
                Margin = new(10, 10, 10, 10),
                Dock = DockStyle.Fill

            };

            CurveDetail_Instance.Size = new Size(500, 80); // Set desired size
            CurveDetail_Instance.Margin = new Padding(5); // Add some spacing around the control
            CurveDetail_Instance.MaximumSize = new Size(1000, 80); // Set maximum size to prevent stretching 

            
            tlp_CurveDetails.RowStyles.Add(new RowStyle(SizeType.AutoSize));
           
            // Add the user control to the new row (in the first column, index 0)
            tlp_CurveDetails.Controls.Add(CurveDetail_Instance, 0, tlp_CurveDetails.RowCount - 1);
            tlp_CurveDetails.RowCount++;

            CurveDetail_Instance.CurveCheckedChanged += CurveDetail_CurveCheckedChanged;
            CurveDetail_Instance.CurveColorChanged += CurveDetail_CurveColorChanged;
        }

    }
}


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
using static OpenTK.Graphics.OpenGL.GL;
using System.Security.Cryptography.X509Certificates;

namespace Graphics_New
{
    public class Curve_Manager
    {
        private bool isRunning = false;
        private Thread refreshThread;
        private object plotLock = new object();
        private object tlpLock = new object();


        // Create FormsPlot
        ScottPlot.WinForms.FormsPlot formsPlot = new ScottPlot.WinForms.FormsPlot
        {
            Dock = DockStyle.Fill

        };

        //AxisManager axisManager;
        public Curve_Manager()
        {
            InitFormPlot();
            // Get the AxisManager instance from ScottPlot  
            //var axisManager = formsPlot.Plot.Axes;
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
            foreach (Run run in Data.dRuns.Values)
            {
                foreach (Record rec in run.dRecords.Values)
                {
                    foreach (Signal sig in rec.Signals)
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
                }
            }

            /*foreach (Signal sig in Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals())
            {
                if (sig.Name == e.CurveName)
                {
                    sig.CurveLogger.IsVisible = e.IsChecked;
                    if (sig.CurveLogger.Axes.YAxis != null)
                    {
                        sig.CurveLogger.Axes.YAxis.IsVisible = e.IsChecked;
                    }
                }
            }*/
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
               sig.CurveLogger= formsPlot.Plot.Add.DataLogger();
            }
            Configure_Axes();
 
        }
        public void Construct_Axes()
        {

            foreach (Signal sig in Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals())
            {
                sig.YAxis.Label.Text = sig.Name;
                sig.YAxis.Label.ForeColor = sig.Convert_SysColToScottCol(sig.Col);
                sig.YAxis.IsVisible = true;

            }
        }


        public void Configure_Axes()
        {
            lock (formsPlot.Plot.Sync)
            {
                var plot = formsPlot.Plot;
                var axisManager = plot.Axes;
                bool GroupByUnit = false;
                // Optional: Clear existing custom axes
                foreach (var axis in axisManager.GetYAxes().ToList())
                    axisManager.Remove(axis);

                // Group signals
                Dictionary<string, IYAxis> sharedAxes = new();
              

                //var signals = Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals();
                foreach (Run run in Data.dRuns.Values)
                {
                    foreach (Record rec in run.dRecords.Values)
                    {
                        foreach (Signal sig in rec.Signals)
                        {
                            if (!sharedAxes.ContainsKey(sig.Unit))
                            {
                                IYAxis yAxis;
                                if (sig.IsDetector)
                                    yAxis = axisManager.AddLeftAxis();
                                else
                                    yAxis = axisManager.AddRightAxis();

                                yAxis.Label.Text = sig.Name + "(" + sig.Unit + ")";
                                //yAxis.Label.ForeColor = sig.Convert_SysColToScottCol(sig.Col);
                                yAxis.IsVisible = true;

                                sharedAxes[sig.Unit] = yAxis;
                            }

                            // Reuse axis for this signal
                            sig.YAxis = sharedAxes[sig.Unit];
                            sig.CurveLogger.Axes.YAxis = sig.YAxis;
                        }
                    }
                }
                axisManager.AutoScaleExpand();
                
            }
        }


        private void ApplyManualScale()
        {
            foreach (Signal sig in Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals())
            {
                formsPlot.Plot.Axes.SetLimits(0, 50, -20, 20, formsPlot.Plot.Axes.Bottom, sig.YAxis);
            }
        }


        private void ApplyScaleThight()
        {
            formsPlot.Plot.Axes.Margins(0, 0);
        }

        private void ApplyAutoScaleWithMargin()
        {
            formsPlot.Plot.Axes.Margins(1, 1);
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

                    // Use Invoke to update UI on the correct threa
                    if (formsPlot.InvokeRequired)
                    {
                        formsPlot.Invoke((MethodInvoker)delegate
                        {
                            lock (plotLock)
                            {
                                if (formsPlot != null)
                                    formsPlot.Refresh();
                            }
                        });
                    }
                    else
                    {
                        formsPlot.Refresh();
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
            lock (tlpLock)
            {
                if (tlp_CurveDetails.InvokeRequired)
                {
                    tlp_CurveDetails.Invoke((MethodInvoker)delegate
                    {
                        tlp_CurveDetails.Controls.Clear();
                        tlp_CurveDetails.RowStyles.Clear();
                        tlp_CurveDetails.RowCount = 0;

                        foreach (Signal sig in Data.GetRecord(Data.CurrentRun, Data.CurrentRecord).GetSignals())
                        {
                            FillCurveDetails(sig.Name, sig.GetFormatColor(), tlp_CurveDetails);
                        }

                    });
                }
            }
        }


        public void FillCurveDetails(string CurveName, System.Drawing.Color col, TableLayoutPanel tlp_CurveDetails)
        {
            CurveDetail CurveDetail_Instance = new(CurveName, col)
            {
                Margin = new(10, 10, 10, 10),
                Dock = DockStyle.Fill,
                Size = new Size(500, 80),
                MaximumSize = new Size(1000, 80)
            };

            // Increase the row count BEFORE adding the control
            int rowIndex = tlp_CurveDetails.RowCount++;
            tlp_CurveDetails.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            tlp_CurveDetails.Controls.Add(CurveDetail_Instance, 0, rowIndex);

            CurveDetail_Instance.CurveCheckedChanged += CurveDetail_CurveCheckedChanged;
            CurveDetail_Instance.CurveColorChanged += CurveDetail_CurveColorChanged;
        }

    }
}


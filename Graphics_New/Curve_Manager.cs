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
        private  bool isRunning = false;
        private  Thread refreshThread;
        private  object plotLock = new object();
        private  object tlpLock = new object();


        // Create FormsPlot
        ScottPlot.WinForms.FormsPlot formsPlot = new ScottPlot.WinForms.FormsPlot
        {
            Dock = DockStyle.Fill,
            Padding=new Padding(20, 20, 20, 20)
        };

        //AxisManager axisManager;
        public Curve_Manager()
        {
            InitFormPlot();
        }
        private void CurveDetail_CurveColorChanged(object sender, CurveCheckedEventArgs e)
        {
            foreach (Run run in Data.dRuns.Values)
            {
                foreach (Record rec in run.dRecords.Values)
                {
                    //Signal s = Data.GetSignalByName(Data.CurrentRun, Data.CurrentRecord, e.CurveName);
                    Signal s = Data.GetSignalByName(run.RunNumber, rec.RecordNumber, e.CurveName);
                    s.CurveLogger.Color = s.Convert_SysColToScottCol(e.CurveColor);
                }
            }

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
        }
        public void InitFormPlot()
        {
            // disable interactivity by default
            formsPlot.UserInputProcessor.Disable();
        }

    public void RecordToCurves(int? RunNb = null, int? RecNb = null)
        {
            int runNumber = RunNb ?? Data.CurrentRun;
            int recNumber = RecNb ?? Data.CurrentRecord;
            
            foreach (Signal sig in Data.GetRecord(runNumber, recNumber).GetSignals())
            {
                sig.CurveLogger = formsPlot.Plot.Add.DataLogger();
            }
            Configure_Axes();

            Tools.LogToFile("RecordToCurves");

            /*formsPlot.Invoke((MethodInvoker)delegate
            {
                formsPlot.Refresh();
            });*/
        }

        public void AppendAllPLCDataToSignals(Dictionary<int, List<Single>> dictAllDataLoaded, int RunLoaded, int RecLoaded)
        {
            if (dictAllDataLoaded.Count == 0)
                return;
            Data.AddNewRun(RunLoaded);
            Data.dRuns[RunLoaded].AttachNewRecord(RecLoaded);
            RecordToCurves(RunLoaded, RecLoaded);
            List<Signal> signals = Data.GetSignals(RunLoaded, RecLoaded);
            int newCount = dictAllDataLoaded.Count;

            for (int i = 0; i < newCount; i++)
            {
                List<Single> newValues = dictAllDataLoaded[i];
                for (int j = 0; j < signals.Count; j++)
                {
                    Signal s = signals[j];
                    //s.YPoints.Add(newValues[j]);
                    //s.XPoints.Add((i*500.0f)/1000.0f);
                    s.CurveLogger.Add((i * 500.0f) / 1000.0f, newValues[j]);

                }
            }
            Tools.LogToFile("AppendAllPLCDataToSignals");
        }

        public void Configure_Axes(bool? IsDataLoad=false)
        {
            lock (formsPlot.Plot.Sync)
            {
                var plot = formsPlot.Plot;
                var axisManager = plot.Axes;
 
                // Optional: Clear existing custom axes
                foreach (var axis in axisManager.GetYAxes().ToList())
                    axisManager.Remove(axis);
                foreach (var axis in axisManager.GetXAxes().ToList())
                    axisManager.Remove(axis);

                BottomAxis xAxis = axisManager.AddBottomAxis(); // Ensure X axis is added if not already present

                xAxis.LabelText = "Time (s)"; // Set X axis label
                xAxis.LabelOffsetY = 9; // Adjust label position if needed

                // Group signals
                Dictionary<string, IYAxis> sharedAxes = new();
              
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
                Tools.LogToFile("Configure_Axes");

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
            Tools.LogToFile("AttachFormPlot");
        }


        public void StartRefreshPlot()
        {
            isRunning = true;
            refreshThread = new Thread(RefreshPlotLoop)
            {
                IsBackground = true
            };
            refreshThread.Start();
            Tools.LogToFile("StartRefreshPlot");
        }
        private void RefreshPlotLoop()
        {
            while (isRunning)
            {
                var localPlot = formsPlot; // copie thread-safe

                if (localPlot == null)
                    break;

                if (localPlot.InvokeRequired)
                {
                    try
                    {
                        localPlot.Invoke((MethodInvoker)delegate
                        {
                            lock (plotLock)
                            {
                                localPlot.Refresh();
                            }
                        });
                    }
                    catch (ObjectDisposedException) { break; }
                    catch (InvalidOperationException) { break; }
                }
                else
                {
                    try
                    {
                        lock (plotLock)
                        {
                            localPlot.Refresh();
                        }
                    }
                    catch (ObjectDisposedException) { break; }
                    catch (InvalidOperationException) { break; }
                }

                Thread.Sleep(200);
            }
        }

        public void CloseThread()
        {
            isRunning = false;
            Tools.LogToFile("CloseThread (plot)");
            if (refreshThread != null && refreshThread.IsAlive)
            {
                //refreshThread.Join();
            }
        }

        public void GenerateCurveDetails(TableLayoutPanel tlp_CurveDetails, int? RunNb = null, int? RecNb = null)
        {
            lock (tlpLock)
            {
                if (tlp_CurveDetails.InvokeRequired)
                {
                    tlp_CurveDetails.Invoke((MethodInvoker)delegate
                    {
                        UpdateCurveDetails(tlp_CurveDetails, RunNb, RecNb);
                    });
                }
                else
                {
                    // Si on est déjà sur le thread UI : exécuter directement
                    UpdateCurveDetails(tlp_CurveDetails, RunNb, RecNb);
                }
                Tools.LogToFile("GenerateCurveDetails");
            }
        }

        private void UpdateCurveDetails(TableLayoutPanel tlp_CurveDetails, int? RunNb, int? RecNb)
        {
            tlp_CurveDetails.Controls.Clear();
            tlp_CurveDetails.RowStyles.Clear();
            tlp_CurveDetails.RowCount = 0;

            int runNumber = RunNb ?? Data.CurrentRun;
            int recNumber = RecNb ?? Data.CurrentRecord;

            foreach (Signal sig in Data.GetRecord(runNumber, recNumber).GetSignals())
            {
                FillCurveDetails(sig.Name, sig.GetFormatColor(), tlp_CurveDetails);
            }
            Tools.LogToFile("UpdateCurveDetails");
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

            Tools.LogToFile("FillCurveDetails");
        }

    }
}


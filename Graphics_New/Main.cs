using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.Colormaps;
using ScottPlot.Plottables;
using Snap7;
using System.ComponentModel;
using System.Data.Common;
using System.Drawing.Text;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using static OpenTK.Graphics.OpenGL.GL;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.DataFormats;



namespace Graphics_New
{
    public partial class Main : Form
    {

        PLCInterface plc;
        Curve_Manager Cvm;
        Selection selectionForm;

        System.Windows.Forms.Label LabelInfo_CurrRunRec;

        public Main()
        {

    
            this.TopMost = true;
            
            InitializeComponent();
            this.Shown += Main_Shown;

            /*var vl = formsPlot1.Plot.Add.VerticalLine(2);
            vl.IsDraggable = true;
            vl.Text = "???";

            var hl = formsPlot1.Plot.Add.HorizontalLine(2);
            hl.IsDraggable = true;
            hl.Text = "???";*/
            // use events for custom mouse interactivity
            /*formsPlot1.MouseDown += FormsPlot1_MouseDown;
            formsPlot1.MouseUp += FormsPlot1_MouseUp;
            formsPlot1.MouseMove += FormsPlot1_MouseMove;*/

        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            // Lancement des opérations longues
            //await Task.Delay(1000); // juste pour revenir au message loop

            Data.PropertyChanged += (sender, e) =>
            {
                if (lbl_infos.InvokeRequired)
                {
                    lbl_infos.BeginInvoke((System.Windows.Forms.MethodInvoker)(() => HandlePropertyChange(sender, e)));
                }
                else
                {
                    HandlePropertyChange(sender, e);
                }
            };


            Cvm = new Curve_Manager();
            this.Icon = new Icon("growth-curve.ico");
            NotifyIcon trayIcon = new NotifyIcon
            {
                Text = "Graphics new gen",
                Icon = this.Icon, // Use a default icon; replace with your own .ico file
                Visible = true
            };

            LabelInfo_CurrRunRec = new System.Windows.Forms.Label
            {
                Text = "Running Run : ?? Record : ??", // Set the text to display
                Dock = DockStyle.Fill,
                Font = new Font("Roboto", 10), // Set font and size
                Visible = true // Ensure the label is visible
            };
            tlp_Information.Controls.Add(LabelInfo_CurrRunRec, 1, 0);
            //Data.AddNewRun();
            plc = new PLCInterface();


          

            plc.OnNewRecord += () =>
            {
                Data.AddNewRun();
                if (Data.CurrentRecord > 0)
                {
                    SQLite.UpdateRecordDataInSQLite(Data.dRuns[Data.CurrentRun].dRecords[Data.CurrentRecord].Pk_Record, Tools.GetBinaryFilePath(Data.CurrentRun, Data.CurrentRecord));
                }

                plc.NewRecordTriggered = !Data.dRuns[Data.CurrentRun].AttachNewRecord();
                Cvm.GenerateCurveDetails(tlp_CurveDetails);
                Cvm.RecordToCurves();
            };

            // Appliquer les valeurs initiales à l’UI
            HandlePropertyChange(null, new PropertyChangedEventArgs(nameof(Data.CurrentRun)));
            HandlePropertyChange(null, new PropertyChangedEventArgs(nameof(Data.CurrentRecord)));

            Cvm.AttachFormPlot(tlp_MainGraphic);
            Cvm.StartRefreshPlot();

            


        }

        public void HandlePropertyChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Data.CurrentRun))
            {
                if (Data.CurrentRun <= 0)
                {
                    //lbl_infos.Text = "No Run loaded or launched";
                    LabelInfo_CurrRunRec.Text = "No Run launched";

                }
                else
                {
                    lbl_infos.Text = $"Current Run changed to {Data.CurrentRun}";

                    LabelInfo_CurrRunRec.Text = $"Running Run {Data.CurrentRun} Record {Data.CurrentRecord}";
                }

            }
            else if (e.PropertyName == nameof(Data.CurrentRecord))
            {
                if (Data.CurrentRecord <= 0)
                {
                    //LabelInfo_CurrRunRec.Text = "No Run launched";
                }
                else
                {
                    lbl_infos.Text = $"Current Record changed to {Data.CurrentRecord}";
                    LabelInfo_CurrRunRec.Text = $"Running Run {Data.CurrentRun} Record {Data.CurrentRecord}";
                }
            }
            else if (e.PropertyName == nameof(Data.LastLoadedRun))
            {
                //LabelInfo_CurrRunRec.Text = "No Run loaded or launched";
                lbl_infos.Text = $"Loaded Run {Data.LastLoadedRun} and  Record {Data.LastLoadedRecord} successfull";

            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Cvm.CloseThread();
            //plc.StopReadingLoop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectionForm = new Selection();
            selectionForm.ResultReady += (s, data) =>
            {
                // Data.AddNewRun(data._RunNum);
                // Data.dRuns[data._RunNum].AttachNewRecord(data._RecNum);

                plc.AppendAllPLCDataToSignals(data._DictLoaded, data._RunNum, data._RecNum);
                Cvm.GenerateCurveDetails(tlp_CurveDetails, data._RunNum, data._RecNum);
                Cvm.RecordToCurves(data._RunNum, data._RecNum);
                

            };
            selectionForm.ShowDialog();
        }
        private void selectionForm_SelectedToLoad(object sender, string result)
        {

        }

        private void lbl_infos_Click(object sender, EventArgs e)
        {

        }


        /*AxisLine? PlottableBeingDragged = null;
        private System.Windows.Forms.Timer monTimer;

        private void FormsPlot1_MouseDown(object? sender, MouseEventArgs e)
        {
            var lineUnderMouse = GetLineUnderMouse(e.X, e.Y);
            if (lineUnderMouse is not null)
            {
                PlottableBeingDragged = lineUnderMouse;
                formsPlot1.UserInputProcessor.Disable(); // disable panning while dragging
            }
        }

        private void FormsPlot1_MouseUp(object? sender, MouseEventArgs e)
        {
            PlottableBeingDragged = null;
            formsPlot1.UserInputProcessor.Enable(); // enable panning again
            formsPlot1.Refresh();
        }

        private void FormsPlot1_MouseMove(object? sender, MouseEventArgs e)
        {
            // this rectangle is the area around the mouse in coordinate units
            CoordinateRect rect = formsPlot1.Plot.GetCoordinateRect(e.X, e.Y, radius: 10);

            if (PlottableBeingDragged is null)
            {
                // set cursor based on what's beneath the plottable
                var lineUnderMouse = GetLineUnderMouse(e.X, e.Y);
                if (lineUnderMouse is null) Cursor = Cursors.Default;
                else if (lineUnderMouse.IsDraggable && lineUnderMouse is VerticalLine) Cursor = Cursors.SizeWE;
                else if (lineUnderMouse.IsDraggable && lineUnderMouse is HorizontalLine) Cursor = Cursors.SizeNS;
            }
            else
            {
                // update the position of the plottable being dragged
                if (PlottableBeingDragged is HorizontalLine hl)
                {
                    hl.Y = rect.VerticalCenter;
                    hl.Text = $"{hl.Y:0.00}";
                }
                else if (PlottableBeingDragged is VerticalLine vl)
                {
                    vl.X = rect.HorizontalCenter;
                    vl.Text = $"{vl.X:0.00}";
                }
                formsPlot1.Refresh();
            }
        }

        private AxisLine? GetLineUnderMouse(float x, float y)
        {
            CoordinateRect rect = formsPlot1.Plot.GetCoordinateRect(x, y, radius: 10);

            foreach (AxisLine axLine in formsPlot1.Plot.GetPlottables<AxisLine>().Reverse())
            {
                if (axLine.IsUnderMouse(rect))
                    return axLine;
            }

            return null;
        }*/

    }
}

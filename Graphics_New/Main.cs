using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using Snap7;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using static OpenTK.Graphics.OpenGL.GL;



namespace Graphics_New
{
    public partial class Main : Form
    {
 
        PLCInterface plc;
        Curve_Manager Cvm;


        public Main()
        {
            InitializeComponent();
            this.Icon = new Icon("growth-curve.ico");
            NotifyIcon trayIcon = new NotifyIcon
            {
                Text = "Graphics new gen",
                Icon = this.Icon, // Use a default icon; replace with your own .ico file
                Visible = true
            };

            System.Windows.Forms.Label LabelInfo_CurrRunRec = new System.Windows.Forms.Label
            {
                Text = "Running Run : ?? Record : ??", // Set the text to display
                Dock= DockStyle.Fill,
                Font = new Font("Roboto", 10), // Set font and size
                Visible = true // Ensure the label is visible
            };

            Data.AddNewRun();
            var plc = new PLCInterface();
            Cvm = new Curve_Manager();
            if (!plc.StartReadingLoop())
                return;

            plc.OnNewRecord += () =>
            {
                plc.NewRecordTriggered = !Data.dRuns[Data.CurrentRun].AttachNewRecord();
                
            };

            Data.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Data.CurrentRun))
                {
                    lbl_infos.Text = $"Current Run changed to {Data.CurrentRun}";
                    LabelInfo_CurrRunRec.Text= $"Running Run {Data.CurrentRun} Record {Data.CurrentRecord}";
                }
                else if (e.PropertyName == nameof(Data.CurrentRecord))
                {
                    lbl_infos.Text = $"Current Record changed to {Data.CurrentRecord}";
                    LabelInfo_CurrRunRec.Text = $"Running Run {Data.CurrentRun} Record {Data.CurrentRecord}";
                }
            };

            Cvm.AttachFormPlot(tlp_MainGraphic);
            Cvm.RefreshCurveSelector(tlp_CurveDetails);
            Cvm.AttachCurves();

            Cvm.StartRefreshPlot();

           tlp_Information.Controls.Add(LabelInfo_CurrRunRec,1,0);
            
            //tlp_CurveDetails.Controls.Add(CurveDetail_Instance, 0, tlp_CurveDetails.RowCount - 1);


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
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Cvm.CloseThread();
            //plc.StopReadingLoop();
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

using ScottPlot;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Graphics_New
{
    public partial class Selection : Form
    {
        public int MemSelectedRun;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TreeView treeView2;
        TableLayoutPanel layoutRunInfo;
        TableLayoutPanel layoutRecInfo;
        Icon icoDB = new Icon("database-solid.ico");
        Icon icoSelect = new Icon("right-long-solid.ico");

        System.Drawing.Image iconRun = System.Drawing.Image.FromFile("database-solid.png");
        System.Drawing.Image iconName = System.Drawing.Image.FromFile("tag-solid.png");
        System.Drawing.Image iconDesc = System.Drawing.Image.FromFile("pen-to-square-solid.png");
        System.Drawing.Image iconStart = System.Drawing.Image.FromFile("calendar-days-solid.png");
        System.Drawing.Image iconEnd = System.Drawing.Image.FromFile("calendar-days-solid.png");
        System.Drawing.Image iconNoRec = System.Drawing.Image.FromFile("circle-exclamation-solid.png");

        public Selection()
        {
            InitializeComponent();
            // Initialize form
            this.Text = "Selection of saved Runs/Records";
            

            // Initialize TreeView
            treeView1 = new System.Windows.Forms.TreeView
            {
                //Location = new Point(10, 10),
                //Size = new Size(360, 240),
                Font = new Font("Roboto", 10), // Modern font
                BackColor = System.Drawing.Color.White,
                ForeColor = System.Drawing.Color.DarkSlateGray,
                BorderStyle = BorderStyle.FixedSingle, // Clean border
                ShowLines = true,
                FullRowSelect = true, // Highlight full row on selection
                ItemHeight = 42,// Larger item height for modern look
                Scrollable = true, // Enable scrolling if needed
                Dock = DockStyle.Fill // Fill the parent container

            };

            // Initialize TreeView
            treeView2 = new System.Windows.Forms.TreeView
            {
                //Location = new Point(10, 10),
                //Size = new Size(360, 240),
                Font = new Font("Roboto", 10), // Modern font
                BackColor = System.Drawing.Color.White,
                ForeColor = System.Drawing.Color.DarkSlateGray,
                BorderStyle = BorderStyle.FixedSingle, // Clean border
                ShowLines = true,
                FullRowSelect = true, // Highlight full row on selection
                ItemHeight = 42,// Larger item height for modern look
                Scrollable = true, // Enable scrolling if needed
                Dock = DockStyle.Fill // Fill the parent container

            };

            layoutRunInfo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 5,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
            };

            layoutRecInfo = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 5,
                AutoSize = true,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
            };

            layoutRunInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24)); // Icon column
            layoutRunInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));  // Label
            layoutRunInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));  // TextBox

            layoutRecInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 24)); // Icon column
            layoutRecInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));  // Label
            layoutRecInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));  // TextBox



            tlp_Selection.Controls.Add(layoutRunInfo, 0, 1);
            tlp_Selection.Controls.Add(layoutRecInfo, 1, 1);
            tlp_Selection.Controls.Add(treeView1, 0, 0);
            tlp_Selection.Controls.Add(treeView2, 1, 0);

            // Setup icons
            SetupTreeViewIcons();

            // Populate TreeView
            PopulateTreeViewRuns();
            PopulateTreeViewRecords();

            // Handle events
            treeView1.AfterSelect += TreeView1_AfterSelect;
            treeView1.NodeMouseHover += TreeView1_NodeMouseHover;
            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;

            treeView2.AfterSelect += TreeView2_AfterSelect;
            treeView2.NodeMouseHover += TreeView2_NodeMouseHover;
            treeView2.NodeMouseDoubleClick += TreeView2_NodeMouseDoubleClick;

        }

        private void SetupTreeViewIcons()
        {
            // Create ImageList for icons
            ImageList imageList = new ImageList
            {
                ImageSize = new Size(26, 26), // Standard icon size
                ColorDepth = ColorDepth.Depth32Bit // High-quality icons
            };

            // Add the database icon from Resources
            // Fallback to SystemIcon if resource is missing
            imageList.Images.Add("Runs", icoDB);
            imageList.Images.Add("Selected", icoSelect);
            treeView1.ImageList = imageList;
            treeView2.ImageList = imageList;
        }

        private void PopulateTreeViewRuns()
        {
            treeView1.Nodes.Clear();

            // Create Run node with database icon
            TreeNode runsNode = new TreeNode("List of saved Runs")
            {
                ImageKey = "Runs",
                SelectedImageKey = "Runs",
                Tag = new { Id = 1 } // Placeholder for Run object
            };

            foreach (var kvp in SQLite.GetAllRunsIdx_Name())
            {
                TreeNode runNode = new TreeNode($"Run {kvp.Key} - " + kvp.Value)
                {
                    ImageKey = "Runs",
                    SelectedImageKey = "Selected",
                    Tag = new { Id = kvp.Key } // Placeholder for Run object
                };
                runsNode.Nodes.Add(runNode);
            }

            // Add Run node to TreeView
            treeView1.Nodes.Add(runsNode);

            // Expand Run node by default
            runsNode.Expand();
        }

        private void PopulateTreeViewRecords(int? runNumber = null)
        {
            treeView2.Nodes.Clear();

            TreeNode recsNode = new TreeNode($"Records for Run {(runNumber.HasValue ? runNumber.Value.ToString() : "")}")
            {
                ImageKey = "Runs",
                SelectedImageKey = "Selected",
                Tag = new { Id = runNumber }  // Don't force .Value — just store nullable
            };

            if (runNumber.HasValue)
            {
                var records = SQLite.GetAllRecordsOfRun(runNumber.Value); // Safe, we already checked HasValue

                foreach (var kvp in records)
                {
                    TreeNode recNode = new TreeNode($"Record {kvp.Key} - {kvp.Value}")
                    {
                        Tag = new { Id = kvp.Key }
                    };
                    recsNode.Nodes.Add(recNode);
                }
            }

            treeView2.Nodes.Add(recsNode);
            recsNode.Expand();
        }

        public void GetRunInfo(int SelectedRunNum)
        {
            long pk;
            int runNb;
            string name;
            string desc;
            layoutRunInfo.Controls.Clear();
            (pk,runNb,name,desc)=SQLite.GetRunDetails(SelectedRunNum);
            AddRow(0, "Run Number:", runNb.ToString(),iconRun, layoutRunInfo);
            AddRow(1, "Name:", name, iconName, layoutRunInfo);
            AddRow(2, "Description:", desc, iconDesc, layoutRunInfo);
            AddRow(3, "Start Time:", "2025-06-26 09:00", iconStart, layoutRunInfo);
            AddRow(4, "End Time:", "2025-06-26 10:30", iconEnd, layoutRunInfo);
        }

        public void GetRecordInfo(int SelectedRunNum,int SelectedRecNum)
        {
            long pk;
            int recNb;
            string name;
            string desc;
            layoutRecInfo.Controls.Clear();
            (pk, recNb, name, desc) = SQLite.GetRecordDetails(SelectedRecNum, SelectedRunNum);
            if (pk>=0)
            {
                AddRow(0, "Run Number:", recNb.ToString(), iconRun, layoutRecInfo);
                AddRow(1, "Name:", name, iconName, layoutRecInfo);
                AddRow(2, "Description:", desc, iconDesc, layoutRecInfo);
                AddRow(3, "Start Time:", "2025-06-26 09:00", iconStart, layoutRecInfo);
                AddRow(4, "End Time:", "2025-06-26 10:30", iconEnd, layoutRecInfo);
            }
            else
            {
                AddRow(0, "No Records found", "", iconNoRec, layoutRecInfo);
            }


        }
        void AddRow(int rowIndex, string label, string text, System.Drawing.Image icon,TableLayoutPanel tlp_info)
        {
            PictureBox pb = new PictureBox
            {
                Image = icon,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(20, 20),
                Anchor = AnchorStyles.Left
            };

            tlp_info.Controls.Add(pb, 0, rowIndex);
           
            //tlp_info.Controls.Add(new System.Windows.Forms.Label() { Text = label, TextAlign = ContentAlignment.MiddleLeft, Dock = DockStyle.Fill }, 1, rowIndex);
            tlp_info.Controls.Add(new System.Windows.Forms.Label() { Text = label, Anchor=AnchorStyles.Left | AnchorStyles.Right, AutoSize = true}, 1, rowIndex);
            tlp_info.Controls.Add(new System.Windows.Forms.TextBox() { Text = text, ReadOnly = true, Anchor = AnchorStyles.Left | AnchorStyles.Right }, 2, rowIndex);
        }
        
        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Handle node selection
            if (e.Node?.Tag != null)
            {
                // Extract the ID from the Tag object
                var tagObject = e.Node.Tag;
                var tagType = tagObject.GetType();

                // Use reflection to get the "Id" property (since it’s an anonymous type)
                var idProperty = tagType.GetProperty("Id");
                if (idProperty != null)
                {
                    var value = idProperty.GetValue(tagObject);
                    if (value is int runNumber)
                    {
                        MemSelectedRun = runNumber; // Store the selected run number
                        GetRunInfo((int)value);
                      
                    }
                }
            }
        }
        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Reset other nodes
            foreach (TreeNode node in treeView1.Nodes)
            {
                ResetNodeStyle(node, treeView1);
            }
            if (e.Node?.Tag != null)
            {
                // Extract the ID from the Tag object
                var tagObject = e.Node.Tag;
                var tagType = tagObject.GetType();

                // Use reflection to get the "Id" property (since it’s an anonymous type)
                var idProperty = tagType.GetProperty("Id");
                if (idProperty != null)
                {
                    var value = idProperty.GetValue(tagObject);
                    if (value is int runNumber)
                    {
                        MemSelectedRun = runNumber; // Store the selected run number
                        PopulateTreeViewRecords(runNumber);
                    }
                }
            }
        }
        private void TreeView2_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Reset other nodes
            
            foreach (TreeNode node in treeView2.Nodes)
            {
                ResetNodeStyle(node, treeView2);
            }
            // Handle node selection
            if (e.Node?.Tag != null)
            {
                // Extract the ID from the Tag object
                var tagObject = e.Node.Tag;
                var tagType = tagObject.GetType();

                // Use reflection to get the "Id" property (since it’s an anonymous type)
                var idProperty = tagType.GetProperty("Id");
                if (idProperty != null)
                {
                    var value = idProperty.GetValue(tagObject);
                    if (value is int runNumber)
                    {
                        //GetRecordInfo(MemSelectedRun, (int)value);

                    }
                }
            }
        }
        private void TreeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Handle node selection
            if (e.Node?.Tag != null)
            {
                // Extract the ID from the Tag object
                var tagObject = e.Node.Tag;
                var tagType = tagObject.GetType();

                // Use reflection to get the "Id" property (since it’s an anonymous type)
                var idProperty = tagType.GetProperty("Id");
                if (idProperty != null)
                {
                    var value = idProperty.GetValue(tagObject);
                    if (value is int runNumber)
                    {
                        GetRecordInfo(MemSelectedRun, (int)value);
                    }
                }
            }
        }
        private void TreeView1_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            // Highlight node on hover
            e.Node.BackColor = System.Drawing.Color.FromArgb(230, 240, 255); // Light blue highlight
            e.Node.ForeColor = System.Drawing.Color.Black;

            // Reset other nodes
            foreach (TreeNode node in treeView1.Nodes)
            {
                ResetNodeStyle(node,treeView1);
            }
        }
        private void TreeView2_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            // Highlight node on hover
            e.Node.BackColor = System.Drawing.Color.FromArgb(230, 240, 255); // Light blue highlight
            e.Node.ForeColor = System.Drawing.Color.Black;

            // Reset other nodes
            foreach (TreeNode node in treeView2.Nodes)
            {
                ResetNodeStyle(node,treeView2);
            }
        }
        private void ResetNodeStyle(TreeNode node,System.Windows.Forms.TreeView tv)
        {
            // Reset style for non-selected nodes
            if (node != tv.SelectedNode)
            {
                node.BackColor = System.Drawing.Color.White;
                node.ForeColor = System.Drawing.Color.DarkSlateGray;
            }
            foreach (TreeNode child in node.Nodes)
            {
                ResetNodeStyle(child, tv);
            }
        }




    }
}

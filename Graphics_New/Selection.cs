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
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.TreeView treeView2;
        Icon icoDB = new Icon("database-solid.ico");
        Icon icoSelect = new Icon("right-long-solid.ico");
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
                BackColor = Color.White,
                ForeColor = Color.DarkSlateGray,
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
                BackColor = Color.White,
                ForeColor = Color.DarkSlateGray,
                BorderStyle = BorderStyle.FixedSingle, // Clean border
                ShowLines = true,
                FullRowSelect = true, // Highlight full row on selection
                ItemHeight = 42,// Larger item height for modern look
                Scrollable = true, // Enable scrolling if needed
                Dock = DockStyle.Fill // Fill the parent container

            };
            //treeView2.CheckBoxes = true;

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

            // Add Record nodes (1 to 5) with document icon
            for (int i = 1; i <= 15; i++)
            {
                TreeNode runNode = new TreeNode($"Run {i}")
                {
                    //ImageKey = "Run",
                    SelectedImageKey = "Selected",
                    //StateImageKey = "Selected",
                   
                    Tag = new { Id = i } // Placeholder for Record object
                };
                runsNode.Nodes.Add(runNode);
            }

            // Add Run node to TreeView
            treeView1.Nodes.Add(runsNode);

            // Expand Run node by default
            runsNode.Expand();
        }

        private void PopulateTreeViewRecords()
        {
            treeView2.Nodes.Clear();

            // Create Run node with database icon
            TreeNode recsNode = new TreeNode("List of saved Records")
            {
                ImageKey = "Runs",
                SelectedImageKey = "Runs",
                Tag = new { Id = 1 } // Placeholder for Run object
          
            };

            // Add Record nodes (1 to 5) with document icon
            for (int i = 1; i <= 10; i++)
            {
                TreeNode recNode = new TreeNode($"Record {i}")
                {
                    //ImageKey = "Run",
                    //SelectedImageKey = "Selected",
                  
                    //StateImageKey = "Selected",
                    Tag = new { Id = i } // Placeholder for Record object
                };
                recsNode.Nodes.Add(recNode);
            }

            // Add Run node to TreeView
            treeView2.Nodes.Add(recsNode);

            // Expand Run node by default
            recsNode.Expand();
        }


        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Handle node selection
            if (e.Node.Tag != null)
            {
                dynamic tag = e.Node.Tag;
               
                //add ico selected
                //e.Node.ImageKey = "Selected";
                //MessageBox.Show($"Selected: {e.Node.Text} (ID: {tag.Id})");
            }
        }
        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // Reset other nodes
            foreach (TreeNode node in treeView1.Nodes)
            {
                ResetNodeStyle(node, treeView1);
            }
            // Handle node selection
            if (e.Node.Tag != null)
            {
                dynamic tag = e.Node.Tag;
                //add ico selected
                //e.Node.ImageKey = "Selected";
                //MessageBox.Show($"Selected: {e.Node.Text} (ID: {tag.Id})");
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
            if (e.Node.Tag != null)
            {
                dynamic tag = e.Node.Tag;
                e.Node.SelectedImageKey = "Selected"; // Change selected image key
                //add ico selected
                //e.Node.ImageKey = "Selected";
                //MessageBox.Show($"Selected: {e.Node.Text} (ID: {tag.Id})");
            }
        }

        private void TreeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Handle node selection
            if (e.Node.Tag != null)
            {
                dynamic tag = e.Node.Tag;
                //add ico selected
                //e.Node.ImageKey = "Selected";
                //MessageBox.Show($"Selected: {e.Node.Text} (ID: {tag.Id})");
            }
        }

        private void TreeView1_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            // Highlight node on hover
            e.Node.BackColor = Color.FromArgb(230, 240, 255); // Light blue highlight
            e.Node.ForeColor = Color.Black;

            // Reset other nodes
            foreach (TreeNode node in treeView1.Nodes)
            {
                ResetNodeStyle(node,treeView1);
            }
        }

        private void TreeView2_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
            // Highlight node on hover
            e.Node.BackColor = Color.FromArgb(230, 240, 255); // Light blue highlight
            e.Node.ForeColor = Color.Black;

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
                node.BackColor = Color.White;
                node.ForeColor = Color.DarkSlateGray;
            }
            foreach (TreeNode child in node.Nodes)
            {
                ResetNodeStyle(child, tv);
            }
        }
    }
}

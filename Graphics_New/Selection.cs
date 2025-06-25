using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Graphics_New
{
    public partial class Selection : Form
    {
        public Selection()
        {
            InitializeComponent();


            // Clear existing nodes
            treeView_Run.Nodes.Clear();

            // Create top-level nodes (Categories)
            TreeNode booksNode = new TreeNode("Books");
            TreeNode electronicsNode = new TreeNode("Electronics");

            // Create second-level nodes (Items)
            TreeNode fictionNode = new TreeNode("Fiction");
            TreeNode nonFictionNode = new TreeNode("Non-Fiction");
            TreeNode phonesNode = new TreeNode("Phones");
            TreeNode laptopsNode = new TreeNode("Laptops");

            // Create third-level nodes (Details)
            fictionNode.Nodes.Add(new TreeNode("Harry Potter"));
            fictionNode.Nodes.Add(new TreeNode("Lord of the Rings"));
            nonFictionNode.Nodes.Add(new TreeNode("Sapiens"));
            nonFictionNode.Nodes.Add(new TreeNode("Atomic Habits"));
            phonesNode.Nodes.Add(new TreeNode("iPhone"));
            phonesNode.Nodes.Add(new TreeNode("Galaxy"));
            laptopsNode.Nodes.Add(new TreeNode("MacBook"));
            laptopsNode.Nodes.Add(new TreeNode("ThinkPad"));

            // Build the hierarchy
            booksNode.Nodes.Add(fictionNode);
            booksNode.Nodes.Add(nonFictionNode);
            electronicsNode.Nodes.Add(phonesNode);
            electronicsNode.Nodes.Add(laptopsNode);

            // Add top-level nodes to TreeView
            treeView_Run.Nodes.Add(booksNode);
            treeView_Run.Nodes.Add(electronicsNode);

            // Expand the first node by default
            if (treeView_Run.Nodes.Count > 0)
            {
                treeView_Run.Nodes[0].Expand();
            }
        }
    }
}

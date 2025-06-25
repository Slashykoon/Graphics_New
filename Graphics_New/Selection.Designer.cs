namespace Graphics_New
{
    partial class Selection
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            treeView_Run = new TreeView();
            SuspendLayout();
            // 
            // treeView_Run
            // 
            treeView_Run.Location = new Point(12, 12);
            treeView_Run.Name = "treeView_Run";
            treeView_Run.Size = new Size(346, 533);
            treeView_Run.TabIndex = 0;
            // 
            // Selection
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1081, 705);
            Controls.Add(treeView_Run);
            Name = "Selection";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        public TreeView treeView_Run;
    }
}
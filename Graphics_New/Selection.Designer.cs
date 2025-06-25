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
            tlp_Selection = new TableLayoutPanel();
            SuspendLayout();
            // 
            // tlp_Selection
            // 
            tlp_Selection.ColumnCount = 2;
            tlp_Selection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp_Selection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp_Selection.Dock = DockStyle.Fill;
            tlp_Selection.Location = new Point(0, 0);
            tlp_Selection.Name = "tlp_Selection";
            tlp_Selection.RowCount = 2;
            tlp_Selection.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlp_Selection.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tlp_Selection.Size = new Size(1247, 727);
            tlp_Selection.TabIndex = 0;
            // 
            // Selection
            // 
            AutoScaleDimensions = new SizeF(15F, 33F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1247, 727);
            Controls.Add(tlp_Selection);
            Font = new Font("Roboto", 10.125F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "Selection";
            Text = "Selection";
            ResumeLayout(false);
        }

        #endregion

        public TableLayoutPanel tlp_Selection;
    }
}
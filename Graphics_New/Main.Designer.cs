﻿namespace Graphics_New
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tlp_global = new TableLayoutPanel();
            tlp_MainGraphic = new TableLayoutPanel();
            tlp_CurveDetails = new TableLayoutPanel();
            tlp_Information = new TableLayoutPanel();
            lbl_infos = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            button1 = new Button();
            button2 = new Button();
            timer1 = new System.Windows.Forms.Timer(components);
            notifyIcon1 = new NotifyIcon(components);
            tlp_global.SuspendLayout();
            tlp_MainGraphic.SuspendLayout();
            tlp_Information.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tlp_global
            // 
            tlp_global.ColumnCount = 1;
            tlp_global.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlp_global.Controls.Add(tlp_MainGraphic, 0, 1);
            tlp_global.Controls.Add(tlp_Information, 0, 2);
            tlp_global.Controls.Add(tableLayoutPanel1, 0, 0);
            tlp_global.Dock = DockStyle.Fill;
            tlp_global.Location = new Point(0, 0);
            tlp_global.Name = "tlp_global";
            tlp_global.RowCount = 3;
            tlp_global.RowStyles.Add(new RowStyle(SizeType.Percent, 7.216495F));
            tlp_global.RowStyles.Add(new RowStyle(SizeType.Percent, 92.78351F));
            tlp_global.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tlp_global.Size = new Size(1734, 916);
            tlp_global.TabIndex = 0;
            // 
            // tlp_MainGraphic
            // 
            tlp_MainGraphic.BackColor = SystemColors.Control;
            tlp_MainGraphic.ColumnCount = 2;
            tlp_MainGraphic.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tlp_MainGraphic.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tlp_MainGraphic.Controls.Add(tlp_CurveDetails, 1, 0);
            tlp_MainGraphic.Dock = DockStyle.Fill;
            tlp_MainGraphic.Location = new Point(3, 65);
            tlp_MainGraphic.Name = "tlp_MainGraphic";
            tlp_MainGraphic.RowCount = 1;
            tlp_MainGraphic.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlp_MainGraphic.Size = new Size(1728, 797);
            tlp_MainGraphic.TabIndex = 0;
            // 
            // tlp_CurveDetails
            // 
            tlp_CurveDetails.ColumnCount = 1;
            tlp_CurveDetails.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlp_CurveDetails.Dock = DockStyle.Fill;
            tlp_CurveDetails.Location = new Point(1212, 3);
            tlp_CurveDetails.Name = "tlp_CurveDetails";
            tlp_CurveDetails.RowCount = 1;
            tlp_CurveDetails.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            tlp_CurveDetails.Size = new Size(513, 791);
            tlp_CurveDetails.TabIndex = 1;
            // 
            // tlp_Information
            // 
            tlp_Information.ColumnCount = 3;
            tlp_Information.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlp_Information.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tlp_Information.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tlp_Information.Controls.Add(lbl_infos, 0, 0);
            tlp_Information.Dock = DockStyle.Fill;
            tlp_Information.Location = new Point(3, 868);
            tlp_Information.Name = "tlp_Information";
            tlp_Information.RowCount = 1;
            tlp_Information.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlp_Information.Size = new Size(1728, 45);
            tlp_Information.TabIndex = 1;
            // 
            // lbl_infos
            // 
            lbl_infos.AutoSize = true;
            lbl_infos.Dock = DockStyle.Fill;
            lbl_infos.Font = new Font("Roboto", 10F);
            lbl_infos.Location = new Point(3, 0);
            lbl_infos.Name = "lbl_infos";
            lbl_infos.Size = new Size(858, 45);
            lbl_infos.TabIndex = 0;
            lbl_infos.Text = "Information";
            lbl_infos.TextAlign = ContentAlignment.MiddleLeft;
            lbl_infos.Click += lbl_infos_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(button1, 0, 0);
            tableLayoutPanel1.Controls.Add(button2, 1, 0);
            tableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            tableLayoutPanel1.Location = new Point(3, 3);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 1;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(400, 56);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Fill;
            button1.Font = new Font("Roboto", 10.125F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(3, 3);
            button1.Name = "button1";
            button1.Size = new Size(194, 50);
            button1.TabIndex = 2;
            button1.Text = "Selection";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Dock = DockStyle.Fill;
            button2.Font = new Font("Roboto", 10.125F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button2.Location = new Point(203, 3);
            button2.Name = "button2";
            button2.Size = new Size(194, 50);
            button2.TabIndex = 3;
            button2.Text = "Stop";
            button2.UseVisualStyleBackColor = true;
            // 
            // notifyIcon1
            // 
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1734, 916);
            Controls.Add(tlp_global);
            Name = "Main";
            Text = "Graphics Software - V4.0";
            tlp_global.ResumeLayout(false);
            tlp_MainGraphic.ResumeLayout(false);
            tlp_Information.ResumeLayout(false);
            tlp_Information.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlp_global;
        private FlowLayoutPanel flp_curvedetails;
        private Label lbl_infos;
        private System.Windows.Forms.Timer timer1;
        public TableLayoutPanel tlp_CurveDetails;
        public TableLayoutPanel tlp_MainGraphic;
        public TableLayoutPanel tlp_Information;
        private NotifyIcon notifyIcon1;
        private Button button1;
        private TableLayoutPanel tableLayoutPanel1;
        private Button button2;
    }
}

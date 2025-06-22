namespace Graphics_New
{
    partial class CurveDetail
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            tlp__curvedetail = new TableLayoutPanel();
            btn_color = new Button();
            lbl_value = new Label();
            cb_CurveTitle = new CheckBox();
            dialog_curvecolor = new ColorDialog();
            tlp__curvedetail.SuspendLayout();
            SuspendLayout();
            // 
            // tlp__curvedetail
            // 
            tlp__curvedetail.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tlp__curvedetail.ColumnCount = 3;
            tlp__curvedetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            tlp__curvedetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tlp__curvedetail.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12F));
            tlp__curvedetail.Controls.Add(btn_color, 2, 0);
            tlp__curvedetail.Controls.Add(lbl_value, 1, 0);
            tlp__curvedetail.Controls.Add(cb_CurveTitle, 0, 0);
            tlp__curvedetail.Location = new Point(0, 0);
            tlp__curvedetail.Name = "tlp__curvedetail";
            tlp__curvedetail.RowCount = 1;
            tlp__curvedetail.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlp__curvedetail.Size = new Size(678, 77);
            tlp__curvedetail.TabIndex = 0;
            // 
            // btn_color
            // 
            btn_color.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btn_color.Location = new Point(598, 3);
            btn_color.Name = "btn_color";
            btn_color.Size = new Size(77, 71);
            btn_color.TabIndex = 1;
            btn_color.UseVisualStyleBackColor = true;
            btn_color.BackColorChanged += btn_color_BackColorChanged;
            btn_color.Click += btn_color_Click;
            // 
            // lbl_value
            // 
            lbl_value.AutoSize = true;
            lbl_value.Dock = DockStyle.Fill;
            lbl_value.Font = new Font("Roboto", 10.125F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbl_value.Location = new Point(375, 0);
            lbl_value.Name = "lbl_value";
            lbl_value.Size = new Size(217, 77);
            lbl_value.TabIndex = 2;
            lbl_value.Text = "???";
            lbl_value.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cb_CurveTitle
            // 
            cb_CurveTitle.AutoSize = true;
            cb_CurveTitle.Dock = DockStyle.Fill;
            cb_CurveTitle.Font = new Font("Roboto", 10.125F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cb_CurveTitle.Location = new Point(3, 3);
            cb_CurveTitle.Name = "cb_CurveTitle";
            cb_CurveTitle.Padding = new Padding(5, 0, 0, 0);
            cb_CurveTitle.Size = new Size(366, 71);
            cb_CurveTitle.TabIndex = 0;
            cb_CurveTitle.Text = "CurveTitle";
            cb_CurveTitle.UseVisualStyleBackColor = true;
            cb_CurveTitle.CheckedChanged += cb_CurveTitle_CheckedChanged;
            // 
            // CurveDetail
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLight;
            Controls.Add(tlp__curvedetail);
            Name = "CurveDetail";
            Size = new Size(678, 77);
            tlp__curvedetail.ResumeLayout(false);
            tlp__curvedetail.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlp__curvedetail;
        private CheckBox cb_CurveTitle;
        private Button btn_color;
        private Label lbl_value;
        private ColorDialog dialog_curvecolor;
    }
}

namespace SpaceInvaders
{
    partial class GameForm
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
            this.pctShip = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pctShip)).BeginInit();
            this.SuspendLayout();
            // 
            // pctShip
            // 
            this.pctShip.BackgroundImage = global::SpaceInvaders.Properties.Resources.nave_png;
            this.pctShip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pctShip.Location = new System.Drawing.Point(367, 377);
            this.pctShip.Name = "pctShip";
            this.pctShip.Size = new System.Drawing.Size(157, 112);
            this.pctShip.TabIndex = 0;
            this.pctShip.TabStop = false;
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(910, 501);
            this.Controls.Add(this.pctShip);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.Name = "GameForm";
            this.Text = "Space Invaders";
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pctShip)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pctShip;
    }
}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.soundIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.soundIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // soundIcon
            // 
            this.soundIcon.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.soundIcon.BackgroundImage = global::SpaceInvaders.Properties.Resources.sound_icon;
            this.soundIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.soundIcon.Location = new System.Drawing.Point(849, 12);
            this.soundIcon.Name = "soundIcon";
            this.soundIcon.Size = new System.Drawing.Size(49, 42);
            this.soundIcon.TabIndex = 1;
            this.soundIcon.TabStop = false;
            this.soundIcon.Click += new System.EventHandler(this.soundIcon_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(910, 501);
            this.Controls.Add(this.soundIcon);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Transparent;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "GameForm";
            this.Text = "Space Invaders";
            this.Load += new System.EventHandler(this.GameForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.soundIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox soundIcon;
    }
}
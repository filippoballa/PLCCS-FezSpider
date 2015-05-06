namespace ProjectAppBackgroundServer
{
    partial class PictureCamera
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PictureCamera));
            this.TitleOneLabel = new System.Windows.Forms.Label();
            this.ImagePictureBox = new System.Windows.Forms.PictureBox();
            this.TakeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // TitleOneLabel
            // 
            this.TitleOneLabel.AutoSize = true;
            this.TitleOneLabel.BackColor = System.Drawing.Color.Transparent;
            this.TitleOneLabel.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleOneLabel.ForeColor = System.Drawing.Color.White;
            this.TitleOneLabel.Location = new System.Drawing.Point(369, 20);
            this.TitleOneLabel.Name = "TitleOneLabel";
            this.TitleOneLabel.Size = new System.Drawing.Size(297, 23);
            this.TitleOneLabel.TabIndex = 0;
            this.TitleOneLabel.Text = "Take a photo using the Web Camera";
            // 
            // ImagePictureBox
            // 
            this.ImagePictureBox.BackColor = System.Drawing.Color.Transparent;
            this.ImagePictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ImagePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImagePictureBox.Image = ((System.Drawing.Image)(resources.GetObject("ImagePictureBox.Image")));
            this.ImagePictureBox.Location = new System.Drawing.Point(321, 61);
            this.ImagePictureBox.Name = "ImagePictureBox";
            this.ImagePictureBox.Size = new System.Drawing.Size(397, 254);
            this.ImagePictureBox.TabIndex = 1;
            this.ImagePictureBox.TabStop = false;
            // 
            // TakeButton
            // 
            this.TakeButton.BackColor = System.Drawing.Color.LightSeaGreen;
            this.TakeButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.TakeButton.FlatAppearance.BorderSize = 3;
            this.TakeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TakeButton.Font = new System.Drawing.Font("Comic Sans MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TakeButton.ForeColor = System.Drawing.Color.White;
            this.TakeButton.Location = new System.Drawing.Point(452, 336);
            this.TakeButton.Name = "TakeButton";
            this.TakeButton.Size = new System.Drawing.Size(147, 35);
            this.TakeButton.TabIndex = 10;
            this.TakeButton.Text = "Take a Picture";
            this.TakeButton.UseVisualStyleBackColor = false;
            this.TakeButton.Click += new System.EventHandler(this.TakeButton_Click);
            // 
            // PictureCamera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(759, 385);
            this.Controls.Add(this.TakeButton);
            this.Controls.Add(this.ImagePictureBox);
            this.Controls.Add(this.TitleOneLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "PictureCamera";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PictureCamera";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PictureCamera_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TitleOneLabel;
        private System.Windows.Forms.PictureBox ImagePictureBox;
        private System.Windows.Forms.Button TakeButton;
    }
}
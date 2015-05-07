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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PictureCamera));
            this.TitleOneLabel = new System.Windows.Forms.Label();
            this.TakeButton = new System.Windows.Forms.Button();
            this.ImageBox = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).BeginInit();
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
            // TakeButton
            // 
            this.TakeButton.BackColor = System.Drawing.Color.LightSeaGreen;
            this.TakeButton.Cursor = System.Windows.Forms.Cursors.Hand;
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
            // ImageBox
            // 
            this.ImageBox.BackColor = System.Drawing.Color.Transparent;
            this.ImageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageBox.Location = new System.Drawing.Point(358, 70);
            this.ImageBox.Name = "ImageBox";
            this.ImageBox.Size = new System.Drawing.Size(320, 240);
            this.ImageBox.TabIndex = 2;
            this.ImageBox.TabStop = false;
            // 
            // PictureCamera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(739, 385);
            this.Controls.Add(this.ImageBox);
            this.Controls.Add(this.TakeButton);
            this.Controls.Add(this.TitleOneLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "PictureCamera";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PictureCamera";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PictureCamera_FormClosing);
            this.Load += new System.EventHandler(this.PictureCamera_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label TitleOneLabel;
        private System.Windows.Forms.Button TakeButton;
        private Emgu.CV.UI.ImageBox ImageBox;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;

namespace ProjectAppBackgroundServer
{
    public partial class PictureCamera : Form
    {
        private ProjectServerApp proj;
        private Capture capture;

        public PictureCamera( ProjectServerApp proj )
        {
            InitializeComponent();
            this.proj = proj;
            this.capture = new Capture();
        }

        private void TakeButton_Click(object sender, EventArgs e)
        {           
            this.proj.SetPictureBoxImage(this.ImagePictureBox.Image);
            this.Close();
        }

        private void PictureCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( !this.proj.Enabled )
                this.proj.Enabled = true;
        }
    }
}

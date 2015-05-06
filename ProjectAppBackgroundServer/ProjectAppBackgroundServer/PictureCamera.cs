﻿using System;
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
using Emgu.CV.CvEnum;

namespace ProjectAppBackgroundServer
{
    public partial class PictureCamera : Form
    {
        private ProjectServerApp proj;
        private Capture capture;
        private Image<Bgr, Byte> imgUser;
        private CascadeClassifier haarcascade;
        Rectangle rect;

        public PictureCamera( ProjectServerApp proj )
        {
            InitializeComponent();
            this.rect = Rectangle.Empty;
            //this.imgUser = new Image<Bgr, byte>(100, 100);
            this.proj = proj;
            this.haarcascade = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        }

        private void TakeButton_Click(object sender, EventArgs e)
        {
            Image<Bgr, Byte> ImageFrame = capture.QueryFrame();            
            this.imgUser = ImageFrame.Copy(); 
            this.proj.SetPictureBoxImage(this.imgUser.Bitmap);
            this.Close();
        }

        private void PictureCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            if( !this.proj.Enabled )
                this.proj.Enabled = true;
        }

        private void PictureCamera_Load(object sender, EventArgs e)
        {
            try{

                this.capture = new Capture();
                capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 320);
                capture.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 240);
                Application.Idle += ProcessFrame;
            }
            catch (NullReferenceException nullEx){
                MessageBox.Show(nullEx.Message, "ANOMALY", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (TypeInitializationException typeEx) {
                MessageBox.Show(typeEx.Message + ": No camera connected", "ANOMALY", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessFrame( object sender, EventArgs arg ) 
        {
            Image<Bgr, Byte> ImageFrame = capture.QueryFrame();
            
            if (ImageFrame != null) {
                this.rect = DetectFace(ImageFrame);
                ImageFrame.Draw(this.rect, new Bgr(Color.DarkRed), 2);
            }
            
            ImageBox.Image = ImageFrame;           
        }

        public Rectangle DetectFace(Image<Bgr, Byte> ImageFrame) 
        {
            Rectangle r = Rectangle.Empty;
            
            Image<Gray, Byte> grayframe = ImageFrame.Convert<Gray, Byte>();
            Rectangle[] faces = this.haarcascade.DetectMultiScale(grayframe, 1.2, 2, new Size(20, 20), new Size(800, 800));

            foreach ( Rectangle a in faces)
                r = a;         
            
            return r;
        }
    }
}

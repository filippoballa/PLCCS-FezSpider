using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;

namespace FaceRecognizer
{
    public class FaceRecognizer
    {
        private CascadeClassifier HaarCascade { get; set; }
        public Image<Bgr, Byte> FaceExtracted { get; set; }
        public Image<Gray, Byte> MostSimilarFace { get; private set; }
        public int MostSimilarFaceIndex { get; private set; }
        public float MostSimilarFaceDistance { get; private set; }
        public String MostSimilarFaceLabel { get; private set; }
        public double ScaleFactor { get; set; }
        public int MinNeighbors { get; set; }
        public Size MinSize { get; set; }
        public Size MaxSize { get; set; }
        public double Eps { get; set; }
        public int Treshold { get; set; }
        public int LBPHTreshold { get; set; }

        public enum RECOGNIZER {
            PCA_RECOG,
            LBPH_RECOG,
            FISHER_FACE_RECOG
        }

        /// <summary>
        /// FaceRecognizer class constructor
        /// </summary>
        /// <param name="scaleFactor">The factor by which the search window is
        /// scaled between the subsequent scans for the face detection</param>
        /// <param name="minNeighbors">Minimum number (minus 1) of neighbor 
        /// rectangles for the face detection that makes up an object</param>
        /// <param name="minSize">Minimum window size for the face 
        /// detection</param>
        /// <param name="eps">The desired accuracy at which the
        /// iterative algorithm of the face recognition stops</param>
        /// <param name="treshold">The eigen distance treshold for face 
        /// recognition. The smaller the number, the more likely an examined
        /// image will be treated as unrecognized object</param>
        /// <param name="lbphTreshold">Treshold for LBPH face recognition
        /// algorithm</param>
        public FaceRecognizer(double scaleFactor, int minNeighbors,
            Size minSize, Size maxSize, double eps, int treshold,
            int lbphTreshold)
        {
            //file for the Viola-Jones face detector
            HaarCascade = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
            ScaleFactor = scaleFactor;
            MinNeighbors = minNeighbors;
            MinSize = minSize;
            MaxSize = maxSize;
            Eps = eps;
            Treshold = treshold;
            LBPHTreshold = lbphTreshold;
            FaceExtracted = null;
            InitParams();
        }
        /// <summary>
        /// FaceRecognizer constructor with default values
        /// </summary>
        public FaceRecognizer()
        {
            //file for the Viola-Jones face detector
            HaarCascade = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
            ScaleFactor = 1.1;
            MinNeighbors = 2;
            MinSize = new Size(20,20);
            MaxSize = new Size(800,800);
            Eps = 0.001;
            Treshold = 5000;
            LBPHTreshold = 100;
            FaceExtracted = null;
            InitParams();
        }

        private void InitParams()
        {
            MostSimilarFace = null;
            MostSimilarFaceIndex = -1;
            MostSimilarFaceDistance = -1;
            MostSimilarFaceLabel = String.Empty;
        }

        /// <summary>
        /// Face detection based on haar cascade and Viola-Jones algorithm
        /// </summary>
        /// <param name="ImageFrame">The image from which to extract
        /// the face</param>
        /// <returns>A rectangle that defines the region of interess</returns>
        public Rectangle detectFace(Image<Bgr,Byte> ImageFrame)
        {
            InitParams();
            Rectangle roi = Rectangle.Empty;
            if (ImageFrame != null)
            {
                Image<Gray, Byte> grayframe = ImageFrame.Convert<Gray, Byte>().Copy();
                Rectangle[] faces = HaarCascade.DetectMultiScale(grayframe, ScaleFactor, 
                    MinNeighbors,MinSize,MaxSize);
                foreach (Rectangle face in faces)
                {
                    roi = face;
                }     
            }
            return roi;
        }

        /// <summary>
        /// Face recognition based on Priciple Component Analysis (PCA)
        /// classifier using eigenfaces
        /// </summary>
        /// <param name="labels">The set of labels in the training set</param>
        /// <param name="trainingImages">The set of images(faces) in the
        /// training set</param>
        /// <param name="face">The face detected in gray scale
        /// to be recognized. The dimension of the image must be
        /// equal to the dimension of the images in the training set</param>
        /// <returns>A string representing the label of the face recognized
        /// or an empty string if no matches were found</returns>
        public String recognizeEigenFace(List<String> labels, 
            List<Image<Gray, Byte>> trainingImages,
            Image<Bgr, Byte> face)
        {
            String label = String.Empty;
            int numTraining = trainingImages.ToArray().Length;
            InitParams();
            Image<Gray, Byte> extractedFace = face.Convert<Gray, Byte>().Copy().Resize(
                                100, 100, INTER.CV_INTER_CUBIC);
            extractedFace._EqualizeHist();

            if (numTraining != 0)
            {
                //TermCriteria for face recognition with numbers of trained 
                //images like maxIteration
                MCvTermCriteria termCrit = new MCvTermCriteria(numTraining, Eps);

                //Eigen face recognizer
                EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                   trainingImages.ToArray(),
                   labels.ToArray(),
                   Treshold,
                   ref termCrit);
                EigenObjectRecognizer.RecognitionResult r;
                r = recognizer.Recognize(extractedFace);
                if (r != null)
                {
                    label = r.Label;
                    MostSimilarFace = trainingImages[r.Index];
                    MostSimilarFaceIndex = r.Index;
                    MostSimilarFaceDistance = r.Distance;
                    MostSimilarFaceLabel = r.Label;
                }
                else
                {
                    float[] distances = recognizer.GetEigenDistances(extractedFace);
                    int minIndex = 0;
                    float minVal = distances[0];
                    for (int i = 1; i < distances.Length; i++)
                    {
                        if (distances[i] < minVal)
                        {
                            minVal = distances[i];
                            minIndex = i;
                        }
                        MostSimilarFace = trainingImages[minIndex];
                        MostSimilarFaceIndex = minIndex;
                        MostSimilarFaceDistance = minVal;
                        MostSimilarFaceLabel = labels[minIndex];
                    }
                }
            }
 
            return label;
        }

        /// <summary>
        /// Face recognition based on Fisher classifier using eigen
        /// fisher faces</summary>
        /// <param name="labels">The set of labels in the training set</param>
        /// <param name="trainingImages">The set of images(faces) in the
        /// training set</param>
        /// <param name="face">The face detected in gray scale
        /// to be recognized. The dimension of the image must be
        /// equal to the dimension of the images in the training set</param>
        /// <returns>A string representing the label of the face recognized
        /// or an empty string if no matches were found</returns>
        public String recognizeFisherFace(List<String> labels,
           List<Image<Gray, Byte>> trainingImages,
           Image<Bgr, Byte> face)
        {
            String label = String.Empty;
            InitParams();
            Image<Gray, Byte> extractedFace = face.Convert<Gray, Byte>().Copy().Resize(
                                100, 100, INTER.CV_INTER_CUBIC);
            extractedFace._EqualizeHist();

            if (trainingImages.ToArray().Length != 0)
            {
                FisherFaceRecognizer recognizer = new FisherFaceRecognizer(0, Treshold);
                int[] labelsInt = new int[labels.ToArray().Length];
                for (int i = 0; i < labels.ToArray().Length; i++ )
                    labelsInt[i] = i;
                recognizer.Train(trainingImages.ToArray(), labelsInt.ToArray());
                FisherFaceRecognizer.PredictionResult pr;
                pr = recognizer.Predict(extractedFace);
                if (pr.Label != -1)
                {
                    label = labels[pr.Label];
                    MostSimilarFace = trainingImages[pr.Label];
                    MostSimilarFaceIndex = pr.Label;
                    MostSimilarFaceDistance = (float)pr.Distance;
                    MostSimilarFaceLabel = labels[pr.Label];
                }
                else
                {
                    recognizer = new FisherFaceRecognizer(0, 10000);
                    recognizer.Train(trainingImages.ToArray(), labelsInt.ToArray());
                    pr = recognizer.Predict(extractedFace);
                    MostSimilarFace = trainingImages[pr.Label];
                    MostSimilarFaceIndex = pr.Label;
                    MostSimilarFaceDistance = (float)pr.Distance;
                    MostSimilarFaceLabel = labels[pr.Label];
                }
            }
            return label;
        }

        /// <summary>
        /// Face recognition based on Local Binary Pattern Histogram
        /// (LBPH) classifier </summary>
        /// <param name="labels">The set of labels in the training set</param>
        /// <param name="trainingImages">The set of images(faces) in the
        /// training set</param>
        /// <param name="face">The face detected in gray scale
        /// to be recognized. The dimension of the image must be
        /// equal to the dimension of the images in the training set</param>
        /// <returns>A string representing the label of the face recognized
        /// or an empty string if no matches were found</returns>
        public String recognizeLBPHFace(List<String> labels,
           List<Image<Gray, Byte>> trainingImages,
           Image<Bgr,Byte> face)
        {
            String label = String.Empty;
            Image<Gray, Byte> extractedFace = face.Convert<Gray, Byte>().Copy().Resize(
                                100, 100, INTER.CV_INTER_CUBIC);
            extractedFace._EqualizeHist();
            InitParams();

            if (trainingImages.ToArray().Length != 0)
            {
                LBPHFaceRecognizer recognizer = new LBPHFaceRecognizer(
                                                1, 8, 8, 8, LBPHTreshold);
                int[] labelsInt = new int[labels.ToArray().Length];
                for (int i = 0; i < labels.ToArray().Length; i++)
                    labelsInt[i] = i;
                recognizer.Train(trainingImages.ToArray(), labelsInt.ToArray());
                LBPHFaceRecognizer.PredictionResult pr;
                pr = recognizer.Predict(extractedFace);
                if (pr.Label != -1)
                {
                    label = labels[pr.Label];
                    MostSimilarFace = trainingImages[pr.Label];
                    MostSimilarFaceIndex = pr.Label;
                    MostSimilarFaceDistance = (float)pr.Distance;
                    MostSimilarFaceLabel = labels[pr.Label];
                }
                else
                {
                    recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 10000);
                    recognizer.Train(trainingImages.ToArray(), labelsInt.ToArray());
                    pr = recognizer.Predict(extractedFace);
                    MostSimilarFace = trainingImages[pr.Label];
                    MostSimilarFaceIndex = pr.Label;
                    MostSimilarFaceDistance = (float)pr.Distance;
                    MostSimilarFaceLabel = labels[pr.Label];
                }
            }
            return label;
        }

        public String recognize(List<String> labels,
            List<Image<Gray, Byte>> trainingImages,
            Image<Bgr, Byte> face, RECOGNIZER type)
        {
            String label = String.Empty;
            switch (type)
            {
                case RECOGNIZER.LBPH_RECOG:
                    label = recognizeLBPHFace(labels, trainingImages, face);
                    break;
                case RECOGNIZER.PCA_RECOG:
                    label = recognizeEigenFace(labels, trainingImages, face);
                    break;
                case RECOGNIZER.FISHER_FACE_RECOG:
                    label = recognizeFisherFace(labels, trainingImages, face);
                    break;
            }

            return label;
        }

        /// <summary>
        /// Face detection based on haar cascade and Viola-Jones algorithm
        /// </summary>
        /// <param name="ImageFrame">The image from which to extract
        /// the face</param>
        /// <returns>A rectangle that defines the region of interess</returns>
        public Rectangle detectFace(Bitmap ImageFrame)
        {
            InitParams();
            Image<Bgr, Byte> imageEmgu = new Image<Bgr, Byte>(ImageFrame);
            Rectangle roi = Rectangle.Empty;
            if (ImageFrame != null)
            {
                Image<Gray, Byte> grayframe = imageEmgu.Convert<Gray, Byte>().Copy();
                Rectangle[] faces = HaarCascade.DetectMultiScale(grayframe, ScaleFactor,
                    MinNeighbors, MinSize, MaxSize);
                foreach (Rectangle face in faces)
                {
                    roi = face;
                }
            }
            return roi;
        }

        /// <summary>
        /// Face recognition based on Priciple Component Analysis (PCA)
        /// classifier using eigenfaces
        /// </summary>
        /// <param name="labels">The set of labels in the training set</param>
        /// <param name="trainingImages">The set of images(faces) in the
        /// training set</param>
        /// <param name="face">The face detected in gray scale
        /// to be recognized. The dimension of the image must be
        /// equal to the dimension of the images in the training set</param>
        /// <returns>A string representing the label of the face recognized
        /// or an empty string if no matches were found</returns>
        public String recognizeEigenFace(List<String> labels,
            List<Image<Gray, Byte>> trainingImages,
            Bitmap face)
        {
            String label = String.Empty;
            int numTraining = trainingImages.ToArray().Length;
            InitParams();
            Image<Bgr, Byte> imageEmgu = new Image<Bgr, Byte>(face);
            Image<Gray, Byte> extractedFace = imageEmgu.Convert<Gray, Byte>().Copy().Resize(
                                100, 100, INTER.CV_INTER_CUBIC);
            extractedFace._EqualizeHist();

            if (numTraining != 0)
            {
                //TermCriteria for face recognition with numbers of trained 
                //images like maxIteration
                MCvTermCriteria termCrit = new MCvTermCriteria(numTraining, Eps);

                //Eigen face recognizer
                EigenObjectRecognizer recognizer = new EigenObjectRecognizer(
                   trainingImages.ToArray(),
                   labels.ToArray(),
                   Treshold,
                   ref termCrit);
                EigenObjectRecognizer.RecognitionResult r;
                r = recognizer.Recognize(extractedFace);
                if (r != null)
                {
                    label = r.Label;
                    MostSimilarFace = trainingImages[r.Index];
                    MostSimilarFaceIndex = r.Index;
                    MostSimilarFaceDistance = r.Distance;
                    MostSimilarFaceLabel = r.Label;
                }
                else
                {
                    float[] distances = recognizer.GetEigenDistances(extractedFace);
                    int minIndex = 0;
                    float minVal = distances[0];
                    for (int i = 1; i < distances.Length; i++)
                    {
                        if (distances[i] < minVal)
                        {
                            minVal = distances[i];
                            minIndex = i;
                        }
                        MostSimilarFace = trainingImages[minIndex];
                        MostSimilarFaceIndex = minIndex;
                        MostSimilarFaceDistance = minVal;
                        MostSimilarFaceLabel = labels[minIndex];
                    }
                }
            }

            return label;
        }

        /// <summary>
        /// Face recognition based on Fisher classifier using eigen
        /// fisher faces</summary>
        /// <param name="labels">The set of labels in the training set</param>
        /// <param name="trainingImages">The set of images(faces) in the
        /// training set</param>
        /// <param name="face">The face detected in gray scale
        /// to be recognized. The dimension of the image must be
        /// equal to the dimension of the images in the training set</param>
        /// <returns>A string representing the label of the face recognized
        /// or an empty string if no matches were found</returns>
        public String recognizeFisherFace(List<String> labels,
           List<Image<Gray, Byte>> trainingImages,
           Bitmap face)
        {
            String label = String.Empty;
            InitParams();
            Image<Bgr, Byte> imageEmgu = new Image<Bgr, Byte>(face);
            Image<Gray, Byte> extractedFace = imageEmgu.Convert<Gray, Byte>().Copy().Resize(
                                100, 100, INTER.CV_INTER_CUBIC);
            extractedFace._EqualizeHist();

            if (trainingImages.ToArray().Length != 0)
            {
                FisherFaceRecognizer recognizer = new FisherFaceRecognizer(0, Treshold);
                int[] labelsInt = new int[labels.ToArray().Length];
                for (int i = 0; i < labels.ToArray().Length; i++)
                    labelsInt[i] = i;
                recognizer.Train(trainingImages.ToArray(), labelsInt.ToArray());
                FisherFaceRecognizer.PredictionResult pr;
                pr = recognizer.Predict(extractedFace);
                if (pr.Label != -1)
                {
                    label = labels[pr.Label];
                    MostSimilarFace = trainingImages[pr.Label];
                    MostSimilarFaceIndex = pr.Label;
                    MostSimilarFaceDistance = (float)pr.Distance;
                    MostSimilarFaceLabel = labels[pr.Label];
                }
                else
                {
                    recognizer = new FisherFaceRecognizer(0, 10000);
                    recognizer.Train(trainingImages.ToArray(), labelsInt.ToArray());
                    pr = recognizer.Predict(extractedFace);
                    MostSimilarFace = trainingImages[pr.Label];
                    MostSimilarFaceIndex = pr.Label;
                    MostSimilarFaceDistance = (float)pr.Distance;
                    MostSimilarFaceLabel = labels[pr.Label];
                }
            }
            return label;
        }

        /// <summary>
        /// Face recognition based on Local Binary Pattern Histogram
        /// (LBPH) classifier </summary>
        /// <param name="labels">The set of labels in the training set</param>
        /// <param name="trainingImages">The set of images(faces) in the
        /// training set</param>
        /// <param name="face">The face detected in gray scale
        /// to be recognized. The dimension of the image must be
        /// equal to the dimension of the images in the training set</param>
        /// <returns>A string representing the label of the face recognized
        /// or an empty string if no matches were found</returns>
        public String recognizeLBPHFace(List<String> labels,
           List<Image<Gray, Byte>> trainingImages,
           Bitmap face)
        {
            String label = String.Empty;
            Image<Bgr, Byte> imageEmgu = new Image<Bgr, Byte>(face);
            Image<Gray, Byte> extractedFace = imageEmgu.Convert<Gray, Byte>().Copy().Resize(
                                100, 100, INTER.CV_INTER_CUBIC);
            extractedFace._EqualizeHist();
            InitParams();

            if (trainingImages.ToArray().Length != 0)
            {
                LBPHFaceRecognizer recognizer = new LBPHFaceRecognizer(
                                                1, 8, 8, 8, LBPHTreshold);
                int[] labelsInt = new int[labels.ToArray().Length];
                for (int i = 0; i < labels.ToArray().Length; i++)
                    labelsInt[i] = i;
                recognizer.Train(trainingImages.ToArray(), labelsInt.ToArray());
                LBPHFaceRecognizer.PredictionResult pr;
                pr = recognizer.Predict(extractedFace);
                if (pr.Label != -1)
                {
                    label = labels[pr.Label];
                    MostSimilarFace = trainingImages[pr.Label];
                    MostSimilarFaceIndex = pr.Label;
                    MostSimilarFaceDistance = (float)pr.Distance;
                    MostSimilarFaceLabel = labels[pr.Label];
                }
                else
                {
                    recognizer = new LBPHFaceRecognizer(1, 8, 8, 8, 10000);
                    recognizer.Train(trainingImages.ToArray(), labelsInt.ToArray());
                    pr = recognizer.Predict(extractedFace);
                    MostSimilarFace = trainingImages[pr.Label];
                    MostSimilarFaceIndex = pr.Label;
                    MostSimilarFaceDistance = (float)pr.Distance;
                    MostSimilarFaceLabel = labels[pr.Label];
                }
            }
            return label;
        }

        //overload
        public String recognize(List<String> labels,
            List<Bitmap> images,
            Image<Bgr, Byte> face, RECOGNIZER type)
        {
            String label = String.Empty;
            List<Image<Gray, Byte>> trainingImages = new List<Image<Gray, Byte>>();
            foreach (Bitmap bmp in images)
            {
                Image<Gray, Byte> img = new Image<Gray, Byte>(bmp);
                trainingImages.Add(img);
            }

            switch (type)
            {
                case RECOGNIZER.LBPH_RECOG:
                    label = recognizeLBPHFace(labels, trainingImages, face);
                    break;
                case RECOGNIZER.PCA_RECOG:
                    label = recognizeEigenFace(labels, trainingImages, face);
                    break;
                case RECOGNIZER.FISHER_FACE_RECOG:
                    label = recognizeFisherFace(labels, trainingImages, face);
                    break;
            }

            return label;
        }

        //overload
        public String recognize(List<String> labels,
            List<Bitmap> images,
            Bitmap face, RECOGNIZER type)
        {
            String label = String.Empty;
            List<Image<Gray, Byte>> trainingImages = new List<Image<Gray, Byte>>();
            foreach (Bitmap bmp in images)
            {
                Image<Gray, Byte> img = new Image<Gray, Byte>(bmp);
                trainingImages.Add(img);
            }

            switch (type)
            {
                case RECOGNIZER.LBPH_RECOG:
                    label = recognizeLBPHFace(labels, trainingImages, face);
                    break;
                case RECOGNIZER.PCA_RECOG:
                    label = recognizeEigenFace(labels, trainingImages, face);
                    break;
                case RECOGNIZER.FISHER_FACE_RECOG:
                    label = recognizeFisherFace(labels, trainingImages, face);
                    break;
            }

            return label;
        }

        //overload
        public String recognize(List<String> labels,
            List<Bitmap> images,
            Byte[] image, RECOGNIZER type)
        {
            String label = String.Empty;
            Image<Bgr, Byte> face = new Image<Bgr, Byte>(320, 240);
            face.Bytes = image;
            List<Image<Gray, Byte>> trainingImages = new List<Image<Gray, Byte>>();
            foreach (Bitmap bmp in images)
            {
                Image<Gray, Byte> img = new Image<Gray, Byte>(bmp);
                trainingImages.Add(img);
            }

            switch (type)
            {
                case RECOGNIZER.LBPH_RECOG:
                    label = recognizeLBPHFace(labels, trainingImages, face);
                    break;
                case RECOGNIZER.PCA_RECOG:
                    label = recognizeEigenFace(labels, trainingImages, face);
                    break;
                case RECOGNIZER.FISHER_FACE_RECOG:
                    label = recognizeFisherFace(labels, trainingImages, face);
                    break;
            }

            return label;
        }
    }
}

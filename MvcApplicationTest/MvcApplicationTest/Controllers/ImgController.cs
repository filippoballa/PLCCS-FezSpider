using MvcApplicationTest.Models;
using ProjectAppBackgroundServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using HttpMultipartParser;
using FaceRecognizer;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.CvEnum;

namespace MvcApplicationTest.Controllers
{
    public class ImgController : ApiController
    {
        private string LogPath = @"C://MYSITE/LOG/";
        private string strConn = "Data Source=FILIPPO-PC;Initial Catalog=PLCCS_DB;Integrated Security=True";
        public string[] Get(string id = "", string data = "")
        {
            
            return new string[]
                        {
                             id,
                             data
                        };
        }
        public HttpResponseMessage Post()
        {
            string msg = "OK";
            bool login = true;
            HttpResponseMessage response=null;
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.Created;

            DatabaseManagement db = new DatabaseManagement(strConn,LogPath);
            //db.NewErrorLog("Sono entrato e mi faccio un giro!", DateTime.Now);

            try
            {
                string filename = "Img.bmp";
                string path = @"../../../MYSITE/Images/";

                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                //db.NewErrorLog("IP Schedina : "+HttpContext.Current.Request.UserHostAddress.ToString(),DateTime.Now);

                MultipartFormDataParser dataParser = new MultipartFormDataParser(HttpContext.Current.Request.InputStream);

                //Dictionary<string, ParameterPart> dictionary = dataParser.Parameters;
                //db.NewErrorLog("Numero di files : "+dataParser.Files.Count,DateTime.Now);
                List<FilePart> list = dataParser.Files;

                if (list.Count > 0)
                {
                    foreach (FilePart f in list)
                    {
                        Image returnImage = Image.FromStream(f.Data);

                        Bitmap b = new Bitmap(returnImage);

                        //INIZIO RICERCA
                        List<String> labels = new List<String>();
                        List<Bitmap> images = new List<Bitmap>();
                        db.GetFaces(labels, images);
                        if (labels.Count == 0 && images.Count == 0)
                        {
                            //TODO errore no immagini database!
                        }
                        String label = String.Empty;

                        //FaceRecMethod()
                        // Save the image as a BMP.
                        //b.Save(path + f.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        //db.NewErrorLog(f.ToString(), DateTime.Now);
                    }
                }
                else
                {
                    db.NewErrorLog("ANOMALY-no data", DateTime.Now);
                }
                

                /*if (dataStream.Length == 0)
                {
                    msg = "KO no images in the post content";
                    httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                    db.NewErrorLog("ANOMALY-" + msg, DateTime.Now);
                }
                else
                {
                    byte[] buff = new byte[230454];
                    MemoryStream ms = new MemoryStream();
                    int Bread = 0;
                    int TotBread = 0;

                    do
                    {
                        Bread = dataStream.Read(buff, 0, buff.Length);
                        TotBread += Bread;
                        ms.Write(buff, 0, Bread);
                    }
                    while (Bread > 0);

                    Image returnImage = Image.FromStream(ms);

                    Bitmap b = new Bitmap(returnImage);
                    // Save the image as a GIF.
                    b.Save(path+filename, System.Drawing.Imaging.ImageFormat.Bmp);
                    
                    db.NewErrorLog("SUCCESS-" + "new file created", DateTime.Now);

                }
                /*var files =  HttpContext.Current.Request.Files;
                var file = files.Count > 0 ? files[0] : null;

                if (file != null)
                {
                }
                */
                /*MemoryStream ms = new MemoryStream(bytearray);
                Image returnImage = Image.FromStream(ms);//HttpContext.Current.Request.InputStream

                Bitmap b = new Bitmap(returnImage);
                // Save the image as a GIF.
                b.Save(@"C://MYSITE/LOG" + "image.bmp", System.Drawing.Imaging.ImageFormat.Bmp);*/
            }
            catch (DatabaseException e)//DB exception
            {
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                db.NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                msg = e.Mex;
            }
            catch (Exception e)
            {
                //in caso di errore la response diventa http 500 
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                db.NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                msg = e.Message;
            }
            finally
            {
                db.CloseConnection();

                response = Request.CreateResponse<String>(httpStatusCode, msg);

                //aggiungo un messaggio allo status code
                response.ReasonPhrase = msg;
            }
            
            return response;
        }

        private string FaceRecMethod(List<String> labels,List<Bitmap> images, Bitmap face)
        {
            string code="";
            FaceRecognizer.FaceRecognizer rec = new FaceRecognizer.FaceRecognizer();
            rec.MaxSize = new Size(320, 240);
            rec.LBPHTreshold = 65;
            FaceRecognizer.FaceRecognizer.RECOGNIZER type = FaceRecognizer.FaceRecognizer.RECOGNIZER.LBPH_RECOG;
            Rectangle roi = rec.detectFace(face);
            if (roi == Rectangle.Empty)
            {
                code = "No faces detected";
                return code;
            }
            Image<Bgr, Byte> extractedFace = new Image<Bgr, Byte>(face);
            extractedFace.Copy(roi);
            code = rec.recognize(labels, images, extractedFace, type);
            if (code == String.Empty)
            {
                code = "Not recognized";
                return code;
            }
            return code;
        }
    }
}

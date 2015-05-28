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
        DatabaseManagement db = new DatabaseManagement("Data Source=FILIPPO-PC;Initial Catalog=PLCCS_DB;Integrated Security=True", @"C://MYSITE/LOG/");
        string path = @"../../../MYSITE/Images/";

        /// <summary>
        /// Post method for receiving an image. Server receive an HTTP multipart-data message containing a .bmp file used for face recognition.
        /// Metodo Post per la ricezione di un'immagine. Il server riceve un HTTP message di tipo multipart-data che contiene un file .bmp su cui viene effettuato il riconoscimento.
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Post()
        {
            User ricerca = null;
            string msg = "OK";
            //bool login = true;
            HttpResponseMessage response=null;
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.Created;

            //db.NewErrorLog("Sono entrato e mi faccio un giro!", DateTime.Now);

            try
            {
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                //db.NewErrorLog("IP Schedina : "+HttpContext.Current.Request.UserHostAddress.ToString(),DateTime.Now);

                MultipartFormDataParser dataParser = new MultipartFormDataParser(HttpContext.Current.Request.InputStream);

                Dictionary<string, ParameterPart> dictionary = dataParser.Parameters;
                //db.NewErrorLog("Numero di files : "+dataParser.Files.Count,DateTime.Now);
                List<FilePart> list = dataParser.Files;

                if (list.Count > 0)
                {
                    foreach (FilePart f in list)
                    {
                        Image returnImage = Image.FromStream(f.Data);

                        Bitmap b = new Bitmap(returnImage);

                        //INIZIO RICERCA
                        Image<Bgr, byte> img = new Image<Bgr, byte>(b);
                        List<String> labels = new List<String>();
                        List<Bitmap> images = new List<Bitmap>();
                        db.GetFaces(labels, images);

                        if (labels.Count == 0 && images.Count == 0)
                        {
                            //TODO errore no immagini database!
                            msg = "errore no immagini database!";
                            httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                        }
                        else
                        {
                            String result = String.Empty;
                            if (FaceRecMethod(labels, images, b, ref result))
                            {
                                //trovata corrispondenza in db
                                //result contiene label dell'immagine trovata
                                
                                if (db.VerifyUserExists(result))
                                {
                                    //l'utente è registrato
                                    msg = "Welcome "+result+"!";
                                    db.NewErrorLog("result found : " + result, DateTime.Now);

                                    //INSERIMENTO ACCESSO
                                    int codice = Convert.ToInt32(result.Substring(1,result.Length-1));
                                    ricerca = db.SelectSimpleUser(codice);

                                    if (ricerca == null)
                                    {
                                        //non ho trovato nessun utente semplice
                                        ricerca = db.SelectAdministrator(codice);
                                    }

                                    if (ricerca != null)
                                    {
                                        //inserisco un record nella tabella degli ingressi
                                        db.InserAccessUser(ricerca.Codice, DateTime.Now, DatabaseManagement.FACE, ricerca.Img);
                                    }
                                }
                                else
                                {
                                    //trovata corrispondenza con utente non registrato!
                                    //unauthorized 401
                                    msg = "Not registered user!";
                                    db.NewErrorLog("result not registered : "+ result, DateTime.Now);
                                    httpStatusCode = HttpStatusCode.Unauthorized;
                                }
                            }
                            else
                            {
                                //result contiene l'errore incontrato
                                //unauthorized 401
                                msg = result;
                                db.NewErrorLog("no result : " + result, DateTime.Now);
                                httpStatusCode = HttpStatusCode.Unauthorized;
                            }
                        }
                        
                        // Save the image as a BMP.
                        //b.Save(path + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + f.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                        //db.NewErrorLog(f.FileName, DateTime.Now);
                    }
                }
                else
                {
                    db.NewErrorLog("ANOMALY-no data", DateTime.Now);
                }
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
                //db.NewErrorLog(httpStatusCode.ToString()+" MESSAGGIO : "+msg, DateTime.Now);
            }
            
            return response;
        }

        private bool FaceRecMethod(List<String> labels,List<Bitmap> images, Bitmap face,ref String result)
        {
            FaceRecognizer.FaceRecognizer rec = new FaceRecognizer.FaceRecognizer();
            rec.MaxSize = new Size(320, 240);
            rec.LBPHTreshold = 65;
            FaceRecognizer.FaceRecognizer.RECOGNIZER type = FaceRecognizer.FaceRecognizer.RECOGNIZER.LBPH_RECOG;
            Rectangle roi = rec.detectFace(face);
            if (roi == Rectangle.Empty)
            {
                result = "No faces detected";
                return false;
            }
            Image<Bgr, Byte> extractedFace = new Image<Bgr, Byte>(face).Copy(roi);
            result = rec.recognize(labels, images, extractedFace, type);
            if (result == String.Empty)
            {
                db.NewErrorLog("Soglia : " + rec.MostSimilarFaceDistance + " label : " + rec.MostSimilarFaceLabel, DateTime.Now);
                result = "Not recognized";
                return false;
            }
            return true;
        }
    }
}

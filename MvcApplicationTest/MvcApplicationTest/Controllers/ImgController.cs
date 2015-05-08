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

namespace MvcApplicationTest.Controllers
{
    public class ImgController : ApiController
    {
        private string strConn = "Data Source=FILIPPO-PC;Initial Catalog=PLCCS_DB;Integrated Security=True";
        public string[] Get(string id = "", string data = "")
        {
            
            return new string[]
                        {
                             id,
                             data
                        };
        }
        public async Task<HttpResponseMessage>/*HttpResponseMessage Post(Img i)*/Post()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            string msg = "OK";
            bool login = true;
            HttpResponseMessage response=null;
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.Created;

            DatabaseManagement db = new DatabaseManagement(strConn);

            try
            {
                string filename;
                string path;
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    filename = file.Headers.ContentDisposition.FileName;
                    path = file.LocalFileName;
                }

                /*MemoryStream ms = new MemoryStream(i.data);
                Image returnImage = Image.FromStream(ms);

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

                if(httpStatusCode==HttpStatusCode.InternalServerError)
                {
                    response = Request.CreateErrorResponse<String>(httpStatusCode, msg);
                }
                else
                {
                    response = Request.CreateResponse<String>(httpStatusCode, msg);
                }
                

                //aggiungo un messaggio allo status code
                response.ReasonPhrase = msg;
            }
            
            return response;
        }
    }
}

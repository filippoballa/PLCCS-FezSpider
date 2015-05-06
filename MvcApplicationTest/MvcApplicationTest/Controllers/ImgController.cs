using MvcApplicationTest.Models;
using ProjectAppBackgroundServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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
        public HttpResponseMessage Post(Img i)
        {
            string msg = "OK";
            bool login = true;
            HttpResponseMessage response=null;
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.Created;

            DatabaseManagement db = new DatabaseManagement(strConn);

            try
            {
                //riconosciment0   

            }
            catch (DatabaseException e)//DB exception
            {
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                msg = e.Mex;
            }
            catch (Exception e)
            {
                //in caso di errore la response diventa http 500 
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
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
    }
}

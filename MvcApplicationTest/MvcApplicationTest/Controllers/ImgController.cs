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

            DatabaseManagement db = new DatabaseManagement(strConn);

            try
            {
                //User = StartRecognition();
                //if(User!= null){
                response = Request.CreateResponse<String>(System.Net.HttpStatusCode.Created, "Id : " + i.Id);
                //}
                //else{
                //  var response = Request.CreateResponse<String>(System.Net.HttpStatusCode.InternalServerError, "Id : " + i.Id);
                //}
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            finally
            {
                db.CloseConnection();
            }
            
            return response;
        }
    }
}

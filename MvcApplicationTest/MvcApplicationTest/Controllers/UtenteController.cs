using MvcApplicationTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MvcApplicationTest.Controllers
{
    public class UtenteController : ApiController
    {
        public string[] Get(string user = "", string pwd = "")
        {
            return new string[]
                        {
                             user,
                             pwd
                        };
        }

        public HttpResponseMessage Post(Utente u)
        {
            var response = Request.CreateResponse<String>(System.Net.HttpStatusCode.Created, "Id : " + u.Id + "; User : " + u.User + "; Pwd : " + u.Pwd);

            return response;
        }
    }
}

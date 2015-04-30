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
            string user = "123456";
            string pwd = "1234";
            string msg = "OK";
            bool login = true;

            if (u.User == user)
            {
                if (u.Pwd != pwd)
                {
                    msg = "Invalid Password";
                    login = false;
                }
            }
            else
            {
                msg = "Invalid Username";
                login = false;
            }
            HttpResponseMessage response;

            if (login)
            {
                response = Request.CreateResponse<String>(System.Net.HttpStatusCode.Created, "Id : " + u.Id + "; User : " + u.User + "; Pwd : " + u.Pwd);
            }
            else
            {
                response = Request.CreateResponse<String>(System.Net.HttpStatusCode.InternalServerError, msg);
                response.ReasonPhrase = msg;
            }
            
            return response;
        }
    }
}

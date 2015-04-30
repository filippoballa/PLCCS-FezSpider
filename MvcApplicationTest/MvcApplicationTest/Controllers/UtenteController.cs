using MvcApplicationTest.Models;
using ProjectAppBackgroundServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;

namespace MvcApplicationTest.Controllers
{
    public class UtenteController : ApiController
    {
        private string strConn = "Data Source=FILIPPO-PC;Initial Catalog=PLCCS_DB;Integrated Security=True";
        public string[] Get(string user = "", string pwd = "")
        {
            User ricerca = null;
            string msg = "OK";

            //inizializzazione db e ricerca utente in base alla matricola(codice)
            DatabaseManagement db = new DatabaseManagement(strConn);
            try
            {
                int codice = Convert.ToInt32(user);
                ricerca = db.SelectSimpleUser(codice);

                if (ricerca == null)
                {
                    //non ho trovato nessun utente semplice
                    ricerca = db.SelectAdministrator(codice);
                }

                if (ricerca != null)
                {
                    //converto in bytes la stringa da codificare
                    byte[] data = GetBytes(pwd);
                    byte[] result;

                    SHA1 sha1 = new SHA1CryptoServiceProvider();
                    //ottengo lo SHA1 dei dati
                    result = sha1.ComputeHash(data);

                    //confronto lo SHA1 della pwd inviata con lo SHA1 di quella nel DB
                    if (GetString(result) != ricerca.Password)
                    {
                        msg = "Invalid Password";
                    }
                }
                else
                {
                    msg = "Invalid Username";
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            finally
            {
                db.CloseConnection();
            }
            return new string[]
                        {
                             msg
                        };
        }

        public HttpResponseMessage Post(Utente u)
        {
            User ricerca=null;
            string msg = "OK";
            bool login = true;
            HttpResponseMessage response=null;

            //inizializzazione db e ricerca utente in base alla matricola(codice)
            DatabaseManagement db = new DatabaseManagement(strConn);

            try
            {
                int codice = Convert.ToInt32(u.User);
                ricerca = db.SelectSimpleUser(codice);

                if (ricerca == null)
                {
                    //non ho trovato nessun utente semplice
                    ricerca = db.SelectAdministrator(codice);
                }

                if (ricerca != null)
                {
                    //converto in bytes la stringa da codificare
                    byte[] data = GetBytes(u.Pwd);
                    byte[] result;

                    SHA1 sha1 = new SHA1CryptoServiceProvider();
                    //ottengo lo SHA1 dei dati
                    result = sha1.ComputeHash(data);

                    //confronto lo SHA1 della pwd inviata con lo SHA1 di quella nel DB
                    if (GetString(result) != ricerca.Password)
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
                //utente e password corrispondono
                if (login)
                {
                    response = Request.CreateResponse<String>(System.Net.HttpStatusCode.Created, msg);
                }
                //utente o password NON corrispondono
                else
                {
                    response = Request.CreateResponse<String>(System.Net.HttpStatusCode.InternalServerError, msg);
                }

                //aggiungo un messaggio allo status code
                response.ReasonPhrase = msg;
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
        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}

using MvcApplicationTest.Models;
using ProjectAppBackgroundServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Cryptography;
using System.Text;

namespace MvcApplicationTest.Controllers
{
    public class UtenteController : ApiController
    {
        private string LogPath = @"C://MYSITE/LOG/";
        private string strConn = "Data Source=FILIPPO-PC;Initial Catalog=PLCCS_DB;Integrated Security=True";

        /// <summary>
        /// Post method for receiving User/Password. Server receive an HTTP message containing an 'User' object containing the User's code and pin used for authentication.
        /// Metodo Post per la ricezione di una coppia User/Password. Il server riceve un HTTP message che contiene un oggetto 'User' che contiene la matricola e il pin dell'Utente usati per l'autenticazione.
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public HttpResponseMessage Post(Utente u)
        {
            User ricerca=null;
            string msg = "OK";
            bool login = true;
            HttpResponseMessage response=null;
            System.Net.HttpStatusCode httpStatusCode = System.Net.HttpStatusCode.Created;

            //inizializzazione db e ricerca utente in base alla matricola(codice)
            DatabaseManagement db = new DatabaseManagement(strConn,LogPath);

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
                    byte[] data = Encoding.ASCII.GetBytes(u.Pwd);
                    string result;

                    SHA1 sha1 = new SHA1CryptoServiceProvider();
                    //ottengo lo SHA1 dei dati
                    result = Encoding.ASCII.GetString(sha1.ComputeHash(data));

                    //confronto lo SHA1 della pwd inviata con lo SHA1 di quella nel DB
                    if (result != ricerca.Password)
                    {
                        msg = "Invalid Password";
                        httpStatusCode = System.Net.HttpStatusCode.Unauthorized;
                        login = false;
                    }
                }
                else
                {
                    msg = "Invalid Username";
                    httpStatusCode = System.Net.HttpStatusCode.Forbidden;
                    login = false;
                }
                //utente e password corrispondono
                if (login)
                {
                    //inserisco un record nella tabella degli ingressi
                    db.InserAccessUser(ricerca.Codice, DateTime.Now, DatabaseManagement.LOGIN, null);

                    //invio ad ogni amministratore una mail 
                    List<Administrator> AdminList = db.ShowAdministrators();

                    if (AdminList.Count > 0)
                    {
                        foreach (Administrator a in AdminList)
                        {
                            MailManagement mailmanagment = new MailManagement(a.MailAddress, a.MailPassword);
                            mailmanagment.SendMailToAdmin(a.MailAddress, ricerca, DateTime.Now);
                        }
                    }
                }
            }
            catch(DatabaseException e)//DB exception
            {
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                msg = e.Mex;
                db.NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
            }
            catch (Exception e)
            {
                //in caso di errore la response diventa http 500 
                httpStatusCode = System.Net.HttpStatusCode.InternalServerError;
                msg = e.Message;
                db.NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
            }
            finally
            {
                db.CloseConnection();

                response = Request.CreateResponse(httpStatusCode, msg);

                //aggiungo un messaggio allo status code
                response.ReasonPhrase = msg;
            }

            return response;
        }
    }
}

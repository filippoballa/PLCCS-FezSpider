using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime; 
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace ProjectAppBackgroundServer
{
    public class MailManagement
    {
        private string password;
        private MailAddress mittente;
        private string hostType;
        private static string MAILLOG;

        public MailManagement(string address, string pwd, string pathLog) 
        {            
            this.mittente = new MailAddress(address);
            this.password = pwd;
            MailManagement.MAILLOG = pathLog;
            CalculateHost();
        }

        public void SendMailToAdmin( string mailDest, User utente, DateTime date )
        { 
            MailAddress destinatario = new MailAddress(mailDest);
            MailMessage message = new MailMessage(this.mittente, destinatario);

            string filename = MailManagement.MAILLOG + "temp" + utente.Codice.ToString() + ".png";
            utente.Img.Save(filename, ImageFormat.Png);
            
            Attachment att = new Attachment(filename);
            //att.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
            att.ContentDisposition.Inline = true;
            att.ContentId = "imgUser";
            att.ContentType.MediaType = "image/png";
            att.ContentType.Name = filename;
           
            message.Subject = "NOTICE FOR ADMINISTRATOR";
            message.IsBodyHtml = true;
            string aux = "<div>The User " + utente.Name + " " + utente.Surname + " with the following registration number: ";
            aux += utente.Codice + "<br>has entered in the stable without facial recognition. Pay Attention and ";
            aux += "alert security staff!!<br><br> Access occurred at " + date.ToLongTimeString();
            aux += " on the " + date.ToShortDateString();
            aux += "</div><br><br><img src=\"cid:" + att.ContentId + "\"/><br><br>";
            message.Body = aux;
            message.Attachments.Add(att);

            SmtpClient sc = new SmtpClient();
            sc.UseDefaultCredentials = false;
            sc.DeliveryMethod = SmtpDeliveryMethod.Network;
            sc.Host = "smtp." + this.hostType + ".com";
            sc.EnableSsl = true;
            DataEncript de = new DataEncript();
            sc.Credentials = new NetworkCredential(mittente.Address, de.DecryptString(this.password));
            sc.Port = 587;
            sc.Send(message);

            //File.Delete(filename);
            //utente.Img.
        }

        private void CalculateHost()
        {
            int indexOne = this.mittente.Address.IndexOf('@');
            int indexTwo = this.mittente.Address.LastIndexOf('.');
            indexOne++;
            this.hostType = this.mittente.Address.Substring(indexOne, indexTwo - indexOne );

        }

        public static bool VerificaCorrettezzaMail(string addr)
        {
            int index = addr.IndexOf('@');

            if (index < 0)
                return false;

            char c = addr.ElementAt(++index);

            if (c == '.' || Char.IsDigit(c))
                return false;

            string aux = addr.Substring(index);
            index = aux.LastIndexOf('.');

            if (index < 0)
                return false;

            aux = aux.Substring(index);

            if (aux.Length > 4)
                return false;

            return true;
        }
    }
}

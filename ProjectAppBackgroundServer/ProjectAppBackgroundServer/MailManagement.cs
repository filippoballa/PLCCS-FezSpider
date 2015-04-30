﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace ProjectAppBackgroundServer
{
    public class MailManagement
    {
        private string password;
        private MailAddress mittente;
        private string hostType;

        public MailManagement(string address, string pwd) 
        {
            this.mittente = new MailAddress(address);
            this.password = pwd;
            CalculateHost();
        }

        public void SendMailToAdmin( string mailDest, User utente, DateTime date )
        { 
            MailAddress destinatario = new MailAddress(mailDest);
            MailMessage message = new MailMessage(this.mittente, destinatario);

            message.Subject = "NOTICE FOR ADMINISTRATOR";
            string aux = "The User " + utente.Name + " " + utente.Surname + " with the following registration number: ";
            aux += utente.Codice + "\nhas entered in the stable without facial recognition. Pay Attention and ";
            aux += "alert security staff!!\n\n Access occurred at " + date.ToLongTimeString();
            aux += "on the " + date.ToShortDateString();
            message.Body = aux;

            SmtpClient sc = new SmtpClient();
            sc.UseDefaultCredentials = false;
            sc.DeliveryMethod = SmtpDeliveryMethod.Network;
            sc.Host = "smtp." + this.hostType + ".com";
            sc.EnableSsl = true;
            DataEncript de = new DataEncript();
            sc.Credentials = new NetworkCredential(mittente.Address, de.DecryptString(this.password));
            sc.Port = 587;
            sc.Send(message);
        }

        private void CalculateHost() 
        {
            int indexOne = this.mittente.Address.IndexOf('@');
            int indexTwo = this.mittente.Address.LastIndexOf('.');
            int length = this.mittente.Address.Length;
            this.hostType = this.mittente.Address.Substring(indexOne, length - indexTwo);
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
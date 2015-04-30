using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ProjectAppBackgroundServer
{
    public class Administrator : User
    {
        private string mailAddress;
        private string mailPassword;

        public Administrator(int cod, string pass, char gender, DateTime birth, string name, string surname, 
            string Mail, string PwdM, Image img) : base(cod, pass, gender, birth, name, surname,img)
        {
            this.mailAddress = Mail;
            this.mailPassword = PwdM;
        }

        public string MailAddress
        {
            get { return this.mailAddress; }
            set { this.mailAddress = value; }
        }

        public string MailPassword
        {
            get { return this.mailPassword; }
            set { this.mailPassword = value; }
        }
    }
}

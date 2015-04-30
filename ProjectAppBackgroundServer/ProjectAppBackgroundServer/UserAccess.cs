using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ProjectAppBackgroundServer
{
    public class UserAccess
    {
        private int userCode;
        private DateTime timestamp;
        private char accesstype;
        private Image image;

        public UserAccess(int cod, DateTime date, char type) 
        {
            this.userCode = cod;
            this.timestamp = date;
            this.accesstype = type;
            this.image = null;
        }

        public UserAccess(int cod, DateTime date, char type, Image img) 
        {
            this.userCode = cod;
            this.timestamp = date;
            this.accesstype = type;
            this.image = img;
        }

        public int UserCode
        {
            get { return this.userCode; }
            set { this.userCode = value; }
        }

        public char AccessType
        {
            get { return this.accesstype; }
            set { this.accesstype = value; }
        }

        public DateTime Timestamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        public Image Img
        {
            get { return this.image; }
            set { this.image = value; }
        }
    }
}

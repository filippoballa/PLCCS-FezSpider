using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ProjectAppBackgroundServer
{
    public class User
    {
        private int codice;
        private string password;
        private char gender;
        private DateTime birthDate;
        private string name;
        private string surname;
        private Image img;

        public User(int cod, string pwd, char gender, DateTime birth, string name, string surname, Image img) 
        {
            this.codice = cod;
            this.password = pwd;
            this.gender = gender;
            this.birthDate = birth;
            this.name = name;
            this.surname = surname;
            this.img = img;
        }

        public int Codice 
        {
            get { return this.codice; }
            set { this.codice = value; }
        }

        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public string Surname
        {
            get { return this.surname; }
            set { this.surname = value; }
        }

        public char Gender 
        {
            get { return this.gender; }
            set { this.gender = value; }
        }

        public DateTime BirthDate 
        {
            get { return this.birthDate; }
            set { this.birthDate = value; }
        }

        public Image Img
        {
            get { return this.img; }
            set { this.img = value; }
        }

    }
}

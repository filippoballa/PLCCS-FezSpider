﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ProjectAppBackgroundServer
{
    public class DatabaseManagement 
    {
        public const char LOGIN = 'L';
        public const char FACE = 'F';
        public const char ADMIN_YES = 'S';
        public const char ADMIN_NO = 'N';

        private static string STRCONN;
        private static string PATHLOG;
        private SqlConnection conn;

        public DatabaseManagement(string strConn, string pathLog) 
        {   
            DatabaseManagement.STRCONN = strConn;
            DatabaseManagement.PATHLOG = pathLog;
            this.conn = new SqlConnection(DatabaseManagement.STRCONN);
            this.conn.Open();
        }

        /// <summary>
        ///  Il metodo serve per scrivere un messaggio in un file di Log, riguardante un'anomalia 
        ///  che si è verificata durante l'interazione con il database.
        /// </summary>
        /// <param name="mex">
        ///     messaggio da scrivere sul file, dovrebbe trattarsi del messaggio 
        ///     sollevato da un'eccezione
        /// </param>
        /// <param name="date">La data di quando è stata riscontrata l'anomalia</param>
        public void NewErrorLog(string mex, DateTime date) 
        {
            
            string filename = DatabaseManagement.PATHLOG + "LOG_" + date.Day + "_" + date.Month + "_" + date.Year + ".txt";

            StreamWriter writer = new StreamWriter(File.Open(filename, FileMode.Append));
            writer.WriteLine();
            writer.WriteLine();
            writer.Write("--- " + date.ToShortTimeString() + " --- " + mex + " --------------- " + Environment.NewLine);
            writer.Close();
        }

        /// <summary>
        ///  Il metodo inserisce una nuova tupla nella tabella IMAGES_PROJECT
        /// </summary>
        /// <param name="userCode">Codice dell'Utente</param>
        /// <param name="gimg">Byte dell'immagine associata all'utente</param>
        public void InsertImage(int userCode, byte[] gimg) 
        {
            SqlTransaction transaction = this.conn.BeginTransaction();

            try {

                string query = "INSERT INTO IMAGES_PROJECT ( Username, Image) Values(@user, @img)";
                SqlCommand sqlCmd = new SqlCommand(query, this.conn);
                sqlCmd.Transaction = transaction;

                sqlCmd.Parameters.Add("@user", SqlDbType.VarChar, 50).Value = "s" + userCode.ToString();
                sqlCmd.Parameters.Add("@img", SqlDbType.VarBinary, gimg.Length).Value = gimg;

                sqlCmd.ExecuteNonQuery();
                transaction.Commit();

            } catch ( Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }
            
        }

        /// <summary>
        ///  Il metodo inserisce una nuova tupla nella tabella IMAGES_PROJECT
        /// </summary>
        /// <param name="userCode">Codice dell'Utente</param>
        /// <param name="gimg">Byte dell'immagine associata all'utente</param>
        public void InsertImage(string userCode, byte[] gimg)
        {
            SqlTransaction transaction = this.conn.BeginTransaction();

            try {

                string query = "INSERT INTO IMAGES_PROJECT ( Username, Image) Values(@user, @img)";
                SqlCommand sqlCmd = new SqlCommand(query, this.conn);
                sqlCmd.Transaction = transaction;

                sqlCmd.Parameters.Add("@user", SqlDbType.VarChar, 50).Value = userCode;
                sqlCmd.Parameters.Add("@img", SqlDbType.VarBinary, gimg.Length).Value = gimg;

                sqlCmd.ExecuteNonQuery();
                transaction.Commit();

            } catch (Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }

        }

        /// <summary>
        ///  Il metodo inserisce un nuovo record nella tabella ACCESSES_PROJECT
        /// </summary>
        /// <param name="userCode">codice dell'utente</param>
        /// <param name="time">istante di tempo in cui è avvenuto l'accesso</param>
        /// <param name="type">tipo di accesso( 'L' for LOGIN, 'F' for face recognition)</param>
        /// <param name="userImg">
        ///     Immagine con cui l'utente accede allo stabile. Tale dato sarà presente solo
        ///     se l'accesso avviene tramite il riconoscimento facciale.
        /// </param>
        public void InserAccessUser(int userCode, DateTime time, char type, Image userImg)
        {
            SqlTransaction transaction = this.conn.BeginTransaction();

            try {

                string com = "INSERT INTO ACCESSES_PROJECT (Username,DateAccess,TypeOfAccess,ImageAccess)";
                com += "Values(@user,@date,@type,@img)";
                SqlCommand sqlCmd = new SqlCommand(com, this.conn);
                sqlCmd.Transaction = transaction;

                sqlCmd.Parameters.Add("@user", SqlDbType.Int).Value = userCode;
                sqlCmd.Parameters.Add("@date", SqlDbType.DateTime).Value = time;
                sqlCmd.Parameters.Add("type", SqlDbType.NChar, 1).Value = type;

                ImageConverter converter = new ImageConverter();
                byte[] buff = (byte[])converter.ConvertTo(userImg, typeof(byte[]));
                sqlCmd.Parameters.Add("@img", SqlDbType.VarBinary, buff.Length).Value = buff;

                sqlCmd.ExecuteNonQuery();
                transaction.Commit();

            }
            catch ( Exception e ) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>
        ///  Il metodo inserisce un amministratore nella tabella USERS_PROJECT
        /// </summary>
        /// <param name="admin">
        ///     Oggetto di tipo 'Administrator' contenente le informazioni
        ///     associate ad un amministratore.
        /// </param>
        public void InsertAdministrator( Administrator admin ) 
        {
            SqlTransaction transaction = this.conn.BeginTransaction();

            try {

                string com = "INSERT INTO USERS_PROJECT (Username,PasswordLogin,Name,Surname,Gender,";
                com += "BirthDate,Administrator,MailAddress,MailPassword,Image) Values(@user,@pwd,";
                com += "@name,@surname,@gender,@birth,@admin,@addr,@pwdM, @image)";
                SqlCommand sqlCmd = new SqlCommand(com, this.conn);
                sqlCmd.Transaction = transaction;

                sqlCmd.Parameters.Add("@user", SqlDbType.Int).Value = admin.Codice;
                sqlCmd.Parameters.Add("@pwd", SqlDbType.VarChar, 20).Value = admin.Password;
                sqlCmd.Parameters.Add("@name", SqlDbType.VarChar, 50).Value = admin.Name;
                sqlCmd.Parameters.Add("@surname", SqlDbType.VarChar, 50).Value = admin.Surname;
                sqlCmd.Parameters.Add("@gender", SqlDbType.NChar, 1).Value = admin.Gender;
                sqlCmd.Parameters.Add("@admin", SqlDbType.NChar, 1).Value = DatabaseManagement.ADMIN_YES;
                sqlCmd.Parameters.Add("@birth", SqlDbType.Date).Value = admin.BirthDate.ToShortDateString();
                sqlCmd.Parameters.Add("@addr", SqlDbType.VarChar, 50).Value = admin.MailAddress;
                sqlCmd.Parameters.Add("@pwdM", SqlDbType.VarChar, 100).Value = admin.MailPassword;

                ImageConverter converter = new ImageConverter();
                byte[] buff = (byte[])converter.ConvertTo(admin.Img, typeof(byte[]));
                sqlCmd.Parameters.Add("@image", SqlDbType.VarBinary, buff.Length).Value = buff;

                sqlCmd.ExecuteNonQuery();
                transaction.Commit();

            } catch (Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }

        }

        /// <summary>
        ///     Il metodo inserisce un utente semplice nella tabella USERS_PROJECT
        /// </summary>
        /// <param name="u">
        ///     Oggetto di tipo 'User' contenente le informazioni
        ///     associate ad un amministratore.
        /// </param>
        public void InsertSimpleUser( User u ) 
        {
            SqlTransaction transaction = this.conn.BeginTransaction();

            try
            {
                string com = "INSERT INTO USERS_PROJECT (Username,PasswordLogin,Name,Surname,Gender,";
                com += "BirthDate,Administrator,Image) Values(@user,@pwd,";
                com += "@name,@surname,@gender,@birth, @admin,@image)";
                SqlCommand sqlCmd = new SqlCommand(com, this.conn);
                sqlCmd.Transaction = transaction;

                sqlCmd.Parameters.Add("@user", SqlDbType.Int).Value = u.Codice;
                sqlCmd.Parameters.Add("@pwd", SqlDbType.VarChar, 20).Value = u.Password;
                sqlCmd.Parameters.Add("@name", SqlDbType.VarChar, 50).Value = u.Name;
                sqlCmd.Parameters.Add("@surname", SqlDbType.VarChar, 50).Value = u.Surname;
                sqlCmd.Parameters.Add("@gender", SqlDbType.NChar, 1).Value = u.Gender;
                sqlCmd.Parameters.Add("@birth", SqlDbType.Date).Value = u.BirthDate.ToShortDateString();
                sqlCmd.Parameters.Add("@admin", SqlDbType.NChar, 1).Value = DatabaseManagement.ADMIN_NO;

                ImageConverter converter = new ImageConverter();
                byte[] buff = (byte[])converter.ConvertTo(u.Img, typeof(byte[]));
                sqlCmd.Parameters.Add("@image", SqlDbType.VarBinary, buff.Length).Value = buff;

                sqlCmd.ExecuteNonQuery();
                transaction.Commit();

            } catch (Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>
        ///     Il metodo preleva il contenuto di tutta la tabella IMAGES_PROJECT,
        ///     aggiornando le due liste passate come parametri.
        /// </summary>
        /// <param name="liststr">
        ///     Lista che conterrà gli usercode (oggetto string) degli utenti
        /// </param>
        /// <param name="listfaces">
        ///     Lista che conterrà le immagini degli utenti salvate nella tabella.
        /// </param>
        public void GetFaces( List<string> liststr, List<Bitmap> listfaces) 
        {
            string query = "SELECT * FROM IMAGES_PROJECT";
            SqlCommand c1 = new SqlCommand(query, this.conn);
            SqlDataReader reader = c1.ExecuteReader();

            while (reader.Read())
            {
                liststr.Add((string)reader["Username"]);
                byte[] buff = (byte[])reader["Image"];
                Image<Gray, Byte> gimg = new Image<Gray, byte>(100, 100);
                gimg.Bytes = buff;
                Bitmap bmp = new Bitmap(gimg.Bitmap);
                listfaces.Add(bmp);
            }

            reader.Close();
        }

        /// <summary>
        ///     Il metodo ritorna una lista degli Utenti ( elementi di tipo 'User')
        ///     in cui solo i campi 'username' e 'image' sono stati riempiti.
        ///     Tali campi sono riempiti con i dati salvati nella tabella "IMAGES_PROJECT"
        /// </summary>
        public List<User> ShowImages() 
        {
            List<User> list = new List<User>();

            string query = "SELECT * FROM IMAGES_PROJECT";
            SqlCommand c1 = new SqlCommand(query, this.conn);
            SqlDataReader reader = c1.ExecuteReader();
            int a;

            while ( reader.Read() )
            {
                string user = (string)reader["Username"];
                user = user.Substring(1);
                
                if (Int32.TryParse(user, out a)) { 

                    int codice = Convert.ToInt32(user);
                    byte[] buff = (byte[])reader["Image"];
                    /*MemoryStream ms = new MemoryStream(buff);
                    Bitmap bmp = new Bitmap(ms);
                    ms.Close();*/
                    Image<Gray, Byte> gimg = new Image<Gray, byte>(100, 100);
                    gimg.Bytes = buff;
                    Bitmap bmp = new Bitmap(gimg.Bitmap);

                    User nuovo = new User(codice, " ", 'M', DateTime.Now, " ", " ", /*gimg.Bitmap*/bmp );

                    list.Add(nuovo);
                }
            }

            reader.Close();

            return list;
        }

        /// <summary>
        ///     Il metodo restituisce tutti gli amministratori salvati nella tabella USERS_PROJECT
        /// </summary>
        /// <returns>Lista degli Amministratori</returns>
        public List<Administrator> ShowAdministrators() 
        {
            string query = "SELECT * FROM USERS_PROJECT WHERE Administrator='" 
                + DatabaseManagement.ADMIN_YES.ToString() + "'";
            SqlCommand c1 = new SqlCommand(query, this.conn);
            SqlDataReader reader = c1.ExecuteReader();
            List<Administrator> list = new List<Administrator>();

            while (reader.Read()) {
                
                byte[] buff = (byte[])reader["Image"];
                MemoryStream ms = new MemoryStream(buff);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                
                int user = (int)reader["Username"];
                string pwdL = (string)reader["PasswordLogin"];
                string name = (string)reader["Name"];
                string surname = (string)reader["Surname"];
                char gender = Convert.ToChar((string)reader["Gender"]);
                DateTime date = (DateTime)reader["BirthDate"];
                string mail = (string)reader["MailAddress"];
                string pwdM = (string)reader["MailPassword"];

                list.Add(new Administrator(user,pwdL,gender,date,name,surname,mail,pwdM,bmp));

            }

            reader.Close();

            return list;
        }

        /// <summary>
        ///     Il metodo restituisce tutti gli utenti semplici salvati nella tabella USERS_PROJECT   
        /// </summary>
        /// <returns>Lista degli Utenti</returns>
        public List<User> ShowSimpleUsers() 
        {
            string query = "SELECT * FROM USERS_PROJECT WHERE Administrator='" + 
                DatabaseManagement.ADMIN_NO.ToString() + "'";
            SqlCommand c1 = new SqlCommand(query, this.conn);
            SqlDataReader reader = c1.ExecuteReader();
            List<User> list = new List<User>();

            while (reader.Read()) {

                byte[] buff = (byte[])reader["Image"];
                MemoryStream ms = new MemoryStream(buff);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                int user = (int)reader["Username"];
                string pwdL = (string)reader["PasswordLogin"];
                string name = (string)reader["Name"];
                string surname = (string)reader["Surname"];
                char gender = Convert.ToChar((string)reader["Gender"]);
                DateTime date = (DateTime)reader["BirthDate"];                

                list.Add(new User(user, pwdL, gender, date, name, surname, bmp));
            }

            reader.Close();

            return list;
        }

        /// <summary>
        ///     Il metodo verifica la presenza di un certo utente nel database
        /// </summary>
        /// <param name="codice">codice dell'utente da cercare</param>
        /// <returns>true se l'utente è stato trovato, false altrimenti</returns>
        public bool VerifyUserExists(int codice) 
        {
            string query = "SELECT * FROM USERS_PROJECT WHERE Username='" + codice.ToString() + "'";
            SqlCommand c1 = new SqlCommand(query, this.conn);
            SqlDataReader reader = c1.ExecuteReader();
            bool res;

            if (reader.Read()) 
                res = true;
            else
                res = false;

            reader.Close();
            return res;
        }

        /// <summary>
        ///     Il metodo verifica la presenza di un certo utente nel database
        /// </summary>
        /// <param name="codice">codice dell'utente da cercare</param>
        /// <returns>true se l'utente è stato trovato, false altrimenti</returns>
        public bool VerifyUserExists(string codice)
        {
            codice = codice.Substring(1);
            int a, cod;

            if (!Int32.TryParse(codice, out a))
                return false;

            cod = Convert.ToInt32(codice);

            string query = "SELECT * FROM USERS_PROJECT WHERE Username='" + cod + "'";
            SqlCommand c1 = new SqlCommand(query, this.conn);
            SqlDataReader reader = c1.ExecuteReader();
            bool res;

            if (reader.Read())
                res = true;
            else
                res = false;

            reader.Close();
            return res;
        }

        /// <summary>
        ///     Il metodo ritorna, se presente nella tabella USERS_PROJECT, l'utente semplice 
        ///     associato al codice passato come primo parametro.
        /// </summary>
        /// <param name="codice">codice dell'utente da cercare</param>
        /// <returns>
        ///     Se l'utente è stato trovato ritorna un oggetto User completo con tutte le informazioni
        ///     lette dal database, altrimenti il metodo ritorna 'null'. 
        /// </returns>
        public User SelectSimpleUser(int codice) 
        {
            string query = "SELECT * FROM USERS_PROJECT WHERE Administrator='" + 
                DatabaseManagement.ADMIN_NO.ToString() + "' AND Username='" + codice.ToString() + "'";
            SqlCommand com = new SqlCommand(query, this.conn);
            SqlDataReader reader = com.ExecuteReader();

            if (reader.Read()) {

                byte[] buff = (byte[])reader["Image"];
                MemoryStream ms = new MemoryStream(buff);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                int user = (int)reader["Username"];
                string pwdL = (string)reader["PasswordLogin"];
                string name = (string)reader["Name"];
                string surname = (string)reader["Surname"];
                char gender = Convert.ToChar((string)reader["Gender"]);
                DateTime date = (DateTime)reader["BirthDate"];

                User u = new User(user, pwdL, gender, date, name, surname, bmp);
                reader.Close();

                return u;
            }
            else {
                reader.Close();
                return null;
            }

        }

        /// <summary>
        ///     Il metodo ritorna, se presente nella tabella USERS_PROJECT, l'amministratore 
        ///     associato al codice passato come primo parametro.
        /// </summary>
        /// <param name="codice">codice dell'amministratore</param>
        /// <returns>
        ///     Se l'amministratore è stato trovato ritorna un oggetto Administrator completo 
        ///     con tutte le informazioni lette dal database, altrimenti il metodo ritorna 'null'. 
        /// </returns>
        public Administrator SelectAdministrator(int codice) 
        {

            string query = "SELECT * FROM USERS_PROJECT WHERE Administrator='" + 
                DatabaseManagement.ADMIN_YES.ToString() + "' AND Username='" + codice.ToString() + "'";
            SqlCommand com = new SqlCommand(query, this.conn);
            SqlDataReader reader = com.ExecuteReader();

            if (reader.Read())
            {

                byte[] buff = (byte[])reader["Image"];
                MemoryStream ms = new MemoryStream(buff);
                Bitmap bmp = new Bitmap(ms);
                ms.Close();
                int user = (int)reader["Username"];
                string pwdL = (string)reader["PasswordLogin"];
                string name = (string)reader["Name"];
                string surname = (string)reader["Surname"];
                char gender = Convert.ToChar((string)reader["Gender"]);
                DateTime date = (DateTime)reader["BirthDate"];
                string mail = (string)reader["MailAddress"];
                string pwdM = (string)reader["MailPassword"];

                Administrator admin = new Administrator(user, pwdL, gender, date, name, surname, mail, pwdM, bmp);
                reader.Close();

                return admin;
            }
            else {
                reader.Close();
                return null;
            }

        }

        /// <summary>
        ///     Il metodo torna una lista di oggetti UserAccess
        /// </summary>
        public List<UserAccess> ShowAccessUsers() 
        {
            string query = "SELECT * FROM ACCESSES_PROJECT";
            SqlCommand com = new SqlCommand(query, this.conn);
            SqlDataReader reader = com.ExecuteReader();
            List<UserAccess> list = new List<UserAccess>();

            while (reader.Read()) {
                char type = Convert.ToChar((string)reader["TypeOfAccess"]);
                int user = (int)reader["Username"];
                DateTime date = (DateTime)reader["DateAccess"];

                if ( type == DatabaseManagement.FACE ) {
                    byte[] buff = (byte[])reader["ImageAccess"];
                    MemoryStream ms = new MemoryStream(buff);
                    Bitmap bmp = new Bitmap(ms);
                    ms.Close();
                    list.Add( new UserAccess(user,date,type,bmp));
                }
                else 
                    list.Add( new UserAccess(user,date,type));
                              
            }

            reader.Close();

            return list;
        }

        /// <summary>
        ///     Il metodo elimina un utente dalla tabella USERS_PROJECT
        /// </summary>
        /// <param name="usercode">codice dell'utente da cancellare</param>
        public void DeleteUser(int usercode) 
        {
            SqlTransaction transaction = this.conn.BeginTransaction();

            try {

                string query = "DELETE FROM USERS_PROJECT WHERE Username='" + usercode.ToString() + "'";
                SqlCommand com = new SqlCommand(query, this.conn);
                com.Transaction = transaction;
                com.ExecuteNonQuery();
                transaction.Commit();

            } catch (Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>
        ///     Il metodo cancella tutte le immagini di un utente salvate nella tabella IMAGES_PROJECT
        /// </summary>
        /// <param name="usercode">codice dell'utente</param>
        public void DeleteImagesUser(int usercode) 
        {
            SqlTransaction transaction = this.conn.BeginTransaction();

            try {
                string query = "DELETE FROM IMAGES_PROJECT WHERE Username='s" + usercode.ToString() + "'";

                SqlCommand com = new SqlCommand(query, this.conn);
                com.Transaction = transaction;
                com.ExecuteNonQuery();
                transaction.Commit();

            } catch (Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>
        ///     Il seguente metodo cancella tutto il contenuto della tabella ACCESSES_PROJECT
        /// </summary>
        public void DeleteAccessUsers() 
        { 
            SqlTransaction transaction = this.conn.BeginTransaction();

            try {
                string query = "DELETE FROM ACCESSES_PROJECT";

                SqlCommand com = new SqlCommand(query, this.conn);
                com.Transaction = transaction;
                com.ExecuteNonQuery();
                transaction.Commit();

            } catch (Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }

        }

        /// <summary>
        ///     Il metodo cambia il pin ( password Login ) di un utente specifico
        /// </summary>
        /// <param name="codice">Codice dell'utente che si desidera cambiare il PIN</param>
        /// <param name="pin">
        ///     Nuovo PIN ( deve essere stata già cifrato in precedenza con SHA1)
        /// </param>
        public void ChangePIN(int codice, string pin) 
        {
            SqlTransaction transaction = this.conn.BeginTransaction();

            try {

                string query = "UPDATE USERS_PROJECT SET PasswordLogin='" + pin + "' WHERE ";
                query += " Username='" + codice.ToString() + "'";

                SqlCommand com = new SqlCommand(query, this.conn);
                com.Transaction = transaction;

                com.ExecuteNonQuery();
                transaction.Commit();

            } catch (Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>
        ///      Il metodo cambia la password dell'account mail di un amministratore.
        ///      Occorre verificare in precedenza con il metodo "SelectAdministrator" se il codice 
        ///      passato al metodo è quello di un amministratore
        /// </summary>
        /// <param name="codice">Codice dell'amministratore</param>
        /// <param name="mailPwd">
        ///     Nuova Mail Password ( deve essere stata già cifrata in precedenza usando la 
        ///     classe DataEncript )
        /// </param>
        public void ChangeMailPassword(int codice, string mailPwd) 
        {
            SqlTransaction transaction = this.conn.BeginTransaction();

            try {

                string query = "UPDATE USERS_PROJECT SET MailPassword='" + mailPwd + "' WHERE ";
                query += " Username='" + codice.ToString() + "'";

                SqlCommand com = new SqlCommand(query, this.conn);
                com.Transaction = transaction;

                com.ExecuteNonQuery();
                transaction.Commit();

            } catch (Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }
        }

        /// <summary>
        ///     Il metodo aggiorna la tabella USERS_PROJECT con i dati prelevati da un DatagridViewRow
        /// </summary>
        public void UpdateTableUser( User u, DataGridViewRow row ) 
        {

            SqlTransaction transaction = this.conn.BeginTransaction();
            bool trovato = false;

            try {
                string query = "UPDATE USERS_PROJECT SET";

                if (row.Cells[1].Style.BackColor == Color.DarkRed) {
                    trovato = true;
                    query += " Name= '" + (string)row.Cells[1].Value + "'";
                }

                if (row.Cells[2].Style.BackColor == Color.DarkRed) {

                    if (trovato)
                        query += " , Surname= '" + (string)row.Cells[2].Value + "'";
                    else {
                        trovato = true;
                        query += " Surname= '" + (string)row.Cells[2].Value + "'";
                    }
                }


                if (row.Cells[3].Style.BackColor == Color.DarkRed) {

                    if (trovato)
                        query += " , BirthDate=@date";
                    else {
                        trovato = true;
                        query += " BirthDate=@date";
                    }
                }

                if (row.Cells[4].Style.BackColor == Color.DarkRed) {

                    if (trovato)
                        query += " , Image=@img";
                    else {
                        trovato = true;
                        query += " Image=@img";
                    }
                }

                query += " WHERE Username='" + u.Codice.ToString() + "'";

                SqlCommand com = new SqlCommand(query, this.conn);
                com.Transaction = transaction;

                if (row.Cells[3].Style.BackColor == Color.DarkRed)
                    com.Parameters.Add("@birth", SqlDbType.Date).Value = row.Cells[3].Value;

                if (row.Cells[4].Style.BackColor == Color.DarkRed) {
                    ImageConverter converter = new ImageConverter();
                    byte[] buff = (byte[])converter.ConvertTo(row.Cells[4].Value, typeof(byte[]));
                    com.Parameters.Add("@img", SqlDbType.VarBinary, buff.Length).Value = buff;
                }

                com.ExecuteNonQuery();
                transaction.Commit();

            } catch (Exception e) {
                NewErrorLog("ANOMALY-" + e.Message, DateTime.Now);
                transaction.Rollback();
                throw new DatabaseException(e.Message);
            }

        }

        /// <summary>
        ///     Il metodo chiude la connessione al database
        /// </summary>
        public void CloseConnection() 
        {
            this.conn.Close();
        }
    }
}

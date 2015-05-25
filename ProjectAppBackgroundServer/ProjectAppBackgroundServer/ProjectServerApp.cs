using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Threading;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.CvEnum;

namespace ProjectAppBackgroundServer
{
    public partial class ProjectServerApp : Form
    {
        private DatabaseManagement db;
        private Image<Gray, Byte> grayFace;
     
        public ProjectServerApp()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);            
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
            //this.db = new DatabaseManagement("Data Source=FILIPPO-PC;Initial Catalog=PLCCS_DB;Integrated Security=True", Program.LOGDIR); 
            this.db = new DatabaseManagement("Data Source=DAVE-PC\\SQLEXPRESS;Initial Catalog=PAZZODAVEDB;Integrated Security=True", Program.LOGDIR);
            this.openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            this.openFileDialog1.Filter = "Images (*.bmp,*.jpg,*.gif,*.png)|*.png;*.bmp;*.jpg;*.gif";
            this.openFileDialog1.FileName = "";
            this.AdminDataGridView.RowTemplate.Height = 130;
            this.SimpleUserDataGridView.RowTemplate.Height = 130;
            this.ImagesDataGridView.RowTemplate.Height = 140;
            this.AccessDataGridView.RowTemplate.Height = 130;           
            this.ModUserDataGridView.RowTemplate.Height = this.ModUserDataGridView.Height;
                      
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void registerNewUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            this.RegisterPanel.Visible = true;
            this.HomePanel.Visible = false;
            this.DeletePanel.Visible = false;
            this.AdminPanel.Visible = false;
            this.ImagesPanel.Visible = false;
            this.SimpleUserPanel.Visible = false;
            this.InfoPanel.Visible = false;
            this.AccessPanel.Visible = false;
            this.ModifyUserPanel.Visible = false;
            this.NameTextBox.Focus();
            
        }

        private void AdminCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.AdminCheckBox.Checked) {
                this.AdminGroupBox.Visible = true;
                this.MailTextBox.Focus();
            }
            else {
                this.AdminGroupBox.Visible = false;
                this.MailTextBox.Clear();
                this.MailPwdTextBox.Clear();
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            int larg = this.PhotoPictureBox.Width;
            int alt = this.PhotoPictureBox.Height;
            Bitmap face = new Bitmap(Image.FromFile(this.openFileDialog1.FileName), larg, alt);

            Rectangle rect = PictureCamera.DetectFace(new Image<Bgr,Byte>(face));

            if (rect != Rectangle.Empty) {
                this.PhotoPictureBox.BackgroundImage = face;
                this.grayFace = new Image<Gray, byte>(face).Copy(rect).Resize(100,100,INTER.CV_INTER_CUBIC);
                //this.grayFace._EqualizeHist();
                this.PhotoCheckBox.AutoCheck = true;
                this.PhotoCheckBox.Checked = true;
                this.PhotoCheckBox.Enabled = false;
            }
            else 
                MessageBox.Show("The photo is not valid!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            int aux;
            bool errorInsert = false;

            if (this.NameTextBox.Text == "" && this.SurnameTextBox.Text == ""
                && this.PasswordTextBox.Text == "" && UsernameTextBox.Text == "" && !this.PhotoCheckBox.Checked) 
            {
                MessageBox.Show("Complete all fields before continuing, after having\ncompleted them " +
                    "click on the botton Insert!", "NOTICE", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.NameTextBox.Focus();
                return;            
            }

            if (this.NameTextBox.Text == "") {
                MessageBox.Show("Complete the Name field before continuing!!", "NOTICE", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.NameTextBox.Focus();
                return;    
            }

            if (this.SurnameTextBox.Text == "") {
                MessageBox.Show("Complete the Surname field before continuing!!", "NOTICE", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.SurnameTextBox.Focus();
                return;   
            }

            if (this.UsernameTextBox.Text == "" || !Int32.TryParse(this.UsernameTextBox.Text, out aux)
                && this.UsernameTextBox.Text.Length < 6 ) {
                MessageBox.Show("Enter a valid username in the appropriate field!!", "NOTICE", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.UsernameTextBox.Clear();
                this.UsernameTextBox.Focus();
                return;   
            }

            if (this.PasswordTextBox.Text == "" || !Int32.TryParse(this.PasswordTextBox.Text, out aux)
                && this.UsernameTextBox.Text.Length < 4 )
            {
                MessageBox.Show("Enter a valid password in the appropriate field!!", "NOTICE",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.PasswordTextBox.Clear();
                this.UsernameTextBox.Focus();
                return;
            }

            if (this.db.VerifyUserExists(Convert.ToInt32(this.UsernameTextBox.Text))) {
                MessageBox.Show("Already exists in the system, a user with this username code!!", "NOTICE",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!this.PhotoCheckBox.Checked) {
                MessageBox.Show("Select your own pictures before proceeding", "NOTICE",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SHA1 shaM = new SHA1Managed();
            char gender = Convert.ToChar(this.GenderComboBox.Text);
            byte[] pass = Encoding.ASCII.GetBytes(this.PasswordTextBox.Text);
            string hash1 = Encoding.ASCII.GetString(shaM.ComputeHash(pass));

            if (this.AdminCheckBox.Checked) {

                if( this.MailPwdTextBox.Text == "" && this.MailTextBox.Text == "" ) {
                    MessageBox.Show("Enter the credentials of the email account of the administrator!!", "NOTICE",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.MailTextBox.Focus();
                    return;
                }

                if( this.MailTextBox.Text == "" || !MailManagement.VerificaCorrettezzaMail(this.MailTextBox.Text) ) {
                    MessageBox.Show("Enter the correct Mail address of the Administrator!!","NOTICE",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.MailTextBox.Focus();
                    return;
                }

                if( this.MailPwdTextBox.Text == "" ) {
                    MessageBox.Show("Enter tha password associated with the mail of the administrator!!", "NOTICE", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.MailPwdTextBox.Focus();
                    return;
                }

                DataEncript de = new DataEncript();
                Administrator admin = new Administrator(Convert.ToInt32(this.UsernameTextBox.Text), hash1, gender,
                    this.dateTimePicker1.Value, this.NameTextBox.Text, this.SurnameTextBox.Text, this.MailTextBox.Text,
                    de.EncryptString(this.MailPwdTextBox.Text), this.PhotoPictureBox.BackgroundImage);

                try {
                    this.db.InsertAdministrator(admin);
                    this.db.InsertImage(admin.Codice, /*this.grayFace.Bitmap*/ this.grayFace.Bytes);
                    this.MailPwdTextBox.Clear();
                    this.MailTextBox.Clear();
                    this.AdminCheckBox.Checked = false;

                } catch (DatabaseException dbEx) {
                    errorInsert = true;
                    MessageBox.Show(dbEx.Mex, "ANOMALY", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else { 
                User u = new User(Convert.ToInt32(this.UsernameTextBox.Text), hash1, gender,this.dateTimePicker1.Value, 
                    this.NameTextBox.Text, this.SurnameTextBox.Text, this.PhotoPictureBox.BackgroundImage);

                try {
                    this.db.InsertSimpleUser(u);
                    this.db.InsertImage(u.Codice, /*this.grayFace.Bitmap*/this.grayFace.Bytes);
                } catch (DatabaseException dbEx) {
                    errorInsert = true;
                    MessageBox.Show(dbEx.Mex, "ANOMALY", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (!errorInsert) {
                MessageBox.Show("The User has been properly registered!!", "SUCCESS",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.UsernameTextBox.Clear();
                this.SurnameTextBox.Clear();
                this.PhotoPictureBox.BackgroundImage = null;
                this.dateTimePicker1.Value = DateTime.Now;
                this.GenderComboBox.Text = "F";
                this.NameTextBox.Clear();
                this.PasswordTextBox.Clear();
                this.PhotoCheckBox.Checked = false;
                this.UsernameTextBox.Focus();
            }
        }

        private void ProjectServerApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.db.CloseConnection();
        }

        private void showAdministratorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.HomePanel.Visible = false;
            this.RegisterPanel.Visible = false;
            this.DeletePanel.Visible = false;
            this.InfoPanel.Visible = false;
            this.ImagesPanel.Visible = false;
            this.SimpleUserPanel.Visible = false;
            this.AccessPanel.Visible = false;
            this.ModifyUserPanel.Visible = false;
            this.AdminPanel.Visible = true;
        }

        private void AdminPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.AdminPanel.Visible)
            {
                List<Administrator> lad = this.db.ShowAdministrators();

                if (lad.Count > 0)
                {

                    this.AdminDataGridView.Visible = true;

                    for (int i = 0; i < lad.Count; i++)
                    {
                        object[] obj = new object[7];
                        obj[0] = lad[i].Codice.ToString();
                        obj[1] = lad[i].Name;
                        obj[2] = lad[i].Surname;
                        obj[3] = lad[i].Gender.ToString();
                        obj[4] = lad[i].BirthDate.ToShortDateString();
                        obj[5] = lad[i].MailAddress;
                        obj[6] = lad[i].Img;

                        this.AdminDataGridView.Rows.Add(obj);

                    }

                }
                else
                {
                    this.ErrorAdminLabel.Visible = true;
                    this.WarningAdminPictureBox.Visible = true;
                }
            }
            else {
                this.AdminDataGridView.Rows.Clear();
                this.ErrorAdminLabel.Visible = false;
                this.WarningAdminPictureBox.Visible = false;
                this.AdminDataGridView.Visible = false;
            }
            
        }

        private void showSimpleUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.HomePanel.Visible = false;
            this.DeletePanel.Visible = false;
            this.RegisterPanel.Visible = false;
            this.AdminPanel.Visible = false;
            this.InfoPanel.Visible = false;
            this.ImagesPanel.Visible = false;
            this.SimpleUserPanel.Visible = true;
            this.AccessPanel.Visible = false;
            this.ModifyUserPanel.Visible = false;
        }

        private void SimpleUserPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.SimpleUserPanel.Visible) {

                List<User> lad = this.db.ShowSimpleUsers();

                if (lad.Count > 0) {

                    this.SimpleUserDataGridView.Visible = true;

                    for (int i = 0; i < lad.Count; i++) {

                        object[] obj = new object[6];
                        obj[0] = lad[i].Codice.ToString();
                        obj[1] = lad[i].Name;
                        obj[2] = lad[i].Surname;
                        obj[3] = lad[i].Gender.ToString();
                        obj[4] = lad[i].BirthDate.ToShortDateString();
                        obj[5] = lad[i].Img;

                        this.SimpleUserDataGridView.Rows.Add(obj);
                    }
                }
                else {
                    this.ErrorUserLabel.Visible = true;
                    this.WarningUserPictureBox.Visible = true;                    
                }
            }
            else {
                this.SimpleUserDataGridView.Rows.Clear();
                this.ErrorUserLabel.Visible = false;
                this.WarningUserPictureBox.Visible = false;                    
                this.SimpleUserDataGridView.Visible = false;
            }
        }

        private void showAccessUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.HomePanel.Visible = false;
            this.RegisterPanel.Visible = false;
            this.SimpleUserPanel.Visible = false;
            this.ImagesPanel.Visible = false;
            this.DeletePanel.Visible = false;
            this.InfoPanel.Visible = false;
            this.AccessPanel.Visible = true;
            this.AdminPanel.Visible = false;
            this.ModifyUserPanel.Visible = false;
        }

        private void UserPictureBox_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.UserPictureBox, "Enter in the textbox on the left, the \"usercode\" that\n" +
                 "you will use to log on to the stable, when\nthe face recognition failed!!\n" +
                 "The usercode must be composed by six digits!!");
        }

        private void PasswordPictureBox_MouseHover(object sender, EventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.SetToolTip(this.PasswordPictureBox, "Enter in the textbox on the left, the \"password\" that\n" +
                 "you will use to log on to the stable, when\nthe face recognition failed!!\n" + 
                 "The password must be composed by four digits!!");
        }

        private void modifyUserDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.HomePanel.Visible = false;
            this.RegisterPanel.Visible = false;
            this.SimpleUserPanel.Visible = false;
            this.AccessPanel.Visible = false;
            this.DeletePanel.Visible = false;
            this.AdminPanel.Visible = false;
            this.InfoPanel.Visible = false;
            this.ModifyUserPanel.Visible = true;
            this.ImagesPanel.Visible = false;
            this.SearchUserTextBox.Focus();
        }

        private void informazioniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.HomePanel.Visible = false;
            this.RegisterPanel.Visible = false;
            this.SimpleUserPanel.Visible = false;
            this.AccessPanel.Visible = false;
            this.AdminPanel.Visible = false;
            this.DeletePanel.Visible = false;
            this.InfoPanel.Visible = true;
            this.ModifyUserPanel.Visible = false;
            this.ImagesPanel.Visible = false;
        }

        private void AccessPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.AccessPanel.Visible) {

                List<UserAccess> list = this.db.ShowAccessUsers();

                if (list.Count > 0) {
                    this.AccessDataGridView.Visible = true;

                    for( int i = 0; i < list.Count; i++ ) {
                        bool trovato = false;

                        User u = this.db.SelectSimpleUser(list[i].UserCode);
                        Administrator admin = null;

                        if( u == null ) {
                            admin = this.db.SelectAdministrator(list[i].UserCode);
                            trovato = true;
                        }

                        if ( admin == null && u == null ) {
                            this.AccessDataGridView.Rows.Clear();
                            this.AccessDataGridView.Visible = false;
                            this.ErrorAccessLabel.Visible = true;
                            this.WarningAccessPictureBox.Visible = true;
                            return;
                        }

                        object[] obj = new object[6];
                        obj[0] = list[i].UserCode.ToString();
                        obj[3] = list[i].AccessType.ToString(); 
                        obj[4] = list[i].Timestamp.ToShortDateString();
                        obj[5] = list[i].Img;

                        if (trovato) {
                            obj[1] = admin.Name;
                            obj[2] = admin.Surname;
                        }
                        else {
                            obj[1] = u.Name;
                            obj[2] = u.Surname;
                        }

                        this.AccessDataGridView.Rows.Add(obj);
                    }
                }
                else {
                    this.ErrorAccessLabel.Visible = true;
                    this.WarningAccessPictureBox.Visible = true;
                }
            }
            else {
                this.AccessDataGridView.Rows.Clear();
                this.AccessDataGridView.Visible = false;
                this.ErrorAccessLabel.Visible = false;
                this.WarningAccessPictureBox.Visible = false;
            }
        }

        private void CameraButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            PictureCamera pic = new PictureCamera(this, "PIC");
            pic.ShowDialog();             
        }

        public void setGrayFace(Image<Gray, Byte> grayFace) 
        {
            this.grayFace = grayFace;
        }

        public void SetPictureBoxImage(Image img) 
        {
            int larg = this.PhotoPictureBox.Width;
            int alt = this.PhotoPictureBox.Height;
            this.PhotoPictureBox.BackgroundImage = new Bitmap(img, larg, alt);
            this.PhotoCheckBox.AutoCheck = true;
            this.PhotoCheckBox.Checked = true;
            this.PhotoCheckBox.Enabled = false;
            this.Enabled = true;
        }

        private void showImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.HomePanel.Visible = false;
            this.RegisterPanel.Visible = false;
            this.SimpleUserPanel.Visible = false;
            this.AccessPanel.Visible = false;
            this.DeletePanel.Visible = false;
            this.AdminPanel.Visible = false;
            this.InfoPanel.Visible = false;
            this.ModifyUserPanel.Visible = false;
            this.ImagesPanel.Visible = true;
        }

        private void ImagesPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (this.ImagesPanel.Visible)
            {
                List<User> list = this.db.ShowImages();

                if (list.Count > 0) {
                    list = list.OrderBy(x => x.Codice).ToList();
                    this.ImagesDataGridView.Visible = true;

                    for (int i = 0; i < list.Count; i++) {
                        object[] obj = new object[2];
                        obj[0] = list[i].Codice.ToString();
                        obj[1] = list[i].Img;

                        this.ImagesDataGridView.Rows.Add(obj);
                    }


                }
                else {
                    this.ErrorImagesLabel.Visible = true;
                    this.ErrorImagesPictureBox.Visible = true;
                }

            }
            else {
                this.ImagesDataGridView.Rows.Clear();
                this.ImagesDataGridView.Visible = false;
                this.ErrorImagesLabel.Visible = false;
                this.ErrorImagesPictureBox.Visible = false;
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            int a;

            ResetModifyUserPanel();

            if( this.SearchUserTextBox.Text == "" || !Int32.TryParse(this.SearchUserTextBox.Text,out a ) 
                || this.SearchUserTextBox.Text.Length < 6 ) 
            {
                MessageBox.Show("","INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.SearchUserTextBox.Clear();
                this.SearchUserTextBox.Focus();
                return;
            }

            int codice = Convert.ToInt32(this.SearchUserTextBox.Text);

            if( this.TypeComboBox.Text == "Administrator" ) {

                Administrator admin = this.db.SelectAdministrator(codice);

                if( admin == null ) {
                    MessageBox.Show("", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.SearchUserTextBox.Clear();
                    this.SearchUserTextBox.Focus();
                    return;
                }
                else {
                    this.ModUserDataGridView.Visible = true;
                    this.QuestionLabel.Visible = true;
                    this.TakePictureButton.Visible = true;
                    this.SaveButton.Visible = true;
                    object[] obj = new object[5];
                    obj[0] = admin.Codice;
                    obj[1] = admin.Name;
                    obj[2] = admin.Surname;
                    obj[3] = admin.BirthDate.ToShortDateString();
                    obj[4] = admin.Img;
                    this.ModUserDataGridView.Rows.Add(obj);
                }
            }
            else {
                User usr = this.db.SelectSimpleUser(codice);

                if( usr == null ) {
                    MessageBox.Show("","INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.SearchUserTextBox.Clear();
                    this.SearchUserTextBox.Focus();
                    return;
                }
                else {
                    this.ModUserDataGridView.Visible = true;
                    this.QuestionLabel.Visible = true;
                    this.TakePictureButton.Visible = true;
                    this.SaveButton.Visible = true;
                    object[] obj = new object[5];
                    obj[0] = usr.Codice;
                    obj[1] = usr.Name;
                    obj[2] = usr.Surname;
                    obj[3] = usr.BirthDate.ToShortDateString();                   
                    obj[4] = usr.Img;
                    this.ModUserDataGridView.Rows.Add(obj);
                    
                }
            }
        }

        private void ModifyUserPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.ModifyUserPanel.Visible) {
                ResetModifyUserPanel();
            }
        }

        private void ResetModifyUserPanel()
        {            
            this.ModUserDataGridView.Rows.Clear();
            this.ModUserDataGridView.Visible = false;
            this.QuestionLabel.Visible = false;
            this.TakePictureButton.Visible = false;
            this.SaveButton.Visible = false;
        }

        private void ModUserDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (this.ModUserDataGridView.Visible) {
                DataGridViewCell cella = this.ModUserDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cella.Style.BackColor = Color.DarkRed;
                cella.Style.SelectionBackColor = Color.Maroon;
            }
        }

        public void SetDataUserImageGV(Image img) 
        {            
            int larg = this.PhotoPictureBox.Width;
            int alt = this.PhotoPictureBox.Height;
            this.ModUserDataGridView.Rows[0].Cells[4].Value = new Bitmap(img, larg, alt);
            this.Enabled = true;
        }

        private void TakePictureButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            PictureCamera pic = new PictureCamera(this, "DATA");
            pic.ShowDialog();  
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            bool trovato = false;

            foreach (DataGridViewCell cella in this.ModUserDataGridView.Rows[0].Cells) {

                if (cella.Style.BackColor == Color.DarkRed && cella.Style.SelectionBackColor == Color.Maroon) {
                    trovato = true;
                    break;
                }                    
            }

            if (!trovato) {
                MessageBox.Show("", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Bitmap bmp = (Bitmap)this.ModUserDataGridView.Rows[0].Cells[4].Value;
            int codice = Convert.ToInt32(this.ModUserDataGridView.Rows[0].Cells[0].Value);
            User u = this.db.SelectSimpleUser(codice);
            Administrator admin = null;

            if (u == null)
                admin = this.db.SelectAdministrator(codice);

            Rectangle rect = PictureCamera.DetectFace(new Image<Bgr, Byte>(bmp));

            if (rect != Rectangle.Empty) {
                this.grayFace = new Image<Gray, byte>(bmp).Copy(rect).Resize(100, 100, INTER.CV_INTER_CUBIC);
                this.db.InsertImage(codice, this.grayFace.Bytes);
            }

            if( u != null )
                this.db.UpdateTableUser(u,this.ModUserDataGridView.Rows[0]);
            else
                this.db.UpdateTableUser(admin, this.ModUserDataGridView.Rows[0]);

            MessageBox.Show("Information updated successfully!!", "NOTICE",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void deleteUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.RegisterPanel.Visible = false;
            this.HomePanel.Visible = false;
            this.AdminPanel.Visible = false;
            this.ImagesPanel.Visible = false;
            this.SimpleUserPanel.Visible = false;
            this.InfoPanel.Visible = false;
            this.AccessPanel.Visible = false;
            this.ModifyUserPanel.Visible = false;
            this.DeletePanel.Visible = true;
            this.DelCodeTextBox.Focus();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            int aux, usercode;

            if (this.DelCodeTextBox.Text == "" || !Int32.TryParse(this.DelCodeTextBox.Text, out aux)
                && this.DelCodeTextBox.Text.Length < 6)
            {
                MessageBox.Show("Enter a valid username in the appropriate field!!", "NOTICE",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DelCodeTextBox.Clear();
                this.DelCodeTextBox.Focus();
                return;
            }

            usercode = Convert.ToInt32(this.DelCodeTextBox.Text);

            if (!this.db.VerifyUserExists(usercode) ) {
                MessageBox.Show("The code entered does not belong to any user on the system!!", "NOTICE",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DelCodeTextBox.Clear();
                this.DelCodeTextBox.Focus();
                return;
            }

            DialogResult result = MessageBox.Show("Would you like to continue with the cancellation?", "NOTICE",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information );

            if (result == DialogResult.Yes) {
                this.db.DeleteUser(usercode);
                this.db.DeleteImagesUser(usercode);

                if( this.db.VerifyAccessUser(usercode) )
                    this.db.DeleteInformationAccessUser(usercode);

                MessageBox.Show("User information has been deleted successfully!!", "NOTICE", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information );
            }

        }        
    }
}

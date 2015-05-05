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

namespace ProjectAppBackgroundServer
{
    public partial class ProjectServerApp : Form
    {
        private DatabaseManagement db;

        public ProjectServerApp()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);            
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
            this.db = new DatabaseManagement("Data Source=DAVE-PC\\SQLEXPRESS;Initial Catalog=PAZZODAVEDB;Integrated Security=True");
            this.openFileDialog1.InitialDirectory = Environment.CurrentDirectory;
            this.openFileDialog1.Filter = "Images (*.bmp,*.jpg,*.gif,*.png)|*.png;*.bmp;*.jpg;*.gif";
            this.openFileDialog1.FileName = "";
            this.AdminDataGridView.RowTemplate.Height = 130;
            this.SimpleUserDataGridView.RowTemplate.Height = 130;
            
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void registerNewUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            this.RegisterPanel.Visible = true;
            this.HomePanel.Visible = false;
            this.AdminPanel.Visible = false;
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
            this.PhotoPictureBox.Image = new Bitmap(Image.FromFile(this.openFileDialog1.FileName), larg, alt);
            this.PhotoCheckBox.AutoCheck = true;
            this.PhotoCheckBox.Checked = true;
            this.PhotoCheckBox.Enabled = false;
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            int aux;

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
                this.UsernameTextBox.Focus();
                return;   
            }

            if (this.PasswordTextBox.Text == "" || !Int32.TryParse(this.PasswordTextBox.Text, out aux)
                && this.UsernameTextBox.Text.Length < 4 )
            {
                MessageBox.Show("Enter a valid password in the appropriate field!!", "NOTICE",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    de.EncryptString(this.MailPwdTextBox.Text), this.PhotoPictureBox.Image);
                this.db.InsertAdministrator(admin);
                this.MailPwdTextBox.Clear();
                this.MailTextBox.Clear();
                this.AdminCheckBox.Checked = false;
            }
            else { 
                User u = new User(Convert.ToInt32(this.UsernameTextBox.Text), hash1, gender,this.dateTimePicker1.Value, 
                    this.NameTextBox.Text, this.SurnameTextBox.Text, this.PhotoPictureBox.Image);
                this.db.InsertSimpleUser(u);
            }

            MessageBox.Show("The User has been properly registered!!", "SUCCESS", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            this.UsernameTextBox.Clear();
            this.SurnameTextBox.Clear();
            this.PhotoPictureBox.Image = null;
            this.dateTimePicker1.Value = DateTime.Now;
            this.GenderComboBox.Text = "F";
            this.NameTextBox.Clear();
            this.PasswordTextBox.Clear();
            this.UsernameTextBox.Focus();

        }

        private void ProjectServerApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.db.CloseConnection();
        }

        private void showAdministratorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.HomePanel.Visible = false;
            this.RegisterPanel.Visible = false;
            this.InfoPanel.Visible = false;
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
            this.RegisterPanel.Visible = false;
            this.AdminPanel.Visible = false;
            this.InfoPanel.Visible = false;
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
            this.AdminPanel.Visible = false;
            this.InfoPanel.Visible = false;
            this.ModifyUserPanel.Visible = true;
        }

        private void informazioniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.HomePanel.Visible = false;
            this.RegisterPanel.Visible = false;
            this.SimpleUserPanel.Visible = false;
            this.AccessPanel.Visible = false;
            this.AdminPanel.Visible = false;
            this.InfoPanel.Visible = true;
            this.ModifyUserPanel.Visible = false;
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
    }
}

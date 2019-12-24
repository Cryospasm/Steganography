using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Steganography
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        string[] usernames = { "name1", "name2" };
        string[] passwords = { "pass1", "pass2" };

        List<string> user = new List<string>();
        List<string> pass = new List<string>();

        
        private void girisBtn_Click(object sender, EventArgs e)
        {
            if(textBox1.Text=="" || textBox2.Text == "")
            {
                MessageBox.Show("Boş alan bırakmayınız.");
            }
            else if(user.Contains(textBox1.Text) && pass.Contains(textBox2.Text) && Array.IndexOf(user.ToArray(),textBox1.Text)==Array.IndexOf(pass.ToArray(),textBox2.Text))
            {
                Home home = new Home();
                home.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı.");
            }



        }

        private void Giris_Load(object sender, EventArgs e)
        {

            StreamReader sr = new StreamReader(@"user.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string[] components = line.Split("#".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                user.Add(components[0]);
                pass.Add(components[1]);
            }
            sr.Close();
        }
    }
}

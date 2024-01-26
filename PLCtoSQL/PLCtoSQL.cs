using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sharp7;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;

namespace PLCtoSQL
{
    public partial class PLCtoSQL : Form
    {
        
        private PRESS454455 pres454455 = new PRESS454455();
        private Deneme deneme = new Deneme();


        public static string connstring = "Data Source=DESKTOP-BAS2D0N\\SQLEXPRESS;Initial Catalog=master;Integrated Security=True;connect timeout=1";

        public static int sayacinterval = 5000;

        public PLCtoSQL()
        {
            InitializeComponent();
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            
            pres454455.Show();
            

            this.Visible = false;
            
            pres454455.Visible= false;
            
        }

        private void DisconnectBtn_Click(object sender, EventArgs e)
        {

            
            pres454455.Close();
            
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //dosyadanOku();
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        //private void dosyadanOku()
        //{
        //    string dosya_yolu = @"metinbelgesi.txt";
        //    //Okuma işlem yapacağımız dosyanın yolunu belirtiyoruz.
        //    FileStream fs = new FileStream(dosya_yolu, FileMode.Open, FileAccess.Read);
        //    //Bir file stream nesnesi oluşturuyoruz. 1.parametre dosya yolunu,
        //    //2.parametre dosyanın açılacağını,
        //    //3.parametre dosyaya erişimin veri okumak için olacağını gösterir.
        //    StreamReader sw = new StreamReader(fs);
        //    //Okuma işlemi için bir StreamReader nesnesi oluşturduk.
        //    string yazi = sw.ReadLine();
        //    while (yazi != null)
        //    {
        //        //Console.WriteLine(yazi);
        //        yazi = sw.ReadLine();
        //        if (yazi == "close")
        //        {
        //            this.Close();
        //        }
        //        if (yazi == "open")
        //        {
        //            this.Visible = true;
        //        }
        //    }
        //    //Satır satır okuma işlemini gerçekleştirdik ve ekrana yazdırdık
        //    //Son satır okunduktan sonra okuma işlemini bitirdik
        //    sw.Close();
        //    fs.Close();
        //    //İşimiz bitince kullandığımız nesneleri iade ettik.
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void TextError_DoubleClick(object sender, EventArgs e)
        {
            //this.Visible=false;
            
            deneme.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            pres454455.Visible = true;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            pres454455.Visible= false;
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            pres454455.Close();
            deneme.Close();
            
        }

        private void TextError_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

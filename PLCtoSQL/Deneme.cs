using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Sharp7;
using System.Data.SqlClient;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PLCtoSQL
{
    public partial class Deneme : Form
    {

        static bool sistem = false;

        static private string Hata_Durumu = "";
    
        static ThreadStart threadParameters = new ThreadStart(delegate { ReadArea(); });
        static Thread thread2 = new Thread(threadParameters);
        

        static public int result;
  

        static SqlConnection baglanti = new SqlConnection(PLCtoSQL.connstring);
        static SqlCommand komut;
        static SqlDataAdapter da;

        static public S7Client Client;

        static private string[,] tags = new string[100,2];
        static private string[,] tags_guncel = new string[100, 2];
        static private string[,] tags_guncel2 = new string[100, 2];


        
        static DataTable tablo = new DataTable();

        static private string[,] tags_durum = new string[100, 100];

        static DateTime tarih = DateTime.Now;
        static string tarihsql = tarih.ToString("yyyy-MM-dd HH:mm:ss");

        static Veri_Okuma veri_okuma = new Veri_Okuma();
        static IniFile inif = new IniFile("Config.ini");
        public Deneme()
        {
            InitializeComponent();
            Client = new S7Client();
            if (IntPtr.Size == 4)
                this.Text = this.Text + " - Running 32 bit Code";
            else
                this.Text = this.Text + " - Running 64 bit Code";

            
        }

        private void Deneme_Load(object sender, EventArgs e)
        {

            

            timer1.Interval = PLCtoSQL.sayacinterval;
            int Rack = System.Convert.ToInt32(inif.Read("Rack", "PLC"));
            int Slot = System.Convert.ToInt32(inif.Read("Slot", "PLC"));
            Client.ConnectTo(inif.Read("IP", "PLC"), Rack, Slot);

            thread2.Start();

            if (Client.Connected == true)
            {
                TextError.Text = Hata_Durumu + " PDU Negotiated : " + Client.PduSizeNegotiated.ToString();

                sistem = true;
                timer1.Enabled = true;
            }
            if (Client.Connected == false)
            {
                TextError.Text = Hata_Durumu + " Bağlantı Kurulamadı... ";
            }
            
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {

            TextError.Text = Hata_Durumu;

            if (Client.Connected != true)
            {
                timer1.Interval = 30000;
                int Rack = System.Convert.ToInt32(inif.Read("Rack", "PLC"));
                int Slot = System.Convert.ToInt32(inif.Read("Slot", "PLC"));
                Client.ConnectTo(inif.Read("IP", "PLC"), Rack, Slot);
                sistem = false;
                
                TextError.Text = Hata_Durumu + " Bağlantı Koptu... ";

                if (Client.Connected == true)
                {
                    

                    sistem = true;
                    timer1.Interval = PLCtoSQL.sayacinterval;
                    TextError.Text = Hata_Durumu + " PDU Negotiated : " + Client.PduSizeNegotiated.ToString();
                    


                }
            }

            
            
        }
        

        static private void ReadArea()
        {
            while (true)
            {
                while (sistem)
                {
                    int totaltime = 0;

                    int i = 0;
                    
                        //tags[0, 1] = veri_okuma.Read("27","1","1000","1","DB",Client);

                        Console.WriteLine(veri_okuma.Reading_Result());

                    //totaltime = veri_okuma.Reading_Time(); 
                    totaltime = Client.ExecutionTime;
                    

                    Console.WriteLine(totaltime.ToString());
                    
                    tarih = veri_okuma.Read_Time();
                    tarihsql = tarih.ToString("yyyy-MM-dd HH:mm:ss");

                    Hata_Durumu = tarih.ToString() +"  " +Client.ErrorText(veri_okuma.Reading_Result());

                    Thread.Sleep(3000);

                }
                Thread.Sleep(5000);
            }
            
            

        }

        private void ShowResult(int result)
        {
            // This function returns a textual explaination of the error code
            TextError.Text = Client.ErrorText(result);
            if (result == 0)
                TextError.Text = TextError.Text + " (" + Client.ExecutionTime.ToString() + " ms)";
        }

        static private void sql_yazma()
        {

            baglanti.Open();
            da = new SqlDataAdapter("Select TOP(1) *From StoppageLoggings order by Time_Start desc", baglanti);

            tablo.Clear();
            tablo.Columns.Clear();
            da.Fill(tablo);
            baglanti.Close();

            tags_guncel = tags_guncel2;
            
            
            foreach (DataRow row in tablo.Rows)
            {
                tags_durum[0, 1] = row["T_Green"].ToString();
                tags_durum[1, 1] = row["T_Red"].ToString();
                tags_durum[2, 1] = row["TEMP"].ToString();
                tags_durum[3, 1] = row["EQ"].ToString();
                tags_durum[4, 1] = row["SHRT"].ToString();
                tags_durum[5, 1] = row["QUAL"].ToString();
                tags_durum[6, 1] = row["MTCE"].ToString();
                tags_durum[7, 1] = row["MATL"].ToString();
                tags_durum[8, 1] = row["FUL"].ToString();
                tags_durum[9, 1] = row["CONV"].ToString();

                tags_durum[10, 1] = row["Sistem"].ToString();
                tags_durum[11, 1] = row["Time_Start"].ToString();
                tags_durum[12, 1] = row["Line"].ToString();
                
            }

            if (tags_durum[10, 1] == "True")
            {
                for (int i = 0; i < 10; i++)
                {
                    if (tags_guncel[i, 1] != tags_durum[i, 1])
                    {
                        

                        TimeSpan duration;
                        duration = tarih - Convert.ToDateTime(tags_durum[11, 1]);
                        string sorgu = "Update StoppageLoggings Set Time_Stop=@zaman, Duration=@Duration , Sistem=@Sistem " +
                                                        "where Time_Start=@Time_Start";
                        komut = new SqlCommand(sorgu, baglanti);
                        //komut = new OleDbCommand(sorgu, baglanti2);
                        komut.Parameters.AddWithValue("@zaman", tarihsql);
                        komut.Parameters.AddWithValue("@Duration", (int)duration.TotalSeconds);
                        komut.Parameters.AddWithValue("@Sistem", "False");

                        komut.Parameters.AddWithValue("@Time_Start", tags_durum[11, 1]);

                        baglanti.Open();
                        komut.ExecuteNonQuery();
                        baglanti.Close();

                        sorgu = "Insert into StoppageLoggings ( Time_Start, T_Green, T_Red, TEMP, EQ, SHRT, QUAL, MTCE, MATL, FUL, CONV, Stoppage_Detail, Line, Sistem ) " +
                                            "values (@zaman, @T_Green, @T_Red, @TEMP, @EQ, @SHRT, @QUAL, @MTCE, @MATL, @FUL, @CONV, '0', 'C2', 'True')";
                        komut = new SqlCommand(sorgu, baglanti);
                        komut.Parameters.AddWithValue("@zaman", tarihsql);
                        komut.Parameters.AddWithValue("@T_Green", tags_guncel[0, 1].ToString());
                        komut.Parameters.AddWithValue("@T_Red", tags_guncel[1, 1].ToString());
                        komut.Parameters.AddWithValue("@TEMP", tags_guncel[2, 1].ToString());
                        komut.Parameters.AddWithValue("@EQ", tags_guncel[3, 1].ToString());
                        komut.Parameters.AddWithValue("@SHRT", tags_guncel[4, 1].ToString());
                        komut.Parameters.AddWithValue("@QUAL", tags_guncel[5, 1].ToString());
                        komut.Parameters.AddWithValue("@MTCE", tags_guncel[6, 1].ToString());
                        komut.Parameters.AddWithValue("@MATL", tags_guncel[7, 1].ToString());
                        komut.Parameters.AddWithValue("@FUL", tags_guncel[8, 1].ToString());
                        komut.Parameters.AddWithValue("@CONV", tags_guncel[9, 1].ToString());


                        baglanti.Open();
                        komut.ExecuteNonQuery();
                        baglanti.Close();
                        break;
                    }

                }
            }
            if (tags_durum[10, 1] == null || tags_durum[10, 1] == "False")
            {
                
                string sorgu = "Insert into StoppageLoggings ( Time_Start, T_Green, T_Red, TEMP, EQ, SHRT, QUAL, MTCE, MATL, FUL, CONV, Stoppage_Detail, Line, Sistem ) " +
                                            "values (@zaman, @T_Green, @T_Red, @TEMP, @EQ, @SHRT, @QUAL, @MTCE, @MATL, @FUL, @CONV, '0', 'C2', 'True')";
                komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@zaman", tarihsql);
                komut.Parameters.AddWithValue("@T_Green", tags_guncel[0, 1].ToString());
                komut.Parameters.AddWithValue("@T_Red", tags_guncel[1, 1].ToString());
                komut.Parameters.AddWithValue("@TEMP", tags_guncel[2, 1].ToString());
                komut.Parameters.AddWithValue("@EQ", tags_guncel[3, 1].ToString());
                komut.Parameters.AddWithValue("@SHRT", tags_guncel[4, 1].ToString());
                komut.Parameters.AddWithValue("@QUAL", tags_guncel[5, 1].ToString());
                komut.Parameters.AddWithValue("@MTCE", tags_guncel[6, 1].ToString());
                komut.Parameters.AddWithValue("@MATL", tags_guncel[7, 1].ToString());
                komut.Parameters.AddWithValue("@FUL", tags_guncel[8, 1].ToString());
                komut.Parameters.AddWithValue("@CONV", tags_guncel[9, 1].ToString());


                baglanti.Open();
                komut.ExecuteNonQuery();
                baglanti.Close();
            }




        }
        static private void sql_yazma2()
        {
            string sorgu = "Update MachineStat Set time=@zaman, T_Green=@T_Green, T_Red=@T_Red, TEMP=@TEMP, EQ=@EQ, SHRT=@SHRT, QUAL=@QUAL, MTCE=@MTCE, MATL=@MATL, FUL=@FUL, CONV=@CONV " +
                                            "where Line='C2'";
            komut = new SqlCommand(sorgu, baglanti);
            //komut = new OleDbCommand(sorgu, baglanti2);
            komut.Parameters.AddWithValue("@zaman", tarihsql);
            komut.Parameters.AddWithValue("@T_Green", tags_guncel2[0, 1]);
            komut.Parameters.AddWithValue("@T_Red", tags_guncel2[1, 1]);
            komut.Parameters.AddWithValue("@TEMP", tags_guncel2[2, 1]);
            komut.Parameters.AddWithValue("@EQ", tags_guncel2[3, 1]);
            komut.Parameters.AddWithValue("@SHRT", tags_guncel2[4, 1]);
            komut.Parameters.AddWithValue("@QUAL", tags_guncel2[5, 1]);
            komut.Parameters.AddWithValue("@MTCE", tags_guncel2[6, 1]);
            komut.Parameters.AddWithValue("@MATL", tags_guncel2[7, 1]);
            komut.Parameters.AddWithValue("@FUL", tags_guncel2[8, 1]);
            komut.Parameters.AddWithValue("@CONV", tags_guncel2[9, 1]);





            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int Rack = System.Convert.ToInt32(inif.Read("Rack", "PLC"));
            int Slot = System.Convert.ToInt32(inif.Read("Slot", "PLC"));
            result = Client.ConnectTo(inif.Read("IP", "PLC"), Rack, Slot);

            if (Client.Connected == true)
            {
                TextError.Text = Hata_Durumu + " PDU Negotiated : " + Client.PduSizeNegotiated.ToString();
                timer1.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Client.Disconnect();
            timer1.Enabled = false;
        }

        private void Deneme_FormClosed(object sender, FormClosedEventArgs e)
        {
            Client.Disconnect();
            sistem = false;
        }

        private uint BCD5toInt(byte [] bcd)
        {
            uint outInt = 0;
            for (int i = 0; i < bcd.Length; i++)
            {
                int mul = (int)Math.Pow(10, (i * 2));
                outInt += (uint)(((bcd[i] & 0xF)) * mul);
                mul = (int)Math.Pow(10, (i * 2) + 1);
                outInt += (uint)(((bcd[i] >> 4)) * mul);
            }

            return outInt;
        }

        private void Deneme_FormClosing(object sender, FormClosingEventArgs e)
        {
            Client.Disconnect();
            sistem = false;
            thread2.Abort();
        }
    }
}

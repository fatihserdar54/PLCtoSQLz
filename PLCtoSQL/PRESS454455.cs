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
    public partial class PRESS454455 : Form
    {

        static bool sistem = false;
        static private string Hata_Durumu = "";

        static private int target_eff=0;
        static private int capacity_eff = 0;
        static private int plan_eff = 0;
        

        static private int Target_Count_Old = 0;
        static private int Capacity_Count_Old = 0;

        static ThreadStart threadParameters = new ThreadStart(delegate { ReadArea(); });
        static Thread thread2 = new Thread(threadParameters);
        

        static public int result;
        int deneme = 0;

        static SqlConnection baglanti = new SqlConnection(PLCtoSQL.connstring);
        static SqlCommand komut;
        static SqlDataAdapter da;

        static public S7Client Client;

        static private string[,] Durum_Bitleri = new string[400, 2];
        static private int Durum_Bitleri_Count = 0;
        static private string[,] Hat_Degerleri = new string[100, 2];
        static private int Hat_Degerleri_Count = 0;
        static private string[,] Hatalar = new string[2000, 2];
        static private int Hatalar_Count = 0;
        static private string[,] Hat_String = new string[250, 2];
        static private int Hat_String_Count = 0;


        
        static DataTable tablo = new DataTable();

        static private string[,] tags_durum = new string[400, 2];
        static private string[,] shift_durum = new string[400, 2];

        static DateTime tarih = DateTime.Now;
        static string tarihsql = tarih.ToString("yyyy-MM-dd HH:mm:ss");

        static Veri_Okuma veri_okuma = new Veri_Okuma();
        static IniFile inif = new IniFile("Config.ini");
        public PRESS454455()
        {
            InitializeComponent();
            Client = new S7Client();
            if (IntPtr.Size == 4)
                this.Text = this.Text + " - Running 32 bit Code";
            else
                this.Text = this.Text + " - Running 64 bit Code";

            
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader("Durum_Bitleri.txt");
                //Read the first line of text
                
                
                //Continue to read until you reach end of file
                Durum_Bitleri[0, 0] = sr.ReadLine();

                while (Durum_Bitleri[Durum_Bitleri_Count, 0] != null)
                {
                    Durum_Bitleri_Count++;
                    Durum_Bitleri[Durum_Bitleri_Count, 0] = sr.ReadLine();
                }
                
                //close the file
                sr.Close();

                sr = new StreamReader("Hat_Degerleri.txt");

                Hat_Degerleri_Count = 0;
                //Continue to read until you reach end of file
                Hat_Degerleri[0, 0] = sr.ReadLine();

                while (Hat_Degerleri[Hat_Degerleri_Count, 0] != null)
                {
                    Hat_Degerleri_Count++;
                    Hat_Degerleri[Hat_Degerleri_Count, 0] = sr.ReadLine();
                }

                //close the file
                sr.Close();

                sr = new StreamReader("Hatalar.txt");

                Hatalar_Count = 0;
                //Continue to read until you reach end of file
                Hatalar[0, 0] = sr.ReadLine();

                while (Hatalar[Hatalar_Count, 0] != null)
                {
                    Hatalar_Count++;
                    Hatalar[Hatalar_Count, 0] = sr.ReadLine();
                }

                //close the file
                sr.Close();

                sr = new StreamReader("Hat_String.txt");

                Hat_String_Count = 0;
                //Continue to read until you reach end of file
                Hat_String[0, 0] = sr.ReadLine();

                while (Hat_String[Hat_String_Count, 0] != null)
                {
                    Hat_String_Count++;
                    Hat_String[Hat_String_Count, 0] = sr.ReadLine();
                }

                //close the file
                sr.Close();
            }
            catch (Exception f)
            {
                Console.WriteLine("Exception: " + f.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

        private void PRESS454455_Load(object sender, EventArgs e)
        {

            

            timer1.Interval = PLCtoSQL.sayacinterval;
            //int Rack = System.Convert.ToInt32(inif.Read("Rack", "PLC"));
            //int Slot = System.Convert.ToInt32(inif.Read("Slot", "PLC"));
            //result = Client.ConnectTo(inif.Read("IP", "PLC"), Rack, Slot);

            thread2.Start();

            //if (Client.Connected == true)
            //{
            //    TextError.Text = Hata_Durumu + " PDU Negotiated : " + Client.PduSizeNegotiated.ToString();


            //    baglanti.Open();
            //    da = new SqlDataAdapter("Select TOP(1) *From StoppageLoggings order by ID desc", baglanti);

            //    tablo.Clear();
            //    tablo.Columns.Clear();
            //    da.Fill(tablo);
            //    baglanti.Close();

            //    string kontrol="False";
            //    string zamankontrol="False";

            //    foreach (DataRow row in tablo.Rows)
            //    {
            //        kontrol = row["Sistem"].ToString();
            //        zamankontrol = row["Time_Start"].ToString();
            //    }


            //    if (kontrol == "True")
            //    {
            //        DateTime tarih = DateTime.Now;
            //        TimeSpan duration;
            //        duration = tarih - Convert.ToDateTime(zamankontrol);

            //        string sorgu = "Update StoppageLoggings Set Time_Stop=@zaman, Duration=@Duration , Sistem=@Sistem , Machine=@Machine " +
            //                                        "where Time_Start=@Time_Start";
            //        komut = new SqlCommand(sorgu, baglanti);
            //        //komut = new OleDbCommand(sorgu, baglanti2);
            //        komut.Parameters.AddWithValue("@zaman", tarihsql);
            //        komut.Parameters.AddWithValue("@Duration", (int)duration.TotalSeconds);
            //        komut.Parameters.AddWithValue("@Sistem", "False");
            //        komut.Parameters.AddWithValue("@Machine", "Fault");
            //        komut.Parameters.AddWithValue("@Time_Start", zamankontrol);

            //        baglanti.Open();
            //        komut.ExecuteNonQuery();
            //        baglanti.Close();




            //    }

            //    baglanti.Open();
            //    da = new SqlDataAdapter("Select TOP(1) *From ShiftLogging order by ID desc", baglanti);

            //    tablo.Clear();
            //    tablo.Columns.Clear();
            //    da.Fill(tablo);
            //    baglanti.Close();

            //    kontrol = "False";
            //    zamankontrol = "False";

            //    foreach (DataRow row in tablo.Rows)
            //    {
            //        kontrol = row["Sistem"].ToString();
            //        zamankontrol = row["Time_Start"].ToString();
            //    }


            //    if (kontrol == "True")
            //    {
            //        DateTime tarih = DateTime.Now;
            //        TimeSpan duration;
            //        duration = tarih - Convert.ToDateTime(zamankontrol);

            //        string sorgu = "Update ShiftLogging Set Time_Stop=@zaman, Duration=@Duration, Sistem=@Sistem, Machine=@Machine " +
            //                                        "where Time_Start=@Time_Start";
            //        komut = new SqlCommand(sorgu, baglanti);
            //        //komut = new OleDbCommand(sorgu, baglanti2);
            //        komut.Parameters.AddWithValue("@zaman", tarihsql);
            //        komut.Parameters.AddWithValue("@Duration", (int)duration.TotalSeconds);
            //        komut.Parameters.AddWithValue("@Sistem", "False");
            //        komut.Parameters.AddWithValue("@Machine", "Fault");

            //        komut.Parameters.AddWithValue("@Time_Start", zamankontrol);

            //        baglanti.Open();
            //        komut.ExecuteNonQuery();
            //        baglanti.Close();




            //    }



            //    sistem = true;
            //    
            //}

            //if (Client.Connected == false)
            //{
            //    TextError.Text = Hata_Durumu + " Bağlantı Kurulamadı... ";
            //}

            timer1.Enabled = true;
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {

            TextError.Text = Hata_Durumu;

            if (Client.Connected != true)
            {
                timer1.Interval = 30000;
                int Rack = System.Convert.ToInt32(inif.Read("Rack", "PLC"));
                int Slot = System.Convert.ToInt32(inif.Read("Slot", "PLC"));
                result = Client.ConnectTo(inif.Read("IP", "PLC"), Rack, Slot);
                sistem = false;

                TextError.Text = Hata_Durumu + " Bağlantı Koptu... ";


                if (Client.Connected == true)
                {
                    baglanti.Open();
                    da = new SqlDataAdapter("Select TOP(1) *From StoppageLoggings order by ID desc", baglanti);

                    tablo.Clear();
                    tablo.Columns.Clear();
                    da.Fill(tablo);
                    baglanti.Close();

                    string kontrol = "False";
                    string zamankontrol = "False";

                    foreach (DataRow row in tablo.Rows)
                    {
                        kontrol = row["Sistem"].ToString();
                        zamankontrol = row["Time_Start"].ToString();
                    }


                    if (kontrol == "True")
                    {
                        DateTime tarih = DateTime.Now;
                        TimeSpan duration;
                        duration = tarih - Convert.ToDateTime(zamankontrol);

                        string sorgu = "Update StoppageLoggings Set Time_Stop=@zaman, Duration=@Duration , Sistem=@Sistem , Machine=@Machine " +
                                                        "where Time_Start=@Time_Start";
                        komut = new SqlCommand(sorgu, baglanti);
                        //komut = new OleDbCommand(sorgu, baglanti2);
                        komut.Parameters.AddWithValue("@zaman", tarihsql);
                        komut.Parameters.AddWithValue("@Duration", (int)duration.TotalSeconds);
                        komut.Parameters.AddWithValue("@Sistem", "False");
                        komut.Parameters.AddWithValue("@Machine", "Fault");
                        komut.Parameters.AddWithValue("@Time_Start", zamankontrol);

                        baglanti.Open();
                        komut.ExecuteNonQuery();
                        baglanti.Close();




                    }



                    if (true)
                    {
                        DateTime tarih = DateTime.Now;
                        TimeSpan duration;
                        duration = tarih - Convert.ToDateTime(zamankontrol);

                        string sorgu = "Update ShiftLogging Set Time_Stop=@zaman, Duration=@Duration, Sistem=@Sistem, Machine=@Machine " +
                                                        "where Sistem = 'True'";
                        komut = new SqlCommand(sorgu, baglanti);
                        //komut = new OleDbCommand(sorgu, baglanti2);
                        komut.Parameters.AddWithValue("@zaman", tarihsql);
                        komut.Parameters.AddWithValue("@Duration", (int)duration.TotalSeconds);
                        komut.Parameters.AddWithValue("@Sistem", "False");
                        komut.Parameters.AddWithValue("@Machine", "Fault");

                        komut.Parameters.AddWithValue("@Time_Start", zamankontrol);


                        baglanti.Open();
                        komut.ExecuteNonQuery();
                        baglanti.Close();
                    }

                    if (true)
                    {
                        string zaman_aralik_1 = "";
                        string zaman_aralik_2 = "";
                        if (tarih.Hour < 8 && tarih.Hour >= 0)
                        {
                            zaman_aralik_1 = tarih.Date.ToString("yyyy-MM-dd") + " 00:00:00";
                            zaman_aralik_2 = tarih.Date.ToString("yyyy-MM-dd") + " 08:00:00";
                            shift_durum[8, 1] = "1";
                        }
                        if (tarih.Hour >= 8 && tarih.Hour < 16)
                        {
                            zaman_aralik_1 = tarih.Date.ToString("yyyy-MM-dd") + " 08:00:00";
                            zaman_aralik_2 = tarih.Date.ToString("yyyy-MM-dd") + " 16:00:00";
                            shift_durum[8, 1] = "2";
                        }
                        if (tarih.Hour >= 16)
                        {
                            zaman_aralik_1 = tarih.Date.ToString("yyyy-MM-dd") + " 16:00:00";
                            zaman_aralik_2 = tarih.Date.ToString("yyyy-MM-dd") + " 23:59:59";
                            shift_durum[8, 1] = "3";
                        }


                        baglanti.Open();
                        da = new SqlDataAdapter("Select TOP(1) *From ShiftLogging where Machine IS NULL and Shift=" + shift_durum[8, 1] + " and Time_Start between '" + zaman_aralik_1 + "' and '" + zaman_aralik_2 + "'  order by ID desc", baglanti);
                        DataTable tablo = new DataTable();
                        tablo.Clear();
                        tablo.Columns.Clear();
                        da.Fill(tablo);
                        baglanti.Close();

                        kontrol = "False";
                        zamankontrol = "False";
                        int degisken = 0;

                        foreach (DataRow row in tablo.Rows)
                        {

                            Target_Count_Old = Convert.ToInt32(row["Target_Count"]);

                        }
                    }
                    



                    sistem = true;
                    timer1.Interval = PLCtoSQL.sayacinterval;
                    TextError.Text = Hata_Durumu + " PDU Negotiated : " + Client.PduSizeNegotiated.ToString();
                    


                }
            }

            
            
        }
        
        private void ReadAreas()
        {
            
            //if (result == 0)
            //{

            //    M1260M = BitConverter.ToString(M1260MArray);

            //}
            //if (result == 0)
            //{
            //    sql_yazma();
            //    sql_yazma2();
            //}



            //label18.Text = SizeRead.ToString();

            //HexDump(TxtDump, Buffer, SizeRead);
        }

        static private void ReadArea()
        {
            while (true)
            {
                while (sistem)
                {
                    
                    byte[] buffer = new byte[1000];

                    buffer = veri_okuma.Read(inif.Read("DBNumber", "PLC"), 1000, Client);

                    tarih = veri_okuma.Read_Time();
                    tarihsql = tarih.ToString("yyyy-MM-dd HH:mm:ss");
                    Console.WriteLine(Client.ExecutionTime);

                    int i = 0;
                    while (i < Durum_Bitleri_Count)
                    {
                        for (int j=0; j < 50; j++)
                        {
                            for (int k = 0; k < 8; k++)
                            {

                                Durum_Bitleri[i, 1] = S7.GetBitAt(buffer, j, k).ToString();
                                
                                i++;
                                if (Durum_Bitleri[i, 0] == null)
                                {
                                    break;
                                }
                            }

                            if (Durum_Bitleri[i, 0] == null)
                            {
                                break;
                            }
                        }

                        if (Durum_Bitleri[i, 0] == null)
                        {
                            break;
                        }
                    }
                


                    i = 0;
                    while (i < Hat_Degerleri_Count)
                    {

                        Hat_Degerleri[i, 1] = S7.GetIntAt(buffer,i*2+50).ToString();

                        i++;
                    }

                    i = 0;

                    while (i < Hatalar_Count)
                    {
                        for (int j = 0; j < 50; j++)
                        {
                            for (int k = 0; k < 8; k++)
                            {

                                Hatalar[i, 1] = S7.GetBitAt(buffer, j+250, k).ToString();
                                
                                i++;
                                if (Hatalar[i, 0] == null)
                                {
                                    break;
                                }
                            }

                            if (Hatalar[i, 0] == null)
                            {
                                break;
                            }
                        }

                        if (Hatalar[i, 0] == null)
                        {
                            break;
                        }
                    }

                    i = 0;
                    while (i < Hat_String_Count)
                    {

                        Hat_String[i, 1] = S7.GetWordAt(buffer,i*2+500).ToString();

                        
                        
                        i++;
                    }


                    

                    Hata_Durumu = tarih.ToString() + "  " + Client.ErrorText(veri_okuma.Reading_Result());

                    Sql_StoppageLogging();
                    Sql_MachineStat();
                    Sql_ShiftLogging();


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

        static private bool shift_start=false;

        static private void Sql_ShiftLogging()
        {
            TimeSpan fark = new TimeSpan();
            if (tarih.Hour >= 8 && tarih.Hour < 16)
            {
                fark = tarih.TimeOfDay - Convert.ToDateTime("2024-01-01 08:00:00").TimeOfDay;
            }
            if (tarih.Hour >= 16 && tarih.Hour < 24)
            {
                fark = tarih.TimeOfDay - Convert.ToDateTime("2024-01-01 16:00:00").TimeOfDay;
            }
            if (tarih.Hour >= 0 && tarih.Hour < 8)
            {
                fark = tarih.TimeOfDay - Convert.ToDateTime("2024-01-01 00:00:00").TimeOfDay;
            }

            int stoptimes = (int)fark.TotalMinutes - Convert.ToInt32(Hat_Degerleri[1, 1]);

            if (true)
            {
                string zaman_aralik_1 = "";
                string zaman_aralik_2 = "";
                if (tarih.Hour < 8 && tarih.Hour >= 0)
                {
                    zaman_aralik_1 = tarih.Date.ToString("yyyy-MM-dd") + " 00:00:00";
                    zaman_aralik_2 = tarih.Date.ToString("yyyy-MM-dd") + " 08:00:00";
                    shift_durum[8, 1] = "1";
                }
                if (tarih.Hour >= 8 && tarih.Hour < 16)
                {
                    zaman_aralik_1 = tarih.Date.ToString("yyyy-MM-dd") + " 08:00:00";
                    zaman_aralik_2 = tarih.Date.ToString("yyyy-MM-dd") + " 16:00:00";
                    shift_durum[8, 1] = "2";
                }
                if (tarih.Hour >= 16)
                {
                    zaman_aralik_1 = tarih.Date.ToString("yyyy-MM-dd") + " 16:00:00";
                    zaman_aralik_2 = tarih.Date.ToString("yyyy-MM-dd") + " 23:59:59";
                    shift_durum[8, 1] = "3";
                }


                baglanti.Open();
                da = new SqlDataAdapter("Select TOP(1) *From ShiftLogging where Machine IS NULL and Shift=" + shift_durum[8, 1] + " and Time_Start between '" + zaman_aralik_1 + "' and '" + zaman_aralik_2 + "'  order by ID desc", baglanti);
                DataTable tablo = new DataTable();
                tablo.Clear();
                tablo.Columns.Clear();
                da.Fill(tablo);
                baglanti.Close();

                foreach (DataRow row in tablo.Rows)
                {

                    shift_durum[0, 1] = Hat_Degerleri[0, 1].ToString();
                    //shift_durum[1, 1] = row["Worktime"].ToString();
                    shift_durum[2, 1] = row["Line_Speed"].ToString();

                    if (shift_durum[2, 1] == "0")
                    {
                        shift_durum[2, 1] = "1";
                    }

                    shift_durum[3, 1] = row["Target_Count"].ToString();
                    shift_durum[4, 1] = row["Capacity_Count"].ToString();
                    shift_durum[5, 1] = row["Plan_Count"].ToString();
                    if (shift_durum[5, 1] == "")
                    {
                        shift_durum[5, 1] = "1";
                    }
                    shift_durum[6, 1] = row["Plan_Target_Count"].ToString();

                    shift_durum[8, 1] = row["Shift"].ToString();
                    shift_durum[9, 1] = row["Sistem"].ToString();
                    shift_durum[10, 1] = row["Time_Start"].ToString();
                    shift_durum[11, 1] = row["Line"].ToString();
                    shift_durum[12, 1] = row["Time_Stop"].ToString();

                }

            }

            if (shift_durum[9, 1] == "True")
            {


                if (Hat_Degerleri[2, 1] != shift_durum[2, 1] || (tarih.Hour == 8 && tarih.Minute == 0) || (tarih.Hour == 16 && tarih.Minute == 0) || (tarih.Hour == 0 && tarih.Minute == 0))
                {
                    if (shift_start == false)
                    {
                        string zaman_aralik_1 = "";
                        string zaman_aralik_2 = "";
                        if (shift_durum[8, 1] == "1")
                        {
                            zaman_aralik_1 = tarih.Date.ToString("yyyy-MM-dd") + " 00:00:00";
                            zaman_aralik_2 = tarih.Date.ToString("yyyy-MM-dd") + " 08:00:00";
                        }
                        if (shift_durum[8, 1] == "2")
                        {
                            zaman_aralik_1 = tarih.Date.ToString("yyyy-MM-dd") + " 08:00:00";
                            zaman_aralik_2 = tarih.Date.ToString("yyyy-MM-dd") + " 16:00:00";
                        }
                        if (shift_durum[8, 1] == "3")
                        {
                            zaman_aralik_1 = tarih.Date.ToString("yyyy-MM-dd") + " 16:00:00";
                            zaman_aralik_2 = tarih.Date.ToString("yyyy-MM-dd") + " 23:59:59";
                        }


                        baglanti.Open();
                        da = new SqlDataAdapter("Select TOP(1) *From ShiftLogging where Machine IS NULL and Shift=" + shift_durum[8, 1] +  " and Time_Start between '" + zaman_aralik_1 + "' and '" + zaman_aralik_2 + "'  order by ID desc", baglanti);

                        tablo.Clear();
                        tablo.Columns.Clear();
                        da.Fill(tablo);
                        baglanti.Close();

                        
                        foreach (DataRow row in tablo.Rows)
                        {

                            Target_Count_Old = Convert.ToInt32(row["Target_Count"].ToString());

                        }





                        TimeSpan duration;
                        duration = tarih - Convert.ToDateTime(shift_durum[10, 1]);

                        //int Target_Count = Convert.ToInt32(Convert.ToDouble(duration.TotalMinutes) / (Convert.ToDouble(shift_durum[2, 1]) / (1000.0*0.9375)));
                        int Capacity_Count = Convert.ToInt32(Convert.ToDouble(fark.TotalMinutes) / 1.6);
                        int Plan_Target_Count = Convert.ToInt32(Convert.ToDouble(fark.TotalMinutes)*0.9375 / (450.0 / Convert.ToDouble(shift_durum[5, 1])));

                        //Target_Count_Old = Target_Count + Target_Count_Old;
                        Capacity_Count_Old = Capacity_Count;

                        if (tarih.Hour == 8 && tarih.Minute == 0)
                        {
                            Target_Count_Old = 0;
                            Capacity_Count_Old = 0;
                            shift_durum[8, 1] = "2";
                        }
                        if (tarih.Hour == 16 && tarih.Minute == 0)
                        {
                            Target_Count_Old = 0;
                            Capacity_Count_Old = 0;
                            shift_durum[8, 1] = "3";
                        }
                        if (tarih.Hour == 0 && tarih.Minute == 0)
                        {
                            Target_Count_Old = 0;
                            Capacity_Count_Old = 0;
                            shift_durum[8, 1] = "1";
                        }

                        string sorgu = "Update ShiftLogging Set Time_Stop=@zaman, Duration=@Duration, Sistem=@Sistem " +
                                                        "where Time_Start=@Time_Start";
                        komut = new SqlCommand(sorgu, baglanti);
                        //komut = new OleDbCommand(sorgu, baglanti2);
                        komut.Parameters.AddWithValue("@zaman", tarihsql);
                        komut.Parameters.AddWithValue("@Duration", (int)duration.TotalSeconds);
                        komut.Parameters.AddWithValue("@Sistem", "False");

                        komut.Parameters.AddWithValue("@Time_Start", shift_durum[10, 1]);

                        baglanti.Open();
                        komut.ExecuteNonQuery();
                        baglanti.Close();

                        sorgu = "Insert into ShiftLogging ( Time_Start, Production_Count, Target_Count, Capacity_Count, Plan_Target_Count, Plan_Count, Line_Speed, Line, Sistem, Shift ) " +
                                            "values (@zaman, @Production_Count, @Target_Count, @Capacity_Count, @Plan_Target_Count, @Plan_Count, @Line_Speed, 'C2', 'True', @Shift)";
                        komut = new SqlCommand(sorgu, baglanti);
                        komut.Parameters.AddWithValue("@zaman", tarihsql);
                        komut.Parameters.AddWithValue("@Production_Count", Hat_Degerleri[0, 1].ToString());
                        komut.Parameters.AddWithValue("@Target_Count", shift_durum[3, 1].ToString());
                        komut.Parameters.AddWithValue("@Capacity_Count", shift_durum[4, 1].ToString());
                        komut.Parameters.AddWithValue("@Plan_Count", shift_durum[5, 1].ToString());
                        komut.Parameters.AddWithValue("@Plan_Target_Count", shift_durum[6, 1].ToString());
                        komut.Parameters.AddWithValue("@Line_Speed", Hat_Degerleri[2, 1].ToString());
                        komut.Parameters.AddWithValue("@Shift", shift_durum[8, 1].ToString());

                        baglanti.Open();
                        komut.ExecuteNonQuery();
                        baglanti.Close();
                    }
                    shift_start = true;
                }
                else
                {
                    shift_start = false;
                    TimeSpan duration;
                    duration = tarih - Convert.ToDateTime(shift_durum[10, 1]);

                    int Target_Count = Target_Count_Old + Convert.ToInt32(Convert.ToDouble(duration.TotalMinutes)/(Convert.ToDouble(shift_durum[2, 1])/(1000.0*0.9375)));
                    int Capacity_Count = Convert.ToInt32(Convert.ToDouble(fark.TotalMinutes) / 1.6);
                    int Plan_Target_Count = Convert.ToInt32(Convert.ToDouble(fark.TotalMinutes) * 0.9375 / (450.0 / Convert.ToDouble(shift_durum[5, 1])));

                    if (tarih.Hour >= 8 && tarih.Hour < 16)
                    {
                        
                        shift_durum[8, 1] = "2";
                    }
                    if (tarih.Hour >= 16)
                    {
                        
                        shift_durum[8, 1] = "3";
                    }
                    if (tarih.Hour >= 0 && tarih.Hour < 8)
                    {
                        
                        shift_durum[8, 1] = "1";
                    }

                    try
                    {
                        target_eff = (int)((Convert.ToDouble(shift_durum[0, 1]) / Target_Count) * 100.0);
                        capacity_eff = (int)((Convert.ToDouble(shift_durum[0, 1]) / Capacity_Count) * 100.0);
                        plan_eff = (int)((Convert.ToDouble(shift_durum[0, 1]) / Plan_Target_Count) * 100.0);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Eff Hesaplama yapılmadı...");
                        
                    }
                    Console.WriteLine("capacity_eff " + capacity_eff + " " + shift_durum[0,1] + " " + Capacity_Count);

                    string sorgu = "Update ShiftLogging Set  Duration=@Duration , Production_Count=@Production_Count, Target_Count=@Target_Count, Capacity_Count=@Capacity_Count, Plan_Target_Count=@Plan_Target_Count " +
                                                    "where Time_Start=@Time_Start";
                    
                    
                    komut = new SqlCommand(sorgu, baglanti);
                    

                    
                    komut.Parameters.AddWithValue("@Duration", (int)duration.TotalSeconds);
                    komut.Parameters.AddWithValue("@Target_Count", Target_Count);
                    komut.Parameters.AddWithValue("@Capacity_Count", Capacity_Count);
                    komut.Parameters.AddWithValue("@Plan_Target_Count", Plan_Target_Count);
                    komut.Parameters.AddWithValue("@Production_Count", Hat_Degerleri[0,1]);



                    komut.Parameters.AddWithValue("@Time_Start", shift_durum[10, 1]);

                    baglanti.Open();
                    komut.ExecuteNonQuery();
                    baglanti.Close();
                }

            }
            if (shift_durum[9, 1] == "False")
            {
                

                
                

                string sorgu = "Insert into ShiftLogging ( Time_Start, Production_Count, Target_Count, Capacity_Count, Plan_Target_Count, Line_Speed, Line, Sistem , Shift ) " +
                                        "values ( @zaman, @Production_Count, @Target_Count, @Capacity_Count, @Plan_Target_Count, @Line_Speed, 'C2', 'True', @Shift)";
                komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@zaman", shift_durum[12, 1]).ToString();
                komut.Parameters.AddWithValue("@Production_Count", Hat_Degerleri[0, 1].ToString());
                komut.Parameters.AddWithValue("@Target_Count", "0");
                komut.Parameters.AddWithValue("@Capacity_Count", "0");
                komut.Parameters.AddWithValue("@Plan_Target_Count", "0");
                komut.Parameters.AddWithValue("@Line_Speed", Hat_Degerleri[2, 1].ToString());
                komut.Parameters.AddWithValue("@Shift", shift_durum[8, 1].ToString());

                baglanti.Open();
                komut.ExecuteNonQuery();
                baglanti.Close();
            }
            if (shift_durum[9, 1] == null)
            {
                if (tarih.Hour >= 8 && tarih.Hour < 16)
                {
                    shift_durum[8, 1] = "2";
                    
                        shift_durum[10, 1] = tarih.Date.ToString("yyyy-MM-dd") + " 08:00:00";
                    
                }
                if (tarih.Hour >= 16)
                {

                    shift_durum[8, 1] = "3";
                    
                        shift_durum[10, 1] = tarih.Date.ToString("yyyy-MM-dd") + " 16:00:00";
                    

                }
                if (tarih.Hour >= 0 && tarih.Hour < 8)
                {

                    shift_durum[8, 1] = "1";
                    
                        shift_durum[10, 1] = tarih.Date.ToString("yyyy-MM-dd") + " 00:00:00";
                    

                }

                string sorgu = "Insert into ShiftLogging ( Time_Start, Plan_Count, Production_Count, Target_Count, Capacity_Count, Plan_Target_Count, Line_Speed, Line, Sistem , Shift ) " +
                                        "values ( @zaman, @Plan_Count, @Production_Count, @Target_Count, @Capacity_Count, @Plan_Target_Count, @Line_Speed, 'C2', 'True', @Shift)";
                komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@zaman", shift_durum[10, 1]).ToString();
                komut.Parameters.AddWithValue("@Production_Count", Hat_Degerleri[0, 1].ToString());
                komut.Parameters.AddWithValue("@Target_Count", "1");
                komut.Parameters.AddWithValue("@Capacity_Count", "1");
                komut.Parameters.AddWithValue("@Plan_Target_Count", "1");
                komut.Parameters.AddWithValue("@Plan_Count", "1");
                komut.Parameters.AddWithValue("@Line_Speed", Hat_Degerleri[2, 1].ToString());
                komut.Parameters.AddWithValue("@Shift", shift_durum[8, 1].ToString());

                baglanti.Open();
                komut.ExecuteNonQuery();
                baglanti.Close();
            }



        }

        static private void Sql_StoppageLogging()
        {

            baglanti.Open();
            da = new SqlDataAdapter("Select TOP(1) *From StoppageLoggings order by Time_Start desc", baglanti);

            tablo.Clear();
            tablo.Columns.Clear();
            da.Fill(tablo);
            baglanti.Close();

            
            
            
            foreach (DataRow row in tablo.Rows)
            {
                tags_durum[0, 1] = row[Durum_Bitleri[0, 0]].ToString();
                tags_durum[1, 1] = row[Durum_Bitleri[1, 0]].ToString();
                tags_durum[2, 1] = row[Durum_Bitleri[2, 0]].ToString();
                tags_durum[3, 1] = row[Durum_Bitleri[3, 0]].ToString();
                tags_durum[4, 1] = row[Durum_Bitleri[4, 0]].ToString();
                tags_durum[5, 1] = row[Durum_Bitleri[5, 0]].ToString();
                tags_durum[6, 1] = row[Durum_Bitleri[6, 0]].ToString();
                tags_durum[7, 1] = row[Durum_Bitleri[7, 0]].ToString();
                tags_durum[8, 1] = row[Durum_Bitleri[8, 0]].ToString();

                tags_durum[9, 1] = row["Sistem"].ToString();
                tags_durum[10, 1] = row["Time_Start"].ToString();
                tags_durum[11, 1] = row["Line"].ToString();
                
            }

            if (tags_durum[9, 1] == "True")
            {
                int k = 0;
                while (k < 9)
                {
                    if (Durum_Bitleri[k, 1] != tags_durum[k, 1])
                    {

                        TimeSpan duration;
                        duration = tarih - Convert.ToDateTime(tags_durum[10, 1]);
                        string sorgu = "Update StoppageLoggings Set Time_Stop=@zaman, Duration=@Duration , Sistem=@Sistem " +
                                                        "where Time_Start=@Time_Start";
                        komut = new SqlCommand(sorgu, baglanti);
                        //komut = new OleDbCommand(sorgu, baglanti2);
                        komut.Parameters.AddWithValue("@zaman", tarihsql);
                        komut.Parameters.AddWithValue("@Duration", (int)duration.TotalSeconds);
                        komut.Parameters.AddWithValue("@Sistem", "False");

                        komut.Parameters.AddWithValue("@Time_Start", tags_durum[10, 1]);

                        baglanti.Open();
                        komut.ExecuteNonQuery();
                        baglanti.Close();

                        sorgu = "Insert into StoppageLoggings ( Time_Start, Run, Temp, Eq, Shrt, Qual, Mtce, Matl, [Full], Conv, Stoppage_Detail, Line, Sistem ) " +
                                            "values (@zaman, @Run, @Temp, @Eq, @Shrt, @Qual, @Mtce, @Matl, @Full, @Conv, '0', 'C2', 'True')";
                        komut = new SqlCommand(sorgu, baglanti);
                        komut.Parameters.AddWithValue("@zaman", tarihsql);
                        komut.Parameters.AddWithValue("@Run", Durum_Bitleri[0, 1].ToString());

                        komut.Parameters.AddWithValue("@Temp", Durum_Bitleri[1, 1].ToString());
                        komut.Parameters.AddWithValue("@Eq", Durum_Bitleri[2, 1].ToString());
                        komut.Parameters.AddWithValue("@Shrt", Durum_Bitleri[3, 1].ToString());
                        komut.Parameters.AddWithValue("@Qual", Durum_Bitleri[4, 1].ToString());
                        komut.Parameters.AddWithValue("@Mtce", Durum_Bitleri[5, 1].ToString());
                        komut.Parameters.AddWithValue("@Matl", Durum_Bitleri[6, 1].ToString());
                        komut.Parameters.AddWithValue("@Full", Durum_Bitleri[7, 1].ToString());
                        komut.Parameters.AddWithValue("@Conv", Durum_Bitleri[8, 1].ToString());


                        baglanti.Open();
                        komut.ExecuteNonQuery();
                        baglanti.Close();
                        break;
                    }
                    k++;
                }
                
            }
            if (tags_durum[9, 1] == null || tags_durum[9, 1] == "False")
            {
                
                string sorgu = "Insert into StoppageLoggings ( Time_Start, Run, Temp, Eq, Shrt, Qual, Mtce, Matl, [Full], Conv, Stoppage_Detail, Line, Sistem ) " +
                                            "values (@zaman, @Run, @Temp, @Eq, @Shrt, @Qual, @Mtce, @Matl, @Full, @Conv, '0', 'C2', 'True')";
                komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@zaman", tarihsql);
                komut.Parameters.AddWithValue("@Run", Durum_Bitleri[0, 1].ToString());
                komut.Parameters.AddWithValue("@Temp", Durum_Bitleri[1, 1].ToString());
                komut.Parameters.AddWithValue("@Eq", Durum_Bitleri[2, 1].ToString());
                komut.Parameters.AddWithValue("@Shrt", Durum_Bitleri[3, 1].ToString());
                komut.Parameters.AddWithValue("@Qual", Durum_Bitleri[4, 1].ToString());
                komut.Parameters.AddWithValue("@Mtce", Durum_Bitleri[5, 1].ToString());
                komut.Parameters.AddWithValue("@Matl", Durum_Bitleri[6, 1].ToString());
                komut.Parameters.AddWithValue("@Full", Durum_Bitleri[7, 1].ToString());
                komut.Parameters.AddWithValue("@Conv", Durum_Bitleri[8, 1].ToString());


                baglanti.Open();
                komut.ExecuteNonQuery();
                baglanti.Close();
            }




        }
        static private void Sql_MachineStat()
        {
            TimeSpan fark=new TimeSpan();
            if (tarih.Hour >= 8 && tarih.Hour<16){
                fark = tarih.TimeOfDay - Convert.ToDateTime("2024-01-01 08:00:00").TimeOfDay;
            }
            if (tarih.Hour >= 16 && tarih.Hour < 24)
            {
                fark = tarih.TimeOfDay - Convert.ToDateTime("2024-01-01 16:00:00").TimeOfDay;
            }
            if (tarih.Hour >= 0 && tarih.Hour < 8)
            {
                fark = tarih.TimeOfDay - Convert.ToDateTime("2024-01-01 00:00:00").TimeOfDay;
            }

            int stoptimes = (int)fark.TotalMinutes - Convert.ToInt32(Hat_Degerleri[1, 1]);

            string sorgu = "Update MachineStat Set Time=@zaman, Total_Stoppage=@Total_Stoppage, Plan_Target_Count=@Plan_Target_Count, Target_OEE=@Target_OEE, OEE=@OEE, OU=@OU, Plan_Count=@Plan_Count, Production_Count=@Production_Count, Worktime=@Worktime, Line_Speed=@Line_Speed, Run=@Run, Temp=@Temp, Eq=@Eq, Shrt=@Shrt, Qual=@Qual, Mtce=@Mtce, Matl=@Matl, [Full]=@Full, Conv=@Conv " +
                                            "where Line='C2'";
            komut = new SqlCommand(sorgu, baglanti);
            
            
            komut.Parameters.AddWithValue("@zaman", tarihsql);
            komut.Parameters.AddWithValue("@Run", Durum_Bitleri[0, 1]);
            komut.Parameters.AddWithValue("@Temp", Durum_Bitleri[1, 1]);
            komut.Parameters.AddWithValue("@Eq", Durum_Bitleri[2, 1]);
            komut.Parameters.AddWithValue("@Shrt", Durum_Bitleri[3, 1]);
            komut.Parameters.AddWithValue("@Qual", Durum_Bitleri[4, 1]);
            komut.Parameters.AddWithValue("@Mtce", Durum_Bitleri[5, 1]);
            komut.Parameters.AddWithValue("@Matl", Durum_Bitleri[6, 1]);
            komut.Parameters.AddWithValue("@Full", Durum_Bitleri[7, 1]);
            komut.Parameters.AddWithValue("@Conv", Durum_Bitleri[8, 1]);

            komut.Parameters.AddWithValue("@Total_Stoppage", stoptimes);
            komut.Parameters.AddWithValue("@Target_OEE", plan_eff);
            komut.Parameters.AddWithValue("@OEE", target_eff);
            komut.Parameters.AddWithValue("@OU", capacity_eff);
            komut.Parameters.AddWithValue("@Production_Count", Hat_Degerleri[0, 1]);
            komut.Parameters.AddWithValue("@Plan_Count", shift_durum[5, 1]);
            komut.Parameters.AddWithValue("@Plan_Target_Count", shift_durum[6, 1]);
            komut.Parameters.AddWithValue("@Worktime", Hat_Degerleri[1, 1]);
            komut.Parameters.AddWithValue("@Line_Speed", Hat_Degerleri[2, 1]);


            try
            {
                baglanti.Open();
                komut.ExecuteNonQuery();
                baglanti.Close();
            }
            catch (Exception)
            {

                baglanti.Close();
            }

            
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

        private void PRESS454455_FormClosed(object sender, FormClosedEventArgs e)
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

        private void PRESS454455_FormClosing(object sender, FormClosingEventArgs e)
        {
            Client.Disconnect();
            sistem = false;
            thread2.Abort();
        }
    }
}

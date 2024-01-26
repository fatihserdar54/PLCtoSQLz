using Sharp7;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLCtoSQL
{
    public class Veri_Okuma
    {
        
        int result=0;
        private byte[] Buffer = new byte[2500];
        int SizeRead = 0;
        DateTime guncelzaman;

        public DateTime Read_Time()
        {
            return guncelzaman;
        }

        public int Reading_Result()
        {
            return result;
        }
        

        public byte[] Read(string DBNumber,int count, S7Client Client)
        {
            
            
            
                        result = Client.ReadArea(S7Area.DB, Convert.ToInt16(DBNumber), 0, count, S7WordLength.Byte, Buffer, ref SizeRead);

            guncelzaman = DateTime.Now.AddMilliseconds(-Client.ExecutionTime);


            return Buffer;
            
        }

        
    }
}

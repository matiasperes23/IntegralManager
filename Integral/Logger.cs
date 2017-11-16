using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Integral
{
    public class Logger
    {
        public static void Log(String message)
        {
            DateTime datetime = DateTime.Now;
            String oFileName = "logs/log" + DateTime.Now.ToString("M-d-yyyy") + ".txt";
            if (!File.Exists(oFileName))
            {
                System.IO.FileStream f = File.Create(oFileName);
                f.Close();
            }

            try
            {
                System.IO.StreamWriter writter = File.AppendText(oFileName);
                writter.WriteLine(datetime.ToString("MM-dd hh:mm") + " &gt; " + message);
                writter.Flush();
                writter.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
        }
    }
}

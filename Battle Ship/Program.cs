using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

namespace Battle_Ship
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            /*
            TcpClient client = null;
            NetworkStream stream = null;
            */

            Form1 form1 = new Form1();
            Application.Run(form1);

            /*
            client = form1.getClient();
            stream = form1.getStream();

            Form2 form2 = new Form2(client, stream);
            Application.Run(form2);
            */
        }
    }
}

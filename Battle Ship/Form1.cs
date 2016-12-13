using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 소켓
using System.Net;
using System.Net.Sockets;

namespace Battle_Ship
{
    public partial class Form1 : Form
    {
        const int port = 7036;
        const String server = "163.180.116.24";

        public TcpClient client;
        public NetworkStream stream;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Battle Ship";
        }
        
        public TcpClient getClient()
        {
            return this.client;
        }

        public NetworkStream getStream()
        {
            return this.stream;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // 연결
                client = new TcpClient(server, port);

                // 스트림 생성
                stream = client.GetStream();

                // 쓰기
                String message = "Message from client";
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: {0}", message);

                // 읽기
                data = new Byte[256];
                String responseData = String.Empty;
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                // 출력
                MessageBox.Show(responseData);

                // Program.cs에 TcpClient, NetworkStream 넘겨주기 ?
                // form2 실행
                Form2 form2 = new Form2(client, stream);

                /*
                form2.TopLevel = false;
                form2.TopMost = true;
                form2.Parent = this;
                Panel
                */
                 
                form2.ShowDialog();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("ArgumentNullException: {0}", ex);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException: {0}", ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 종료
            this.Close();
        }
    }
}

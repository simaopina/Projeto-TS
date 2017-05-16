using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        // private static ProtocolSI protocolSI;
        private const int PORT = 9999;


        ProtocolSI protocolSI = new ProtocolSI();
        private TcpListener tcpListener = null;
        private TcpClient tcpClient= null;
        private NetworkStream networkStream=null;
        private char[] msg;

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
           /* TcpListener tcpListener = null;
            TcpClient tcpClient = null;

            NetworkStream networkStream = null;
            */


        }

        private void btnLigacao_Click(object sender, EventArgs e)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            tcpListener = new TcpListener(endPoint);

            tcpListener.Start();

            tcpClient = tcpListener.AcceptTcpClient();

            networkStream = tcpClient.GetStream();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string mgs = txbMensagem.Text;

        }

        private void btnEnviarMensagem_Click(object sender, EventArgs e)
        {
            try
            {
            
                int bytesRead = 0;

                int bufferSize = tcpClient.ReceiveBufferSize;

                bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                byte[] packet = protocolSI.GetData();

                Console.WriteLine(Encoding.UTF8.GetString(packet));

                if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
                {

                    Byte[] ack = Encoding.UTF8.GetBytes(msg);
                    networkStream.Write(ack, 0, ack.Length);

                }

            }
            catch (Exception)
            {

                throw;
            }

            finally
            {
                if (networkStream != null)
                {
                    networkStream.Close();
                }

                if (tcpClient != null)
                {
                    tcpClient.Close();
                }

                if (tcpListener != null)
                {
                    tcpListener.Stop();
                }
            }
        }
    }
}

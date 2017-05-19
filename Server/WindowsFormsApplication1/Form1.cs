using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        private static ProtocolSI protocolSI;


       
        //private TcpListener tcpListener = null;
        private TcpClient tcpClient= null;
        private NetworkStream networkStream=null;
      

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            protocolSI = new ProtocolSI();

        }

        private void btnLigacao_Click(object sender, EventArgs e)
        {

            try
            {
                MessageBox.Show("Começando cliente...");
               // Console.WriteLine("Starting client...");
                tcpClient = new TcpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);

                MessageBox.Show("Quer começar a coneção?");
               // Console.WriteLine("Start connection? press any key");
                //Console.ReadKey();
                tcpClient.Connect(endPoint);

                MessageBox.Show("Conectando ao servior...");
               // Console.WriteLine("Connect to server...");
                networkStream = tcpClient.GetStream();

                #region Send String Message


                string msg = "Hello from client!!";
                byte[] msgByte = Encoding.UTF8.GetBytes(msg);

                //-----------Ficha 7---------------------------------------
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, msgByte);
                //---------------------------------------------------------   

                //networkStream.Write(msgByte, 0, msgByte.Length);
                networkStream.Write(packet, 0, packet.Length);

                //Receber ack

                Byte[] ack = new byte[2];
                networkStream.Read(ack, 0, ack.Length);
                MessageBox.Show("Recebido" + Encoding.UTF8.GetString(ack));
               // Console.WriteLine("Received" + Encoding.UTF8.GetString(ack));

                #endregion

                // Console.WriteLine("Connect to server...");
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


            }
            //Console.ReadKey();
        }
    }

       
}

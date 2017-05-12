using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using EI.SI;

namespace WindowsFormsApplication1
{
   class cliente
    {
        //Declaração de variaveis globais 
        private static ProtocolSI protocolSI;

        private const int PORT = 9999;

        public static object ProtocolSICmdType { get; private set; }

        static void Main(string args)
        {
            protocolSI = new ProtocolSI();


            TcpClient tcpClient = null;
            NetworkStream networkStream = null;


            try
            {
                Console.WriteLine("A iniciar cliente...");
                tcpClient = new TcpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);

                Console.WriteLine("Quer começar a conecção? Pressione em qualquer tecla");
                Console.ReadKey();
                tcpClient.Connect(endPoint);

                Console.WriteLine("Conectar ao servidor...");
                networkStream = tcpClient.GetStream();

                #region mandar string mensagem

                string msg = "Olá do cliente!!";
                byte[] msgByte = Encoding.UTF8.GetBytes(msg);
                byte[] packet = protocolSI.Make(EI.SI.ProtocolSICmdType.DATA, msgByte);

                networkStream.Write(msgByte, 0, msgByte.Length);

                //receber ack

                Byte[] ack = new byte[2];
                networkStream.Read(ack, 0, ack.Length);
                Console.WriteLine("Recebido" + Encoding.UTF8.GetString(ack));

                #endregion
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

            Console.ReadKey();

        }

    }
    }


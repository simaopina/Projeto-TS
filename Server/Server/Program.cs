using EI.SI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Server
    {
        private static ProtocolSI protocolSI;

        private const int PORT = 9999;

      //  public static object ProtocolSICmdType { get; private set; }

        static void Main(string[] args)
        {
            protocolSI = new ProtocolSI();

            TcpListener tcpListener = null;
            TcpClient tcpClient = null;
            NetworkStream networkStream = null;


            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
            tcpListener = new TcpListener(endPoint);


            Console.WriteLine("Starting Server...");

            tcpListener.Start();
            Console.WriteLine("Waiting for connections...");

            tcpClient = tcpListener.AcceptTcpClient();
            Console.WriteLine("Cliente connected");

            networkStream = tcpClient.GetStream();
            int bytesRead = 0;

            #region Receive string message

            int bufferSize = tcpClient.ReceiveBufferSize;
            // byte[] buffer = new byte[bufferSize];

           
            bytesRead = networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

            byte[] packet = protocolSI.GetData();


            Console.WriteLine(Encoding.UTF8.GetString(packet));

            if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
            {

            }

            //------------------------


            //Enviar ack

            Byte[] ack = Encoding.UTF8.GetBytes("OK");
            networkStream.Write(ack, 0, ack.Length);

            #endregion
           /* catch (Exception)
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
            Console.ReadKey();*/
        }
    }
}


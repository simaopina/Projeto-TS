using EI.SI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Server
{
    class Server
    {
        private static ProtocolSI protocolSI;

        private const int PORT = 9999;

        static string pathFilesFolder = Path.Combine(Environment.CurrentDirectory, @"Files\");

        //----------------obter nomes dos ficheiros numa pasta---------------------
        //static string pathFilesFolder = Path.Combine(Environment.CurrentDirectory, @"Files\");
        //string[] filesCollection = Directory.GetFiles(pathFilesFolder);

        //  public static object ProtocolSICmdType { get; private set; }

        static void Main(string[] args)
        {
            protocolSI = new ProtocolSI();

            TcpListener tcpListener = null;
            TcpClient tcpClient = null;
            NetworkStream networkStream = null;


            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
                tcpListener = new TcpListener(endPoint);

                tcpListener.Start();

                tcpClient = tcpListener.AcceptTcpClient();

                networkStream = tcpClient.GetStream();

                int bytesRead = 0;


                //############################

                int requestListSize;
                byte[] bufferRequestList;

                requestListSize = tcpClient.ReceiveBufferSize;
                bufferRequestList = new byte[requestListSize];

                networkStream.Read(bufferRequestList, 0, requestListSize);

                byte[] fileList = GetFiles();
                networkStream.Write(fileList, 0, fileList.Length);



                //*********************************

                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                if (protocolSI.GetCmdType() == ProtocolSICmdType.USER_OPTION_1)
                {
                    Console.WriteLine(protocolSI.GetStringFromData());

                }
                
                //*********************************

                //############################

                int requestFileSize;
                byte[] bufferRequestFile;
                string requestFile;

                requestFileSize = tcpClient.ReceiveBufferSize;
                bufferRequestFile = new byte[requestFileSize];

                bytesRead = networkStream.Read(bufferRequestFile, 0, requestFileSize);
                requestFile = Encoding.UTF8.GetString(bufferRequestFile, 0, bytesRead);

                //------------
                FileStream fileStream = new FileStream(Path.Combine(pathFilesFolder, requestFile), FileMode.Open);

                int bufferSizes = 20480;
                byte[] buffer = new byte[bufferSizes];

                while ((bytesRead = fileStream.Read(buffer, 0, bufferSizes)) > 0)
                {
                    networkStream.Write(buffer, 0, bytesRead);
                }

                //-------------


                fileStream.Close();
                //############################
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

                

                //------------------------


                //Enviar ack

                /*Byte[] ack = Encoding.UTF8.GetBytes("OK");
                networkStream.Write(ack, 0, ack.Length);*/


            }
        }

        //############################################################
        //Funções para a criação das chaves de encriptação e para as guardar em ficheiros
        private RSACryptoServiceProvider rsa;
        string chavePublica;
        string chavePrivada;
        
        public void GenerateKeys()
        {
            rsa = new RSACryptoServiceProvider();

            string publicKey = rsa.ToXmlString(false);
            chavePublica = publicKey;

            string privateKey = rsa.ToXmlString(true);
            chavePrivada = privateKey;
        }
        
        public void SavePublicKey_ToFile()
        {
            string publicKey = rsa.ToXmlString(false);
            File.WriteAllText("publicKey.txt", publicKey);
        }

        public void SavePrivatePublicKey_ToFile()
        {
            string publicKey = rsa.ToXmlString(false);
            File.WriteAllText("privatePublicKey.txt", publicKey);
        }


        //############################################################
        private static byte[] GetFiles()
        {
            string[] filesCollection = Directory.GetFiles(pathFilesFolder);

            string files = "";
            byte[] bufferFiles;


            for (int i = 0; i < filesCollection.Count(); i++)
            {
                files += Path.GetFileName(filesCollection[i]);
                files += ";";
            }

            bufferFiles = Encoding.UTF8.GetBytes(files);


            return bufferFiles;
        }


        public void sendFile()
        {
            int bytesread = 0;

            int buffersize = 20480;

            byte[] buffer = new byte[buffersize];

            string originalFilePath = Path.Combine(Environment.CurrentDirectory, @"Files");

            FileStream originalFileStream = new FileStream(originalFilePath, FileMode.Open);

            while ((bytesread = originalFileStream.Read(buffer, 0, buffersize)) > 0)
            {
                System.Threading.Thread.Sleep(1000);

                originalFileStream.Read(buffer, 0, bytesread);

            }

            originalFileStream.Close();


        }

    }
}

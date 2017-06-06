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
using System.Data.SqlClient;

namespace Server
{
    class Server
    {
        private static ProtocolSI protocolSI;

        static string user;

        static string pass;

        static byte[] login_decrypted;

        private const int PORT = 9999;

        static NetworkStream networkStream = null;

        static string pathFilesFolder = Path.Combine(Environment.CurrentDirectory, @"Files\");

        //----------------obter nomes dos ficheiros numa pasta---------------------
        //static string pathFilesFolder = Path.Combine(Environment.CurrentDirectory, @"Files\");
        //string[] filesCollection = Directory.GetFiles(pathFilesFolder);

        //  public static object ProtocolSICmdType { get; private set; }

        static void Main(string[] args)
        {
            aes = new AesCryptoServiceProvider();
            protocolSI = new ProtocolSI();
            TcpListener tcpListener = null;
            TcpClient tcpClient = null;
            //NetworkStream networkStream = null;
            GenerateKeys();

            //try
           // {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);
                tcpListener = new TcpListener(endPoint);

                tcpListener.Start();

                tcpClient = tcpListener.AcceptTcpClient();

                networkStream = tcpClient.GetStream();


                //RECEBE A PUBLIC KEY DO CLIENTE
                byte[] packet = protocolSI.Make(ProtocolSICmdType.PUBLIC_KEY, chavePublica);
                networkStream.Write(packet, 0, packet.Length);

                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                if (protocolSI.GetCmdType() == ProtocolSICmdType.SECRET_KEY)
                {
                    aes.Key = rsa.Decrypt(protocolSI.GetData(), true);
                }

                //send ack
                packet = protocolSI.Make(ProtocolSICmdType.ACK);
                networkStream.Write(packet, 0, packet.Length);


                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                if (protocolSI.GetCmdType() == ProtocolSICmdType.IV)
                {
                    aes.IV = rsa.Decrypt(protocolSI.GetData(), true);
                }

                //send ack
                packet = protocolSI.Make(ProtocolSICmdType.ACK);
                networkStream.Write(packet, 0, packet.Length);


                //Autenticação
                //byte[] login = protocolSI.Make(ProtocolSICmdType.USER_OPTION_5, //encrypt_symmetric("USERNAME"));
                //networkStream.Write(login, 0 ,login.Length);

                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                login_decrypted = decrypt_symmetric(protocolSI.GetData());
                user = Encoding.UTF8.GetString(login_decrypted);

                //send ACK
                packet = protocolSI.Make(ProtocolSICmdType.ACK);
                networkStream.Write(packet, 0, packet.Length);

                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                login_decrypted = decrypt_symmetric(protocolSI.GetData());
                pass = Encoding.UTF8.GetString(login_decrypted);

                //send ACK
                packet = protocolSI.Make(ProtocolSICmdType.ACK);
                networkStream.Write(packet, 0, packet.Length);

                string login_bool = "";    

                if (VerifyLogin(user, pass))
                {
                    login_bool = "True";
                }
                else
                {
                    login_bool = "False";
                }

                packet = protocolSI.Make(ProtocolSICmdType.DATA, login_bool);
                networkStream.Write(packet, 0, packet.Length);


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
                    /*
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (protocolSI.GetCmdType() == ProtocolSICmdType.USER_OPTION_1)
                    {
                        Console.WriteLine(protocolSI.GetStringFromData());

                    }*/

            //*********************************

            //############################

            //-------------
            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

            if (protocolSI.GetStringFromData() == "file")
            {
                networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                string file = protocolSI.GetStringFromData();
                sendFile(file);

            }

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


                //}

            }
    

        //############################################################
        //Funções para a criação das chaves de encriptação e para as guardar em ficheiros
        private static RSACryptoServiceProvider rsa;
        private static string chavePublica;
        private static string chavePrivada;
        
        public static void GenerateKeys()
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



        public static void sendFile(string file)
        {
            int bytesread = 0;

            int buffersize = 1024;

            byte[] buffer = new byte[buffersize];

            byte[] packet;

            byte[] mensagenzinha;

            string originalFilePath = Path.Combine(Environment.CurrentDirectory, @"Files\");

            FileStream originalFileStream = new FileStream(originalFilePath+file, FileMode.Open);
            Console.WriteLine("iniciozito do while");

            while ((bytesread = originalFileStream.Read(buffer, 0, buffersize)) > 0)
            { 
                Console.WriteLine("Inicio do while");

                //originalFileStream.Read(buffer, 0, bytesread);

                buffer = protocolSI.Make(ProtocolSICmdType.USER_OPTION_8, buffer);
                networkStream.Write(buffer, 0, buffer.Length);

                //send ack
               networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                if (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
                {
                    break;
                }

                Console.WriteLine("Fim do while");
            }

            mensagenzinha = protocolSI.Make(ProtocolSICmdType.EOF);
            networkStream.Write(mensagenzinha, 0, mensagenzinha.Length);

            originalFileStream.Close();


        }

        private static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            using (HashAlgorithm hashAlgorithm = SHA512.Create())
            {
                // Declarar e inicializar buffer para o texto e salt
                byte[] plainTextWithSaltBytes =
                              new byte[plainText.Length + salt.Length];

                // Copiar texto para buffer
                for (int i = 0; i < plainText.Length; i++)
                {
                    plainTextWithSaltBytes[i] = plainText[i];
                }
                // Copiar salt para buffer a seguir ao texto
                for (int i = 0; i < salt.Length; i++)
                {
                    plainTextWithSaltBytes[plainText.Length + i] = salt[i];
                }

                //Devolver hash do text + salt
                return hashAlgorithm.ComputeHash(plainTextWithSaltBytes);
            }
        }

        private static bool VerifyLogin(string username, string password)
        {
            SqlConnection conn = null;
            try
            {
                // Configurar ligação à Base de Dados
                conn = new SqlConnection();
                conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\BaseTopSeg\BaseDadosTopSeg.mdf;Integrated Security=True;Connect Timeout=30");

                // Abrir ligação à Base de Dados
                conn.Open();

                // Declaração do comando SQL
                String sql = "SELECT * FROM Users WHERE Username = @username";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;

                // Declaração dos parâmetros do comando SQL
                SqlParameter param = new SqlParameter("@username", username);

                // Introduzir valor ao parâmentro registado no comando SQL
                cmd.Parameters.Add(param);

                // Associar ligação à Base de Dados ao comando a ser executado
                cmd.Connection = conn;

                // Executar comando SQL
                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    throw new Exception("Error while trying to access an user");
                }

                // Ler resultado da pesquisa
                reader.Read();

                // Obter Hash (password + salt)
                byte[] saltedPasswordHashStored = (byte[])reader["SaltedPasswordHash"];

                // Obter salt
                byte[] saltStored = (byte[])reader["Salt"];

                conn.Close();


                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                byte[] hash = GenerateSaltedHash(passwordBytes, saltStored);

                return saltedPasswordHashStored.SequenceEqual(hash);
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred");
                return false;
            }
        }


        private static AesCryptoServiceProvider aes;

        private static byte[] encrypt_symmetric(String msg)
        {
            byte[] data = Encoding.UTF8.GetBytes(msg);
            using (ICryptoTransform ct = aes.CreateEncryptor())
            {
                return ct.TransformFinalBlock(data, 0, data.Length);
            }
        }
        private static byte[] encrypt_symmetric(byte[] data)
        {
            using (ICryptoTransform ct = aes.CreateEncryptor())
            {
                return ct.TransformFinalBlock(data, 0, data.Length);

                Console.Write(ct.TransformFinalBlock(data, 0, data.Length));
            }

           
        }
        private static byte[] encrypt_symmetric(byte[] data, int bytesRead)
        {
            using (ICryptoTransform ct = aes.CreateEncryptor())
            {
                return ct.TransformFinalBlock(data, 0, bytesRead);
            }
        }
        private static byte[] decrypt_symmetric(byte[] data)
        {
            using (ICryptoTransform ct = aes.CreateDecryptor())
            {
                return ct.TransformFinalBlock(data, 0, data.Length);
            }
        }

    }
}

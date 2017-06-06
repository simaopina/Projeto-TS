using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Login : Form
    {



        public Login()
        {
            InitializeComponent();
        }

        // private static ProtocolSI protocolSI;
        private const int PORT = 9999;

        private static AesCryptoServiceProvider aes;
        private static RSACryptoServiceProvider rsa;

        private static ProtocolSI protocolSI;



        //private TcpListener tcpListener = null;
        TcpListener tcpListener = null;
        TcpClient tcpClient= null;
        NetworkStream networkStream=null;

        int requestFileSize;
        byte[] bufferRequestFile;
        string requestFile;
        int bytesRead = 0;

        static string pathFilesFolder = Path.Combine(Environment.CurrentDirectory, @"Files\");


        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            protocolSI = new ProtocolSI();

            try
            {
                aes = new AesCryptoServiceProvider();
                rsa = new RSACryptoServiceProvider();
                 tcpClient = new TcpClient();
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, PORT);

                MessageBox.Show("Connecting...");

                tcpClient.Connect(endPoint);
                MessageBox.Show("Connected to Server");

                networkStream = tcpClient.GetStream();

                
                // DÁ A PUBLIC KEY DO SERVIDOR PARA VERIFICAR SE A LIGAÇÃO ESTÁ SEGURA

                networkStream.Read(protocolSI.Buffer,0, protocolSI.Buffer.Length);
                if (protocolSI.GetCmdType() == ProtocolSICmdType.PUBLIC_KEY)
                {
                    rsa.FromXmlString(protocolSI.GetStringFromData());  //
                   
                    byte[] secretKey = protocolSI.Make(ProtocolSICmdType.SECRET_KEY, rsa.Encrypt(aes.Key, true));
                    networkStream.Write(secretKey, 0, secretKey.Length);

                    //Receive ack
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
                    {
                        MessageBox.Show("Erro!");
                    }

                    secretKey = protocolSI.Make(ProtocolSICmdType.IV, rsa.Encrypt(aes.IV, true));
                    networkStream.Write(secretKey, 0, secretKey.Length);

                    //Receive ack
                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
                    {
                        MessageBox.Show("Erro!");
                    }
                }


            }
            catch (Exception)
            {

                throw;
            }

        }

        private void btnLigacao_Click(object sender, EventArgs e)
        {

        }

        private void WriteStatus(string msg)
        {
            
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {


            byte[] login = protocolSI.Make(ProtocolSICmdType.USER_OPTION_5, encrypt_symmetric(Encoding.UTF8.GetBytes(txtUsername.Text)));
            networkStream.Write(login, 0, login.Length);

            //Receive ack
            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            if (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                MessageBox.Show("Erro!");
            }



            login = protocolSI.Make(ProtocolSICmdType.USER_OPTION_5, encrypt_symmetric(Encoding.UTF8.GetBytes(txtPassword.Text)));
            networkStream.Write(login, 0, login.Length);

            //Receive ack
            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            if (protocolSI.GetCmdType() != ProtocolSICmdType.ACK)
            {
                MessageBox.Show("Erro!");
            }


            networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
            string resposta = protocolSI.GetStringFromData();


            if (resposta == "True")
            {
                this.BeginInvoke((Action)delegate
                {
                    NovoMenu();
                });
            }
            else
            {
                MessageBox.Show("Invalid User");
            }




        }

        private void NovoMenu()
        {
            Menu me = new Menu(tcpClient,  tcpListener,  networkStream, rsa);
            me.Show();
        }


        private static byte[] GenerateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return buff;
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

        private void button2_Click(object sender, EventArgs e)
        {
            /*byte[] salt = GenerateSalt(8);

            byte[] pass = Encoding.UTF8.GetBytes("123"); //não buscar nas txt

            byte[] hash = GenerateSaltedHash(pass, salt);

            string username = "adm";//textBoxUsername.Text;
*/
          
        }

        private bool VerifyLogin(string username, string password)
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
                MessageBox.Show("An error occurred");
                return false;
            }
        }

        private static byte[] encrypt_symmetric(byte[] data)
        {
            using (ICryptoTransform ct = aes.CreateEncryptor())
            {
                return ct.TransformFinalBlock(data, 0, data.Length);
            }
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            byte[] mensagem = protocolSI.Make(ProtocolSICmdType.DATA, "shutdown");
            networkStream.Write(mensagem, 0, mensagem.Length);
        }
    }

       
}

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
        }

        private void btnLigacao_Click(object sender, EventArgs e)
        {

           
            //Console.ReadKey();
        }

        private void WriteStatus(string msg)
        {
            
        }


        private void btnLogin_Click(object sender, EventArgs e)
        {

            /*byte[] username = Encoding.UTF8.GetBytes(txtUsername.Text);

            byte[] password = Encoding.UTF8.GetBytes(txtPassword.Text);

            networkStream.Write(username, 0, username.Length);

            networkStream.Write(password, 0, password.Length);


            byte[] loginResponse = new byte[2];
            networkStream.Read(loginResponse, 0, 2);
            string response = Encoding.UTF8.GetString(loginResponse);
            */


            string username = txtUsername.Text;

            string password = txtPassword.Text;


            if (VerifyLogin(username, password))
            {
                MessageBox.Show("Valid user");
            }

            else
            {
                MessageBox.Show("Invalid User");
            }

        }

        private void Register(string username, byte[] saltedPasswordHash, byte[] salt)
        {
            SqlConnection conn = null;
            try
            {
                // Configurar ligação à Base de Dados
                conn = new SqlConnection();
                conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\sawak\Source\Repos\Projeto-TS\Server\WindowsFormsApplication1\DatabaseT_Seg.mdf;Integrated Security=True");

                // Abrir ligação à Base de Dados
                conn.Open();

                // Declaração dos parâmetros do comando SQL
                SqlParameter paramUsername = new SqlParameter("@username", username);
                SqlParameter paramPassHash = new SqlParameter("@saltedPasswordHash", saltedPasswordHash);
                SqlParameter paramSalt = new SqlParameter("@salt", salt);

                // Declaração do comando SQL
                String sql = "INSERT INTO Users (Username, SaltedPasswordHash, Salt) VALUES (@username,@saltedPasswordHash,@salt)";

                // Prepara comando SQL para ser executado na Base de Dados
                SqlCommand cmd = new SqlCommand(sql, conn);

                // Introduzir valores aos parâmentros registados no comando SQL
                cmd.Parameters.Add(paramUsername);
                cmd.Parameters.Add(paramPassHash);
                cmd.Parameters.Add(paramSalt);

                // Executar comando SQL
                int lines = cmd.ExecuteNonQuery();

                // Fechar ligação
                conn.Close();
                if (lines == 0)
                {
                    // Se forem devolvidas 0 linhas alteradas então o não foi executado com sucesso
                    throw new Exception("Error while inserting an user");
                }

                else
                {
                    MessageBox.Show("User inserted!");

                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while inserting an user:" + e.Message);
            }
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
            byte[] salt = GenerateSalt(8);

            //  textBoxSalt.Text = Convert.ToBase64String(salt);

            //  textBoxSizeSalt.Text = (salt.Length * 8).ToString();

            byte[] pass = Encoding.UTF8.GetBytes("123"); //não buscar nas txt

            byte[] hash = GenerateSaltedHash(pass, salt);

            //textBoxSaltedHash.Text = Convert.ToBase64String(hash);

            // textBoxSizePass.Text = (hash.Length * 8).ToString();

            string username = "adm";//textBoxUsername.Text;

            Register(username, hash, salt);
        }

        private bool VerifyLogin(string username, string password)
        {
            SqlConnection conn = null;
            try
            {
                // Configurar ligação à Base de Dados
                conn = new SqlConnection();
                conn.ConnectionString = String.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\sawak\Desktop\Ipleiria\2º Semestre\Tópicos de Segurança\Ficha08\FichaPratica8_Base\Database1.mdf';Integrated Security=True");

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

    }

       
}

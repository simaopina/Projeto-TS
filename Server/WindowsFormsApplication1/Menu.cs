using EI.SI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
    public partial class Menu : Form
    {
     
        static string pathFilesFolder = Path.Combine(Environment.CurrentDirectory, @"Files\");
        string fileName = "";
        private NetworkStream networkStream;
        private ProtocolSI protocolSI;

        public Menu(TcpClient tcpClient, TcpListener tcpListener, NetworkStream networkStream)
        {
            InitializeComponent();
            protocolSI = new ProtocolSI();
            this.networkStream = networkStream;
            byte[] request = Encoding.UTF8.GetBytes("FileList");
            networkStream.Write(request, 0, request.Length);

            int bufferResponse = tcpClient.ReceiveBufferSize;
            int bytesRead = 0;
            byte[] response = new byte[bufferResponse];
            bytesRead = networkStream.Read(response, 0, bufferResponse);

            ShowFiles(response, bytesRead);


            if (File.Exists(fileName))
            {
                Process.Start(fileName);
            }


            string[] filesCollection = Directory.GetFiles(pathFilesFolder);

            string files = "";
            byte[] bufferFiles;


            for (int i = 0; i < filesCollection.Count(); i++)
            {
                files += Path.GetFileName(filesCollection[i]);
                files += ";";
            }

            bufferFiles = Encoding.UTF8.GetBytes(files);

           

        }



        private void ShowFiles(byte[] buffer, int size)
        {
            string fileList = Encoding.UTF8.GetString(buffer);
            string file;
            int indexStart = 0;
            int indexEnd = 0;


            do
            {
                indexEnd = fileList.IndexOf(";", indexStart);
                file = fileList.Substring(indexStart, indexEnd - indexStart);

                indexStart = indexEnd + 1;

                listViewFicheiros.Items.Add(file);
            } while (indexStart != size);
        }

   

        private void listViewFicheiros_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (File.Exists(fileName))
            {
                Process.Start(fileName);
                caminhoImagem();
            }
        }

        private void btnPedirFicheiro_Click(object sender, EventArgs e)
        {
            byte[] mensagem = protocolSI.Make(ProtocolSICmdType.DATA, "file");
            networkStream.Write(mensagem, 0, mensagem.Length);


            String imagem1 = listViewFicheiros.SelectedItems[0].Text;

            byte[] mensagemFicheiro = protocolSI.Make(ProtocolSICmdType.DATA, imagem1);

            networkStream.Write(mensagemFicheiro, 0, mensagemFicheiro.Length);

            int bytesread = 0;

            int buffersize = 1024;

            byte[] packet;

            string copiaFilePath = Path.Combine(Environment.CurrentDirectory, @"Files\");



            using (FileStream copiaFileStream = new FileStream(copiaFilePath + imagem1, FileMode.CreateNew))
            { 

                do
                {

                    MessageBox.Show("Inicio do DO");

                    networkStream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                    if (protocolSI.GetCmdType() == ProtocolSICmdType.USER_OPTION_8)
                    { 
                        copiaFileStream.Write(protocolSI.GetData(), 0, protocolSI.GetData().Length);

                        //Enviar ack
                        packet = protocolSI.Make(ProtocolSICmdType.ACK);
                        networkStream.Write(packet, 0, packet.Length);
                    }


                   
                } while (protocolSI.GetCmdType() != ProtocolSICmdType.EOF);

            }
            MessageBox.Show("Fim do while");

            if (imagem1 != null)
            {

                string copyFilePath = pbxFoto.ImageLocation = Path.Combine(Environment.CurrentDirectory, @"Files\" + imagem1);

                /*packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, imagem1);
                networkStream.Write(packet, 0, packet.Length);*/

                MessageBox.Show("copiar");
            }


           // FileStream CopyFileStream = new FileStream(copyFilePath, FileMode.CreateNew);

           
           //CopyFileStream.Write()


           //// originalFileStream.Close();
           // CopyFileStream.Close();

            MessageBox.Show("Copiado com sucesso!");

        }


        public void caminhoImagem()
        {
            pbxFoto.ImageLocation = Path.Combine(Environment.CurrentDirectory, @"Files\");
        }
    }
}

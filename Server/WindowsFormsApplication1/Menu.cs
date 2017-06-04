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

        public Menu(TcpClient tcpClient, TcpListener tcpListener, NetworkStream networkStream)
        {
            InitializeComponent();

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

        private void listViewFicheiros_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (File.Exists(fileName))
            {
                Process.Start(fileName);
                pictureBox1.ImageLocation = Path.Combine(Environment.CurrentDirectory, @"Files\");
            }
        }
    }
}

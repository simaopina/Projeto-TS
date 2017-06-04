namespace WindowsFormsApplication1
{
    partial class Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewFicheiros = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnPedirFicheiro = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // listViewFicheiros
            // 
            this.listViewFicheiros.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewFicheiros.Location = new System.Drawing.Point(12, 12);
            this.listViewFicheiros.Name = "listViewFicheiros";
            this.listViewFicheiros.Size = new System.Drawing.Size(126, 201);
            this.listViewFicheiros.TabIndex = 13;
            this.listViewFicheiros.UseCompatibleStateImageBehavior = false;
            this.listViewFicheiros.View = System.Windows.Forms.View.Details;
            this.listViewFicheiros.SelectedIndexChanged += new System.EventHandler(this.listViewFicheiros_SelectedIndexChanged_1);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Nome";
            this.columnHeader1.Width = 99;
            // 
            // btnPedirFicheiro
            // 
            this.btnPedirFicheiro.Location = new System.Drawing.Point(242, 174);
            this.btnPedirFicheiro.Name = "btnPedirFicheiro";
            this.btnPedirFicheiro.Size = new System.Drawing.Size(75, 39);
            this.btnPedirFicheiro.TabIndex = 14;
            this.btnPedirFicheiro.Text = "Pedir Ficheiro";
            this.btnPedirFicheiro.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(192, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(185, 126);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 252);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnPedirFicheiro);
            this.Controls.Add(this.listViewFicheiros);
            this.Name = "Menu";
            this.Text = "Menu";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewFicheiros;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Button btnPedirFicheiro;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
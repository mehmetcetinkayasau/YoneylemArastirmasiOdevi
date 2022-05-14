using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics;
using ScottPlot;

//DENEMEEE

namespace YöneylemAraştırması3
{
    
    public partial class Form1 : Form
    {
        private Rectangle orjFormRect;
        private Rectangle orjDataGridRect;
        private Rectangle orjLabel1Rect;
        private Rectangle orjLabel2Rect;
        private Rectangle orjLabel3Rect;
        private Rectangle orjButon1Rect;
        private Rectangle orjButon2Rect;
        private Rectangle orjCombo1Rect;
        private Rectangle orjCombo2Rect;
        private Rectangle orjRichRect;
        private Rectangle orjPlotRect;


        private bool rakamVeyaVirgul;
        private bool virgulSayisi;
        bool tekVirgul;
        bool tekSifir;
        bool eksiSayisi;
        bool tekEksi;
        short virguladet;
        short rakamVirgulHataTipi;
        short yVeriSayisi, x1VeriSayisi, x2VeriSayisi, x3VeriSayisi;
        short yYerKntrlVSayisi, x1YerKntrlVSayisi, x2YerKntrlVSayisi, x3YerKntrlVSayisi;
        short degiskenSayisi;
        short eksiAdet;
        double mse;
        double mape;
        double mae;


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "Polinom")
            {
                label2.Visible = true;
                comboBox2.Visible = true;
                comboBox2.SelectedItem = "2";
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Cells[2].Value = "";
                    dataGridView1.Rows[i].Cells[3].Value = "";
                    dataGridView1.Rows[i].Cells[2].ReadOnly = true;
                    dataGridView1.Rows[i].Cells[3].ReadOnly = true;

                }
            }
            else
            {
                label2.Visible = false;
                comboBox2.Visible = false;
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[i].Cells[2].ReadOnly = false;
                    dataGridView1.Rows[i].Cells[3].ReadOnly = false;

                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            orjFormRect = new Rectangle(this.Location, this.Size);
            orjDataGridRect = new Rectangle(dataGridView1.Location, dataGridView1.Size);
            orjLabel1Rect = new Rectangle(label1.Location, label1.Size);
            orjLabel2Rect = new Rectangle(label2.Location, label2.Size);
            orjLabel3Rect = new Rectangle(label3.Location, label3.Size);
            orjButon1Rect = new Rectangle(button1.Location, button1.Size);
            orjButon2Rect = new Rectangle(button2.Location, button2.Size);
            orjCombo1Rect = new Rectangle(comboBox1.Location, comboBox1.Size);
            orjCombo2Rect = new Rectangle(comboBox2.Location, comboBox2.Size);
            orjRichRect = new Rectangle(richTextBox1.Location, richTextBox1.Size);
            orjPlotRect = new Rectangle(formsPlot1.Location, formsPlot1.Size);

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            nesneYenidenBoyutlandir();
        }
        private void nesneYenidenBoyutlandir()
        {
            ResizeControl(dataGridView1, orjDataGridRect);
            ResizeControl(label1, orjLabel1Rect);
            ResizeControl(label2, orjLabel2Rect);
            ResizeControl(label3, orjLabel3Rect);
            ResizeControl(button1, orjButon1Rect);
            ResizeControl(button2, orjButon2Rect);
            ResizeControl(comboBox1, orjCombo1Rect);
            ResizeControl(comboBox2, orjCombo2Rect);
            ResizeControl(richTextBox1, orjRichRect);
            ResizeControl(formsPlot1, orjPlotRect);

        }
        private void ResizeControl(System.Windows.Forms.Control control,Rectangle orjKontrolRect)
        {
            float xRatio = (float)this.ClientRectangle.Width / (float)orjFormRect.Width;
            float yRatio = (float)this.ClientRectangle.Height / (float)orjFormRect.Height;
            float yeniX = orjKontrolRect.Location.X * xRatio;
            float yeniY = orjKontrolRect.Location.Y * yRatio;

            control.Location = new Point((int)yeniX, (int)yeniY);
            control.Width = (int)(orjKontrolRect.Width * xRatio);
            control.Height=(int)(orjKontrolRect.Height * yRatio);
        }
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 1500; i++)
            {
                dataGridView1.Rows.Add();
                
            }
            this.dataGridView1.KeyDown += new KeyEventHandler(dataGridView1_KeyDown);
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            for (int i = 0; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            int satırSayisi = 1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                row.HeaderCell.Value = satırSayisi.ToString();
                satırSayisi = satırSayisi + 1;
            }
            dataGridView1.RowHeadersWidth = 90;

        }
        private short degiskenSayisii()
        {
            yVeriSayisi = 0; x1VeriSayisi = 0; x2VeriSayisi = 0; x3VeriSayisi = 0;
            yYerKntrlVSayisi = 0; x1YerKntrlVSayisi = 0; x2YerKntrlVSayisi = 0; x3YerKntrlVSayisi = 0;
            
            degiskenSayisi=0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                
                
                if (!(String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[0].Value as string)))
                {
                    yVeriSayisi++;
                    dataGridView1.Rows[i].Cells[0].Value = dataGridView1.Rows[i].Cells[0].Value.ToString().Replace("\r", "");
                }
                    
                if (!(String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[1].Value as string)))
                {
                    x1VeriSayisi++;
                    dataGridView1.Rows[i].Cells[1].Value = dataGridView1.Rows[i].Cells[1].Value.ToString().Replace("\r", "");
                }
                if (!(String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[2].Value as string)))
                {
                    x2VeriSayisi++;
                    dataGridView1.Rows[i].Cells[2].Value = dataGridView1.Rows[i].Cells[2].Value.ToString().Replace("\r", "");
                }
                if (!(String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[3].Value as string)))
                {
                    x3VeriSayisi++;
                    dataGridView1.Rows[i].Cells[3].Value = dataGridView1.Rows[i].Cells[3].Value.ToString().Replace("\r", "");
                }
            }
            for (int i = 0; i < yVeriSayisi; i++)
            {
                if (!(String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[0].Value as string)))
                    yYerKntrlVSayisi++;
            }
            for (int i = 0; i < x1VeriSayisi; i++)
            {
                
                if (!(String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[1].Value as string)))
                    x1YerKntrlVSayisi++;
            }
            for (int i = 0; i < x2VeriSayisi; i++)
            {
               
                if (!(String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[2].Value as string)))
                    x2YerKntrlVSayisi++;
            }
            for (int i = 0; i < x3VeriSayisi; i++)
            {
                if (!(String.IsNullOrEmpty(dataGridView1.Rows[i].Cells[3].Value as string)))
                    x3YerKntrlVSayisi++;
            }
            if (yVeriSayisi == 0 && x1VeriSayisi == 0 && x2VeriSayisi == 0 && x3VeriSayisi == 0)
                degiskenSayisi = 0;
            else if ((yVeriSayisi== yYerKntrlVSayisi) && (yYerKntrlVSayisi == x1YerKntrlVSayisi) && (yYerKntrlVSayisi == x1VeriSayisi)&&(yYerKntrlVSayisi == x2YerKntrlVSayisi) && (yYerKntrlVSayisi == x2VeriSayisi) && (yYerKntrlVSayisi == x3YerKntrlVSayisi) && (yYerKntrlVSayisi == x3VeriSayisi))
                degiskenSayisi = 3;
            else if ((yVeriSayisi == yYerKntrlVSayisi) && (yYerKntrlVSayisi == x1YerKntrlVSayisi) && (yYerKntrlVSayisi == x1VeriSayisi) && (yYerKntrlVSayisi == x2YerKntrlVSayisi) && (yYerKntrlVSayisi == x2VeriSayisi) && (yYerKntrlVSayisi != x3YerKntrlVSayisi) && (yYerKntrlVSayisi != x3VeriSayisi) && x3VeriSayisi == 0)
                degiskenSayisi = 2;
            else if ((yVeriSayisi == yYerKntrlVSayisi) && (yYerKntrlVSayisi == x1YerKntrlVSayisi) && (yYerKntrlVSayisi == x1VeriSayisi) && (yYerKntrlVSayisi != x2YerKntrlVSayisi) && (yYerKntrlVSayisi != x2VeriSayisi) && (yYerKntrlVSayisi != x3YerKntrlVSayisi) && (yYerKntrlVSayisi != x3VeriSayisi) && x3VeriSayisi == 0 && x2VeriSayisi==0)
                degiskenSayisi = 1;
            else
                degiskenSayisi = -1; // Eksik veri girişi olmamalı, veri girişi ilk satırdan başlamalı ve ilk sütun ile son sütun arasında boş sütun bulunmamalı.
            return degiskenSayisi;
        }
        
        private short rakamVeyaVirgulVeyaVirgulSayisi()
        {

            rakamVeyaVirgul = true;
            virgulSayisi = true;
            tekVirgul = true;
            tekSifir = true;
            eksiSayisi = true;
            tekEksi = true;
            virguladet = 0;
            eksiAdet = 0;
            if (comboBox1.SelectedItem == "Polinom")
            {
                for (int i = 0; i < degiskenSayisi+1; i++)
                {
                    for (int j = 0; j < yYerKntrlVSayisi; j++)
                    {
                        for (int k = 0; k < dataGridView1.Rows[j].Cells[i].Value.ToString().Length; k++)
                        {
                            if (char.IsDigit(Convert.ToString(dataGridView1.Rows[j].Cells[i].Value)[k]) || Convert.ToString(dataGridView1.Rows[j].Cells[i].Value)[k] == ',' || Convert.ToString(dataGridView1.Rows[j].Cells[i].Value)[k] == '-')
                                rakamVeyaVirgul = true;
                            else
                            {
                                rakamVeyaVirgul = false;
                                break;
                            }
                            if (dataGridView1.Rows[j].Cells[i].Value.ToString()[k] == ',')
                                virguladet++;
                            if (dataGridView1.Rows[j].Cells[i].Value.ToString()[k] == '-')
                                eksiAdet++;
                        }
                        if (rakamVeyaVirgul == false)
                            break;

                        if (virguladet >= 2 )
                        {
                            virgulSayisi = false;
                            break;
                        }
                        else if(dataGridView1.Rows[j].Cells[i].Value.ToString().Length==2)
                        {
                            if (dataGridView1.Rows[j].Cells[i].Value.ToString()[0] == '-' && dataGridView1.Rows[j].Cells[i].Value.ToString()[1] == ',') 
                            {
                                virgulSayisi = false;
                                break;
                            }
                        }
                        else if (virguladet == 1 && dataGridView1.Rows[j].Cells[i].Value.ToString().Length == 1)
                        {
                            tekVirgul = false;
                            break;
                        }
                        else
                            virguladet = 0;
                        if (eksiAdet > 1 || (eksiAdet==1 && dataGridView1.Rows[j].Cells[i].Value.ToString().Length>1 && dataGridView1.Rows[j].Cells[i].Value.ToString()[0]!='-'))
                        {
                            eksiSayisi = false;
                            break;
                        }
                        else if (eksiAdet == 1 && dataGridView1.Rows[j].Cells[i].Value.ToString().Length == 1)
                        {
                            tekEksi = false;
                            break;
                        }
                        else
                            eksiAdet = 0; 
                    }
                    if (rakamVeyaVirgul == false || virgulSayisi == false || tekVirgul == false || tekEksi == false || eksiSayisi==false)
                        break;
                }
                if (rakamVeyaVirgul == true && virgulSayisi == true && tekVirgul == true  && tekEksi == true && eksiSayisi == true)
                    rakamVirgulHataTipi = 1; // hatayok
                else if (rakamVeyaVirgul == true && virgulSayisi == false)
                    rakamVirgulHataTipi = 2; // fazladan virgul
                else if (rakamVeyaVirgul == true && tekVirgul == false)
                    rakamVirgulHataTipi = 3; // tekvirgül
                else if (rakamVeyaVirgul == true && eksiSayisi == false)
                    rakamVirgulHataTipi = 4; // fazla eksi
                else if (rakamVeyaVirgul == true && tekEksi == false)
                    rakamVirgulHataTipi = 5; // tek eksi
                else if (rakamVeyaVirgul == false)
                    rakamVirgulHataTipi = 6; // rakamveyavirguldegil
            }
            else if (comboBox1.SelectedItem=="Kuvvet")
            {
                for (int i = 0; i < degiskenSayisi + 1; i++)
                {
                    for (int j = 0; j < yYerKntrlVSayisi; j++)
                    {
                        for (int k = 0; k < dataGridView1.Rows[j].Cells[i].Value.ToString().Length; k++)
                        {
                            if (char.IsDigit(Convert.ToString(dataGridView1.Rows[j].Cells[i].Value)[k]) || Convert.ToString(dataGridView1.Rows[j].Cells[i].Value)[k] == ',')
                                rakamVeyaVirgul = true;
                            else
                            {
                                rakamVeyaVirgul = false;
                                break;
                            }
                            if (dataGridView1.Rows[j].Cells[i].Value.ToString()[k] == ',')
                                virguladet++;
                        }
                        if (rakamVeyaVirgul == false)
                            break;

                        if (virguladet >= 2)
                        {
                            virgulSayisi = false;
                            break;
                        }
                        else if (virguladet == 1 && dataGridView1.Rows[j].Cells[i].Value.ToString().Length == 1)
                        {
                            tekVirgul = false;
                            break;
                        }
                        else
                            virguladet = 0;
                        if (Convert.ToDouble(dataGridView1.Rows[j].Cells[i].Value.ToString().Replace(",", "")) == 0)
                        {
                            tekSifir = false;
                            break;
                        }
                    }
                    if (rakamVeyaVirgul == false || virgulSayisi == false || tekVirgul == false || tekSifir == false)
                        break;
                }
                if (rakamVeyaVirgul == true && virgulSayisi == true && tekVirgul == true && tekSifir == true)
                    rakamVirgulHataTipi = 1; // hatayok
                else if (rakamVeyaVirgul == true && virgulSayisi == false)
                    rakamVirgulHataTipi = 2; // fazladan virgul
                else if (rakamVeyaVirgul == true && tekVirgul == false)
                    rakamVirgulHataTipi = 3; // tekvirgül
                else if (rakamVeyaVirgul == true && tekSifir == false)
                    rakamVirgulHataTipi = 4; // tekvirgül
                else if (rakamVeyaVirgul == false)
                    rakamVirgulHataTipi = 5; // rakamveyavirguldegil
            }
            
            
            return rakamVirgulHataTipi;
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            richTextBox1.Text = "";
            formsPlot1.Plot.Clear();
            formsPlot1.Plot.Title("");
            formsPlot1.Plot.XLabel("");
            formsPlot1.Plot.YLabel("");
            formsPlot1.Refresh();

            for (int i = 0; i < 1500; i++)
            {
                dataGridView1.Rows.Add();

            }
            int satırSayisi = 1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                row.HeaderCell.Value = satırSayisi.ToString();
                satırSayisi = satırSayisi + 1;
            }
        }



        void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            
            if (e.Control && e.KeyCode == Keys.V)
            {
                string s = Clipboard.GetText();
                string[] lines = s.Split('\n');
                int row = dataGridView1.CurrentCell.RowIndex;
                int col = dataGridView1.CurrentCell.ColumnIndex;
                foreach (string line in lines)
                {
                    if (row < dataGridView1.RowCount && line.Length > 0)
                    {
                        string[] cells = line.Split('\t');
                        for (int i = 0; i < cells.GetLength(0); ++i)
                        {
                            if (col + i <this.dataGridView1.ColumnCount)
                            {
                                dataGridView1[col + i, row].Value =Convert.ChangeType(cells[i], dataGridView1[col + i, row].ValueType);
                            }
                            else
                            {
                                break;
                            }
                        }
                        row++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (comboBox1.SelectedItem=="Polinom")
                {
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        dataGridView1.Rows[i].Cells[2].Value = "";
                        dataGridView1.Rows[i].Cells[3].Value = "";
                    }
                }
            }
            if ((e.KeyCode == Keys.Delete) || (e.KeyCode == Keys.Back))
            {
                for (int i = 0;i < dataGridView1.SelectedCells.Count; i++)
                    dataGridView1.SelectedCells[i].Value = "";
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            formsPlot1.Plot.Clear();
            formsPlot1.Plot.Title("");
            formsPlot1.Plot.XLabel("");
            formsPlot1.Plot.YLabel("");
            formsPlot1.Refresh();
            
            degiskenSayisi = degiskenSayisii();
            rakamVirgulHataTipi = rakamVeyaVirgulVeyaVirgulSayisi();
            mae = 0;
            mse = 0;
            mape = 0;

            if (comboBox1.SelectedItem == "Polinom")
            {
                if (degiskenSayisi == 0)
                    MessageBox.Show("Veri girişi yapılmadı.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if(degiskenSayisi==3)
                    MessageBox.Show("Polinom fonksiyonu için 1 adet bağımsız değişken kullanabilirsiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if(degiskenSayisi==2)
                    MessageBox.Show("Polinom fonksiyonu için 1 adet bağımsız değişken kullanabilirsiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (degiskenSayisi == 1)
                {
                    if (rakamVirgulHataTipi == 1)
                    {
                        double[] y = new double[yVeriSayisi];
                        double[] x = new double[yVeriSayisi];
                        double max = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                        double min = Convert.ToDouble(dataGridView1.Rows[0].Cells[1].Value);
                        for (int i = 0; i < yVeriSayisi; i++)
                        {
                            if (Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) > max)
                                max = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                            if (Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) < min)
                                min = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                            x[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                            y[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value);

                        }
                        

                        
                        if (y.Length >= 3)
                        {
                            double[] p = Fit.Polynomial(x, y, Convert.ToInt32(comboBox2.SelectedItem));
                            double[] xs = new double[Convert.ToInt32((Math.Truncate(max)-Math.Truncate(min))/0.1 + 35)];
                            double[] ys = new double[Convert.ToInt32((Math.Truncate(max) - Math.Truncate(min)) / 0.1 + 35)];
                            int j = 0;
                            for (double i = Math.Truncate(min); i < Math.Truncate(max) + 3.5; i += 0.1)
                            {
                                if (j == xs.Length)
                                    break;
                                else
                                {
                                    xs[j] = i;
                                    if (Convert.ToInt32(comboBox2.SelectedItem) == 1)
                                        ys[j] = Math.Round(p[0], 3) + Math.Round(p[1], 3) * i;
                                    else if (Convert.ToInt32(comboBox2.SelectedItem) == 2)
                                        ys[j] = Math.Round(p[0], 3) + Math.Round(p[1], 3) * i + Math.Round(p[2], 3) * Math.Pow(i, 2);
                                    else if (Convert.ToInt32(comboBox2.SelectedItem) == 3)
                                        ys[j] = Math.Round(p[0], 3) + Math.Round(p[1], 3) * i + Math.Round(p[2], 3) * Math.Pow(i, 2) + Math.Round(p[3], 3) * Math.Pow(i, 3);
                                    else if (Convert.ToInt32(comboBox2.SelectedItem) == 4)
                                        ys[j] = Math.Round(p[0], 3) + Math.Round(p[1], 3) * i + Math.Round(p[2], 3) * Math.Pow(i, 2) + Math.Round(p[3], 3) * Math.Pow(i, 3) + Math.Round(p[4], 3) * Math.Pow(i, 4);
                                    else if (Convert.ToInt32(comboBox2.SelectedItem) == 5)
                                        ys[j] = Math.Round(p[0], 3) + Math.Round(p[1], 3) * i + Math.Round(p[2], 3) * Math.Pow(i, 2) + Math.Round(p[3], 3) * Math.Pow(i, 3) + Math.Round(p[4], 3) * Math.Pow(i, 4) + Math.Round(p[5], 3) * Math.Pow(i, 5);
                                }
                                j++;
                            }
                            
                            formsPlot1.Plot.AddScatterLines(xs, ys, Color.Blue, 2);
                            formsPlot1.Plot.AddScatter(x, y, lineWidth: 0, color: Color.Black);
                            
                            formsPlot1.Plot.XLabel("X");
                            formsPlot1.Plot.YLabel("Y");
                            formsPlot1.Refresh();

                            if (Convert.ToInt32(comboBox2.SelectedItem) == 1)
                            {
                                richTextBox1.Text = "Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x";
                                formsPlot1.Plot.Title("Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x");
                                for (int i = 0; i < yVeriSayisi; i++)
                                {
                                    mse+=Math.Pow(y[i]-(Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i]),2)/yVeriSayisi;
                                    mape += 100 * Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i])) / (yVeriSayisi * Math.Abs(y[i]));
                                    mae += Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i])) / yVeriSayisi;
                                }
                                dataGridView1.Rows[0].Cells[4].Value = Math.Round(mse,10);
                                dataGridView1.Rows[0].Cells[5].Value = Math.Round(mape,10) + "%";
                                dataGridView1.Rows[0].Cells[6].Value = Math.Round(mae, 10);

                            }
                            else if (Convert.ToInt32(comboBox2.SelectedItem) == 2)
                            {
                                richTextBox1.Text = "Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x" + "+" + "(" + Math.Round(p[2], 3).ToString() + ")" + "*" + "x^2";
                                formsPlot1.Plot.Title("Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x" + "+" + "(" + Math.Round(p[2], 3).ToString() + ")" + "*" + "x^2");
                                for (int i = 0; i < yVeriSayisi; i++)
                                {
                                    mse += Math.Pow(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2)), 2) / yVeriSayisi;
                                    mape += 100 * Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2))) / (yVeriSayisi * Math.Abs(y[i]));
                                    mae += Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2))) / yVeriSayisi;
                                }
                                dataGridView1.Rows[0].Cells[4].Value = Math.Round(mse, 10);
                                dataGridView1.Rows[0].Cells[5].Value = Math.Round(mape, 10) + "%";
                                dataGridView1.Rows[0].Cells[6].Value = Math.Round(mae, 10);
                            }
                            else if (Convert.ToInt32(comboBox2.SelectedItem) == 3)
                            {
                                richTextBox1.Text = "Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x" + "+" + "(" + Math.Round(p[2], 3).ToString() + ")" + "*" + "x^2" + "+" + "(" + Math.Round(p[3], 3).ToString() + ")" + "*" + "x^3";
                                formsPlot1.Plot.Title("Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x" + "+" + "(" + Math.Round(p[2], 3).ToString() + ")" + "*" + "x^2" + "+" + "(" + Math.Round(p[3], 3).ToString() + ")" + "*" + "x^3");
                                for (int i = 0; i < yVeriSayisi; i++)
                                {
                                    mse += Math.Pow(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2) + Math.Round(p[3], 3) * Math.Pow(x[i], 3)), 2) / yVeriSayisi;
                                    mape += 100 * Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2) + Math.Round(p[3], 3) * Math.Pow(x[i], 3))) / (yVeriSayisi * Math.Abs(y[i]));
                                    mae += Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2) + Math.Round(p[3], 3) * Math.Pow(x[i], 3))) / yVeriSayisi;
                                }
                                dataGridView1.Rows[0].Cells[4].Value = Math.Round(mse, 10);
                                dataGridView1.Rows[0].Cells[5].Value = Math.Round(mape, 10) + "%";
                                dataGridView1.Rows[0].Cells[6].Value = Math.Round(mae, 10);
                            }
                            else if (Convert.ToInt32(comboBox2.SelectedItem) == 4)
                            {
                                richTextBox1.Text = "Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x" + "+" + "(" + Math.Round(p[2], 3).ToString() + ")" + "*" + "x^2" + "+" + "(" + Math.Round(p[3], 3).ToString() + ")" + "*" + "x^3" + "+" + "(" + Math.Round(p[4], 3).ToString() + ")" + "*" + "x^4";
                                formsPlot1.Plot.Title("Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x" + "+" + "(" + Math.Round(p[2], 3).ToString() + ")" + "*" + "x^2" + "+" + "(" + Math.Round(p[3], 3).ToString() + ")" + "*" + "x^3" + "+" + "(" + Math.Round(p[4], 3).ToString() + ")" + "*" + "x^4");
                                for (int i = 0; i < yVeriSayisi; i++)
                                {
                                    mse += Math.Pow(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2) + Math.Round(p[3], 3) * Math.Pow(x[i], 3) + Math.Round(p[4], 3) * Math.Pow(x[i], 4)), 2) / yVeriSayisi;
                                    mape += 100 * Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2) + Math.Round(p[3], 3) * Math.Pow(x[i], 3) + Math.Round(p[4], 3) * Math.Pow(x[i], 4))) / (yVeriSayisi * Math.Abs(y[i]));
                                    mae += Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2) + Math.Round(p[3], 3) * Math.Pow(x[i], 3) + Math.Round(p[4], 3) * Math.Pow(x[i], 4))) / yVeriSayisi;
                                }
                                dataGridView1.Rows[0].Cells[4].Value = Math.Round(mse, 10);
                                dataGridView1.Rows[0].Cells[5].Value = Math.Round(mape, 10) + "%";
                                dataGridView1.Rows[0].Cells[6].Value = Math.Round(mae, 10);
                            } 
                            else if (Convert.ToInt32(comboBox2.SelectedItem) == 5)
                            {
                                richTextBox1.Text = "Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x" + "+" + "(" + Math.Round(p[2], 3).ToString() + ")" + "*" + "x^2" + "+" + "(" + Math.Round(p[3], 3).ToString() + ")" + "*" + "x^3" + "+" + "(" + Math.Round(p[4], 3).ToString() + ")" + "*" + "x^4" + "+" + "(" + Math.Round(p[5], 3).ToString() + ")" + "*" + "x^5";
                                formsPlot1.Plot.Title("Y = " + Math.Round(p[0], 3).ToString() + "+" + "(" + Math.Round(p[1], 3).ToString() + ")" + "*x" + "+" + "(" + Math.Round(p[2], 3).ToString() + ")" + "*" + "x^2" + "+" + "(" + Math.Round(p[3], 3).ToString() + ")" + "*" + "x^3" + "+" + "(" + Math.Round(p[4], 3).ToString() + ")" + "*" + "x^4" + "+" + "(" + Math.Round(p[5], 3).ToString() + ")" + "*" + "x^5");
                                for (int i = 0; i < yVeriSayisi; i++)
                                {
                                    mse += Math.Pow(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2) + Math.Round(p[3], 3) * Math.Pow(x[i], 3) + Math.Round(p[4], 3) * Math.Pow(x[i], 4) + Math.Round(p[5], 3) * Math.Pow(x[i], 5)), 2) / yVeriSayisi;
                                    mape += 100 * Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2) + Math.Round(p[3], 3) * Math.Pow(x[i], 3) + Math.Round(p[4], 3) * Math.Pow(x[i], 4) + Math.Round(p[5], 3) * Math.Pow(x[i], 5))) / (yVeriSayisi * Math.Abs(y[i]));
                                    mae += Math.Abs(y[i] - (Math.Round(p[0], 3) + Math.Round(p[1], 3) * x[i] + Math.Round(p[2], 3) * Math.Pow(x[i], 2) + Math.Round(p[3], 3) * Math.Pow(x[i], 3) + Math.Round(p[4], 3) * Math.Pow(x[i], 4) + Math.Round(p[5], 3) * Math.Pow(x[i], 5))) / yVeriSayisi;
                                }
                                dataGridView1.Rows[0].Cells[4].Value = Math.Round(mse, 10);
                                dataGridView1.Rows[0].Cells[5].Value = Math.Round(mape, 10) + "%";
                                dataGridView1.Rows[0].Cells[6].Value = Math.Round(mae, 10);
                            }
                        }
                        else
                            MessageBox.Show("Üç veya daha fazla satır girmelisiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (rakamVirgulHataTipi == 2)
                        MessageBox.Show("Hatalı virgül kullanımı yaptınız.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 3)
                        MessageBox.Show("Rakam kullanmadan virgül kullanamazsınız.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 4)
                        MessageBox.Show("Hatalı eksi kullanımı yaptınız.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 5)
                        MessageBox.Show("Rakam kullanmadan eksi kullanamazsınız", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 6)
                        MessageBox.Show("Sadece sayı veya virgül kullanabilirsiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else if (degiskenSayisi==-1)
                {
                    MessageBox.Show("Eksik veri girişi olmamalı.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            else if(comboBox1.SelectedItem == "Kuvvet")
            {
                if (degiskenSayisi == 0)
                    MessageBox.Show("Veri girişi yapılmadı.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (degiskenSayisi == 3)
                {
                    if (rakamVirgulHataTipi == 1)
                    {
                        double[][] x1x2x3 = new double[yVeriSayisi][];

                        double[] y = new double[yVeriSayisi];

                        for (int i = 0; i < yVeriSayisi; i++)
                        {

                            x1x2x3[i] = new double[3];
                            x1x2x3[i][0] = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                            x1x2x3[i][1] = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value);
                            x1x2x3[i][2] = Convert.ToDouble(dataGridView1.Rows[i].Cells[3].Value);
                            y[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value);

                        }
                        if (y.Length >= 4)
                        {
                            var z_hat = y.Select(r => Math.Log(r)).ToArray();
                            double[] p_hat = Fit.LinearMultiDim(x1x2x3, z_hat, d => 1.0, d => Math.Log(d[0]), d => Math.Log(d[1]), d => Math.Log(d[2]));
                            double u = Math.Exp(p_hat[0]);
                            double v = p_hat[1];
                            double w = p_hat[2];
                            double z = p_hat[3];
                            for (int i = 0; i < yVeriSayisi; i++)
                            {
                                mse += Math.Pow(y[i] - (Math.Round(u, 3) * Math.Pow(Math.Round(x1x2x3[i][0], 3), v) * Math.Pow(Math.Round(x1x2x3[i][1], 3), w) * Math.Pow(Math.Round(x1x2x3[i][2], 3), z)),2) / yVeriSayisi;
                                mape+=100*Math.Abs(y[i]- (Math.Round(u, 3) * Math.Pow(Math.Round(x1x2x3[i][0], 3), v) * Math.Pow(Math.Round(x1x2x3[i][1], 3), w) * Math.Pow(Math.Round(x1x2x3[i][2], 3), z))) / (yVeriSayisi * Math.Abs(y[i]));
                                mae += Math.Abs(y[i] - (Math.Round(u, 3) * Math.Pow(Math.Round(x1x2x3[i][0], 3), v) * Math.Pow(Math.Round(x1x2x3[i][1], 3), w) * Math.Pow(Math.Round(x1x2x3[i][2], 3), z))) / yVeriSayisi;
                            }
                            dataGridView1.Rows[0].Cells[4].Value = Math.Round(mse, 10);
                            dataGridView1.Rows[0].Cells[5].Value = Math.Round(mape, 10);
                            dataGridView1.Rows[0].Cells[6].Value = Math.Round(mae, 10);
                            richTextBox1.Text = "Y = " + Math.Round(u, 3).ToString() + "*(x1)^(" + Math.Round(v, 3).ToString() + ")*(x2)^" + "(" + Math.Round(w, 3).ToString() + ")"+ "*(x3)^"+"("+Math.Round(z,3).ToString()+")";
                        }
                        else
                            MessageBox.Show("Dört veya daha fazla satır girmelisiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        
                    }
                    else if (rakamVirgulHataTipi == 2)
                        MessageBox.Show("Hatalı virgül kullanımı yaptınız.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 3)
                        MessageBox.Show("Rakam kullanmadan virgül kullanamazsınız.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 4)
                        MessageBox.Show("Veri seti içerisinde 0 bulunmamalıdır.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 5)
                        MessageBox.Show("Sadece rakam veya virgül kullanabilirsiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (degiskenSayisi == 2)
                {
                    if (rakamVirgulHataTipi == 1)
                    {
                        double[][] x1x2 = new double[yVeriSayisi][];

                        double[] y = new double[yVeriSayisi];

                        for (int i = 0; i < yVeriSayisi; i++)
                        {

                            x1x2[i] = new double[2];
                            x1x2[i][0] = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                            x1x2[i][1] = Convert.ToDouble(dataGridView1.Rows[i].Cells[2].Value);
                            y[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value);
                        }

                        if(y.Length>=4)
                        {
                            var z_hat = y.Select(r => Math.Log(r)).ToArray();
                            double[] p_hat = Fit.LinearMultiDim(x1x2, z_hat, d => 1.0, d => Math.Log(d[0]), d => Math.Log(d[1]));
                            double u = Math.Exp(p_hat[0]);
                            double v = p_hat[1];
                            double w = p_hat[2];
                            for (int i = 0; i < yVeriSayisi; i++)
                            {
                                mse += Math.Pow(y[i] - (Math.Round(u, 3) * Math.Pow(Math.Round(x1x2[i][0], 3), v) * Math.Pow(Math.Round(x1x2[i][1], 3), w)), 2) / yVeriSayisi;
                                mape += 100 * Math.Abs(y[i] - (Math.Round(u, 3) * Math.Pow(Math.Round(x1x2[i][0], 3), v) * Math.Pow(Math.Round(x1x2[i][1], 3), w))) / (yVeriSayisi * Math.Abs(y[i]));
                                mae += Math.Abs(y[i] - (Math.Round(u, 3) * Math.Pow(Math.Round(x1x2[i][0], 3), v) * Math.Pow(Math.Round(x1x2[i][1], 3), w))) / yVeriSayisi;
                            }
                            dataGridView1.Rows[0].Cells[4].Value = Math.Round(mse, 10);
                            dataGridView1.Rows[0].Cells[5].Value = Math.Round(mape, 10);
                            dataGridView1.Rows[0].Cells[6].Value = Math.Round(mae, 10);

                            richTextBox1.Text = "Y = " + Math.Round(u, 3).ToString() + "*(x1)^(" + Math.Round(v, 3).ToString() + ")*(x2)^"+"("+ Math.Round(w,3).ToString()+")";
                        }
                        else
                            MessageBox.Show("Dört veya daha fazla satır girmelisiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    else if (rakamVirgulHataTipi == 2)
                        MessageBox.Show("Hatalı virgül kullanımı yaptınız.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 3)
                        MessageBox.Show("Rakam kullanmadan virgül kullanamazsınız.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 4)
                        MessageBox.Show("Veri seti içerisinde 0 bulunmamalıdır.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 5)
                        MessageBox.Show("Sadece rakam veya virgül kullanabilirsiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (degiskenSayisi == 1)
                {
                    if (rakamVirgulHataTipi == 1)
                    {
                        double[][] x1 = new double[yVeriSayisi][];
                        double[] y = new double[yVeriSayisi];
                        double[] x = new double[yVeriSayisi];
                        double max = -1;
                        for (int i = 0; i < yVeriSayisi; i++)
                        {
                            x[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                            x1[i] = new double[1];
                            x1[i][0] = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                            y[i] = Convert.ToDouble(dataGridView1.Rows[i].Cells[0].Value);
                            if (Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value) > max)
                                max = Convert.ToDouble(dataGridView1.Rows[i].Cells[1].Value);
                        }

                        if(y.Length>=4)
                        {
                            var z_hat = y.Select(r => Math.Log(r)).ToArray();
                            double[] p_hat = Fit.LinearMultiDim(x1, z_hat, d => 1.0, d => Math.Log(d[0]));
                            double u = Math.Exp(p_hat[0]);
                            double v = p_hat[1];

                            double[] xs = new double[Convert.ToInt32((Math.Truncate(max)/0.1)+15)];
                            double[] ys = new double[Convert.ToInt32((Math.Truncate(max) / 0.1) + 15)];
                            int j = 0;
                            for (float i = 0F; i < Math.Truncate(max) + 1.5; i += 0.1F)
                            {
                                if (j == xs.Length)
                                    break;
                                else
                                {
                                    xs[j] = i;
                                    ys[j] = u * Math.Pow((double)i, v);
                                }
                                
                                j++;
                            }

                            formsPlot1.Plot.AddScatterLines(xs, ys, Color.Red, 2);
                            formsPlot1.Plot.AddScatter(x, y, lineWidth: 0, color: Color.Black);
                            formsPlot1.Plot.Title("Y = "+Math.Round(u,3).ToString()+"*(x)^("+Math.Round(v,3).ToString()+")");
                            formsPlot1.Plot.XLabel("X");
                            formsPlot1.Plot.YLabel("Y");
                            for (int i = 0; i < yVeriSayisi; i++)
                            {
                                mse += Math.Pow(y[i] - (Math.Round(u, 3) * Math.Pow(Math.Round(x1[i][0], 3), v)), 2) / yVeriSayisi;
                                mape += 100 * Math.Abs(y[i] - (Math.Round(u, 3) * Math.Pow(Math.Round(x1[i][0], 3), v))) / (yVeriSayisi * Math.Abs(y[i]));
                                mae += Math.Abs(y[i] - (Math.Round(u, 3) * Math.Pow(Math.Round(x1[i][0], 3), v))) / yVeriSayisi;
                            }
                            dataGridView1.Rows[0].Cells[4].Value = Math.Round(mse, 10);
                            dataGridView1.Rows[0].Cells[5].Value = Math.Round(mape, 10);
                            dataGridView1.Rows[0].Cells[6].Value = Math.Round(mae, 10);

                            richTextBox1.Text = "Y = " + Math.Round(u, 3).ToString() + "*(x)^(" + Math.Round(v, 3).ToString() + ")";
                            formsPlot1.Refresh(); // Nan hatasına bak
                        }
                        else
                            MessageBox.Show("Dört veya daha fazla satır girmelisiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    else if (rakamVirgulHataTipi == 2)
                        MessageBox.Show("Hatalı virgül kullanımı yaptınız.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 3)
                        MessageBox.Show("Rakam kullanmadan virgül kullanamazsınız.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 4)
                        MessageBox.Show("Veri seti içerisinde 0 bulunmamalıdır.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (rakamVirgulHataTipi == 5)
                        MessageBox.Show("Sadece rakam veya virgül kullanabilirsiniz.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                else if (degiskenSayisi == -1)
                {
                    MessageBox.Show("Eksik veri girişi olmamalı, veri girişi ilk satırdan başlamalı ve ilk sütun ile son sütun arasında boş sütun bulunmamalı.", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Fonksiyon seçmelisiniz", "HATA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            




        }

        
    }
}

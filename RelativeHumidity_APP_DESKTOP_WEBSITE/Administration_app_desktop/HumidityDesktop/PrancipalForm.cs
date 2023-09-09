using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Microsoft.Office.Interop.Excel;

namespace HumidityDesktop
{
    public partial class PrancipalForm : Form
    {
        HumidityCalculatorEntities db = new HumidityCalculatorEntities();
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
           (
               int nLeftRect,     // x-coordinate of upper-left corner
               int nTopRect,      // y-coordinate of upper-left corner
               int nRightRect,    // x-coordinate of lower-right corner
               int nBottomRect,   // y-coordinate of lower-right corner
               int nWidthEllipse, // height of ellipse
               int nHeightEllipse // width of ellipse
           );
        public PrancipalForm()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 50, 50));
            bunifuFormDock1.SubscribeControlToDragEvents(bunifuGradientPanel1);
        }

       

        private void PrancipalForm_Load(object sender, EventArgs e)
        {
            bunifuLabel5.Text = "COPYRIGHT © " + DateTime.Now.Year + " ABHGZR, ";
           
            RemplireLesGrid();
        }

        private void RemplireLesGrid()
        {
            var obser = (from o in db.Observateurs
                         select new
                         {
                             ID = o.ObservateurId,
                             NomPrenom = o.NomPrenomObservateur,
                         });
            datagridvObs.DataSource = obser.ToList();
            var Bas = (from b in db.Bassins
                       select new
                       {
                           ID = b.BassinId,
                           NomBassin = b.NomBassin,
                       });
            datagridvB.DataSource = Bas.ToList();
            var sta = (from s in db.Stations
                       select new
                       {
                           ID = s.StationId,
                           NomStation = s.NomStation,
                       });
            datagridvStat.DataSource = sta.ToList();

            var rhs = (from rh in db.RelativeHumidities
                       select new
                       {
                           ID = rh.RelativeHumidityId,
                           Sec = rh.Sec,
                           Mou = rh.Mou,
                           Hum = rh.Hum,
                           Heure = rh.Heur,
                           MAX = rh.ThermometreMax,
                           MIN = rh.ThermometreMin,
                           MOY = Math.Round(rh.ThermometreMoyMaxMin, 2),
                           MA = rh.ThermometreMA,
                           MI = rh.ThermometreMI,
                           Date = rh.DateObservation
                       });
            datagridvHumidity.DataSource = rhs.ToList();
            
            if (comboBoxObs.Items.Count > 0)
            {
                comboBoxObs.Items.Clear();
            }
            else
            {
                foreach (var ob in obser)
                {
                    comboBoxObs.Items.Add(ob.ID + " " + ob.NomPrenom);
                }
            }

            if (comboBoxStatImport.Items.Count > 0)
            {
                comboBoxStatImport.Items.Clear();
                comboBoxStat.Items.Clear();
            }
            else
            {
                foreach (var st in sta)
                {
                    comboBoxStatImport.Items.Add(st.ID + " " + st.NomStation);
                    comboBoxStat.Items.Add(st.ID + " " + st.NomStation);
                }
            }
        } 

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("Tableau");
            
        }

        private void bunifuButton2_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("Humidite");
        }

        private void bunifuButton3_Click(object sender, EventArgs e)
        {
            bunifuPages1.SetPage("Imprimer");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            bunifuPages1.SetPage("Contact");
        }

        private void datagridvObs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                textBoxCodeObs.Text = datagridvObs.Rows[e.RowIndex].Cells[0].Value.ToString();
                textBoxNomObs.Text = datagridvObs.Rows[e.RowIndex].Cells[1].Value == null ? "" : datagridvObs.Rows[e.RowIndex].Cells[1].Value.ToString();
            }
        }

        private void datagridvStat_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                textBoxNomStat.Text = datagridvStat.Rows[e.RowIndex].Cells[1].Value.ToString();
                textBoxCodeStat.Text = datagridvStat.Rows[e.RowIndex].Cells[0].Value.ToString();
        
            }
        }

        private void datagridvB_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                textBoxNomBas.Text = datagridvB.Rows[e.RowIndex].Cells[1].Value.ToString();
                textBoxCodeBas.Text = datagridvB.Rows[e.RowIndex].Cells[0].Value.ToString();
            }
        }

        private async void BtnAjObs_Click(object sender, EventArgs e)
        {
            Observateur observateur = new Observateur();
            try
            {
                observateur.NomPrenomObservateur = textBoxNomObs.Text;
                observateur.StationId = Convert.ToInt32(textBoxCodeStat.Text);
                db.Observateurs.Add(observateur);
                await db.SaveChangesAsync();
                SuccesMsg S = new SuccesMsg();
                S.Show();
            }
            catch (Exception)
            {
                ErrorMsg E = new ErrorMsg();
                E.msg = "Station";
                E.Show();
            }
            RemplireLesGrid();
        }

        private async void BtnSObs_Click(object sender, EventArgs e)
        {
            if (textBoxCodeObs.Text != null && textBoxCodeObs.Text.Length > 0)
            {
                int id = Convert.ToInt32(textBoxCodeObs.Text);
                Observateur observateur = db.Observateurs.SingleOrDefault(ob => ob.ObservateurId == id);
                db.Observateurs.Remove(observateur);
                await db.SaveChangesAsync();
                RemplireLesGrid();
                SuccesMsg S = new SuccesMsg();
                S.Show();
            }
            else
            {
                ErrorMsg E = new ErrorMsg();
                E.msg = "Observateur";
                E.Show();
            }
        }

        private async void BtnMObs_Click(object sender, EventArgs e)
        {
            if (textBoxCodeObs.Text != null && textBoxCodeObs.Text.Length > 0)
            {
                int id = Convert.ToInt32(textBoxCodeObs.Text);
                Observateur observateur = db.Observateurs.SingleOrDefault(ob => ob.ObservateurId == id);
                observateur.NomPrenomObservateur = textBoxNomObs.Text;
                await db.SaveChangesAsync();
                RemplireLesGrid();
                SuccesMsg S = new SuccesMsg();
                S.Show();
            } else
            {
                MessageBox.Show("Invalid Nom");
            }
        }

        private void BtnReObs_Click(object sender, EventArgs e)
        {
            RemplireLesGrid();
            string txt = textBoxNomObs.Text.ToLower();
            if (txt != "" || txt != null)
            {
                for (int i = 0; i < datagridvObs.Rows.Count; i++)
                {
                    if (datagridvObs.Rows[i].Cells[1].Value != null && datagridvObs.Rows[i].Cells[1].Value.ToString().ToLower().Contains(txt.ToLower()))
                    {
                        datagridvObs.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(39, 174, 96);
                        break;
                    }
                }
            }
        }

        private async void BtnAStat_Click(object sender, EventArgs e)
        {
            Station station = new Station();
            try
            {
                station.NomStation = textBoxNomStat.Text;
                station.BassinId = Convert.ToInt32(textBoxCodeBas.Text);
                db.Stations.Add(station);
                await db.SaveChangesAsync();
                SuccesMsg S = new SuccesMsg();
                S.Show();
            }
            catch (Exception)
            {
                ErrorMsg E = new ErrorMsg();
                E.msg = "Bassin";
                E.Show();
            }
            RemplireLesGrid();
        }

        private async void BtnMStat_Click(object sender, EventArgs e)
        {
            if (textBoxCodeStat.Text != null && textBoxCodeStat.Text.Length > 0)
            {
                int id = Convert.ToInt32(textBoxCodeStat.Text);
                Station station = db.Stations.SingleOrDefault(st => st.StationId == id);
                station.NomStation = textBoxNomStat.Text;
                await db.SaveChangesAsync();
                RemplireLesGrid();
                SuccesMsg S = new SuccesMsg();
                S.Show();
            } else
            {
                MessageBox.Show("Invalid Nom");
            }
        }

        private async void BtnSuprStat_Click(object sender, EventArgs e)
        {
            if (textBoxCodeStat.Text != null && textBoxCodeStat.Text.Length > 0)
            {
                int id = Convert.ToInt32(textBoxCodeStat.Text);
                Station station = db.Stations.SingleOrDefault(st => st.StationId == id);
                db.Stations.Remove(station);
                await db.SaveChangesAsync();
                RemplireLesGrid();
                SuccesMsg S = new SuccesMsg();
                S.Show();
            } else
            {
                ErrorMsg E = new ErrorMsg();
                E.msg = "Station";
                E.Show();
            }
        }

        private void BtnReStat_Click(object sender, EventArgs e)
        {
            RemplireLesGrid();
            string txt = textBoxNomStat.Text.ToLower();
            if (txt != "" || txt != null)
            {
                for (int i = 0; i < datagridvStat.Rows.Count; i++)
                {
                    if (datagridvStat.Rows[i].Cells[1].Value != null && datagridvStat.Rows[i].Cells[1].Value.ToString().ToLower().Contains(txt.ToLower()))
                    {
                        datagridvStat.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(39, 174, 96);
                        break;
                    }
                }
            }
        }

        private async void BtnAjB_Click(object sender, EventArgs e)
        {
            Bassin bassin = new Bassin();
            try
            {
                bassin.NomBassin = textBoxNomBas.Text;
                db.Bassins.Add(bassin);
                await db.SaveChangesAsync();
                SuccesMsg S = new SuccesMsg();
                S.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            RemplireLesGrid();
        }

        private async void BtnMB_Click(object sender, EventArgs e)
        {
            if (textBoxCodeBas.Text != null && textBoxCodeBas.Text.Length > 0)
            {
                int id = Convert.ToInt32(textBoxCodeBas.Text);
                Bassin bassin = db.Bassins.SingleOrDefault(bs => bs.BassinId == id);
                bassin.NomBassin = textBoxNomBas.Text;
                await db.SaveChangesAsync();
                RemplireLesGrid();
                SuccesMsg S = new SuccesMsg();
                S.Show();
            }
            else
            {
                MessageBox.Show("Invalid Nom");
            }
        }

        private async void BtnSB_Click(object sender, EventArgs e)
        {
            if (textBoxCodeBas.Text != null && textBoxCodeBas.Text.Length > 0)
            {
                int id = Convert.ToInt32(textBoxCodeBas.Text);
                Bassin bassin = db.Bassins.SingleOrDefault(bs => bs.BassinId == id);
                db.Bassins.Remove(bassin);
                await db.SaveChangesAsync();
                RemplireLesGrid();
                SuccesMsg S = new SuccesMsg();
                S.Show();
            } else
            {
                ErrorMsg E = new ErrorMsg();
                E.msg = "Bassin";
                E.Show();
            }
        }

        private void BtnReB_Click(object sender, EventArgs e)
        {
            RemplireLesGrid();
            string txt = textBoxNomBas.Text.ToLower();
            if (txt != "" || txt != null)
            {
                for (int i = 0; i < datagridvB.Rows.Count; i++)
                {
                    if (datagridvB.Rows[i].Cells[1].Value != null && datagridvB.Rows[i].Cells[1].Value.ToString().ToLower().Contains(txt.ToLower()))
                    {
                        datagridvB.Rows[i].DefaultCellStyle.BackColor = Color.FromArgb(39, 174, 96);
                        break;
                    }
                }
            }
        }

        private async void BtnAjouter_Click(object sender, EventArgs e)
        {
            RelativeHumidity humidity = new RelativeHumidity();
            try
            {
                if (comboBoxHeur.SelectedItem != null && comboBoxObs.SelectedItem != null && comboBoxStat.SelectedItem != null)
                {
                    humidity.DateObservation = datePicker.Value;
                    humidity.Heur = Convert.ToInt32(comboBoxHeur.SelectedItem.ToString());
                    humidity.Sec = (float)Convert.ToDouble(txtSec.Text);
                    humidity.Mou = (float)Convert.ToDouble(txtMou.Text);
                    humidity.Hum = (float)Convert.ToDouble(txtHum.Text);
                    humidity.ThermometreMA = txtMa.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMa.Text);
                    humidity.ThermometreMax = txtMax.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMax.Text);
                    humidity.ThermometreMoyMaxMin = txtMoy.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMoy.Text);
                    humidity.ThermometreMin = txtMin.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMin.Text);
                    humidity.ThermometreMI = txtMi.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMi.Text);
                    humidity.ObservateurId = Convert.ToInt32(comboBoxObs.SelectedItem.ToString().Split(' ')[0]);
                    humidity.StationId = Convert.ToInt32(comboBoxStat.SelectedItem.ToString().Split(' ')[0]);
                    db.RelativeHumidities.Add(humidity);
                    await db.SaveChangesAsync();
                    RemplireLesGrid();
                    SuccesMsg S = new SuccesMsg();
                    S.Show();
                } else
                {
                    ErrorMsg E = new ErrorMsg();
                    E.msg = "heure ou bien un station et un observateure";
                    E.Show();
                }

            }
            catch (Exception)
            {
                ErrorMsg E = new ErrorMsg();
                E.msg = "station et un observateure";
                E.Show();
            }
        }

        private void datagridvHumidity_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtID.Text = datagridvHumidity.Rows[e.RowIndex].Cells[0].Value.ToString();
                txtSec.Text = datagridvHumidity.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtMou.Text = datagridvHumidity.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtHum.Text = datagridvHumidity.Rows[e.RowIndex].Cells[3].Value.ToString();
                comboBoxHeur.Text = datagridvHumidity.Rows[e.RowIndex].Cells[4].Value.ToString();
                txtMax.Text = datagridvHumidity.Rows[e.RowIndex].Cells[5].Value.ToString();
                txtMin.Text = datagridvHumidity.Rows[e.RowIndex].Cells[6].Value.ToString();
                txtMoy.Text = datagridvHumidity.Rows[e.RowIndex].Cells[7].Value.ToString();
                txtMa.Text = datagridvHumidity.Rows[e.RowIndex].Cells[8].Value.ToString();
                txtMi.Text = datagridvHumidity.Rows[e.RowIndex].Cells[9].Value.ToString();
                datePicker.Text = datagridvHumidity.Rows[e.RowIndex].Cells[10].Value.ToString();
            }
        }

        private async void BtnModifier_Click(object sender, EventArgs e)
        {
            if (txtID.Text == null || txtID.Text.Length <= 0)
            {
                ErrorMsg E = new ErrorMsg();
                E.msg = "heure ou bien un station et un enregistrement";
                E.Show();
            } else
            {
                int id = Convert.ToInt32(txtID.Text);
                RelativeHumidity humidity = db.RelativeHumidities.SingleOrDefault(h => h.RelativeHumidityId == id);
                try
                {
                    if (comboBoxHeur.SelectedItem != null && comboBoxObs.SelectedItem != null && comboBoxStat.SelectedItem != null)
                    {
                        humidity.DateObservation = datePicker.Value;
                        humidity.Heur = Convert.ToInt32(comboBoxHeur.SelectedItem.ToString());
                        humidity.Sec = (float)Convert.ToDouble(txtSec.Text);
                        humidity.Mou = (float)Convert.ToDouble(txtMou.Text);
                        humidity.Hum = (float)Convert.ToDouble(txtHum.Text);
                        humidity.ThermometreMA = txtMa.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMa.Text);
                        humidity.ThermometreMax = txtMax.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMax.Text);
                        humidity.ThermometreMoyMaxMin = txtMoy.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMoy.Text);
                        humidity.ThermometreMin = txtMin.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMin.Text);
                        humidity.ThermometreMI = txtMi.Text.Length <= 0 ? 0 : (float)Convert.ToDouble(txtMi.Text);
                        humidity.ObservateurId = Convert.ToInt32(comboBoxObs.SelectedItem.ToString().Split(' ')[0]);
                        humidity.StationId = Convert.ToInt32(comboBoxStat.SelectedItem.ToString().Split(' ')[0]);
                        await db.SaveChangesAsync();
                        RemplireLesGrid();
                        SuccesMsg S = new SuccesMsg();
                        S.Show();
                    }
                    else
                    {
                        ErrorMsg E = new ErrorMsg();
                        E.msg = "station et un observateure";
                        E.Show();
                    }
                }
                catch (Exception)
                {
                    ErrorMsg E = new ErrorMsg();
                    E.msg = "station et un observateure";
                    E.Show();
                }
            }
        }

        private void BtnImprimer_Click(object sender, EventArgs e)
        {
            if (comboBoxStatImport.SelectedItem == null || moieImporter.Value == null)
            {
                ErrorMsg E = new ErrorMsg();
                E.msg = "station";
                E.Show();
            }
            else
            {
                int id = Convert.ToInt32(comboBoxStatImport.SelectedItem.ToString().Split(' ')[0]);
                int month = moieImporter.Value.Month;
                var stations = db.RelativeHumidities.Where(hm => hm.StationId == id).ToList();
                var moi = stations.Where(hm => hm.DateObservation.Month == month).ToList();
                var h7 = (from rh in moi
                          where rh.Heur == 7
                          select new
                          {
                              Sec = rh.Sec,
                              Mou = rh.Mou,
                              Hum = rh.Hum,
                              date = rh.DateObservation
                          }).ToList();
                var h14 = (from rh in moi
                           where rh.Heur == 14
                           select new
                           {
                               Sec = rh.Sec,
                               Mou = rh.Mou,
                               Hum = rh.Hum,
                               date = rh.DateObservation
                           }).ToList();
                var h18 = (from rh in moi
                           where rh.Heur == 18
                           select new
                           {
                               Sec = rh.Sec,
                               Mou = rh.Mou,
                               Hum = rh.Hum,
                               date = rh.DateObservation
                           }).ToList();
                var h21 = (from rh in moi
                           where rh.Heur == 21
                           select new
                           {
                               Sec = rh.Sec,
                               Mou = rh.Mou,
                               Hum = rh.Hum,
                               MAX = rh.ThermometreMax,
                               MIN = rh.ThermometreMin,
                               MOY = Math.Round(rh.ThermometreMoyMaxMin, 2),
                               MA = rh.ThermometreMA,
                               MI = rh.ThermometreMI,
                               date = rh.DateObservation
                           }).ToList();

                if (moi.ToList().Count > 0)
                {

                    Microsoft.Office.Interop.Excel.Application xcelApp = new Microsoft.Office.Interop.Excel.Application();
                    xcelApp.Application.Workbooks.Add(Type.Missing);
                    string[] str = { "Jour", "Sec", "Mou", "Hum", "Sec", "Mou", "Hum", "Sec", "Mou", "Hum", "Sec", "Mou", "Hum", "Max", "Min", "Moy", "MA", "MI" };
                    string[] heure = { "", "", "7H", "", "", "14H", "", "", "18H", "", "", "21H", "", "", "", "21", "Heures", "", "" };
                    for (int i = 0; i < heure.Length; i++)
                    {
                        xcelApp.Cells[1, i + 1] = heure[i];
                    }
                    for (int i = 0; i < str.Length; i++)
                    {
                        xcelApp.Cells[2, i + 1] = str[i];
                    }

                    int days = DateTime.DaysInMonth(moieImporter.Value.Year, moieImporter.Value.Month);

                    for (int i = 1; i <= days; i++)
                    {
                        xcelApp.Cells[i + 2, 1] = i;
                    }

                    for (int i = 0; i < h7.Count; i++)
                    {
                        int day = h7[i].date.Day;
                        xcelApp.Cells[2 + day, 2] = h7[i].Sec.ToString();
                        xcelApp.Cells[2 + day, 3] = h7[i].Mou.ToString();
                        xcelApp.Cells[2 + day, 4] = h7[i].Hum.ToString();
                    }

                    for (int i = 0; i < h14.Count; i++)
                    {
                        int day = h14[i].date.Day;
                        xcelApp.Cells[2 + day, 5] = h14[i].Sec.ToString();
                        xcelApp.Cells[2 + day, 6] = h14[i].Mou.ToString();
                        xcelApp.Cells[2 + day, 7] = h14[i].Hum.ToString();
                    }

                    for (int i = 0; i < h18.Count; i++)
                    {
                        int day = h18[i].date.Day;
                        xcelApp.Cells[2 + day, 8] = h18[i].Sec.ToString();
                        xcelApp.Cells[2 + day, 9] = h18[i].Mou.ToString();
                        xcelApp.Cells[2 + day, 10] = h18[i].Hum.ToString();
                    }

                    for (int i = 0; i < h21.Count; i++)
                    {
                        int day = h21[i].date.Day;
                        xcelApp.Cells[2 + day, 11] = h21[i].Sec.ToString();
                        xcelApp.Cells[2 + day, 12] = h21[i].Mou.ToString();
                        xcelApp.Cells[2 + day, 13] = h21[i].Hum.ToString();
                        xcelApp.Cells[2 + day, 14] = h21[i].MAX.ToString();
                        xcelApp.Cells[2 + day, 15] = h21[i].MIN.ToString();
                        xcelApp.Cells[2 + day, 16] = h21[i].MOY.ToString();
                        xcelApp.Cells[2 + day, 17] = h21[i].MA.ToString();
                        xcelApp.Cells[2 + day, 18] = h21[i].MI.ToString();
                    }


                    xcelApp.Columns.AutoFit();
                    xcelApp.Visible = true;
                }
                else
                {
                    MessageBox.Show("Il n'y a pas d'enregistrements dans cette station","Information", MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
        }

        private async void BtnSupprimer_Click(object sender, EventArgs e)
        {
            if (txtID.Text == null || txtID.Text.Length <= 0)
            {
                ErrorMsg E = new ErrorMsg();
                E.msg = "enregistrement";
                E.Show();
            }
            else
            {
                int id = Convert.ToInt32(txtID.Text);
                RelativeHumidity humidity = db.RelativeHumidities.SingleOrDefault(h => h.RelativeHumidityId == id);
                try
                {
                    db.RelativeHumidities.Remove(humidity);
                    await db.SaveChangesAsync();
                    RemplireLesGrid();
                    SuccesMsg S = new SuccesMsg();
                    S.Show();
                }
                catch (Exception)
                {
                    ErrorMsg E = new ErrorMsg();
                    E.msg = "station et un observateure";
                    E.Show();
                }
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/meggouriIsmail");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Process.Start("https://ma.linkedin.com/in/ismail-meggouri-7437a71b4");
       
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.instagram.com/ismail_meggouri/?hl=en");
        
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.facebook.com/el.meggo");
       
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/BRAAMSIF");
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            Process.Start("https://ma.linkedin.com/in/brahim-amssayef-08080317b");
        }

        private void pictureBox9_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.instagram.com/baamif");
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.facebook.com/bra.amsif");
        }
    }
}

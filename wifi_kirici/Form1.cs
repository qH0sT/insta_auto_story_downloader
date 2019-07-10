using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Text;
using HtmlAgilityPack;
using System;
using System.Linq;
namespace wifi_kirici
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            if (File.Exists("data.base")) { Oku(); }
            
        }
       enum Durum
        {
            Hikaye_Yok,
            User_Yok,
            İndi
        }
        public delegate void _dt(byte[] bt, string nick);
        public void dt(byte[] bit, string s)
        {
            dataGridView1.Rows.Add(bit, s);
        }
        int g = 0;
        int dos = 0;
        bool calis = false;
        Dictionary<string, Thread> dic = new Dictionary<string, Thread>();
        List<string> kontrolled = new List<string>();
        public void indir(WebClient wc, ListViewItem lv)
        {
            while (true) {
                try
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    
                    IEnumerable<DataGridViewRow> dtgr = dataGridView1.Rows.Cast<DataGridViewRow>().Where(b => b.Cells[1].Value.ToString() == lv.Text);
                    if (dtgr.Count() == 0)
                    {
                        try
                        {

                            WebClient profil_resmi = new WebClient();
                            string pf_resmi = profil_resmi.DownloadString("https://www.instadp.com/profile/" + lv.Text);
                            if (!pf_resmi.Contains("This profile doesn't exist, check your username spelling") && !pf_resmi.Contains("No stories posted on the profile"))
                            {
                                HtmlAgilityPack.HtmlDocument _dokuman_ = new HtmlAgilityPack.HtmlDocument();
                                _dokuman_.LoadHtml(pf_resmi);

                                foreach (HtmlNode linke in _dokuman_.DocumentNode.SelectNodes("//a[@href]"))
                                {
                                    if (linke.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("https://scontent"))
                                    {
                                        Invoke(new _dt(dt), profil_resmi.DownloadData(linke.GetAttributeValue("href", "Gösterilecek veri yok.")),lv.Text);
                                    }

                                }
                            }
                        }
                        catch (Exception) { dataGridView1.Rows.Add(System.Drawing.Image.FromFile("nichts.png"), "profil_resmi_yüklenemedi"); }
                    }
                    ////Hikaye Kısmı////
                    wc = new WebClient();
                    WebClient _wc_ = new WebClient();
                    _wc_.Encoding = Encoding.Default;
                    string h = _wc_.DownloadString("https://www.instadp.com/stories/" + lv.Text);
                    if (!h.Contains("This profile doesn't exist, check your username spelling") && !h.Contains("No stories posted on the profile"))
                    {
                        HtmlAgilityPack.HtmlDocument _dokuman_ = new HtmlAgilityPack.HtmlDocument();
                        _dokuman_.LoadHtml(h);
                       
                        foreach (HtmlNode link in _dokuman_.DocumentNode.SelectNodes("//a[@href]"))
                        {
                           
                            if (link.GetAttributeValue("href", "Gösterilecek veri yok.").Contains("https://scontent"))
                            {

                                string resim_veya_video = link.GetAttributeValue("href", "Gösterilecek veri yok.");
                                if (resim_veya_video.Contains("&_nc_ig_catcb=1")) { resim_veya_video = resim_veya_video.Replace("&_nc_ig_catcb=1", ""); }

                                if (!Directory.Exists(Environment.CurrentDirectory + @"\Hikayeler\" + lv.Text)) { Directory.CreateDirectory(Environment.CurrentDirectory + @"\Hikayeler\" + lv.Text); }
                               
                                if (!kontrolled.Contains(resim_veya_video))
                                    {
                                    if (!resim_veya_video.Contains(".jpg"))
                                       {
                                        g = g + 1;
                                        IEnumerable<ListViewItem> aytim = listView1.Items.Cast<ListViewItem>().Where(c => c.SubItems[0].Text == lv.Text);
                                        if (aytim.Count() > 0)
                                        {
                                            ListViewItem links = aytim.FirstOrDefault();
                                            links.ImageIndex = 4;
                                            links.SubItems[1].Text = g.ToString() + ".Hikaye İniyor..";
                                        }
                                        File.WriteAllBytes(Environment.CurrentDirectory + @"\Hikayeler\" + lv.Text + @"\" + lv.Text + "_"+ dos++ + ".mp4", wc.DownloadData(link.GetAttributeValue("href", "Gösterilecek veri yok.")));
                                        IEnumerable<ListViewItem> aytime = listView1.Items.Cast<ListViewItem>().Where(m => m.SubItems[0].Text == lv.Text);
                                        if (aytime.Count() > 0)
                                        {
                                            ListViewItem aytimm = aytime.FirstOrDefault();
                                            aytimm.ImageIndex = (int)Durum.İndi;
                                            aytimm.SubItems[1].Text = g.ToString() + ".Hikaye İndi.";
                                        }
                                        if (checkBox1.Checked) { 
                                        Invoke(new force_popup(show_popup), "İndirme Tamamlandı", lv.Text + " adlı kişinin "+ g.ToString() + ".hikayesi indi.");
                                        }
                                        kontrolled.Add(resim_veya_video);
                                        break;
                                    }
                                    else
                                    {
                                        if (!kontrolled.Contains(resim_veya_video))
                                        {
                                            g = g + 1;
                                            IEnumerable<ListViewItem> aytim = listView1.Items.Cast<ListViewItem>().Where(y => y.SubItems[0].Text == lv.Text);
                                            if (aytim.Count() > 0)
                                            {
                                                ListViewItem linque = aytim.FirstOrDefault();
                                                linque.ImageIndex = 4;
                                                linque.SubItems[1].Text = g.ToString() + ".Hikaye İniyor..";
                                            }
                                            File.WriteAllBytes(Environment.CurrentDirectory + @"\Hikayeler\" + lv.Text + @"\" + lv.Text + "_" + dos++ + ".jpg", wc.DownloadData(link.GetAttributeValue("href", "Gösterilecek veri yok.")));
                                            IEnumerable<ListViewItem> aytime = listView1.Items.Cast<ListViewItem>().Where(a => a.SubItems[0].Text == lv.Text);
                                            if (aytime.Count() > 0)
                                            {
                                                ListViewItem item = aytime.FirstOrDefault();
                                                item.ImageIndex = (int)Durum.İndi;
                                                item.SubItems[1].Text = g.ToString() + ".Hikaye İndi.";
                                            }
                                            if (checkBox1.Checked)
                                            {
                                                Invoke(new force_popup(show_popup), "İndirme Tamamlandı", lv.Text + " adlı kişinin " + g.ToString() + ".hikayesi indi.");
                                            }
                                            kontrolled.Add(resim_veya_video);
                                            break;
                                        }

                                    }
                                   
                                }
                                

                            }
                          
                        }
      
                    }
                    else if (h.Contains("This profile doesn't exist, check your username spelling")) { lv.ImageIndex = (int)Durum.User_Yok; lv.SubItems[1].Text = "Böyle bir profil yok."; }
                    else if (h.Contains("No stories posted on the profile")) { lv.ImageIndex = (int)Durum.Hikaye_Yok; lv.SubItems[1].Text = "Güncel hikaye yok."; }
                    if (!dic.ContainsKey(lv.Text))
                    {
                        dic.Add(lv.Text, th);
                    }

                }
                catch (Exception ex)
                {
                
                    IEnumerable<ListViewItem> aytim2 = listView1.Items.Cast<ListViewItem>().Where(v => v.SubItems[0].Text == lv.Text);
                    if (aytim2.Count() > 0)
                    {
                        ListViewItem item = aytim2.FirstOrDefault();
                        item.ImageIndex = (int)Durum.User_Yok;
                        item.SubItems[1].Text = ex.Message;

                    }
                    th.Abort();
                }

                Thread.Sleep(Convert.ToInt32(numericUpDown1.Value) * 1000);
                if(calis == false) { break; }
            }
           
           }
           Thread th;
        private void button1_Click(object sender, System.EventArgs e)
        {
            if (listView1.Items.Count > 0)
            {
                foreach (ListViewItem lvi in listView1.Items)
                {
                    th = new Thread(() => indir(new WebClient(), lvi));
                    th.Start();
                }
                calis = true;
                button2.Enabled = true;
                button1.Enabled = false;
                başlatToolStripMenuItem.Enabled = false;
                durdurToolStripMenuItem.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text)) { 

             textBox1.Text = textBox1.Text.ToLower().Replace("ı", "i")
             .Replace("ş", "s")
             .Replace("ö", "o")
             .Replace("ü", "u")
             .Replace("ğ", "g")
             .Replace(" ", "_");

            ListViewItem lvi = new ListViewItem(textBox1.Text);
            lvi.SubItems.Add("");
            lvi.ImageIndex = 5;
            listView1.Items.Add(lvi);
            if (button1.Enabled == false)
            {
                th = new Thread(() => indir(new WebClient(),lvi));
                th.Start();
            }
            Kaydet();
          }
        }
        bool op = true;
        private void gösterGizleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (op)
            { Opacity = 0; ShowInTaskbar = false; op = false; }
            else { Opacity = 100; ShowInTaskbar = true; op = true; }
               
        }

        private void başlatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1.PerformClick();
        }

        private void durdurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button2.PerformClick();
        }

        private void hakkındaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CODED BY 20071999 WITH C#\nWorks with HtmlAgilityPack\nWorks based on https://www.instadp.com/", "ABOUT", 0, (MessageBoxIcon)64);
        }

        private void türkhackteamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://turkhackteam.org/");
        }
        public delegate void force_popup(string title, string context);
        public void show_popup(string title, string context)
        {
            notifyIcon1.BalloonTipTitle = title;
            notifyIcon1.BalloonTipText = context;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon1.ShowBalloonTip(2000);
        }
        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count > 0)
            {
                foreach (KeyValuePair<string, Thread> th in dic)
                {

                    th.Value.Abort();

                }
                calis = false;
                dic.Clear();
                button2.Enabled = false;
                button1.Enabled = true;
                başlatToolStripMenuItem.Enabled = true;
                durdurToolStripMenuItem.Enabled = false;
            }
        }
        public void Oku()
        {
            try
            {
                string[] satirlar = File.ReadAllLines(Environment.CurrentDirectory + "\\data.base");
                foreach (string satir in satirlar)
                {
                    string[] aytimlar = satir.Split('|');
                    ListViewItem lv = new ListViewItem(aytimlar[0]);
                    lv.ImageIndex = 5;
                    lv.SubItems.Add(aytimlar[1]);
                    listView1.Items.Add(lv);
                }
            }
            catch (Exception) { }
        }
        public void Kaydet()
        {
           
                try
                {
                    using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\data.base"))
                    {
                        foreach (ListViewItem item in listView1.Items)
                        {

                            sw.WriteLine("{0}{1}",
                            item.Text, "|"
                            );
                        }
                    }
                }
                catch (Exception) { }

        }
        private void kaldırToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count > 0 && listView1.SelectedItems.Count > 0)
            {
                if (listView1.SelectedItems[0].SubItems[1].Text != "")
                {
                    if (dic.Count > 0)
                    {
                        foreach (KeyValuePair<string, Thread> th in dic)
                        {
                            if (listView1.SelectedItems.Count > 0)
                            {
                                if (th.Key == listView1.SelectedItems[0].Text)
                                {
                                    th.Value.Abort();
                                    IEnumerable<DataGridViewRow> aytime = dataGridView1.Rows.Cast<DataGridViewRow>().Where(s => s.Cells[1].Value.ToString() == listView1.SelectedItems[0].Text);
                                    if (aytime.Count() > 0)
                                    {
                                        
                                        dataGridView1.Rows.Remove(aytime.FirstOrDefault());
                                    }
                                    listView1.Items.Remove(listView1.SelectedItems[0]);
                                    Kaydet();

                                }
                                button1.Enabled = true;
                                button2.Enabled = false;
                            }
                        }
                    }
                    else {
                        IEnumerable<DataGridViewRow> aytime = dataGridView1.Rows.Cast<DataGridViewRow>().Where(t => t.Cells[1].Value.ToString() == listView1.SelectedItems[0].Text);
                        if (aytime.Count() > 0)
                        {
                            dataGridView1.Rows.Remove(aytime.FirstOrDefault());
                        }
                        listView1.Items.Remove(listView1.SelectedItems[0]); Kaydet(); }

                }
                else if (listView1.SelectedItems[0].SubItems[1].Text == "") {
                    IEnumerable<DataGridViewRow> aytime = dataGridView1.Rows.Cast<DataGridViewRow>().Where(q => q.Cells[1].Value.ToString() == listView1.SelectedItems[0].Text);
                    if (aytime.Count() > 0)
                    {
                        dataGridView1.Rows.Remove(aytime.FirstOrDefault());
                    }
                    listView1.Items.Remove(listView1.SelectedItems[0]); Kaydet(); }

            }
        }

        private void hikayeKlasörünüAçToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count == 1 && Directory.Exists(Environment.CurrentDirectory + @"\Hikayeler\" + listView1.SelectedItems[0].Text))
            Process.Start(Environment.CurrentDirectory+@"\Hikayeler\"+listView1.SelectedItems[0].Text);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Kaydet();
            Environment.Exit(0);
        }
    }
}

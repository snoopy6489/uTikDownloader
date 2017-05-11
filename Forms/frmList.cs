using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using libNUS.WiiU;

namespace uTikDownloadHelper
{
    public partial class frmList : Form
    {
        TitleList titles = new TitleList();
        String myExe = System.Reflection.Assembly.GetEntryAssembly().Location;
        List<TitleInfo> dataSource = new List<TitleInfo> { };
        Dictionary<string, string> titleSizes = Common.Settings.cachedSizes;

        public frmList()
        {
            InitializeComponent();
        }

        private void populateList()
        {
            lstMain.Items.Clear();
            List<TitleInfo> titleList = titleList = titles.filter(txtSearch.Text, comboRegion.SelectedItem.ToString());

            foreach (TitleInfo title in titleList)
            {
                lstMain.Items.Add(title.getListViewItem());
            }
            frmList_SizeChanged(null, null);
            enableDisableDownloadButton();
        }

        private void frmList_SizeChanged(object sender, EventArgs e)
        {
            lstMain.BeginUpdate();
            lstMain.Columns[4].Width = -1;
            lstMain.Columns[2].Width = lstMain.Width - lstMain.Columns[0].Width - lstMain.Columns[1].Width - lstMain.Columns[3].Width - lstMain.Columns[4].Width - 4 - SystemInformation.VerticalScrollBarWidth;
            lstMain.EndUpdate();
        }

        private void enableDisableDownloadButton()
        {
            if (lstMain.SelectedItems.Count > 0)
            {
                btnDownload.Enabled = true;
            }
            else
            {
                btnDownload.Enabled = false;
            }
        }

        private void frmList_Load(object sender, EventArgs e)
        {
            this.lblLoading.Location = lstMain.Location;
            this.lblLoading.Size = lstMain.Size;
            btnTitleKeyCheck.Location = lstMain.Location;
            btnTitleKeyCheck.Size = lstMain.Size;

            if (Common.Settings.ticketWebsite != null && Common.Settings.ticketWebsite.Length > 0)
                btnTitleKeyCheck.Dispose();

            titles.ListUpdated += (object send, EventArgs ev) =>
            {
                comboRegion.Items.Clear();
                comboRegion.Items.Add("Any");
                foreach (TitleInfo title in titles.titles)
                {
                    if (!comboRegion.Items.Contains(title.region) && title.region.Length > 0)
                    {
                        comboRegion.Items.Add(title.region);
                    }
                }
                String lastRegion = Common.Settings.lastSelectedRegion;
                if (comboRegion.Items.Contains(lastRegion))
                {
                    comboRegion.SelectedIndex = comboRegion.Items.IndexOf(lastRegion);
                }
                else
                {
                    comboRegion.SelectedIndex = 0;
                }
                comboRegion.Enabled = true;
                txtSearch.Enabled = true;
                lstMain.Enabled = true;
                lblLoading.Dispose();
            };
        }

        public void getSizes()
        {
            var titlesCopy = titles.titles.Where(o => !titleSizes.Keys.Contains(o.titleID)).ToList();
            var titlesCopy2 = titles.titles.Where(o => titleSizes.Keys.Contains(o.titleID)).ToList();
            List<Task> workers = new List<Task> { };
            for (var i=0; i < 8; i++)
            {
                Task.Run(async () =>{
                    while(true)
                    {
                        TitleInfo item;
                        lock (titlesCopy)
                        {
                            if (titlesCopy.Count == 0)
                                break;
                            
                            item = titlesCopy[0];
                            titlesCopy.RemoveAt(0);
                        }

                        String contentSize;
                        titleSizes.TryGetValue(item.titleID, out contentSize);
                        if (contentSize == null || contentSize.Length == 0)
                        {
                            try
                            {
                                contentSize = HelperFunctions.SizeSuffix((await NUS.DownloadTMD(item.titleID)).TitleContentSize);
                            }
                            catch(Exception ex)
                            {
                                Debug.WriteLine(ex?.Message);
                                Debug.WriteLine(ex?.InnerException?.Message);
                                contentSize = "";
                            }

                            if (titleSizes.ContainsKey(item.titleID) == false && contentSize != "")
                            {
                                titleSizes.Add(item.titleID, contentSize);
                            }
                        }

                        if(contentSize != "")
                            this.Invoke((MethodInvoker)delegate
                            {
                                item.size = contentSize;
                                frmList_SizeChanged(null, null);
                            });
                    }
                });
            }
            Task.Run(() =>
            {
                this.Invoke((MethodInvoker)delegate
                {
                    lstMain.BeginUpdate();
                    foreach (TitleInfo item in titlesCopy2)
                    {
                        string size = "";
                        titleSizes.TryGetValue(item.titleID, out size);
                        if (size != null)
                            item.size = size;

                    }

                    frmList_SizeChanged(null, null);
                    lstMain.EndUpdate();
                });
                while (workers.Count > 0) ;

                Common.Settings.cachedSizes = titleSizes;
            });
        }

        private async void getTitleList()
        {
            var status = await titles.getTitleList();
            if(status != TitleList.TitleListResult.NotFound)
                getSizes();

            if (status == TitleList.TitleListResult.Offline)
                MessageBox.Show(Localization.Strings.UnableToContactKeyWebsite + "\n\n" + Localization.Strings.ViewingCachedData);

            if (status == TitleList.TitleListResult.NotFound && lblLoading != null)
                lblLoading.Text = Localization.Strings.Error;
        }

        private void frmList_Shown(object sender, EventArgs e)
        {
            if (Common.Settings.ticketWebsite != null && Common.Settings.ticketWebsite.Length > 0)
            {
                getTitleList();
            }
        }

        private void comboRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateList();
            Common.Settings.lastSelectedRegion = comboRegion.SelectedItem.ToString();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (ofdTik.ShowDialog() == DialogResult.OK && ofdTik.FileNames.Count() > 0)
            {
                List<TitleInfo> list = new List<TitleInfo> { };
                foreach (string filename in ofdTik.FileNames)
                {
                    byte[] data = File.ReadAllBytes(filename);
                    string hexID = HelperFunctions.getTitleIDFromTicket(data);
                    string basename = System.IO.Path.GetFileNameWithoutExtension(filename);
                    TitleInfo info = new TitleInfo(hexID, "", (basename.ToLower() != "title" ? hexID + " - " + basename : hexID), "", "", true);
                    info.ticket = data;
                    list.Add(info);
                }
                frmDownload.OpenDownloadForm(list);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            lstMain.BeginUpdate();
            populateList();
            lstMain.EndUpdate();
        }

        private void lstMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            enableDisableDownloadButton();
        }

        private void lstMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            handleDownload(sender, e);
        }

        private void handleDownload(object sender, EventArgs e)
        {
            if (lstMain.SelectedItems.Count == 0)
                return;

            var list = new List<TitleInfo> { };
            foreach(ListViewItem item in lstMain.SelectedItems)
            {
                list.Add(((TitleInfo)item.Tag));
            }

            frmDownload.OpenDownloadForm(list);
        }

        private async void btnTitleKeyCheck_Click(object sender, EventArgs e)
        {
            String website = Microsoft.VisualBasic.Interaction.InputBox(Localization.Strings.WhatIsTheAddress + "\n\n" + Localization.Strings.TitleKeyWebsiteName + "\n\n" + Localization.Strings.JustTypeTheHostname, Localization.Strings.AnswerThisQuestion, "", -1, -1).ToLower();
            
            if (Common.getMD5Hash(website) == "3cbe074f2b138e86d15cd9a5e1082893") {
                Common.Settings.ticketWebsite = website;
                ((Button)sender).Dispose();
                getTitleList();
            }
        }
    }
}

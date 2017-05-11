using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;
using System.Collections.Generic;
using libNUS.WiiU;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;

namespace uTikDownloadHelper
{
    public partial class frmDownload: Form
    {
        public struct DownloadItem
        {
            public readonly byte[] ticket;
            public readonly TMD tmd;
            public readonly string name;
            public readonly NUS.UrlFilenamePair[] URLs;
            public readonly string absolutePath;
            public object tag;
            public bool madeTicket;
            public DownloadItem(string name, TMD tmd, NUS.UrlFilenamePair[] URLs, byte[] ticket, string absolutePath = null, object tag = null, bool madeTicket = false)
            {
                this.name = name;
                this.tmd = tmd;
                this.ticket = ticket;
                this.URLs = URLs;
                this.absolutePath = absolutePath;
                this.tag = tag;
                this.madeTicket = madeTicket;
            }
        }
        public enum DownloadType {
            None = 0,
            Game = 1,
            Update = 2,
            DLC = 4,
            All = Game | Update | DLC
        }
        public List<TitleInfo> TitleQueue = new List<TitleInfo> { };
        public DownloadType AutoDownloadType = DownloadType.None;
        public bool AutoClose = false;
        public string DownloadPath;
        public bool Downloading { get; private set; } = false;

        private Process runningProcess;
        private bool isClosing = false;
        internal List<DownloadItem> DownloadQueue = new List<DownloadItem> { };
        private Nullable<DownloadItem> TitleItem = null;
        private Nullable<DownloadItem> UpdateItem = null;
        private Nullable<DownloadItem> DLCItem = null;
        private bool TitleExists = false;
        private bool UpdateExists = false;
        private bool DLCExists = false;
        private Stopwatch stopwatch1 = new LapStopwatch();
        private Stopwatch stopwatch2 = new LapStopwatch();
        private long dataDownloadedSinceLastTick = 0;
        private long dataToDownload;
        private long completeFileDataDownloaded;
        private FileInfo CurrentFile;
        public frmDownload()
        {
            InitializeComponent();
        }

        public static void DownloadMissing(string tmdPath, bool autoClose = false)
        {
            if (!File.Exists(tmdPath))
                return;

            FileInfo info = new FileInfo(tmdPath);

            TMD tmd = new TMD(File.ReadAllBytes(tmdPath));
            var onlineTMDTask = NUS.DownloadTMD(tmd.TitleID);
            onlineTMDTask.Wait();
            TMD onlineTMD = onlineTMDTask.Result;


            if (!tmd.rawBytes.SequenceEqual(onlineTMD.rawBytes)) {
                MessageBox.Show(Localization.Strings.RepairFailed, Localization.Strings.Error);
                return;
            }

            List<DownloadItem> queue = new List<DownloadItem> { };
            var URLTask = NUS.GetTitleContentURLs(tmd, true);
            URLTask.Wait();
            NUS.UrlFilenamePair[] URLs = URLTask.Result;

            AppDomain.CurrentDomain.DoCallBack(() =>
            {
                frmDownload frm = new frmDownload();
                frm.DownloadQueue.Add(new DownloadItem(tmd.TitleID, tmd, URLs, new byte[] { }, info.DirectoryName));
                frm.AutoClose = autoClose;
                frm.DownloadPath = info.DirectoryName;
                frm.lblDownloadingMetadata.Dispose();
                Program.FormContext.AddForm(frm);
            });
        }

        public static void OpenDownloadForm(List<TitleInfo> list)
        {
            Action<DialogTitlePatch.Result> openDownload = (DialogTitlePatch.Result e) =>
            {
                AppDomain.CurrentDomain.DoCallBack(() =>
                {
                    frmDownload frm = new frmDownload();
                    frm.TitleQueue = list;
                    if (list.Count > 1)
                    {
                        frm.AutoClose = true;

                        if (e.Game)
                            frm.AutoDownloadType |= frmDownload.DownloadType.Game;

                        if (e.Update)
                            frm.AutoDownloadType |= frmDownload.DownloadType.Update;

                        if (e.DLC)
                            frm.AutoDownloadType |= frmDownload.DownloadType.DLC;

                        if (frm.ChooseFolder() == false)
                            return;
                    }

                    Program.FormContext.AddForm(frm);
                    frm.BringToFront();
                });
            };

            if (list.Count > 1)
            {
                DialogTitlePatch dialog = new DialogTitlePatch();
                dialog.OKClicked += (object s, DialogTitlePatch.Result e) => {
                    openDownload(e);
                };
                dialog.ShowDialog();
            }
            else
            {
                openDownload(new DialogTitlePatch.Result());
            }

        }

        private void progressTimer_Tick(object sender, EventArgs e)
        {
            if (!Directory.Exists(DownloadPath))
                return;

            CurrentFile.Refresh();
            long dirSize = completeFileDataDownloaded + CurrentFile.Length;

            double progress = (double)dirSize / (double)dataToDownload;

            if (progress > 1.0)
                progress = 1.0;

            progMain.Value = Convert.ToInt32(progress * (double)progMain.Maximum);

            // Transfer speed since last tick
            long transferred = dirSize - dataDownloadedSinceLastTick;
            dataDownloadedSinceLastTick = dirSize;
            if (transferred > 0 && stopwatch1.ElapsedMilliseconds > 0)
            {
                long transferRate = Convert.ToInt64((1000.0 / (double)(stopwatch1.ElapsedMilliseconds)) * (double)transferred);
                lblTransferRate.Text = HelperFunctions.SizeSuffix(transferRate) + "ps";
            }
            stopwatch1.Restart();

            // Average speed for title
            if (stopwatch2.ElapsedMilliseconds > 0)
            {
                long avg = Convert.ToInt64((double)dirSize / ((double)stopwatch2.ElapsedMilliseconds / 1000.0));
                lblAvgTransferRate.Text = HelperFunctions.SizeSuffix(Convert.ToInt64(avg)) + "ps";

                if (avg > 0)
                {
                    // Time remaining
                    TimeSpan remainingDuration = new TimeSpan(0, 0, (int)((dataToDownload - dirSize) / avg));
                    lblTimeRemaining.Text = remainingDuration.ToString("g");
                }
            }

            
        }

        struct TaskQueueValues
        {
            public TitleInfo info;
            public DownloadType type;
            public TaskQueueValues(TitleInfo info, DownloadType type)
            {
                this.info = info;
                this.type = type;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadQueue.Clear();
            if (chkTitle.Checked)
                DownloadQueue.Add(TitleItem.Value);

            if (chkUpdate.Checked)
                DownloadQueue.Add(UpdateItem.Value);

            if (chkDLC.Checked)
                DownloadQueue.Add(DLCItem.Value);

            ProcessDownloadQueue(DownloadQueue.ToArray());
        }

        private void adjustProgMainValue(int value)
        {
            progMain.Invoke((MethodInvoker)delegate {
                if(progMain.Value + value <= progMain.Maximum && progMain.Value + value >= progMain.Minimum)
                progMain.Value += value;
            });
        }

        private async void frmDownload_Shown(object sender, EventArgs e)
        {
            BringToFront();
            if (TitleQueue.Count == 0)
            {
                if (DownloadQueue.Count > 0)
                {
                    ProcessDownloadQueue(DownloadQueue.ToArray());
                }
                else
                {
                    Close();
                }

                return;
            }

            if (AutoDownloadType == DownloadType.None)
            {
                TitleInfo QueueItem = TitleQueue[0];
                this.Text = QueueItem.displayName;

                if (!QueueItem.isUpdate) { 
                    await Task.Run(() => {
                        var result = getDownloadItem(QueueItem, DownloadType.Game);
                        if (result.HasValue && result.Value.ticket.Length > 0)
                        {
                            TitleItem = result.Value;
                            TitleExists = true;
                        }
                    });
                }

                if (QueueItem.dlcKey.Length > 0)
                {
                    await Task.Run(() =>
                    {
                        var result = getDownloadItem(QueueItem, DownloadType.DLC);
                        if (result.HasValue)
                        {
                            DLCItem = result.Value;
                            DLCExists = true;
                        }
                    });
                }

                await Task.Run(() =>
                {
                    var result = getDownloadItem(QueueItem, DownloadType.Update);
                    if (result.HasValue)
                    {
                        UpdateItem = result.Value;
                        UpdateExists = true;
                    }
                });

                if (TitleExists)
                {
                    chkTitle.Enabled = true;
                    chkTitle.Checked = true;
                    if (TitleItem.Value.madeTicket)
                        chkTitle.ForeColor = Color.Red;
                }

                if (DLCExists)
                {
                    chkDLC.Enabled = true;
                    chkDLC.ForeColor = Color.Red;
                }
                
                if (UpdateExists)
                {
                    chkUpdate.Enabled = true;
                }
                

                btnDownload.Enabled = TitleExists || UpdateExists || DLCExists;
                lblDownloadingMetadata.Dispose();
            } else
            {
                var previousMax = progMain.Maximum;
                List<TaskQueueValues> taskValues = new List<TaskQueueValues> { };
                for(int i = 0; i < TitleQueue.Count; i++)
                {
                    var item = TitleQueue[i];

                    if ((AutoDownloadType & DownloadType.Game) != 0)
                    {
                        taskValues.Add(new TaskQueueValues(item, DownloadType.Game));
                    }

                    if ((AutoDownloadType & DownloadType.DLC) != 0)
                    {
                        taskValues.Add(new TaskQueueValues(item, DownloadType.DLC));
                    }

                    if ((AutoDownloadType & DownloadType.Update) != 0)
                    {
                        taskValues.Add(new TaskQueueValues(item, DownloadType.Update));
                    }
                }
                progMain.Maximum = taskValues.Count();

                int runningWorkers = 0;

                for (int i = 0; i < 4; i++)
                {
                    runningWorkers++;

                    #pragma warning disable CS4014
                    Task.Run(() =>
                    #pragma warning restore CS4014
                    {
                        TaskQueueValues value;
                        while (true)
                        {
                            lock (taskValues)
                            {
                                if (taskValues.Count == 0)
                                    break;

                                value = taskValues[0];
                                taskValues.RemoveAt(0);
                            }

                            var result = getDownloadItem(value.info, value.type);
                            if (result.HasValue)
                            {
                                DownloadQueue.Add(result.Value);
                            }

                            adjustProgMainValue(1);

                        }
                        runningWorkers--;
                    });
                }

                await Task.Run(() =>
                {
                    while(runningWorkers > 0)
                    {
                        Thread.Sleep(100);
                    }
                });

                progMain.Value = 0;
                progMain.Maximum = previousMax;
                lblDownloadingMetadata.Dispose();
            }
            if(DownloadQueue.Count > 0 || AutoClose)
            {
                ProcessDownloadQueue(DownloadQueue.ToArray());
            }
        }
        private Nullable<DownloadItem> getDownloadItem(TitleInfo item, DownloadType type)
        {
            TitleInfo info;
            
            lock (item)
            {
                info = item;
            }

            switch (type)
            {
                case DownloadType.Game:
                    if (!info.isUpdate)
                    {
                        try
                        {
                            bool madeTicket = false;
                            TMD titleTMD = AsyncHelpers.RunSync<TMD>(() => NUS.DownloadTMD(info.titleID));

                            if (info.ticket.Length == 0 && info.hasTicket)
                                info.ticket = AsyncHelpers.RunSync<byte[]>(() => HelperFunctions.DownloadTitleKeyWebsiteTicket(info.titleID));

                            
                            if (!info.hasTicket && info.titleKey.Length > 0)
                            {
                                madeTicket = true;
                                info.ticket = info.getGeneratedTitleTicket(titleTMD.TitleVersion);
                            }

                            return (new DownloadItem(
                                info.displayName + (madeTicket ? " (FakeSign)" : ""),
                                titleTMD,
                                AsyncHelpers.RunSync<NUS.UrlFilenamePair[]>(() => NUS.GetTitleContentURLs(titleTMD, true)),
                                info.ticket,
                                null,
                                null,
                                madeTicket
                            ));
                        }
                        catch { }
                    }
                    break;

                case DownloadType.DLC:
                    try
                    {
                        if (info.dlcKey.Length > 0)
                        {
                            TMD dlcTMD = AsyncHelpers.RunSync<TMD>(() => NUS.DownloadTMD(info.dlcID));

                            return (new DownloadItem(
                                info.DisplayNameWithVersion(dlcTMD.TitleVersion, "DLC") + " (FakeSign)",
                                dlcTMD,
                                AsyncHelpers.RunSync<NUS.UrlFilenamePair[]>(() => NUS.GetTitleContentURLs(dlcTMD, true)),
                                info.getDLCTicket(dlcTMD.TitleVersion),
                                null,
                                null,
                                true
                            ));
                        }
                    }
                    catch { }
                    break;

                case DownloadType.Update:
                    try
                    {
                        TMD updateTMD = AsyncHelpers.RunSync<TMD>(() => NUS.DownloadTMD(info.updateID));
                        return (new DownloadItem(
                            info.DisplayNameWithVersion(updateTMD.TitleVersion, "Update"),
                            updateTMD,
                            AsyncHelpers.RunSync<NUS.UrlFilenamePair[]>(() => NUS.GetTitleContentURLs(updateTMD, true)),
                            AsyncHelpers.RunSync<byte[]>(() => NUS.DownloadTicket(info.updateID))
                        ));
                    }
                    catch { }
                    break;
            }
            return null;
        }

        private async void ProcessDownloadQueue(DownloadItem[] items)
        {
            if(DownloadPath == null)
            {
                if (ChooseFolder() == false)
                    return;
            }

            string basePath = DownloadPath;
            Downloading = true;
            dataDownloadedSinceLastTick = 0;
            dataToDownload = 0;
            progressTimer.Enabled = true;
            int count = 0;
            string previousTitle = this.Text;
            stopwatch1.Start();
            stopwatch2.Start();
            var errors = new List<string> { };

            bool hideWget = Common.Settings.hideWget;
            bool shellExecute = Common.Settings.shellExecute;

            btnDownload.Enabled = false;
            foreach(DownloadItem title in DownloadQueue)
            {
                count++;
                completeFileDataDownloaded = 0;
                this.Text = "(" + count + "/" + DownloadQueue.Count + ")" + title.name;
                dataDownloadedSinceLastTick = 0;
                dataToDownload = title.tmd.TitleContentSize;

                var itemPath = HelperFunctions.GetAutoIncrementedDirectory(basePath, title.name, "title.tmd", HelperFunctions.md5sum(title.tmd.rawBytes));

                if (title.absolutePath != null)
                    itemPath = title.absolutePath;

                if(!Directory.Exists(itemPath))
                   Directory.CreateDirectory(itemPath);

                byte[] ticket = (title.ticket != null ? title.ticket : new byte[] { });

                File.WriteAllBytes(Path.Combine(itemPath, "title.tmd"), title.tmd.rawBytes);

                if (ticket.Length > 0)
                {
                    if (title.tmd.TitleID.ToLower()[7] != "e"[0])
                        HelperFunctions.patchTicket(ref ticket);

                    File.WriteAllBytes(Path.Combine(itemPath, "title.tik"), ticket);
                }

                if(!File.Exists(Path.Combine(itemPath, "title.cert")))
                    File.WriteAllBytes(Path.Combine(itemPath, "title.cert"), NUS.TitleCert);

                DownloadPath = itemPath;
                bool error = false;
                stopwatch1.Restart();
                stopwatch2.Restart();
                foreach (var url in title.URLs)
                {
                    string filePath = Path.Combine(itemPath, url.Filename);
                    if (!File.Exists(filePath))
                        File.Create(filePath).Close();

                    CurrentFile = new FileInfo(filePath);
                    lblCurrentFile.Text = url.Filename;
                    for (var i = 0; i < Common.Settings.downloadTries; i++)
                    {
                        int exitCode = await Task.Run(() => {

                            var procStIfo = new ProcessStartInfo();
                            procStIfo.FileName = Program.ResourceFiles.wget;
                            procStIfo.Arguments = HelperFunctions.escapeCommandArgument(url.URL) + " -c -O " + HelperFunctions.escapeCommandArgument(filePath);
                            procStIfo.UseShellExecute = shellExecute;
                            procStIfo.CreateNoWindow = hideWget;

                            if (shellExecute == true && hideWget == false)
                                procStIfo.WindowStyle = ProcessWindowStyle.Minimized;

                            runningProcess = new Process();
                            runningProcess.StartInfo = procStIfo;
                            runningProcess.Start();
                            runningProcess.WaitForExit();
                            return runningProcess.ExitCode;
                        });

                        if(!isClosing)
                            error = (exitCode != 0);

                        if (isClosing || exitCode == 0)
                            break;
                    }

                    if (isClosing || error)
                        break;


                    progressTimer_Tick(null, null);
                    CurrentFile.Refresh();
                    completeFileDataDownloaded += CurrentFile.Length;
                }
                if (error || isClosing)
                {
                    progressTimer.Enabled = false;
                    if(title.absolutePath == null)
                        if(error || MessageBox.Show(Localization.Strings.DeleteIncompleteFilesQuestion, Localization.Strings.IncompleteFiles, MessageBoxButtons.YesNo) == DialogResult.Yes)
                            Directory.Delete(itemPath, true);

                    if (!isClosing)
                        errors.Add(title.name);
                }
                if (isClosing)
                    break;
            }
            progressTimer.Enabled = false;
            stopwatch1.Stop();
            stopwatch2.Stop();
            Downloading = false;
            this.Text = previousTitle;
            DownloadPath = null;
            if(errors.Count > 0)
            {
                MessageBox.Show((errors.Count > 1 ? Localization.Strings.FollowingTitlesEncounteredAnErorr : Localization.Strings.FollowingTitleEnounteredAnError) + "\n\n" + String.Join("\n", errors.ToArray()), Localization.Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            } else
            {
                MessageBox.Show(Localization.Strings.DownloadsCompletedSuccessfully, Localization.Strings.Success);
            }
            btnDownload.Enabled = true;
            if (AutoClose && !isClosing)
                Close();
        }

        public bool ChooseFolder()
        {
            folderDialog.SelectedPath = Common.Settings.lastPath;
            if (folderDialog.ShowDialog() != DialogResult.OK)
                return false;

            DownloadPath = folderDialog.SelectedPath;
            Common.Settings.lastPath = folderDialog.SelectedPath;
            return true;
        }

        private void frmDownload_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Downloading)
            {
                if (MessageBox.Show(Localization.Strings.DownloadsInProgressCancelQuestion, Localization.Strings.DownloadsInProgress, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    isClosing = true;
                    if (runningProcess != null && runningProcess.HasExited == false)
                    {
                        runningProcess.Kill();
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void frmDownload_Load(object sender, EventArgs e)
        {
            this.lblDownloadingMetadata.Location = new System.Drawing.Point(12, 38);
            this.lblDownloadingMetadata.Size = new System.Drawing.Size(474,68);
        }
    }
}

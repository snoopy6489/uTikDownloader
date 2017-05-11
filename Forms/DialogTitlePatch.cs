using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uTikDownloadHelper
{
    public partial class DialogTitlePatch : Form
    {
        public struct Result
        {
            public bool Game;
            public bool Update;
            public bool DLC;
            public Result(bool Game, bool Update, bool DLC)
            {
                this.Game = Game;
                this.Update = Update;
                this.DLC = DLC;
            }
        }
        public DialogTitlePatch()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        public event EventHandler<Result> OKClicked;
        protected virtual void OnOKClicked(Result result)
        {
            OKClicked?.Invoke(this, result);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            OnOKClicked(new Result(chkGame.Checked, chkUpdate.Checked, chkDLC.Checked));
            btnCancel_Click(sender, e);
        }
    }
}

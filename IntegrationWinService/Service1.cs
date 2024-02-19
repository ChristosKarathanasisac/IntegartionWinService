using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace IntegrationWinService
{
    public partial class Service1 : ServiceBase
    {
        Timer tmrExecutor = new Timer();

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            tmrExecutor.Elapsed += new ElapsedEventHandler(tmrExecutor_Elapsed);
            tmrExecutor.Interval = 10;
           


            tmrExecutor.Enabled = true;
            tmrExecutor.Start();
        }

        private void tmrExecutor_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            tmrExecutor.Enabled = false;
            tmrExecutor.Stop();

            if (true)
            {
                IntegrationBuilder oUpdate = new IntegrationBuilder();
                oUpdate.StartUpdate();
            }

            tmrExecutor.Enabled = true;
            tmrExecutor.Start();
        }

        protected override void OnStop()
        {
            tmrExecutor.Enabled = false;
        }
    }
}

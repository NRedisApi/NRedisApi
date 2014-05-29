using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;

namespace NRedisApi.RedisWindowsService
{
    public partial class RedisService : ServiceBase
    {
        private Process _process;

        public RedisService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Run method for debugging use only
        /// </summary>
        public void RunService()
        {
            try
            {
                var bw = new BackgroundWorker();
                bw.DoWork += BwDoWork;
                bw.RunWorkerAsync();
                EventLog.WriteEntry("The Oscar Redis Service was started successfully.",
                                    EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format(@"Exception occurred in BwDoWork: {0}" + Environment.NewLine + @"{1}" + Environment.NewLine + @"{2}", ex.Message, ex.InnerException, ex.StackTrace), EventLogEntryType.Error);
            }

        }

        protected override void OnStart(string[] args)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += BwDoWork;
            bw.RunWorkerAsync();
            EventLog.WriteEntry("The Oscar Redis Service was started successfully.", EventLogEntryType.Information);
        }

        private void BwDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                using (_process = new Process())
                {
                    var info = new ProcessStartInfo(ConfigurationManager.AppSettings["RedisServerPath"])
                    {
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        ErrorDialog = false,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };
                    _process.StartInfo = info;
                    _process.Start();
                    _process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(string.Format(@"Exception occurred in BwDoWork: {0}" + Environment.NewLine + @"{1}" + Environment.NewLine + @"{2}", ex.Message, ex.InnerException, ex.StackTrace), EventLogEntryType.Error);
            }
            finally
            {
                Stop();
            }
        }

        protected override void OnStop()
        {
            _process.Kill();
            _process.Close();
            _process.Dispose();
            EventLog.WriteEntry("The Oscar Redis Service was stopped successfully.", EventLogEntryType.Information);
        }

        protected override void OnShutdown()
        {
            EventLog.WriteEntry("The Oscar Redis Service was shutdown successfully", EventLogEntryType.Information);
        }
    }
}

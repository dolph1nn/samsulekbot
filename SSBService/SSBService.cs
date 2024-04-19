#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
using System.ServiceProcess;

namespace SSB.Service
{
    public partial class SSBService : ServiceBase
    {
        public SSBService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Discord.DiscordHandler.Init();
        }

        protected override void OnStop()
        {
            Discord.DiscordHandler.Stop();
        }
    }
}
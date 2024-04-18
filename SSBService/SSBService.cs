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
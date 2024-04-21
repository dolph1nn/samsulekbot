using System;
using System.Threading.Tasks;
using System.Timers;

namespace SSB.Core
{
    public static class GlobalChat
    {
        public static ulong GlobalChatChannel = 0;
        private static DateTime LastChecked = DateTime.MinValue;
        private static bool Jank = true;
        public static async Task StartNewMessageCheck()
        {
            LastChecked = DateTime.Now;
            Timer T = new Timer(1000);
            T.Enabled = true;
            T.Elapsed += T_Elapsed;
        }
        private static async void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            await Database.DBHandler.CheckNewGlobalChatMessages(LastChecked);
            LastChecked = DateTime.Now;
        }
    }

    public class GlobalChatMessage
    {
        public int ID { get; set; }
        public DateTime Timestamp { get; set; }
        public string Source { get; set; }
        public string Author { get; set; }
        public string Message { get; set; }
    }
}
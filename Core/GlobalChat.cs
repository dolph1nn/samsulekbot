using System;
using System.Threading.Tasks;

namespace SSB.Core
{
    public static class GlobalChat
    {
        public static ulong GlobalChatChannel = 0;
        private static DateTime LastChecked = DateTime.MinValue;
        private static bool Jank = true;
        public static async Task StartNewMessageCheck()
        {
            while (Jank)
            {
                await Database.DBHandler.CheckNewGlobalChatMessages(LastChecked);
                LastChecked = DateTime.Now;
                await Task.Delay(500);
            }
        }
    }

    public class GlobalChatMessage
    {
        public int ID { get; }
        public DateTime Timestamp { get; }
        public string Source { get; }
        public string Author { get; }
        public string Message { get; }

        public GlobalChatMessage(int _ID, DateTime _Timestamp, string _Source, string _Author, string _Message)
        {
            ID = _ID;
            Timestamp = _Timestamp;
            Source = _Source;
            Author = _Author;
            Message = _Message;
        }
    }
}
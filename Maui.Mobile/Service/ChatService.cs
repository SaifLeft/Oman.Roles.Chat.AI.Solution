using MauiKit.Models.MainChat;

namespace Maui.Service
{
    // All the code in this file is included in all platforms.
    public class ChatService
    {
        static ChatService _instance;

        public static ChatService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ChatService();

                return _instance;
            }
        }

        public List<RecentlyChatCV> GetTransactions
        {
            get
            {
                return new List<RecentlyChatCV>
        {
            new RecentlyChatCV  {
            ImageIcon =MauiKitIcons.Database ,
            Title = "Salary",
            Date = "3:05 PM - Aug 22, 2022",
            Amount = 4789.89,
            IsCredited = true },
            new RecentlyChatCV  {
            ImageIcon =MauiKitIcons.VanUtility ,
            Title = "Salary",
            Date = "3:05 PM - Aug 22, 2022",
            Amount = 4789.89,
            IsCredited = true },
        };
            }
        }
    }
}

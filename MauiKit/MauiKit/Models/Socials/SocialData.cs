using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiKit.Models
{
    public class FriendData
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
    }

    public class ProfileData
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string About { get; set; }
        public string Avatar { get; set; }
        public string CoverImage { get; set; }
        public List<string> Images { get; set; }
        public List<FriendData> Friends { get; set; }
    }

    public class TimelineEventData
    {
        public string EventTitle { get; set; }
        public string EventDescription { get; set; }
        public string Image { get; set; }
        public string When { get; set; }
    }
}

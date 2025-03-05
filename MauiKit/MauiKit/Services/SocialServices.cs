using MauiKit.Models;

namespace MauiKit.Services
{
    public class SocialServices
    {
        static SocialServices _instance;
        public static SocialServices Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SocialServices();

                return _instance;
            }
        }

        readonly SocialUser user1 = new SocialUser
        {
            Name = "Alaya Cordova",
            Status = "Online",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-1.jpg",
            Color = Color.FromArgb("#FFE0EC")
        };
        readonly SocialUser user2 = new()
        {
            Name = "Cecily Trujillo",
            Status = "Online",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-2.jpg",
            Color = Color.FromArgb("#BFE9F2")
        };
        readonly SocialUser user3 = new()
        {
            Name = "Eathan Sheridan",
            Status = "Busy",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-3.jpg",
            Color = Color.FromArgb("#FFD6C4")
        };
        readonly SocialUser user4 = new()
        {
            Name = "Komal Orr",
            Status = "Busy",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-4.jpg",
            Color = Color.FromArgb("#C3C1E6")
        };
        readonly SocialUser user5 = new()
        {
            Name = "Sariba Abood",
            Status = "Offline",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-5.jpg",
            Color = Color.FromArgb("#FFE0EC")
        };
        readonly SocialUser user6 = new()
        {
            Name = "Justin O'Moore",
            Status = "Busy",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-6.jpg",
            Color = Color.FromArgb("#FFE5A6")
        };
        readonly SocialUser user7 = new()
        {
            Name = "Alissia Shah",
            Status = "Online",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-7.jpg",
            Color = Color.FromArgb("#FFE0EC")
        };
        readonly SocialUser user8 = new()
        {
            Name = "Antoni Whitney",
            Status = "Away",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-8.jpg",
            Color = Color.FromArgb("#FFE0EC")
        };
        readonly SocialUser user9 = new()
        {
            Name = "Jaime Zuniga",
            Status = "Online",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-9.jpg",
            Color = Color.FromArgb("#C3C1E6")
        };
        readonly SocialUser user10 = new()
        {
            Name = "Barbara Cherry",
            Status = "Offline",
            Image = AppSettings.ImageServerPath +  "ecommerce/users/150-10.jpg",
            Color = Color.FromArgb("#FF95A2")
        };

        public List<SocialUser> GetUsers()
        {
            return new List<SocialUser>
            {
                user1, user2, user3, user4, user5, user6, user7, user8, user9, user10
            };
        }
        public List<SocialMessage> GetChats()
        {
            return new List<SocialMessage>
            {
                new SocialMessage
                {
                  Sender = user5,
                  Time = "18:32",
                  Text = "Hey there! What\'s up? Is everything ok?",
                },
              new SocialMessage
              {
                Sender = user1,
                Time = "14:05",
                Text = "Can I call you back later?, I\'m in a meeting.",
              },
              new SocialMessage
              {
                Sender = user3,
                Time = "14:00",
                Text = "Yeah. Do you have any good song to recommend?",
              },
              new SocialMessage
              {
                Sender = user2,
                Time = "13:35",
                Text = "Hi! I went shopping today and found a nice t-shirt.",
              },
              new SocialMessage
              {
                Sender = user4,
                Time= "12:11",
                Text= "I passed you on the ride to work today, see you later.",
              },
              new SocialMessage
              {
                Sender = user6,
                Time = "14:00",
                Text = "Yeah. Do you have any good song to recommend?",
              },
              new SocialMessage
              {
                Sender = user7,
                Time = "13:35",
                Text = "Hi! I went shopping today and found a nice t-shirt.",
              },
              new SocialMessage
              {
                Sender = user8,
                Time= "12:11",
                Text= "I passed you on the ride to work today, see you later.",
              },
              new SocialMessage
              {
                Sender = user10,
                Time = "13:35",
                Text = "Hi! I went shopping today and found a nice t-shirt.",
              },
              new SocialMessage
              {
                Sender = user9,
                Time = "14:00",
                Text = "Yeah. Do you have any good song to recommend?",
              },
            };
        }
        public List<SocialMessage> GetMessages(SocialUser sender)
        {
            return new List<SocialMessage> {
              new SocialMessage
              {
                Sender = null,
                Time = "18:01",
                Text = "Hey there! What's up? Is everything ok?. 😂",
              },
              new SocialMessage
              {
                Sender = null,
                Time = "18:02",
                Text = "Can I call you back later?, I'm in a meeting.",
              },
              new SocialMessage
              {
                Sender = sender,
                Time = "18:05",
                Text = "Yeah. Do you have any good song to recommend?",
              },
              new SocialMessage
              {
                Sender = sender,
                Time = "18:06",
                Text = "Hi! I went shopping today and found a nice t-shirt. 😅",
              },
              new SocialMessage
              {
                Sender = null,
                Time = "18:07",
                Text = "I passed you on the ride to work today, see you later.",
              },
              new SocialMessage
              {
                Sender= sender,
                Time = "18:15",
                Text= "Ok thanks and thanks again!! This is really awesome",
              },
              new SocialMessage
              {
                Sender = null,
                Time = "18:17",
                Text = "You're always welcome yah! See you next Monday.",
              },
              new SocialMessage
              {
                Sender= sender,
                Time = "18:17",
                Text= "See you then.",
              },
            };
        }

        public ProfileData GetProfile()
        {
            return new ProfileData
            {
                Name = "Harley Devincen",
                Description = "Designer | Saint Louis CA",
                About = "I am a mobile app designer with a passion for creating beautiful and user-friendly applications. With a degree in graphic design and years of experience in the industry, I have honed my skills in design, user experience, and project management.",
                Avatar = AppSettings.ImageServerPath +  "ecommerce/users/150-13.jpg",
                CoverImage = AppSettings.ImageServerPath +  "15.jpg",
                Images = new List<string>()
                        {
                            "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/social/gallery_01.jpg",
                            "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/social/gallery_02.jpg",
                            "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/social/gallery_03.jpg",
                            "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/social/gallery_04.jpg",
                            "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/social/gallery_05.jpg",
                            "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/social/gallery_06.jpg",
                            "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/social/gallery_07.jpg",
                            "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/social/gallery_08.jpg",
                            "https://raw.githubusercontent.com/tlssoftware/raw-material/master/maui-kit/social/gallery_09.jpg"
                        },
                Friends = new List<FriendData>()
                        {
                            new FriendData()
                            {
                                Name = "Amanda Winston",
                                Avatar = AppSettings.ImageServerPath +  "avatars/150-2.jpg"
                            },
                            new FriendData()
                            {
                                Name = "Nijel Jonas",
                                Avatar = AppSettings.ImageServerPath +  "avatars/150-3.jpg"
                            },
                            new FriendData()
                            {
                                Name = "Jennifer Clarke",
                                Avatar = AppSettings.ImageServerPath +  "avatars/150-4.jpg"
                            },
                            new FriendData()
                            {
                                Name = "Dolph Green",
                                Avatar = AppSettings.ImageServerPath +  "avatars/150-5.jpg"
                            },
                            new FriendData()
                            {
                                Name = "Shee Huang Tee",
                                Avatar = AppSettings.ImageServerPath +  "avatars/150-6.jpg"
                            },
                            new FriendData()
                            {
                                Name = "Rocky",
                                Avatar = AppSettings.ImageServerPath +  "avatars/150-7.jpg"
                            },
                            new FriendData()
                            {
                                Name = "Eerie Johnson",
                                Avatar = AppSettings.ImageServerPath +  "avatars/150-8.jpg"
                            },
                            new FriendData()
                            {
                                Name = "John Obi Mikel",
                                Avatar = AppSettings.ImageServerPath +  "avatars/150-9.jpg"
                            },
                            new FriendData()
                            {
                                Name = "Xavier Doherty",
                                Avatar = AppSettings.ImageServerPath +  "avatars/150-10.jpg"
                            },
                        }
            };
        }

        public List<ContactData> GetContactFields()
        {
            return new List<ContactData>
            {
                new ContactData
                {
                    Label = "Name",
                    Value = "Harley Devincen"
                },
                new ContactData
                {
                    Label = "Address 1",
                    Value = "56437 South Westminster"
                },
                new ContactData
                {
                    Label = "Address 2",
                    Value = "2442 Boulevard Ave"
                },
                new ContactData
                {
                    Label = "Phone Number",
                    Value = "(+52) 654-2434 8784"
                },
                new ContactData
                {
                    Label = "Email",
                    Value = "harley-devi@mauikit.com"
                },
                new ContactData
                {
                    Label = "Organization",
                    Value = "TLS SOFTWARE"
                },
                new ContactData
                {
                    Label = "Country",
                    Value = "Australia"
                },
                new ContactData
                {
                    Label = "City",
                    Value = "Sydney"
                },
                new ContactData
                {
                    Label = "ZIP",
                    Value = "SY 67003"
                },
                new ContactData
                {
                    Label = "Notes",
                    Value = "What is on your mind? Share it with the world."
                },
            };
        }
    }
}
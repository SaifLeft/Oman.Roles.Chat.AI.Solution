namespace MauiKit.Models
{
    public class TravelMessage
    {
        public TravelUser Sender { get; set; }
        public TravelUser Receiver { get; set; }
        public string Text { get; set; }
        public string Time { get; set; }
    }
}
﻿
namespace MauiKit.Models.Ecommerce
{

    public class DeliveryStepsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string DateMonth { get; set; }
        public string Time { get; set; }
        public bool IsComplete { get; set; }
        public Color StatusColor { get { return IsComplete == true ? Color.FromArgb("#00C569") : Color.FromArgb("#919191"); } }
        public bool IsLineVisible { get { return Id < 5 ? true : false; } }

    }
}

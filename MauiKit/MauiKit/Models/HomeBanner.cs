using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiKit.Models
{
    public class HomeBanner
    {
        public string? Icon { get; set; }
        public string? BackgroundImage { get; set; }
        public LinearGradientBrush? BackgroundGradient { get; set; }
        public Color? BackgroundColor { get; set; }
        public Color? BackgroundGradientStart { get; set; }
        public Color? BackgroundGradientEnd { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        
    }
}

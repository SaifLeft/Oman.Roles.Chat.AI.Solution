using MauiStore.Data.GoogleMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiStore.Data
{
    public class StoreInfo
    {
        public int Id { get; set; }
        public string Avatar { get; set; }
        public string Image { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string Distance { get; set; }
        public GoogleMapPosition Location { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiStore.Data
{
    public class ProductItemInfo
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string ItemName { get; set; }
        public string Descriptions { get; set; }
        public string Price { get; set; }
        public string Discount { get; set; }
        public bool IsFavorite { get; set; }
        public bool OnSale { get; set; }
    }
}

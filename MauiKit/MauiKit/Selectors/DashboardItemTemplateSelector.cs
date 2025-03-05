using MauiKit.Models;

namespace MauiKit.Selectors
{
    public class DashboardItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NotificationItemTemplate { get; set; }
        public DataTemplate VariantItemTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (((DashboardVariantItem)item).IsNotification)
            {
                return NotificationItemTemplate;
            }
            else
            return VariantItemTemplate;
        }
    }
}

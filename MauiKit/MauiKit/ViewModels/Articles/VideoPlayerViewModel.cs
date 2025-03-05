
namespace MauiKit.ViewModels.Articles;
public  class VideoPlayerViewModel : BaseViewModel
{
    public VideoPlayerViewModel(ArticleData articleData)
    {
        ArticleData = articleData;
    }


    private ArticleData _articleData;
    public ArticleData ArticleData
    {
        get { return _articleData; }
        set
        {
            _articleData = value;
            OnPropertyChanged("ArticleData");
        }
    }
}

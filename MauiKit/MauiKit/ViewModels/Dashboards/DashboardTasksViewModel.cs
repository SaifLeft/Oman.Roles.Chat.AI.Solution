namespace MauiKit.ViewModels.Dashboards;
public partial class DashboardTasksViewModel : ObservableObject
{
    public ObservableCollection<DaskboardTaskModel> TaskList { get; set; } = new ObservableCollection<DaskboardTaskModel>();

    private List<DaskboardTaskModel> _allTaskList = new List<DaskboardTaskModel>();

    private DaskboardTaskModel _draggedItem;

    private DaskboardTaskModel _tappedItem;

    [ObservableProperty]
    private int _newTaskCount;

    [ObservableProperty]
    private int _inProgressCount;

    [ObservableProperty]
    private int _inReviewCount;

    [ObservableProperty]
    private int _doneCount;

    [ObservableProperty]
    private int _selectedOption;

    [ObservableProperty]
    private bool _isBusy;

    public DashboardTasksViewModel()
    {
        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 1, TaskName = "Website Development", TaskDescription = "Develop the website according to the design that has been created", Priority = "Mid", TaskStatus = (int)TaskStatusOption.NewTask,  OwnerName = "Alex", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user1.png", CreatedDate = "2d" });
        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 2, TaskName = "Website Development", TaskDescription = "Develop the Dashboard according to the design that has been created", Priority = "High", TaskStatus = (int)TaskStatusOption.NewTask, OwnerName = "Alex", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user1.png", CreatedDate = "2d" });
        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 3, TaskName = "Design Pitchdeck", TaskDescription = "Develop the Dashboard according to the design that has been created", Priority = "Low", TaskStatus = (int)TaskStatusOption.NewTask, OwnerName = "Alex", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user1.png", CreatedDate = "1d" });

        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 4, TaskName = "Wireframe", TaskDescription = "Please create a user flow and wireframe for a real estate sales agency", Priority = "Mid", TaskStatus = (int)TaskStatusOption.InProgress, OwnerName = "Alex", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user1.png", CreatedDate = "8hr" });
        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 5, TaskName = "User Flow", TaskDescription = "Please create a user flow and wireframe for a real estate sales agency", Priority = "Mid", TaskStatus = (int)TaskStatusOption.InProgress, OwnerName = "Alex", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user1.png", CreatedDate = "8hr" });

        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 6, TaskName = "Design System", TaskDescription = "Create 6 logo concept sketches along with their explainations", Priority = "Mid", TaskStatus = (int)TaskStatusOption.InReview, OwnerName = "Belvin", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user2.png", CreatedDate = "8hr" });
        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 7, TaskName = "Prototype Dashboard", TaskDescription = "Create a website design for a real estate company", Priority = "High", TaskStatus = (int)TaskStatusOption.InReview, OwnerName = "Belvin", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user2.png", CreatedDate = "2d" });
        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 8, TaskName = "Dashboard Design", TaskDescription = "Create a website design for a real estate company", Priority = "Mid", TaskStatus = (int)TaskStatusOption.InReview, OwnerName = "Belvin", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user2.png", CreatedDate = "1d" });
        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 9, TaskName = "Website Design", TaskDescription = "Create a website design for a real estate company", Priority = "Mid", TaskStatus = (int)TaskStatusOption.InReview, OwnerName = "Belvin", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user2.png", CreatedDate = "1d" });

        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 10, TaskName = "Brand Guide", TaskDescription = "Create a horizontal brand guide that follows the existing design", Priority = "Mid", TaskStatus = (int)TaskStatusOption.Completed, OwnerName = "Sarah", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user3.png", CreatedDate = "3d" });
        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 11, TaskName = "Brand Assets", TaskDescription = "Create print and digital branding for a real estate logo", Priority = "Mid", TaskStatus = (int)TaskStatusOption.Completed, OwnerName = "Sarah", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user3.png", CreatedDate = "1d" });
        _allTaskList.Add(new DaskboardTaskModel() { TaskId = 12, TaskName = "Logo Brainstorming", TaskDescription = "Create 6 logo concept sketches along with their explainations", Priority = "Low", TaskStatus = (int)TaskStatusOption.Completed, OwnerName = "Sarah", OwnerAvatar = AppSettings.ImageServerPath + "avatars/user3.png", CreatedDate = "8hr" });

        AddTaskList();
    }

    private void SetCount()
    {
        NewTaskCount = _allTaskList.Count(f => f.TaskStatus == (int)TaskStatusOption.NewTask);
        InProgressCount = _allTaskList.Count(f => f.TaskStatus == (int)TaskStatusOption.InProgress);
        InReviewCount = _allTaskList.Count(f => f.TaskStatus == (int)TaskStatusOption.InReview);
        DoneCount = _allTaskList.Count(f => f.TaskStatus == (int)TaskStatusOption.Completed);
    }

    private void AddTaskList()
    {
        var filterTaskList = _allTaskList.Where(f => f.TaskStatus == SelectedOption).ToList();

        TaskList.Clear();
        foreach (var task in filterTaskList)
        {
            TaskList.Add(task);
        }

        SetCount();
    }


    [RelayCommand]
    public void TaskTapped(DaskboardTaskModel task)
    {
        _tappedItem = task;
    }

    [RelayCommand]
    public void FilterTaskList(string optionStr)
    {
        int option = Convert.ToInt32(optionStr);
        SelectedOption = -1;
        SelectedOption = option;
        AddTaskList();
    }


    [RelayCommand]
    public void DragStarted(DaskboardTaskModel task)
    {
        _draggedItem = task;
    }

    [RelayCommand]
    public async void TaskDroped(string optionStr)
    {
        int option = Convert.ToInt32(optionStr);
        if (SelectedOption == option) return;

        IsBusy = true;
        // api call
        await Task.Delay(500);

        if (_draggedItem != null)
        {
            var currentItem = _allTaskList.Where(f => f.TaskId == _draggedItem.TaskId).FirstOrDefault();
            currentItem.TaskStatus = option;

            AddTaskList();
        }
        IsBusy = false;
    }
}

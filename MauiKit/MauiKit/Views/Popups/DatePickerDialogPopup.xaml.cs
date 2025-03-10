﻿
using PropertyChanged;
using XCalendar.Core.Extensions;

namespace MauiKit.Views.Popups;

[AddINotifyPropertyChangedInterface]
public partial class DatePickerDialogPopup : Popup
{
    #region Properties
    public DateTime InitialDate { get; }
    public DateTime SelectedDate { get; set; }
    public Calendar<CalendarDay> Calendar { get; } = new Calendar<CalendarDay>()
    {
        NavigatedDate = DateTime.Today,
        SelectedDates = new ObservableRangeCollection<DateTime>() { DateTime.Today },
        SelectionAction = SelectionAction.Replace,
        SelectionType = SelectionType.Single
    };
    #endregion

    #region Commands
    public ICommand ReturnSelectedDateCommand { get; set; }
    public ICommand ReturnInitialDateCommand { get; set; }
    public ICommand ResetNavigatedDateCommand { get; set; }
    public ICommand NavigateCalendarCommand { get; set; }
    public ICommand ChangeDateSelectionCommand { get; set; }
    #endregion

    #region Constructors
    public DatePickerDialogPopup(DateTime initialDate)
    {
        ReturnSelectedDateCommand = new Command(ReturnSelectedDate);
        ReturnInitialDateCommand = new Command(ReturnInitialDate);
        ResetNavigatedDateCommand = new Command(ResetNavigatedDate);
        NavigateCalendarCommand = new Command<int>(NavigateCalendar);
        ChangeDateSelectionCommand = new Command<DateTime>(ChangeDateSelection);

        Calendar.SelectedDates.CollectionChanged += SelectedDates_CollectionChanged;

        InitialDate = initialDate;
        ResultWhenUserTapsOutsideOfPopup = initialDate;
        Calendar.SelectedDates.Replace(initialDate);

        InitializeComponent();
        ResetNavigatedDate();
    }

    private void SelectedDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        SelectedDate = Calendar.SelectedDates.FirstOrDefault();
    }
    #endregion

    #region Methods
    public void ResetNavigatedDate()
    {
        Calendar.NavigatedDate = SelectedDate;
    }
    public void ReturnInitialDate()
    {
        Close(InitialDate);
    }
    public void ReturnSelectedDate()
    {
        Close(SelectedDate);
    }
    public void NavigateCalendar(int amount)
    {
        if (Calendar.NavigatedDate.TryAddMonths(amount, out DateTime targetDate))
        {
            Calendar.Navigate(targetDate - Calendar.NavigatedDate);
        }
        else
        {
            Calendar.Navigate(amount > 0 ? TimeSpan.MaxValue : TimeSpan.MinValue);
        }
    }
    public void ChangeDateSelection(DateTime dateTime)
    {
        Calendar?.ChangeDateSelection(dateTime);
    }
    #endregion
}
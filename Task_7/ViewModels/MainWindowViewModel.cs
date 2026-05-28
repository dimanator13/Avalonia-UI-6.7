using System;
using System.Linq;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Task_7.Models;

namespace Task_7.ViewModels;

public enum TaskFilter
{
    All,
    Active,
    Done,
    Overdue
}

public partial class MainWindowViewModel : ViewModelBase
{
    [NotifyPropertyChangedFor(nameof(HasSelectedTask))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    [ObservableProperty] private TaskItem? _selectedTask;
    public bool HasSelectedTask => SelectedTask is not null;
    [ObservableProperty] private string? _newTaskTitle;
    
    [ObservableProperty] private TaskFilter _selectedFilter = TaskFilter.All;
    [ObservableProperty] private string _searchText = string.Empty;
    
    public ObservableCollection<Priority> Priorities { get; } = new()
    {
        Priority.Low,
        Priority.Normal,
        Priority.High
    };
    
    public int TotalCount => Tasks.Count;

    public int DoneCount => Tasks.Count(task => task.IsDone);

    public int ActiveCount => Tasks.Count(task => !task.IsDone);

    public int OverdueCount => Tasks.Count(task => task.IsOverdue);

    public ObservableCollection<TaskItem> FilteredTasks { get; } = new();
    
    public ObservableCollection<TaskItem> Tasks { get; } = new()
    {
        new TaskItem("Task_1", "BlaBlaBla", false, Priority.Low, new DateTime(2026, 6, 1)),
        new TaskItem("Task_2", "BlaBlaBla", false, Priority.Normal, new DateTime(2026, 6, 1)),
        new TaskItem("Task_3", "BlaBlaBla", true, Priority.Low, new DateTime(2025, 6, 1))
    };

    public ObservableCollection<TaskFilter> Filters { get; } = new()
    {
        TaskFilter.All,
        TaskFilter.Active,
        TaskFilter.Done,
        TaskFilter.Overdue
    };
    
    public MainWindowViewModel()
    {
        RefreshFilteredTasks();
        RefreshCounters();
    }

    [RelayCommand]
    public void Add()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
            return;
        
        TaskItem task = new TaskItem(NewTaskTitle.Trim(), "BlaBlaBla", false, Priority.Low, DateTime.Now);
        
        Tasks.Add(task);
        SelectedTask = task;
        NewTaskTitle = string.Empty;
        RefreshFilteredTasks();
        RefreshCounters();
    }

    [RelayCommand(CanExecute = nameof(HasSelectedTask))]
    public void Delete()
    {
        if (SelectedTask != null)
        {
            Tasks.Remove(SelectedTask);
            SelectedTask = null;
            RefreshFilteredTasks();
            RefreshCounters();
        }
    }
    
    private void RefreshFilteredTasks()
    {
        FilteredTasks.Clear();

        foreach (var task in Tasks)
        {
            bool matchesFilter =
                SelectedFilter == TaskFilter.All ||
                SelectedFilter == TaskFilter.Active && !task.IsDone ||
                SelectedFilter == TaskFilter.Done && task.IsDone ||
                SelectedFilter == TaskFilter.Overdue && task.IsOverdue;

            bool matchesSearch =
                string.IsNullOrWhiteSpace(SearchText) ||
                task.Title?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                task.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true;

            if (matchesFilter && matchesSearch)
            {
                FilteredTasks.Add(task);
            }
        }
    }
    
    partial void OnSelectedFilterChanged(TaskFilter value)
    {
        RefreshFilteredTasks();
    }
    
    private void RefreshCounters()
    {
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(DoneCount));
        OnPropertyChanged(nameof(ActiveCount));
        OnPropertyChanged(nameof(OverdueCount));
    }
    
    partial void OnSearchTextChanged(string value)
    {
        RefreshFilteredTasks();
    }
}
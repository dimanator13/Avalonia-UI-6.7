using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Task_7.Models;

public enum Priority
{
    Low,
    Normal,
    High
}

public partial class TaskItem : ObservableObject
{
    [ObservableProperty] private string? _title;
    [ObservableProperty] private string? _description;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsOverdue))] private bool _isDone;
    [ObservableProperty] private Priority _priority;
    [ObservableProperty] [NotifyPropertyChangedFor(nameof(IsOverdue))] private DateTimeOffset _dueDate;
    [ObservableProperty] private DateTimeOffset _createdDate;
    
    public bool IsOverdue => DueDate.Date < DateTimeOffset.Now.Date && !IsDone;

    public TaskItem(string title, string description, bool isDone, Priority priority, DateTimeOffset dueDate)
    {
        Title = title;
        Description = description;
        IsDone = isDone;
        Priority = priority;
        DueDate = dueDate;
        CreatedDate = DateTimeOffset.Now;
    }
}
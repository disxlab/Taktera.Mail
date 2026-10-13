using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Taktera.Mail.Desktop.Models;
namespace Taktera.Mail.Desktop.ViewModels;
public partial class MainWindowViewModel : ObservableObject
{
    public ObservableCollection<MailItem> Messages { get; } = new()
    {
        new("Анна Петрова", "Договор на согласование", "Добрый день, направляю документ...", "10:24", "Добрый день!\n\nНаправляю документ для согласования.\n\nС уважением, Анна"),
        new("Команда проекта", "Обновление проекта", "Подготовили изменения...", "09:18", "Добрый день!\n\nПодготовили изменения по проекту."),
        new("Поддержка", "Статус обращения", "Ваш запрос обработан...", "Вчера", "Ваш запрос успешно обработан.")
    };
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedMessage))]
    private MailItem? selectedMessage;

    public bool HasSelectedMessage => SelectedMessage is not null;
    public bool IsInboxEmpty => Messages.Count == 0;

    public MainWindowViewModel()
    {
        SelectedMessage = Messages[0];
        Messages.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsInboxEmpty));
            if (SelectedMessage is not null && !Messages.Contains(SelectedMessage))
                SelectedMessage = null;
        };
    }
}

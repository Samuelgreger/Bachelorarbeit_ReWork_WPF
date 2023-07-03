using CommunityToolkit.Mvvm.ComponentModel;

namespace ReWork_WPF.ViewModels
{
    /// <summary>
    ///     Represents the base view model class that provides common functionality for view models.
    ///     Declares the <see cref="IsBusy" /> and <see cref="Title" /> properties.
    ///     These can be set or read.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        public bool isBusy;

        [ObservableProperty]
        public string title;

        public bool IsNotBusy => !IsBusy;
    }
}
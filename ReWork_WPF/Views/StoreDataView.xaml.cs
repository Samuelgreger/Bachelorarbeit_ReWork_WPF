using ReWork_WPF.ViewModels;
using System.Windows;

namespace ReWork_WPF.Views
{
    public partial class StoreDataView : Window
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StoreDataView"/> class with the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model associated with the view.</param>
        public StoreDataView(StoreDataViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
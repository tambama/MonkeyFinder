using MonkeyFinder.ViewModels;

namespace MonkeyFinder.Views;

public partial class MainPage : ContentPage
{
	public MainPage(MonkeyViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}


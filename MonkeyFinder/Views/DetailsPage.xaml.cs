using MonkeyFinder.ViewModels;

namespace MonkeyFinder.Views;

public partial class DetailsPage : ContentPage
{
	public DetailsPage(MonkeyDetailsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}


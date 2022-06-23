using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyFinder.Models;

namespace MonkeyFinder.ViewModels
{
	[QueryProperty("Monkey", "Monkey")]
    public partial class MonkeyDetailsViewModel : BaseViewModel
    {
        private readonly IMap _map;

        public MonkeyDetailsViewModel(IMap map)
        {
            Title = "The Monkey";
            _map = map;
        }

        [ObservableProperty]
        Monkey monkey;

        [ICommand]
        async Task OpenMapAsync()
        {
            try
            {
                await _map.OpenAsync(Monkey.Latitude, Monkey.Longitude,
                    new MapLaunchOptions
                    {
                        Name = Monkey.Name,
                        NavigationMode = NavigationMode.None
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Unable to open Map: {ex.Message}", "Ok");
            }
        }
    }
}


using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonkeyFinder.Models;
using MonkeyFinder.Services;
using MonkeyFinder.Views;

namespace MonkeyFinder.ViewModels
{
    public partial class MonkeyViewModel: BaseViewModel
	{
		MonkeyService _monkeyService;
        IConnectivity _connectivity;
        private readonly IGeolocation _geolocation;
        private readonly IMap _map;

        public MonkeyViewModel(MonkeyService monkeyService, IConnectivity connectivity, IGeolocation geolocation, IMap map)
        {
            Title = "Monkey Finder";
            _monkeyService = monkeyService;
            _connectivity = connectivity;
            _geolocation = geolocation;
            _map = map;
        }

        public ObservableCollection<Monkey> Monkeys { get; } = new();

        [ObservableProperty]
        bool isRefreshing;

        [ICommand]
        async Task GetClosestMonkeyAsync()
        {
            if (IsBusy || Monkeys.Count == 0)
                return;

            try
            {
                var location = await _geolocation.GetLastKnownLocationAsync();
                if(location is null)
                {
                    location = await _geolocation.GetLocationAsync(
                        new GeolocationRequest
                        {
                            DesiredAccuracy = GeolocationAccuracy.Medium,
                            Timeout = TimeSpan.FromSeconds(30)
                        });
                }

                if (location == null)
                    return;

                var first = Monkeys.OrderBy(m =>
                location.CalculateDistance(m.Latitude, m.Longitude, DistanceUnits.Kilometers))
                    .FirstOrDefault();

                if (first is null)
                    return;

                await Shell.Current.DisplayAlert("Closest Monkey",
                    $"{first.Name} in {first.Location}", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Unable to get closest monkey: {ex.Message}", "Ok");
            }
        }

        [ICommand]
        async Task GoToDetailsAsync(Monkey monkey)
        {
            if (monkey is null)
                return;

            await Shell.Current.GoToAsync($"{nameof(DetailsPage)}", true,
                new Dictionary<string, object>
                {
                    { "Monkey", monkey }
                });
        }

        [ICommand]
		async Task GetMonkeysAsync()
        {
			if (IsBusy)
				return;

            try
            {
                if(_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    await Shell.Current.DisplayAlert("Internet Issue!",
                    $"Check your internet and try again", "Ok");
                    return;
                }
                IsBusy = true;
                var monkeys = await _monkeyService.GetMonkeys();

                if (Monkeys.Count != 0)
                    Monkeys.Clear();

                foreach (var monkey in monkeys)
                {
                    Monkeys.Add(monkey);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Shell.Current.DisplayAlert("Error!",
                    $"Unable to get monkeys: {ex.Message}", "Ok");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }
	}
}


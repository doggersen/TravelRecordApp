using Plugin.Geolocator;
using SQLite;
using System;
using System.Linq;
using TravelRecordApp.Logic;
using TravelRecordApp.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelRecordApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTravelPage : ContentPage
    {
        public NewTravelPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync();

            var venues = await VenueLogic.GetVenues(position.Latitude, position.Longitude);
            venueListView.ItemsSource = venues;

        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {


                var selectedVenue = venueListView.SelectedItem as Venue;
                var firstCategory = selectedVenue.categories.FirstOrDefault();
                Post post = new Post()

                {
                    Experience = experienceEntry.Text,
                    CategoryId = firstCategory.id,
                    CategoryName = firstCategory.name,
                    Address = selectedVenue.location.address,
                    Distance = selectedVenue.location.distance,
                    Latitude = selectedVenue.location.lat,
                    Longitude = selectedVenue.location.lng,
                    VenueName = selectedVenue.name

                };

                //bruger "using" i stedet for conn.Close(), da using bruger Idisposable til at dispose alt inden i curly-brackets'ne under using.
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Post>();
                    int rows = conn.Insert(post);

                    if (rows > 0)
                    {
                        DisplayAlert("Succes", "Experience succesfully inserted", "Ok");
                    }
                    else
                    {
                        DisplayAlert("Failure", "Experience failed to be inserted", "Ok");
                    }

                }
            }

            catch (NullReferenceException nre)
            {

            }

            catch (Exception ex)
            {

            }

        }
    }


}




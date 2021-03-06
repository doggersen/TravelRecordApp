﻿using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TravelRecordApp.Model;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelRecordApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
            CheckAndRequestLocationPermission();
        }

        public async Task<PermissionStatus> CheckAndRequestLocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            // Additionally could prompt the user to turn on in settings

            return status;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var locator = CrossGeolocator.Current;
            locator.PositionChanged += Locator_PositionChanged;
            await locator.StartListeningAsync(TimeSpan.Zero, 100);

            GetLocation();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Post>();
                var posts = conn.Table<Post>().ToList();

                DisplayInMap(posts);
            }

        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            var locator = CrossGeolocator.Current;
            locator.PositionChanged -= Locator_PositionChanged;
            await locator.StopListeningAsync();

        }

        private void DisplayInMap(List<Post> posts)
        {
            foreach (var post in posts)
            {
                try
                {


                    var position = new Xamarin.Forms.Maps.Position(post.Latitude, post.Longitude);

                    var pin = new Xamarin.Forms.Maps.Pin()
                    {
                        Type = Xamarin.Forms.Maps.PinType.SavedPin,
                        Position = position,
                        Label = post.VenueName,
                        Address = post.Address
                    };

                    locationsMap.Pins.Add(pin);
                }
                catch(NullReferenceException nre)
                {

                }
                catch(Exception ex)
                {

                }
            }

        }

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //    CrossGeolocator.Current.StopListeningAsync();
        //    CrossGeolocator.Current.PositionChanged -= Locator_PositionChanged;
        //}

        private void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            MoveMap(e.Position);
        }

        private async void GetLocation()
        {
            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync();

            MoveMap(position);
        }

        private void MoveMap(Position position)
        {

            var center = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 1, 1);
            locationsMap.MoveToRegion(span);
        }


        //private async void GetPermissions()
        //{
        //    try
        //    {
        //        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
        //        if (status != PermissionStatus.Granted)
        //        {
        //            if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.LocationWhenInUse))
        //            {
        //                await DisplayAlert("Need your location", "We need to access your location", "Ok");
        //            }

        //            var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.LocationWhenInUse);
        //            if (results.ContainsKey(Permission.LocationWhenInUse))
        //                status = results[Permission.LocationWhenInUse];
        //        }

        //        if (status == PermissionStatus.Granted)
        //        {
        //            locationsMap.IsShowingUser = true;
        //        }
        //        else
        //        {
        //            await DisplayAlert("Location denied", "You didn't give us permission to access location, so we can't show you where you are", "Ok");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Error", ex.Message, "Ok");
        //    }
        //}


    }
}
using Kp.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Data.Xml.Dom;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Services.Maps;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Kp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AlertCasePage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        ObservableCollection<ContactData> contactinfo = new ObservableCollection<ContactData>();

        Geolocator geolocator;

        public AlertCasePage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            contactinfo = new ObservableCollection<ContactData> { 
                new ContactData{Name="Kayode Ramadan",Message="No recent message",Pic="Assets/ramadan.jpg"},
                new ContactData{Name="Femi",Message="No recent message",Pic="Assets/jfod.jpg"},
                new ContactData{Name="Dapo",Message="No recent message",Pic="Assets/broAdeola.jpg"}
            };
            contactsList.ItemsSource = contactinfo;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            MyMap.MapServiceToken = "abcdef-abcdefghijklmno";

            //var httpsource = new HttpMapTileDataSource("http://a.tile.openstreetmap.org/{zoomlevel}/{x}/{y}.png");
            //var ts = new MapTileSource(httpsource);
            //MyMap.TileSources.Add(ts);

            geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;
            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));

                MapIcon mapIcon = new MapIcon();
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/my-position.png"));
                mapIcon.Title = "Current Location";
                mapIcon.Location = new Geopoint(new BasicGeoposition()
                {
                    //Latitude = geoposition.Coordinate.Latitude,
                    //Longitude = geoposition.Coordinate.Longitude
                    Latitude = geoposition.Coordinate.Point.Position.Latitude,
                    Longitude = geoposition.Coordinate.Point.Position.Longitude
                });
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                MyMap.MapElements.Add(mapIcon);
                await MyMap.TrySetViewAsync(mapIcon.Location, 18D, 0, 0, MapAnimationKind.Bow);

                /*var pushpin = CreatePushPin();
                MyMap.Children.Add(pushpin);
                var location = new Geopoint(new BasicGeoposition()
                {
                    Latitude = geoposition.Coordinate.Latitude,
                    Longitude = geoposition.Coordinate.Longitude
                });
                MapControl.SetLocation(pushpin, location);
                MapControl.SetNormalizedAnchorPoint(pushpin, new Point(0.0, 1.0));
                await MyMap.TrySetViewAsync(location, 18D, 0, 0, MapAnimationKind.Bow);*/
                
                //progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                //mySlider.Value = MyMap.ZoomLevel;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox("Location service is turned off!");
            }

            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void Redbtn_click(object sender, RoutedEventArgs e)
        {
            string mess = "I've just been kidnapped";
            // Ensure updates are enabled.
            var notifier = ToastNotificationManager.CreateToastNotifier();
            if (notifier.Setting == NotificationSetting.Enabled)
            {
                var templateType = ToastTemplateType.ToastImageAndText02;
                var toastTemp = ToastNotificationManager.GetTemplateContent(templateType);
              
                ((XmlElement)toastTemp.GetElementsByTagName("text")[0]).InnerText = "Kayode Ramadan";
                ((XmlElement)toastTemp.GetElementsByTagName("text")[1]).InnerText = mess;
                
                ((XmlElement)toastTemp.GetElementsByTagName("image")[0]).SetAttribute("src", "ms-appx:///Assets/jfod.jpg");
                var toast = new ToastNotification(toastTemp);
                
                ToastNotificationManager.CreateToastNotifier().Show(toast);


            }
            geolocator = new Geolocator();
            Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));

            ContactData temp = new ContactData { Name = "Kayode Ramadan", location = geoposition.Coordinate.Point, Message = mess, Pic = "Assets/ramadan.jpg" };
            contactinfo.Add(temp);

        }

        private async void Yellowbtn_click(object sender, RoutedEventArgs e)
        {
            string mess = "Robbery Attack";
            // Ensure updates are enabled.
            var notifier = ToastNotificationManager.CreateToastNotifier();
            if (notifier.Setting == NotificationSetting.Enabled)
            {
                var templateType = ToastTemplateType.ToastImageAndText02;
                var toastTemp = ToastNotificationManager.GetTemplateContent(templateType);
                //var node = toastTemp.GetElementsByTagName("text")[0];
                //node.AppendChild(toastTemp.CreateTextNode("Kayode Ramadan"));
                ((XmlElement)toastTemp.GetElementsByTagName("text")[0]).InnerText="Kayode Ramadan";
                ((XmlElement)toastTemp.GetElementsByTagName("text")[1]).InnerText = mess;
                //var node1 = toastTemp.GetElementsByTagName("text")[1];
                //node.AppendChild(toastTemp.CreateTextNode(mess));
                ((XmlElement)toastTemp.GetElementsByTagName("image")[0]).SetAttribute("src", "ms-appx:///Assets/jfod.jpg");
                var toast = new ToastNotification(toastTemp);
                //toast.ExpirationTime = DateTimeOffset.Parse(TimeSpan.FromMinutes(100).ToString());
                ToastNotificationManager.CreateToastNotifier().Show(toast);


            }
            geolocator = new Geolocator();
            Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));

            ContactData temp = new ContactData { Name = "Kayode Ramadan", location = geoposition.Coordinate.Point, Message = mess, Pic = "Assets/ramadan.jpg" };
            contactinfo.Add(temp);

        }

        private async void Greenbtn_click(object sender, RoutedEventArgs e)
        {
            string mess = "it's a false alarm,all is well";
            // Ensure updates are enabled.
            var notifier = ToastNotificationManager.CreateToastNotifier();
            if (notifier.Setting == NotificationSetting.Enabled)
            {
                var templateType = ToastTemplateType.ToastImageAndText02;
                var toastTemp = ToastNotificationManager.GetTemplateContent(templateType);
                //var node = toastTemp.GetElementsByTagName("text")[0];
                //node.AppendChild(toastTemp.CreateTextNode("Kayode Ramadan"));
                ((XmlElement)toastTemp.GetElementsByTagName("text")[0]).InnerText = "Kayode Ramadan";
                ((XmlElement)toastTemp.GetElementsByTagName("text")[1]).InnerText = mess;
                //var node1 = toastTemp.GetElementsByTagName("text")[1];
                //node.AppendChild(toastTemp.CreateTextNode(mess));
                ((XmlElement)toastTemp.GetElementsByTagName("image")[0]).SetAttribute("src", "ms-appx:///Assets/jfod.jpg");
                var toast = new ToastNotification(toastTemp);
                //toast.ExpirationTime = DateTimeOffset.Parse(TimeSpan.FromMinutes(100).ToString());
                ToastNotificationManager.CreateToastNotifier().Show(toast);


            }
            geolocator = new Geolocator();
            Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10));

            ContactData temp = new ContactData { Name = "Kayode Ramadan", location = geoposition.Coordinate.Point, Message = mess, Pic = "Assets/ramadan.jpg" };
            contactinfo.Add(temp);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(addcontactPage));
        }

        private async void MessageBox(string message)
        {
            var dialog = new MessageDialog(message.ToString());
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
        }

        private DependencyObject CreatePushPin()
        {
            // Creating a Grid element.
            //var myGrid = new Grid();
            //myGrid.RowDefinitions.Add(new RowDefinition());
            //myGrid.RowDefinitions.Add(new RowDefinition());
            //myGrid.Background = new SolidColorBrush(Colors.Transparent);

            // Creating a Polygon Marker
            Polygon polygon = new Polygon();
            polygon.Points.Add(new Point(0, 0));
            polygon.Points.Add(new Point(0, 50));
            polygon.Points.Add(new Point(25, 0));
            polygon.Fill = new SolidColorBrush(Colors.Red);

            // Adding the Polygon to the Grid
            //myGrid.Children.Add(polygon);
            return polygon;
        }

        private async void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            //Geopoint pointToReverseGeocode = new Geopoint(args.Location.Position);

            //// Reverse geocode the specified geographic location.
            //MapLocationFinderResult result =
            //    await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

            //var resultText = new StringBuilder();

            //if (result.Status == MapLocationFinderStatus.Success)
            //{
            //    resultText.AppendLine(result.Locations[0].Address.District + ", " + result.Locations[0].Address.Town + ", " + result.Locations[0].Address.Country);
            //}

            //MessageBox(resultText.ToString());
        }

        private void contactsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(ShowLocationPage), e.ClickedItem);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// Namespace
namespace FileManager
{



    // Über
    public sealed partial class About : Page
    {



        // Bauteile und Variablen
        // -----------------------------------------------------------------------
        // Resources // Für verschiedene Sprachen
        private ResourceLoader resource = new ResourceLoader();
        // -----------------------------------------------------------------------



        // Klasse erzeugen
        // -----------------------------------------------------------------------
        public About()
        {
            // UI Initialisieren
            this.InitializeComponent();

            // Größe erstellen
            grHeader.Height = MainPage.actionBarHeight + 3;

            // Überschrift erstellen
            tbHeader.FontSize = MainPage.actionBarHeight / 8 * 3;

            // Bauteile erstellen
            tbHeader.Text = resource.GetString("002_Über");
            tbBuy.Text = resource.GetString("002_Kaufen");
            tbRate.Text = resource.GetString("002_Bewerten");
            tbKontakt.Text = resource.GetString("002_Kontakt");
            tbWebsite.Text = resource.GetString("002_Webseite");
            tbDanke1.Text = resource.GetString("002_Danke_1");
            tbDanke2.Text = resource.GetString("002_Danke_2");
            tbAddressHeader.Text = resource.GetString("002_Postanschrift");
            tbAddress.Text = "xtrose App Studios\r\nIm Wiesengrund 24\r\n73540 Heubach\r\nGermany";
            
        }
        // -----------------------------------------------------------------------



        // Button zurück
        // -----------------------------------------------------------------------
        private void btnBack_Released(object sender, PointerRoutedEventArgs e)
        {
            // Zurück zur Startseite
            Frame.GoBack();
        }
        // -----------------------------------------------------------------------





        // Button Kaufen
        // -----------------------------------------------------------------------
        private async void BtnBuy(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?ProductId=9nblggh4xv1w"));
        }
        // -----------------------------------------------------------------------





        // Button Bewerten
        // -----------------------------------------------------------------------
        private async void BtnRate(object sender, PointerRoutedEventArgs e)
        {
            Frame.Navigate(typeof (Rate));
        }
        // -----------------------------------------------------------------------





        // Button Facebook
        // -----------------------------------------------------------------------
        private async void BtnWeb(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.xtrose.com"));
        }
        // -----------------------------------------------------------------------





        // Button Facebook
        // -----------------------------------------------------------------------
        private async void BtnFacebook(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.facebook.com/xtrose.xtrose/"));
        }
        // -----------------------------------------------------------------------





        // Button Twitter
        // -----------------------------------------------------------------------
        private async void BtnTwitter(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://twitter.com/xtrose"));
        }
        // -----------------------------------------------------------------------





        // Button YouTube
        // -----------------------------------------------------------------------
        private async void BtnYT(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.youtube.com/user/xtrose2overdose"));
        }
        // -----------------------------------------------------------------------





        // Button VK
        // -----------------------------------------------------------------------
        private async void BtnVK(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://vk.com/public54083459"));
        }
        // -----------------------------------------------------------------------





        // Button Kontakt
        // -----------------------------------------------------------------------
        private async void BtnContact(object sender, PointerRoutedEventArgs e)
        {
            EmailMessage msg = new EmailMessage();
            msg.Subject = "File Explorer X | Support";
            msg.To.Add(new EmailRecipient("xtrose@hotmail.com"));
            await EmailManager.ShowComposeNewEmailAsync(msg);
        }
        // -----------------------------------------------------------------------





        // Button PocketPC
        // -----------------------------------------------------------------------
        private async void BtnPocketPC(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://www.pocketpc.ch"));
        }
        // -----------------------------------------------------------------------





        // Button DrWindows
        // -----------------------------------------------------------------------
        private async void BtnDrWindows(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.drwindows.de"));
        }
        // -----------------------------------------------------------------------





        // Button DrWindows
        // -----------------------------------------------------------------------
        private async void BtnWindowsPhoneCentral(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.windowscentral.com"));
        }
        // -----------------------------------------------------------------------





        // Button MyCSharp
        // -----------------------------------------------------------------------
        private async void BtnCSharp(object sender, PointerRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.mycsharp.de"));
        }
        // -----------------------------------------------------------------------
    }
}

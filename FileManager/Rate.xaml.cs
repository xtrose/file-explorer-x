using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xtrose;



// Namespace
namespace FileManager
{



    // Diese App bewerten
    public sealed partial class Rate : Page
    {



        // Bauteile und Variablen
        // -----------------------------------------------------------------------
        // Resources // Für verschiedene Sprachen
        private ResourceLoader resource = new ResourceLoader();
        // -----------------------------------------------------------------------





        // Klasse erzeugen
        // -----------------------------------------------------------------------
        public Rate()
        {
            // UI Initialisieren
            this.InitializeComponent();

            // Größe erstellen
            grHeader.Height = MainPage.actionBarHeight + 3;

            // Überschrift erstellen
            tbHeader.FontSize = MainPage.actionBarHeight / 8 * 3;

            // Bauteile erstellen
            tbHeader.Text = resource.GetString("002_Bewerten");
            tbText.Text = resource.GetString("002_BewertenText");
            tbAppName.Text = resource.GetString("002_PromoApp");
            tbfree.Text = resource.GetString("002_Kostenlos");
            btnRate.Content = resource.GetString("002_JetztBewerten");
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





        // Button bewerten
        // -----------------------------------------------------------------------
        private async void btnRate_Click(object sender, RoutedEventArgs e)
        {
            // Bewertung aufrufen
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=6490xtrose.Datei-ExplorerX_efkxxe1mmwgf2"));

            // Nachricht ausgeben // Kein Name eingegeben
            DialogEx dEx = new DialogEx(resource.GetString("002_VielenDank"));
            dEx.Content = resource.GetString("002_BewertungNachricht");
            DialogExButtonsSet dBSet = new DialogExButtonsSet();
            DialogExButton dBtn = new DialogExButton(resource.GetString("001_Weiter"));
            dBSet.AddButton(dBtn);
            dEx.AddButtonSet(dBSet);
            await dEx.ShowAsync(grMain);

            // Email erstellen
            EmailMessage msg = new EmailMessage();
            msg.Subject = "File Explorer X | Free App";
            msg.Body = ">>>  " + resource.GetString("002_MicrosoftBenutzerName") + "  <<<";
            msg.To.Add(new EmailRecipient("xtrose@hotmail.com"));
            await EmailManager.ShowComposeNewEmailAsync(msg);

            // Zurück
            Frame.GoBack();
        }
        // -----------------------------------------------------------------------
    }
}

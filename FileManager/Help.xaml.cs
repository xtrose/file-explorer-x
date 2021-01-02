using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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



// Namaspace
namespace FileManager
{



    // Hilfe
    public sealed partial class Help : Page
    {



        // Bauteile und Variablen
        // -----------------------------------------------------------------------
        // Resources // Für verschiedene Sprachen
        private ResourceLoader resource = new ResourceLoader();
        // -----------------------------------------------------------------------



        // Klasse erzeugen
        // -----------------------------------------------------------------------
        public Help()
        {
            // UI Initialisieren
            this.InitializeComponent();

            // Größe erstellen
            grHeader.Height = MainPage.actionBarHeight + 3;

            // Überschrift erstellen
            tbHeader.FontSize = MainPage.actionBarHeight / 8 * 3;

            // Bauteile erstellen
            tbHeader.Text = resource.GetString("002_Help");
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
    }



}

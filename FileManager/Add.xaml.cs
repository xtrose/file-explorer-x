using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Security.Authentication.Web;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Xtrose;



// Namespace
namespace FileManager
{



    // Hinzufügen
    public sealed partial class Add : Page
    {



        // Bauteile und Variablen
        // -----------------------------------------------------------------------
        // Resources // Für verschiedene Sprachen
        private ResourceLoader resource = new ResourceLoader();

        // Ordner
        private StorageFolder folderSettings;
        private StorageFolder folderTemp;

        // Dropbox token
        string dropboxToken = "";
        // -----------------------------------------------------------------------





        // Klasse erzeugen
        // -----------------------------------------------------------------------
        public Add()
        {
            // UI Komponenten laden
            InitializeComponent();

            // UI Bauteile anpassen
            tbHeader.Text = resource.GetString("001_Hinzufügen");

            // Größe erstellen
            grHeader.Height = MainPage.actionBarHeight + 3;

            // Überschrift erstellen
            tbHeader.FontSize = MainPage.actionBarHeight / 8 * 3;
        }
        // -----------------------------------------------------------------------





        // Wird bei jedem Aufruf ausgeführt
        // -----------------------------------------------------------------------
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Ordner
            folderSettings = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Settings", CreationCollisionOption.OpenIfExists);
            folderTemp = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Temp", CreationCollisionOption.OpenIfExists);

            // Gibt an welche Elemente geladen werden
            bool oneDrive = true;
            bool filePicker = true;

            // Start Menü Dateien auslesen
            string[] spStart = Regex.Split(MainPage.setStartMenu, ";;;");

            // Dateien durchlaufen
            for (int i = 0; i < spStart.Count(); i++)
            {
                // Eintrag splitten
                string[] spSpStart = Regex.Split(spStart[i], ";");

                // Wenn One Drive
                if (spSpStart[0] == "oneDrive")
                {
                    oneDrive = false;
                }

                // Wenn FilePicker
                if (spSpStart[0] == "filePicker")
                {
                    filePicker = false;
                }
            }

            // Wenn One Drive
            if (oneDrive)
            {
                Grid grid = new Grid();
                grid.PointerReleased += OneDrive_PointerReleased;
                grid.Margin = new Thickness(0, 4, 4, 4);
                StackPanel stackPanal = new StackPanel();
                BitmapImage img = await loadIcon("oneDrive");
                Image imgBtn = new Image();
                imgBtn.Source = img;
                stackPanal.Children.Add(imgBtn);
                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.Text = "One Drive";
                stackPanal.Children.Add(textBlock);
                grid.Children.Add(stackPanal);
                wpItems.Children.Add(grid);

                // Wenn Mobil
                if (MainPage.IsMobile)
                {
                    grid.Width = 50;
                    grid.MaxHeight = 100;
                    imgBtn.Width = 50;
                    textBlock.Width = 50;
                    textBlock.FontSize = 8;
                }

                // Desktop
                else
                {
                    grid.Width = 50 * 1.75;
                    grid.MaxHeight = 100 * 1.75;
                    imgBtn.Width = 50 * 1.75;
                    textBlock.Width = 50 * 1.75;
                    textBlock.FontSize = 8 * 1.75;
                }
            }

            // Dropbox
            Grid grid3 = new Grid();
            grid3.PointerReleased += Dropbox_PointerReleased;
            grid3.Margin = new Thickness(0, 4, 4, 4);
            StackPanel stackPanal3 = new StackPanel();
            BitmapImage img3 = await loadIcon("dropbox");
            Image imgBtn3 = new Image();
            imgBtn3.Source = img3;
            stackPanal3.Children.Add(imgBtn3);
            TextBlock textBlock3 = new TextBlock();
            textBlock3.TextWrapping = TextWrapping.Wrap;
            textBlock3.TextAlignment = TextAlignment.Center;
            textBlock3.Text = "Dropbox";
            stackPanal3.Children.Add(textBlock3);
            grid3.Children.Add(stackPanal3);
            wpItems.Children.Add(grid3);

            // Wenn Mobil
            if (MainPage.IsMobile)
            {
                grid3.Width = 50;
                grid3.MaxHeight = 100;
                imgBtn3.Width = 50;
                textBlock3.Width = 50;
                textBlock3.FontSize = 8;
            }

            // Desktop
            else
            {
                grid3.Width = 50 * 1.75;
                grid3.MaxHeight = 100 * 1.75;
                imgBtn3.Width = 50 * 1.75;
                textBlock3.Width = 50 * 1.75;
                textBlock3.FontSize = 8 * 1.75;
            }

            // Wenn FilePicker
            if (filePicker)
            {
                Grid grid = new Grid();
                grid.PointerReleased += FilePicker_PointerReleased;
                grid.Margin = new Thickness(0, 4, 4, 4);
                StackPanel stackPanal = new StackPanel();
                BitmapImage img = await loadIcon("filePicker");
                Image imgBtn = new Image();
                imgBtn.Source = img;
                stackPanal.Children.Add(imgBtn);
                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.TextAlignment = TextAlignment.Center;
                textBlock.Text = resource.GetString("002_Dateiauswahl");
                stackPanal.Children.Add(textBlock);
                grid.Children.Add(stackPanal);
                wpItems.Children.Add(grid);

                // Wenn Mobil
                if (MainPage.IsMobile)
                {
                    grid.Width = 50;
                    grid.MaxHeight = 100;
                    imgBtn.Width = 50;
                    textBlock.Width = 50;
                    textBlock.FontSize = 8;
                }

                // Desktop
                else
                {
                    grid.Width = 50 * 1.75;
                    grid.MaxHeight = 100 * 1.75;
                    imgBtn.Width = 50 * 1.75;
                    textBlock.Width = 50 * 1.75;
                    textBlock.FontSize = 8 * 1.75;
                }
            }

            /*
            // Ftp
            Grid grid2 = new Grid();
            grid2.PointerReleased += Ftp_PointerReleased;
            grid2.Width = 50;
            grid2.MaxHeight = 100;
            grid2.Margin = new Thickness(0, 4, 4, 4);
            StackPanel stackPanal2 = new StackPanel();
            BitmapImage img2 = await loadIcon("ftp");
            Image imgBtn2 = new Image();
            imgBtn2.Source = img2;
            imgBtn2.Width = 50;
            stackPanal2.Children.Add(imgBtn2);
            TextBlock textBlock2 = new TextBlock();
            textBlock2.Width = 50;
            textBlock2.TextWrapping = TextWrapping.Wrap;
            textBlock2.FontSize = 8;
            textBlock2.TextAlignment = TextAlignment.Center;
            textBlock2.Text = "FTP";
            stackPanal2.Children.Add(textBlock2);
            grid2.Children.Add(stackPanal2);
            wpItems.Children.Add(grid2);
            */


        }
        // -----------------------------------------------------------------------





        // Button One Drive
        // -----------------------------------------------------------------------
        private async void OneDrive_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // One Drive in Start hinzufügen
            MainPage.setStartMenu += "oneDrive;;;";

            // Einstellungen speichern
            StorageFile storageFile = await folderSettings.CreateFileAsync("StartMenu.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(storageFile, MainPage.setStartMenu);

            // Zurück zur Startseite
            Frame.GoBack();
        }
        // -----------------------------------------------------------------------





        // Button Dropbox
        // -----------------------------------------------------------------------
        private async void Dropbox_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Antwort
            string answer = "";
            string name = "";

            // Schleife bis Daten vorhanden
            while (answer != resource.GetString("001_Abbrechen"))
            {
                // Passwort Eingabe erstellen
                DialogEx dEx = new DialogEx(resource.GetString("001_Einstellungen"));
                DialogExTextBox tbUser = new DialogExTextBox(resource.GetString("001_Name"), name);
                dEx.AddTextBox(tbUser);
                DialogExButtonsSet btnSet = new DialogExButtonsSet();
                btnSet.Margin = new Thickness(0, 6, 0, 24);
                DialogExButton btnAbort = new DialogExButton(resource.GetString("001_Abbrechen"));
                btnSet.AddButton(btnAbort);
                DialogExButton btnCheck = new DialogExButton(resource.GetString("001_Hinzufügen"));
                btnSet.AddButton(btnCheck);
                dEx.AddButtonSet(btnSet);
                await dEx.ShowAsync(grMain);

                // Daten auslesen
                answer = dEx.GetAnswer();
                name = dEx.GetTextBoxTextByTitle(resource.GetString("001_Name"));

                // Wenn Antwort Hinzufügen
                if (answer == resource.GetString("001_Hinzufügen"))
                {
                    // Wenn kein Dropbox Name vorhanden
                    if (dEx.GetTextBoxTextByIndex(0) == "")
                    {
                        // Nachricht ausgeben // Kein Name eingegeben
                        dEx = new DialogEx(resource.GetString("001_KeinName"));
                        DialogExButtonsSet dBSet = new DialogExButtonsSet();
                        DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                        dBSet.AddButton(dBtn);
                        dEx.AddButtonSet(dBSet);
                        await dEx.ShowAsync(grMain);
                    }

                    // Wenn ein Name eingegeben
                    else
                    {
                        // Semikolon entfernen
                        name = Regex.Replace(name, ";", "");

                        // Versuchen einen Ordner zu erstellen
                        try
                        {
                            // Ordner erstellen
                            StorageFolder sf = await folderTemp.CreateFolderAsync(name);
                            await sf.DeleteAsync();

                            // Dropbox verbinden
                            dropboxToken = "";
                            dropboxToken = await DropboxConnect();

                            // Wenn Token vorhanden
                            if (dropboxToken != "")
                            {
                                // One Drive in Start hinzufügen
                                MainPage.setStartMenu += "dropbox;" + name + ";" + dropboxToken + ";;;";

                                // Einstellungen speichern
                                StorageFile storageFile = await folderSettings.CreateFileAsync("StartMenu.txt", CreationCollisionOption.OpenIfExists);
                                await FileIO.WriteTextAsync(storageFile, MainPage.setStartMenu);

                                // Zurück zur Startseite
                                Frame.GoBack();

                                // Schleife beenden
                                break;
                            }

                            // Bei Fehlern
                            else
                            {
                                // Nachricht ausgeben // Verbindung nicht möglich
                                dEx = new DialogEx(resource.GetString("001_VerbindungNicht"));
                                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                                dBSet.AddButton(dBtn);
                                dEx.AddButtonSet(dBSet);
                                await dEx.ShowAsync(grMain);
                            }
                        }

                        // Wenn Name nicht verwendet werden kann
                        catch
                        {
                            // Nachricht ausgeben // Kein Name eingegeben
                            dEx = new DialogEx(resource.GetString("001_NameFalsch"));
                            DialogExButtonsSet dBSet = new DialogExButtonsSet();
                            DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                            dBSet.AddButton(dBtn);
                            dEx.AddButtonSet(dBSet);
                            await dEx.ShowAsync(grMain);
                        }
                    }
                }
            }
        }



        // Task // Dropbox verbinden
        private async Task<string> DropboxConnect()
        {
            // Dropbox verbinden und Token erzeugen
            var authUri = DropboxOAuth2Helper.GetAuthorizeUri(OAuthResponseType.Token, "u2etrormxhi8v3i", new Uri("http://localhost:5000/admin/auth"));
            var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, authUri, new Uri("http://localhost:5000/admin/auth"));

            // Resultat auswerten
            ProcessResult(result);

            // Rückgabe
            return dropboxToken;
        }

        // Ergebnis auswerten
        private async void ProcessResult(WebAuthenticationResult result)
        {
            try
            {
                var response = DropboxOAuth2Helper.ParseTokenFragment(new Uri(result.ResponseData));
                dropboxToken = response.AccessToken;
            }
            catch { }
        }
        // -----------------------------------------------------------------------





        // Button FilePicker
        // -----------------------------------------------------------------------
        private async void FilePicker_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Begrüßung erstellen
            DialogEx dialogEx = new DialogEx(resource.GetString("002_Dateiauswahl"), resource.GetString("002_DateiauswahlInfo"));
            DialogExButtonsSet dxBtnSet = new DialogExButtonsSet();
            DialogExButton btn1 = new DialogExButton(resource.GetString("001_Abbrechen"));
            dxBtnSet.AddButton(btn1);
            DialogExButton btn2 = new DialogExButton(resource.GetString("001_Hinzufügen"));
            dxBtnSet.AddButton(btn2);
            dialogEx.AddButtonSet(dxBtnSet);
            await dialogEx.ShowAsync(grMain);

            // Wenn Antwort hinzufügen
            if (dialogEx.GetAnswer() == resource.GetString("001_Hinzufügen"))
            {

                // One Drive in Start hinzufügen
                MainPage.setStartMenu += "filePicker;;;";

                // Einstellungen speichern
                StorageFile storageFile = await folderSettings.CreateFileAsync("StartMenu.txt", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(storageFile, MainPage.setStartMenu);

                // Zurück zur Startseite
                Frame.GoBack();
            }
        }
        // -----------------------------------------------------------------------





        // Button FTP
        // -----------------------------------------------------------------------
        private async void Ftp_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Antwort
            string answer = "";

            // Schleife bis Daten vorhanden
            while (answer != resource.GetString("001_Abbrechen"))
            {
                // Passwort Eingabe erstellen
                DialogEx dialogEx = new DialogEx(resource.GetString("001_Einstellungen"));
                DialogExTextBox tbServer = new DialogExTextBox(resource.GetString("001_Server"), "ftp://");
                dialogEx.AddTextBox(tbServer);
                DialogExTextBox tbUser = new DialogExTextBox(resource.GetString("001_Benutzername"));
                dialogEx.AddTextBox(tbUser);
                DialogExPasswordBox pbPass = new DialogExPasswordBox(resource.GetString("001_Passwort"));
                dialogEx.AddPasswordBox(pbPass);
                DialogExCheckBox chSave = new DialogExCheckBox(resource.GetString("001_PasswortSpeichern"), false);
                dialogEx.AddCheckbox(chSave);
                DialogExButtonsSet btnSet = new DialogExButtonsSet();
                btnSet.Margin = new Thickness(0, 6, 0, 24);
                DialogExButton btnAbort = new DialogExButton(resource.GetString("001_Abbrechen"));
                btnSet.AddButton(btnAbort);
                DialogExButton btnCheck = new DialogExButton(resource.GetString("001_Hinzufügen"));
                btnSet.AddButton(btnCheck);
                dialogEx.AddButtonSet(btnSet);
                await dialogEx.ShowAsync(grMain);

                // Antwort auslesen
                answer = dialogEx.GetAnswer();

                // Wenn Antwort Hinzufügen
                if (answer == resource.GetString("001_Hinzufügen"))
                {
                    // Server Daten zusammenstellen
                    Uri serverUri = null;
                    string serverUriStr = dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Server").Trim());

                    if (Uri.TryCreate(serverUriStr, UriKind.Absolute, out serverUri))
                    {
                        serverUrl = serverUri.ToString();
                        pathStack = new Stack<string>();

                        if (!string.IsNullOrEmpty(dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Benutzername")).Trim()) &&
                           !string.IsNullOrEmpty(dialogEx.GetPasswordBoxTextByTitle(resource.GetString("001_Passwort")).Trim()))
                        {
                            credential = new NetworkCredential();
                            credential.UserName = dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Benutzername")).Trim();
                            credential.Password = dialogEx.GetPasswordBoxTextByTitle(resource.GetString("001_Passwort")).Trim();
                        }
                        else
                        {
                            credential = null;
                        }

                        // List the sub folders and file.
                        ListDirectory();
                    }
                    else
                    {
                        // NotifyUser(serverUriStr + " is not a valid FTP server");
                    }


                    answer = resource.GetString("001_Abbrechen");
                }
            }


        }
        private string serverUrl;
        Stack<string> pathStack;
        NetworkCredential credential;

        public async void ListDirectory()
        {



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










        // Funktion // Icons laden
        // -----------------------------------------------------------------------
        async Task<BitmapImage> loadIcon(string iconName)
        {
            if (iconName == null)
            {
                iconName = "folder";
            }
            BitmapImage bi = new BitmapImage();
            var iconSource = new Uri("ms-appx:///Images/" + iconName + ".png");
            var iconFile = await StorageFile.GetFileFromApplicationUriAsync(iconSource);
            using (IRandomAccessStream fileStream = await iconFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                bi.SetSource(fileStream);
            }
            return bi;
        }
        // -----------------------------------------------------------------------




    }
}

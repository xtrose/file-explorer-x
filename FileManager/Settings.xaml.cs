using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Xtrose;




// Namespace
namespace FileManager
{



    // Einstellungen
    public sealed partial class Settings : Page
    {



        // Bauteile und Variablen
        // -----------------------------------------------------------------------
        // Resources // Für verschiedene Sprachen
        private ResourceLoader resource = new ResourceLoader();

        // Einstellungen
        public static string setSettings = "";              // String mit allen Einstellungen
        private string setDdCopyOrMove = "ask";             // Einstellung // Umgang mit Drag and Drop, kopieren oder verschieben // move = verschieben // copy = kopieren // standard = Windows Standard // ask = Fragen
        private string setDdConfirmCopyOrMove = "always";   // Einstellung // Bestätigung Drag and Drop // several = bei mehreren // always = immer // never = nie
        private string setPrivate = "openFolder";           // Einstellungen // Umgang mit privatem Ordner // openFolder = bei jedem Öffnen des Ordners // once = Einmalig bis die App geschlossen wird
        private bool setPrivateOpen = false;                // Gibt an ob Privater Ordner geöffnet
        private bool setSaveSort = false;                   // Ob Sortierung gespeichert wird
        private string setSortLeftFiles = "date↓";          // Sortierung Dateien links // name↓ = nach Name absteigend // name↑ = nach Name aufsteigend // date↓ = nach Datum absteigend // date↑ = nach Datum aufsteigend // type↓ = Datei Typ absteigend // type↑ = Datei Typ aufsteigend
        private string setSortRightFiles = "date↓";         // Sortierung Dateien rechts // name↓ = nach Name absteigend // name↑ = nach Name aufsteigend // date↓ = nach Datum absteigend // date↑ = nach Datum aufsteigend // type↓ = Datei Typ absteigend // type↑ = Datei Typ aufsteigend
        private string setSortLeftFolders = "name↑";        // Sortierung Ordner links // name↓ = nach Name absteigend // name↑ = nach Name aufsteigend // date↓ = nach Datum absteigend // date↑ = nach Datum aufsteigend // type↓ = Datei Typ absteigend // type↑ = Datei Typ aufsteigend
        private string setSortRightFolders = "name↑";       // Sortierung Ordner rechts // name↓ = nach Name absteigend // name↑ = nach Name aufsteigend // date↓ = nach Datum absteigend // date↑ = nach Datum aufsteigend // type↓ = Datei Typ absteigend // type↑ = Datei Typ aufsteigend
        private string setStartMenu = "";
        private int setOpenByDownloadingLimit = 2000000;    // Byte Limit mit dem Dateien durch Download geöffnet werden
        private int setBufferSize = 1024;                   // Buffer Größe
        private bool setShowFolderTemp = true;              // Gibt an ob Temponärer Ordner angezeigt wird
        private bool setDeleteTempFiles = true;             // Gibt an ob Temponäre Dateien beim Start gelöscht werden

        // Ordner // Einstellungen
        private StorageFolder folderSettings;
        // -----------------------------------------------------------------------




        // Klasse erzeugen
        // -----------------------------------------------------------------------
        public Settings()
        {
            // XAML laden
            this.InitializeComponent();

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
            // Einstellungen laden oder erstellen
            folderSettings = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Settings", CreationCollisionOption.OpenIfExists);

            // Einstellungen laden
            StorageFile storageFile = await folderSettings.CreateFileAsync("Settings.txt", CreationCollisionOption.OpenIfExists);
            var readFile = await FileIO.ReadTextAsync(storageFile);
            setSettings = readFile.ToString();

            // Wenn keine Einstellungen vorhanden
            if (setSettings.Length == 0)
            {
                // Einstellungen speichern
                saveSettings();
            }

            // Einstellungen auslesen
            string[] sp = setSettings.Split(new char[] { ';' });

            // Einstellungen durchlaufen
            for (int i = 0; i < sp.Count(); i++)
            {
                // Einzelen splitten
                string[] spSp = sp[i].Split(new char[] { '=' });

                // Wenn Umgang mit Drag and Drop
                if (spSp[0] == "setDdCopyOrMove")
                {
                    setDdCopyOrMove = spSp[1];
                }

                // Wenn Drag and Drop Abfrage
                else if (spSp[0] == "setDdConfirmCopyOrMove")
                {
                    setDdConfirmCopyOrMove = spSp[1];
                }

                // Wenn Umgang Privater Ordner
                else if (spSp[0] == "setPrivate")
                {
                    setPrivate = spSp[1];
                }

                // Wenn Sortierung speichern
                else if (spSp[0] == "setSaveSort")
                {
                    setSaveSort = Convert.ToBoolean(spSp[1]);
                }

                // Sortierungen
                else if (spSp[0] == "setSortLeftFiles")
                {
                    setSortLeftFiles = spSp[1];
                }
                else if (spSp[0] == "setSortLeftFolders")
                {
                    setSortLeftFolders = spSp[1];
                }
                else if (spSp[0] == "setSortRightFiles")
                {
                    setSortLeftFiles = spSp[1];
                }
                else if (spSp[0] == "setSortRightFolders")
                {
                    setSortLeftFiles = spSp[1];
                }

                // Online Verhalten
                else if (spSp[0] == "setOpenByDownloadingLimit")
                {
                    setOpenByDownloadingLimit = Convert.ToInt32(spSp[1]);
                }
                else if (spSp[0] == "setBufferSize")
                {
                    setBufferSize = Convert.ToInt32(spSp[1]);
                }

                // Temponären Ordner anzeigen
                else if (spSp[0] == "setShowFolderTemp")
                {
                    setShowFolderTemp = Convert.ToBoolean(spSp[1]);
                }

                // Temponäre Datei beim Start Löschen
                else if (spSp[0] == "setDeleteTempFiles")
                {
                    setDeleteTempFiles = Convert.ToBoolean(spSp[1]);
                }
            }

            // UI neu erstellen
            updateUI();
        }
        // -----------------------------------------------------------------------





        // UI neu erstellen
        // -----------------------------------------------------------------------
        private void updateUI()
        {
            // Bauteile erstellen
            tbHeader.Text = resource.GetString("001_Einstellungen");
            tbDDHeader.Text = resource.GetString("001_DragAndDrop");
            tbDD1Header.Text = resource.GetString("001_DDUmgang");
            tbDD2Header.Text = resource.GetString("001_DDBestätigen");
            tbPDHeader.Text = resource.GetString("001_PrivaterOrdner");
            tbPD1Header.Text = resource.GetString("001_PasswortAbfrage");
            tbPD2Header.Text = resource.GetString("001_PasswortZurück");
            btnPD2.Content = resource.GetString("001_PasswortZurück");
            tbSHeader.Text = resource.GetString("001_Sortierung");
            tbS1Header.Text = resource.GetString("001_SortierungSpeichern");
            tbOBHeader.Text = resource.GetString("001_OnlineVerhalten");
            tbOB1Header.Text = resource.GetString("001_DownloadLimit");
            tbTHeader.Text = resource.GetString("001_TemporäreDateien");
            tbT1Header.Text = resource.GetString("001_TemporäreDateienAnzeigen");
            tbT2Header.Text = resource.GetString("001_TemporäreDateienLöschen");
            tbSpHeader.Text = resource.GetString("002_Sprache");
            btnSp1.Content = resource.GetString("002_SpracheÄndern");

            // Drag and Drop kopieren oder verschieben
            if (setDdCopyOrMove == "move")
            {
                btnDD1.Content = resource.GetString("001_Verschieben");
            }
            else if (setDdCopyOrMove == "copy")
            {
                btnDD1.Content = resource.GetString("001_Kopieren");
            }
            else if (setDdCopyOrMove == "standard")
            {
                btnDD1.Content = resource.GetString("001_Standard");
            }
            else if (setDdCopyOrMove == "ask")
            {
                btnDD1.Content = resource.GetString("001_Fragen");
            }

            // Wenn kopieren oder Verschieben auf fragen
            if (setDdCopyOrMove == "ask")
            {
                tbDD2Header.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                btnDD2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            // Wenn kopieren oder Verschieben nicht fragen
            else
            {
                tbDD2Header.Visibility = Windows.UI.Xaml.Visibility.Visible;
                btnDD2.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            // Drag and Drop Aktion bestätigen
            if (setDdConfirmCopyOrMove == "always")
            {
                btnDD2.Content = resource.GetString("001_Immer");
            }
            else if (setDdConfirmCopyOrMove == "several")
            {
                btnDD2.Content = resource.GetString("001_Mehreren");
            }
            else if (setDdConfirmCopyOrMove == "never")
            {
                btnDD2.Content = resource.GetString("001_Nie");
            }

            // Passwort Abfrage
            if (setPrivate == "openFolder")
            {
                btnPD1.Content = resource.GetString("001_ÖffnenVomOrdner");
            }
            else if (setPrivate == "once")
            {
                btnPD1.Content = resource.GetString("001_Einmal");
            }

            // Sortierung
            if (setSaveSort)
            {
                btnS1.Content = resource.GetString("001_Ja");
            }
            else
            {
                btnS1.Content = resource.GetString("001_Nein");
            }

            // Download Limit
            if (setOpenByDownloadingLimit == 0)
            {
                btnOB1.Content = resource.GetString("001_Unendlich");
            }
            else
            {
                btnOB1.Content = (setOpenByDownloadingLimit / 1000000) + " MB";
            }

            // Temporäre Dateien
            if (setShowFolderTemp)
            {
                btnT1.Content = resource.GetString("001_Ja");
                btnT2.Visibility = Windows.UI.Xaml.Visibility.Visible;
                tbT2Header.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                btnT1.Content = resource.GetString("001_Nein");
                btnT2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                tbT2Header.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            if (setDeleteTempFiles)
            {
                btnT2.Content = resource.GetString("001_Ja");
            }
            else
            {
                btnT2.Content = resource.GetString("001_Nein");
            }

        }
        // -----------------------------------------------------------------------






        // Einstellungen speichern
        // -----------------------------------------------------------------------
        private async void saveSettings()
        {
            // Settings String erstellen
            setSettings = "setVersion=1;setDdCopyOrMove=" + setDdCopyOrMove + ";setDdConfirmCopyOrMove=" + setDdConfirmCopyOrMove + ";setPrivate=" + setPrivate + ";setSaveSort=" + setSaveSort.ToString() + ";setSortLeftFiles=" + setSortLeftFiles + ";setSortLeftFolders=" + setSortLeftFolders + ";setSortRightFiles=" + setSortRightFiles + ";setSortRightFolders=" + setSortRightFolders + ";setOpenByDownloadingLimit=" + setOpenByDownloadingLimit + ";setBufferSize=" + setBufferSize + ";setShowFolderTemp=" + setShowFolderTemp.ToString() + ";setDeleteTempFiles=" + setDeleteTempFiles.ToString();

            // Datei speichern
            StorageFile sf = await folderSettings.CreateFileAsync("Settings.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(sf, setSettings);
        }
        // -----------------------------------------------------------------------





        // Button Drag and Drop Aktion
        // -----------------------------------------------------------------------
        private async void btnDD1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Nachricht ausgeben
            DialogEx dQuery = new DialogEx();
            dQuery.Title = resource.GetString("001_DDUmgang");
            DialogExButtonsSet btnSet = new DialogExButtonsSet();
            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Kopieren"));
            btnSet.AddButton(btn_1);
            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Verschieben"));
            btnSet.AddButton(btn_2);
            DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Standard"));
            btnSet.AddButton(btn_3);
            DialogExButton btn_4 = new DialogExButton(resource.GetString("001_Fragen"));
            btnSet.AddButton(btn_4);
            dQuery.AddButtonSet(btnSet);
            await dQuery.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dQuery.GetAnswer();

            // Wenn Kopieren
            if (answer == resource.GetString("001_Kopieren"))
            {
                setDdCopyOrMove = "copy";
            }

            // Wenn Verschieben
            else if (answer == resource.GetString("001_Verschieben"))
            {
                setDdCopyOrMove = "move";
            }

            // Wenn Standard
            else if (answer == resource.GetString("001_Standard"))
            {
                setDdCopyOrMove = "standard";
            }

            // Wenn Fragen
            else if (answer == resource.GetString("001_Fragen"))
            {
                setDdCopyOrMove = "ask";
            }

            // Einstellungen speichern
            saveSettings();

            // UI Updaten
            updateUI();
        }
        // -----------------------------------------------------------------------





        // Button Drag and Drop bestätigen
        // -----------------------------------------------------------------------
        private async void btnDD2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Nachricht ausgeben
            DialogEx dQuery = new DialogEx();
            dQuery.Title = resource.GetString("001_DDUmgang");
            DialogExButtonsSet btnSet = new DialogExButtonsSet();
            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Immer"));
            btnSet.AddButton(btn_1);
            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Mehreren"));
            btnSet.AddButton(btn_2);
            DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Nie"));
            btnSet.AddButton(btn_3);
            dQuery.AddButtonSet(btnSet);
            await dQuery.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dQuery.GetAnswer();

            // Wenn Immer
            if (answer == resource.GetString("001_Immer"))
            {
                setDdConfirmCopyOrMove = "always";
            }

            // Wenn Mehrere
            else if (answer == resource.GetString("001_Mehreren"))
            {
                setDdConfirmCopyOrMove = "several";
            }

            // Wenn  Nie
            else if (answer == resource.GetString("001_Nie"))
            {
                setDdConfirmCopyOrMove = "never";
            }

            // Einstellungen speichern
            saveSettings();

            // UI Updaten
            updateUI();
        }
        // -----------------------------------------------------------------------



        // Button Privater Bereich öffnen
        // -----------------------------------------------------------------------
        private async void btnPD1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Nachricht ausgeben
            DialogEx dQuery = new DialogEx();
            dQuery.Title = resource.GetString("001_PasswortAbfrage");
            DialogExButtonsSet btnSet = new DialogExButtonsSet();
            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_ÖffnenVomOrdner"));
            btnSet.AddButton(btn_1);
            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Einmal"));
            btnSet.AddButton(btn_2);
            dQuery.AddButtonSet(btnSet);
            await dQuery.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dQuery.GetAnswer();

            // Wenn Kopieren
            if (answer == resource.GetString("001_ÖffnenVomOrdner"))
            {
                setPrivate = "openFolder";
            }

            // Wenn Verschieben
            else if (answer == resource.GetString("001_Einmal"))
            {
                setPrivate = "once";
            }

            // Einstellungen speichern
            saveSettings();

            // UI Updaten
            updateUI();
        }
        // -----------------------------------------------------------------------





        // Button Password zurücksetzen
        // -----------------------------------------------------------------------
        private async void btnPD2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Versuchen Passwort zu laden
            try
            {
                StorageFile passFile = await folderSettings.GetFileAsync("Private.txt");
                var passTemp = await FileIO.ReadTextAsync(passFile);
                string pass1 = passTemp.ToString();

                // Passwort abfragen
                DialogEx dEx = new DialogEx(resource.GetString("001_PasswortZurück"));
                DialogExPasswordBox pb_1 = new DialogExPasswordBox(resource.GetString("001_Passwort"));
                dEx.AddPasswordBox(pb_1);
                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                DialogExButton dBtn_1 = new DialogExButton(resource.GetString("001_PasswortZurück"));
                dBSet.AddButton(dBtn_1);
                DialogExButton dBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                dBSet.AddButton(dBtn_2);
                dEx.AddButtonSet(dBSet);
                await dEx.ShowAsync(grMain);

                // Antwort auslesen
                string answer = dEx.GetAnswer();

                // Wenn Antwort Passwort Zurücksetzen
                if (answer == resource.GetString("001_PasswortZurück"))
                {
                    // Passwort auslesen
                    string pass0 = dEx.GetPasswordBoxTextByIndex(0);

                    // Wenn Passwort vorhanden
                    if (pass0.Length > 0)
                    {
                        // Wenn Passwört richtig
                        if (pass0 == pass1 | pass0 == "I am stupid and lost my password")
                        {
                            // Passwort löschen
                            await passFile.DeleteAsync();
                        }

                        // Wenn Passwort falsch
                        else
                        {
                            //Fehlermeldung ausgeben
                            dEx = new DialogEx(resource.GetString("001_Fehler"), resource.GetString("001_ZugriffVerweigert"));
                            dBSet = new DialogExButtonsSet();
                            dBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                            dBSet.AddButton(dBtn_1);
                            dEx.AddButtonSet(dBSet);
                            await dEx.ShowAsync(grMain);
                        }
                    }

                    // Wenn kein Passwort vorhanden
                    else
                    {
                        //Fehlermeldung ausgeben
                        dEx = new DialogEx(resource.GetString("001_Fehler"), resource.GetString("001_KeinPasswort"));
                        dBSet = new DialogExButtonsSet();
                        dBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                        dBSet.AddButton(dBtn_1);
                        dEx.AddButtonSet(dBSet);
                        await dEx.ShowAsync(grMain);
                    }
                }
            }
            catch { }
        }
        // -----------------------------------------------------------------------





        // Sortierung
        // -----------------------------------------------------------------------
        private async void btnS1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Nachricht ausgeben
            DialogEx dQuery = new DialogEx();
            dQuery.Title = resource.GetString("001_SortierungSpeichern");
            DialogExButtonsSet btnSet = new DialogExButtonsSet();
            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Ja"));
            btnSet.AddButton(btn_1);
            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Nein"));
            btnSet.AddButton(btn_2);
            dQuery.AddButtonSet(btnSet);
            await dQuery.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dQuery.GetAnswer();

            // Wenn Kopieren
            if (answer == resource.GetString("001_Ja"))
            {
                setSaveSort = true;
            }

            // Wenn Verschieben
            else if (answer == resource.GetString("001_Nein"))
            {
                setSaveSort = false;
            }

            // Einstellungen speichern
            saveSettings();

            // UI Updaten
            updateUI();
        }
        // -----------------------------------------------------------------------




        // Button Download Limit
        // -----------------------------------------------------------------------
        private async void btnOB1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Nachricht ausgeben
            DialogEx dQuery = new DialogEx();
            dQuery.Title = resource.GetString("001_DownloadLimit");
            DialogExButtonsSet btnSet = new DialogExButtonsSet();
            DialogExButton btn_1 = new DialogExButton("1 MB");
            btnSet.AddButton(btn_1);
            DialogExButton btn_2 = new DialogExButton("2 MB");
            btnSet.AddButton(btn_2);
            DialogExButton btn_3 = new DialogExButton("3 MB");
            btnSet.AddButton(btn_3);
            DialogExButton btn_4 = new DialogExButton("5 MB");
            btnSet.AddButton(btn_4);
            DialogExButton btn_5 = new DialogExButton("10 MB");
            btnSet.AddButton(btn_5);
            DialogExButton btn_6 = new DialogExButton("15 MB");
            btnSet.AddButton(btn_6);
            DialogExButton btn_7 = new DialogExButton("20 MB");
            btnSet.AddButton(btn_7);
            DialogExButton btn_8 = new DialogExButton(resource.GetString("001_Unendlich"));
            btnSet.AddButton(btn_8);
            dQuery.AddButtonSet(btnSet);
            await dQuery.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dQuery.GetAnswer();

            // Wenn Unendlich
            if (answer == resource.GetString("001_Unendlich"))
            {
                setOpenByDownloadingLimit = 0;
            }

            // Wenn Limit
            else
            {
                string[] sp = answer.Split(new char[] { ' ' });
                setOpenByDownloadingLimit = Convert.ToInt32(sp[0] + "000000");
            }

            // Einstellungen speichern
            saveSettings();

            // UI Updaten
            updateUI();
        }
        // -----------------------------------------------------------------------





        // Ordner Temporäre Dateien anzeigen
        // -----------------------------------------------------------------------
        private async void btnT1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Nachricht ausgeben
            DialogEx dQuery = new DialogEx();
            dQuery.Title = resource.GetString("001_TemporäreDateienAnzeigen");
            DialogExButtonsSet btnSet = new DialogExButtonsSet();
            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Ja"));
            btnSet.AddButton(btn_1);
            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Nein"));
            btnSet.AddButton(btn_2);
            dQuery.AddButtonSet(btnSet);
            await dQuery.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dQuery.GetAnswer();

            // Wenn Kopieren
            if (answer == resource.GetString("001_Ja"))
            {
                setShowFolderTemp = true;
            }

            // Wenn Verschieben
            else if (answer == resource.GetString("001_Nein"))
            {
                setShowFolderTemp = false;
                setDeleteTempFiles = true;
            }

            // Einstellungen speichern
            saveSettings();

            // UI Updaten
            updateUI();
        }
        // -----------------------------------------------------------------------





        // Ordner Temporäre Dateien löschen
        // -----------------------------------------------------------------------
        private async void btnT2_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Nachricht ausgeben
            DialogEx dQuery = new DialogEx();
            dQuery.Title = resource.GetString("001_TemporäreDateienLöschen");
            DialogExButtonsSet btnSet = new DialogExButtonsSet();
            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Ja"));
            btnSet.AddButton(btn_1);
            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Nein"));
            btnSet.AddButton(btn_2);
            dQuery.AddButtonSet(btnSet);
            await dQuery.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dQuery.GetAnswer();

            // Wenn Kopieren
            if (answer == resource.GetString("001_Ja"))
            {
                setDeleteTempFiles = true;
            }

            // Wenn Verschieben
            else if (answer == resource.GetString("001_Nein"))
            {
                setDeleteTempFiles = false;
            }

            // Einstellungen speichern
            saveSettings();

            // UI Updaten
            updateUI();
        }
        // -----------------------------------------------------------------------





        // Button Sprache ändern
        // -----------------------------------------------------------------------
        private async void btnSp1_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Language));
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

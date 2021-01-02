using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Xtrose;
using Windows.UI.Text;
using Windows.System;
using Microsoft.OneDrive.Sdk;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Dropbox.Api;
using Dropbox.Api.Files;
using Windows.Storage.Pickers;
using Windows.System.UserProfile;
using System.Globalization;
using Windows.ApplicationModel.Store;



// Namespace
namespace FileManager
{



    // Hauptseite // Aufteilung der Seite
    public sealed partial class MainPage : Page
    {
        


#region Start
        // Bauteile und Variablen
        // -----------------------------------------------------------------------
        #region Bauteile und Variablen

        // Resources // Für verschiedene Sprachen
        private ResourceLoader resource = new ResourceLoader();

        // CultureInfo laden
        string Language = "en-us";
        public static string language = "en-us";

        // Fenster Größen // Größe des gesamten Fensters
        private double frameHeight = 0;
        private double frameWidth = 0;
        // Fenster Größen // Prozent der Trennlinie
        private double percent = 50;
        // Fenster Größen // Größe der Einzelteile
        private double frameLeftHeight = 0;
        private double frameLeftWidth = 0;
        private double frameRightHeight = 0;
        private double frameRightWidth = 0;
        public static double actionBarHeight = 0;

        // Gibt an ob die App das erste mal gestartet wird
        public static bool firstStart = true;

        // Einstellungen
        private bool setFullVersion = false;                 // Einstellung Vollversion
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
        public static string setStartMenu = "";
        private int setOpenByDownloadingLimit = 2000000;    // Byte Limit mit dem Dateien durch Download geöffnet werden
        private int setBufferSize = 1024;                   // Buffer Größe
        private bool setShowFolderTemp = false;             // Gibt an ob Temponärer Ordner angezeigt wird
        private bool setDeleteTempFiles = true;             // Gibt an ob Temponäre Dateien beim Start gelöscht werden
        public static double setIconSizeLeft = 1.0;         // Gibt die größe der Icons an
        public static double setIconSizeRight = 1.0;        // Gibt die größe der Icons an

        // Listen // Liste des Start Ordners
        private ObservableCollection<ClassStartFolders> listFolderStart = new ObservableCollection<ClassStartFolders>();
        // Listen // Liste des System Ordners erstellen
        private ObservableCollection<ClassPhoneFolders> listFolderPhone = new ObservableCollection<ClassPhoneFolders>();
        // Listen // Listen der Dateien
        private ObservableCollection<ClassFiles> listFilesLeft = new ObservableCollection<ClassFiles>();
        private ObservableCollection<ClassFiles> listFilesRight = new ObservableCollection<ClassFiles>();
        // Listen // Listen Ordnerbaum
        private ObservableCollection<ClassFolderTree> listFolderTreeLeft = new ObservableCollection<ClassFolderTree>();
        private ObservableCollection<ClassFolderTree> listFolderTreeRight = new ObservableCollection<ClassFolderTree>();

        // Tokens um Thred abzubrechen
        private CancellationTokenSource cancelTokenLeft;
        private CancellationTokenSource cancelTokenRight;
        private CancellationTokenSource cancelTokenAction;

        // Ordner // Einstellungen
        private StorageFolder folderSettings;
        // Ordner // Temp
        private StorageFolder folderTemp;
        // Ordner // Private
        private StorageFolder folderPrivate;
        // Ordner // SD Karte
        private StorageFolder sdCard;

        // Timer // Größe ändern
        private DispatcherTimer scTimer = new DispatcherTimer();

        // Farben // xtrose Rot
        private SolidColorBrush scbXtrose = new SolidColorBrush(Color.FromArgb(255, 207, 40, 40));
        // Farben // Hintergrundfarbe für Scroll Bereich
        private SolidColorBrush scbScroll = new SolidColorBrush(Color.FromArgb(10, 0, 0, 0));
        // Farben // Hintergrundfarbe für Auswahl
        private SolidColorBrush scbSelection = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
        // Farben // Hintergrundfarbe für Drop
        private SolidColorBrush scbGrDrop = new SolidColorBrush(Color.FromArgb(40, 0, 0, 0));
        private SolidColorBrush scbSvDrop = new SolidColorBrush(Color.FromArgb(40, 0, 0, 0));

        // One Drive Client
        private IOneDriveClient oneDriveClient;
        private string oneDriveToken = "";

        // Gibt an ob Maus aktiv ist
        public static bool continuum = false;

        // Prüft ob die App Mobil oder am Desktop ausgeführt wird
        public static bool IsMobile
        {
            get
            {
                var qualifiers = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().QualifierValues;
                return (qualifiers.ContainsKey("DeviceFamily") && qualifiers["DeviceFamily"] == "Mobile");
            }
        }
        #endregion
        // -----------------------------------------------------------------------





        // Klasse erzeugen
        // -----------------------------------------------------------------------
        #region Klasse erzeugen

        // Klasse erzeugen
        public MainPage()
        {
            // Sprache einstellen
            Language = GlobalizationPreferences.Languages.First();
            language = Language;
            // Sprache auswählen
            selectLanguage();

            // UI Komponenten laden
            InitializeComponent();

            // StatusBar und Navigationsleiste verbergen
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();

            // Farben ändern
            grScrollLeft.Background = scbScroll;
            grScrollRight.Background = scbScroll;

            // Timer einstellen // Größe änden
            scTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
            scTimer.Tick += ScTimer_Tick;
            scTimer.Stop();

            // Listboxen Binden
            lbLeft.ItemsSource = listFilesLeft;
            lbRight.ItemsSource = listFilesRight;

            // Nur Wenn Mobile
            if (IsMobile)
            {
                // Prüft ob eine Maus angeschlossen wurde
                switch (UIViewSettings.GetForCurrentView().UserInteractionMode)
                {
                    // Wenn Maus aktiv ist
                    case UserInteractionMode.Mouse:
                        continuum = true;
                        grScrollLeft.Visibility = Visibility.Collapsed;
                        grScrollRight.Visibility = Visibility.Collapsed;
                        break;

                    // Wenn Touch aktiv ist
                    case UserInteractionMode.Touch:
                    default:
                        continuum = false;
                        grScrollLeft.Visibility = Visibility.Visible;
                        grScrollRight.Visibility = Visibility.Visible;
                        break;
                }
            }

            // Wenn Größe von grMain geändert wird
            grMain.SizeChanged += GrMain_SizeChanged;
        }



        // Sprache auswählen
        public async void selectLanguage()
        {
            // Default Sprache ermitteln
            var DefaultCulture = CultureInfo.DefaultThreadCurrentCulture;

            // Prüfen ob Sprachdatei besteht
            folderSettings = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Settings", CreationCollisionOption.OpenIfExists);
            var F_Language = await folderSettings.CreateFileAsync("Language.txt", CreationCollisionOption.OpenIfExists);
            var Temp_Language = await FileIO.ReadTextAsync(F_Language);

            // Prüfen ob Sprachdatei besteht
            Language = Convert.ToString(Temp_Language);

            // Wenn Sprachdatei noch nicht bestht
            if (Language.Length > 0)
            {
                // Sprache festlegen
                try
                {
                    language = Language;
                    var culture = new CultureInfo(Language);
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = culture.Name;
                    CultureInfo.DefaultThreadCurrentCulture = culture;
                    CultureInfo.DefaultThreadCurrentUICulture = culture;
                }
                catch
                { }
            }
        }



        // StatusBar verbergen
        private async void hideStatusBar()
        {
            // Status Bar verbergen
            StatusBar statusBar = StatusBar.GetForCurrentView();
            await statusBar.HideAsync();
        }

        #endregion
        // -----------------------------------------------------------------------





        // Wird bei jedem Aufruf ausgeführt
        // -----------------------------------------------------------------------
        #region Wird bei jedem Aufruf ausgeführt

        // Wird bei jedem Aufruf ausgeführt
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Bewegung erfassen
            PointerMoved += pointerMoved;

            // Einstellungen laden oder erstellen
            await createAndLoadSettings();

            // Start Ordner erstellen
            await createStartFolder();

            // System Ordner erstellen
            createSystemFolder();

            // Temporäre Dateien löschen
            if (firstStart & setDeleteTempFiles)
            {
                await deleteTempFiles();
            }

            // Animation starten
            if (firstStart)
            {
                // Nur Mobil
                if (IsMobile)
                {
                    try
                    {
                        await folderSettings.GetFileAsync("animationStart.txt");
                    }
                    catch
                    {
                        animationStart();
                    }
                }
            }

            // Angeben das erster Start vorüber
            firstStart = false;
        }



        // List Position des Fingers beim bewegen auf dem Display aus 
        double pointerX = 0;
        double pointerY = 0;
        void pointerMoved(object sender, PointerRoutedEventArgs e)
        {
            PointerPoint pp = e.GetCurrentPoint(null);
            pointerX = pp.Position.X;
            pointerY = pp.Position.Y;
        }

        // Einstellungen laden oder erstellen
        private async Task createAndLoadSettings()
        {
            // Einstellungen laden oder erstellen
            folderSettings = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Settings", CreationCollisionOption.OpenIfExists);

            // Private Ordner laden oder erstellen
            folderPrivate = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Private", CreationCollisionOption.OpenIfExists);

            // Temponären Ordner laden oder erstellen
            folderTemp = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Temp", CreationCollisionOption.OpenIfExists);

            // Einstellungen laden
            StorageFile storageFile = await folderSettings.CreateFileAsync("Settings.txt", CreationCollisionOption.OpenIfExists);
            var readFile = await FileIO.ReadTextAsync(storageFile);
            setSettings = readFile.ToString();

            // Vollversion laden
#if DEBUG
            setFullVersion = true;
#else
            storageFile = await folderSettings.CreateFileAsync("FullVersion.txt", CreationCollisionOption.OpenIfExists);
            readFile = await FileIO.ReadTextAsync(storageFile);
            if(readFile.ToString() == "sxSdeFvbg61Wsa0&0gGg")
            {
                setFullVersion = true;
            }
            else
            {
                LicenseInformation licenseInformation = CurrentApp.LicenseInformation;
                if (licenseInformation != null)
                {
                    if (licenseInformation.IsActive)
                    {
                        if (!licenseInformation.IsTrial)
                        {
                            await FileIO.WriteTextAsync(storageFile, "sxSdeFvbg61Wsa0&0gGg");
                            setFullVersion = true;
                        }
                    }
                }
            }
#endif
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
                if(spSp[0] == "setDdCopyOrMove")
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
                    if (setSaveSort)
                    {
                        setSortLeftFiles = spSp[1];
                    }
                }
                else if (spSp[0] == "setSortLeftFolders")
                {
                    if (setSaveSort)
                    {
                        setSortLeftFolders = spSp[1];
                    }
                }
                else if (spSp[0] == "setSortRightFiles")
                {
                    if (setSaveSort)
                    {
                        setSortLeftFiles = spSp[1];
                    }
                }
                else if (spSp[0] == "setSortRightFolders")
                {
                    if (setSaveSort)
                    {
                        setSortLeftFiles = spSp[1];
                    }
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

                // Temponäre Dateien anzeigen
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


            // Start Menü Datei laden
            storageFile = await folderSettings.CreateFileAsync("StartMenu.txt", CreationCollisionOption.OpenIfExists);
            readFile = await FileIO.ReadTextAsync(storageFile);
            setStartMenu = readFile.ToString();
        }



        // Einstellungen speichern
        private async void saveSettings()
        {
            // Settings String erstellen
            setSettings = "setVersion=1;setDdCopyOrMove=" + setDdCopyOrMove + ";setDdConfirmCopyOrMove=" + setDdConfirmCopyOrMove + ";setPrivate=" + setPrivate + ";setSaveSort=" + setSaveSort.ToString() + ";setSortLeftFiles=" + setSortLeftFiles + ";setSortLeftFolders=" + setSortLeftFolders + ";setSortRightFiles=" + setSortRightFiles + ";setSortRightFolders=" + setSortRightFolders + ";setOpenByDownloadingLimit=" + setOpenByDownloadingLimit + ";setBufferSize=" + setBufferSize + ";setShowFolderTemp=" + setShowFolderTemp.ToString() + ";setDeleteTempFiles=" + setDeleteTempFiles.ToString();

            // Datei speichern
            StorageFile sf = await folderSettings.CreateFileAsync("Settings.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(sf, setSettings);
        }



        // Start Ordner erstellen
        private async Task createStartFolder()
        {
            // Start Liste leeren
            listFolderStart.Clear();

            // Dieses Gerät hinzufügen
            listFolderStart.Add(new ClassStartFolders(-1, "phone", "phone", resource.GetString("001_DiesesGerät"), "folderPhone", null, null, false, false, false, false, false));

            // Prüfen ob SD Karte vorhanden
            try
            {
                sdCard = (await KnownFolders.RemovableDevices.GetFoldersAsync()).FirstOrDefault();
            }
            catch
            {
            }

            // Wenn SD Karte vorhanden
            if (sdCard != null)
            {
                // SD Karten auslesen
                StorageFolder folderSD = KnownFolders.RemovableDevices;
                IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
                folderList = await folderSD.GetFoldersAsync();

                // SD Karten hinzufügen
                for (int i = 0; i < folderList.Count(); i++)
                {
                    listFolderStart.Add(new ClassStartFolders(i, "sd", "sd", folderList[i].DisplayName, "folderSD", folderList[i], null, false, false, true, false, false));
                }
            }

            // Privat hinzufügen
            listFolderStart.Add(new ClassStartFolders(-1, "private", "private", resource.GetString("001_Privat"), "folderPrivate", null, null, false, true, false, false, false));

            // Temp Hinzufügen
            if (setShowFolderTemp)
            {
                listFolderStart.Add(new ClassStartFolders(-1, "temp", "temp", resource.GetString("001_TemporäreDateien"), "folderTemp", null, null, false, true, false, false, false));
            }

            // Eigene Einträge hinzufügen
            string[] spStart = Regex.Split(setStartMenu, ";;;");

            // Eigene Einträge durchlaufen
            for (int i = 0; i < spStart.Count(); i++)
            {
                // Eintrag splitten
                string[] spSpStart = Regex.Split(spStart[i], ";");

                // Wenn One Drive
                if (spSpStart[0] == "oneDrive")
                {
                    // One Drive hinzufügen
                    listFolderStart.Add(new ClassStartFolders(-1, "oneDrive", "oneDrive", "One Drive", "oneDrive", null, null, true, false, false, true, true));
                }

                // Wenn Dropbox
                if (spSpStart[0] == "dropbox")
                {
                    // One Drive hinzufügen
                    listFolderStart.Add(new ClassStartFolders(-1, "dropbox", "dropbox", spSpStart[1], "dropbox", null, spSpStart[2], true, false, false, true, false));
                }

                // File Picker
                if (spSpStart[0] == "filePicker")
                {
                    // One Drive hinzufügen
                    listFolderStart.Add(new ClassStartFolders(-1, "filePicker", "filePicker", resource.GetString("002_Dateiauswahl"), "filePicker", null, null, true, false, false, true, false));
                }
            }

            // Links // Wenn Start Ordner offen oder nichts geladen
            if (listFolderTreeLeft.Count() < 2)
            {
                // Neues Task Token erstellen
                cancelTokenLeft = new CancellationTokenSource();
                // Start Ordner erstellen
                await outputStart("left", cancelTokenLeft.Token);
            }

            // Rechts // Wenn Start Ordner offen oder nichts geladen
            if (listFolderTreeRight.Count() < 2)
            {
                // Neues Task Token erstellen
                cancelTokenRight = new CancellationTokenSource();
                // Start Ordner erstellen
                await outputStart("right", cancelTokenRight.Token);
            }
        }



        // Phone Ordner erstellen
        private void createSystemFolder()
        {
            listFolderPhone.Add(new ClassPhoneFolders("folderPictures", resource.GetString("001_Bilder"), KnownFolders.PicturesLibrary, "folderPicture", true, true));
            listFolderPhone.Add(new ClassPhoneFolders("folderMusic", resource.GetString("001_Musik"), KnownFolders.MusicLibrary, "folderMusic", true, true));
            listFolderPhone.Add(new ClassPhoneFolders("folderVideo", resource.GetString("001_Videos"), KnownFolders.VideosLibrary, "folderVideo", true, true));
        }
#endregion
        // -----------------------------------------------------------------------
#endregion










#region Ausgaben erstellen
        // Start Ordner
        // -----------------------------------------------------------------------
#region Start Ordner

        // Task // Start Ordner erstellen
        async Task outputStart(string side, CancellationToken token)
        {
            // Token um Task abzubrechen
            token.ThrowIfCancellationRequested();

            // Links // vorhandene Daten löschen
            if (side == "left")
            {
                // listFiles leeren
                listFilesLeft.Clear();

                // MultiSelect entfernen
                lbLeft.SelectionMode = SelectionMode.Single;

                // Ordnerbaum neu erstellen
                listFolderTreeLeft.Clear();
                listFolderTreeLeft.Add(new ClassFolderTree(-1, null, null, null, null, "folderStart", false, false, false, true, false, false, false, false));

                // Privater Ordner Passwort zurücksetzen
                try
                {
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType != "private" & setPrivate == "openFolder")
                    {
                        setPrivateOpen = false;
                    }
                }
                catch { }
                
                // Menü erstellen
                createAndShowMenu("left");

                // Ladeanzeigen ändern
                loadingHide("left");
            }

            // Rechts // vorhandene Daten löschen
            if (side == "right")
            {
                // listFiles leeren
                listFilesRight.Clear();

                // MultiSelect entfernen
                lbRight.SelectionMode = SelectionMode.Single;

                // Ordnerbaum neu erstellen
                listFolderTreeRight.Clear();
                listFolderTreeRight.Add(new ClassFolderTree(-1, null, null, null, null, "folderStart", false, false, false, true, false , false, false, false));

                // Privater Ordner Passwort zurücksetzen
                try
                {
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType != "private" & setPrivate == "openFolder")
                    {
                        setPrivateOpen = false;
                    }
                }
                catch { }

                // Menü erstellen
                createAndShowMenu("right");

                // Ladeanzeigen ändern
                loadingHide("right");
            }

            // Start Ordner Daten durchlaufen
            for (int i = 0; i < listFolderStart.Count(); i++)
            {
                // Wenn Task nicht abgebrochen wurde
                if (!token.IsCancellationRequested)
                {
                    // Links // Ausgabe erstellen
                    if (side == "left")
                    {
                        // In listFiles schreiben
                        listFilesLeft.Add(new ClassFiles(i, "left", listFolderStart[i].driveType, listFolderStart[i].folderType, listFolderStart[i].name, new DateTimeOffset(), listFolderStart[i].storageFolder, null, null, null, await loadIcon(listFolderStart[i].iconName), listFolderStart[i].canDrag, listFolderStart[i].canDrop, listFolderStart[i].canFlyout, listFolderStart[i].canDelete, listFolderStart[i].canCopy, listFolderStart[i].canCut, listFolderStart[i].canPaste, listFolderStart[i].canShare, listFolderStart[i].canRemove, listFolderStart[i].canSignOut));
                    }

                    // Rechts // Ausgabe erstellen
                    if (side == "right")
                    {
                        // In ListFiles schreiben
                        listFilesRight.Add(new ClassFiles(i, "right", listFolderStart[i].driveType, listFolderStart[i].folderType, listFolderStart[i].name, new DateTimeOffset(), listFolderStart[i].storageFolder, null, null, null, await loadIcon(listFolderStart[i].iconName), listFolderStart[i].canDrag, listFolderStart[i].canDrop, listFolderStart[i].canFlyout, listFolderStart[i].canDelete, listFolderStart[i].canCopy, listFolderStart[i].canCut, listFolderStart[i].canPaste, listFolderStart[i].canShare, listFolderStart[i].canRemove, listFolderStart[i].canSignOut));
                    }
                }

                // Wenn Task abgebrochen wurde
                else
                {
                    break;
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Telefon Ordner
        // -----------------------------------------------------------------------
#region Telefon Ordner

        // Task // Telefon Ordner erstellen
        async Task outputPhone(string side, CancellationToken token)
        {
            // Token um Task abzubrechen
            token.ThrowIfCancellationRequested();

            // Links // vorhandene Daten löschen
            if (side == "left")
            {
                // listFiles leeren
                listFilesLeft.Clear();

                // MultiSelect entfernen
                lbLeft.SelectionMode = SelectionMode.Single;

                // In Ordnerbaum hinzu
                listFolderTreeLeft.Add(new ClassFolderTree(-1, "phone", null, null, null, "folderPhone", true, false, false, false, false, false, false, false));

                // Menü erstellen
                createAndShowMenu("left");

                // Ladeanzeigen ändern
                loadingHide("left");
                prLeft.IsActive = true;
            }

            // Rechts // vorhandene Daten löschen
            if (side == "right")
            {
                // listFiles leeren
                listFilesRight.Clear();

                // MultiSelect entfernen
                lbRight.SelectionMode = SelectionMode.Single;

                // In Ordnerbaum hinzu
                listFolderTreeRight.Add(new ClassFolderTree(-1, "phone", null, null, null, "folderPhone", true, false, false, false, false, false, false, false));

                // Menü erstellen
                createAndShowMenu("right");

                // Ladeanzeigen ändern
                loadingHide("right");
                prRight.IsActive = true;
            }

            // Telefon Ordner Daten durchlaufen
            for (int i = 0; i < listFolderPhone.Count(); i++)
            {
                // Wenn Task nicht abgebrochen wurde
                if (!token.IsCancellationRequested)
                {
                    // Links // Ausgabe erstellen
                    if (side == "left")
                    {
                        // In listFiles schreiben
                        listFilesLeft.Add(new ClassFiles(i, "left", "phone", listFolderPhone[i].folderType, listFolderPhone[i].name, new DateTimeOffset(), listFolderPhone[i].storageFolder, null, null, null, await loadIcon(listFolderPhone[i].iconName), listFolderPhone[i].canDrag, listFolderPhone[i].canDrop, listFolderPhone[i].canFlyout, listFolderPhone[i].canDelete, listFolderPhone[i].canCopy, listFolderPhone[i].canCut, listFolderPhone[i].canPaste, listFolderPhone[i].canShare, false, false));
                    }

                    // Rechts // Ausgabe erstellen
                    if (side == "right")
                    {
                        // In listFiles schreiben
                        listFilesRight.Add(new ClassFiles(i, "right", "phone", listFolderPhone[i].folderType, listFolderPhone[i].name, new DateTimeOffset(), listFolderPhone[i].storageFolder, null, null, null, await loadIcon(listFolderPhone[i].iconName), listFolderPhone[i].canDrag, listFolderPhone[i].canDrop, listFolderPhone[i].canFlyout, listFolderPhone[i].canDelete, listFolderPhone[i].canCopy, listFolderPhone[i].canCut, listFolderPhone[i].canPaste, listFolderPhone[i].canShare, false, false));
                    }
                }

                // Wenn Task abgebrochen wurde
                else
                {
                    break;
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Lokale
        // -----------------------------------------------------------------------
#region Lokal

        // Task // Lokale Ordner erstellen
        async Task outputPhoneItems(int id, string driveType, StorageFolder storageFolder, string item, string side, CancellationToken token, bool addToFolderTree = true, bool createMenu = true)
        {
            // Token um Task abzubrechen
            token.ThrowIfCancellationRequested();

            // Ordner festlegen
            if (storageFolder == null)
            {
                if (item == "folderPictures")
                {
                    storageFolder = KnownFolders.PicturesLibrary;
                }
                else if (item == "folderMusic")
                {
                    storageFolder = KnownFolders.MusicLibrary;
                }
                else if (item == "folderVideo")
                {
                    storageFolder = KnownFolders.VideosLibrary;
                }
            }

            // Dateien auslesen
            IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
            IReadOnlyList<StorageFile> fileList = new List<StorageFile>();
            folderList = await storageFolder.GetFoldersAsync();
            fileList = await storageFolder.GetFilesAsync();

            // Links // vorhandene Daten löschen
            if (side == "left")
            {
                // Dateien sortieren // Namen aufsteigend
                if (setSortLeftFiles == "name↑")
                {
                    IEnumerable<StorageFile> files = fileList.OrderBy((x) => x.Name);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Namen absteigend
                else if (setSortLeftFiles == "name↓")
                {
                    IEnumerable<StorageFile> files = fileList.OrderByDescending((x) => x.Name);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Datum aufsteigend
                else if (setSortLeftFiles == "date↑")
                {
                    IEnumerable<StorageFile> files = fileList.OrderBy((x) => x.DateCreated);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Datum absteigend
                else if (setSortLeftFiles == "date↓")
                {
                    IEnumerable<StorageFile> files = fileList.OrderByDescending((x) => x.DateCreated);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Type aufsteigend
                else if (setSortLeftFiles == "type↑")
                {
                    IEnumerable<StorageFile> files = fileList.OrderBy((x) => x.FileType);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Type absteigend
                else if (setSortLeftFiles == "type↓")
                {
                    IEnumerable<StorageFile> files = fileList.OrderByDescending((x) => x.FileType);
                    fileList = files.ToList();
                }

                // Ordner sortieren // Namen absteigend
                if (setSortLeftFolders == "name↓")
                {
                    List<StorageFolder> folders = new List<StorageFolder>();
                    for (int i = folderList.Count() - 1; i >= 0; i--)
                    {
                        folders.Add(folderList[i]);
                    }
                    folderList = folders.ToList();
                }

                // Ordner sortieren // Datum aufsteigend
                else if (setSortLeftFolders == "date↑")
                {
                    IEnumerable<StorageFolder> folders = folderList.OrderBy((x) => x.DateCreated);
                    folderList = folders.ToList();
                }

                // Ordner sortieren // Datum absteigend
                else if (setSortLeftFolders == "date↓")
                {
                    IEnumerable<StorageFolder> folders = folderList.OrderByDescending((x) => x.DateCreated);
                    folderList = folders.ToList();
                }

                // driveType festlegen
                if (driveType == null)
                {
                    driveType = listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType;
                }

                // listFiles leeren
                listFilesLeft.Clear();

                // MultiSelect entfernen
                lbLeft.SelectionMode = SelectionMode.Single;

                // In Ordnerbaum hinzu
                if (addToFolderTree)
                {
                    listFolderTreeLeft.Add(new ClassFolderTree(id, driveType, storageFolder, null, null, item, true, true, true, false, true, true, true, true));
                }

                // Menü erstellen
                if (createMenu)
                {
                    createAndShowMenu("left");
                }

                // Ladeanzeigen ändern
                loadingHide("left");
                prLeft.IsActive = true;
            }

            // Rechts // vorhandene Daten löschen
            if (side == "right")
            {
                // Dateien sortieren // Namen aufsteigend
                if (setSortRightFiles == "name↑")
                {
                    IEnumerable<StorageFile> files = fileList.OrderBy((x) => x.Name);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Namen absteigend
                else if (setSortRightFiles == "name↓")
                {
                    IEnumerable<StorageFile> files = fileList.OrderByDescending((x) => x.Name);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Datum aufsteigend
                else if (setSortRightFiles == "date↑")
                {
                    IEnumerable<StorageFile> files = fileList.OrderBy((x) => x.DateCreated);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Datum absteigend
                else if (setSortRightFiles == "date↓")
                {
                    IEnumerable<StorageFile> files = fileList.OrderByDescending((x) => x.DateCreated);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Type aufsteigend
                else if (setSortRightFiles == "type↑")
                {
                    IEnumerable<StorageFile> files = fileList.OrderBy((x) => x.FileType);
                    fileList = files.ToList();
                }

                // Dateien sortieren // Type absteigend
                else if (setSortRightFiles == "type↓")
                {
                    IEnumerable<StorageFile> files = fileList.OrderByDescending((x) => x.FileType);
                    fileList = files.ToList();
                }

                // Ordner sortieren // Namen absteigend
                if (setSortRightFolders == "name↓")
                {
                    List<StorageFolder> folders = new List<StorageFolder>();
                    for (int i = folderList.Count() - 1; i >= 0; i--)
                    {
                        folders.Add(folderList[i]);
                    }
                    folderList = folders.ToList();
                }

                // Ordner sortieren // Datum aufsteigend
                else if (setSortRightFolders == "date↑")
                {
                    IEnumerable<StorageFolder> folders = folderList.OrderBy((x) => x.DateCreated);
                    folderList = folders.ToList();
                }

                // Ordner sortieren // Datum absteigend
                else if (setSortRightFolders == "date↓")
                {
                    IEnumerable<StorageFolder> folders = folderList.OrderByDescending((x) => x.DateCreated);
                    folderList = folders.ToList();
                }

                // driveType festlegen
                if (driveType == null)
                {
                    driveType = listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType;
                }

                // listFiles leeren
                listFilesRight.Clear();

                // MultiSelect entfernen
                lbRight.SelectionMode = SelectionMode.Single;

                // In Ordnerbaum hinzufügen
                if (addToFolderTree)
                {
                    listFolderTreeRight.Add(new ClassFolderTree(id, driveType, storageFolder, null, null, item, true, true, true, false, true, true, true, true));
                }

                // Menü erstellen
                if (createMenu)
                {
                    createAndShowMenu("right");
                }

                // Ladeanzeigen ändern
                loadingHide("right");
                prRight.IsActive = true;
            }

            // Ordner durchlaufen
            for (int i = 0; i < folderList.Count(); i++)
            {
                // Wenn Task nicht abgebrochen wurde
                if (!token.IsCancellationRequested)
                {
                    // Links // Ausgabe erstellen
                    if (side == "left" | side == "both")
                    {
                        // In listFiles schreiben
                        listFilesLeft.Add(new ClassFiles(i, "left", driveType, "folder", folderList[i].DisplayName, folderList[i].DateCreated, folderList[i], null, null, null, await loadIcon("folder"), true, true, true, true, true, true, true, false, false, false));
                        
                        // Wenn MultiSelect
                        if (lbLeft.SelectionMode == SelectionMode.Multiple)
                        {
                            listFilesLeft[listFilesLeft.Count() - 1].imgSelected = new BitmapImage(new Uri("ms-appx:/Images/MulitUnchecked.png", UriKind.RelativeOrAbsolute));
                            listFilesLeft[listFilesLeft.Count() - 1].imgSelectedVisibility = Visibility.Visible;
                        }
                    }

                    // Rechts // Ausgabe erstellen
                    if (side == "right" | side == "both")
                    {
                        // In listFiles schreiben
                        listFilesRight.Add(new ClassFiles(i, "right", driveType, "folder", folderList[i].DisplayName, folderList[i].DateCreated, folderList[i], null, null, null, await loadIcon("folder"), true, true, true, true, true, true, true, false, false, false));

                        // Wenn MultiSelect
                        if (lbRight.SelectionMode == SelectionMode.Multiple)
                        {
                            listFilesRight[listFilesRight.Count() - 1].imgSelected = new BitmapImage(new Uri("ms-appx:/Images/MulitUnchecked.png", UriKind.RelativeOrAbsolute));
                            listFilesRight[listFilesRight.Count() - 1].imgSelectedVisibility = Visibility.Visible;
                        }
                    }
                }

                // Wenn Task abgebrochen wurde
                else
                {
                    break;
                }
            }

            // Dateien durchlaufen
            for (int i = 0; i < fileList.Count(); i++)
            {
                // Gibt an ob Task abgebrochen wurde
                bool br = false;

                // Links // Ausgabe erstellen
                if (side == "left")
                {
                    // Thumbnail laden
                    var thumbnail = await fileList[i].GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, 50);
                    BitmapImage icon = new BitmapImage();
                    await icon.SetSourceAsync(thumbnail);

                    // Wenn Task nicht abgebrochen wurde
                    if (!token.IsCancellationRequested)
                    {
                        // In File Liste schreiben
                        listFilesLeft.Add(new ClassFiles(i + folderList.Count(), "left", driveType, "file", fileList[i].DisplayName, fileList[i].DateCreated, null, fileList[i], null, null, icon, true, false, true, true, true, true, false, true, false, false));

                        // Wenn MultiSelect
                        if (lbLeft.SelectionMode == SelectionMode.Multiple)
                        {
                            listFilesLeft[listFilesLeft.Count() - 1].imgSelected = new BitmapImage(new Uri("ms-appx:/Images/MulitUnchecked.png", UriKind.RelativeOrAbsolute));
                            listFilesLeft[listFilesLeft.Count() - 1].imgSelectedVisibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        br = true;
                    }
                }

                // Rechts // Ausgabe erstellen
                if (side == "right")
                {
                    // Thumbnail laden
                    var thumbnail = await fileList[i].GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, 50);
                    BitmapImage icon = new BitmapImage();
                    await icon.SetSourceAsync(thumbnail);

                    // Wenn Task nicht abgebrochen wurde
                    if (!token.IsCancellationRequested)
                    {
                        // In File Liste schreiben
                        listFilesRight.Add(new ClassFiles(i + folderList.Count(), "right", driveType, "file", fileList[i].DisplayName, fileList[i].DateCreated, null, fileList[i], null, null, icon, true, false, true, true, true, true, false, true, false, false));

                        // Wenn MultiSelect
                        if (lbRight.SelectionMode == SelectionMode.Multiple)
                        {
                            listFilesRight[listFilesRight.Count() - 1].imgSelected = new BitmapImage(new Uri("ms-appx:/Images/MulitUnchecked.png", UriKind.RelativeOrAbsolute));
                            listFilesRight[listFilesRight.Count() - 1].imgSelectedVisibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        br = true;
                    }
                }

                // Wenn Task abgebrochen wurde
                if (br)
                {
                    break;
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Fenster erneuern
        // -----------------------------------------------------------------------
#region Fenster erneuern

        // Task // Fenster erneuern
        private async void outputRefresh(string side)
        {
            // Links
            if (side == "left" | side == "leftOnly" | side == "both")
            {
                // Ladeanzeige ausgeben
                loadingShow(resource.GetString("001_EinenMoment") + "...", "left");

                // Wenn beide Seiten erneuert werden
                if (side == "both")
                {
                    outputRefresh("rightOnly");
                }

                // Vorherigen Task abbrechen
                if (cancelTokenLeft != null)
                {
                    cancelTokenLeft.Cancel();
                }
                // Neues Token erstellen
                cancelTokenLeft = new CancellationTokenSource();

                // Startseite
                if (listFolderTreeLeft.Count() == 1)
                {
                    // Wenn nicht nur Links
                    if (side != "leftOnly")
                    {
                        if (listFolderTreeRight.Count() == 1)
                        {
                            // Angeben das rechts erneuert werden
                            outputRefresh("rightOnly");
                        }
                    }

                    // Fenster erneuern
                    await outputStart("left", cancelTokenLeft.Token);
                }

                // Lokal
                else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                {
                    // Wenn nicht nur Links
                    if (side != "leftOnly")
                    {
                        // Wenn beide Seiten Lokal
                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                        {
                            // Wenn links und rechts gleicher Ordner
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId)
                            {
                                // Angeben das rechts erneuert werden
                                outputRefresh("rightOnly");
                            }
                        }
                    }

                    // Fenster erneuern
                    await outputPhoneItems(listFolderTreeLeft[listFolderTreeLeft.Count() - 1].id, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description, "left", cancelTokenLeft.Token, false, false);
                }

                // One Drive
                else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                {
                    // Wenn nicht nur Links
                    if (side != "leftOnly")
                    {
                        // Wenn beide Seiten One Drive
                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                        {
                            // Wenn links und rechts gleicher Ordner
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                            {
                                // Angeben das rechts erneuert wird
                                outputRefresh("rightOnly");
                            }
                        }
                    }

                    // Fenster erneuern
                    await OneDriveLoadFolder(listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, "left", cancelTokenLeft.Token, false, false);
                }

                // Dropbox
                else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                {
                    // Wenn nicht nur Links
                    if (side != "leftOnly")
                    {
                        // Wenn beide Seiten One Drive
                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                        {
                            // Wenn links und rechts gleicher Ordner und gleiche Dropbox
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token == listFolderTreeRight[listFolderTreeRight.Count() - 1].token & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                            {
                                // Angeben das rechts erneuert wird
                                outputRefresh("rightOnly");
                            }
                        }
                    }

                    // Fenster erneuern
                    await DropboxLoadFolder(listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, "left", listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, cancelTokenLeft.Token, false, false);
                }

                // Ladeanzeigen verbergen
                loadingHide("left");
                prLeft.IsActive = false;
            }

            // Rechts
            else if (side == "right" | side == "rightOnly")
            {
                // Ladeanzeige ausgeben
                loadingShow(resource.GetString("001_EinenMoment") + "...", "right");

                // Vorherigen Task abbrechen
                if (cancelTokenRight != null)
                {
                    cancelTokenRight.Cancel();
                }
                // Neues Token erstellen
                cancelTokenRight = new CancellationTokenSource();

                // Startseite
                if (listFolderTreeRight.Count() == 1)
                {
                    // Wenn nicht nur rechts
                    if (side != "rightOnly")
                    {
                        if (listFolderTreeLeft.Count() == 1)
                        {
                            // Angeben das links erneuert werden
                            outputRefresh("leftOnly");
                        }
                    }

                    // Fenster erneuern
                    await outputStart("right", cancelTokenLeft.Token);
                }

                // Lokal
                else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                {
                    // Wenn nicht nur Rechts
                    if (side != "rightOnly")
                    {
                        // Wenn beide Seiten Lokal
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                        {
                            // Wenn links und rechts gleicher Ordner
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId)
                            {
                                // Angeben das rechts erneuert werden
                                outputRefresh("leftOnly");
                            }
                        }
                    }

                    // Fenster erneuern
                    await outputPhoneItems(listFolderTreeRight[listFolderTreeRight.Count() - 1].id, listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType, listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder, listFolderTreeRight[listFolderTreeRight.Count() - 1].description, "right", cancelTokenRight.Token, false, false);
                }

                // One Drive
                else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                {
                    // Wenn nicht nur Rechts
                    if (side != "rightOnly")
                    {
                        // Wenn beide Seiten One Drive
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                        {
                            // Wenn links und rechts gleicher Ordner
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                            {
                                // Angeben das rechts erneuert wird
                                outputRefresh("leftOnly");
                            }
                        }
                    }

                    // Fenster erneuern
                    await OneDriveLoadFolder(listFolderTreeRight[listFolderTreeRight.Count() - 1].path, "right", cancelTokenRight.Token, false, false);
                }

                // Dropbox
                else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                {
                    // Wenn nicht nur Rechts
                    if (side != "rightOnly")
                    {
                        // Wenn beide Seiten One Drive
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                        {
                            // Wenn links und rechts gleicher Ordner
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token == listFolderTreeRight[listFolderTreeRight.Count() - 1].token & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                            {
                                // Angeben das rechts erneuert wird
                                outputRefresh("leftOnly");
                            }
                        }
                    }

                    // Fenster erneuern
                    await DropboxLoadFolder(listFolderTreeRight[listFolderTreeRight.Count() - 1].path, "right", listFolderTreeRight[listFolderTreeRight.Count() - 1].token, cancelTokenRight.Token, false, false);
                }

                // Ladeanzeigen verbergen
                loadingHide("right");
                prRight.IsActive = false;
            }
        }
#endregion
        // -----------------------------------------------------------------------
#endregion

            








#region Aktionen PointerRelease // Drag and Drop // ListBox Items Selection Changed

        // ListBox Items // PointerReleased
        // -----------------------------------------------------------------------
        private async void GrList_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Gibt an ob Rechtsklick
            bool IsRightButton = false;

            // Nur bei Desktop
            if (!IsMobile)
            {
                // Wenn Maus aktiv
                if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                {
                    // Nur bei Rechtsklick
                    var properties = e.GetCurrentPoint(this).Properties;
                    if (properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
                    {
                        // Angeben das Rechtsklick
                        IsRightButton = true;

                        // Flyout Menü öffnen
                        GrList_OpenFlyout(sender);
                    }
                }
            }

            // Wenn kein Menü Flyout und kein Rechtsklick
            if (menuFlyout == null & !IsRightButton)
            {
                // Grid aus sender kopieren
                Grid copy = (Grid)sender;
                // Daten auslesen
                string tag = copy.Tag.ToString();
                string[] spTag = Regex.Split(tag, "~");
                int targetIndex = Convert.ToInt32(spTag[0]);
                string targetSide = spTag[1];

                // Wenn kein Drag and Drop // Wenn kein MultiSelect
                if (!dd & ((targetSide == "left" & lbLeft.SelectionMode == SelectionMode.Single) | targetSide == "right" & lbRight.SelectionMode == SelectionMode.Single))
                {
                    // Links // WrapPanal Tasks ausführen
                    if (targetSide == "left")
                    {
                        // Ladeanzeige ausgeben
                        loadingShow(resource.GetString("001_EinenMoment") + "...", "left");

                        // Wenn aktuelle Ordner // Start Ordner
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "folderStart")
                        {
                            // Wenn Auswahl phone
                            if (listFolderStart[targetIndex].folderType == "phone")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenLeft != null)
                                {
                                    cancelTokenLeft.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenLeft = new CancellationTokenSource();

                                // Telefon Ordner laden
                                await outputPhone(targetSide, cancelTokenLeft.Token);
                            }

                            // Wenn Auswahl sd
                            else if (listFolderStart[targetIndex].folderType == "sd")
                            {
                                // SD Karten auslesen
                                StorageFolder storageFolder = KnownFolders.RemovableDevices;
                                IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
                                folderList = await storageFolder.GetFoldersAsync();

                                // Vorherigen Task abbrechen
                                if (cancelTokenLeft != null)
                                {
                                    cancelTokenLeft.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenLeft = new CancellationTokenSource();

                                // Ordner laden
                                await outputPhoneItems(listFolderStart[targetIndex].id, "sd", folderList[listFolderStart[targetIndex].id], "sd", targetSide, cancelTokenLeft.Token);
                            }

                            // Wenn Auswahl Private
                            else if (listFolderStart[targetIndex].folderType == "private")
                            {
                                // Gibt an ob Private geöffnet wird
                                bool open = false;

                                // Wenn Private noch nicht geöffnet
                                if (!setPrivateOpen)
                                {
                                    // Gibt an ob Private geöffnet wird
                                    open = await openPrivate();
                                }

                                // Wenn Private schon geöffnet
                                else
                                {
                                    open = true;
                                }
                                    
                                // Wenn Private geöffnet wird
                                if (open)
                                {
                                    // Vorherigen Task abbrechen
                                    if (cancelTokenLeft != null)
                                    {
                                        cancelTokenLeft.Cancel();
                                    }
                                    // Neues Token erstellen
                                    cancelTokenLeft = new CancellationTokenSource();

                                    // Ordner laden
                                    await outputPhoneItems(-1, "private", folderPrivate, "private", targetSide, cancelTokenLeft.Token);
                                }
                            }

                            // Wenn Auswahl Temp
                            else if (listFolderStart[targetIndex].folderType == "temp")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenLeft != null)
                                {
                                    cancelTokenLeft.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenLeft = new CancellationTokenSource();

                                // Ordner laden
                                await outputPhoneItems(-1, "temp", folderTemp, "temp", targetSide, cancelTokenLeft.Token);
                            }

                            // Wenn Auswahl One Drive
                            else if (listFolderStart[targetIndex].folderType == "oneDrive")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenLeft != null)
                                {
                                    cancelTokenLeft.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenLeft = new CancellationTokenSource();

                                // Daten laden
                                await OneDriveLoadFolder(null, "left", cancelTokenLeft.Token);
                            }

                            // Wenn Auswahl Dropbox
                            else if (listFolderStart[targetIndex].folderType == "dropbox")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenLeft != null)
                                {
                                    cancelTokenLeft.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenLeft = new CancellationTokenSource();

                                // Daten laden
                                await DropboxLoadFolder(null, "left", listFolderStart[targetIndex].token, cancelTokenLeft.Token);
                            }

                            // Wenn File Picker
                            else if (listFolderStart[targetIndex].folderType == "filePicker")
                            {
                                FilePicker();
                            }
                        }

                        // Wenn aktueller Ordner // phone
                        else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "folderPhone")
                        {
                            // Vorherigen Task abbrechen
                            if (cancelTokenLeft != null)
                            {
                                cancelTokenLeft.Cancel();
                            }
                            // Neues Token erstellen
                            cancelTokenLeft = new CancellationTokenSource();

                            // Ordner laden
                            await outputPhoneItems(-1, "phone", null, listFolderPhone[targetIndex].folderType, targetSide, cancelTokenLeft.Token);
                        }

                        // Wenn aktueller Ordner // Lokal
                        else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                        {
                            // Wenn Auswahl ein Ordner
                            if (listFilesLeft[targetIndex].type == "folder")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenLeft != null)
                                {
                                    cancelTokenLeft.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenLeft = new CancellationTokenSource();

                                // Versuchen Ordner Daten zu laden
                                try
                                {
                                    // Ordner Daten laden
                                    await outputPhoneItems(listFolderTreeLeft[listFolderTreeLeft.Count() - 1].id, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType, listFilesLeft[targetIndex].storageFolder, "folder", targetSide, cancelTokenLeft.Token);
                                }
                                catch
                                {
                                    // Nachricht ausgeben // Zugriff verweigert
                                    DialogEx dEx = new DialogEx(resource.GetString("001_ZugriffVerweigert"));
                                    DialogExButtonsSet dBSet = new DialogExButtonsSet();
                                    DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                                    dBSet.AddButton(dBtn);
                                    dEx.AddButtonSet(dBSet);
                                    await dEx.ShowAsync(grMain);
                                }
                            }

                            // Wenn Auswahl eine Datei
                            else
                            {
                                // Datei öffnen
                                var options = new LauncherOptions();
                                options.DisplayApplicationPicker = true;
                                bool success = await Launcher.LaunchFileAsync(listFilesLeft[targetIndex].storageFile, options);

                                // Wenn Datei nicht geöffnet werden kann
                                if (!success)
                                {
                                    // Nachricht ausgeben // Nicht möglich
                                    DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"));
                                    DialogExButtonsSet dBSet = new DialogExButtonsSet();
                                    DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                                    dBSet.AddButton(dBtn);
                                    dEx.AddButtonSet(dBSet);
                                    await dEx.ShowAsync(grMain);
                                }
                            }
                        }

                        // Wenn One Drive Ordner geöffnet wird
                        else if (listFilesLeft[targetIndex].driveType == "oneDrive" & listFilesLeft[targetIndex].type == "folder")
                        {
                            // Pfad erstellen
                            string path = listFilesLeft[targetIndex].item.ParentReference.Path;
                            path = Regex.Replace(path, ":", "/");
                            path += "/" + listFilesLeft[targetIndex].item.Name;
                            path = Regex.Replace(path, "/drive/root/", "");
                            
                            // Vorherigen Task abbrechen
                            if (cancelTokenLeft != null)
                            {
                                cancelTokenLeft.Cancel();
                            }
                            // Neues Token erstellen
                            cancelTokenLeft = new CancellationTokenSource();
                            // Daten laden
                            await OneDriveLoadFolder(path, "left", cancelTokenLeft.Token);
                        }

                        // Wenn One Drive Datei geöffnet wird
                        else if (listFilesLeft[targetIndex].driveType == "oneDrive" & listFilesLeft[targetIndex].type == "file")
                        {
                            // Gibt an ob geöffnet wird
                            bool open = false;

                            // Wenn Datei durch Internet Explorer geöffnet wird
                            if (listFilesLeft[targetIndex].item.Size > setOpenByDownloadingLimit)
                            {
                                // Content erstellen
                                double dbSize = Convert.ToDouble(listFilesLeft[targetIndex].item.Size);
                                dbSize = Math.Round(dbSize / Convert.ToDouble(1000000), 2);
                                string content = dbSize + " MB";

                                // Nachricht ausgeben
                                DialogEx dQuery = new DialogEx();
                                dQuery.Title = resource.GetString("001_DateiGrößerLimit");
                                dQuery.Content = content;
                                DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Ignorieren"));
                                btnSet.AddButton(btn_1);
                                DialogExButton btn_2 = new DialogExButton(resource.GetString("001_OnlineÖffnen"));
                                btnSet.AddButton(btn_2);
                                DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                btnSet.AddButton(btn_3);
                                dQuery.AddButtonSet(btnSet);
                                await dQuery.ShowAsync(grMain);

                                // Antwort auslesen
                                string answer = dQuery.GetAnswer();

                                // Wenn Ignorieren
                                if (answer == resource.GetString("001_Ignorieren"))
                                {
                                    open = true;
                                }

                                // Wenn Online Öffnen
                                else if (answer == resource.GetString("001_OnlineÖffnen"))
                                {
                                    // Datei im Internet Explorer öffnen
                                    await Launcher.LaunchUriAsync(new Uri(listFilesLeft[targetIndex].item.WebUrl));
                                }
                            }

                            // Wenn Datei kleiner als das Limit
                            else
                            {
                                open = true;
                            }

                            // Wenn Datei durch Download geöffnet wird
                            if (open)
                            {
                                // Ausgabe erstellen
                                DialogEx dWaitAbort = new DialogEx();
                                dWaitAbort.Title = resource.GetString("001_DateiHeruntergeladen");
                                dWaitAbort.Content = resource.GetString("001_EinenMoment") + "...";
                                DialogExProgressRing progressRing = new DialogExProgressRing();
                                progressRing = new DialogExProgressRing();
                                dWaitAbort.AddProgressRing(progressRing);
                                DialogExTextBlock tBlock = new DialogExTextBlock();
                                tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                                tBlock.FontSize = 10;
                                tBlock.Margin = new Thickness(0, 8, 0, -8);
                                tBlock.Text = listFilesLeft[targetIndex].item.Name;
                                dWaitAbort.AddTextBlock(tBlock);
                                DialogExProgressBar proBar = new DialogExProgressBar();
                                proBar.Maximun = Convert.ToInt32(listFilesLeft[targetIndex].item.Size);
                                dWaitAbort.AddProgressBar(proBar);
                                DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                                DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                                dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                                dWaitAbort.ShowAsync(grMain);

                                // Datei herunterladen
                                await OneDriveOpenFileByDownload(listFilesLeft[targetIndex].item, dWaitAbort);

                                // Ausgabe entfernen
                                dWaitAbort.Hide();

                                // Wenn rechts Temponäre geöffnet
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "temp")
                                {
                                    outputRefresh("right");
                                }
                            }
                        }

                        // Wenn Dropbox Ordner geöffnet wird
                        else if (listFilesLeft[targetIndex].driveType == "dropbox" & listFilesLeft[targetIndex].type == "folder")
                        {
                            // Pfad erstellen
                            string path = listFilesLeft[targetIndex].metadata.AsFolder.PathDisplay;

                            // Vorherigen Task abbrechen
                            if (cancelTokenLeft != null)
                            {
                                cancelTokenLeft.Cancel();
                            }
                            // Neues Token erstellen
                            cancelTokenLeft = new CancellationTokenSource();
                            // Daten laden
                            await DropboxLoadFolder(path, "left", listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, cancelTokenLeft.Token);
                        }

                        // Wenn Dropbox Datei geöffnet wird
                        else if (listFilesLeft[targetIndex].driveType == "dropbox" & listFilesLeft[targetIndex].type == "file")
                        {
                            // Gibt an ob geöffnet wird
                            bool open = false;

                            // Wenn Datei durch Internet Explorer geöffnet wird
                            if (Convert.ToInt32(listFilesLeft[targetIndex].metadata.AsFile.Size) > setOpenByDownloadingLimit)
                            {
                                // Content erstellen
                                double dbSize = Convert.ToDouble(listFilesLeft[targetIndex].metadata.AsFile.Size);
                                dbSize = Math.Round(dbSize / Convert.ToDouble(1000000), 2);
                                string content = dbSize + " MB";

                                // Nachricht ausgeben
                                DialogEx dQuery = new DialogEx();
                                dQuery.Title = resource.GetString("001_DateiGrößerLimit");
                                dQuery.Content = content;
                                DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Ignorieren"));
                                btnSet.AddButton(btn_1);
                                DialogExButton btn_2 = new DialogExButton(resource.GetString("001_OnlineÖffnen"));
                                btnSet.AddButton(btn_2);
                                DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                btnSet.AddButton(btn_3);
                                dQuery.AddButtonSet(btnSet);
                                await dQuery.ShowAsync(grMain);

                                // Antwort auslesen
                                string answer = dQuery.GetAnswer();

                                // Wenn Ignorieren
                                if (answer == resource.GetString("001_Ignorieren"))
                                {
                                    open = true;
                                }

                                // Wenn Online Öffnen
                                else if (answer == resource.GetString("001_OnlineÖffnen"))
                                {
                                    // Uri erstellen
                                    string uri = "https://www.dropbox.com/home";
                                    string[] spPath = Regex.Split(listFilesLeft[targetIndex].metadata.AsFile.PathDisplay, "/");
                                    for (int i = 0; i < spPath.Count() - 1; i++)
                                    {
                                        uri += "/" + spPath[i];
                                    }
                                    uri += "?Preview=" + spPath[spPath.Count() - 1];

                                    // Datei im Internet Explorer öffnen
                                    await Launcher.LaunchUriAsync(new Uri(uri));
                                }
                            }

                            // Wenn Datei kleiner als das Limit
                            else
                            {
                                open = true;
                            }

                            // Wenn Datei durch Download geöffnet wird
                            if (open)
                            {
                                // Ausgabe erstellen
                                DialogEx dWaitAbort = new DialogEx();
                                dWaitAbort.Title = resource.GetString("001_DateiHeruntergeladen");
                                dWaitAbort.Content = resource.GetString("001_EinenMoment") + "...";
                                DialogExProgressRing progressRing = new DialogExProgressRing();
                                progressRing = new DialogExProgressRing();
                                dWaitAbort.AddProgressRing(progressRing);
                                DialogExTextBlock tBlock = new DialogExTextBlock();
                                tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                                tBlock.FontSize = 10;
                                tBlock.Margin = new Thickness(0, 8, 0, -8);
                                tBlock.Text = listFilesLeft[targetIndex].metadata.Name;
                                dWaitAbort.AddTextBlock(tBlock);
                                DialogExProgressBar proBar = new DialogExProgressBar();
                                proBar.Maximun = Convert.ToInt32(listFilesLeft[targetIndex].metadata.AsFile.Size);
                                dWaitAbort.AddProgressBar(proBar);
                                DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                                DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                                dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                                dWaitAbort.ShowAsync(grMain);

                                // Datei herunterladen
                                await DropboxOpenFileByDownload(listFilesLeft[targetIndex].metadata, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, dWaitAbort);

                                // Ausgabe entfernen
                                dWaitAbort.Hide();

                                // Wenn rechts Temponäre geöffnet
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "temp")
                                {
                                    outputRefresh("right");
                                }
                            }
                        }

                        // Ladeanzeigen verbergen
                        loadingHide("left");
                        prLeft.IsActive = false;
                    }

                    // Rechts // WrapPanal Tasks ausführen
                    else if (targetSide == "right")
                    {
                        // Ladeanzeige ausgeben
                        loadingShow(resource.GetString("001_EinenMoment") + "...", "right");

                        // Wenn aktuelle Ordner // Start Ordner
                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "folderStart")
                        {
                            // Wenn Auswahl Phone
                            if (listFolderStart[targetIndex].folderType == "phone")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenRight != null)
                                {
                                    cancelTokenRight.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenRight = new CancellationTokenSource();

                                // Telefon Ordner laden
                                await outputPhone(targetSide, cancelTokenRight.Token);
                            }

                            // Wenn Auswahl sd
                            else if (listFolderStart[targetIndex].folderType == "sd")
                            {
                                // SD Karten auslesen
                                StorageFolder storageFolder = KnownFolders.RemovableDevices;
                                IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
                                folderList = await storageFolder.GetFoldersAsync();

                                // Vorherigen Task abbrechen
                                if (cancelTokenRight != null)
                                {
                                    cancelTokenRight.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenRight = new CancellationTokenSource();

                                // Ordner laden
                                await outputPhoneItems(listFolderStart[targetIndex].id, "sd", folderList[listFolderStart[targetIndex].id], "sd", targetSide, cancelTokenRight.Token);
                            }

                            // Wenn Auswahl Private
                            else if (listFolderStart[targetIndex].folderType == "private")
                            {
                                // Gibt an ob Private geöffnet wird
                                bool open = false;

                                // Wenn Private noch nicht geöffnet
                                if (!setPrivateOpen)
                                {
                                    // Gibt an ob Private geöffnet wird
                                    open = await openPrivate();
                                }

                                // Wenn Private schon geöffnet
                                else
                                {
                                    open = true;
                                }

                                // Wenn Private geöffnet wird
                                if (open)
                                {
                                    // Vorherigen Task abbrechen
                                    if (cancelTokenLeft != null)
                                    {
                                        cancelTokenLeft.Cancel();
                                    }
                                    // Neues Token erstellen
                                    cancelTokenLeft = new CancellationTokenSource();

                                    // Ordner laden
                                    await outputPhoneItems(-1, "private", folderPrivate, "private", targetSide, cancelTokenLeft.Token);
                                }
                            }

                            // Wenn Auswahl Temp
                            else if (listFolderStart[targetIndex].folderType == "temp")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenRight != null)
                                {
                                    cancelTokenRight.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenRight = new CancellationTokenSource();

                                // Ordner laden
                                await outputPhoneItems(-1, "temp", folderTemp, "temp", targetSide, cancelTokenLeft.Token);
                            }

                            // Wenn Auswahl One Drive
                            else if (listFolderStart[targetIndex].folderType == "oneDrive")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenRight != null)
                                {
                                    cancelTokenRight.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenRight = new CancellationTokenSource();

                                // Daten laden
                                await OneDriveLoadFolder(null, "right", cancelTokenRight.Token);
                            }

                            // Wenn Auswahl Dropbox
                            else if (listFolderStart[targetIndex].folderType == "dropbox")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenRight != null)
                                {
                                    cancelTokenRight.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenRight = new CancellationTokenSource();

                                // Daten laden
                                await DropboxLoadFolder(null, "right", listFolderStart[targetIndex].token, cancelTokenRight.Token);
                            }

                            // Wenn File Picker
                            else if (listFolderStart[targetIndex].folderType == "filePicker")
                            {
                                FilePicker();
                            }

                        }

                        // Wenn aktuller Ordner // phone
                        else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "folderPhone")
                        {
                            // Vorherigen Task abbrechen
                            if (cancelTokenRight != null)
                            {
                                cancelTokenRight.Cancel();
                            }
                            // Neues Token erstellen
                            cancelTokenRight = new CancellationTokenSource();

                            // Ordner laden
                            await outputPhoneItems(-1, "phone", null, listFolderPhone[targetIndex].folderType, targetSide, cancelTokenRight.Token);
                        }

                        // Wenn aktueller Ordner // Lokal
                        else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                        {
                            // Wenn Auswahl ein Ordner
                            if (listFilesRight[targetIndex].type == "folder")
                            {
                                // Vorherigen Task abbrechen
                                if (cancelTokenRight != null)
                                {
                                    cancelTokenRight.Cancel();
                                }
                                // Neues Token erstellen
                                cancelTokenRight = new CancellationTokenSource();

                                // Versuchen Ordner Daten zu laden
                                try
                                {
                                    // Ordner Daten laden
                                    await outputPhoneItems(listFolderTreeRight[listFolderTreeRight.Count() - 1].id, listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType, listFilesRight[targetIndex].storageFolder, "folder", targetSide, cancelTokenRight.Token);
                                }
                                catch
                                {
                                    // Nachricht ausgeben // Zugriff verweigert
                                    DialogEx dEx = new DialogEx(resource.GetString("001_ZugriffVerweigert"));
                                    DialogExButtonsSet dBSet = new DialogExButtonsSet();
                                    DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                                    dBSet.AddButton(dBtn);
                                    dEx.AddButtonSet(dBSet);
                                    await dEx.ShowAsync(grMain);
                                }
                            }

                            // Wenn Auswahl eine Datei
                            else
                            {
                                // Datei öffnen
                                var options = new LauncherOptions();
                                options.DisplayApplicationPicker = true;
                                bool success = await Launcher.LaunchFileAsync(listFilesRight[targetIndex].storageFile, options);

                                // Wenn Datei nicht geöffnet werden kann
                                if (!success)
                                {
                                    // Nachricht ausgeben // Nicht möglich
                                    DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"));
                                    DialogExButtonsSet dBSet = new DialogExButtonsSet();
                                    DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                                    dBSet.AddButton(dBtn);
                                    dEx.AddButtonSet(dBSet);
                                    await dEx.ShowAsync(grMain);
                                }
                            }
                        }

                        // Wenn One Drive Ordner geöffnet wird
                        else if (listFilesRight[targetIndex].driveType == "oneDrive" & listFilesRight[targetIndex].type == "folder")
                        {
                            // Pfad erstellen
                            string path = listFilesRight[targetIndex].item.ParentReference.Path;
                            path = Regex.Replace(path, ":", "/");
                            path += "/" + listFilesRight[targetIndex].item.Name;
                            path = Regex.Replace(path, "/drive/root/", "");

                            // Vorherigen Task abbrechen
                            if (cancelTokenRight != null)
                            {
                                cancelTokenRight.Cancel();
                            }
                            //Neues Token erstellen
                            cancelTokenRight = new CancellationTokenSource();

                            // Daten laden
                            await OneDriveLoadFolder(path, "right", cancelTokenRight.Token);
                        }

                        // Wenn One Drive Datei geöffnet wird
                        else if (listFilesRight[targetIndex].driveType == "oneDrive" & listFilesRight[targetIndex].type == "file")
                        {
                            // Gibt an ob geöffnet wird
                            bool open = false;

                            // Wenn Datei durch Internet Explorer geöffnet wird
                            if (listFilesRight[targetIndex].item.Size > setOpenByDownloadingLimit)
                            {
                                // Content erstellen
                                double dbSize = Convert.ToDouble(listFilesRight[targetIndex].item.Size);
                                dbSize = Math.Round(dbSize / Convert.ToDouble(1000000), 2);
                                string content = dbSize + " MB";

                                // Nachricht ausgeben
                                DialogEx dQuery = new DialogEx();
                                dQuery.Title = resource.GetString("001_DateiGrößerLimit");
                                dQuery.Content = content;
                                DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Ignorieren"));
                                btnSet.AddButton(btn_1);
                                DialogExButton btn_2 = new DialogExButton(resource.GetString("001_OnlineÖffnen"));
                                btnSet.AddButton(btn_2);
                                DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                btnSet.AddButton(btn_3);
                                dQuery.AddButtonSet(btnSet);
                                await dQuery.ShowAsync(grMain);

                                // Antwort auslesen
                                string answer = dQuery.GetAnswer();

                                // Wenn Ignorieren
                                if (answer == resource.GetString("001_Ignorieren"))
                                {
                                    open = true;
                                }

                                // Wenn Online Öffnen
                                else if (answer == resource.GetString("001_OnlineÖffnen"))
                                {
                                    // Datei im Internet Explorer öffnen
                                    await Launcher.LaunchUriAsync(new Uri(listFilesRight[targetIndex].item.WebUrl));
                                }
                            }

                            // Wenn Datei kleiner als das Limit
                            else
                            {
                                open = true;
                            }

                            // Wenn Datei durch Download geöffnet wird
                            if (open)
                            {
                                // Ausgabe erstellen
                                DialogEx dWaitAbort = new DialogEx();
                                dWaitAbort.Title = resource.GetString("001_DateiHeruntergeladen");
                                dWaitAbort.Content = resource.GetString("001_EinenMoment") + "...";
                                DialogExProgressRing progressRing = new DialogExProgressRing();
                                progressRing = new DialogExProgressRing();
                                dWaitAbort.AddProgressRing(progressRing);
                                DialogExTextBlock tBlock = new DialogExTextBlock();
                                tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                                tBlock.FontSize = 10;
                                tBlock.Margin = new Thickness(0, 8, 0, -8);
                                tBlock.Text = listFilesRight[targetIndex].item.Name;
                                dWaitAbort.AddTextBlock(tBlock);
                                DialogExProgressBar proBar = new DialogExProgressBar();
                                proBar.Maximun = Convert.ToInt32(listFilesRight[targetIndex].item.Size);
                                dWaitAbort.AddProgressBar(proBar);
                                DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                                DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                                dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                                dWaitAbort.ShowAsync(grMain);

                                // Datei herunterladen
                                await OneDriveOpenFileByDownload(listFilesRight[targetIndex].item, dWaitAbort);

                                // Ausgabe entfernen
                                dWaitAbort.Hide();

                                // Wenn links Temponäre geöffnet
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "temp")
                                {
                                    outputRefresh("left");
                                }
                            }
                        }

                        // Wenn Dropbox Ordner geöffnet wird
                        else if (listFilesRight[targetIndex].driveType == "dropbox" & listFilesRight[targetIndex].type == "folder")
                        {
                            // Pfad erstellen
                            string path = listFilesRight[targetIndex].metadata.AsFolder.PathDisplay;

                            // Vorherigen Task abbrechen
                            if (cancelTokenRight != null)
                            {
                                cancelTokenRight.Cancel();
                            }
                            //Neues Token erstellen
                            cancelTokenRight = new CancellationTokenSource();

                            // Daten laden
                            await DropboxLoadFolder(path, "right", listFolderTreeRight[listFolderTreeRight.Count() - 1].token, cancelTokenRight.Token);
                        }

                        // Wenn Dropbox Datei geöffnet wird
                        else if (listFilesRight[targetIndex].driveType == "dropbox" & listFilesRight[targetIndex].type == "file")
                        {
                            // Gibt an ob geöffnet wird
                            bool open = false;

                            // Wenn Datei größer als das Limit
                            if (Convert.ToInt32(listFilesRight[targetIndex].metadata.AsFile.Size) > setOpenByDownloadingLimit)
                            {
                                // Content erstellen
                                double dbSize = Convert.ToDouble(listFilesRight[targetIndex].metadata.AsFile.Size);
                                dbSize = Math.Round(dbSize / Convert.ToDouble(1000000), 2);
                                string content = dbSize + " MB";

                                // Nachricht ausgeben
                                DialogEx dQuery = new DialogEx();
                                dQuery.Title = resource.GetString("001_DateiGrößerLimit");
                                dQuery.Content = content;
                                DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Ignorieren"));
                                btnSet.AddButton(btn_1);
                                DialogExButton btn_2 = new DialogExButton(resource.GetString("001_OnlineÖffnen"));
                                btnSet.AddButton(btn_2);
                                DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                btnSet.AddButton(btn_3);
                                dQuery.AddButtonSet(btnSet);
                                await dQuery.ShowAsync(grMain);

                                // Antwort auslesen
                                string answer = dQuery.GetAnswer();

                                // Wenn Ignorieren
                                if (answer == resource.GetString("001_Ignorieren"))
                                {
                                    open = true;
                                }

                                // Wenn Online Öffnen
                                else if (answer == resource.GetString("001_OnlineÖffnen"))
                                {
                                    // Uri erstellen
                                    string uri = "https://www.dropbox.com/home";
                                    string[] spPath = Regex.Split(listFilesRight[targetIndex].metadata.AsFile.PathDisplay, "/");
                                    for (int i = 0; i < spPath.Count() - 1; i++)
                                    {
                                        uri += "/" + spPath[i];
                                    }
                                    uri += "?Preview=" + spPath[spPath.Count() - 1];

                                    // Datei im Internet Explorer öffnen
                                    await Launcher.LaunchUriAsync(new Uri(uri));
                                }
                            }

                            // Wenn Datei kleiner als das Limit
                            else
                            {
                                open = true;
                            }

                            // Wenn Datei durch Download geöffnet wird
                            if (open)
                            {
                                // Ausgabe erstellen
                                DialogEx dWaitAbort = new DialogEx();
                                dWaitAbort.Title = resource.GetString("001_DateiHeruntergeladen");
                                dWaitAbort.Content = resource.GetString("001_EinenMoment") + "...";
                                DialogExProgressRing progressRing = new DialogExProgressRing();
                                progressRing = new DialogExProgressRing();
                                dWaitAbort.AddProgressRing(progressRing);
                                DialogExTextBlock tBlock = new DialogExTextBlock();
                                tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                                tBlock.FontSize = 10;
                                tBlock.Margin = new Thickness(0, 8, 0, -8);
                                tBlock.Text = listFilesRight[targetIndex].metadata.Name;
                                dWaitAbort.AddTextBlock(tBlock);
                                DialogExProgressBar proBar = new DialogExProgressBar();
                                proBar.Maximun = Convert.ToInt32(listFilesRight[targetIndex].metadata.AsFile.Size);
                                dWaitAbort.AddProgressBar(proBar);
                                DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                                DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                                dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                                dWaitAbort.ShowAsync(grMain);

                                // Datei herunterladen
                                await DropboxOpenFileByDownload(listFilesRight[targetIndex].metadata, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, dWaitAbort);

                                // Ausgabe entfernen
                                dWaitAbort.Hide();

                                // Wenn links Temponäre geöffnet
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "temp")
                                {
                                    outputRefresh("left");
                                }
                            }
                        }

                        // Ladeanzeigen verbergen
                        loadingHide("right");
                        prRight.IsActive = false;
                    }
                }

                // Wenn kein Drag and Drop // Wenn Multiselect
                else if (!dd & ((targetSide == "left" & lbLeft.SelectionMode == SelectionMode.Multiple) | targetSide == "right" & lbRight.SelectionMode == SelectionMode.Multiple))
                {
                    // Gibt an ob Item ausgewählt
                    bool selected = true;

                    // Wenn links
                    if (targetSide == "left")
                    {
                        // Prüfen of Item ausgewählt ist
                        foreach (var item in lbLeft.SelectedItems)
                        {
                            // Index in Liste schreibenz
                            if (targetIndex == lbLeft.Items.IndexOf(item))
                            {
                                selected = false;
                                break;
                            }
                        }

                        // Wenn Item ausgewählt ist
                        if (selected)
                        {
                            listFilesLeft[targetIndex].selectItem();
                        }

                        // Wenn Item nicht ausgewählt ist
                        else
                        {
                            listFilesLeft[targetIndex].unselectItem();
                        }
                    }

                    // Wenn rechts
                    if (targetSide == "right")
                    {
                        // Prüfen of Item ausgewählt ist
                        foreach (var item in lbRight.SelectedItems)
                        {
                            // Index in Liste schreibenz
                            if (targetIndex == lbRight.Items.IndexOf(item))
                            {
                                selected = false;
                                break;
                            }
                        }

                        // Wenn Item ausgewählt ist
                        if (selected)
                        {
                            listFilesRight[targetIndex].selectItem();
                        }

                        // Wenn Item nicht ausgewählt ist
                        else
                        {
                            listFilesRight[targetIndex].unselectItem();
                        }
                    }
                }

                // Bei Drag and Drop
                else
                {
                    // Wenn Drop möglich
                    if (dropAllow)
                    {
                        // Wenn links
                        if (ddTargetSide_Entered == "left")
                        {
                            // ListBox Item Hintergrund ändern
                            listFilesLeft[targetIndex].background = new SolidColorBrush(Colors.Transparent);
                        }

                        // Wenn rechts
                        else if (ddTargetSide_Entered == "right")
                        {
                            // ListBox Item Hintergrund ändern
                            listFilesRight[targetIndex].background = new SolidColorBrush(Colors.Transparent);
                        }

                        // Aktione Drag and Drop starten
                        acDragAndDrop(ddTargetSide_Entered, targetIndex);
                    }

                    // Drag and Drop Beenden
                    dd = false;
                }
            }
        }
        // -----------------------------------------------------------------------
#endregion










#region Drag and Drop
        // Drag and Drop
        // -----------------------------------------------------------------------
#region Drag and Drop

        // Bauteile // Drag and Drop Anzeige
        private Grid grDdData;
        private StackPanel spDd;
        private Windows.UI.Xaml.Controls.Image imgDd;
        private TextBlock tbDdData;

        // Parameter // Gibt an ob Drag and Drop ausgeführt wird // Verwaltet Drag and Drop Anzeige
        private bool _dd = false;
        private bool dd
        {
            // Status abrufen
            get
            {
                // Argument zurückgeben
                return _dd;
            }

            // Status setzen
            set
            {
                // Argument setzen
                _dd = value;

                // Wenn Drag and Drop aktiviert wird
                if (_dd)
                {
                    // Drag and Drop Anzeige entfernen, wenn noch vorhanden
                    try
                    {
                        grMain.Children.Remove(grMain.FindName("grDdData") as UIElement);
                    }
                    catch { }

                    // Drag and Drop Anzeige neu erstellen
                    grDdData = new Grid
                    {
                        Name = "grDdData",
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Background = new SolidColorBrush(Color.FromArgb(153,0,0,0)),
                    };
                    spDd = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(6),
                    };
                    imgDd = new Windows.UI.Xaml.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("ms-appx:/Images/PointerDrag.png", UriKind.RelativeOrAbsolute)),
                        Width = 30,
                        Height = 30,
                    };
                    tbDdData = new TextBlock
                    {
                        Margin = new Thickness(6, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 12,
                        Foreground = new SolidColorBrush(Colors.White),
                        TextWrapping = TextWrapping.Wrap,
                    };
                    spDd.Children.Add(imgDd);
                    spDd.Children.Add(tbDdData);
                    grDdData.Children.Add(spDd);
                    grMain.Children.Add(grDdData);
                }

                // Wenn Drag and Drop deaktiviert wird
                else
                {
                    // Drop möglich zurücksetzen
                    dropAllow = false;

                    // Drag and Drop Anzeige entfernen
                    grMain.Children.Remove(grMain.FindName("grDdData") as UIElement);
                }
            }
        }

        // Parameter // Gibt an ob Drop möglich ist // Verändert Drag and Drop Anzeige
        bool _dropAllow = false;
        bool dropAllow
        {
            // Status abrufen
            get
            {
                // Argument zurückgeben
                return _dropAllow;
            }

            // Status setzen
            set
            {
                // Argument setzen
                _dropAllow = value;

                // Wenn Drag and Drop Aktiv
                if(dd)
                {
                    // Wenn Drop möglich
                    if (_dropAllow)
                    {
                        // Ziel --> Item Links
                        if (_ddTargetSide_Entered == "left")
                        {
                            // Hintergrund Item Links ändern
                            listFilesLeft[ddTargetIndex].changeBackground(scbGrDrop);

                            // Hintergrund ScrollViewer Links ändern
                            grLeft.Background = new SolidColorBrush(Colors.Transparent);

                            // Hintergrund ScrollViewer Rechts ändern
                            grRight.Background = new SolidColorBrush(Colors.Transparent);
                        }

                        // Ziel --> Item Rechts
                        else if (_ddTargetSide_Entered == "right")
                        {
                            // Hintergrund Item Links ändern
                            listFilesRight[ddTargetIndex].changeBackground(scbGrDrop);

                            // Hintergrund ScrollViewer Links ändern
                            grLeft.Background = new SolidColorBrush(Colors.Transparent);

                            // Hintergrund ScrollViewer Rechts ändern
                            grRight.Background = new SolidColorBrush(Colors.Transparent);
                        }

                        // Ziel --> Scrollviewer Links
                        else if (_ddTargetSide_Entered == "svLeft")
                        {
                            // Hintergrund ScrollViewer Links ändern
                            grLeft.Background = scbSvDrop;

                            // Hintergrund ScrollViewer Rechts ändern
                            grRight.Background = new SolidColorBrush(Colors.Transparent);
                        }

                        // Ziel --> Scrollviewer Rechts
                        else if (_ddTargetSide_Entered == "svRight")
                        {
                            // Hintergrund ScrollViewer Links ändern
                            grLeft.Background = new SolidColorBrush(Colors.Transparent);

                            // Hintergrund ScrollViewer Rechts ändern
                            grRight.Background = scbSvDrop;
                        }

                        // Drag and Drop Anzeige ändern
                        imgDd.Source = new BitmapImage(new Uri("ms-appx:/Images/PointerDrop.png", UriKind.RelativeOrAbsolute));
                    }

                    // Wenn Drop nicht möglich
                    else
                    {
                        // Hintergrund ScrollViewer Links ändern
                        grLeft.Background = new SolidColorBrush(Colors.Transparent);

                        // Hintergrund ScrollViewer Rechts ändern
                        grRight.Background = new SolidColorBrush(Colors.Transparent);

                        // Drag and Drop Anzeige ändern
                        imgDd.Source = new BitmapImage(new Uri("ms-appx:/Images/PointerDrag.png", UriKind.RelativeOrAbsolute));
                    }
                }
            }
        }

        // Gibt an wo Drag and Drop einfährt // Ändert das UI
        private string _ddTargetSide_Entered;
        private string ddTargetSide_Entered
        {
            // Status abrufen
            get
            {
                // Argument zurückgeben
                return _ddTargetSide_Entered;
            }

            // Status setzen
            set
            {
                // Wenn Drag and Drop aktiv
                if (dd)
                {
                    // Argument setzen
                    _ddTargetSide_Entered = value;

                    // Gibt an ob Drop möglich
                    bool isAllow = true;

                    // Quelle --> Links
                    if (ddSourceSide == "left")
                    {
                        // Quelle --> Links // Ziel --> Item Links
                        if (_ddTargetSide_Entered == "left")
                        {
                            // Quelle --> Lokal // Ziel --> Lokal
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFilesLeft[ddTargetIndex].storageFolder != null)
                            {
                                // Quelldaten durchlaufen
                                for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                {
                                    // Wenn Quelldatei ein Ordner
                                    if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                                    {
                                        // Wenn Quelle gleich Ziel
                                        if (listFilesLeft[ddListSourceIndexes[i]].storageFolder.FolderRelativeId == listFilesLeft[ddTargetIndex].storageFolder.FolderRelativeId)
                                        {
                                            // Angeben das Drop nicht möglich
                                            isAllow = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            // Quelle --> One Drive // Ziel --> One Dive
                            else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFilesLeft[ddTargetIndex].driveType == "oneDrive")
                            {
                                // Quelldaten durchlaufen
                                for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                {
                                    // Wenn Quelldatei ein Ordner
                                    if (listFilesLeft[ddListSourceIndexes[i]].item.Folder != null)
                                    {
                                        // Wenn Quelle gleich Ziel
                                        if (listFilesLeft[ddListSourceIndexes[i]].item.Id == listFilesLeft[ddTargetIndex].item.Id)
                                        {
                                            // Angeben das Drop nicht möglich
                                            isAllow = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            // Quelle --> Dropbox // Ziel --> Dropbox
                            else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFilesLeft[ddTargetIndex].driveType == "dropbox")
                            {
                                // Quelldaten durchlaufen
                                for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                {
                                    // Wenn Quelldatei ein Ordner
                                    if (listFilesLeft[ddListSourceIndexes[i]].metadata.IsFolder)
                                    {
                                        // Wenn Quelle gleich Ziel
                                        if (listFilesLeft[ddListSourceIndexes[i]].metadata.AsFolder.Id == listFilesLeft[ddTargetIndex].metadata.AsFolder.Id)
                                        {
                                            // Angeben das Drop nicht möglich
                                            isAllow = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // Quelle --> Links // Ziel --> Item Rechts
                        else if (_ddTargetSide_Entered == "right")
                        {
                            // Quelle --> Lokal // Ziel --> Lokal
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFilesRight[ddTargetIndex].storageFolder != null)
                            {
                                // Wenn Quelle nicht gleich Ziel
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId != listFilesRight[ddTargetIndex].storageFolder.FolderRelativeId)
                                {
                                    // Quelldaten durchlaufen
                                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                    {
                                        // Wenn Quelle ein Ordner
                                        if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                                        {
                                            // Wenn Quelle gleich Ziel
                                            if (listFilesLeft[ddListSourceIndexes[i]].storageFolder.FolderRelativeId == listFilesRight[ddTargetIndex].storageFolder.FolderRelativeId)
                                            {
                                                // Angeben das Drop nicht möglich
                                                isAllow = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // Wenn Quelle gleich Ziel
                                else
                                {
                                    // Angeben das Drop nicht möglich
                                    isAllow = false;
                                }
                            }

                            // Quelle --> One Drive // Ziel --> One Drive
                            else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFilesRight[ddTargetIndex].driveType == "oneDrive")
                            {
                                // Quelldaten durchlaufen
                                for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                {
                                    // Wenn Quelldatei ein Ordner
                                    if (listFilesLeft[ddListSourceIndexes[i]].item.Folder != null)
                                    {
                                        // Wenn Quelle gleich Ziel
                                        if (listFilesLeft[ddListSourceIndexes[i]].item.Id == listFilesRight[ddTargetIndex].item.Id)
                                        {
                                            // Angeben das Drop nicht möglich
                                            isAllow = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            // Quelle --> Dropbox // Ziel --> Dropbox
                            else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFilesRight[ddTargetIndex].driveType == "dropbox")
                            {
                                // Wenn Quelle und Ziel gleiche Dropbox
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                                {
                                    // Quelldaten durchlaufen
                                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                    {
                                        // Wenn Quelldatei ein Ordner
                                        if (listFilesLeft[ddListSourceIndexes[i]].metadata.IsFolder)
                                        {
                                            // Wenn Quelle gleich Ziel
                                            if (listFilesLeft[ddListSourceIndexes[i]].metadata.AsFolder.Id == listFilesRight[ddTargetIndex].metadata.AsFolder.Id)
                                            {
                                                // Angeben das Drop nicht möglich
                                                isAllow = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            // Quelle != Ziel
                            else
                            {
                                // Wenn Ouelle und Ziel verschiedene Clouds
                                if ((listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFilesRight[ddTargetIndex].driveType == "oneDrive") | (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFilesRight[ddTargetIndex].driveType == "dropbox"))
                                {
                                    isAllow = false;
                                }
                            }
                        }

                        // Quelle --> Links // Ziel --> Scrollviewer Links
                        else if (_ddTargetSide_Entered == "svLeft")
                        {
                            isAllow = false;
                        }

                        // Quelle --> Links // Ziel --> Scrollviewer Rechts
                        else if (_ddTargetSide_Entered == "svRight")
                        {
                            // Quelle --> Lokal // Ziel --> Lokal
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                            {
                                // Wenn Quelle nicht gleich Ziel
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId != listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId)
                                {
                                    // Quelldaten durchlaufen
                                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                    {
                                        // Wenn Quelle ein Ordner
                                        if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                                        {
                                            // Wenn Quelle gleich Ziel
                                            if (listFilesLeft[ddListSourceIndexes[i]].storageFolder.FolderRelativeId == listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId)
                                            {
                                                // Angeben das Drop nicht möglich
                                                isAllow = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // Wenn Quelle gleich Ziel
                                else
                                {
                                    // Angeben das nicht Drop möglich
                                    isAllow = false;
                                }
                            }

                            // Quelle --> One Drive // Ziel --> One Drive
                            else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                            {
                                // Wenn Quelle nicht gleich Ziel
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path != listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                                {
                                    // Quelldaten durchlaufen
                                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                    {
                                        // Wenn Quelldatei ein Ordner
                                        if (listFilesLeft[ddListSourceIndexes[i]].item.Folder != null)
                                        {
                                            // Wenn Quelle gleich Ziel
                                            if (Regex.Replace(listFilesLeft[ddListSourceIndexes[i]].item.ParentReference.Path, "/drive/root:", "") + "/" + listFilesLeft[ddListSourceIndexes[i]].item.Name == listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                                            {
                                                // Angeben das Drop nicht möglich
                                                isAllow = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // Wenn Quelle gleich Ziel
                                else
                                {
                                    // Angeben das nicht Drop möglich
                                    isAllow = false;
                                }
                            }

                            // Quelle --> Dropbox // Ziel --> Dropbox
                            else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                            {
                                // Wenn Quelle und Ziel gleiche Dropbox
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                                {
                                    // Wenn Quelle nicht gleich Ziel
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path != listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                                    {
                                        // Quelldaten durchlaufen
                                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                        {
                                            // Wenn Quelldatei ein Ordner
                                            if (listFilesLeft[ddListSourceIndexes[i]].metadata.IsFolder)
                                            {
                                                // Wenn Quelle gleich Ziel
                                                if (listFilesLeft[ddListSourceIndexes[i]].metadata.PathDisplay == listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                                                {
                                                    // Angeben das Drop nicht möglich
                                                    isAllow = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    // Wenn Quelle gleich Ziel
                                    else
                                    {
                                        // Angeben das nicht Drop möglich
                                        isAllow = false;
                                    }
                                }

                                // Wenn Quelle und Ziel verschiedene Dropboxen
                                else
                                {
                                    isAllow = false;
                                }
                            }

                            // Quelle != Ziel
                            else
                            {
                                // Wenn Ouelle und Ziel verschiedene Clouds
                                if ((listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive") | (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive"))
                                {
                                    // Angeben das Drop möglich
                                    isAllow = false;
                                }
                            }
                        }
                    }

                    // Quelle --> Rechts
                    if (ddSourceSide == "right")
                    {
                        // Quelle-- > Rechts // Ziel --> Item Links
                        if (_ddTargetSide_Entered == "left")
                        {
                            // Quelle --> Lokal // Ziel --> Lokal
                            if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null & listFilesLeft[ddTargetIndex].storageFolder != null)
                            {
                                // Wenn Quelle nicht gleich Ziel
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId != listFilesLeft[ddTargetIndex].storageFolder.FolderRelativeId)
                                {
                                    // Quelldaten durchlaufen
                                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                    {
                                        // Wenn Quelle ein Ordner
                                        if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                                        {
                                            // Wenn Quelle gleich Ziel
                                            if (listFilesRight[ddListSourceIndexes[i]].storageFolder.FolderRelativeId == listFilesLeft[ddTargetIndex].storageFolder.FolderRelativeId)
                                            {
                                                // Angeben das Drop nicht möglich
                                                isAllow = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // Wenn Quelle gleich Ziel
                                else
                                {
                                    // Angeben das Drop nicht möglich
                                    isAllow = false;
                                }
                            }

                            // Quelle --> One Drive // Ziel --> One Drive
                            else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive" & listFilesLeft[ddTargetIndex].driveType == "oneDrive")
                            {
                                // Quelldaten durchlaufen
                                for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                {
                                    // Wenn Quelldatei ein Ordner
                                    if (listFilesRight[ddListSourceIndexes[i]].item.Folder != null)
                                    {
                                        // Wenn Quelle gleich Ziel
                                        if (listFilesRight[ddListSourceIndexes[i]].item.Id == listFilesLeft[ddTargetIndex].item.Id)
                                        {
                                            // Angeben das Drop nicht möglich
                                            isAllow = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            // Quelle --> Dropbox // Ziel --> Dropbox
                            else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFilesLeft[ddTargetIndex].driveType == "dropbox")
                            {
                                // Wenn Quelle und Ziel gleiche Dropbox
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                                {
                                    // Quelldaten durchlaufen
                                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                    {
                                        // Wenn Quelldatei ein Ordner
                                        if (listFilesRight[ddListSourceIndexes[i]].metadata.IsFolder)
                                        {
                                            // Wenn Quelle gleich Ziel
                                            if (listFilesRight[ddListSourceIndexes[i]].metadata.AsFolder.Id == listFilesLeft[ddTargetIndex].metadata.AsFolder.Id)
                                            {
                                                // Angeben das Drop nicht möglich
                                                isAllow = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            // Quelle != Ziel
                            else
                            {
                                // Wenn Ouelle und Ziel verschiedene Clouds
                                if ((listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFilesLeft[ddTargetIndex].driveType == "oneDrive") | (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive" & listFilesLeft[ddTargetIndex].driveType == "dropbox"))
                                {
                                    // Angeben das Drop möglich
                                    isAllow = false;
                                }
                            }
                        }

                        // Quelle-- > Rechts // Ziel --> Item Rechts
                        else if (_ddTargetSide_Entered == "right")
                        {
                            // Quelle --> Lokal // Ziel --> Lokal
                            if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null & listFilesRight[ddTargetIndex].storageFolder != null)
                            {
                                // Quelldaten durchlaufen
                                for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                {
                                    // Wenn Quelldatei ein Ordner
                                    if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                                    {
                                        // Wenn Quelle gleich Ziel
                                        if (listFilesRight[ddListSourceIndexes[i]].storageFolder.FolderRelativeId == listFilesRight[ddTargetIndex].storageFolder.FolderRelativeId)
                                        {
                                            // Angeben das Drop nicht möglich
                                            isAllow = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            // Quelle --> One Drive // Ziel --> One Drive
                            else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive" & listFilesRight[ddTargetIndex].driveType == "oneDrive")
                            {
                                // Quelldaten durchlaufen
                                for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                {
                                    // Wenn Quelldatei ein Ordner
                                    if (listFilesRight[ddListSourceIndexes[i]].item.Folder != null)
                                    {
                                        // Wenn Quelle gleich Ziel
                                        if (listFilesRight[ddListSourceIndexes[i]].item.Id == listFilesRight[ddTargetIndex].item.Id)
                                        {
                                            // Angeben das Drop nicht möglich
                                            isAllow = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            // Quelle --> Dropbox // Ziel --> Dropbox
                            else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFilesRight[ddTargetIndex].driveType == "dropbox")
                            {
                                // Quelldaten durchlaufen
                                for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                {
                                    // Wenn Quelldatei ein Ordner
                                    if (listFilesRight[ddListSourceIndexes[i]].metadata.IsFolder)
                                    {
                                        // Wenn Quelle gleich Ziel
                                        if (listFilesRight[ddListSourceIndexes[i]].metadata.AsFolder.Id == listFilesRight[ddTargetIndex].metadata.AsFolder.Id)
                                        {
                                            // Angeben das Drop nicht möglich
                                            isAllow = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        // Quelle-- > Rechts // Ziel --> Scrollviewer Links
                        else if (_ddTargetSide_Entered == "svLeft")
                        {
                            // Quelle --> Lokal // Ziel --> Lokal
                            if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                            {
                                // Wenn Quelle nicht gleich Ziel
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId != listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId)
                                {
                                    // Quelldaten durchlaufen
                                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                    {
                                        // Wenn Quelle ein Ordner
                                        if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                                        {
                                            // Wenn Quelle gleich Ziel
                                            if (listFilesRight[ddListSourceIndexes[i]].storageFolder.FolderRelativeId == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId)
                                            {
                                                // Angeben das Drop nicht möglich
                                                isAllow = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // Wenn Quelle gleich Ziel
                                else
                                {
                                    // Angeben das nicht Drop möglich
                                    isAllow = false;
                                }
                            }

                            // Quelle --> One Drive // Ziel --> One Drive
                            else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                            {
                                // Wenn Quelle nicht gleich Ziel
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path != listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                                {
                                    // Quelldaten durchlaufen
                                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                    {
                                        // Wenn Quelldatei ein Ordner
                                        if (listFilesRight[ddListSourceIndexes[i]].item.Folder != null)
                                        {
                                            // Wenn Quelle gleich Ziel
                                            if (Regex.Replace(listFilesRight[ddListSourceIndexes[i]].item.ParentReference.Path, "/drive/root:", "") + "/" + listFilesRight[ddListSourceIndexes[i]].item.Name == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path)
                                            {
                                                // Angeben das Drop nicht möglich
                                                isAllow = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                // Wenn Quelle gleich Ziel
                                else
                                {
                                    // Angeben das nicht Drop möglich
                                    isAllow = false;
                                }
                            }

                            // Quelle --> Dropbox // Ziel --> Dropbox
                            else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                            {
                                // Wenn Quelle und Ziel gleiche Dropbox
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                                {
                                    // Wenn Quelle nicht gleich Ziel
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path != listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                                    {
                                        // Quelldaten durchlaufen
                                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                                        {
                                            // Wenn Quelldatei ein Ordner
                                            if (listFilesRight[ddListSourceIndexes[i]].metadata.IsFolder)
                                            {
                                                // Wenn Quelle gleich Ziel
                                                if (listFilesRight[ddListSourceIndexes[i]].metadata.PathDisplay == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path)
                                                {
                                                    // Angeben das Drop nicht möglich
                                                    isAllow = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    // Wenn Quelle gleich Ziel
                                    else
                                    {
                                        // Angeben das nicht Drop möglich
                                        isAllow = false;
                                    }
                                }

                                // Wenn Quelle und Ziel unterschiedliche Dropboxen
                                else
                                {
                                    isAllow = false;
                                }
                            }

                            // Quelle != Ziel
                            else
                            {
                                // Wenn Ouelle und Ziel verschiedene Clouds
                                if ((listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive") | (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive"))
                                {
                                    // Angeben das Drop möglich
                                    isAllow = false;
                                }
                            }
                        }

                        // Quelle --> Rechts // Ziel --> Scrollviewer Rechts
                        else if (_ddTargetSide_Entered == "svRight")
                        {
                            isAllow = false;
                        }
                    }

                    // Wenn Drag and Drop möglich
                    if (isAllow)
                    {
                        dropAllow = true;
                    }

                    // Wenn Drag and Drop nicht möglich
                    else
                    {
                        dropAllow = false;
                    }
                }
            }
        }

        // Parameter // Sonstige Drag and Drop Parameter
        private int ddTargetIndex;
        private string ddSourceSide = null;
        private List<int> ddListSourceIndexes = new List<int>();
        public static double grDdSubFromY;



        // Items // Drag starten
        private void GrList_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            // Variablen zurücksetzen
            ddListSourceIndexes.Clear();

            // Grid aus Sender kopieren
            Grid copy = (Grid)sender;
            // Daten auslesen
            string tag = copy.Tag.ToString();
            string[] spTag = Regex.Split(tag, "~");
            ddSourceSide = spTag[1];
            int sourceIndex = Convert.ToInt32(spTag[0]);

            // Links // Drag and Drop Anzeige erstellen
            if (ddSourceSide == "left")
            {
                // Bei einfacher Auswahl
                if (lbLeft.SelectionMode == SelectionMode.Single)
                {
                    // Wenn Drag and Drop möglich
                    if (listFilesLeft[sourceIndex].canDrag)
                    {
                        // Indexes erstellen
                        ddListSourceIndexes.Add(sourceIndex);
                    }
                }

                // Bei Mehrfacher Auswahl
                else if (lbLeft.SelectionMode == SelectionMode.Multiple)
                {
                    // Gibt an ob Drag and Drop Item ausgewählt
                    bool selected = false;

                    // Indexes erstellen
                    foreach (var item in lbLeft.SelectedItems)
                    {
                        // Index in Liste schreiben
                        ddListSourceIndexes.Add(lbLeft.Items.IndexOf(item));

                        // Prüfen ob Drag and Drop Item ausgewählt
                        if (lbLeft.Items.IndexOf(item) == sourceIndex & !selected)
                        {
                            selected = true;
                        }
                    }

                    // Wenn Drag and Drop Item nicht ausgewählt
                    if (!selected)
                    {
                        // Liste leeren
                        ddListSourceIndexes.Clear();
                    }
                }

                // Wenn Drag and Drop möglich
                if (ddListSourceIndexes.Count > 0)
                {
                    // Variablen
                    int folders = 0;
                    int files = 0;

                    // Auswahl durchlaufen
                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                    {
                        // Bei einem Ordner
                        if (listFilesLeft[ddListSourceIndexes[i]].type == "folder")
                        {
                            folders++;
                        }
                        // Bei einer Datei
                        else
                        {
                            files++;
                        }
                    }

                    // Angeben das Drag and Drop ausgeführt wird // Erstellt die Anzeige
                    dd = true;

                    // Anzeige Text einfügen
                    tbDdData.Text = filesFoldersOutput(files, folders);
                }
            }

            // Rechts // Drag and Drop Anzeige erstellen
            else if (ddSourceSide == "right")
            {
                // Bei einfacher Auswahl
                if (lbRight.SelectionMode == SelectionMode.Single)
                {
                    // Wenn Drag and Drop möglich
                    if (listFilesRight[sourceIndex].canDrag)
                    {
                        // Indexes erstellen
                        ddListSourceIndexes.Add(sourceIndex);
                    }
                }

                // Bei Mehrfacher Auswahl
                else if (lbRight.SelectionMode == SelectionMode.Multiple)
                {
                    // Gibt an ob Drag and Drop Item ausgewählt
                    bool selected = false;

                    // Indexes erstellen
                    foreach (var item in lbRight.SelectedItems)
                    {
                        // Index in Liste schreiben
                        ddListSourceIndexes.Add(lbRight.Items.IndexOf(item));
                        // Prüfen ob Drag and Drop Item ausgewählt
                        if (lbRight.Items.IndexOf(item) == sourceIndex & !selected)
                        {
                            selected = true;
                        }
                    }

                    // Wenn Drag and Drop Item nicht ausgewählt
                    if (!selected)
                    {
                        // Liste leeren
                        ddListSourceIndexes.Clear();
                    }
                }

                // Wenn Drag and Drop möglich
                if (ddListSourceIndexes.Count > 0)
                {
                    // Variablen
                    int folders = 0;
                    int files = 0;

                    // Auswahl durchlaufen
                    for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                    {
                        // Bei einem Ordner
                        if (listFilesRight[ddListSourceIndexes[i]].type == "folder")
                        {
                            folders++;
                        }
                        // Bei einer Datei
                        else
                        {
                            files++;
                        }
                    }

                    // Angeben das Drag and Drop ausgeführt wird // Erstellt die Anzeige
                    dd = true;

                    // Anzeige Text einfügen
                    tbDdData.Text = filesFoldersOutput(files, folders);
                }
            }
        }



        // Items // Drag bewegen
        private void GrList_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            // Drag and Drop Anzeige bewegen
            try
            {
                grDdData.Margin = new Thickness(pointerX - (grDdData.ActualWidth / 2), pointerY - (grDdData.ActualHeight / 2) - grDdSubFromY, 0, 0);
            }
            catch { }
        }

        

        // Items // Drag Beenden
        private void GrList_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            // Drag and Drop anzeige verbergen
            try
            {
                grDdData.Visibility = Visibility.Collapsed;
            }
            catch { }
        }



        // Items // Drag reinfahren
        private void GrList_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop aktiv
            if (dd)
            {
                // Grid aus Sender kopieren
                Grid copy = (Grid)sender;
                // Daten auslesen
                string tag = copy.Tag.ToString();
                string[] spTag = Regex.Split(tag, "~");
                ddTargetIndex = Convert.ToInt32(spTag[0]);

                // Wenn Links
                if (spTag[1] == "left")
                {
                    // Wenn Drop möglich
                    if (listFilesLeft[ddTargetIndex].canDrop)
                    {
                        ddTargetSide_Entered = spTag[1];
                    }
                }

                // Wenn Rechts
                if (spTag[1] == "right")
                {
                    // Wenn Drop möglich
                    if (listFilesRight[ddTargetIndex].canDrop)
                    {
                        ddTargetSide_Entered = spTag[1];
                    }
                }
            }
        }



        // Items // Drag rausfahren
        private void GrList_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop aktiv
            if (dd)
            {
                // Grid aus Sender kopieren
                Grid copy = (Grid)sender;
                // Daten auslesen
                string tag = copy.Tag.ToString();
                string[] spTag = Regex.Split(tag, "~");
                string ddTargetSide = spTag[1];
                ddTargetIndex = Convert.ToInt32(spTag[0]);

                // Wenn links
                if (ddTargetSide == "left")
                {
                    // Hintergrund Item Links ändern
                    listFilesLeft[ddTargetIndex].changeBackground(new SolidColorBrush(Colors.Transparent));

                    // Wenn ScrollViewer links Drag and Drop möglich
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canDrop)
                    {
                        ddTargetSide_Entered = "svLeft";
                    }
                }

                // Wenn rechts
                else
                {
                    // Hintergrund Item Rechts ändern
                    listFilesRight[ddTargetIndex].changeBackground(new SolidColorBrush(Colors.Transparent));

                    // Wenn ScrollViewer rechts Drag and Drop möglich
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canDrop)
                    {
                        ddTargetSide_Entered = "svRight";
                    }
                }

            }
        }



        // Links ScrollViewer // Drag reinfahren
        private void grLeft_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop aktiv
            if (dd)
            {
                // Wenn ScrollViewer links Drag and Drop möglich
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canDrop)
                {
                    // Argumente erstellen
                    ddTargetSide_Entered = "svLeft";
                }
            }
        }



        // Links // ScrollViewer // Drag rausfahren
        private void grLeft_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop aktiv
            if (dd)
            {
                // Wenn Drag and Drop Target Side nicht Item links
                if (ddTargetSide_Entered != "left")
                {
                    dropAllow = false;
                }
            }
        }



        // Links // ScrollViewer // Released
        private void grLeft_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop aktiv
            if (dd)
            {
                // ScrollViewer Hintergrund ändern
                grLeft.Background = new SolidColorBrush(Colors.Transparent);

                // Wenn Drag and Drop möglich
                if (dropAllow)
                {
                    // Aktione Drag and Drop starten
                    acDragAndDrop(ddTargetSide_Entered, -1);
                }

                // Drag and Drop beenden
                dd = false;
            }

            // Wenn Drag and Drop nich aktiv
            else
            {
                // Nur bei Desktop
                if (!IsMobile)
                {
                    // Wenn Maus aktiv
                    if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                    {
                        // Nur bei Rechtsklick
                        var properties = e.GetCurrentPoint(this).Properties;
                        if (properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
                        {
                            // Flyout Menü öffnen
                            GrList_OpenFlyout(sender);
                        }
                    }
                }
            }
        }



        // Rechts // ScrollViewer // Drag reinfahren
        private void grRight_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop aktiv
            if (dd)
            {
                // Wenn ScrollViewer rechts Drag and Drop möglich
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canDrop)
                {
                    // Argumente erstellen
                    ddTargetSide_Entered = "svRight";
                }
            }
        }



        // Rechts // ScrollViewer // Drag rausfahren
        private void grRight_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop aktiv
            if (dd)
            {
                // Wenn Drag and Drop Target Side nicht Item rechts
                if (ddTargetSide_Entered != "right")
                {
                    dropAllow = false;
                }
            }
        }



        // Rechts // ScrollViewer // Drag Released
        private void grRight_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop aktiv
            if (dd)
            {
                // ScrollViewer Hintergrund ändern
                grRight.Background = new SolidColorBrush(Colors.Transparent);

                // Wenn Drag and Drop möglich
                if (dropAllow)
                {
                    // Aktione Drag and Drop starten
                    acDragAndDrop(ddTargetSide_Entered, -1);
                }

                // Drag and Drop beenden
                dd = false;
            }

            // Wenn Drag and Drop nich aktiv
            else
            {
                // Nur bei Desktop
                if (!IsMobile)
                {
                    // Wenn Maus aktiv
                    if (e.Pointer.PointerDeviceType == PointerDeviceType.Mouse)
                    {
                        // Nur bei Rechtsklick
                        var properties = e.GetCurrentPoint(this).Properties;
                        if (properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
                        {
                            // Flyout Menü öffnen
                            GrList_OpenFlyout(sender);
                        }
                    }
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------




        // Drag and Drop Aktionen
        // -----------------------------------------------------------------------
#region Drag and Drop Aktionen

        // Drag and Drop // Aktionen
        private async void acDragAndDrop(string targetSide, int targetIndex)
        {
            // Vorherigen Task abbrechen
            if (cancelTokenAction != null)
            {
                cancelTokenAction.Cancel();
            }
            // Neues Token erstellen
            cancelTokenAction = new CancellationTokenSource();

            // Klasse Kopieren und Verschieben erstellen
            ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders = new ClassCopyOrMoveFilesAndFolders();

            // Angeben das Drag and Drop
            classCopyOrMoveFilesAndFolders.dragAndDrop = true;

            

            // Ziel --> Links Item
            if (targetSide == "left")
            {

                // Quelle --> links
                if (ddSourceSide == "left")
                {
                    // Links --> Links Item // Quelle --> Lokal // Ziel --> Lokal
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFilesLeft[targetIndex].storageFolder != null)
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesLeft[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesLeft[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesLeft[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal -- > Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listFiles, listFolders, listFilesLeft[targetIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn rechtes Fenster Lokal
                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                        {
                            // Wenn rechtes Fenster gleich Ziel
                            if (listFilesLeft[targetIndex].storageFolder.FolderRelativeId == listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId)
                            {
                                // Fenste links erneuern
                                outputRefresh("right");
                            }
                        }

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenste links erneuern
                            outputRefresh("left");
                        }
                    }

                    // Links --> Links Item // Quelle --> One Drive // Ziel --> One Drive
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFilesLeft[targetIndex].driveType == "oneDrive")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren und verschieben // One Drive --> One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listItems, listFilesLeft[targetIndex].item.ParentReference.Path + "/" + listFilesLeft[targetIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn rechtes Fenster One Drive
                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                        {
                            // Wenn rechtes Fenster gleich Ziel
                            if (Regex.Replace(listFilesLeft[targetIndex].item.ParentReference.Path + "/" + listFilesLeft[targetIndex].item.Name, "/drive/root:", "") == listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                            {
                                // Fenster rechts erneuern
                                outputRefresh("right");
                            }
                        }

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("left");
                        }
                    }

                    // Links --> Links Item // Quelle --> Dropbox // Ziel --> Dropbox
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFilesLeft[targetIndex].driveType == "dropbox")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren und verschieben // Dropbox --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listItems, listFilesLeft[targetIndex].metadata.PathDisplay, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn rechtes Fenster Dropbox
                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                        {
                            // Wenn rechtes Fenster gleich Ziel
                            if (listFilesLeft[targetIndex].metadata.PathDisplay == listFolderTreeRight[listFolderTreeRight.Count() - 1].path)
                            {
                                // Fenster rechts erneuern
                                outputRefresh("right");
                            }
                        }

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("left");
                        }
                    }
                }

                // Quelle --> Rechts
                else if (ddSourceSide == "right")
                {
                    // Rechts --> Links Item // Quelle --> Lokal // Ziel --> Lokal
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null & listFilesLeft[targetIndex].storageFolder != null)
                    {
                        // Prüfen ob Daten verschoben werden
                        if (listFilesLeft[targetIndex].driveType == listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType)
                        {
                            classCopyOrMoveFilesAndFolders.move = true;
                        }

                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesRight[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesRight[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesRight[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listFiles, listFolders, listFilesLeft[targetIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }

                    // Rechts --> Links Item // Quelle --> One Drive // Ziel --> One Drive
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive" & listFilesLeft[targetIndex].driveType == "oneDrive")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren und verschieben // One Drive --> One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listItems, listFilesLeft[targetIndex].item.ParentReference.Path + "/" + listFilesLeft[targetIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }

                    // Rechts --> Links Item // Quelle --> Lokal // Ziel --> One Drive
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null & listFilesLeft[targetIndex].driveType == "oneDrive")
                    {
                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesRight[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesRight[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesRight[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToOneDrive(listFiles, listFolders, listFilesLeft[targetIndex].item.ParentReference.Path + "/" + listFilesLeft[targetIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }

                    // Rechts --> Links Item // Quelle --> One Drive // Ziel --> Lokal
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive" & listFilesLeft[targetIndex].storageFolder != null)
                    {
                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren oder Verschieben // One Drive --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToLokal(listItems, listFilesLeft[targetIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }

                    // Rechts --> Links Item // Quelle --> Dropbox // Ziel --> Dropbox
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFilesLeft[targetIndex].driveType == "dropbox")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren und verschieben // Dropbox --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listItems, listFilesLeft[targetIndex].metadata.PathDisplay, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }

                    // Rechts --> Links Item // Quelle --> Lokal // Ziel --> Dropbox
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null & listFilesLeft[targetIndex].driveType == "dropbox")
                    {
                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesRight[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesRight[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesRight[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToDropbox(listFiles, listFolders, listFilesLeft[targetIndex].metadata.PathDisplay, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }

                    // Rechts --> Links Item // Quelle --> Dropbox // Ziel --> Lokal
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFilesLeft[targetIndex].storageFolder != null)
                    {
                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren oder Verschieben // Dropbox --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToLokal(listItems, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, listFilesLeft[targetIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }
                }
            }



            // Ziel --> Rechts Item
            else if (targetSide == "right")
            {

                // Quelle --> Links
                if (ddSourceSide == "left")
                {
                    // Links --> Rechts Item // Quelle --> Lokal // Ziel --> Lokal
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFilesRight[targetIndex].storageFolder != null)
                    {
                        // Prüfen ob Daten verschoben werden
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == listFilesRight[targetIndex].driveType)
                        {
                            classCopyOrMoveFilesAndFolders.move = true;
                        }

                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesLeft[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesLeft[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesLeft[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listFiles, listFolders, listFilesRight[targetIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);


                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("left");
                        }
                    }

                    // Links --> Rechts Item // Quelle --> One Drive // Ziel --> One Drive
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFilesRight[targetIndex].driveType == "oneDrive")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren und verschieben // One Drive // One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listItems, listFilesRight[targetIndex].item.ParentReference.Path + "/" + listFilesRight[targetIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("left");
                        }
                    }

                    // Links --> Rechts Item  // Quelle --> Lokal // Ziel --> One Drive
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFilesRight[targetIndex].driveType == "oneDrive")
                    {
                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesLeft[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesLeft[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesLeft[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und vereschieben // Lokal --> One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToOneDrive(listFiles, listFolders, listFilesRight[targetIndex].item.ParentReference.Path + "/" + listFilesRight[targetIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("left");
                        }
                    }

                    // Links --> Rechts Item // Quelle --> One Drive // Ziel --> Lokal
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFilesRight[targetIndex].storageFolder != null)
                    {
                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren oder Verschieben // One Drive --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToLokal(listItems, listFilesRight[targetIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("left");
                        }
                    }

                    // Links --> Rechts Item // Quelle --> Dropbox // Ziel --> Dropbox
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFilesRight[targetIndex].driveType == "dropbox")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren und verschieben // Dropbox --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listItems, listFilesRight[targetIndex].metadata.PathDisplay, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("left");
                        }
                    }

                    // Links --> Rechts Item // Quelle --> Lokal // Ziel --> Dropbox
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFilesRight[targetIndex].driveType == "dropbox")
                    {
                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesLeft[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesLeft[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesLeft[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToDropbox(listFiles, listFolders, listFilesRight[targetIndex].metadata.PathDisplay, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("left");
                        }
                    }

                    // Links --> Rechts Item // Quelle --> Dropbox // Ziel --> Lokal
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFilesRight[targetIndex].storageFolder != null)
                    {
                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren oder Verschieben // Dropbox --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToLokal(listItems, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, listFilesRight[targetIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("left");
                        }
                    }
                }

                // Quelle --> Rechts
                else if (ddSourceSide == "right")
                {
                    // Rechts --> Rechts Item // Quelle --> Lokal // Ziel --> Lokal
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null & listFilesRight[targetIndex].storageFolder != null)
                    {
                        // Gibt an ob Daten verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesRight[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesRight[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesRight[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listFiles, listFolders, listFilesRight[targetIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn linkes Fenster Lokal
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                        {
                            // Wenn linkes Fenster gleich Ziel
                            if (listFilesRight[targetIndex].storageFolder.FolderRelativeId == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId)
                            {
                                // Fenste links erneuern
                                outputRefresh("left");
                            }
                        }

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }

                    // Rechts --> Rechts Item // Quelle --> One Drive // Ziel --> One Drive
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive" & listFilesRight[targetIndex].driveType == "oneDrive")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren und verschieben // One Drive --> One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listItems, listFilesRight[targetIndex].item.ParentReference.Path + "/" + listFilesRight[targetIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn linkes Fenster One Drive
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                        {
                            // Wenn linkes Fenster gleich Ziel
                            if (Regex.Replace(listFilesRight[targetIndex].item.ParentReference.Path + "/" + listFilesRight[targetIndex].item.Name, "/drive/root:", "") == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path)
                            {
                                // Fenster rechts erneuern
                                outputRefresh("left");
                            }
                        }

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }

                    // Rechts --> Rechts Item // Quelle --> Dropbox // Ziel --> Dropbox
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFilesRight[targetIndex].driveType == "dropbox")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren und verschieben // Dropbox --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listItems, listFilesRight[targetIndex].metadata.PathDisplay, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn linkes Fenster Dropbox
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                        {
                            // Wenn linke Fenster gleich Ziel
                            if (listFilesRight[targetIndex].metadata.PathDisplay == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path)
                            {
                                // Fenster links erneuern
                                outputRefresh("left");
                            }
                        }

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("right");
                        }
                    }
                }
            }



            // Ziel --> Links ScrolViewer
            else if (targetSide == "svLeft")
            {
                // Quelle --> Rechts
                if (ddSourceSide == "right")
                {
                    // Rechts --> Links ScrollViewer // Quelle --> Lokal // Ziel Lokal
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                    {
                        // Prüfen ob Daten verschoben werden
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType)
                        {
                            classCopyOrMoveFilesAndFolders.move = true;
                        }

                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesRight[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesRight[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesRight[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listFiles, listFolders, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("rightOnly");
                        }

                        // Fenster links erneuern
                        outputRefresh("leftOnly");
                    }

                    // Rechts --> Links ScrollViewer // Quelle --> One Drive // Ziel --> One Drive
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren und verschieben // One Drive --> One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listItems, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("rightOnly");
                        }

                        // Fenster links erneuern
                        outputRefresh("leftOnly");
                    }

                    // Rechts --> Links ScrollViewer  // Quelle --> Lokal // Ziel --> One Drive
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive" & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                    {
                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesRight[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesRight[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesRight[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToOneDrive(listFiles, listFolders, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("rightOnly");
                        }

                        // Fenster links erneuern
                        outputRefresh("leftOnly");
                    }

                    // Rechts --> Links ScrollViewer  // Quelle --> One Drive // Ziel --> Lokal
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                    {
                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren oder Verschieben // One Drive --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToLokal(listItems, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("rightOnly");
                        }

                        // Fenster links erneuern
                        outputRefresh("leftOnly");
                    }

                    // Rechts --> Links ScrollViewer // Quelle --> Dropbox // Ziel --> Dropbox
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren und verschieben // Dropbox --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listItems, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("rightOnly");
                        }

                        // Fenster links erneuern
                        outputRefresh("leftOnly");
                    }

                    // Rechts --> Links ScrollViewer  // Quelle --> Lokal // Ziel --> Dropbox
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox" & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                    {
                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesRight[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesRight[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesRight[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesRight[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToDropbox(listFiles, listFolders, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("rightOnly");
                        }

                        // Fenster links erneuern
                        outputRefresh("leftOnly");
                    }

                    // Rechts --> Links ScrollViewer  // Quelle --> Dropbox // Ziel --> Lokal
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                    {
                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesRight[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren oder Verschieben // Dropbox --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToLokal(listItems, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster rechts erneuern
                            outputRefresh("rightOnly");
                        }

                        // Fenster links erneuern
                        outputRefresh("leftOnly");
                    }
                }
            }



            // Ziel --> Rechts ScrollViewer
            else if (targetSide == "svRight")
            {
                // Quelle --> Links
                if (ddSourceSide == "left")
                {
                    // Links --> Rechts ScrollViewer // Quelle --> Lokal // Ziel --> Lokal
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                    {
                        // Prüfen ob Daten verschoben werden
                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType)
                        {
                            classCopyOrMoveFilesAndFolders.move = true;
                        }

                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesLeft[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesLeft[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesLeft[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listFiles, listFolders, listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("leftOnly");
                        }
                        // Fenster rechts erneuern
                        outputRefresh("rightOnly");
                    }

                    // Links --> Rechts ScrollViewer // Quelle --> One Drive // Ziel --> One Drive
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive" & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren verschieben // One Drive --> One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listItems, listFolderTreeRight[listFolderTreeRight.Count() - 1].path, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("leftOnly");
                        }

                        // Fenster rechts erneuern
                        outputRefresh("rightOnly");
                    }

                    // Links --> Rechts ScrollViewer // Quelle --> Lokal // Ziel --> One Drive
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive" & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                    {
                        // Liste der Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesLeft[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesLeft[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesLeft[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> One Drive
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToOneDrive(listFiles, listFolders, listFolderTreeRight[listFolderTreeRight.Count() - 1].path, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("leftOnly");
                        }

                        // Fenster rechts erneuern
                        outputRefresh("rightOnly");
                    }

                    // Links --> Rechts ScrollViewer  // Quelle --> One Drive // Ziel --> Lokal
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                    {
                        // List der Items erstellen
                        List<Item> listItems = new List<Item>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].item);
                        }

                        // Kopieren oder Verschieben // One Drive --> lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToLokal(listItems, listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("leftOnly");
                        }

                        // Fenster rechts erneuern
                        outputRefresh("rightOnly");
                    }

                    // Links --> Rechts ScrollViewer // Quelle --> Dropbox // Ziel --> Dropbox
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                    {
                        // Angeben das Dateien verschoben werden
                        classCopyOrMoveFilesAndFolders.move = true;

                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren und verschieben // Dropbox --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listItems, listFolderTreeRight[listFolderTreeRight.Count() - 1].path, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("leftOnly");
                        }

                        // Fenster rechts erneuern
                        outputRefresh("rightOnly");
                    }

                    // Links --> Rechts ScrollViewer  // Quelle --> Lokal // Ziel --> Dropbox
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox" & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                    {
                        // Liste Dateien und Ordner erstellen
                        List<StorageFile> listFiles = new List<StorageFile>();
                        List<StorageFolder> listFolders = new List<StorageFolder>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            // Wenn Datei
                            if (listFilesLeft[ddListSourceIndexes[i]].storageFile != null)
                            {
                                // Datei hinzufügen
                                listFiles.Add(listFilesLeft[ddListSourceIndexes[i]].storageFile);
                            }
                            // Wenn Ordner
                            else if (listFilesLeft[ddListSourceIndexes[i]].storageFolder != null)
                            {
                                // Ordner hinzufügen
                                listFolders.Add(listFilesLeft[ddListSourceIndexes[i]].storageFolder);
                            }
                        }

                        // Kopieren und verschieben // Lokal --> Dropbox
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToDropbox(listFiles, listFolders, listFolderTreeRight[listFolderTreeRight.Count() - 1].path, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("leftOnly");
                        }

                        // Fenster rechts erneuern
                        outputRefresh("rightOnly");
                    }

                    // Links --> Rechts ScrollViewer  // Quelle --> Dropbox // Ziel --> Lokal
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                    {
                        // List der Items erstellen
                        List<Metadata> listItems = new List<Metadata>();
                        for (int i = 0; i < ddListSourceIndexes.Count(); i++)
                        {
                            listItems.Add(listFilesLeft[ddListSourceIndexes[i]].metadata);
                        }

                        // Kopieren oder Verschieben // Dropbox --> Lokal
                        classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToLokal(listItems, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                        // Wenn verschieben
                        if (classCopyOrMoveFilesAndFolders.move)
                        {
                            // Fenster links erneuern
                            outputRefresh("leftOnly");
                        }

                        // Fenster rechts erneuern
                        outputRefresh("rightOnly");
                    }
                }           
            }
        }
#endregion
        // -----------------------------------------------------------------------
#endregion










#region Flyout Menüs
        // Flyout Menü // Erstellen und öffnen
        // -----------------------------------------------------------------------
#region Flyout Menü // Erstellen und öffnen

        // Parameter und Argumente
        private MenuFlyout menuFlyout = null;
        private string mfSource = null;
        private int mfSourceIndex = -1;



        // Methode // Flyout Menü // ListBox Item // Öffnen // Grid Holding
        private void GrScroll_Holding(object sender, HoldingRoutedEventArgs e)
        {
            GrList_OpenFlyout(sender);
        }



        // Methode // Flyout Menü // ListBox Item // Öffnen
        private void GrList_OpenFlyout(object sender)
        {
            // Wenn Flyout Menü noch nicht offen
            if (menuFlyout == null)
            {
                // Grid aus Sender kopieren
                Grid copy = (Grid)sender;
                // Daten auslesen
                string tag = copy.Tag.ToString();
                string[] spTag = Regex.Split(tag, "~");
                mfSource = spTag[1];
                mfSourceIndex = Convert.ToInt32(spTag[0]);

                // Parameter // Ob Flyout Menü möglich
                bool canFlyout = false;

                // Links // ListBox Item // Wenn Flyout möglich
                if (mfSource == "left")
                {
                    if (listFilesLeft[mfSourceIndex].canFlyout)
                    {
                        canFlyout = true;
                    }
                }

                // Rechts // Wenn Flyout möglich
                else if (mfSource == "right")
                {
                    if (listFilesRight[mfSourceIndex].canFlyout)
                    {
                        canFlyout = true;
                    }
                }

                // Links // ScrollViewer
                else if (mfSource == "svLeft")
                {
                    // Wenn Flyout Menü möglich
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canFlyout)
                    {
                        canFlyout = true;
                    }
                }

                // Rechts // ScrollViewer
                else if (mfSource == "svRight")
                {
                    // Wenn Flyout Menü möglich
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canFlyout)
                    {
                        canFlyout = true;
                    }
                }

                // Flyout Menü ausgeben
                if (canFlyout)
                {
                    // Alpha zum Abdecken einblenden
                    grAlpha.Visibility = Visibility.Visible;

                    // Flyout Menü ausgeben // Methode
                    createAndShowFlyout(sender);
                }
            }
        }



        // Methode // Flyout Menü // ListBox Item // Öffnen // Button Menu Pointer Released
        private void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Grid aus Sender kopieren
            Windows.UI.Xaml.Controls.Image copy = (Windows.UI.Xaml.Controls.Image)sender;
            // Daten auslesen
            string tag = copy.Tag.ToString();
            string[] spTag = Regex.Split(tag, "~");
            mfSource = spTag[1];
            mfSourceIndex = Convert.ToInt32(spTag[0]);

            // Parameter // Ob Flyout Menü möglich
            bool canFlyout = false;

            // Links // ListBox Item // Wenn Flyout möglich
            if (mfSource == "left")
            {
                if (listFilesLeft[mfSourceIndex].canFlyout)
                {
                    canFlyout = true;
                }
            }

            // Rechts // Wenn Flyout möglich
            else if (mfSource == "right")
            {
                if (listFilesRight[mfSourceIndex].canFlyout)
                {
                    canFlyout = true;
                }
            }

            // Links // ScrollViewer
            else if (mfSource == "svLeft")
            {
                // Wenn Flyout Menü möglich
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canFlyout)
                {
                    canFlyout = true;
                }
            }

            // Rechts // ScrollViewer
            else if (mfSource == "svRight")
            {
                // Wenn Flyout Menü möglich
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canFlyout)
                {
                    canFlyout = true;
                }
            }

            // Flyout Menü ausgeben
            if (canFlyout)
            {
                // Alpha zum Abdecken einblenden
                grAlpha.Visibility = Visibility.Visible;

                // Flyout Menü ausgeben // Methode
                createAndShowFlyout(sender);
            }
        }



        // Methode // Flyout Menü ausgeben // ListBox Items // ListBox
        private void createAndShowFlyout(object sender)
        {
            // Parameter und Argumente
            bool canCopy = false;
            bool canCut = false;
            bool canPaste = false;
            bool canDelete = false;
            bool canShare = false;
            bool canRename = false;
            bool canMultiSelect = false;
            bool canRemove = false;
            bool canSignOut = false;
            bool canShareLink = false;
            bool canPinToStart = false;

            // Flyout Menü erstellen
            menuFlyout = new MenuFlyout();
            menuFlyout.Closed += MenuFlyout_Closed;

            // Links // ListBox Item // Daten auslesen
            if (mfSource == "left")
            {
                // Links // Wenn kopieren möglich
                canCopy = listFilesLeft[mfSourceIndex].canCopy;

                // Links // Wenn ausschneiden möglich
                canCut = listFilesLeft[mfSourceIndex].canCut;

                // Links // Wenn einfügen möglich
                if (listFilesLeft[mfSourceIndex].canPaste & paste)
                {
                    canPaste = true;
                }

                // Links // Wenn löschen möglich
                canDelete = listFilesLeft[mfSourceIndex].canDelete;

                // Links // Wenn Teilen möglich
                if (listFilesLeft[mfSourceIndex].canShare)
                {
                    // Wenn One Drive
                    if (listFilesLeft[mfSourceIndex].driveType == "oneDrive")
                    {
                        canShareLink = true;
                    }

                    // Wenn Dropbox
                    if (listFilesLeft[mfSourceIndex].driveType == "dropbox")
                    {
                    }

                    // Wenn nicht One Drive und nicht Dropbox
                    else
                    {
                        canShare = true;
                    }
                }

                // Links // Wenn umbenennen möglich
                if (listFilesLeft[mfSourceIndex].canRename)
                {
                    canRename = true;
                }

                // Links // Wenn alles auswählen möglich
                if (lbLeft.SelectionMode == SelectionMode.Multiple)
                {
                    canMultiSelect = true;
                }

                // Links // Wenn an Start anheften möglich
                if (listFilesLeft[mfSourceIndex].canPinToStart)
                {
                    canPinToStart = true;
                }

                // Links // Wenn entfernen möglich
                if (listFilesLeft[mfSourceIndex].canRemove)
                {
                    canRemove = true;
                }

                // Links // Wenn ausloggen möglich
                if (listFilesLeft[mfSourceIndex].canSignOut)
                {
                    canSignOut = true;
                }
            }

            // Rechts // ListBox Item // Daten auslesen
            else if (mfSource == "right")
            {
                // Rechts // Wenn kopieren möglich
                canCopy = listFilesRight[mfSourceIndex].canCopy;

                // Rechts // Wenn ausschneiden möglich
                canCut = listFilesRight[mfSourceIndex].canCut;

                // Rechts // Wenn einfügen möglich
                if (listFilesRight[mfSourceIndex].canPaste & paste)
                {
                    canPaste = true;
                }

                // Rechts // Wenn löschen möglich
                canDelete = listFilesRight[mfSourceIndex].canDelete;

                // Rechts // Wenn Teilen möglich
                if (listFilesRight[mfSourceIndex].canShare)
                {
                    // Wenn One Drive
                    if (listFilesRight[mfSourceIndex].driveType == "oneDrive")
                    {
                        canShareLink = true;
                    }

                    // Wenn nicht One Drive
                    else
                    {
                        canShare = true;
                    }
                }

                // Rechts // Wenn umbenennen möglich
                if (listFilesRight[mfSourceIndex].canRename)
                {
                    canRename = true;
                }

                // Rechts // Wenn alles auswählen möglich
                if (lbRight.SelectionMode == SelectionMode.Multiple)
                {
                    canMultiSelect = true;
                }

                // Rechts // Wenn an Start anheften möglich
                if (listFilesRight[mfSourceIndex].canPinToStart)
                {
                    canPinToStart = true;
                }

                // Rechts // Wenn entfernen möglich
                if (listFilesRight[mfSourceIndex].canRemove)
                {
                    canRemove = true;
                }

                // Rechts // Wenn ausloggen möglich
                if (listFilesRight[mfSourceIndex].canSignOut)
                {
                    canSignOut = true;
                }
            }

            // Links // ScrollViewer // Daten auslesen
            else if (mfSource == "svLeft")
            {
                // Links // Wenn einfügen möglich
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canPaste)
                {
                    canPaste = true;
                }

                // Links // Wenn alles auswählen möglich
                if (lbLeft.SelectionMode == SelectionMode.Multiple)
                {
                    canMultiSelect = true;
                }
            }

            // Rechts // ScrollViewer // Daten auslesen
            else if (mfSource == "svRight")
            {
                // Rechts // Wenn einfügen möglich
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canPaste)
                {
                    canPaste = true;
                }

                // Rechts // Wenn alles auswählen möglich
                if (lbRight.SelectionMode == SelectionMode.Multiple)
                {
                    canMultiSelect = true;
                }
            }

            // Item kopieren erstellen
            if (canCopy)
            {
                // Eintrag erstellen // Kopieren
                MenuFlyoutItem mfiCopy = new MenuFlyoutItem();
                mfiCopy.Text = resource.GetString("001_Kopieren");
                mfiCopy.Margin = new Thickness(0, -6, 0, -6);
                mfiCopy.Click += MfiCopy_Click;
                menuFlyout.Items.Add(mfiCopy);
            }

            // Item ausschneiden erstellen
            if (canCut)
            {
                // Eintrage erstellen // Ausschneiden
                MenuFlyoutItem mfiCut = new MenuFlyoutItem();
                mfiCut.Text = resource.GetString("001_Ausschneiden");
                mfiCut.Margin = new Thickness(0, -6, 0, -6);
                mfiCut.Click += MfiCut_Click;
                menuFlyout.Items.Add(mfiCut);
            }

            // Item einfügen erstellen
            if (canPaste)
            {
                // Eintrag erstellen // Einfügen
                MenuFlyoutItem mfiPaste = new MenuFlyoutItem();
                mfiPaste.Text = resource.GetString("001_Einfügen");
                mfiPaste.Margin = new Thickness(0, -6, 0, -6);
                mfiPaste.Click += MfiPaste_Click;
                menuFlyout.Items.Add(mfiPaste);
            }

            // Item löschen erstellen
            if (canDelete)
            {
                // Wenn vorherige Einträge schon vorhanden
                if (menuFlyout.Items.Count() > 0)
                {
                    // Trennlinie einfügen
                    MenuFlyoutSeparator mfiSeperator = new MenuFlyoutSeparator();
                    mfiSeperator.Margin = new Thickness(0, -3, 0, -3);
                    menuFlyout.Items.Add(mfiSeperator);
                }

                // Eintrag erstellen // Löschen
                MenuFlyoutItem mfiDelete = new MenuFlyoutItem();
                mfiDelete.Text = resource.GetString("001_Löschen");
                mfiDelete.Margin = new Thickness(0, -6, 0, -6);
                mfiDelete.Click += MfiDelete_Click;
                menuFlyout.Items.Add(mfiDelete);
            }

            // Item share erstellen
            if (canShare | canShareLink | canRename)
            {
                // Wenn vorherige Einträge schon vorhanden
                if (menuFlyout.Items.Count() > 0)
                {
                    // Trennlinie einfügen
                    MenuFlyoutSeparator mfiSeperator = new MenuFlyoutSeparator();
                    mfiSeperator.Margin = new Thickness(0, -3, 0, -3);
                    menuFlyout.Items.Add(mfiSeperator);
                }

                // Item Teilen erstellen
                if (canShare)
                {
                    // Eintrag erstellen // Teilen
                    MenuFlyoutItem mfiShare = new MenuFlyoutItem();
                    mfiShare.Text = resource.GetString("001_Teilen");
                    mfiShare.Margin = new Thickness(0, -6, 0, -6);
                    mfiShare.Click += MfiShare_Click;
                    menuFlyout.Items.Add(mfiShare);
                }

                // Item Freigabelink erstellen
                if (canShareLink)
                {
                    // Eintrag erstellen // Teilen
                    MenuFlyoutItem mfiShareLink = new MenuFlyoutItem();
                    mfiShareLink.Text = resource.GetString("001_FreigabeLink");
                    mfiShareLink.Margin = new Thickness(0, -6, 0, -6);
                    mfiShareLink.Click += MfiShareLink_Click;
                    menuFlyout.Items.Add(mfiShareLink);
                }

                // Item Umbennenen erstellen
                if (canRename)
                {
                    // Eintrag erstellen // Umbenennen
                    MenuFlyoutItem mfiRename = new MenuFlyoutItem();
                    mfiRename.Text = resource.GetString("001_Umbenennen");
                    mfiRename.Margin = new Thickness(0, -6, 0, -6);
                    mfiRename.Click += MfiRename_Click;
                    menuFlyout.Items.Add(mfiRename);
                }
            }

            // Alles auswählen // Alles abwählen
            if (canMultiSelect)
            {
                // Wenn vorherige Einträge schon vorhanden
                if (menuFlyout.Items.Count() > 0)
                {
                    // Trennlinie einfügen
                    MenuFlyoutSeparator mfiSeperator = new MenuFlyoutSeparator();
                    mfiSeperator.Margin = new Thickness(0, -3, 0, -3);
                    menuFlyout.Items.Add(mfiSeperator);
                }

                // Eintrag erstellen // Alles auswählen
                MenuFlyoutItem mfiSelectAll = new MenuFlyoutItem();
                mfiSelectAll.Text = resource.GetString("001_AllesAuswählen");
                mfiSelectAll.Margin = new Thickness(0, -6, 0, -6);
                mfiSelectAll.Click += MfiSelectAll_Click;
                menuFlyout.Items.Add(mfiSelectAll);

                // Eintrag erstellen // Alles abwählen
                MenuFlyoutItem mfiUnselectAll = new MenuFlyoutItem();
                mfiUnselectAll.Text = resource.GetString("001_AllesAbwählen");
                mfiUnselectAll.Margin = new Thickness(0, -6, 0, -6);
                mfiUnselectAll.Click += MfiUnselectAll_Click;
                menuFlyout.Items.Add(mfiUnselectAll);
            }

            // An Start anheften
            if (canPinToStart)
            {
                // Wenn vorherige Einträge schon vorhanden
                if (menuFlyout.Items.Count() > 0)
                {
                    // Trennlinie einfügen
                    MenuFlyoutSeparator mfiSeperator = new MenuFlyoutSeparator();
                    mfiSeperator.Margin = new Thickness(0, -3, 0, -3);
                    menuFlyout.Items.Add(mfiSeperator);
                }

                // Eintrag erstellen // An Start anheften
                if (canPinToStart)
                {
                    MenuFlyoutItem mfiPinToStart = new MenuFlyoutItem();
                    mfiPinToStart.Text = resource.GetString("001_AnStart");
                    mfiPinToStart.Margin = new Thickness(0, -6, 0, -6);
                    mfiPinToStart.Click += MfiPinToStart_Click;
                    menuFlyout.Items.Add(mfiPinToStart);
                }
            }

            // Abmelden // Enfernen
            if (canRemove | canSignOut)
            {
                // Wenn vorherige Einträge schon vorhanden
                if (menuFlyout.Items.Count() > 0)
                {
                    // Trennlinie einfügen
                    MenuFlyoutSeparator mfiSeperator = new MenuFlyoutSeparator();
                    mfiSeperator.Margin = new Thickness(0, -3, 0, -3);
                    menuFlyout.Items.Add(mfiSeperator);
                }

                // Eintrag erstellen // Abmelden
                if (canSignOut)
                {
                    MenuFlyoutItem mfiRemove = new MenuFlyoutItem();
                    mfiRemove.Text = resource.GetString("001_Abmelden");
                    mfiRemove.Margin = new Thickness(0, -6, 0, -6);
                    mfiRemove.Click += MfiSignOut_Click;
                    menuFlyout.Items.Add(mfiRemove);
                }

                // Eintrag erstellen // Entfernen
                if (canRemove)
                {
                    MenuFlyoutItem mfiRemove = new MenuFlyoutItem();
                    mfiRemove.Text = resource.GetString("001_Entfernen");
                    mfiRemove.Margin = new Thickness(0, -6, 0, -6);
                    mfiRemove.Click += MfiRemove_Click;
                    menuFlyout.Items.Add(mfiRemove);
                }
            }

            // Flyout Menü ausgeben
            if (menuFlyout.Items.Count() > 0)
            {
                menuFlyout.ShowAt((FrameworkElement)sender);
            }
        }



        // Methode // Flyout Menü // Schließen
        private void MenuFlyout_Closed(object sender, object e)
        {
            // Menü FlyOut löschen
            menuFlyout = null;

            // Alpha zum Abdecken ausblenden
            grAlpha.Visibility = Visibility.Collapsed;

            // Verlorene Flyout Daten wiederherstellen
            grLeft.Holding += GrScroll_Holding;
            grLeft.Tag = "0~svLeft";
            grRight.Holding += GrScroll_Holding;
            grRight.Tag = "0~svRight";
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button kopieren // Button Ausschneiden
        // -----------------------------------------------------------------------
#region Button kopieren // Button auschneiden
        // Parameter und Argumente // Kopieren und Ausschneiden
        private bool paste = false;
        private bool cut = false;
        private string cutSourceId = null;
        private string cutPath = null;
        private string cutToken = null;
        private List<StorageFolder> listPasteFoldersLocal = new List<StorageFolder>();
        private List<StorageFile> listPasteFilesLocal = new List<StorageFile>();
        private List<Item> listPasteItemsOneDrive = new List<Item>();
        private List<Metadata> listPasteItemsDropbox = new List<Metadata>();



        // Methode // Button Kopieren
        private void MfiCopy_Click(object sender, RoutedEventArgs e)
        {
            // Kopieren und ausschneiden // Methode
            MfiCopyAndCut(false);
        }



        // Methode // Button ausschneiden
        private void MfiCut_Click(object sender, RoutedEventArgs e)
        {
            // Kopien und ausschneiden // Methode
            MfiCopyAndCut(true);
        }



        // Methode // Kopieren und Ausschneiden
        private async void MfiCopyAndCut(bool cut)
        {
            // Argumente der Parameter von den Kopien zurücksetzen
            paste = false;
            this.cut = cut;
            cutSourceId = null;
            cutPath = null;
            cutToken = null;
            listPasteFoldersLocal.Clear();
            listPasteFilesLocal.Clear();
            listPasteItemsOneDrive.Clear();
            listPasteItemsDropbox.Clear();

            // Links
            if (mfSource == "left")
            {
                // Lokal
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                {
                    // Ordner id Festlegen
                    cutSourceId = listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId;

                    // Kopieren und ausschneiden lokal // Task
                    mfiCopyAndCutLocal(lbLeft, listFilesLeft);
                }

                // One Drive
                else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                {
                    // Ordner Pfad Festlegen
                    cutPath = listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path;

                    // Kopieren und ausschneiden One Drive // Task
                    mfiCopyAndCutOneDrive(lbLeft, listFilesLeft);
                }

                // Dropbox
                else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                {
                    // Ordner Pfad Festlegen
                    cutPath = listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path;

                    // Token Festlegen
                    cutToken = listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token;

                    // Kopieren und ausschneiden One Drive // Task
                    mfiCopyAndCutDropbox(lbLeft, listFilesLeft);
                }
            }

            // Rechts
            if (mfSource == "right")
            {
                // Lokal
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                {
                    // Ordner id Festlegen
                    cutSourceId = listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId;

                    // Kopieren und Ausschneiden lokal // Task
                    mfiCopyAndCutLocal(lbRight, listFilesRight);
                }

                // One Drive
                else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                {
                    // Ordner Path Festlegen
                    cutPath = listFolderTreeRight[listFolderTreeRight.Count() - 1].path;

                    // Kopieren und ausschneiden One Drive // Task
                    mfiCopyAndCutOneDrive(lbRight, listFilesRight);
                }

                // Dropbox
                else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                {
                    // Ordner Path Festlegen
                    cutPath = listFolderTreeRight[listFolderTreeRight.Count() - 1].path;

                    // Token Festlegen
                    cutToken = listFolderTreeRight[listFolderTreeRight.Count() - 1].token;

                    // Kopieren und ausschneiden One Drive // Task
                    mfiCopyAndCutDropbox(lbRight, listFilesRight);
                }
            }
        }
#endregion



#region Kopieren und ausschneiden // Lokal
        // Methode // Kopieren und Ausschneiden // lokal
        private void mfiCopyAndCutLocal(ListBox listBox, ObservableCollection<ClassFiles> listFiles)
        {
            // Paramete Liste der Auswahl erstellen
            List<int> indexes = new List<int>();

            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Index hinzufüen
                indexes.Add(mfSourceIndex);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;

                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Index in Liste schreiben
                    indexes.Add(listBox.Items.IndexOf(item));

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }

                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Item der Liste hinzufügen
                    indexes.Clear();
                    indexes.Add(mfSourceIndex);
                }
            }

            // Wenn Dateien vorhanden
            if (indexes.Count() > 0)
            {
                // Parameter paste setzen
                paste = true;

                // Argumente in Parameter der lokalen Kopien schreiben
                for (int i = 0; i < indexes.Count; i++)
                {
                    // Wenn Ordner
                    if (listFiles[indexes[i]].type == "folder")
                    {
                        listPasteFoldersLocal.Add(listFiles[indexes[i]].storageFolder);
                    }

                    // Wenn Datei
                    else if (listFiles[indexes[i]].type == "file")
                    {
                        listPasteFilesLocal.Add(listFiles[indexes[i]].storageFile);
                    }
                }
            }

            // Wenn keine Dateien vorhanden
            else
            {
                // Parameter paste setzen
                paste = false;
            }
        }
#endregion



#region Kopieren und ausschneiden // One Drive
        // Mehtode // Kopieren und Ausschneiden One Drive
        private void mfiCopyAndCutOneDrive(ListBox listBox, ObservableCollection<ClassFiles> listFiles)
        {
            // Paramete Liste der Auswahl erstellen
            List<int> indexes = new List<int>();

            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Index hinzufüen
                indexes.Add(mfSourceIndex);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;

                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Index in Liste schreiben
                    indexes.Add(listBox.Items.IndexOf(item));

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }

                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Item der Liste hinzufügen
                    indexes.Clear();
                    indexes.Add(mfSourceIndex);
                }
            }

            // Wenn Dateien vorhanden
            if (indexes.Count() > 0)
            {
                // Parameter paste setzen
                paste = true;

                // Argumente in Parameter der lokalen Kopien schreiben
                for (int i = 0; i < indexes.Count; i++)
                {
                    // Items in Liste schreiben
                    listPasteItemsOneDrive.Add(listFiles[indexes[i]].item);
                }
            }

            // Wenn keine Dateien vorhanden
            else
            {
                // Parameter paste setzen
                paste = false;
            }
        }
#endregion



#region Kopieren und ausschneiden // Dropbox
        // Mehtode // Kopieren und Ausschneiden One Drive
        private void mfiCopyAndCutDropbox(ListBox listBox, ObservableCollection<ClassFiles> listFiles)
        {
            // Paramete Liste der Auswahl erstellen
            List<int> indexes = new List<int>();

            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Index hinzufüen
                indexes.Add(mfSourceIndex);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;

                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Index in Liste schreiben
                    indexes.Add(listBox.Items.IndexOf(item));

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }

                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Item der Liste hinzufügen
                    indexes.Clear();
                    indexes.Add(mfSourceIndex);
                }
            }

            // Wenn Dateien vorhanden
            if (indexes.Count() > 0)
            {
                // Parameter paste setzen
                paste = true;

                // Argumente in Parameter der lokalen Kopien schreiben
                for (int i = 0; i < indexes.Count; i++)
                {
                    // Items in Liste schreiben
                    listPasteItemsDropbox.Add(listFiles[indexes[i]].metadata);
                }
            }

            // Wenn keine Dateien vorhanden
            else
            {
                // Parameter paste setzen
                paste = false;
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button einfügen
        // -----------------------------------------------------------------------
#region Button einfügen
        // Methode // Button einfügen
        private async void MfiPaste_Click(object sender, RoutedEventArgs e)
        {
            // Wenn Kopie vorhanden
            if (paste)
            {
                // Gibt an welches Fenster erneuert wird
                bool rLeft = false;
                bool rRight = false;

                // Klasse zum Einfügen erstellen
                ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders = new ClassCopyOrMoveFilesAndFolders();

                // Wenn ausschneiden
                if (cut)
                {
                    classCopyOrMoveFilesAndFolders.move = true;
                }

                // Vorherigen Task abbrechen
                if (cancelTokenAction != null)
                {
                    cancelTokenAction.Cancel();
                }
                // Neues Token erstellen
                cancelTokenAction = new CancellationTokenSource();



                // Ziel --> Links Item
                if (mfSource == "left")
                {
                    // Wenn möglich
                    if (listFilesLeft[mfSourceIndex].canPaste)
                    {
                        // Links Item // Ziel --> Lokal
                        if (listFilesLeft[mfSourceIndex].storageFolder != null)
                        {
                            // Links Item // Quelle --> Lokal // Ziel --> Lokal
                            if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                            {
                                // Kopieren und verschieben // Lokal --> Lokal
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listPasteFilesLocal, listPasteFoldersLocal, listFilesLeft[mfSourceIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn rechts lokal
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                                {
                                    // Wenn rechts gleich Zielordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == listFilesLeft[mfSourceIndex].storageFolder.FolderRelativeId)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }

                                // Wenn ausschneiden
                                if (classCopyOrMoveFilesAndFolders.move)
                                {
                                    // Wenn links lokal
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                                    {
                                        // Wenn links gleich Quellordner
                                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                        {
                                            // Fenster links erneuern
                                            rLeft = true;
                                        }
                                    }

                                    // Wenn rechts lokal
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                                    {
                                        // Wenn rechts gleich Quellordner
                                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                        {
                                            // Fenster rechts erneuern
                                            rRight = true;
                                        }
                                    }
                                }
                            }

                            // Links Item // Quelle --> One Drive // Ziel --> Lokal
                            else if (listPasteItemsOneDrive.Count() > 0)
                            {
                                // Kopieren und verschieben // One Drive --> Lokal
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToLokal(listPasteItemsOneDrive, listFilesLeft[mfSourceIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn rechts lokal
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                                {
                                    // Wenn rechts gleich Zielordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == listFilesLeft[mfSourceIndex].storageFolder.FolderRelativeId)
                                    {
                                        // Fenster rechts
                                        rRight = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn rechts One Drive
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                                {
                                    // Wenn rechts gleich Quellordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == cutPath)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }
                            }

                            // Links Item // Quelle --> Dropbox // Ziel --> Lokal
                            else if (listPasteItemsDropbox.Count() > 0)
                            {
                                // Kopieren und verschieben // Dropbox --> Lokal
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToLokal(listPasteItemsDropbox, cutToken, listFilesLeft[mfSourceIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn rechts lokal
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                                {
                                    // Wenn rechts gleich Zielordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == listFilesLeft[mfSourceIndex].storageFolder.FolderRelativeId)
                                    {
                                        // Fenster rechts
                                        rRight = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn rechts Dropbox
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                                {
                                    // cutToken gleich Token rechts
                                    if (cutToken == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                                    {
                                        // Wenn rechts gleich Qellordner
                                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == cutPath)
                                        {
                                            // Fenster rechts erneuern
                                            rRight = true;
                                        }
                                    }
                                }
                            }
                        }

                        // Links Item // Ziel --> One Drive
                        else if (listFilesLeft[mfSourceIndex].driveType == "oneDrive")
                        {
                            // Links Item // Quelle --> Lokal // Ziel --> One Drive
                            if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                            {
                                // Kopieren und verschieben // Lokal --> One Drive
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToOneDrive(listPasteFilesLocal, listPasteFoldersLocal, listFilesLeft[mfSourceIndex].item.ParentReference.Path + "/" + listFilesLeft[mfSourceIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn rechts One Drive
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                                {
                                    // Wenn rechts gleich Zielordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == listFilesLeft[mfSourceIndex].item.ParentReference.Path)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn rechts lokal
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                                {
                                    // Wenn rechts gleich Quellordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }
                            }

                            // Links Item // Quelle --> One Drive // Ziel --> One Drive
                            else if (listPasteItemsOneDrive.Count() > 0)
                            {
                                // Kopieren und verschieben // One Drive --> One Drive
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listPasteItemsOneDrive, listFilesLeft[mfSourceIndex].item.ParentReference.Path + "/" + listFilesLeft[mfSourceIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn rechts One Drive
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                                {
                                    // Wenn rechts gleich Zielordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == listFilesLeft[mfSourceIndex].item.ParentReference.Path)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn rechts One Drive
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                                {
                                    // Wenn rechts gleich Quellordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == cutPath)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }
                            }

                            // Links Item // Quelle --> Andere Cloud // Ziel --> One Drive 
                            else
                            {
                                // Nachricht ausgeben // Nicht möglich
                                DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"), resource.GetString("001_KopierenVerschiebenOnlineOnline"));
                                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                                dBSet.AddButton(dBtn);
                                dEx.AddButtonSet(dBSet);
                                await dEx.ShowAsync(grMain);
                            }
                        }

                        // Links Item // Ziel --> Dropbox
                        else if (listFilesLeft[mfSourceIndex].driveType == "dropbox")
                        {
                            // Links Item // Quelle --> Lokal // Ziel --> Dropbox
                            if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                            {
                                // Kopieren und verschieben // Lokal --> Dropbox
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToDropbox(listPasteFilesLocal, listPasteFoldersLocal, listFilesLeft[mfSourceIndex].metadata.PathDisplay, cutToken, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn rechts Dropbox
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                                {
                                    // Wenn Token links gleich Token rechts
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].token == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token)
                                    {
                                        // Wenn rechts gleich Zielordner
                                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == listFilesLeft[mfSourceIndex].metadata.PathDisplay)
                                        {
                                            // Fenster rechts erneuern
                                            rRight = true;
                                        }
                                    }
                                }

                                // Wenn ausschneiden // Wenn rechts lokal
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                                {
                                    // Wenn rechts gleich Quellordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }
                            }

                            // Links Item // Quelle --> Dropbox // Ziel --> Dropbox
                            else if (listPasteItemsDropbox.Count() > 0 & cutToken == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token)
                            {
                                // Kopieren und verschieben // Dropbox --> Dropbox
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listPasteItemsDropbox, listFilesLeft[mfSourceIndex].metadata.PathDisplay, cutToken, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn rechts Dropbox
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                                {
                                    // Wenn rechts gleich Zielordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == listFilesLeft[mfSourceIndex].metadata.PathDisplay)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn rechts Dropbox
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                                {
                                    // Wenn cutToken gleich Token rechts
                                    if (cutToken == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                                    {
                                        // Wenn rechts gleich Quellordner
                                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == cutPath)
                                        {
                                            // Fenster rechts erneuern
                                            rRight = true;
                                        }
                                    }
                                }
                            }

                            // Links Item // Quelle --> Andere Cloud // Ziel --> Dropbox
                            else
                            {
                                // Nachricht ausgeben // Nicht möglich
                                DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"), resource.GetString("001_KopierenVerschiebenOnlineOnline"));
                                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                                dBSet.AddButton(dBtn);
                                dEx.AddButtonSet(dBSet);
                                await dEx.ShowAsync(grMain);
                            }
                        }
                    }
                }

                // Ziel --> Rechts Item
                else if (mfSource == "right")
                {
                    // Wenn möglich 
                    if (listFilesRight[mfSourceIndex].canPaste)
                    {
                        // Rechts Item // Ziel --> Lokal
                        if (listFilesRight[mfSourceIndex].storageFolder != null)
                        {
                            // Rechts Item // Quelle --> Lokal // Ziel --> Lokal
                            if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                            {
                                // Kopieren verschieben // Lokal --> Lokal
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listPasteFilesLocal, listPasteFoldersLocal, listFilesRight[mfSourceIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn links lokal
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                                {
                                    // Wenn links gleich Zielordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == listFilesRight[mfSourceIndex].storageFolder.FolderRelativeId)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }

                                // Wenn ausschneiden
                                if (classCopyOrMoveFilesAndFolders.move)
                                {
                                    // Wenn links lokal
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                                    {
                                        // Wenn links gleich Quellordner
                                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                        {
                                            // Fenster links erneuern
                                            rLeft = true;
                                        }
                                    }
                                    // Wenn rechts lokal
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                                    {
                                        // Wenn rechts gleich Quellordner
                                        if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                        {
                                            // Fenster rechts erneuern
                                            rRight = true;
                                        }
                                    }
                                }
                            }

                            // Rechts Item // Quelle --> One Drive // Ziel --> Lokal
                            else if (listPasteItemsOneDrive.Count() > 0)
                            {
                                // Kopieren und Verschieben // One Drive --> Lokal
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToLokal(listPasteItemsOneDrive, listFilesRight[mfSourceIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn links lokal
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                                {
                                    // Wenn links gleich Ziel Ordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == listFilesRight[mfSourceIndex].storageFolder.FolderRelativeId)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn links One Drive
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                                {
                                    // Wenn links gleich Quellordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == cutPath)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }
                            }

                            // Rechts Item // Quelle --> Dropbox // Ziel --> Lokal
                            else if (listPasteItemsDropbox.Count() > 0)
                            {
                                // Kopieren und verschieben // Dropbox --> Lokal
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToLokal(listPasteItemsDropbox, cutToken, listFilesRight[mfSourceIndex].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn links lokal
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                                {
                                    // Wenn links gleich Zielordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == listFilesRight[mfSourceIndex].storageFolder.FolderRelativeId)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn links Dropbox
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                                {
                                    // cutToken gleich Token links
                                    if (cutToken == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token)
                                    {
                                        // Wenn links gleich Qellordner
                                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == cutPath)
                                        {
                                            // Fenster links erneuern
                                            rLeft = true;
                                        }
                                    }
                                }
                            }
                        }

                        // Rechts Item // Ziel --> One Drive
                        else if (listFilesRight[mfSourceIndex].driveType == "oneDrive")
                        {
                            // Rechts Item // Quelle --> Lokal // Ziel --> One Drive
                            if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                            {
                                // Kopieren und verschieben // Lokal --> One Drive
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToOneDrive(listPasteFilesLocal, listPasteFoldersLocal, listFilesRight[mfSourceIndex].item.ParentReference.Path + "/" + listFilesRight[mfSourceIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn links One Drive
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                                {
                                    // Wenn links gleich Ziel Ordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == listFilesRight[mfSourceIndex].item.ParentReference.Path)
                                    {
                                        // Fenster rechtes erneuern
                                        rLeft = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn links lokal
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                                {
                                    // Wenn rechts links Quellordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }
                            }

                            // Rechts Item // Quelle --> One Drive // Ziel --> One Drive
                            else if (listPasteItemsOneDrive.Count() > 0)
                            {
                                // Kopieren und verschieben // One Drive --> One Drive
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listPasteItemsOneDrive, listFilesRight[mfSourceIndex].item.ParentReference.Path + "/" + listFilesRight[mfSourceIndex].item.Name, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn links One Drive
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                                {
                                    // Wenn links gleich Ziel Ordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == listFilesRight[mfSourceIndex].item.ParentReference.Path)
                                    {
                                        // Fenster rechtes erneuern
                                        rLeft = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn links One Drive
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                                {
                                    // Wenn links Quellordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == cutPath)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }
                            }

                            // Rechts Item // Quelle --> Andere Cloud // Ziel --> One Drive 
                            else
                            {
                                // Nachricht ausgeben // Nicht möglich
                                DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"), resource.GetString("001_KopierenVerschiebenOnlineOnline"));
                                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                                dBSet.AddButton(dBtn);
                                dEx.AddButtonSet(dBSet);
                                await dEx.ShowAsync(grMain);
                            }
                        }

                        // Rechts Item // Ziel --> Dropbox
                        else if (listFilesRight[mfSourceIndex].driveType == "dropbox")
                        {
                            // Rechts Item // Quelle --> Lokal // Ziel --> Dropbox
                            if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                            {
                                // Kopieren und verschieben // Lokal --> Dropbox
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToDropbox(listPasteFilesLocal, listPasteFoldersLocal, listFilesRight[mfSourceIndex].metadata.PathDisplay, cutToken, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn links Dropbox
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                                {
                                    // Wenn Token links gleich Token rechts
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].token == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token)
                                    {
                                        // Wenn links gleich Zielordner
                                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == listFilesRight[mfSourceIndex].metadata.PathDisplay)
                                        {
                                            // Fenster links erneuern
                                            rLeft = true;
                                        }
                                    }
                                }

                                // Wenn ausschneiden // Wenn links lokal
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                                {
                                    // Wenn links gleich Quellordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }
                            }

                            // Rechts Item // Quelle --> Dropbox // Ziel --> Dropbox
                            else if (listPasteItemsDropbox.Count() > 0 & cutToken == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                            {
                                // Kopieren und verschieben // Dropbox --> Dropbox
                                classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listPasteItemsDropbox, listFilesRight[mfSourceIndex].metadata.PathDisplay, cutToken, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                                // Wenn links Dropbox
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                                {
                                    // Wenn links gleich Zielordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == listFilesRight[mfSourceIndex].metadata.PathDisplay)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }

                                // Wenn ausschneiden // Wenn links Dropbox
                                if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                                {
                                    // Wenn cutToken gleich Token links
                                    if (cutToken == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token)
                                    {
                                        // Wenn links gleich Quellordner
                                        if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == cutPath)
                                        {
                                            // Fenster links erneuern
                                            rLeft = true;
                                        }
                                    }
                                }
                            }

                            // Rechts Item // Quelle --> Andere Cloud // Ziel --> Dropbox
                            else
                            {
                                // Nachricht ausgeben // Nicht möglich
                                DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"), resource.GetString("001_KopierenVerschiebenOnlineOnline"));
                                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                                dBSet.AddButton(dBtn);
                                dEx.AddButtonSet(dBSet);
                                await dEx.ShowAsync(grMain);
                            }
                        }
                    }
                }

                // Ziel --> Links ScrollViewer
                else if (mfSource == "svLeft")
                {
                    // Links ScrollViewer // Ziel --> Lokal
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                    {
                        // Links ScrollViewer // Quelle --> Lokal // Ziel --> Lokal
                        if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                        {
                            // Kopieren und verschieben // Lokal --> lokal
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listPasteFilesLocal, listPasteFoldersLocal, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster links erneuern
                            rLeft = true;

                            // Wenn ausschneiden // Wenn rechts lokal
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                            {
                                // Wenn rechts gleich Quellordner
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                {
                                    // Fenster rechts erneuern
                                    rRight = true;
                                }
                            }
                        }

                        // Links ScrollViewer // Quelle --> One Drive // Ziel --> Lokal
                        else if (listPasteItemsOneDrive.Count() > 0)
                        {
                            // Kopieren und verschieben // One Drive --> Lokal
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToLokal(listPasteItemsOneDrive, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster links erneuern
                            rLeft = true;

                            // Wenn ausschneiden // Wenn rechts One Drive
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                            {
                                // Wenn rechts gleich Qellordner
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == cutPath)
                                {
                                    // Fenster rechts erneuern
                                    rRight = true;
                                }
                            }
                        }

                        // Links ScrollViewer // Quelle --> Dropbox // Ziel --> Lokal
                        else if (listPasteItemsDropbox.Count() > 0)
                        {
                            // Kopieren und verschieben // Dropbox --> Lokal
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToLokal(listPasteItemsDropbox, cutToken, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster links erneuern
                            rLeft = true;

                            // Wenn ausschneiden // Wenn rechts Dropbox
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                            {
                                // Wenn cutToken gleich Token rechts
                                if (cutToken == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                                {
                                    // Wenn rechts gleich Qellordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == cutPath)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }
                            }
                        }
                    }

                    // Links ScrollViewer // Ziel --> One Drive
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                    {
                        // Links ScrollViewer // Quelle --> Lokal // Ziel --> One Drive
                        if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                        {
                            // Kopieren und verschieben // Lokal --> One Drive 
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToOneDrive(listPasteFilesLocal, listPasteFoldersLocal, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster links erneuern
                            rLeft = true;

                            // Wenn ausschneiden// Wenn rechts lokal
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                            {
                                // Wenn rechts gleich Quellordner
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                {
                                    // Fenster rechts erneuern
                                    rRight = true;
                                }
                            }
                        }

                        // Links ScrollViewer // Quelle --> One Drive // Ziel --> One Drive
                        else if (listPasteItemsOneDrive.Count() > 0)
                        {
                            // Kopieren und verschieben // One Drive --> One Drive 
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listPasteItemsOneDrive, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster links erneuern
                            rLeft = true;

                            // Wenn ausschneiden// Wenn rechts One Drive
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                            {
                                // Wenn rechts gleich Quellordner
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == cutPath)
                                {
                                    // Fenster rechts erneuern
                                    rRight = true;
                                }
                            }
                        }

                        // Links Scrollviewer // Quelle --> Andere Cloud // Ziel --> One Drive
                        else
                        {
                            // Nachricht ausgeben // Nicht möglich
                            DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"), resource.GetString("001_KopierenVerschiebenOnlineOnline"));
                            DialogExButtonsSet dBSet = new DialogExButtonsSet();
                            DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                            dBSet.AddButton(dBtn);
                            dEx.AddButtonSet(dBSet);
                            await dEx.ShowAsync(grMain);
                        }
                    }

                    // Links ScrollViewer // Ziel --> Dropbox
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                    {
                        // Links ScrollViewer // Quelle --> Lokal // Ziel --> Dropbox
                        if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                        {
                            // Kopieren und verschieben // Lokal --> Dropbox
                           classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToDropbox(listPasteFilesLocal, listPasteFoldersLocal, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster links erneuern
                            rLeft = true;

                            // Wenn ausschneiden// Wenn rechts lokal
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                            {
                                // Wenn rechts gleich Quellordner
                                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                {
                                    // Fenster rechts erneuern
                                    rRight = true;
                                }
                            }
                        }

                        // Links ScrollViewer // Quelle --> Dropbox // Ziel --> Dropbox
                        else if (listPasteItemsDropbox.Count() > 0 & cutToken == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token)
                        {
                            // Kopieren und verschieben // Dropbox --> Dropbox 
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listPasteItemsDropbox, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, cutToken, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster links erneuern
                            rLeft = true;

                            // Wenn ausschneiden// Wenn rechts Dropbox
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                            {
                                // Wenn cutToken gleich Token rechts
                                if (cutToken == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                                {
                                    // Wenn rechts gleich Quellordner
                                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].path == cutPath)
                                    {
                                        // Fenster rechts erneuern
                                        rRight = true;
                                    }
                                }
                            }
                        }

                        // Links Scrollviewer // Quelle --> Andere Cloud // Ziel --> Dropbox
                        else
                        {
                            // Nachricht ausgeben // Nicht möglich
                            DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"), resource.GetString("001_KopierenVerschiebenOnlineOnline"));
                            DialogExButtonsSet dBSet = new DialogExButtonsSet();
                            DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                            dBSet.AddButton(dBtn);
                            dEx.AddButtonSet(dBSet);
                            await dEx.ShowAsync(grMain);
                        }
                    }
                }

                // Ziel --> Rechts ScrollViewer
                else if (mfSource == "svRight")
                {
                    // Rechts ScrollViewer // Ziel --> Lokal
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                    {
                        // Rechts ScrollViewer // Quelle --> Lokal // Ziel --> Lokal
                        if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                        {
                            // Kopieren und verschieben // Lokal --> lokal
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToLokal(listPasteFilesLocal, listPasteFoldersLocal, listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster rechts erneuern
                            rRight = true;

                            // Wenn ausschneiden // Wenn Links lokal
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                            {
                                // Wenn links gleich Quellordner
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                {
                                    // Fenster links erneuern
                                    rLeft = true;
                                }
                            }
                        }

                        // Rechts ScrollViewer // Quelle --> One Drive // Ziel --> Lokal
                        else if (listPasteItemsOneDrive.Count() > 0)
                        {
                            // Kopieren und verschieben // One Drive --> Lokal
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToLokal(listPasteItemsOneDrive, listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster rechts erneuern
                            rRight = true;

                            // Wenn ausschneiden // Wenn links One Drive
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                            {
                                // Wenn links gleich Quellordner
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == cutPath)
                                {
                                    // Fenster links erneuern
                                    rLeft = true;
                                }
                            }
                        }

                        // Rechts ScrollViewer // Quelle --> Dropbox // Ziel --> Lokal
                        else if (listPasteItemsDropbox.Count() > 0)
                        {
                            // Kopieren und verschieben // Dropbox --> Lokal
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToLokal(listPasteItemsDropbox, cutToken, listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster rechts erneuern
                            rRight = true;

                            // Wenn ausschneiden // Wenn links Dropbox
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                            {
                                // Wenn cutToken gleich Token links
                                if (cutToken == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token)
                                {
                                    // Wenn links gleich Qellordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == cutPath)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }
                            }
                        }
                    }

                    // Rechts ScrollViewer // Ziel --> One Drive
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                    {

                        // Rechts ScrollViewer // Quelle --> Lokal // Ziel --> One Drive
                        if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                        {
                            // Kopieren und verschieben // Lokal --> One Drive
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToOneDrive(listPasteFilesLocal, listPasteFoldersLocal, listFolderTreeRight[listFolderTreeRight.Count() - 1].path, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster rechts erneuern
                            rRight = true;

                            // Wenn ausschneiden // Wenn links Lokal
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                            {
                                // Wenn links gleich Quellordner
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                {
                                    // Fenster links erneuern
                                    rLeft = true;
                                }
                            }
                        }

                        // Rechts ScrollViewer // Quelle --> One Drive // Ziel --> One Drive
                        else if (listPasteItemsOneDrive.Count() > 0)
                        {
                            // Kopieren und verschieben // One Drive --> One Drive
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveOneDriveToOneDrive(listPasteItemsOneDrive, listFolderTreeRight[listFolderTreeRight.Count() - 1].path, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster rechts erneuern
                            rRight = true;

                            // Wenn ausschneiden // Wenn links One Drive
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                            {
                                // Wenn links gleich Quellordner
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == cutPath)
                                {
                                    // Fenster links erneuern
                                    rLeft = true;
                                }
                            }
                        }

                        // Rechts Scrollviewer // Quelle --> Andere Cloud // Ziel --> One Drive
                        else
                        {
                            // Nachricht ausgeben // Nicht möglich
                            DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"), resource.GetString("001_KopierenVerschiebenOnlineOnline"));
                            DialogExButtonsSet dBSet = new DialogExButtonsSet();
                            DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                            dBSet.AddButton(dBtn);
                            dEx.AddButtonSet(dBSet);
                            await dEx.ShowAsync(grMain);
                        }
                    }

                    // Rechts ScrollViewer // Ziel --> Dropbox
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                    {
                        // Rechts ScrollViewer // Quelle --> Lokal // Ziel --> Dropbox
                        if (listPasteFoldersLocal.Count() > 0 | listPasteFilesLocal.Count() > 0)
                        {
                            // Kopieren und verschieben // Lokal --> Dropbox
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveLokalToDropbox(listPasteFilesLocal, listPasteFoldersLocal, listFolderTreeRight[listFolderTreeRight.Count() - 1].path, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster rechts erneuern
                            rRight = true;

                            // Wenn ausschneiden// Wenn links lokal
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                            {
                                // Wenn links gleich Quellordner
                                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder.FolderRelativeId == cutSourceId)
                                {
                                    // Fenster links erneuern
                                    rLeft = true;
                                }
                            }
                        }

                        // Rechts ScrollViewer // Quelle --> Dropbox // Ziel --> Dropbox
                        else if (listPasteItemsDropbox.Count() > 0 & cutToken == listFolderTreeRight[listFolderTreeRight.Count() - 1].token)
                        {
                            // Kopieren und verschieben // Dropbox --> Dropbox 
                            classCopyOrMoveFilesAndFolders = await acCopyOrMoveDropboxToDropbox(listPasteItemsDropbox, listFolderTreeRight[listFolderTreeRight.Count() - 1].path, cutToken, classCopyOrMoveFilesAndFolders, cancelTokenAction.Token);

                            // Fenster rechts erneuern
                            rRight = true;

                            // Wenn ausschneiden// Wenn links Dropbox
                            if (classCopyOrMoveFilesAndFolders.move & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                            {
                                // Wenn cutToken gleich Token links
                                if (cutToken == listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token)
                                {
                                    // Wenn links gleich Quellordner
                                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path == cutPath)
                                    {
                                        // Fenster links erneuern
                                        rLeft = true;
                                    }
                                }
                            }
                        }

                        // Rechts Scrollviewer // Quelle --> Andere Cloud // Ziel --> Dropbox
                        else
                        {
                            // Nachricht ausgeben // Nicht möglich
                            DialogEx dEx = new DialogEx(resource.GetString("001_NichtMöglich"), resource.GetString("001_KopierenVerschiebenOnlineOnline"));
                            DialogExButtonsSet dBSet = new DialogExButtonsSet();
                            DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                            dBSet.AddButton(dBtn);
                            dEx.AddButtonSet(dBSet);
                            await dEx.ShowAsync(grMain);
                        }
                    }
                }



                // Beide Fenster erneuern
                if (rLeft & rRight)
                {
                    outputRefresh("both");
                }

                // Linkes Fenster erneuern
                else if (rLeft)
                {
                    outputRefresh("left");
                }

                // Rechtes Fenster erneuern
                else if (rRight)
                {
                    outputRefresh("right");
                }

                // Parameter zurücksetzen
                paste = false;
                cut = false;
                listPasteFilesLocal.Clear();
                listPasteFoldersLocal.Clear();
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button löschen
        // -----------------------------------------------------------------------
#region Button löschen
        // Methode // Button löschen
        private async void MfiDelete_Click(object sender, RoutedEventArgs e)
        {
            // Klasse zum löschen erstellen
            ClassDeleteFiles classDeleteFiles = new ClassDeleteFiles();

            // Vorherigen Task abbrechen
            if (cancelTokenAction != null)
            {
                cancelTokenAction.Cancel();
            }
            cancelTokenAction = new CancellationTokenSource();

            // Links
            if (mfSource == "left")
            {
                // Lokal
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                {
                    // Daten löschen Lokal // Task
                    classDeleteFiles = await mfiDeleteLocal(lbLeft, listFilesLeft, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder, classDeleteFiles, cancelTokenAction.Token);

                    // Linke Ausgabe erneuern
                    outputRefresh("left");
                }

                // One Drive
                else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                {
                    // Daten löschen // Task
                    classDeleteFiles = await mfiDeleteOneDrive(lbLeft, listFilesLeft, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, classDeleteFiles, cancelTokenAction.Token);

                    // Linke Ausgabe erneuern
                    outputRefresh("left");
                }

                // Dropbox
                else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                {
                    // Daten löschen // Task
                    classDeleteFiles = await mfiDeleteDropbox(lbLeft, listFilesLeft, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, classDeleteFiles, cancelTokenAction.Token);

                    // Linke Ausgabe erneuern
                    outputRefresh("left");
                }
            }

            // Rechts
            else if (mfSource == "right")
            {
                // Lokal
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                {
                    // Daten löschen Lokal // Task
                    classDeleteFiles = await mfiDeleteLocal(lbRight, listFilesRight, listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder, classDeleteFiles, cancelTokenAction.Token);

                    // Rechte Ausgabe erneuern
                    outputRefresh("right");
                }

                // One Drive
                else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                {
                    // Daten löschen // Task
                    classDeleteFiles = await mfiDeleteOneDrive(lbRight, listFilesRight, listFolderTreeRight[listFolderTreeRight.Count() - 1].path, classDeleteFiles, cancelTokenAction.Token);

                    // Rechte Ausgabe erneuern
                    outputRefresh("right");
                }

                // Dropbox
                else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                {
                    // Daten löschen // Task
                    classDeleteFiles = await mfiDeleteDropbox(lbRight, listFilesRight, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, classDeleteFiles, cancelTokenAction.Token);

                    // Rechte Ausgabe erneuern
                    outputRefresh("right");
                }
            }
        }
#endregion



#region Löschen // Lokal
        // Task // Löschen // Lokal
        private async Task<ClassDeleteFiles> mfiDeleteLocal(ListBox listBox, ObservableCollection<ClassFiles> listFiles, StorageFolder storageFolder, ClassDeleteFiles classDeleteFiles, CancellationToken token)
        {
            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Index hinzufüen
                classDeleteFiles.indexes.Add(mfSourceIndex);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;

                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Index in Liste schreiben
                    classDeleteFiles.indexes.Add(listBox.Items.IndexOf(item));

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }
                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Item liste Leeren
                    classDeleteFiles.indexes.Clear();

                    // Item der Liste hinzufügen
                    classDeleteFiles.indexes.Add(mfSourceIndex);
                }
            }

            // Wenn Dateien vorhanden
            if (classDeleteFiles.indexes.Count() > 0)
            {
                // Lade Anzeige augeben
                DialogEx dWaitAbort = new DialogEx();
                dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
                DialogExProgressRing progressRing = new DialogExProgressRing();
                dWaitAbort.AddProgressRing(progressRing);
                dWaitAbort.ShowAsync(grMain);


                // Liste Dateien und Ordner
                List<int> listFilesFolders = new List<int>();
                listFilesFolders.Add(0);
                listFilesFolders.Add(0);

                // Dateien und Ordner zählen
                for (int i = 0; i < classDeleteFiles.indexes.Count(); i++)
                {
                    // Wenn eine Datei
                    if (listFiles[classDeleteFiles.indexes[i]].type == "file")
                    {
                        // Datei hinzufügen
                        listFilesFolders[0]++;
                    }
                    // Wenn ein Ordner
                    else
                    {
                        // Ordner hinzufügen
                        listFilesFolders[1]++;

                        // Datein in Ordner und Unterordner zählen // Task
                        listFilesFolders = await acGetFilesAndFoldersLokal(listFiles[classDeleteFiles.indexes[i]].storageFolder, listFilesFolders);
                    }
                }

                // Nachricht erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");
                // Lade Anzeige entfernen
                dWaitAbort.Hide();
                // Nachricht ausgeben
                DialogEx dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Löschen");
                dQuery.Content = content;
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Löschen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                DialogExButton dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                dQueryBtnSet.AddButton(dQueryBtn_2);
                dQuery.AddButtonSet(dQueryBtnSet);
                await dQuery.ShowAsync(grMain);

                // Antwort auslesen
                string answer = dQuery.GetAnswer();

                // Wenn Antwort löschen
                if (answer == resource.GetString("001_Löschen"))
                {
                    // Löschen Ausgebe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = resource.GetString("001_Löschen");
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    tBlock.Margin = new Thickness(0, 5, 0, -8);
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien durchlaufen
                    for (classDeleteFiles.index = 0; classDeleteFiles.index < classDeleteFiles.indexes.Count(); classDeleteFiles.index++)
                    {
                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                            }
                            // Daten löschen
                            else
                            {
                                // Wenn Ordner
                                if (listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].type == "folder")
                                {
                                    // Ordner mit allen Dateien löschen
                                    classDeleteFiles = await mfDeleteFolderAndFiles(listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].storageFolder, classDeleteFiles, dWaitAbort, token);
                                }

                                // Wenn Datei
                                else if (listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].type == "file")
                                {
                                    // Versuchen Datei zu löschen
                                    try
                                    {
                                        // DialogEx updaten
                                        dWaitAbort.SetTextBoxTextByIndex(0, listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].storageFile.Name);

                                        // Datei löschen
                                        await listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].storageFile.DeleteAsync();

                                        // Datei in Index eintragen
                                        classDeleteFiles.deletedIndexes.Add(classDeleteFiles.indexes[classDeleteFiles.index]);

                                        // Anzahl der gelöschten Dateien erhöhen
                                        classDeleteFiles.value++;
                                    }
                                    // Bei Fehlern
                                    catch
                                    {
                                        // Wenn Ausgabe erstellt wird
                                        if (!classDeleteFiles.allCantDeleteFiles)
                                        {
                                            // Ausgabe erstellen
                                            dQuery = new DialogEx();
                                            dQuery.Title = resource.GetString("001_DateiNichtGelöscht");
                                            dQuery.Content = '"' + listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].storageFile.Name + '"';
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            dQueryBtnSet = new DialogExButtonsSet();
                                            dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                                            dQueryBtn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            dQueryBtnSet.AddButton(dQueryBtn_1);
                                            DialogExButton btn_4 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            dQueryBtnSet.AddButton(btn_4);
                                            dQuery.AddButtonSet(dQueryBtnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Antwort auslesen
                                            answer = dQuery.GetAnswer();

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }

                                            // Wenn überspringen
                                            else if (answer == resource.GetString("001_Überspringen"))
                                            {
                                                // Wenn für alle übernehmen
                                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                                {
                                                    classDeleteFiles.allCantDeleteFiles = true;
                                                }
                                            }
                                        }
                                        // Datei in Liste nicht gelöschter Dateien einfügen
                                        classDeleteFiles.cantDeleteFiles.Add(listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].storageFile.Path + "//" + listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].storageFile.Name);
                                    }
                                }
                            }
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                        // Value erhöhen und Updaten
                        classDeleteFiles.value++;
                        dWaitAbort.SetProgressBarValueByIndex(0, classDeleteFiles.value);
                    }
                    // Löschen Ausgabe entfernen
                    dWaitAbort.Hide();
                }
            }
            // Rückgabe erstellen
            return classDeleteFiles;
        }



        // Task // Ordner mit allen Dateien löschen // Lokal
        private async Task<ClassDeleteFiles> mfDeleteFolderAndFiles(StorageFolder storageFolder, ClassDeleteFiles classDeleteFiles, DialogEx dWaitAbort, CancellationToken token, bool isTemp = false)
        {
            // Parameter // Liste Ordner // Liste Dateien
            IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
            IReadOnlyList<StorageFile> fileList = new List<StorageFile>();

            // Versuchen Daten auszulesen
            try
            {
                // Dateien auslesen
                folderList = await storageFolder.GetFoldersAsync();
                fileList = await storageFolder.GetFilesAsync();
            }
            // Wenn Dateien nicht ausgelesen werden konnten
            catch
            {
                // Wenn Ausgabe erstellt wird
                if (!classDeleteFiles.allCantOpenFolders & !isTemp)
                {
                    // Ausgabe erstellen
                    DialogEx dQuery = new DialogEx();
                    dQuery.Title = resource.GetString("001_OrdnerNichÖffnen");
                    dQuery.Content = '"' + storageFolder.Name + '"';
                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                    dQuery.AddCheckbox(chBox);
                    DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                    DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                    dQueryBtnSet.AddButton(dQueryBtn_1);
                    DialogExButton dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dQueryBtnSet.AddButton(dQueryBtn_2);
                    dQuery.AddButtonSet(dQueryBtnSet);
                    // Neue Ausgabe erstellen
                    dWaitAbort.Hide();
                    await dQuery.ShowAsync(grMain);
                    dWaitAbort.ShowAsync(grMain);

                    // Antwort auslesen
                    string answer = dQuery.GetAnswer();

                    // Wenn Abbrechen
                    if (answer == resource.GetString("001_Abbrechen"))
                    {
                        // Task abbrechen
                        cancelTokenAction.Cancel();
                    }

                    // Wenn überspringen
                    else if (answer == resource.GetString("001_Überspringen"))
                    {
                        // Wenn für alle übernehmen
                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                        {
                            classDeleteFiles.allCantOpenFolders = true;
                        }
                    }
                }
                // Ordner in nicht öffnen Liste schreiben
                classDeleteFiles.cantOpenFolders.Add(storageFolder.Path + "//" + storageFolder.Name);
            }

            // Ordner durchlaufen
            for (int i = 0; i < folderList.Count(); i++)
            {
                // Wenn abbrechen gedrück wurde
                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                {
                    cancelTokenAction.Cancel();
                }

                // Wenn Task nicht abgebrochen
                if (!token.IsCancellationRequested)
                {
                    // Ordner mit allen Dateien löschen
                    classDeleteFiles = await mfDeleteFolderAndFiles(folderList[i], classDeleteFiles, dWaitAbort, token, isTemp);
                }
                // Wenn Task abgebrochen
                else
                {
                    break;
                }
            }

            // Dateien durchlaufen
            for (int i = 0; i < fileList.Count(); i++)
            {
                // Wenn abbrechen gedrück wurde
                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                {
                    cancelTokenAction.Cancel();
                }

                // Wenn Task nicht abgebrochen
                if (!token.IsCancellationRequested)
                {
                    // Versuchen Datei zu löschen
                    try
                    {
                        // DialogEx updaten
                        dWaitAbort.SetTextBoxTextByIndex(0, fileList[i].Name);

                        // Datei löschen
                        await fileList[i].DeleteAsync();

                        // Zahl der gelöschten Dateien erhöhen
                        classDeleteFiles.value++;
                    }
                    // Bei Fehlern
                    catch
                    {
                        // Wenn Ausgabe erstellt wird
                        if (!classDeleteFiles.allCantDeleteFiles & !isTemp)
                        {
                            // Ausgabe erstellen
                            DialogEx dQuery = new DialogEx();
                            dQuery.Title = resource.GetString("001_DateiNichtGelöscht");
                            dQuery.Content = '"' + fileList[i].Name + '"';
                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                            dQuery.AddCheckbox(chBox);
                            DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                            DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                            dQueryBtnSet.AddButton(dQueryBtn_1);
                            DialogExButton dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dQueryBtnSet.AddButton(dQueryBtn_2);
                            dQuery.AddButtonSet(dQueryBtnSet);
                            // Neue Ausgabe erstellen
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                            dWaitAbort.ShowAsync(grMain);

                            // Antwort auslesen
                            string answer = dQuery.GetAnswer();

                            // Wenn Abbrechen
                            if (answer == resource.GetString("001_Abbrechen"))
                            {
                                // Task abbrechen
                                cancelTokenAction.Cancel();
                            }

                            // Wenn überspringen
                            else if (answer == resource.GetString("001_Überspringen"))
                            {
                                // Wenn für alle übernehmen
                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                {
                                    classDeleteFiles.allCantDeleteFiles = true;
                                }
                            }
                        }
                        // Datei in nicht gelöschte Dateien hinzufügen
                        classDeleteFiles.cantDeleteFiles.Add(fileList[i].Path + "//" + fileList[i].Name);
                    }
                }
                // Wenn Task abgebrochen
                else
                {
                    break;
                }
                // Value erhöhen und Updaten
                classDeleteFiles.value++;
                dWaitAbort.SetProgressBarValueByIndex(0, classDeleteFiles.value);
            }

            // Wenn abbrechen gedrück wurde
            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
            {
                cancelTokenAction.Cancel();
            }

            // Wenn Task nicht abgebrochen
            if (!token.IsCancellationRequested)
            {
                // Versuchen Ordner zu löschen
                try
                {
                    // Ordner löschen
                    await storageFolder.DeleteAsync();

                    // Datei in Index eintragen
                    classDeleteFiles.deletedIndexes.Add(classDeleteFiles.indexes[classDeleteFiles.index]);

                    // Zahl der gelöschten Dateien erhöhen
                    classDeleteFiles.value++;
                }
                // Wenn Ordner nicht gelöscht werden kann
                catch
                {
                    // Wenn Ausgabe erstellt wird
                    if (!classDeleteFiles.allCantDeleteFolders & !token.IsCancellationRequested & !isTemp)
                    {
                        // Ausgabe erstellen
                        DialogEx dQuery = new DialogEx();
                        dQuery.Title = resource.GetString("001_OrdnerNichtLöschen");
                        dQuery.Content = '"' + storageFolder.Name + '"';
                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                        dQuery.AddCheckbox(chBox);
                        DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                        DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                        dQueryBtnSet.AddButton(dQueryBtn_1);
                        DialogExButton dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        dQueryBtnSet.AddButton(dQueryBtn_2);
                        dQuery.AddButtonSet(dQueryBtnSet);
                        // Neue Ausgabe erstellen
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);
                        dWaitAbort.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn Abbrechen
                        if (answer == resource.GetString("001_Abbrechen"))
                        {
                            // Task abbrechen
                            cancelTokenAction.Cancel();
                        }

                        // Wenn überspringen
                        else if (answer == resource.GetString("001_Überspringen"))
                        {
                            // Wenn für alle übernehmen
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classDeleteFiles.allCantDeleteFolders = true;
                            }
                        }
                    }
                    // Ordner in nicht gelöschte Ordner speichern
                    classDeleteFiles.cantDeleteFolders.Add(storageFolder.Path + "//" + storageFolder.Name);
                }
                // Value erhöhen und Updaten
                classDeleteFiles.value++;
                dWaitAbort.SetProgressBarValueByIndex(0, classDeleteFiles.value);
            }
            // Rückgabe
            return classDeleteFiles;
        }
#endregion



#region Löschen // One Drive
        // Task // Löschen // OneDrive
        private async Task<ClassDeleteFiles> mfiDeleteOneDrive(ListBox listBox, ObservableCollection<ClassFiles> listFiles, string path, ClassDeleteFiles classDeleteFiles, CancellationToken token)
        {
            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Index hinzufüen
                classDeleteFiles.indexes.Add(mfSourceIndex);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;

                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Index in Liste schreiben
                    classDeleteFiles.indexes.Add(listBox.Items.IndexOf(item));

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }
                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Item liste Leeren
                    classDeleteFiles.indexes.Clear();

                    // Item der Liste hinzufügen
                    classDeleteFiles.indexes.Add(mfSourceIndex);
                }
            }

            // Wenn Dateien vorhanden
            if (classDeleteFiles.indexes.Count() > 0)
            {
                // Lade Anzeige augeben
                DialogEx dWaitAbort = new DialogEx();
                dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
                DialogExProgressRing progressRing = new DialogExProgressRing();
                dWaitAbort.AddProgressRing(progressRing);
                dWaitAbort.ShowAsync(grMain);

                // Liste Dateien und Ordner
                List<int> listFilesFolders = new List<int>();
                listFilesFolders.Add(0);
                listFilesFolders.Add(0);

                // Items durchlaufen
                for (int i = 0; i < classDeleteFiles.indexes.Count(); i++)
                {
                    // Wenn Ordner
                    if (listFiles[classDeleteFiles.indexes[i]].type == "folder")
                    {
                        // Ordner hinzufügen
                        listFilesFolders[1]++;

                        // Datein in Ordner und Unterordner zählen // Task
                        listFilesFolders = await acGetFilesAndFoldersOneDrive(listFiles[classDeleteFiles.indexes[i]].item, listFilesFolders);
                    }
                    // Wenn Datei
                    else
                    {
                        listFilesFolders[0]++;
                    }
                }

                // Nachricht erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Lade Anzeige entfernen
                dWaitAbort.Hide();
                // Nachricht ausgeben
                DialogEx dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Löschen");
                dQuery.Content = content;
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Löschen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                DialogExButton dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                dQueryBtnSet.AddButton(dQueryBtn_2);
                dQuery.AddButtonSet(dQueryBtnSet);
                await dQuery.ShowAsync(grMain);

                // Antwort auslesen
                string answer = dQuery.GetAnswer();

                // Wenn Antwort löschen
                if (answer == resource.GetString("001_Löschen"))
                {
                    // Löschen Ausgebe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = resource.GetString("001_Löschen");
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    tBlock.Margin = new Thickness(0, 5, 0, -8);
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien durchlaufen
                    for (classDeleteFiles.index = 0; classDeleteFiles.index < classDeleteFiles.indexes.Count(); classDeleteFiles.index++)
                    {
                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                            }

                            // Versuchen Datei zu löschen
                            try
                            {
                                // DialogEx updaten
                                dWaitAbort.SetTextBoxTextByIndex(0, listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].item.Name);

                                // Datei oder Ordner löschen
                                await oneDriveClient.Drive.Items[listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].item.Id].Request().DeleteAsync();

                                // Datei oder Ordner in Index eintragen
                                classDeleteFiles.deletedIndexes.Add(classDeleteFiles.indexes[classDeleteFiles.index]);

                                // Anzahl der gelöschten Dateien erhöhen
                                classDeleteFiles.value++;
                            }
                            // Bei Fehlern
                            catch
                            {
                                // Wenn Ausgabe erstellt wird
                                if (!classDeleteFiles.allCantDeleteFiles)
                                {
                                    // Ausgabe erstellen
                                    dQuery = new DialogEx();
                                    dQuery.Title = resource.GetString("001_DateiOrdnerNichtGelöscht");
                                    dQuery.Content = '"' + listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].item.Name + '"';
                                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                    dQuery.AddCheckbox(chBox);
                                    dQueryBtnSet = new DialogExButtonsSet();
                                    dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                                    dQueryBtn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                    dQueryBtnSet.AddButton(dQueryBtn_1);
                                    DialogExButton btn_4 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                    dQueryBtnSet.AddButton(btn_4);
                                    dQuery.AddButtonSet(dQueryBtnSet);
                                    // Neue Ausgabe erstellen
                                    dWaitAbort.Hide();
                                    await dQuery.ShowAsync(grMain);
                                    dWaitAbort.ShowAsync(grMain);

                                    // Antwort auslesen
                                    answer = dQuery.GetAnswer();

                                    // Wenn Abbrechen
                                    if (answer == resource.GetString("001_Abbrechen"))
                                    {
                                        // Task abbrechen
                                        cancelTokenAction.Cancel();
                                    }

                                    // Wenn überspringen
                                    else if (answer == resource.GetString("001_Überspringen"))
                                    {
                                        // Wenn für alle übernehmen
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classDeleteFiles.allCantDeleteFiles = true;
                                        }
                                    }
                                }
                            }
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                        // Value erhöhen und Updaten
                        classDeleteFiles.value++;
                        dWaitAbort.SetProgressBarValueByIndex(0, classDeleteFiles.value);
                    }
                    // Löschen Ausgabe entfernen
                    dWaitAbort.Hide();
                }
            }
            // Rückgabe erstellen
            return classDeleteFiles;
        }
#endregion



#region Löschen // Dropbox
        // Task // Löschen // OneDrive
        private async Task<ClassDeleteFiles> mfiDeleteDropbox(ListBox listBox, ObservableCollection<ClassFiles> listFiles, string dropboxToken, ClassDeleteFiles classDeleteFiles, CancellationToken token)
        {
            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Index hinzufüen
                classDeleteFiles.indexes.Add(mfSourceIndex);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;

                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Index in Liste schreiben
                    classDeleteFiles.indexes.Add(listBox.Items.IndexOf(item));

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }
                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Item liste Leeren
                    classDeleteFiles.indexes.Clear();

                    // Item der Liste hinzufügen
                    classDeleteFiles.indexes.Add(mfSourceIndex);
                }
            }

            // Wenn Dateien vorhanden
            if (classDeleteFiles.indexes.Count() > 0)
            {
                // Lade Anzeige augeben
                DialogEx dWaitAbort = new DialogEx();
                dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
                DialogExProgressRing progressRing = new DialogExProgressRing();
                dWaitAbort.AddProgressRing(progressRing);
                dWaitAbort.ShowAsync(grMain);

                // Liste Dateien und Ordner
                List<int> listFilesFolders = new List<int>();
                listFilesFolders.Add(0);
                listFilesFolders.Add(0);

                // Items durchlaufen
                for (int i = 0; i < classDeleteFiles.indexes.Count(); i++)
                {
                    // Wenn Ordner
                    if (listFiles[classDeleteFiles.indexes[i]].type == "folder")
                    {
                        // Ordner hinzufügen
                        listFilesFolders[1]++;

                        // Datein in Ordner und Unterordner zählen // Task
                        listFilesFolders = await acGetFilesAndFoldersDropbox(listFiles[classDeleteFiles.indexes[i]].metadata, dropboxToken, listFilesFolders);
                    }
                    // Wenn Datei
                    else
                    {
                        listFilesFolders[0]++;
                    }
                }

                // Nachricht erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Lade Anzeige entfernen
                dWaitAbort.Hide();
                // Nachricht ausgeben
                DialogEx dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Löschen");
                dQuery.Content = content;
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Löschen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                DialogExButton dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                dQueryBtnSet.AddButton(dQueryBtn_2);
                dQuery.AddButtonSet(dQueryBtnSet);
                await dQuery.ShowAsync(grMain);

                // Antwort auslesen
                string answer = dQuery.GetAnswer();

                // Wenn Antwort löschen
                if (answer == resource.GetString("001_Löschen"))
                {
                    // Löschen Ausgebe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = resource.GetString("001_Löschen");
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    tBlock.Margin = new Thickness(0, 5, 0, -8);
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien durchlaufen
                    for (classDeleteFiles.index = 0; classDeleteFiles.index < classDeleteFiles.indexes.Count(); classDeleteFiles.index++)
                    {
                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                            }

                            // Versuchen Datei zu löschen
                            try
                            {
                                // DialogEx updaten
                                dWaitAbort.SetTextBoxTextByIndex(0, listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].metadata.Name);

                                // Datei oder Ordner löschen
                                DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                await dropboxClient.Files.DeleteAsync(listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].metadata.PathDisplay);

                                // Datei oder Ordner in Index eintragen
                                classDeleteFiles.deletedIndexes.Add(classDeleteFiles.indexes[classDeleteFiles.index]);

                                // Anzahl der gelöschten Dateien erhöhen
                                classDeleteFiles.value++;
                            }
                            // Bei Fehlern
                            catch
                            {
                                // Wenn Ausgabe erstellt wird
                                if (!classDeleteFiles.allCantDeleteFiles)
                                {
                                    // Ausgabe erstellen
                                    dQuery = new DialogEx();
                                    dQuery.Title = resource.GetString("001_DateiOrdnerNichtGelöscht");
                                    dQuery.Content = '"' + listFiles[classDeleteFiles.indexes[classDeleteFiles.index]].item.Name + '"';
                                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                    dQuery.AddCheckbox(chBox);
                                    dQueryBtnSet = new DialogExButtonsSet();
                                    dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                                    dQueryBtn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                    dQueryBtnSet.AddButton(dQueryBtn_1);
                                    DialogExButton btn_4 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                    dQueryBtnSet.AddButton(btn_4);
                                    dQuery.AddButtonSet(dQueryBtnSet);
                                    // Neue Ausgabe erstellen
                                    dWaitAbort.Hide();
                                    await dQuery.ShowAsync(grMain);
                                    dWaitAbort.ShowAsync(grMain);

                                    // Antwort auslesen
                                    answer = dQuery.GetAnswer();

                                    // Wenn Abbrechen
                                    if (answer == resource.GetString("001_Abbrechen"))
                                    {
                                        // Task abbrechen
                                        cancelTokenAction.Cancel();
                                    }

                                    // Wenn überspringen
                                    else if (answer == resource.GetString("001_Überspringen"))
                                    {
                                        // Wenn für alle übernehmen
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classDeleteFiles.allCantDeleteFiles = true;
                                        }
                                    }
                                }
                            }
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                        // Value erhöhen und Updaten
                        classDeleteFiles.value++;
                        dWaitAbort.SetProgressBarValueByIndex(0, classDeleteFiles.value);
                    }
                    // Löschen Ausgabe entfernen
                    dWaitAbort.Hide();
                }
            }
            // Rückgabe erstellen
            return classDeleteFiles;
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button teilen
        // -----------------------------------------------------------------------
#region Button teilen

        // Button Teilen
        private void MfiShare_Click(object sender, RoutedEventArgs e)
        {
            // DataTransferManager erstellen
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            // ShareHandler // Methode
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.ShareHandler);
            // Daten teilen
            DataTransferManager.ShowShareUI();
        }
#endregion



#region Teilen // Lokal

        // Methode // ShareHandler
        private async void ShareHandler(DataTransferManager sender, DataRequestedEventArgs e)
        {
            // Paramete und Argumente
            List<int> indexes = new List<int>();
            ListBox listBox = new ListBox();
            ObservableCollection<ClassFiles> listFiles = new ObservableCollection<ClassFiles>();
            List<IStorageItem> listFilesToShare = new List<IStorageItem>();

            // Links
            if (mfSource == "left")
            {
                // Argumente setzen
                listBox = lbLeft;
                listFiles = listFilesLeft;
            }

            // Rechts
            else if (mfSource == "right")
            {
                // Argumente setzen
                listBox = lbRight;
                listFiles = listFilesRight;
            }

            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Index hinzufüen
                indexes.Add(mfSourceIndex);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;
                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Index in Liste schreiben
                    indexes.Add(listBox.Items.IndexOf(item));

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }

                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Liste indexe leeren
                    indexes.Clear();
                    // Item der Liste hinzufügen
                    indexes.Add(mfSourceIndex);
                }
            }

            // Daten zum Teilen in Parameter setzen
            for (int i = 0; i < indexes.Count(); i++)
            {
                // Wenn datei
                if (listFiles[indexes[i]].type == "file")
                {
                    listFilesToShare.Add(listFiles[indexes[i]].storageFile);
                }
            }

            // Wenn Datei vorhanden
            if (listFilesToShare.Count() > 0)
            {
                // Items Liste erstellen
                IEnumerable<IStorageItem> items = listFilesToShare;

                // DataRequest erstellen
                DataRequest request = e.Request;
                request.Data.Properties.Title = " ";
                request.Data.Properties.Description = " ";

                // Versuchen Daten zu teilen
                try
                {
                    // Daten teilen
                    request.Data.SetStorageItems(items);
                }
                catch
                {
                    // Nachricht ausgeben // Nicht möglich
                    DialogEx dEx = new DialogEx();
                    dEx.Title = resource.GetString("001_NichtMöglich");
                    DialogExButtonsSet dExBtnSet = new DialogExButtonsSet();
                    DialogExButton dExBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                    dExBtnSet.AddButton(dExBtn_1);
                    dEx.AddButtonSet(dExBtnSet);
                    await dEx.ShowAsync(grMain);
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button Freigabe Link
        // -----------------------------------------------------------------------
#region Button Freigabe Link

        // Share Link
        private string shareLink;

        // Button Freigabe Link
        private async void MfiShareLink_Click(object sender, RoutedEventArgs e)
        {
            // Nachricht ausgeben
            DialogEx dQuery = new DialogEx();
            dQuery = new DialogEx();
            dQuery.BorderBrush = scbXtrose;
            dQuery.Title = resource.GetString("001_FreigabeLink");
            dQuery.Content = resource.GetString("001_Berechtigung");
            DialogExButtonsSet btnSet = new DialogExButtonsSet();
            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_LesenSchreiben"));
            btnSet.AddButton(btn_1);
            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Lesen"));
            btnSet.AddButton(btn_2);
            DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
            btnSet.AddButton(btn_3);
            dQuery.AddButtonSet(btnSet);
            await dQuery.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dQuery.GetAnswer();

            // Wenn Antwort nuicht abbrechen
            if (answer != resource.GetString("001_Abbrechen"))
            {
                // Antwort erstellen
                string method = "view";

                // Wenn Antwort Lesen und schreiben
                if (answer == resource.GetString("001_LesenSchreiben"))
                {
                    method = "edit";
                }

                // Lade Anzeige augeben
                DialogEx dWaitAbort = new DialogEx();
                dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
                DialogExProgressRing progressRing = new DialogExProgressRing();
                dWaitAbort.AddProgressRing(progressRing);
                dWaitAbort.ShowAsync(grMain);

                // Versuchen Link abzurufen
                try
                {
                    // Link
                    string link = "";

                    // Links
                    if (mfSource == "left")
                    {
                        // Freigabe Link abrufen
                        var linkVar = await oneDriveClient.Drive.Items[listFilesLeft[mfSourceIndex].item.Id].CreateLink(method).Request().PostAsync();
                        link = linkVar.Link.WebUrl;
                    }

                    // Rechts
                    if (mfSource == "right")
                    {
                        // Freigabe Link abrufen
                        var linkVar = await oneDriveClient.Drive.Items[listFilesRight[mfSourceIndex].item.Id].CreateLink(method).Request().PostAsync();
                        link = linkVar.Link.WebUrl;
                    }

                    // Lade Anzeige verbergen
                    dWaitAbort.Hide();

                    // Nachricht ausgeben
                    dQuery = new DialogEx();
                    dQuery = new DialogEx();
                    dQuery.BorderBrush = scbXtrose;
                    dQuery.Title = resource.GetString("001_FreigabeLink");
                    dQuery.Content = link;
                    btnSet = new DialogExButtonsSet();
                    btn_1 = new DialogExButton(resource.GetString("001_Kopieren"));
                    btnSet.AddButton(btn_1);
                    btn_2 = new DialogExButton(resource.GetString("001_Teilen"));
                    btnSet.AddButton(btn_2);
                    btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    btnSet.AddButton(btn_3);
                    dQuery.AddButtonSet(btnSet);
                    await dQuery.ShowAsync(grMain);

                    // Antwort auslesen
                    answer = dQuery.GetAnswer();

                    // Wenn Antwort Kopieren
                    if (answer == resource.GetString("001_Kopieren"))
                    {
                        // Datei in Clipboard kopieren
                        DataPackage dp = new DataPackage();
                        dp.SetText(link);
                        Clipboard.SetContent(dp);
                    }

                    // Wenn Antwort Teilen
                    else if (answer == resource.GetString("001_Teilen"))
                    {
                        // Share Link erstellen
                        shareLink = link;

                        // DataTransferManager erstellen
                        DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
                        // ShareHandler // Methode
                        dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.acGetShareLink);
                        // Daten teilen
                        DataTransferManager.ShowShareUI();
                    }
                }
                catch
                {
                    // Lade Anzeige verbergen
                    dWaitAbort.Hide();

                    // Nachricht ausgeben // Verbindung nicht möglich
                    dQuery = new DialogEx(resource.GetString("001_VerbindungNicht"));
                    DialogExButtonsSet dBSet = new DialogExButtonsSet();
                    DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                    dBSet.AddButton(dBtn);
                    dQuery.AddButtonSet(dBSet);
                    await dQuery.ShowAsync(grMain);

                }
            }
        }



        // Methode Freigabe Link abrufen
        private async void acGetShareLink(DataTransferManager sender, DataRequestedEventArgs e)
        {
            // DataRequest erstellen
            DataRequest request = e.Request;
            request.Data.Properties.Title = " ";
            request.Data.Properties.Description = " ";

            // Versuchen Daten zu teilen
            try
            {
                // Daten teilen
                request.Data.SetUri(new Uri(shareLink));
            }
            catch
            {
                // Nachricht ausgeben // Nicht möglich
                DialogEx dEx = new DialogEx();
                dEx.Title = resource.GetString("001_NichtMöglich");
                DialogExButtonsSet dExBtnSet = new DialogExButtonsSet();
                DialogExButton dExBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                dExBtnSet.AddButton(dExBtn_1);
                dEx.AddButtonSet(dExBtnSet);
                await dEx.ShowAsync(grMain);
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button umbenennen
        // -----------------------------------------------------------------------
#region Button umbenennen
        private async void MfiRename_Click(object sender, RoutedEventArgs e)
        {
            // Klasse zum umbenennen erstellen
            ClassRenameFiles classRenameFiles = new ClassRenameFiles();

            // Vorherigen Task abbrechen
            if (cancelTokenAction != null)
            {
                cancelTokenAction.Cancel();
            }
            cancelTokenAction = new CancellationTokenSource();

            // Links
            if (mfSource == "left")
            {
                // Lokal
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                {
                    // Daten umbenennen Lokal // Task
                    classRenameFiles = await mfiRenameLocal(lbLeft, listFilesLeft, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder, classRenameFiles, cancelTokenAction.Token);

                    // Linke Ausgabe erneuern
                    outputRefresh("left");
                }

                // One Drive
                else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                {
                    // Daten umbenennen One Drive // Task
                    classRenameFiles = await mfiRenameOneDrive(lbLeft, listFilesLeft, classRenameFiles, cancelTokenAction.Token);

                    // Linke Ausgabe erneuern
                    outputRefresh("left");
                }

                // Dropbox
                else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                {
                    // Daten umbenennen Dropbox // Task
                    classRenameFiles = await mfiRenameDropbox(lbLeft, listFilesLeft, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, classRenameFiles, cancelTokenAction.Token);

                    // Linke Ausgabe erneuern
                    outputRefresh("left");
                }
            }

            // Rechts
            else if (mfSource == "right")
            {
                // Lokal
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                {
                    // Daten umbenennen Lokal // Task
                    classRenameFiles = await mfiRenameLocal(lbRight, listFilesRight, listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder, classRenameFiles, cancelTokenAction.Token);

                    // Rechte Ausgabe erneuern
                    outputRefresh("right");
                }

                // One Drive
                else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                {
                    // Daten umbenennen One Drive // Task
                    classRenameFiles = await mfiRenameOneDrive(lbRight, listFilesRight, classRenameFiles, cancelTokenAction.Token);

                    // Rechte Ausgabe erneuern
                    outputRefresh("right");
                }

                // Dropbox
                else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                {
                    // Daten umbenennen Dropbox // Task
                    classRenameFiles = await mfiRenameDropbox(lbRight, listFilesRight, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, classRenameFiles, cancelTokenAction.Token);

                    // Rechte Ausgabe erneuern
                    outputRefresh("right");
                }
            }
        }
#endregion



#region umbenennen lokal
        // Task // Daten umbennen Lokal
        private async Task<ClassRenameFiles> mfiRenameLocal(ListBox listBox, ObservableCollection<ClassFiles> listFiles, StorageFolder storageFolder, ClassRenameFiles classRenameFiles, CancellationToken token)
        {
            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Index hinzufüen
                classRenameFiles.indexes.Add(mfSourceIndex);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;

                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Index in Liste schreiben
                    classRenameFiles.indexes.Add(listBox.Items.IndexOf(item));

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }
                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Item liste Leeren
                    classRenameFiles.indexes.Clear();

                    // Item der Liste hinzufügen
                    classRenameFiles.indexes.Add(mfSourceIndex);
                }
            }

            // Wenn Dateien vorhanden
            if (classRenameFiles.indexes.Count() > 0)
            {
                // Lade Anzeige augeben
                DialogEx dWaitAbort = new DialogEx();
                dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
                DialogExProgressRing progressRing = new DialogExProgressRing();
                dWaitAbort.AddProgressRing(progressRing);
                dWaitAbort.ShowAsync(grMain);


                // Liste Dateien und Ordner
                List<int> listFilesFolders = new List<int>();
                listFilesFolders.Add(0);
                listFilesFolders.Add(0);

                // Dateien und Ordner zählen
                for (int i = 0; i < classRenameFiles.indexes.Count(); i++)
                {
                    // Wenn eine Datei
                    if (listFiles[classRenameFiles.indexes[i]].type == "file")
                    {
                        // Datei hinzufügen
                        listFilesFolders[0]++;
                    }
                    // Wenn ein Ordner
                    else
                    {
                        // Ordner hinzufügen
                        listFilesFolders[1]++;
                    }
                }

                // Nachricht erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Lade Anzeige entfernen
                dWaitAbort.Hide();
                // Nachricht ausgeben
                DialogEx dialogEx = new DialogEx();
                dialogEx.Title = resource.GetString("001_Umbenennen");
                dialogEx.Content = content;
                DialogExTextBox textBox_1 = new DialogExTextBox();
                textBox_1.Title = resource.GetString("001_Name");
                dialogEx.AddTextBox(textBox_1);
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Umbenennen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                DialogExButton dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                dQueryBtnSet.AddButton(dQueryBtn_2);
                dialogEx.AddButtonSet(dQueryBtnSet);
                await dialogEx.ShowAsync(grMain);

                // Antwort auslesen
                string answer = dialogEx.GetAnswer();

                // Wenn Antwort umbenennen
                if (answer == resource.GetString("001_Umbenennen"))
                {
                    // Löschen Ausgebe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = resource.GetString("001_Umbenennen");
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    tBlock.Margin = new Thickness(0, 5, 0, -8);
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Wenn Name vorhanden
                    if (dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Name")) != "")
                    {
                        // Daten durchlaufen
                        for (classRenameFiles.index = 0; classRenameFiles.index < classRenameFiles.indexes.Count(); classRenameFiles.index++)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                            }
                            // Wenn Task noch nicht abgebrochen
                            if (!token.IsCancellationRequested)
                            {
                                // Wenn Ordner
                                if (listFiles[classRenameFiles.indexes[classRenameFiles.index]].storageFolder != null)
                                {
                                    // Versuchen Ordner umzubenennen
                                    try
                                    {
                                        // DialogEx updaten
                                        dWaitAbort.SetTextBoxTextByIndex(0, listFiles[classRenameFiles.indexes[classRenameFiles.index]].storageFolder.Name);
                                        // Ordner umbennenen
                                        await listFiles[classRenameFiles.indexes[classRenameFiles.index]].storageFolder.RenameAsync(dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Name")), NameCollisionOption.GenerateUniqueName);
                                    }
                                    catch (Exception exception)
                                    {
                                        // Bei Falschen Namen
                                        if (exception.Message == "The filename, directory name, or volume label syntax is incorrect.\r\n" | exception.Message == "The parameter is incorrect.\r\n")
                                        {
                                            // Fehlermeldung
                                            dialogEx = new DialogEx();
                                            dialogEx.BorderBrush = scbXtrose;
                                            dialogEx.Title = resource.GetString("001_OrdnerNichtUmbenannt");
                                            dialogEx.Content = resource.GetString("001_NameFalsch");
                                            dQueryBtnSet = new DialogExButtonsSet();
                                            dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                                            dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                                            dQueryBtnSet.AddButton(dQueryBtn_1);
                                            dialogEx.AddButtonSet(dQueryBtnSet);
                                            // Eingabeaufförderung ausgeben
                                            dWaitAbort.Hide();
                                            await dialogEx.ShowAsync(grMain);

                                            // Umbennenen abbrechen
                                            break;
                                        }

                                        // Bei unbekanntem fehler
                                        else
                                        {
                                            // Wenn Fehlermeldung ausgegeben wird
                                            if (!classRenameFiles.allCantRenameFilesAndFolders)
                                            {
                                                // Fehlermeldung
                                                dialogEx = new DialogEx();
                                                dialogEx.BorderBrush = scbXtrose;
                                                dialogEx.Title = resource.GetString("001_OrdnerNichtUmbenannt");
                                                DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                                dialogEx.AddCheckbox(chBox);
                                                dQueryBtnSet = new DialogExButtonsSet();
                                                dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                                                dQueryBtn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                                dQueryBtnSet.AddButton(dQueryBtn_1);
                                                dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                                dialogEx.AddButtonSet(dQueryBtnSet);
                                                // Eingabeaufförderung ausgeben
                                                dWaitAbort.Hide();
                                                await dialogEx.ShowAsync(grMain);

                                                // Antwort auslesen
                                                answer = dialogEx.GetAnswer();

                                                // Wenn Antwort umbenennen
                                                if (answer == resource.GetString("001_Überspringen"))
                                                {
                                                    // Wenn für Alle
                                                    if (dialogEx.GetCheckboxByContent("001_FürAlle"))
                                                    {
                                                        classRenameFiles.allCantRenameFilesAndFolders = true;
                                                    }
                                                    // DialogEx ausgeben
                                                    dWaitAbort.Hide();
                                                }

                                                // Wenn Antwort abbrechen
                                                else if (answer == resource.GetString("001_Abbrechen"))
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    // Value erhöhen und Updaten
                                    classRenameFiles.value++;
                                    dWaitAbort.SetProgressBarValueByIndex(0, classRenameFiles.value);
                                }

                                // Wenn Datei
                                else
                                {
                                    // Versuchen Datei umzubenennen
                                    try
                                    {
                                        // DialogEx updaten
                                        dWaitAbort.SetTextBoxTextByIndex(0, listFiles[classRenameFiles.indexes[classRenameFiles.index]].storageFile.Name);

                                        // Datei Name festlegen
                                        string fileName = dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Name")) + listFiles[classRenameFiles.indexes[classRenameFiles.index]].storageFile.FileType;

                                        // Datei umbennenen
                                        await listFiles[classRenameFiles.indexes[classRenameFiles.index]].storageFile.RenameAsync(fileName, NameCollisionOption.GenerateUniqueName);
                                    }
                                    catch (Exception exception)
                                    {
                                        // Bei Falschen Namen
                                        if (exception.Message == "The filename, directory name, or volume label syntax is incorrect.\r\n" | exception.Message == "The parameter is incorrect.\r\n")
                                        {
                                            // Fehlermeldung
                                            dialogEx = new DialogEx();
                                            dialogEx.BorderBrush = scbXtrose;
                                            dialogEx.Title = resource.GetString("001_DateiNichtUmbenannt");
                                            dialogEx.Content = resource.GetString("001_NameFalsch");
                                            dQueryBtnSet = new DialogExButtonsSet();
                                            dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                                            dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                                            dQueryBtnSet.AddButton(dQueryBtn_1);
                                            dialogEx.AddButtonSet(dQueryBtnSet);
                                            // Eingabeaufförderung ausgeben
                                            dWaitAbort.Hide();
                                            await dialogEx.ShowAsync(grMain);

                                            // Umbennenen abbrechen
                                            break;
                                        }

                                        // Bei unbekanntem fehler
                                        else
                                        {
                                            // Wenn Fehlermeldung ausgegeben wird
                                            if (!classRenameFiles.allCantRenameFilesAndFolders)
                                            {
                                                // Fehlermeldung
                                                dialogEx = new DialogEx();
                                                dialogEx.BorderBrush = scbXtrose;
                                                dialogEx.Title = resource.GetString("001_DateiNichtUmbenannt");
                                                DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                                dialogEx.AddCheckbox(chBox);
                                                dQueryBtnSet = new DialogExButtonsSet();
                                                dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                                                dQueryBtn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                                dQueryBtnSet.AddButton(dQueryBtn_1);
                                                dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                                dialogEx.AddButtonSet(dQueryBtnSet);
                                                // Eingabeaufförderung ausgeben
                                                dWaitAbort.Hide();
                                                await dialogEx.ShowAsync(grMain);

                                                // Antwort auslesen
                                                answer = dialogEx.GetAnswer();

                                                // Wenn Antwort umbenennen
                                                if (answer == resource.GetString("001_Überspringen"))
                                                {
                                                    // Wenn für Alle
                                                    if (dialogEx.GetCheckboxByContent("001_FürAlle"))
                                                    {
                                                        classRenameFiles.allCantRenameFilesAndFolders = true;
                                                    }
                                                    // DialogEx ausgeben
                                                    dWaitAbort.Hide();
                                                }

                                                // Wenn Antwort abbrechen
                                                else if (answer == resource.GetString("001_Abbrechen"))
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    // Value erhöhen und Updaten
                                    classRenameFiles.value++;
                                    dWaitAbort.SetProgressBarValueByIndex(0, classRenameFiles.value);
                                }
                            }
                            // Wenn Task abgebrochen wurde
                            else
                            {
                                break;
                            }
                        }
                    }

                    // Wenn kein Name vorhanden
                    else
                    {
                        // Fehlermeldung
                        dialogEx = new DialogEx();
                        dialogEx.BorderBrush = scbXtrose;
                        dialogEx.Title = resource.GetString("001_KeinName");
                        dQueryBtnSet = new DialogExButtonsSet();
                        dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                        dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                        dQueryBtnSet.AddButton(dQueryBtn_1);
                        dialogEx.AddButtonSet(dQueryBtnSet);
                        // Eingabeaufförderung ausgeben
                        dWaitAbort.Hide();
                        await dialogEx.ShowAsync(grMain);
                    }

                    // Umbenennen Ausgabe entfernen
                    dWaitAbort.Hide();
                }
            }


            // Rückgabe
            return classRenameFiles;
        }
#endregion



#region Umbenennen One Drive
        // Task // Daten umbennen One Drive
        private async Task<ClassRenameFiles> mfiRenameOneDrive(ListBox listBox, ObservableCollection<ClassFiles> listFiles, ClassRenameFiles classRenameFiles, CancellationToken token)
        {
            // Liste der Items erstellen
            List<Item> listItems = new List<Item>();

            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Item hinzufüen
                listItems.Add(listFiles[mfSourceIndex].item);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;

                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Item hinzufügen
                    listItems.Add(listFiles[listBox.Items.IndexOf(item)].item);

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }
                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Item liste Leeren
                    listItems.Clear();

                    // Item hinzufüen
                    listItems.Add(listFiles[mfSourceIndex].item);
                }
            }

            // Wenn Dateien vorhanden
            if (listItems.Count() > 0)
            {
                // Lade Anzeige augeben
                DialogEx dWaitAbort = new DialogEx();
                dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
                DialogExProgressRing progressRing = new DialogExProgressRing();
                dWaitAbort.AddProgressRing(progressRing);
                dWaitAbort.ShowAsync(grMain);


                // Liste Dateien und Ordner
                List<int> listFilesFolders = new List<int>();
                listFilesFolders.Add(0);
                listFilesFolders.Add(0);

                // Dateien und Ordner zählen
                for (int i = 0; i < listItems.Count(); i++)
                {
                    // Wenn ein Ordner
                    if (listItems[i].Folder != null)
                    {
                        // Datei hinzufügen
                        listFilesFolders[1]++;
                    }
                    // Wenn eine Datei
                    else
                    {
                        // Ordner hinzufügen
                        listFilesFolders[0]++;
                    }
                }

                // Content erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Lade Anzeige entfernen
                dWaitAbort.Hide();
                // Nachricht ausgeben
                DialogEx dialogEx = new DialogEx();
                dialogEx.Title = resource.GetString("001_Umbenennen");
                dialogEx.Content = content;
                DialogExTextBox textBox_1 = new DialogExTextBox();
                textBox_1.Title = resource.GetString("001_Name");
                dialogEx.AddTextBox(textBox_1);
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Umbenennen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                DialogExButton dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                dQueryBtnSet.AddButton(dQueryBtn_2);
                dialogEx.AddButtonSet(dQueryBtnSet);
                await dialogEx.ShowAsync(grMain);

                // Antwort auslesen
                string answer = dialogEx.GetAnswer();

                // Wenn Antwort umbenennen
                if (answer == resource.GetString("001_Umbenennen"))
                {
                    // Ausgabe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = resource.GetString("001_Umbenennen");
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    tBlock.Margin = new Thickness(0, 5, 0, -8);
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Neuen Namen auslesen
                    string newName = dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Name"));

                    // Wenn Name vorhanden
                    if (newName != "")
                    {
                        // Versuchen Ordner lokal zu erstellen
                        try
                        {
                            // Ordner in Temp Verzeichnis erstellen und löschen
                            StorageFolder sf = await folderTemp.CreateFolderAsync(newName, CreationCollisionOption.ReplaceExisting);
                            await sf.DeleteAsync();

                            // Erweiterung
                            int ex = 0;

                            // Daten durchlaufen
                            for (int i = 0; i < listItems.Count(); i++)
                            {
                                // Dateityp auslesen
                                string fileEx = "";
                                if (listItems[i].File != null)
                                {
                                    string[] fileExSp = listItems[i].Name.Split(new char[] { '.' });
                                    if (fileExSp.Count() > 1)
                                    {
                                        fileEx = "." + fileExSp[fileExSp.Count() - 1];
                                    }
                                }

                                // Pfad erstellen
                                string path = listItems[i].ParentReference.Path;
                                path = Regex.Replace(path, ":", "/");
                                path = Regex.Replace(path, "/drive/root/", "");
                                string filePath = "";
                                string fileName = "";

                                // Wenn Erweiterung vorhanden
                                if (ex > 0)
                                {
                                    filePath = path + "/" + newName + " (" + ex + ")" + fileEx;
                                    fileName = newName + " (" + ex + ")" + fileEx;
                                }

                                // Wenn keine Erweiterung vorhanden
                                else
                                {
                                    filePath = path + "/" + newName +  fileEx;
                                    fileName = newName + fileEx; ;
                                }

                                // Ob Name bereits besteht
                                bool exists = true;

                                // Wenn abbrechen gedrück wurde
                                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                {
                                    cancelTokenAction.Cancel();
                                }

                                // Wenn Task noch nicht abgebrochen
                                if (!token.IsCancellationRequested)
                                {
                                    // Schleife bis Dateiname oder Ordnername gefunden
                                    while (exists)
                                    {
                                        // Wenn abbrechen gedrück wurde
                                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                        {
                                            cancelTokenAction.Cancel();
                                            exists = false;
                                        }

                                        try
                                        {
                                            // Versuchen Datei oder Ordner zu öffnen
                                            var builder = oneDriveClient.Drive.Root.ItemWithPath(filePath);
                                            var file = await builder.Request().GetAsync();

                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            filePath = path + "/" + newName + " (" + ex + ")" + fileEx;
                                            fileName = newName + " (" + ex + ")" + fileEx;
                                        }

                                        // Wenn Datei oder Ordner noch nicht existiert
                                        catch
                                        {
                                            // Datei umbenennen
                                            var updateItem = new Item
                                            {
                                                Name = fileName,
                                            };

                                            var itemWithUpdate = await oneDriveClient.Drive.Items[listItems[i].Id].Request().UpdateAsync(updateItem);

                                            // Erweiterung erhöhen
                                            ex++;

                                            // Angeben das umbenannt wurde
                                            exists = false;
                                        }
                                    }

                                    // Value erhöhen und Updaten
                                    classRenameFiles.value++;
                                    dWaitAbort.SetProgressBarValueByIndex(0, classRenameFiles.value);
                                }
                            }
                        }

                        // Falscher Name
                        catch
                        {
                            // Fehlermeldung
                            dialogEx = new DialogEx();
                            dialogEx.BorderBrush = scbXtrose;
                            dialogEx.Content = resource.GetString("001_NameFalsch");
                            dQueryBtnSet = new DialogExButtonsSet();
                            dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                            dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                            dQueryBtnSet.AddButton(dQueryBtn_1);
                            dialogEx.AddButtonSet(dQueryBtnSet);
                            // Eingabeaufförderung ausgeben
                            dWaitAbort.Hide();
                            await dialogEx.ShowAsync(grMain);
                        }
                    }

                    // Wenn kein Name vorhanden
                    else
                    {
                        // Fehlermeldung
                        dialogEx = new DialogEx();
                        dialogEx.BorderBrush = scbXtrose;
                        dialogEx.Title = resource.GetString("001_KeinName");
                        dQueryBtnSet = new DialogExButtonsSet();
                        dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                        dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                        dQueryBtnSet.AddButton(dQueryBtn_1);
                        dialogEx.AddButtonSet(dQueryBtnSet);
                        // Eingabeaufförderung ausgeben
                        dWaitAbort.Hide();
                        await dialogEx.ShowAsync(grMain);
                    }

                    // Umbenennen Ausgabe entfernen
                    dWaitAbort.Hide();
                }
            }

            // Rückgabe
            return classRenameFiles;
        }
#endregion



#region Umbenennen Dropbox
        // Task // Daten umbennen One Drive
        private async Task<ClassRenameFiles> mfiRenameDropbox(ListBox listBox, ObservableCollection<ClassFiles> listFiles, string dropboxToken, ClassRenameFiles classRenameFiles, CancellationToken token)
        {
            // Liste der Items erstellen
            List<Metadata> listItems = new List<Metadata>();

            // Einzelauswahl
            if (listBox.SelectionMode == SelectionMode.Single)
            {
                // Item hinzufüen
                listItems.Add(listFiles[mfSourceIndex].metadata);
            }

            // Mehrfachauswahl
            else
            {
                // Gibt an ob Flyout Item ausgewählt
                bool selected = false;

                // Indexes erstellen
                foreach (var item in listBox.SelectedItems)
                {
                    // Item hinzufügen
                    listItems.Add(listFiles[listBox.Items.IndexOf(item)].metadata);

                    // Prüfen ob Flyout item auch ausgewählt
                    if (listBox.Items.IndexOf(item) == mfSourceIndex & !selected)
                    {
                        selected = true;
                    }
                }
                // Wenn Flyout Item nicht ausgewählt
                if (!selected)
                {
                    // Item liste Leeren
                    listItems.Clear();

                    // Item hinzufüen
                    listItems.Add(listFiles[mfSourceIndex].metadata);
                }
            }

            // Wenn Dateien vorhanden
            if (listItems.Count() > 0)
            {
                // Lade Anzeige augeben
                DialogEx dWaitAbort = new DialogEx();
                dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
                DialogExProgressRing progressRing = new DialogExProgressRing();
                dWaitAbort.AddProgressRing(progressRing);
                dWaitAbort.ShowAsync(grMain);


                // Liste Dateien und Ordner
                List<int> listFilesFolders = new List<int>();
                listFilesFolders.Add(0);
                listFilesFolders.Add(0);

                // Dateien und Ordner zählen
                for (int i = 0; i < listItems.Count(); i++)
                {
                    // Wenn ein Ordner
                    if (listItems[i].IsFolder)
                    {
                        // Datei hinzufügen
                        listFilesFolders[1]++;
                    }
                    // Wenn eine Datei
                    else
                    {
                        // Ordner hinzufügen
                        listFilesFolders[0]++;
                    }
                }

                // Content erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Lade Anzeige entfernen
                dWaitAbort.Hide();
                // Nachricht ausgeben
                DialogEx dialogEx = new DialogEx();
                dialogEx.Title = resource.GetString("001_Umbenennen");
                dialogEx.Content = content;
                DialogExTextBox textBox_1 = new DialogExTextBox();
                textBox_1.Title = resource.GetString("001_Name");
                dialogEx.AddTextBox(textBox_1);
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Umbenennen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                DialogExButton dQueryBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                dQueryBtnSet.AddButton(dQueryBtn_2);
                dialogEx.AddButtonSet(dQueryBtnSet);
                await dialogEx.ShowAsync(grMain);

                // Antwort auslesen
                string answer = dialogEx.GetAnswer();

                // Wenn Antwort umbenennen
                if (answer == resource.GetString("001_Umbenennen"))
                {
                    // Ausgabe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = resource.GetString("001_Umbenennen");
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    tBlock.Margin = new Thickness(0, 5, 0, -8);
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Neuen Namen auslesen
                    string newName = dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Name"));

                    // Wenn Name vorhanden
                    if (newName != "")
                    {
                        // Versuchen Ordner lokal zu erstellen
                        try
                        {
                            // Ordner in Temp Verzeichnis erstellen und löschen
                            StorageFolder sf = await folderTemp.CreateFolderAsync(newName, CreationCollisionOption.ReplaceExisting);
                            await sf.DeleteAsync();

                            // Erweiterung
                            int ex = 0;

                            // Daten durchlaufen
                            for (int i = 0; i < listItems.Count(); i++)
                            {
                                // Pfad erstellen
                                string pathOld = listItems[i].PathDisplay;
                                string[] spPath = Regex.Split(pathOld, "/");
                                string path = "";
                                for ( int i2 = 0; i2 < spPath.Count() - 1; i2++)
                                {
                                    path += spPath[i2] + "/";
                                }
                                string name = spPath[spPath.Count() - 1];

                                // Dateityp auslesen
                                string fileEx = "";
                                if (listItems[i].IsFile)
                                {
                                    string[] fileExSp = name.Split(new char[] { '.' });
                                    if (fileExSp.Count() > 1)
                                    {
                                        fileEx = "." + fileExSp[fileExSp.Count() - 1];
                                    }
                                }

                                string pathNew = "";

                                // Wenn Erweiterung vorhanden
                                if (ex > 0)
                                {
                                    pathNew = path + newName + " (" + ex + ")" + fileEx;
                                }

                                // Wenn keine Erweiterung vorhanden
                                else
                                {
                                    pathNew = path + newName + fileEx;
                                }

                                // Ob Name bereits besteht
                                bool exists = true;

                                // Wenn abbrechen gedrück wurde
                                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                {
                                    cancelTokenAction.Cancel();
                                }

                                // Wenn Task noch nicht abgebrochen
                                if (!token.IsCancellationRequested)
                                {
                                    // Schleife bis Dateiname oder Ordnername gefunden
                                    while (exists)
                                    {
                                        // Wenn abbrechen gedrück wurde
                                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                        {
                                            cancelTokenAction.Cancel();
                                            exists = false;
                                        }

                                        try
                                        {
                                            // Versuchen Datei oder Ordner zu öffnen
                                            DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                            await dropboxClient.Files.GetMetadataAsync(pathNew);

                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            pathNew = path + newName + " (" + ex + ")" + fileEx;
                                        }

                                        // Wenn Datei oder Ordner noch nicht existiert
                                        catch
                                        {
                                            // Datei umbenennen
                                            DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                            await dropboxClient.Files.MoveAsync(new RelocationArg(pathOld, pathNew));

                                            // Erweiterung erhöhen
                                            ex++;

                                            // Angeben das umbenannt wurde
                                            exists = false;
                                        }
                                    }

                                    // Value erhöhen und Updaten
                                    classRenameFiles.value++;
                                    dWaitAbort.SetProgressBarValueByIndex(0, classRenameFiles.value);
                                }
                            }
                        }

                        // Falscher Name
                        catch
                        {
                            // Fehlermeldung
                            dialogEx = new DialogEx();
                            dialogEx.BorderBrush = scbXtrose;
                            dialogEx.Content = resource.GetString("001_NameFalsch");
                            dQueryBtnSet = new DialogExButtonsSet();
                            dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                            dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                            dQueryBtnSet.AddButton(dQueryBtn_1);
                            dialogEx.AddButtonSet(dQueryBtnSet);
                            // Eingabeaufförderung ausgeben
                            dWaitAbort.Hide();
                            await dialogEx.ShowAsync(grMain);
                        }
                    }

                    // Wenn kein Name vorhanden
                    else
                    {
                        // Fehlermeldung
                        dialogEx = new DialogEx();
                        dialogEx.BorderBrush = scbXtrose;
                        dialogEx.Title = resource.GetString("001_KeinName");
                        dQueryBtnSet = new DialogExButtonsSet();
                        dQueryBtnSet.HorizontalAlignment = HorizontalAlignment.Right;
                        dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                        dQueryBtnSet.AddButton(dQueryBtn_1);
                        dialogEx.AddButtonSet(dQueryBtnSet);
                        // Eingabeaufförderung ausgeben
                        dWaitAbort.Hide();
                        await dialogEx.ShowAsync(grMain);
                    }

                    // Umbenennen Ausgabe entfernen
                    dWaitAbort.Hide();
                }
            }

            // Rückgabe
            return classRenameFiles;
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button alles Auswählen
        // -----------------------------------------------------------------------
#region Button alles auswählen

        // Methode // Button Alles Auswählen
        private void MfiSelectAll_Click(object sender, RoutedEventArgs e)
        {
            // Links
            if (mfSource == "svLeft" | mfSource == "left")
            {
                // Wenn Multiselect
                if (lbLeft.SelectionMode == SelectionMode.Multiple)
                {
                    // Alles auswählen
                    lbLeft.SelectAll();

                    // Liste Durchlaufen und Auswahl erstellen
                    for (int i = 0; i < listFilesLeft.Count(); i++)
                    {
                        listFilesLeft[i].selectItem();
                    }
                }
            }

            // Rechts
            if (mfSource == "svRight" | mfSource == "right")
            {
                // Wenn Multiselect
                if (lbRight.SelectionMode == SelectionMode.Multiple)
                {
                    // Alles auswählen
                    lbRight.SelectAll();

                    // Liste Durchlaufen und Auswahl erstellen
                    for (int i = 0; i < listFilesRight.Count(); i++)
                    {
                        listFilesRight[i].selectItem();
                    }
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button an Start anheften
        // -----------------------------------------------------------------------
#region Button An Start anheften

        // Methode // An Start anheften
        private void MfiPinToStart_Click(object sender, RoutedEventArgs e)
        {
            // Prüfen ob Ordner schon vorhanden
            string[] sp = Regex.Split(setStartMenu, "~~");
            for (int i = 0; i < sp.Count(); i++)
            {
                string[] spSp = Regex.Split(sp[i], "~");

                // Wenn StorageFolder
                if(spSp[0] == "StorageFolder")
                {

                }
            }
            
        }
#endregion
        // -----------------------------------------------------------------------






        // Flyout Menü // Button alles Abwählen
        // -----------------------------------------------------------------------
#region Button alles abwählen
        private async void MfiUnselectAll_Click(object sender, RoutedEventArgs e)
        {
            // Links
            if (mfSource == "svLeft" | mfSource == "left")
            {
                // Wenn Multiselect
                if (lbLeft.SelectionMode == SelectionMode.Multiple)
                {
                    // Alles abwählen
                    try
                    {
                        lbLeft.SelectedItem = -1;
                    }
                    catch
                    { }

                    // Liste Durchlaufen und Auswahl erstellen
                    for (int i = 0; i < listFilesLeft.Count(); i++)
                    {
                        listFilesLeft[i].unselectItem();
                    }
                }
            }

            // Rechts
            if (mfSource == "svRight" | mfSource == "right")
            {
                // Wenn Multiselect
                if (lbRight.SelectionMode == SelectionMode.Multiple)
                {
                    // Alles abwählen
                    try
                    {
                        lbRight.SelectedItem = -1;
                    }
                    catch
                    { }

                    // Liste Durchlaufen und Auswahl erstellen
                    for (int i = 0; i < listFilesRight.Count(); i++)
                    {
                        listFilesRight[i].unselectItem();
                    }
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button entfernen
        // -----------------------------------------------------------------------
#region Button entfernen

        private async void MfiRemove_Click(object sender, RoutedEventArgs e)
        {
            // Links
            if (mfSource == "left")
            {
                // Wenn One Drive
                if (listFilesLeft[mfSourceIndex].driveType == "oneDrive")
                {
                    // One Drive entfernen
                    await removeStartEntry("oneDrive");
                }

                // Wenn Dropbox
                else if (listFilesLeft[mfSourceIndex].driveType == "dropbox")
                {
                    // Dropbox entfernen
                    await removeStartEntry("dropbox", listFolderStart[mfSourceIndex].token);
                }

                // Wenn File Picker
                else if (listFilesLeft[mfSourceIndex].driveType == "filePicker")
                {
                    // File Picker entfernen
                    await removeStartEntry("filePicker");
                }

                // Fenster erneuern
                outputRefresh("left");
            }

            // Rechts
            else if (mfSource == "right")
            {
                // Wenn One Drive
                if (listFilesRight[mfSourceIndex].driveType == "oneDrive")
                {
                    // One Drive entfernen
                    await removeStartEntry("oneDrive");
                }

                // Wenn Dropbox
                else if (listFilesRight[mfSourceIndex].driveType == "dropbox")
                {
                    // Dropbox entfernen
                    await removeStartEntry("dropbox", listFolderStart[mfSourceIndex].token);
                }

                // Wenn File Picker
                else if (listFilesRight[mfSourceIndex].driveType == "filePicker")
                {
                    // File Picker entfernen
                    await removeStartEntry("filePicker");
                }

                // Fenster erneuern
                outputRefresh("right");
            }
        }



        // Starteinträge entfernen
        private async Task removeStartEntry(string entry, string string1 = null)
        {
            // Start Menü splitten
            string[] spStart = Regex.Split(setStartMenu, ";;;");

            // Start Menü leeren
            setStartMenu = "";

            // Einträge durchlaufen
            for (int i = 0; i < spStart.Count(); i++)
            {
                // Gibt an ob gelöscht wird
                bool delete = false;

                // Einträge Splitten
                string[] spSpStart = Regex.Split(spStart[i], ";");

                // Wenn kein Eintrag und kein One Drive oder File Picker
                if (spSpStart.Count() == 1 & spSpStart[0] == "")
                {
                    delete = true;
                }

                // Wenn One Drive
                else if (spSpStart[0] == "oneDrive" & entry == "oneDrive")
                {
                    delete = true;
                }

                // Wenn Dropbox
                else if(spSpStart[0] == "dropbox" & entry == "dropbox")
                {
                    if (spSpStart[2] == string1)
                    {
                        delete = true;
                    }
                }

                // Wenn File Picker
                else if (spSpStart[0] == "filePicker" & entry == "filePicker")
                {
                    delete = true;
                }

                // Wenn Eintrag bestehen bleibt
                if (!delete)
                {
                    setStartMenu += spStart[i] + ";;;";
                }
            }

            // Start Menü speichern
            StorageFile storageFile = await folderSettings.CreateFileAsync("StartMenu.txt", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(storageFile, setStartMenu);

            // Strart Menü neu erstellen
            await createStartFolder();
        }
#endregion
        // -----------------------------------------------------------------------





        // Flyout Menü // Button abmelden
        // -----------------------------------------------------------------------
#region Button abmelden

        // Methode // Button abmelden
        private async void MfiSignOut_Click(object sender, RoutedEventArgs e)
        {
            // Links
            if (mfSource == "left")
            {
                // Wenn One Drive
                if (listFilesLeft[mfSourceIndex].driveType == "oneDrive")
                {
                    // One Drive abmelden
                    await OneDriveSignOut();
                }
            }

            // Rechts
            else if (mfSource == "right")
            {
                // Wenn One Drive
                if (listFilesRight[mfSourceIndex].driveType == "oneDrive")
                {
                    // One Drive abmelden
                    await OneDriveSignOut();
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------
#endregion










#region Kopieren und Verschieben
        // Lokal --> Lokal
        // -----------------------------------------------------------------------
#region Lokal --> Lokal 

        // Task // Kopieren und Verschieben // Lokal --> Lokal
        private async Task<ClassCopyOrMoveFilesAndFolders> acCopyOrMoveLokalToLokal(List<StorageFile> listFiles, List<StorageFolder> listFolders, StorageFolder targetFolder, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, CancellationToken token)
        {
            // Lade Anzeige augeben
            DialogEx dWaitAbort = new DialogEx();
            DialogEx dQuery = new DialogEx();
            dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
            DialogExProgressRing progressRing = new DialogExProgressRing();
            dWaitAbort.AddProgressRing(progressRing);
            dWaitAbort.ShowAsync(grMain);

            // Parameter Dateien und Ordner setzen
            List<int> listFilesFolders = new List<int>();
            listFilesFolders.Add(listFiles.Count());
            listFilesFolders.Add(0);

            // Ordner zählen
            for (int i = 0; i < listFolders.Count(); i++)
            {
                // Ordner hinzufügen
                listFilesFolders[1]++;
                // Datein in Ordner und Unterordner zählen // Task
                listFilesFolders = await acGetFilesAndFoldersLokal(listFolders[i], listFilesFolders);
            }

            // Wenn Dateien vorhanden
            if (listFilesFolders[0] + listFilesFolders[1] > 0)
            {
                // Ausgabe erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Abfrage bei Drag and Drop
                if (classCopyOrMoveFilesAndFolders.dragAndDrop)
                {
                    // Wenn Drag and Drop Aktion // ask
                    if (setDdCopyOrMove == "ask")
                    {
                        // Nachricht ausgeben
                        dQuery = new DialogEx();
                        dQuery.BorderBrush = scbXtrose;
                        dQuery.Title = resource.GetString("001_KopierenVerschieben");
                        dQuery.Content = content;
                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Kopieren"));
                        btnSet.AddButton(btn_1);
                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Verschieben"));
                        btnSet.AddButton(btn_2);
                        DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        btnSet.AddButton(btn_3);
                        dQuery.AddButtonSet(btnSet);
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn kopieren
                        if (answer == resource.GetString("001_Kopieren"))
                        {
                            // Auf kopieren stellen
                            classCopyOrMoveFilesAndFolders.move = false;
                        }
                        // Wenn verschieben
                        else if (answer == resource.GetString("001_Verschieben"))
                        {
                            // Auf verschieben stellen
                            classCopyOrMoveFilesAndFolders.move = true;
                        }
                    }

                    // Wenn Drag and Drop Aktion // move // copy // standard
                    else
                    {
                        // Bestätigung erstellen
                        if (setDdConfirmCopyOrMove == "always" | (setDdConfirmCopyOrMove == "several" & ddListSourceIndexes.Count() > 1))
                        {
                            // Title erstellen 
                            string title = resource.GetString("001_Kopieren");

                            if (classCopyOrMoveFilesAndFolders.move)
                            {
                                title = resource.GetString("001_Verschieben");
                            }

                            // Nachricht ausgeben // Bestätigung löschen
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = title;
                            dQuery.Content = content;
                            DialogExButtonsSet dialogExButtonsSet = new DialogExButtonsSet();
                            dialogExButtonsSet.HorizontalAlignment = HorizontalAlignment.Right;
                            DialogExButton button_1 = new DialogExButton(title);
                            dialogExButtonsSet.AddButton(button_1);
                            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dialogExButtonsSet.AddButton(button_2);
                            dQuery.AddButtonSet(dialogExButtonsSet);
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                        }
                    }
                }

                // Wenn nicht abgebrochen
                if (dQuery.GetAnswer() != resource.GetString("001_Abbrechen"))
                {
                    // Title erstellen 
                    string title = resource.GetString("001_Kopieren");

                    if (classCopyOrMoveFilesAndFolders.move)
                    {
                        title = resource.GetString("001_Verschieben");
                    }

                    // Lade Anzeige entfernen
                    dWaitAbort.Hide();

                    // Einfügen Ausgabe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = title;
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    tBlock.Margin = new Thickness(0, 8, 0, -8);
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien einfügen
                    for (int i = 0; i < listFiles.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Versuchen Datei zu kopieren
                            try
                            {
                                // DialogEx updaten
                                dWaitAbort.SetTextBoxTextByIndex(0, listFiles[i].Name);

                                // Wenn Kopieren
                                if (!classCopyOrMoveFilesAndFolders.move)
                                {
                                    // Datei kopieren
                                    await listFiles[i].CopyAsync(targetFolder, listFiles[i].Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles);
                                }

                                // Wenn ausschneiden
                                else
                                {
                                    // Datei verschieben
                                    await listFiles[i].MoveAsync(targetFolder, listFiles[i].Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles);
                                }
                            }

                            // Wenn Datei nicht Eingefügt werden konnte
                            catch (Exception e)
                            {
                                // Wenn Datei bereits vorhanden
                                if (e.HResult == -2147024713)
                                {
                                    // Wenn Nachricht ausgegeben wird
                                    if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles)
                                    {
                                        // Nachricht ausgeben
                                        dQuery = new DialogEx();
                                        dQuery.BorderBrush = scbXtrose;
                                        dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                        dQuery.Content = listFiles[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        btnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                        btnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(btnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Nachricht auswerten
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                        }

                                        // Bei umbenennen
                                        else if (answer == resource.GetString("001_Umbenennen"))
                                        {
                                            // Wenn Kopieren
                                            try
                                            {
                                                if (!classCopyOrMoveFilesAndFolders.move)
                                                {
                                                    // Datei kopieren
                                                    await listFiles[i].CopyAsync(targetFolder, listFiles[i].Name, NameCollisionOption.GenerateUniqueName);
                                                }

                                                // Wenn ausschneiden
                                                else
                                                {
                                                    // Datei verschieben
                                                    await listFiles[i].MoveAsync(targetFolder, listFiles[i].Name, NameCollisionOption.GenerateUniqueName);
                                                }
                                            }
                                            catch
                                            { }

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                            }
                                        }
                                    }
                                }
                                // Wenn Datei nicht mehr vorhanden
                                else if (e.HResult == -2147024894)
                                {
                                    // Wenn Nachricht ausgegeben wird
                                    if (!classCopyOrMoveFilesAndFolders.allFileNotFound)
                                    {
                                        // Nachricht ausgeben
                                        dQuery = new DialogEx();
                                        dQuery.BorderBrush = scbXtrose;
                                        dQuery.Title = resource.GetString("001_DateiNichtVorhanden");
                                        dQuery.Content = listFiles[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                        btnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        btnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(btnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Nachricht auswerten
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allFileNotFound = true;
                                        }

                                        // Wenn Abbrechen
                                        if (answer == resource.GetString("001_Abbrechen"))
                                        {
                                            // Task abbrechen
                                            cancelTokenAction.Cancel();
                                        }
                                    }
                                }
                                // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                {
                                    // Nachricht erstellen
                                    dQuery = new DialogEx();
                                    dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                    dQuery.Content = listFiles[i].Name;
                                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                    dQuery.AddCheckbox(chBox);
                                    DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                    DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                    dQueryBtnSet.AddButton(btn_1);
                                    DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                    dQueryBtnSet.AddButton(btn_2);
                                    dQuery.AddButtonSet(dQueryBtnSet);
                                    // Neue Ausgabe erstellen
                                    dWaitAbort.Hide();
                                    await dQuery.ShowAsync(grMain);
                                    dWaitAbort.ShowAsync(grMain);

                                    // Antwort auslesen
                                    string answer = dQuery.GetAnswer();

                                    // Wenn für alle weiteren ausgeführt wird
                                    if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                    {
                                        classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                    }

                                    // Wenn Abbrechen
                                    if (answer == resource.GetString("001_Abbrechen"))
                                    {
                                        // Task abbrechen
                                        cancelTokenAction.Cancel();
                                    }
                                }
                            }
                            // Value erhöhen und Updaten
                            classCopyOrMoveFilesAndFolders.value++;
                            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }

                    // Ordner Durchlaufen
                    for (int i = 0; i < listFolders.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                            }
                            // Ordner mit allen Dateien einfügen // Task
                            classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesLokalToLokal(listFolders[i], targetFolder, classCopyOrMoveFilesAndFolders, dWaitAbort, token);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Wenn keine Dateien mehr vorhanden
            else
            {
                // Nachricht ausgeben
                dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Einfügen");
                dQuery.Content = resource.GetString("001_DateienNichtVorhanden");
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                dQuery.AddButtonSet(dQueryBtnSet);
                dWaitAbort.Hide();
                await dQuery.ShowAsync(grMain);
            }

            // Ausgabe entfernen
            dWaitAbort.Hide();

            // Drag and Drop beenden
            dd = false;

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }



        // Task // Ordner mit allen Dateien einfügen // Lokal
        private async Task<ClassCopyOrMoveFilesAndFolders> mfPasteFolderAndFilesLokalToLokal(StorageFolder sourceFolder, StorageFolder targetFolder, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, DialogEx dWaitAbort, CancellationToken token)
        {
            // Parameter
            DialogEx dQuery = new DialogEx();
            IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
            IReadOnlyList<StorageFile> fileList = new List<StorageFile>();
            StorageFolder newFolder = null;
            bool skip = false;

            // Versuchen Dateien auszulesen
            try
            {
                // Dateien auslesen
                folderList = await sourceFolder.GetFoldersAsync();
                fileList = await sourceFolder.GetFilesAsync();

                // Versuchen Ordner zu erstellen
                try
                {
                    // Odner erstellen
                    newFolder = await targetFolder.CreateFolderAsync(sourceFolder.Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes);
                }
                // Wenn Ordner nicht erstellt werden kann
                catch (Exception e)
                {
                    // Wenn Ordner bereits vorhanden
                    if (e.HResult == -2147024713)
                    {
                        if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders)
                        {
                            // Nachricht erstellen
                            dQuery = new DialogEx();
                            dQuery.Title = resource.GetString("001_OrdnerBereitsVorhanden");
                            dQuery.Content = sourceFolder.Name;
                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                            dQuery.AddCheckbox(chBox);
                            DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dQueryBtnSet.AddButton(btn_1);
                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                            dQueryBtnSet.AddButton(btn_2);
                            DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Integrieren"));
                            dQueryBtnSet.AddButton(btn_3);
                            dQuery.AddButtonSet(dQueryBtnSet);
                            // Neue Ausgabe erstellen
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                            dWaitAbort.ShowAsync(grMain);

                            // Antwort auslesen
                            string answer = dQuery.GetAnswer();

                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders = true;
                            }

                            // Wenn Integrieren
                            if (answer == resource.GetString("001_Integrieren"))
                            {
                                classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes = CreationCollisionOption.OpenIfExists;
                                await mfPasteFolderAndFilesLokalToLokal(sourceFolder, targetFolder, classCopyOrMoveFilesAndFolders, dWaitAbort, token);
                                skip = true;
                            }

                            // Bei überspringen
                            if (answer == resource.GetString("001_Überspringen"))
                            {
                                skip = true;
                            }

                            // Wenn Abbrechen
                            if (answer == resource.GetString("001_Abbrechen"))
                            {
                                // Task abbrechen
                                cancelTokenAction.Cancel();
                            }
                        }
                    }
                }
                // Wenn Dateien eingefügt werden
                if (!skip)
                {
                    // Dateien durchlaufen
                    for (int i = 0; i < fileList.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                            }

                            // Versuchen Datei zu kopieren
                            try
                            {
                                // DialogEx updaten
                                dWaitAbort.SetTextBoxTextByIndex(0, fileList[i].Name);

                                // Wenn Kopieren
                                if (!classCopyOrMoveFilesAndFolders.move)
                                {
                                    // Datei kopieren
                                    await fileList[i].CopyAsync(newFolder, fileList[i].Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles);
                                }
                                // Wenn ausschneiden
                                else
                                {
                                    // Datei verschieben
                                    await fileList[i].MoveAsync(newFolder, fileList[i].Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles);
                                }
                            }
                            // Wenn Datei nicht Eingefügt werden konnte
                            catch (Exception e)
                            {
                                // Wenn Datei bereits vorhanden
                                if (e.HResult == -2147024713)
                                {
                                    // Wenn Nachricht ausgegeben wird
                                    if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles)
                                    {
                                        // Nachricht ausgeben
                                        dQuery = new DialogEx();
                                        dQuery.BorderBrush = scbXtrose;
                                        dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                        dQuery.Content = fileList[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        btnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                        btnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(btnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Nachricht auswerten
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                        }

                                        // Bei umbenennen
                                        else if (answer == resource.GetString("001_Umbenennen"))
                                        {
                                            // Wenn Kopieren
                                            try
                                            {
                                                if (!classCopyOrMoveFilesAndFolders.move)
                                                {
                                                    // Datei kopieren
                                                    await fileList[i].CopyAsync(newFolder, fileList[i].Name, NameCollisionOption.GenerateUniqueName);
                                                }
                                                // Wenn ausschneiden
                                                else
                                                {
                                                    // Datei verschieben
                                                    await fileList[i].MoveAsync(newFolder, fileList[i].Name, NameCollisionOption.GenerateUniqueName);
                                                }
                                            }
                                            catch
                                            { }

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                            }
                                        }
                                    }
                                }
                                // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                {
                                    // Nachricht erstellen
                                    dQuery = new DialogEx();
                                    dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                    dQuery.Content = fileList[i].Name;
                                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                    dQuery.AddCheckbox(chBox);
                                    DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                    DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                    dQueryBtnSet.AddButton(btn_1);
                                    DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                    dQueryBtnSet.AddButton(btn_2);
                                    dQuery.AddButtonSet(dQueryBtnSet);
                                    // Neue Ausgabe erstellen
                                    dWaitAbort.Hide();
                                    await dQuery.ShowAsync(grMain);
                                    dWaitAbort.ShowAsync(grMain);

                                    // Antwort auslesen
                                    string answer = dQuery.GetAnswer();

                                    // Wenn für alle weiteren ausgeführt wird
                                    if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                    {
                                        classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                    }

                                    // Wenn Abbrechen
                                    if (answer == resource.GetString("001_Abbrechen"))
                                    {
                                        // Task abbrechen
                                        cancelTokenAction.Cancel();
                                    }
                                }
                            }
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                        // Value erhöhen und Updaten
                        classCopyOrMoveFilesAndFolders.value++;
                        dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                    }
                }

                // Ordner durchlaufen
                for (int i = 0; i < folderList.Count(); i++)
                {
                    // Wenn abbrechen gedrück wurde
                    if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                    {
                        cancelTokenAction.Cancel();
                    }

                    // Wenn Task noch nicht abgebrochen
                    if (!token.IsCancellationRequested)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }
                        // Ordner mit allen Dateien einfügen // Task
                        classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesLokalToLokal(folderList[i], newFolder, classCopyOrMoveFilesAndFolders, dWaitAbort, token);
                    }
                    // Wenn Task abgebrochen
                    else
                    {
                        break;
                    }
                }

                // Bei ausschneiden
                if (classCopyOrMoveFilesAndFolders.move & !token.IsCancellationRequested)
                {
                    // Versuchen Ordner zu löschen
                    try
                    {
                        // Ordner löschen
                        await sourceFolder.DeleteAsync();
                    }
                    catch
                    {
                        // Wenn Nachricht ausgegeben wird
                        if (!classCopyOrMoveFilesAndFolders.allCantDeleteFolders)
                        {
                            // Nachricht ausgeben
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = resource.GetString("001_OrdnerNichtLöschen");
                            dQuery.Content = sourceFolder.Name;
                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                            dQuery.AddCheckbox(chBox);
                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            btnSet.AddButton(btn_1);
                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                            btnSet.AddButton(btn_2);
                            dQuery.AddButtonSet(btnSet);
                            // Neue Ausgabe erstellen
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                            dWaitAbort.ShowAsync(grMain);

                            // Antwort auslesen
                            string answer = dQuery.GetAnswer();

                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.allCantDeleteFolders = true;
                            }

                            // Wenn Abbrechen
                            if (answer == resource.GetString("001_Abbrechen"))
                            {
                                // Task abbrechen
                                cancelTokenAction.Cancel();
                            }
                        }
                    }
                }
            }

            // Wenn Dateien nicht ausgelesen werden können
            catch
            {
                // Wenn Nachricht ausgegeben wird
                if (!classCopyOrMoveFilesAndFolders.allCantOpenFolder)
                {
                    // Nachricht ausgeben
                    dQuery = new DialogEx();
                    dQuery.BorderBrush = scbXtrose;
                    dQuery.Title = resource.GetString("001_OrdnerNichÖffnen");
                    dQuery.Content = sourceFolder.Name;
                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                    dQuery.AddCheckbox(chBox);
                    DialogExButtonsSet btnSet = new DialogExButtonsSet();
                    DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    btnSet.AddButton(btn_1);
                    DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                    btnSet.AddButton(btn_2);
                    dQuery.AddButtonSet(btnSet);
                    // Neue Ausgabe erstellen
                    dWaitAbort.Hide();
                    await dQuery.ShowAsync(grMain);
                    dWaitAbort.ShowAsync(grMain);

                    // Nachricht auswerten
                    string answer = dQuery.GetAnswer();

                    // Wenn für alle weiteren ausgeführt wird
                    if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                    {
                        classCopyOrMoveFilesAndFolders.allCantOpenFolder = true;
                    }

                    // Wenn Abbrechen
                    if (answer == resource.GetString("001_Abbrechen"))
                    {
                        // Task abbrechen
                        cancelTokenAction.Cancel();
                    }
                }
            }
            // Value erhöhen und Updaten
            classCopyOrMoveFilesAndFolders.value++;
            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }
#endregion
        // -----------------------------------------------------------------------





        // One Drive --> Lokal
        // -----------------------------------------------------------------------
#region One Drive --> Lokal

        // Task // Kopieren und Verschieben // One Drive --> Lokal
        private async Task<ClassCopyOrMoveFilesAndFolders> acCopyOrMoveOneDriveToLokal(List<Item> listItems, StorageFolder targetFolder, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, CancellationToken token)
        {
            // Lade Anzeige augeben
            DialogEx dWaitAbort = new DialogEx();
            DialogEx dQuery = new DialogEx();
            dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
            DialogExProgressRing progressRing = new DialogExProgressRing();
            dWaitAbort.AddProgressRing(progressRing);
            dWaitAbort.ShowAsync(grMain);

            // Parameter Dateien und Ordner setzen
            List<int> listFilesFolders = new List<int>();
            listFilesFolders.Add(0);
            listFilesFolders.Add(0);

            // Items durchlaufen
            for (int i = 0; i < listItems.Count(); i++)
            {
                // Wenn Ordner
                if (listItems[i].Folder != null)
                {
                    // Ordner hinzufügen
                    listFilesFolders[1]++;

                    // Datein in Ordner und Unterordner zählen // Task
                    listFilesFolders = await acGetFilesAndFoldersOneDrive(listItems[i], listFilesFolders);
                }
                // Wenn Datei
                else
                {
                    listFilesFolders[0]++;
                }
            }

            // Wenn Dateien vorhanden
            if (listFilesFolders[0] + listFilesFolders[1] > 0)
            {
                // Content erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Abfrage bei Drag and Drop
                if (classCopyOrMoveFilesAndFolders.dragAndDrop)
                {
                    // Wenn Drag and Drop Aktion // ask
                    if (setDdCopyOrMove == "ask")
                    {
                        // Nachricht ausgeben
                        dQuery = new DialogEx();
                        dQuery.BorderBrush = scbXtrose;
                        dQuery.Title = resource.GetString("001_KopierenVerschieben");
                        dQuery.Content = content;
                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Kopieren"));
                        btnSet.AddButton(btn_1);
                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Verschieben"));
                        btnSet.AddButton(btn_2);
                        DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        btnSet.AddButton(btn_3);
                        dQuery.AddButtonSet(btnSet);
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn kopieren
                        if (answer == resource.GetString("001_Kopieren"))
                        {
                            // Auf kopieren stellen
                            classCopyOrMoveFilesAndFolders.move = false;
                        }
                        // Wenn verschieben
                        else if (answer == resource.GetString("001_Verschieben"))
                        {
                            // Auf verschieben stellen
                            classCopyOrMoveFilesAndFolders.move = true;
                        }
                    }

                    // Wenn Drag and Drop Aktion // move // copy // standard
                    else
                    {
                        // Bestätigung erstellen
                        if (setDdConfirmCopyOrMove == "always" | (setDdConfirmCopyOrMove == "several" & ddListSourceIndexes.Count() > 1))
                        {
                            // Title erstellen 
                            string title = resource.GetString("001_Kopieren");

                            if (classCopyOrMoveFilesAndFolders.move)
                            {
                                title = resource.GetString("001_Verschieben");
                            }

                            // Nachricht ausgeben // Bestätigung löschen
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = title;
                            dQuery.Content = content;
                            DialogExButtonsSet dialogExButtonsSet = new DialogExButtonsSet();
                            dialogExButtonsSet.HorizontalAlignment = HorizontalAlignment.Right;
                            DialogExButton button_1 = new DialogExButton(title);
                            dialogExButtonsSet.AddButton(button_1);
                            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dialogExButtonsSet.AddButton(button_2);
                            dQuery.AddButtonSet(dialogExButtonsSet);
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                        }
                    }
                }

                // Wenn nicht abgebrochen
                if (dQuery.GetAnswer() != resource.GetString("001_Abbrechen"))
                {
                    // Title erstellen 
                    string title = resource.GetString("001_Kopieren");

                    if (classCopyOrMoveFilesAndFolders.move)
                    {
                        title = resource.GetString("001_Verschieben");
                    }

                    // Lade Anzeige entfernen
                    dWaitAbort.Hide();

                    // Einfügen Ausgabe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = title;
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien und Ordner einfügen
                    for (int i = 0; i < listItems.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn eine Datei
                            if (listItems[i].File != null)
                            {
                                // DialogEx updaten
                                dWaitAbort.SetTextBoxTextByIndex(0, listItems[i].Name);

                                // Versuchen Datei zu erstellen
                                StorageFile file = null;
                                try
                                {
                                    // Datei erstellen
                                    file = await folderTemp.CreateFileAsync(listItems[i].Name, CreationCollisionOption.ReplaceExisting);
                                    var streamFile = await file.OpenAsync(FileAccessMode.ReadWrite);

                                    // Pfad erstellen
                                    string path = listItems[i].ParentReference.Path;
                                    path = Regex.Replace(path, ":", "/");
                                    path += "/" + listItems[i].Name;
                                    path = Regex.Replace(path, "/drive/root/", "");

                                    // Datei herunterladen
                                    var builder = oneDriveClient.Drive.Root.ItemWithPath(path);

                                    // Filestream erstellen
                                    using (Stream fileStream = await builder.Content.Request().GetAsync())
                                    {
                                        // BinaryReader erstellen
                                        using (BinaryReader binaryReader = new BinaryReader(fileStream))
                                        {
                                            // Puffer erstellen
                                            byte[] buffer = new byte[setBufferSize];
                                            int count;
                                            int totalBytes = 0;

                                            // Schleife Download
                                            while ((count = binaryReader.Read(buffer, 0, setBufferSize)) > 0)
                                            {
                                                // Wenn Abbruch durch Benutzer
                                                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                                {
                                                    // Versuchen Datei zu löschen
                                                    try
                                                    {
                                                        await file.DeleteAsync();
                                                    }
                                                    catch
                                                    { }
                                                    break;
                                                }

                                                // Datei als Stream herunterladen
                                                await streamFile.WriteAsync(buffer.AsBuffer());
                                                totalBytes += count;
                                            }
                                        }
                                    }

                                    // Wenn nich abgebrochen wurde
                                    if (dWaitAbort.GetAnswer() != resource.GetString("001_Abbrechen"))
                                    {
                                        // Datei in targetFolder verschieben
                                        await file.MoveAsync(targetFolder, file.Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles);
                                    }

                                    // Wenn verschieben und nicht abgebrochen
                                    if (classCopyOrMoveFilesAndFolders.move & dWaitAbort.GetAnswer() != resource.GetString("001_Abbrechen"))
                                    {
                                        // Datei auf One Drive löschen
                                        await oneDriveClient.Drive.Items[listItems[i].Id].Request().DeleteAsync();
                                    }
                                }

                                // Wenn Datei nicht erstellt werden konnte
                                catch (Exception e)
                                {
                                    // Wenn Datei bereits vorhanden
                                    if (e.HResult == -2147024713)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                            dQuery.Content = listItems[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                            }

                                            // Bei umbenennen
                                            if (answer == resource.GetString("001_Umbenennen"))
                                            {
                                                // Datei in targetFolder verschieben
                                                await file.MoveAsync(targetFolder, file.Name, NameCollisionOption.GenerateUniqueName);

                                                // Wenn für alle weiteren ausgeführt wird
                                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                                {
                                                    classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                                }
                                            }
                                        }
                                    }
                                    // Wenn Datei nicht mehr vorhanden
                                    else if (e.HResult == -2147024894)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allFileNotFound)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiNichtVorhanden");
                                            dQuery.Content = listItems[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allFileNotFound = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }
                                    // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                    else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                    {
                                        // Nachricht erstellen
                                        dQuery = new DialogEx();
                                        dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                        dQuery.Content = listItems[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                        dQueryBtnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        dQueryBtnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(dQueryBtnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Antwort auslesen
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                        }

                                        // Wenn Abbrechen
                                        if (answer == resource.GetString("001_Abbrechen"))
                                        {
                                            // Task abbrechen
                                            cancelTokenAction.Cancel();
                                        }
                                    }
                                }
                            }

                            // Wenn ein Ordner
                            else
                            {
                                // Fehler zurücksetzen
                                classCopyOrMoveFilesAndFolders.errors = false;

                                // Ordner mit allen Dateien einfügen // Task
                                classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesOneDriveToLokal(listItems[i], targetFolder, classCopyOrMoveFilesAndFolders, dWaitAbort, token);

                                // Wenn verschieben und nicht abgebrochen
                                if (classCopyOrMoveFilesAndFolders.move & dWaitAbort.GetAnswer() != resource.GetString("001_Abbrechen") & !classCopyOrMoveFilesAndFolders.errors)
                                {
                                    // Datei auf One Drive löschen
                                    await oneDriveClient.Drive.Items[listItems[i].Id].Request().DeleteAsync();
                                }
                            }

                            // Value erhöhen und Updaten
                            classCopyOrMoveFilesAndFolders.value++;
                            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Wenn keine Dateien vorhanden
            else
            {
                // Nachricht ausgeben
                dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Einfügen");
                dQuery.Content = resource.GetString("001_DateienNichtVorhanden");
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                dQuery.AddButtonSet(dQueryBtnSet);
                dWaitAbort.Hide();
                await dQuery.ShowAsync(grMain);
            }

            // Ausgabe entfernen
            dWaitAbort.Hide();

            // Drag and Drop beenden
            dd = false;

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }



        // Task // Ordner mit allen Dateien einfügen // One Drive zu Lokal
        private async Task<ClassCopyOrMoveFilesAndFolders> mfPasteFolderAndFilesOneDriveToLokal(Item sourceItem, StorageFolder targetFolder, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, DialogEx dWaitAbort, CancellationToken token)
        {
            // Parameter
            DialogEx dQuery = new DialogEx();
            StorageFolder newFolder = null;
            bool skip = false;

            // Gibt an ob was geladen wurde
            bool connect = true;

            // Pfad erstellen
            string path = sourceItem.ParentReference.Path;

            // Wenn Pfad vorhanden
            if (path != null)
            {
                path = Regex.Replace(path, ":", "/");
                path += "/" + sourceItem.Name;
                path = Regex.Replace(path, "/drive/root/", "");
            }

            // IChildrenCollectionPage erstellen
            IChildrenCollectionPage items = null;

            // Versuchen Daten zu laden
            try
            {
                // Ordner und Dateien laden // Root
                if (path == null)
                {
                    items = await oneDriveClient.Drive.Root.Children.Request().GetAsync();
                }

                // Ordner und Dateien laden // Nach Pfad
                else
                {
                    items = await oneDriveClient.Drive.Root.ItemWithPath(path).Children.Request().GetAsync();
                }

                // Versuchen Ordner zu erstellen
                try
                {
                    // Odner erstellen
                    newFolder = await targetFolder.CreateFolderAsync(sourceItem.Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes);
                }

                // Wenn Ordner nicht erstellt werden konnte
                catch (Exception e)
                {
                    // Wenn Ordner bereits vorhanden
                    if (e.HResult == -2147024713)
                    {
                        if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders)
                        {
                            // Nachricht erstellen
                            dQuery = new DialogEx();
                            dQuery.Title = resource.GetString("001_OrdnerBereitsVorhanden");
                            dQuery.Content = sourceItem.Name;
                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                            dQuery.AddCheckbox(chBox);
                            DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dQueryBtnSet.AddButton(btn_1);
                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                            dQueryBtnSet.AddButton(btn_2);
                            DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Integrieren"));
                            dQueryBtnSet.AddButton(btn_3);
                            dQuery.AddButtonSet(dQueryBtnSet);
                            // Neue Ausgabe erstellen
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                            dWaitAbort.ShowAsync(grMain);

                            // Antwort auslesen
                            string answer = dQuery.GetAnswer();

                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders = true;
                            }

                            // Wenn Integrieren
                            if (answer == resource.GetString("001_Integrieren"))
                            {
                                // Temponäre Option erstellen
                                CreationCollisionOption coTemp = classCopyOrMoveFilesAndFolders.nameCreationCollisionOptionFiles;

                                // Ordner neu erstellen und Integrieren
                                classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes = CreationCollisionOption.OpenIfExists;
                                await mfPasteFolderAndFilesOneDriveToLokal(sourceItem, targetFolder, classCopyOrMoveFilesAndFolders, dWaitAbort, token);

                                // Wenn für alle weiteren ausgeführt wird
                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                {
                                    classCopyOrMoveFilesAndFolders.nameCreationCollisionOptionFiles = CreationCollisionOption.OpenIfExists;
                                }

                                // Ordner überspringen
                                skip = true;
                            }

                            // Bei überspringen
                            else if (answer == resource.GetString("001_Überspringen"))
                            {
                                // Angeben das Fehler vorhanden
                                classCopyOrMoveFilesAndFolders.errors = true;

                                // Datei überspringen
                                skip = true;
                            }

                            // Wenn Abbrechen
                            else if (answer == resource.GetString("001_Abbrechen"))
                            {
                                // Angeben das Fehler vorhanden
                                classCopyOrMoveFilesAndFolders.errors = true;

                                // Task abbrechen
                                cancelTokenAction.Cancel();
                            }
                        }
                    }
                }

                // Wenn Dateien eingefügt werden
                if (!skip)
                {
                    // Dateien und Ordner einfügen
                    for (int i = 0; i < items.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn eine Datei
                            if (items[i].File != null)
                            {
                                // DialogEx updaten
                                dWaitAbort.SetTextBoxTextByIndex(0, items[i].Name);

                                // Versuchen Datei zu erstellen
                                StorageFile file = null;
                                try
                                {
                                    // Datei erstellen
                                    file = await folderTemp.CreateFileAsync(items[i].Name, CreationCollisionOption.ReplaceExisting);
                                    var streamFile = await file.OpenAsync(FileAccessMode.ReadWrite);

                                    // Pfad erstellen
                                    string pathFile = items[i].ParentReference.Path;
                                    pathFile = Regex.Replace(pathFile, ":", "/");
                                    pathFile += "/" + items[i].Name;
                                    pathFile = Regex.Replace(pathFile, "/drive/root/", "");

                                    // Datei herunterladen
                                    var builder = oneDriveClient.Drive.Root.ItemWithPath(pathFile);

                                    // Filestream erstellen
                                    using (Stream fileStream = await builder.Content.Request().GetAsync())
                                    {
                                        // BinaryReader erstellen
                                        using (BinaryReader binaryReader = new BinaryReader(fileStream))
                                        {
                                            // Puffer erstellen
                                            byte[] buffer = new byte[setBufferSize];
                                            int count;
                                            int totalBytes = 0;

                                            // Schleife Download
                                            while ((count = binaryReader.Read(buffer, 0, setBufferSize)) > 0)
                                            {
                                                // Wenn Abbruch durch Benutzer
                                                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                                {
                                                    // Versuchen Datei zu löschen
                                                    try
                                                    {
                                                        await file.DeleteAsync();
                                                    }
                                                    catch
                                                    { }
                                                    break;
                                                }

                                                // Datei als Stream herunterladen
                                                await streamFile.WriteAsync(buffer.AsBuffer());
                                                totalBytes += count;
                                            }
                                        }
                                    }

                                    // Wenn nich abgebrochen wurde
                                    if (dWaitAbort.GetAnswer() != resource.GetString("001_Abbrechen"))
                                    {
                                        // Datei in targetFolder verschieben
                                        await file.MoveAsync(newFolder, file.Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles);
                                    }
                                }

                                // Wenn Datei nicht erstellt werden konnte
                                catch (Exception e)
                                {
                                    // Wenn Datei bereits vorhanden
                                    if (e.HResult == -2147024713)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                            dQuery.Content = items[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                            }

                                            // Bei umbenennen
                                            if (answer == resource.GetString("001_Umbenennen"))
                                            {
                                                // Datei in targetFolder verschieben
                                                await file.MoveAsync(newFolder, file.Name, NameCollisionOption.GenerateUniqueName);

                                                // Wenn für alle weiteren ausgeführt wird
                                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                                {
                                                    classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                                }
                                            }
                                        }
                                    }
                                    // Wenn Datei nicht mehr vorhanden
                                    else if (e.HResult == -2147024894)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allFileNotFound)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiNichtVorhanden");
                                            dQuery.Content = items[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allFileNotFound = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }
                                    // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                    else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                    {
                                        // Nachricht erstellen
                                        dQuery = new DialogEx();
                                        dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                        dQuery.Content = items[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                        dQueryBtnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        dQueryBtnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(dQueryBtnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Antwort auslesen
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                        }

                                        // Wenn Abbrechen
                                        if (answer == resource.GetString("001_Abbrechen"))
                                        {
                                            // Task abbrechen
                                            cancelTokenAction.Cancel();
                                        }
                                    }
                                }
                            }

                            // Wenn ein Ordner
                            else
                            {
                                // Ordner mit allen Dateien einfügen // Task
                                classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesOneDriveToLokal(items[i], newFolder, classCopyOrMoveFilesAndFolders, dWaitAbort, token);
                            }

                            // Value erhöhen und Updaten
                            classCopyOrMoveFilesAndFolders.value++;
                            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Wenn Ordner nicht geöffnet werden konnte
            catch
            {
                // Angeben das Fehler vorhanden
                classCopyOrMoveFilesAndFolders.errors = true;

                // Wenn Nachricht ausgegeben wird
                if (!classCopyOrMoveFilesAndFolders.allCantOpenFolder)
                {
                    // Nachricht ausgeben
                    dQuery = new DialogEx();
                    dQuery.BorderBrush = scbXtrose;
                    dQuery.Title = resource.GetString("001_OrdnerNichÖffnen");
                    dQuery.Content = sourceItem.Name;
                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                    dQuery.AddCheckbox(chBox);
                    DialogExButtonsSet btnSet = new DialogExButtonsSet();
                    DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    btnSet.AddButton(btn_1);
                    DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                    btnSet.AddButton(btn_2);
                    dQuery.AddButtonSet(btnSet);
                    // Neue Ausgabe erstellen
                    dWaitAbort.Hide();
                    await dQuery.ShowAsync(grMain);
                    dWaitAbort.ShowAsync(grMain);

                    // Nachricht auswerten
                    string answer = dQuery.GetAnswer();

                    // Wenn für alle weiteren ausgeführt wird
                    if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                    {
                        classCopyOrMoveFilesAndFolders.allCantOpenFolder = true;
                    }

                    // Wenn Abbrechen
                    if (answer == resource.GetString("001_Abbrechen"))
                    {
                        // Task abbrechen
                        cancelTokenAction.Cancel();
                    }
                }
            }

            // Value erhöhen und Updaten
            classCopyOrMoveFilesAndFolders.value++;
            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }
#endregion
        // -----------------------------------------------------------------------





        // Lokal --> One Drive
        // -----------------------------------------------------------------------
#region Lokal --> One Drive

        // Task // Kopieren und Verschieben // Lokal --> One Drive
        private async Task<ClassCopyOrMoveFilesAndFolders> acCopyOrMoveLokalToOneDrive(List<StorageFile> listFiles, List<StorageFolder> listFolders, string path, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, CancellationToken token)
        {
            // Wenn kein Pfad vorhanden
            if (path == null)
            {
                path = "";
            }

            // Pfad umwandeln
            path = Regex.Replace(path, ":", "/");
            path = Regex.Replace(path, "/drive/root/", "");

            // Lade Anzeige augeben
            DialogEx dWaitAbort = new DialogEx();
            DialogEx dQuery = new DialogEx();
            dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
            DialogExProgressRing progressRing = new DialogExProgressRing();
            dWaitAbort.AddProgressRing(progressRing);
            dWaitAbort.ShowAsync(grMain);

            // Parameter Dateien und Ordner setzen
            List<int> listFilesFolders = new List<int>();
            listFilesFolders.Add(listFiles.Count());
            listFilesFolders.Add(0);

            // Ordner zählen
            for (int i = 0; i < listFolders.Count(); i++)
            {
                // Ordner hinzufügen
                listFilesFolders[1]++;
                // Datein in Ordner und Unterordner zählen // Task
                listFilesFolders = await acGetFilesAndFoldersLokal(listFolders[i], listFilesFolders);
            }

            // Wenn Dateien vorhanden
            if (listFilesFolders[0] + listFilesFolders[1] > 0)
            {
                // Content erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Abfrage bei Drag and Drop
                if (classCopyOrMoveFilesAndFolders.dragAndDrop)
                {
                    // Wenn Drag and Drop Aktion // ask
                    if (setDdCopyOrMove == "ask")
                    {
                        // Nachricht ausgeben
                        dQuery = new DialogEx();
                        dQuery.BorderBrush = scbXtrose;
                        dQuery.Title = resource.GetString("001_KopierenVerschieben");
                        dQuery.Content = content;
                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Kopieren"));
                        btnSet.AddButton(btn_1);
                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Verschieben"));
                        btnSet.AddButton(btn_2);
                        DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        btnSet.AddButton(btn_3);
                        dQuery.AddButtonSet(btnSet);
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn kopieren
                        if (answer == resource.GetString("001_Kopieren"))
                        {
                            // Auf kopieren stellen
                            classCopyOrMoveFilesAndFolders.move = false;
                        }
                        // Wenn verschieben
                        else if (answer == resource.GetString("001_Verschieben"))
                        {
                            // Auf verschieben stellen
                            classCopyOrMoveFilesAndFolders.move = true;
                        }
                    }

                    // Wenn Drag and Drop Aktion // move // copy // standard
                    else
                    {
                        // Bestätigung erstellen
                        if (setDdConfirmCopyOrMove == "always" | (setDdConfirmCopyOrMove == "several" & ddListSourceIndexes.Count() > 1))
                        {
                            // Title erstellen 
                            string title = resource.GetString("001_Kopieren");

                            if (classCopyOrMoveFilesAndFolders.move)
                            {
                                title = resource.GetString("001_Verschieben");
                            }

                            // Nachricht ausgeben // Bestätigung löschen
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = title;
                            dQuery.Content = content;
                            DialogExButtonsSet dialogExButtonsSet = new DialogExButtonsSet();
                            dialogExButtonsSet.HorizontalAlignment = HorizontalAlignment.Right;
                            DialogExButton button_1 = new DialogExButton(title);
                            dialogExButtonsSet.AddButton(button_1);
                            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dialogExButtonsSet.AddButton(button_2);
                            dQuery.AddButtonSet(dialogExButtonsSet);
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                        }
                    }
                }

                // Wenn nicht abgebrochen
                if (dQuery.GetAnswer() != resource.GetString("001_Abbrechen"))
                {
                    // Title erstellen 
                    string title = resource.GetString("001_Kopieren");

                    if (classCopyOrMoveFilesAndFolders.move)
                    {
                        title = resource.GetString("001_Verschieben");
                    }

                    // Lade Anzeige entfernen
                    dWaitAbort.Hide();

                    // Einfügen Ausgabe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = title;
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    tBlock.Margin = new Thickness(0, 8, 0, -8);
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien einfügen
                    for (int i = 0; i < listFiles.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // DialogEx updaten
                            dWaitAbort.SetTextBoxTextByIndex(0, listFiles[i].Name);

                            // Prüfen ob Datei bereits existiert
                            bool exists = true;
                            int ex = 0;
                            string filePath = path + "/" + listFiles[i].Name;

                            // Schleife bis Dateiname gefunden
                            while (exists)
                            {
                                // Wenn abbrechen gedrück wurde
                                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                {
                                    cancelTokenAction.Cancel();
                                    exists = false;
                                }

                                try
                                {
                                    // Versuchen Datei zu öffnen
                                    var builder = oneDriveClient.Drive.Root.ItemWithPath(filePath);
                                    var file = await builder.Request().GetAsync();

                                    // Nachricht ausgeben
                                    if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles & ex == 0)
                                    {
                                        // Nachricht ausgeben
                                        dQuery = new DialogEx();
                                        dQuery.BorderBrush = scbXtrose;
                                        dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                        dQuery.Content = listFiles[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        btnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                        btnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(btnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Nachricht auswerten
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                        }

                                        // Bei umbenennen
                                        if (answer == resource.GetString("001_Umbenennen"))
                                        {
                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            filePath = path + "/" + listFiles[i].Name + " (" + ex + ")";

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                            }
                                        }

                                        // Bei überspringen
                                        else if (answer == resource.GetString("001_Überspringen"))
                                        {
                                            // Angeben das erstellt
                                            exists = false;

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.ReplaceExisting;
                                            }
                                        }
                                    }

                                    // Wenn keine Nachricht ausgegeben wird
                                    else
                                    {
                                        // Beim überspringen
                                        if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.ReplaceExisting)
                                        {
                                            exists = false;
                                        }

                                        // Beim umbenennen
                                        else if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.GenerateUniqueName)
                                        {
                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            filePath = path + "/" + listFiles[i].Name + " (" + ex + ")";
                                        }
                                    }
                                }

                                // Wenn Datei noch nicht existiert
                                catch
                                {
                                    // Versuchen Datei zu kopieren
                                    try
                                    {
                                        // Datei für Stream öffnen
                                        Stream fileStream = await listFiles[i].OpenStreamForReadAsync();

                                        // Datei erstellen
                                        var streamFile = await oneDriveClient.Drive.Root.ItemWithPath(filePath).Content.Request().PutAsync<Item>(fileStream);

                                        // Wenn ausschneiden
                                        if (classCopyOrMoveFilesAndFolders.move)
                                        {
                                            // Datei löschen
                                            await listFiles[i].DeleteAsync();
                                        }
                                    }

                                    // Wenn Datei nicht Eingefügt werden konnte
                                    catch (Exception e)
                                    {
                                        if (e.HResult == -2147024894)
                                        {
                                            // Wenn Nachricht ausgegeben wird
                                            if (!classCopyOrMoveFilesAndFolders.allFileNotFound)
                                            {
                                                // Nachricht ausgeben
                                                dQuery = new DialogEx();
                                                dQuery.BorderBrush = scbXtrose;
                                                dQuery.Title = resource.GetString("001_DateiNichtVorhanden");
                                                dQuery.Content = listFiles[i].Name;
                                                DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                                dQuery.AddCheckbox(chBox);
                                                DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                                DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                                btnSet.AddButton(btn_1);
                                                DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                                btnSet.AddButton(btn_2);
                                                dQuery.AddButtonSet(btnSet);
                                                // Neue Ausgabe erstellen
                                                dWaitAbort.Hide();
                                                await dQuery.ShowAsync(grMain);
                                                dWaitAbort.ShowAsync(grMain);

                                                // Nachricht auswerten
                                                string answer = dQuery.GetAnswer();

                                                // Wenn für alle weiteren ausgeführt wird
                                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                                {
                                                    classCopyOrMoveFilesAndFolders.allFileNotFound = true;
                                                }

                                                // Wenn Abbrechen
                                                if (answer == resource.GetString("001_Abbrechen"))
                                                {
                                                    // Task abbrechen
                                                    cancelTokenAction.Cancel();
                                                }
                                            }
                                        }
                                        // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                        else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                        {
                                            // Nachricht erstellen
                                            dQuery = new DialogEx();
                                            dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                            dQuery.Content = listFiles[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            dQueryBtnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            dQueryBtnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(dQueryBtnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Antwort auslesen
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }

                                    // Angeben das erstellt wurde
                                    exists = false;
                                }
                            }


                            // Value erhöhen und Updaten
                            classCopyOrMoveFilesAndFolders.value++;
                            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }

                    // Ordner Durchlaufen
                    for (int i = 0; i < listFolders.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                            }
                            // Angeben das Ordner gelöscht werden kann
                            classCopyOrMoveFilesAndFolders.deleteIfMoving = true;

                            // Ordner mit allen Dateien einfügen // Task
                            classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesLokalToOneDrive(path, listFolders[i], classCopyOrMoveFilesAndFolders, dWaitAbort, token);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Wenn keine Dateien mehr vorhanden
            else
            {
                // Nachricht ausgeben
                dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Einfügen");
                dQuery.Content = resource.GetString("001_DateienNichtVorhanden");
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                dQuery.AddButtonSet(dQueryBtnSet);
                dWaitAbort.Hide();
                await dQuery.ShowAsync(grMain);
            }

            // Ausgabe entfernen
            dWaitAbort.Hide();

            // Drag and Drop beenden
            dd = false;

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }



        // Task // Ordner mit allen Dateien einfügen //One Drive zu Lokal
        private async Task<ClassCopyOrMoveFilesAndFolders> mfPasteFolderAndFilesLokalToOneDrive(string path, StorageFolder sourceFolder, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, DialogEx dWaitAbort, CancellationToken token)
        {
            // Parameter
            DialogEx dQuery = new DialogEx();
            IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
            IReadOnlyList<StorageFile> fileList = new List<StorageFile>();
            string newPath = null;
            bool skip = false;

            // Versuchen Dateien auszulesen
            try
            {
                // Dateien auslesen
                folderList = await sourceFolder.GetFoldersAsync();
                fileList = await sourceFolder.GetFilesAsync();

                // Versuchen Ordner zu öffnen
                try
                {
                    // Wenn Ordner vorhanden
                    var items = await oneDriveClient.Drive.Root.ItemWithPath(path + "/" + sourceFolder.Name).Children.Request().GetAsync();

                    if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders)
                    {
                        // Nachricht erstellen
                        dQuery = new DialogEx();
                        dQuery.Title = resource.GetString("001_OrdnerBereitsVorhanden");
                        dQuery.Content = sourceFolder.Name;
                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                        dQuery.AddCheckbox(chBox);
                        DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        dQueryBtnSet.AddButton(btn_1);
                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                        dQueryBtnSet.AddButton(btn_2);
                        DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Integrieren"));
                        dQueryBtnSet.AddButton(btn_3);
                        dQuery.AddButtonSet(dQueryBtnSet);
                        // Neue Ausgabe erstellen
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);
                        dWaitAbort.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn für alle weiteren ausgeführt wird
                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                        {
                            classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders = true;
                        }

                        // Wenn Integrieren
                        if (answer == resource.GetString("001_Integrieren"))
                        {
                            // Neuen Ordner erstellen
                            var newFolder = new Item
                            {
                                Name = sourceFolder.Name,
                                Folder = new Folder(),
                            };
                            var folderCreated = await oneDriveClient.Drive.Root.ItemWithPath(path).Children.Request().AddAsync(newFolder);

                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes = CreationCollisionOption.OpenIfExists;
                            }
                        }

                        // Bei überspringen
                        if (answer == resource.GetString("001_Überspringen"))
                        {
                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes = CreationCollisionOption.ReplaceExisting;
                            }

                            // Angeben das Ordner nicht gelöscht wird
                            classCopyOrMoveFilesAndFolders.deleteIfMoving = false;

                            // Überspringen
                            skip = true;
                        }

                        // Wenn Abbrechen
                        if (answer == resource.GetString("001_Abbrechen"))
                        {
                            // Task abbrechen
                            cancelTokenAction.Cancel();

                            skip = true;
                        }
                    }

                    // Wenn aktion für alle gewählt
                    else
                    {
                        // Wenn überspringen
                        if (classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes == CreationCollisionOption.ReplaceExisting)
                        {
                            // Angeben das Ordner nicht gelöscht wird
                            classCopyOrMoveFilesAndFolders.deleteIfMoving = false;

                            // Überspringen
                            skip = true;
                        }

                        // Wenn integrieren
                        else if (classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes == CreationCollisionOption.OpenIfExists)
                        {
                            // Neuen Ordner erstellen
                            var newFolder = new Item
                            {
                                Name = sourceFolder.Name,
                                Folder = new Folder(),
                            };
                            var folderCreated = await oneDriveClient.Drive.Root.ItemWithPath(path).Children.Request().AddAsync(newFolder);
                        }
                    }
                }

                // Wenn Ordner nicht vorhanden
                catch
                {
                    // Neuen Ordner erstellen
                    var newFolder = new Item
                    {
                        Name = sourceFolder.Name,
                        Folder = new Folder(),
                    };
                    var folderCreated = await oneDriveClient.Drive.Root.ItemWithPath(path).Children.Request().AddAsync(newFolder);
                }

                // Wenn Dateien eingefügt werden
                if (!skip)
                {
                    // Dateien durchlaufen
                    for (int i = 0; i < fileList.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // DialogEx updaten
                        dWaitAbort.SetTextBoxTextByIndex(0, fileList[i].Name);

                        // Prüfen ob Datei bereits existiert
                        bool exists = true;
                        int ex = 0;
                        string filePath = path + "/" + sourceFolder.Name + "/" + fileList[i].Name;

                        // Schleife bis Dateiname gefunden
                        while (exists)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                                exists = false;
                            }

                            try
                            {
                                // Versuchen Datei zu öffnen
                                var builder = oneDriveClient.Drive.Root.ItemWithPath(filePath);
                                var file = await builder.Request().GetAsync();

                                // Nachricht ausgeben
                                if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles & ex == 0)
                                {
                                    // Nachricht ausgeben
                                    dQuery = new DialogEx();
                                    dQuery.BorderBrush = scbXtrose;
                                    dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                    dQuery.Content = fileList[i].Name;
                                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                    dQuery.AddCheckbox(chBox);
                                    DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                    DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                    btnSet.AddButton(btn_1);
                                    DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                    btnSet.AddButton(btn_2);
                                    dQuery.AddButtonSet(btnSet);
                                    // Neue Ausgabe erstellen
                                    dWaitAbort.Hide();
                                    await dQuery.ShowAsync(grMain);
                                    dWaitAbort.ShowAsync(grMain);

                                    // Nachricht auswerten
                                    string answer = dQuery.GetAnswer();

                                    // Wenn für alle weiteren ausgeführt wird
                                    if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                    {
                                        classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                    }

                                    // Bei umbenennen
                                    if (answer == resource.GetString("001_Umbenennen"))
                                    {
                                        // Erweiterung erhöhen
                                        ex++;

                                        // Neuen Pfad erstellen
                                        filePath = path + "/" + fileList[i].Name + " (" + ex + ")";

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                        }
                                    }

                                    // Bei überspringen
                                    else if (answer == resource.GetString("001_Überspringen"))
                                    {
                                        // Angeben das Ordner nicht gelöscht wird
                                        classCopyOrMoveFilesAndFolders.deleteIfMoving = false;

                                        // Angeben das erstellt
                                        exists = false;

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.ReplaceExisting;
                                        }
                                    }
                                }

                                // Wenn keine Nachricht ausgegeben wird
                                else
                                {
                                    // Beim überspringen
                                    if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.ReplaceExisting)
                                    {
                                        // Angeben das Ordner nicht gelöscht wird
                                        classCopyOrMoveFilesAndFolders.deleteIfMoving = false;

                                        // Angeben das erstellt
                                        exists = false;
                                    }

                                    // Beim umbenennen
                                    else if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.GenerateUniqueName)
                                    {
                                        // Erweiterung erhöhen
                                        ex++;

                                        // Neuen Pfad erstellen
                                        filePath = path + "/" + fileList[i].Name + " (" + ex + ")";
                                    }
                                }
                            }

                            // Wenn Datei noch nicht existiert
                            catch
                            {
                                // Versuchen Datei zu kopieren
                                try
                                {
                                    // Datei für Stream öffnen
                                    Stream fileStream = await fileList[i].OpenStreamForReadAsync();

                                    // Datei erstellen
                                    var streamFile = await oneDriveClient.Drive.Root.ItemWithPath(filePath).Content.Request().PutAsync<Item>(fileStream);

                                    // Wenn ausschneiden
                                    if (classCopyOrMoveFilesAndFolders.move)
                                    {
                                        // Datei löschen
                                        await fileList[i].DeleteAsync();
                                    }
                                }

                                // Wenn Datei nicht Eingefügt werden konnte
                                catch (Exception e)
                                {
                                    if (e.HResult == -2147024894)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allFileNotFound)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiNichtVorhanden");
                                            dQuery.Content = fileList[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allFileNotFound = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }
                                    // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                    else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                    {
                                        // Nachricht erstellen
                                        dQuery = new DialogEx();
                                        dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                        dQuery.Content = fileList[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                        dQueryBtnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        dQueryBtnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(dQueryBtnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Antwort auslesen
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                        }

                                        // Wenn Abbrechen
                                        if (answer == resource.GetString("001_Abbrechen"))
                                        {
                                            // Task abbrechen
                                            cancelTokenAction.Cancel();
                                        }
                                    }
                                }

                                // Angeben das erstellt wurde
                                exists = false;
                            }
                        }

                        // Value erhöhen und Updaten
                        classCopyOrMoveFilesAndFolders.value++;
                        dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                    }
                }

                // Ordner durchlaufen
                for (int i = 0; i < folderList.Count(); i++)
                {
                    // Wenn abbrechen gedrück wurde
                    if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                    {
                        cancelTokenAction.Cancel();
                    }

                    // Wenn Task noch nicht abgebrochen
                    if (!token.IsCancellationRequested)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }
                        // Ordner mit allen Dateien einfügen // Task
                        classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesLokalToOneDrive(path + "/" + sourceFolder.Name, folderList[i], classCopyOrMoveFilesAndFolders, dWaitAbort, token);
                    }
                    // Wenn Task abgebrochen
                    else
                    {
                        break;
                    }
                }

                // Bei ausschneiden
                if (classCopyOrMoveFilesAndFolders.move & !token.IsCancellationRequested & classCopyOrMoveFilesAndFolders.deleteIfMoving)
                {
                    // Versuchen Ordner zu löschen
                    try
                    {
                        // Ordner löschen
                        await sourceFolder.DeleteAsync();
                    }
                    catch
                    {
                        // Wenn Nachricht ausgegeben wird
                        if (!classCopyOrMoveFilesAndFolders.allCantDeleteFolders)
                        {
                            // Nachricht ausgeben
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = resource.GetString("001_OrdnerNichtLöschen");
                            dQuery.Content = sourceFolder.Name;
                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                            dQuery.AddCheckbox(chBox);
                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            btnSet.AddButton(btn_1);
                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                            btnSet.AddButton(btn_2);
                            dQuery.AddButtonSet(btnSet);
                            // Neue Ausgabe erstellen
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                            dWaitAbort.ShowAsync(grMain);

                            // Antwort auslesen
                            string answer = dQuery.GetAnswer();

                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.allCantDeleteFolders = true;
                            }

                            // Wenn Abbrechen
                            if (answer == resource.GetString("001_Abbrechen"))
                            {
                                // Task abbrechen
                                cancelTokenAction.Cancel();
                            }
                        }
                    }
                }
            }

            // Wenn Dateien nicht ausgelesen werden können
            catch
            {
                // Wenn Nachricht ausgegeben wird
                if (!classCopyOrMoveFilesAndFolders.allCantOpenFolder)
                {
                    // Nachricht ausgeben
                    dQuery = new DialogEx();
                    dQuery.BorderBrush = scbXtrose;
                    dQuery.Title = resource.GetString("001_OrdnerNichÖffnen");
                    dQuery.Content = sourceFolder.Name;
                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                    dQuery.AddCheckbox(chBox);
                    DialogExButtonsSet btnSet = new DialogExButtonsSet();
                    DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    btnSet.AddButton(btn_1);
                    DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                    btnSet.AddButton(btn_2);
                    dQuery.AddButtonSet(btnSet);
                    // Neue Ausgabe erstellen
                    dWaitAbort.Hide();
                    await dQuery.ShowAsync(grMain);
                    dWaitAbort.ShowAsync(grMain);

                    // Nachricht auswerten
                    string answer = dQuery.GetAnswer();

                    // Wenn für alle weiteren ausgeführt wird
                    if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                    {
                        classCopyOrMoveFilesAndFolders.allCantOpenFolder = true;
                    }

                    // Wenn Abbrechen
                    if (answer == resource.GetString("001_Abbrechen"))
                    {
                        // Task abbrechen
                        cancelTokenAction.Cancel();
                    }
                }
            }
            // Value erhöhen und Updaten
            classCopyOrMoveFilesAndFolders.value++;
            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }
#endregion
        // -----------------------------------------------------------------------





        // One Drive --> One Drive
        // -----------------------------------------------------------------------
#region One Drive --> One Drive

        // Task // Kopieren und Verschieben // One Drive --> One Drive
        private async Task<ClassCopyOrMoveFilesAndFolders> acCopyOrMoveOneDriveToOneDrive(List<Item> listItems, string path, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, CancellationToken token)
        {
            // Wenn kein Pfad vorhanden
            if (path == null)
            {
                path = "";
            }

            // Pfad umwandeln
            path = Regex.Replace(path, ":", "/");
            path = Regex.Replace(path, "/drive/root/", "");

            // Lade Anzeige augeben
            DialogEx dWaitAbort = new DialogEx();
            DialogEx dQuery = new DialogEx();
            dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
            DialogExProgressRing progressRing = new DialogExProgressRing();
            dWaitAbort.AddProgressRing(progressRing);
            dWaitAbort.ShowAsync(grMain);

            // Parameter Dateien und Ordner setzen
            List<int> listFilesFolders = new List<int>();
            listFilesFolders.Add(0);
            listFilesFolders.Add(0);

            // Items durchlaufen
            for (int i = 0; i < listItems.Count(); i++)
            {
                // Wenn Ordner
                if (listItems[i].Folder != null)
                {
                    // Ordner hinzufügen
                    listFilesFolders[1]++;

                    // Datein in Ordner und Unterordner zählen // Task
                    listFilesFolders = await acGetFilesAndFoldersOneDrive(listItems[i], listFilesFolders);
                }
                // Wenn Datei
                else
                {
                    listFilesFolders[0]++;
                }
            }

            // Wenn Dateien vorhanden
            if (listFilesFolders[0] + listFilesFolders[1] > 0)
            {
                // Content erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Abfrage bei Drag and Drop
                if (classCopyOrMoveFilesAndFolders.dragAndDrop)
                {
                    // Wenn Drag and Drop Aktion // ask
                    if (setDdCopyOrMove == "ask")
                    {
                        // Nachricht ausgeben
                        dQuery = new DialogEx();
                        dQuery.BorderBrush = scbXtrose;
                        dQuery.Title = resource.GetString("001_KopierenVerschieben");
                        dQuery.Content = content;
                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Kopieren"));
                        btnSet.AddButton(btn_1);
                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Verschieben"));
                        btnSet.AddButton(btn_2);
                        DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        btnSet.AddButton(btn_3);
                        dQuery.AddButtonSet(btnSet);
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn kopieren
                        if (answer == resource.GetString("001_Kopieren"))
                        {
                            // Auf kopieren stellen
                            classCopyOrMoveFilesAndFolders.move = false;
                        }
                        // Wenn verschieben
                        else if (answer == resource.GetString("001_Verschieben"))
                        {
                            // Auf verschieben stellen
                            classCopyOrMoveFilesAndFolders.move = true;
                        }
                    }

                    // Wenn Drag and Drop Aktion // move // copy // standard
                    else
                    {
                        // Bestätigung erstellen
                        if (setDdConfirmCopyOrMove == "always" | (setDdConfirmCopyOrMove == "several" & ddListSourceIndexes.Count() > 1))
                        {
                            // Title erstellen 
                            string title = resource.GetString("001_Kopieren");

                            if (classCopyOrMoveFilesAndFolders.move)
                            {
                                title = resource.GetString("001_Verschieben");
                            }

                            // Nachricht ausgeben // Bestätigung löschen
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = title;
                            dQuery.Content = content;
                            DialogExButtonsSet dialogExButtonsSet = new DialogExButtonsSet();
                            dialogExButtonsSet.HorizontalAlignment = HorizontalAlignment.Right;
                            DialogExButton button_1 = new DialogExButton(title);
                            dialogExButtonsSet.AddButton(button_1);
                            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dialogExButtonsSet.AddButton(button_2);
                            dQuery.AddButtonSet(dialogExButtonsSet);
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                        }
                    }
                }

                // Wenn nicht abgebrochen
                if (dQuery.GetAnswer() != resource.GetString("001_Abbrechen"))
                {
                    // Title erstellen 
                    string title = resource.GetString("001_Kopieren");

                    if (classCopyOrMoveFilesAndFolders.move)
                    {
                        title = resource.GetString("001_Verschieben");
                    }

                    // Lade Anzeige entfernen
                    dWaitAbort.Hide();

                    // Einfügen Ausgabe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = title;
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listItems.Count();
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien und Ordner einfügen
                    for (int i = 0; i < listItems.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // DialogEx updaten
                            dWaitAbort.SetTextBoxTextByIndex(0, listItems[i].Name);

                            // Prüfen ob Datei oder Ordner bereits existiert
                            bool exists = true;
                            int ex = 0;
                            string filePath = path + "/" + listItems[i].Name;

                            // Schleife bis Dateiname oder Ordnername gefunden
                            while (exists)
                            {
                                // Wenn abbrechen gedrück wurde
                                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                {
                                    cancelTokenAction.Cancel();
                                    exists = false;
                                }

                                try
                                {
                                    // Versuchen Datei oder Ordner zu öffnen
                                    var builder = oneDriveClient.Drive.Root.ItemWithPath(filePath);
                                    var file = await builder.Request().GetAsync();

                                    // Nachricht ausgeben
                                    if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles & ex == 0)
                                    {
                                        // Nachricht ausgeben
                                        dQuery = new DialogEx();
                                        dQuery.BorderBrush = scbXtrose;
                                        dQuery.Title = resource.GetString("001_DateiOrdnerBereitsVorhanden");
                                        dQuery.Content = listItems[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        btnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                        btnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(btnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Nachricht auswerten
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                        }

                                        // Bei umbenennen
                                        if (answer == resource.GetString("001_Umbenennen"))
                                        {
                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            filePath = path + "/" + listItems[i].Name + " (" + ex + ")";

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                            }
                                        }

                                        // Bei überspringen
                                        else if (answer == resource.GetString("001_Überspringen"))
                                        {
                                            // Angeben das erstellt
                                            exists = false;

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.ReplaceExisting;
                                            }
                                        }
                                    }

                                    // Wenn keine Nachricht ausgegeben wird
                                    else
                                    {
                                        // Beim überspringen
                                        if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.ReplaceExisting)
                                        {
                                            exists = false;
                                        }

                                        // Beim umbenennen
                                        else if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.GenerateUniqueName)
                                        {
                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            filePath = path + "/" + listItems[i].Name + " (" + ex + ")";
                                        }
                                    }
                                }

                                // Wenn Datei oder Ordner noch nicht existiert
                                catch
                                {
                                    // Versuchen Datei oder Ordner zu kopieren
                                    try
                                    {
                                        // Pfad von Datei oder Ordner umwandeln
                                        string fileFolderPath = Regex.Replace(listItems[i].ParentReference.Path, ":", "/");
                                        fileFolderPath = Regex.Replace(fileFolderPath, "/drive/root/", "");
                                        fileFolderPath += "/" + listItems[i].Name;

                                        // Datei oder Ordner verschieben
                                        if (classCopyOrMoveFilesAndFolders.move)
                                        {
                                            var newLocation = await oneDriveClient.Drive.Root.ItemWithPath(path).Request().GetAsync();
                                            var updateItem = new Item
                                            {
                                                ParentReference = new ItemReference
                                                {
                                                    Id = newLocation.Id
                                                }
                                            };
                                            var itemWithUpdate = await oneDriveClient.Drive.Root.ItemWithPath(fileFolderPath).Request().UpdateAsync(updateItem);
                                        }

                                        // Datei oder Ordner kopieren
                                        else
                                        {
                                            var newLocation = await oneDriveClient.Drive.Root.ItemWithPath(path).Request().GetAsync();
                                            var updateItem = new Item
                                            {
                                                ParentReference = new ItemReference
                                                {
                                                    Id = newLocation.Id
                                                }
                                            };
                                            // var file = await oneDriveClient.Drive.Root.ItemWithPath(fileFolderPath).Request().GetAsync();
                                            var itemStatus = await oneDriveClient.Drive.Root.ItemWithPath(fileFolderPath).Copy(listItems[i].Name, new ItemReference { Id = newLocation.Id }).Request().PostAsync();
                                            var newItem = await itemStatus.CompleteOperationAsync(null, CancellationToken.None);
                                        }
                                    }

                                    // Wenn Datei oder Ordner nicht Eingefügt werden konnte
                                    catch (Exception e)
                                    {
                                        // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                        if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                        {
                                            // Nachricht erstellen
                                            dQuery = new DialogEx();
                                            dQuery.Title = resource.GetString("001_DateienOrdnerNichtEingefügt");
                                            dQuery.Content = listItems[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            dQueryBtnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            dQueryBtnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(dQueryBtnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Antwort auslesen
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }

                                    // Angeben das erstellt wurde
                                    exists = false;
                                }
                            }

                            // Value erhöhen und Updaten
                            classCopyOrMoveFilesAndFolders.value++;
                            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }
                }
            }
            // Wenn keine Dateien vorhanden
            else
            {
                // Nachricht ausgeben
                dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Einfügen");
                dQuery.Content = resource.GetString("001_DateienNichtVorhanden");
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                dQuery.AddButtonSet(dQueryBtnSet);
                dWaitAbort.Hide();
                await dQuery.ShowAsync(grMain);
            }

            // Ausgabe entfernen
            dWaitAbort.Hide();

            // Drag and Drop beenden
            dd = false;

            // Ausgabe
            return classCopyOrMoveFilesAndFolders;
        }
#endregion
        // -----------------------------------------------------------------------





        // Dropbox --> Lokal
        // -----------------------------------------------------------------------
#region Dropbox --> Lokal

        // Task // Kopieren und Verschieben // Dropbox --> Lokal
        private async Task<ClassCopyOrMoveFilesAndFolders> acCopyOrMoveDropboxToLokal(List<Metadata> listItems, string dropboxToken, StorageFolder targetFolder, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, CancellationToken token)
        {
            // Lade Anzeige augeben
            DialogEx dWaitAbort = new DialogEx();
            DialogEx dQuery = new DialogEx();
            dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
            DialogExProgressRing progressRing = new DialogExProgressRing();
            dWaitAbort.AddProgressRing(progressRing);
            dWaitAbort.ShowAsync(grMain);

            // Parameter Dateien und Ordner setzen
            List<int> listFilesFolders = new List<int>();
            listFilesFolders.Add(0);
            listFilesFolders.Add(0);

            // Items durchlaufen
            for (int i = 0; i < listItems.Count(); i++)
            {
                // Wenn Ordner
                if (listItems[i].IsFolder)
                {
                    // Ordner hinzufügen
                    listFilesFolders[1]++;

                    // Datein in Ordner und Unterordner zählen // Task
                    listFilesFolders = await acGetFilesAndFoldersDropbox(listItems[i], dropboxToken, listFilesFolders);
                }
                // Wenn Datei
                else
                {
                    listFilesFolders[0]++;
                }
            }

            // Wenn Dateien vorhanden
            if (listFilesFolders[0] + listFilesFolders[1] > 0)
            {
                // Content erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Abfrage bei Drag and Drop
                if (classCopyOrMoveFilesAndFolders.dragAndDrop)
                {
                    // Wenn Drag and Drop Aktion // ask
                    if (setDdCopyOrMove == "ask")
                    {
                        // Nachricht ausgeben
                        dQuery = new DialogEx();
                        dQuery.BorderBrush = scbXtrose;
                        dQuery.Title = resource.GetString("001_KopierenVerschieben");
                        dQuery.Content = content;
                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Kopieren"));
                        btnSet.AddButton(btn_1);
                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Verschieben"));
                        btnSet.AddButton(btn_2);
                        DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        btnSet.AddButton(btn_3);
                        dQuery.AddButtonSet(btnSet);
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn kopieren
                        if (answer == resource.GetString("001_Kopieren"))
                        {
                            // Auf kopieren stellen
                            classCopyOrMoveFilesAndFolders.move = false;
                        }
                        // Wenn verschieben
                        else if (answer == resource.GetString("001_Verschieben"))
                        {
                            // Auf verschieben stellen
                            classCopyOrMoveFilesAndFolders.move = true;
                        }
                    }

                    // Wenn Drag and Drop Aktion // move // copy // standard
                    else
                    {
                        // Bestätigung erstellen
                        if (setDdConfirmCopyOrMove == "always" | (setDdConfirmCopyOrMove == "several" & ddListSourceIndexes.Count() > 1))
                        {
                            // Title erstellen 
                            string title = resource.GetString("001_Kopieren");

                            if (classCopyOrMoveFilesAndFolders.move)
                            {
                                title = resource.GetString("001_Verschieben");
                            }

                            // Nachricht ausgeben // Bestätigung löschen
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = title;
                            dQuery.Content = content;
                            DialogExButtonsSet dialogExButtonsSet = new DialogExButtonsSet();
                            dialogExButtonsSet.HorizontalAlignment = HorizontalAlignment.Right;
                            DialogExButton button_1 = new DialogExButton(title);
                            dialogExButtonsSet.AddButton(button_1);
                            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dialogExButtonsSet.AddButton(button_2);
                            dQuery.AddButtonSet(dialogExButtonsSet);
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                        }
                    }
                }

                // Wenn nicht abgebrochen
                if (dQuery.GetAnswer() != resource.GetString("001_Abbrechen"))
                {
                    // Title erstellen 
                    string title = resource.GetString("001_Kopieren");

                    if (classCopyOrMoveFilesAndFolders.move)
                    {
                        title = resource.GetString("001_Verschieben");
                    }

                    // Lade Anzeige entfernen
                    dWaitAbort.Hide();

                    // Einfügen Ausgabe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = title;
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien und Ordner einfügen
                    for (int i = 0; i < listItems.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn eine Datei
                            if (listItems[i].IsFile)
                            {
                                // DialogEx updaten
                                dWaitAbort.SetTextBoxTextByIndex(0, listItems[i].Name);

                                // Versuchen Datei zu erstellen
                                StorageFile file = null;
                                try
                                {
                                    // Datei erstellen
                                    file = await folderTemp.CreateFileAsync(listItems[i].Name, CreationCollisionOption.ReplaceExisting);
                                    var streamFile = await file.OpenAsync(FileAccessMode.ReadWrite);
                                    DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                    var response = await dropboxClient.Files.DownloadAsync(listItems[i].PathDisplay);

                                    // Puffer erstellen
                                    ulong fileSize = response.Response.Size;
                                    byte[] buffer = new byte[setBufferSize];
                                    int totalBytes = 0;
                                    int lenght;

                                    // Download erstellen
                                    using (var stream = await response.GetContentAsStreamAsync())
                                    {
                                        // Schleife Download
                                        lenght = stream.Read(buffer, 0, setBufferSize);
                                        while (lenght > 0)
                                        {
                                            // Wenn Abbruch durch Benutzer
                                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                            {
                                                // Versuchen Datei zu löschen
                                                try
                                                {
                                                    await file.DeleteAsync();
                                                }
                                                catch
                                                { }
                                                break;
                                            }

                                            // Datei als Stream herunterladen
                                            await streamFile.WriteAsync(buffer.AsBuffer(0, lenght));
                                            lenght = stream.Read(buffer, 0, setBufferSize);
                                            totalBytes += lenght;
                                        }
                                    }

                                    // Wenn nich abgebrochen wurde
                                    if (dWaitAbort.GetAnswer() != resource.GetString("001_Abbrechen"))
                                    {
                                        // Datei in targetFolder verschieben
                                        await file.MoveAsync(targetFolder, file.Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles);
                                    }

                                    // Wenn verschieben und nicht abgebrochen
                                    if (classCopyOrMoveFilesAndFolders.move & dWaitAbort.GetAnswer() != resource.GetString("001_Abbrechen"))
                                    {
                                        // Datei auf DropBox löschen
                                        dropboxClient = new DropboxClient(dropboxToken);
                                        await dropboxClient.Files.DeleteAsync(listItems[i].PathDisplay);
                                    }
                                }

                                // Wenn Datei nicht erstellt werden konnte
                                catch (Exception e)
                                {
                                    // Wenn Datei bereits vorhanden
                                    if (e.HResult == -2147024713)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                            dQuery.Content = listItems[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                            }

                                            // Bei umbenennen
                                            if (answer == resource.GetString("001_Umbenennen"))
                                            {
                                                // Datei in targetFolder verschieben
                                                await file.MoveAsync(targetFolder, file.Name, NameCollisionOption.GenerateUniqueName);

                                                // Wenn für alle weiteren ausgeführt wird
                                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                                {
                                                    classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                                }
                                            }
                                        }
                                    }
                                    // Wenn Datei nicht mehr vorhanden
                                    else if (e.HResult == -2147024894)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allFileNotFound)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiNichtVorhanden");
                                            dQuery.Content = listItems[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allFileNotFound = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }
                                    // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                    else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                    {
                                        // Nachricht erstellen
                                        dQuery = new DialogEx();
                                        dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                        dQuery.Content = listItems[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                        dQueryBtnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        dQueryBtnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(dQueryBtnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Antwort auslesen
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                        }

                                        // Wenn Abbrechen
                                        if (answer == resource.GetString("001_Abbrechen"))
                                        {
                                            // Task abbrechen
                                            cancelTokenAction.Cancel();
                                        }
                                    }
                                }
                            }

                            // Wenn ein Ordner
                            else
                            {
                                // Fehler zurücksetzen
                                classCopyOrMoveFilesAndFolders.errors = false;

                                // Ordner mit allen Dateien einfügen // Task
                                classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesDropboxToLokal(listItems[i], dropboxToken, targetFolder, classCopyOrMoveFilesAndFolders, dWaitAbort, token);

                                // Wenn verschieben und nicht abgebrochen
                                if (classCopyOrMoveFilesAndFolders.move & dWaitAbort.GetAnswer() != resource.GetString("001_Abbrechen") & !classCopyOrMoveFilesAndFolders.errors)
                                {
                                    // Datei auf One Drive löschen
                                    DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                    await dropboxClient.Files.DeleteAsync(listItems[i].PathDisplay);
                                }
                            }

                            // Value erhöhen und Updaten
                            classCopyOrMoveFilesAndFolders.value++;
                            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Wenn keine Dateien vorhanden
            else
            {
                // Nachricht ausgeben
                dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Einfügen");
                dQuery.Content = resource.GetString("001_DateienNichtVorhanden");
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                dQuery.AddButtonSet(dQueryBtnSet);
                dWaitAbort.Hide();
                await dQuery.ShowAsync(grMain);
            }

            // Ausgabe entfernen
            dWaitAbort.Hide();

            // Drag and Drop beenden
            dd = false;

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }



        // Task // Ordner mit allen Dateien einfügen // Dropbox zu Lokal
        private async Task<ClassCopyOrMoveFilesAndFolders> mfPasteFolderAndFilesDropboxToLokal(Metadata sourceItem, string dropboxToken, StorageFolder targetFolder, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, DialogEx dWaitAbort, CancellationToken token)
        {
            // Parameter
            DialogEx dQuery = new DialogEx();
            StorageFolder newFolder = null;
            bool skip = false;

            // Pfad erstellen
            string path = sourceItem.PathDisplay;

            // Versuchen Daten zu laden
            try
            {
                // Dropbox Dateien laden
                DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                ListFolderResult folderResults = await dropboxClient.Files.ListFolderAsync(path);

                // Liste Items Dateien erstellen
                List<Metadata> items = new List<Metadata>();
                for (int i = 0; i < folderResults.Entries.Count(); i++)
                {
                    items.Add(folderResults.Entries[i]);
                }

                // Versuchen Ordner zu erstellen
                try
                {
                    // Odner erstellen
                    newFolder = await targetFolder.CreateFolderAsync(sourceItem.Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes);
                }

                // Wenn Ordner nicht erstellt werden konnte
                catch (Exception e)
                {
                    // Wenn Ordner bereits vorhanden
                    if (e.HResult == -2147024713)
                    {
                        if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders)
                        {
                            // Nachricht erstellen
                            dQuery = new DialogEx();
                            dQuery.Title = resource.GetString("001_OrdnerBereitsVorhanden");
                            dQuery.Content = sourceItem.Name;
                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                            dQuery.AddCheckbox(chBox);
                            DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dQueryBtnSet.AddButton(btn_1);
                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                            dQueryBtnSet.AddButton(btn_2);
                            DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Integrieren"));
                            dQueryBtnSet.AddButton(btn_3);
                            dQuery.AddButtonSet(dQueryBtnSet);
                            // Neue Ausgabe erstellen
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                            dWaitAbort.ShowAsync(grMain);

                            // Antwort auslesen
                            string answer = dQuery.GetAnswer();

                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders = true;
                            }

                            // Wenn Integrieren
                            if (answer == resource.GetString("001_Integrieren"))
                            {
                                // Temponäre Option erstellen
                                CreationCollisionOption coTemp = classCopyOrMoveFilesAndFolders.nameCreationCollisionOptionFiles;

                                // Ordner neu erstellen und Integrieren
                                classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes = CreationCollisionOption.OpenIfExists;
                                await mfPasteFolderAndFilesDropboxToLokal(sourceItem, dropboxToken, targetFolder, classCopyOrMoveFilesAndFolders, dWaitAbort, token);

                                // Wenn für alle weiteren ausgeführt wird
                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                {
                                    classCopyOrMoveFilesAndFolders.nameCreationCollisionOptionFiles = CreationCollisionOption.OpenIfExists;
                                }

                                // Ordner überspringen
                                skip = true;
                            }

                            // Bei überspringen
                            else if (answer == resource.GetString("001_Überspringen"))
                            {
                                // Angeben das Fehler vorhanden
                                classCopyOrMoveFilesAndFolders.errors = true;

                                // Datei überspringen
                                skip = true;
                            }

                            // Wenn Abbrechen
                            else if (answer == resource.GetString("001_Abbrechen"))
                            {
                                // Angeben das Fehler vorhanden
                                classCopyOrMoveFilesAndFolders.errors = true;

                                // Task abbrechen
                                cancelTokenAction.Cancel();
                            }
                        }
                    }
                }

                // Wenn Dateien eingefügt werden
                if (!skip)
                {
                    // Dateien und Ordner einfügen
                    for (int i = 0; i < items.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn eine Datei
                            if (items[i].IsFile)
                            {
                                // DialogEx updaten
                                dWaitAbort.SetTextBoxTextByIndex(0, items[i].Name);

                                // Versuchen Datei zu erstellen
                                StorageFile file = null;
                                try
                                {
                                    // Datei erstellen
                                    file = await folderTemp.CreateFileAsync(items[i].Name, CreationCollisionOption.ReplaceExisting);
                                    var streamFile = await file.OpenAsync(FileAccessMode.ReadWrite);
                                    dropboxClient = new DropboxClient(dropboxToken);
                                    var response = await dropboxClient.Files.DownloadAsync(items[i].PathDisplay);

                                    // Puffer erstellen
                                    ulong fileSize = response.Response.Size;
                                    byte[] buffer = new byte[setBufferSize];
                                    int totalBytes = 0;
                                    int lenght;

                                    // Download erstellen
                                    using (var stream = await response.GetContentAsStreamAsync())
                                    {
                                        // Schleife Download
                                        lenght = stream.Read(buffer, 0, setBufferSize);
                                        while (lenght > 0)
                                        {
                                            // Wenn Abbruch durch Benutzer
                                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                            {
                                                // Versuchen Datei zu löschen
                                                try
                                                {
                                                    await file.DeleteAsync();
                                                }
                                                catch
                                                { }
                                                break;
                                            }

                                            // Datei als Stream herunterladen
                                            await streamFile.WriteAsync(buffer.AsBuffer(0, lenght));
                                            lenght = stream.Read(buffer, 0, setBufferSize);
                                            totalBytes += lenght;
                                        }
                                    }

                                    // Wenn nich abgebrochen wurde
                                    if (dWaitAbort.GetAnswer() != resource.GetString("001_Abbrechen"))
                                    {
                                        // Datei in targetFolder verschieben
                                        await file.MoveAsync(newFolder, file.Name, classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles);
                                    }
                                }

                                // Wenn Datei nicht erstellt werden konnte
                                catch (Exception e)
                                {
                                    // Wenn Datei bereits vorhanden
                                    if (e.HResult == -2147024713)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                            dQuery.Content = items[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                            }

                                            // Bei umbenennen
                                            if (answer == resource.GetString("001_Umbenennen"))
                                            {
                                                // Datei in targetFolder verschieben
                                                await file.MoveAsync(newFolder, file.Name, NameCollisionOption.GenerateUniqueName);

                                                // Wenn für alle weiteren ausgeführt wird
                                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                                {
                                                    classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                                }
                                            }
                                        }
                                    }
                                    // Wenn Datei nicht mehr vorhanden
                                    else if (e.HResult == -2147024894)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allFileNotFound)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiNichtVorhanden");
                                            dQuery.Content = items[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allFileNotFound = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }
                                    // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                    else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                    {
                                        // Nachricht erstellen
                                        dQuery = new DialogEx();
                                        dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                        dQuery.Content = items[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                        dQueryBtnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        dQueryBtnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(dQueryBtnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Antwort auslesen
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                        }

                                        // Wenn Abbrechen
                                        if (answer == resource.GetString("001_Abbrechen"))
                                        {
                                            // Task abbrechen
                                            cancelTokenAction.Cancel();
                                        }
                                    }
                                }
                            }

                            // Wenn ein Ordner
                            else
                            {
                                // Ordner mit allen Dateien einfügen // Task
                                classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesDropboxToLokal(items[i], dropboxToken, newFolder, classCopyOrMoveFilesAndFolders, dWaitAbort, token);
                            }

                            // Value erhöhen und Updaten
                            classCopyOrMoveFilesAndFolders.value++;
                            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Wenn Ordner nicht geöffnet werden konnte
            catch
            {
                // Angeben das Fehler vorhanden
                classCopyOrMoveFilesAndFolders.errors = true;

                // Wenn Nachricht ausgegeben wird
                if (!classCopyOrMoveFilesAndFolders.allCantOpenFolder)
                {
                    // Nachricht ausgeben
                    dQuery = new DialogEx();
                    dQuery.BorderBrush = scbXtrose;
                    dQuery.Title = resource.GetString("001_OrdnerNichÖffnen");
                    dQuery.Content = sourceItem.Name;
                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                    dQuery.AddCheckbox(chBox);
                    DialogExButtonsSet btnSet = new DialogExButtonsSet();
                    DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    btnSet.AddButton(btn_1);
                    DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                    btnSet.AddButton(btn_2);
                    dQuery.AddButtonSet(btnSet);
                    // Neue Ausgabe erstellen
                    dWaitAbort.Hide();
                    await dQuery.ShowAsync(grMain);
                    dWaitAbort.ShowAsync(grMain);

                    // Nachricht auswerten
                    string answer = dQuery.GetAnswer();

                    // Wenn für alle weiteren ausgeführt wird
                    if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                    {
                        classCopyOrMoveFilesAndFolders.allCantOpenFolder = true;
                    }

                    // Wenn Abbrechen
                    if (answer == resource.GetString("001_Abbrechen"))
                    {
                        // Task abbrechen
                        cancelTokenAction.Cancel();
                    }
                }
            }

            // Value erhöhen und Updaten
            classCopyOrMoveFilesAndFolders.value++;
            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }
#endregion
        // -----------------------------------------------------------------------





        // Lokal --> Dropbox
        // -----------------------------------------------------------------------
#region Lokal --> Dropbox

        // Task // Kopieren und Verschieben // Lokal --> Dropbox
        private async Task<ClassCopyOrMoveFilesAndFolders> acCopyOrMoveLokalToDropbox(List<StorageFile> listFiles, List<StorageFolder> listFolders, string path, string dropboxToken, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, CancellationToken token)
        {
            // Wenn kein Pfad vorhanden
            if (path == null)
            {
                path = "";
            }

            // Lade Anzeige augeben
            DialogEx dWaitAbort = new DialogEx();
            DialogEx dQuery = new DialogEx();
            dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
            DialogExProgressRing progressRing = new DialogExProgressRing();
            dWaitAbort.AddProgressRing(progressRing);
            dWaitAbort.ShowAsync(grMain);

            // Parameter Dateien und Ordner setzen
            List<int> listFilesFolders = new List<int>();
            listFilesFolders.Add(listFiles.Count());
            listFilesFolders.Add(0);

            // Ordner zählen
            for (int i = 0; i < listFolders.Count(); i++)
            {
                // Ordner hinzufügen
                listFilesFolders[1]++;
                // Datein in Ordner und Unterordner zählen // Task
                listFilesFolders = await acGetFilesAndFoldersLokal(listFolders[i], listFilesFolders);
            }

            // Wenn Dateien vorhanden
            if (listFilesFolders[0] + listFilesFolders[1] > 0)
            {
                // Content erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Abfrage bei Drag and Drop
                if (classCopyOrMoveFilesAndFolders.dragAndDrop)
                {
                    // Wenn Drag and Drop Aktion // ask
                    if (setDdCopyOrMove == "ask")
                    {
                        // Nachricht ausgeben
                        dQuery = new DialogEx();
                        dQuery.BorderBrush = scbXtrose;
                        dQuery.Title = resource.GetString("001_KopierenVerschieben");
                        dQuery.Content = content;
                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Kopieren"));
                        btnSet.AddButton(btn_1);
                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Verschieben"));
                        btnSet.AddButton(btn_2);
                        DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        btnSet.AddButton(btn_3);
                        dQuery.AddButtonSet(btnSet);
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn kopieren
                        if (answer == resource.GetString("001_Kopieren"))
                        {
                            // Auf kopieren stellen
                            classCopyOrMoveFilesAndFolders.move = false;
                        }
                        // Wenn verschieben
                        else if (answer == resource.GetString("001_Verschieben"))
                        {
                            // Auf verschieben stellen
                            classCopyOrMoveFilesAndFolders.move = true;
                        }
                    }

                    // Wenn Drag and Drop Aktion // move // copy // standard
                    else
                    {
                        // Bestätigung erstellen
                        if (setDdConfirmCopyOrMove == "always" | (setDdConfirmCopyOrMove == "several" & ddListSourceIndexes.Count() > 1))
                        {
                            // Title erstellen 
                            string title = resource.GetString("001_Kopieren");

                            if (classCopyOrMoveFilesAndFolders.move)
                            {
                                title = resource.GetString("001_Verschieben");
                            }

                            // Nachricht ausgeben // Bestätigung löschen
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = title;
                            dQuery.Content = content;
                            DialogExButtonsSet dialogExButtonsSet = new DialogExButtonsSet();
                            dialogExButtonsSet.HorizontalAlignment = HorizontalAlignment.Right;
                            DialogExButton button_1 = new DialogExButton(title);
                            dialogExButtonsSet.AddButton(button_1);
                            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dialogExButtonsSet.AddButton(button_2);
                            dQuery.AddButtonSet(dialogExButtonsSet);
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                        }
                    }
                }

                // Wenn nicht abgebrochen
                if (dQuery.GetAnswer() != resource.GetString("001_Abbrechen"))
                {
                    // Title erstellen 
                    string title = resource.GetString("001_Kopieren");

                    if (classCopyOrMoveFilesAndFolders.move)
                    {
                        title = resource.GetString("001_Verschieben");
                    }

                    // Lade Anzeige entfernen
                    dWaitAbort.Hide();

                    // Einfügen Ausgabe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = title;
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    tBlock.Margin = new Thickness(0, 8, 0, -8);
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien einfügen
                    for (int i = 0; i < listFiles.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // DialogEx updaten
                            dWaitAbort.SetTextBoxTextByIndex(0, listFiles[i].Name);

                            // Prüfen ob Datei bereits existiert
                            bool exists = true;
                            int ex = 0;
                            string filePath = path + "/" + listFiles[i].Name;

                            // Schleife bis Dateiname gefunden
                            while (exists)
                            {
                                // Wenn abbrechen gedrück wurde
                                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                {
                                    cancelTokenAction.Cancel();
                                    exists = false;
                                }

                                try
                                {
                                    // Versuchen Datei zu öffnen
                                    DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                    await dropboxClient.Files.GetMetadataAsync(filePath);

                                    // Nachricht ausgeben
                                    if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles & ex == 0)
                                    {
                                        // Nachricht ausgeben
                                        dQuery = new DialogEx();
                                        dQuery.BorderBrush = scbXtrose;
                                        dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                        dQuery.Content = listFiles[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        btnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                        btnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(btnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Nachricht auswerten
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                        }

                                        // Bei umbenennen
                                        if (answer == resource.GetString("001_Umbenennen"))
                                        {
                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            filePath = path + "/" + listFiles[i].Name + " (" + ex + ")";

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                            }
                                        }

                                        // Bei überspringen
                                        else if (answer == resource.GetString("001_Überspringen"))
                                        {
                                            // Angeben das erstellt
                                            exists = false;

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.ReplaceExisting;
                                            }
                                        }
                                    }

                                    // Wenn keine Nachricht ausgegeben wird
                                    else
                                    {
                                        // Beim überspringen
                                        if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.ReplaceExisting)
                                        {
                                            exists = false;
                                        }

                                        // Beim umbenennen
                                        else if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.GenerateUniqueName)
                                        {
                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            filePath = path + "/" + listFiles[i].Name + " (" + ex + ")";
                                        }
                                    }
                                }

                                // Wenn Datei noch nicht existiert
                                catch
                                {
                                    // Versuchen Datei zu kopieren
                                    try
                                    {
                                        // Darei kopieren
                                        using (Stream stream = await listFiles[i].OpenStreamForReadAsync())
                                        {
                                            DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                            FileMetadata metadate =  await dropboxClient.Files.UploadAsync(new CommitInfo(filePath), stream);
                                        }

                                        // Wenn ausschneiden
                                        if (classCopyOrMoveFilesAndFolders.move)
                                        {
                                            // Datei löschen
                                            await listFiles[i].DeleteAsync();
                                        }
                                    }

                                    // Wenn Datei nicht Eingefügt werden konnte
                                    catch (Exception e)
                                    {
                                        if (e.HResult == -2147024894)
                                        {
                                            // Wenn Nachricht ausgegeben wird
                                            if (!classCopyOrMoveFilesAndFolders.allFileNotFound)
                                            {
                                                // Nachricht ausgeben
                                                dQuery = new DialogEx();
                                                dQuery.BorderBrush = scbXtrose;
                                                dQuery.Title = resource.GetString("001_DateiNichtVorhanden");
                                                dQuery.Content = listFiles[i].Name;
                                                DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                                dQuery.AddCheckbox(chBox);
                                                DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                                DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                                btnSet.AddButton(btn_1);
                                                DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                                btnSet.AddButton(btn_2);
                                                dQuery.AddButtonSet(btnSet);
                                                // Neue Ausgabe erstellen
                                                dWaitAbort.Hide();
                                                await dQuery.ShowAsync(grMain);
                                                dWaitAbort.ShowAsync(grMain);

                                                // Nachricht auswerten
                                                string answer = dQuery.GetAnswer();

                                                // Wenn für alle weiteren ausgeführt wird
                                                if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                                {
                                                    classCopyOrMoveFilesAndFolders.allFileNotFound = true;
                                                }

                                                // Wenn Abbrechen
                                                if (answer == resource.GetString("001_Abbrechen"))
                                                {
                                                    // Task abbrechen
                                                    cancelTokenAction.Cancel();
                                                }
                                            }
                                        }
                                        // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                        else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                        {
                                            // Nachricht erstellen
                                            dQuery = new DialogEx();
                                            dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                            dQuery.Content = listFiles[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            dQueryBtnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            dQueryBtnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(dQueryBtnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Antwort auslesen
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }

                                    // Angeben das erstellt wurde
                                    exists = false;
                                }
                            }


                            // Value erhöhen und Updaten
                            classCopyOrMoveFilesAndFolders.value++;
                            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }

                    // Ordner Durchlaufen
                    for (int i = 0; i < listFolders.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                            }
                            // Angeben das Ordner gelöscht werden kann
                            classCopyOrMoveFilesAndFolders.deleteIfMoving = true;

                            // Ordner mit allen Dateien einfügen // Task
                            classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesLokalToDropbox(path, listFolders[i], dropboxToken, classCopyOrMoveFilesAndFolders, dWaitAbort, token);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // Wenn keine Dateien mehr vorhanden
            else
            {
                // Nachricht ausgeben
                dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Einfügen");
                dQuery.Content = resource.GetString("001_DateienNichtVorhanden");
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                dQuery.AddButtonSet(dQueryBtnSet);
                dWaitAbort.Hide();
                await dQuery.ShowAsync(grMain);
            }

            // Ausgabe entfernen
            dWaitAbort.Hide();

            // Drag and Drop beenden
            dd = false;

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }



        // Task // Ordner mit allen Dateien einfügen // Lokal zu Dropbox
        private async Task<ClassCopyOrMoveFilesAndFolders> mfPasteFolderAndFilesLokalToDropbox(string path, StorageFolder sourceFolder, string dropboxToken, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, DialogEx dWaitAbort, CancellationToken token)
        {
            // Parameter
            DialogEx dQuery = new DialogEx();
            IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
            IReadOnlyList<StorageFile> fileList = new List<StorageFile>();
            bool skip = false;

            // Versuchen Dateien auszulesen
            try
            {
                // Dateien auslesen
                folderList = await sourceFolder.GetFoldersAsync();
                fileList = await sourceFolder.GetFilesAsync();

                // Versuchen Ordner zu öffnen
                try
                {
                    // Wenn Ordner vorhanden
                    DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                    await dropboxClient.Files.GetMetadataAsync(path + "/" + sourceFolder.Name);

                    if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders)
                    {
                        // Nachricht erstellen
                        dQuery = new DialogEx();
                        dQuery.Title = resource.GetString("001_OrdnerBereitsVorhanden");
                        dQuery.Content = sourceFolder.Name;
                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                        dQuery.AddCheckbox(chBox);
                        DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        dQueryBtnSet.AddButton(btn_1);
                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                        dQueryBtnSet.AddButton(btn_2);
                        DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Integrieren"));
                        dQueryBtnSet.AddButton(btn_3);
                        dQuery.AddButtonSet(dQueryBtnSet);
                        // Neue Ausgabe erstellen
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);
                        dWaitAbort.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn für alle weiteren ausgeführt wird
                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                        {
                            classCopyOrMoveFilesAndFolders.allNameCollisionOptionFolders = true;
                        }

                        // Wenn Integrieren
                        if (answer == resource.GetString("001_Integrieren"))
                        {
                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes = CreationCollisionOption.OpenIfExists;
                            }
                        }

                        // Bei überspringen
                        if (answer == resource.GetString("001_Überspringen"))
                        {
                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes = CreationCollisionOption.ReplaceExisting;
                            }

                            // Angeben das Ordner nicht gelöscht wird
                            classCopyOrMoveFilesAndFolders.deleteIfMoving = false;

                            // Überspringen
                            skip = true;
                        }

                        // Wenn Abbrechen
                        if (answer == resource.GetString("001_Abbrechen"))
                        {
                            // Task abbrechen
                            cancelTokenAction.Cancel();

                            // Überspringen
                            skip = true;
                        }
                    }

                    // Wenn aktion für alle gewählt
                    else
                    {
                        // Wenn überspringen
                        if (classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes == CreationCollisionOption.ReplaceExisting)
                        {
                            // Angeben das Ordner nicht gelöscht wird
                            classCopyOrMoveFilesAndFolders.deleteIfMoving = false;

                            // Überspringen
                            skip = true;
                        }

                        // Wenn integrieren
                        else if (classCopyOrMoveFilesAndFolders.nameCollisionOptionsFoldes == CreationCollisionOption.OpenIfExists)
                        {

                        }
                    }
                }

                // Wenn Ordner nicht vorhanden
                catch
                {
                    // Neuen Ordner erstellen
                    DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                    await dropboxClient.Files.CreateFolderAsync(path + "/" + sourceFolder.Name);
                }

                // Wenn Dateien eingefügt werden
                if (!skip)
                {
                    // Dateien durchlaufen
                    for (int i = 0; i < fileList.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // DialogEx updaten
                        dWaitAbort.SetTextBoxTextByIndex(0, fileList[i].Name);

                        // Prüfen ob Datei bereits existiert
                        bool exists = true;
                        int ex = 0;
                        string filePath = path + "/" + sourceFolder.Name + "/" + fileList[i].Name;

                        // Schleife bis Dateiname gefunden
                        while (exists)
                        {
                            // Wenn abbrechen gedrück wurde
                            if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                            {
                                cancelTokenAction.Cancel();
                                exists = false;
                            }

                            try
                            {
                                // Versuchen Datei zu öffnen
                                DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                await dropboxClient.Files.GetMetadataAsync(filePath);

                                // Nachricht ausgeben
                                if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles & ex == 0)
                                {
                                    // Nachricht ausgeben
                                    dQuery = new DialogEx();
                                    dQuery.BorderBrush = scbXtrose;
                                    dQuery.Title = resource.GetString("001_DateiBereitsVorhanden");
                                    dQuery.Content = fileList[i].Name;
                                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                    dQuery.AddCheckbox(chBox);
                                    DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                    DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                    btnSet.AddButton(btn_1);
                                    DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                    btnSet.AddButton(btn_2);
                                    dQuery.AddButtonSet(btnSet);
                                    // Neue Ausgabe erstellen
                                    dWaitAbort.Hide();
                                    await dQuery.ShowAsync(grMain);
                                    dWaitAbort.ShowAsync(grMain);

                                    // Nachricht auswerten
                                    string answer = dQuery.GetAnswer();

                                    // Wenn für alle weiteren ausgeführt wird
                                    if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                    {
                                        classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                    }

                                    // Bei umbenennen
                                    if (answer == resource.GetString("001_Umbenennen"))
                                    {
                                        // Erweiterung erhöhen
                                        ex++;

                                        // Neuen Pfad erstellen
                                        filePath = path + "/" + fileList[i].Name + " (" + ex + ")";

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                        }
                                    }

                                    // Bei überspringen
                                    else if (answer == resource.GetString("001_Überspringen"))
                                    {
                                        // Angeben das Ordner nicht gelöscht wird
                                        classCopyOrMoveFilesAndFolders.deleteIfMoving = false;

                                        // Angeben das erstellt
                                        exists = false;

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.ReplaceExisting;
                                        }
                                    }
                                }

                                // Wenn keine Nachricht ausgegeben wird
                                else
                                {
                                    // Beim überspringen
                                    if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.ReplaceExisting)
                                    {
                                        // Angeben das Ordner nicht gelöscht wird
                                        classCopyOrMoveFilesAndFolders.deleteIfMoving = false;

                                        // Angeben das erstellt
                                        exists = false;
                                    }

                                    // Beim umbenennen
                                    else if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.GenerateUniqueName)
                                    {
                                        // Erweiterung erhöhen
                                        ex++;

                                        // Neuen Pfad erstellen
                                        filePath = path + "/" + fileList[i].Name + " (" + ex + ")";
                                    }
                                }
                            }

                            // Wenn Datei noch nicht existiert
                            catch
                            {
                                // Versuchen Datei zu kopieren
                                try
                                {
                                    // Datei kopieren
                                    using (Stream stream = await fileList[i].OpenStreamForReadAsync())
                                    {
                                        DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                        FileMetadata metadate = await dropboxClient.Files.UploadAsync(new CommitInfo(filePath), stream);
                                    }

                                    // Wenn ausschneiden
                                    if (classCopyOrMoveFilesAndFolders.move)
                                    {
                                        // Datei löschen
                                        await fileList[i].DeleteAsync();
                                    }
                                }

                                // Wenn Datei nicht Eingefügt werden konnte
                                catch (Exception e)
                                {
                                    if (e.HResult == -2147024894)
                                    {
                                        // Wenn Nachricht ausgegeben wird
                                        if (!classCopyOrMoveFilesAndFolders.allFileNotFound)
                                        {
                                            // Nachricht ausgeben
                                            dQuery = new DialogEx();
                                            dQuery.BorderBrush = scbXtrose;
                                            dQuery.Title = resource.GetString("001_DateiNichtVorhanden");
                                            dQuery.Content = fileList[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            btnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            btnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(btnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Nachricht auswerten
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allFileNotFound = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }
                                    // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                    else if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                    {
                                        // Nachricht erstellen
                                        dQuery = new DialogEx();
                                        dQuery.Title = resource.GetString("001_DateienNichtEingefügt");
                                        dQuery.Content = fileList[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                        dQueryBtnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        dQueryBtnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(dQueryBtnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Antwort auslesen
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                        }

                                        // Wenn Abbrechen
                                        if (answer == resource.GetString("001_Abbrechen"))
                                        {
                                            // Task abbrechen
                                            cancelTokenAction.Cancel();
                                        }
                                    }
                                }

                                // Angeben das erstellt wurde
                                exists = false;
                            }
                        }

                        // Value erhöhen und Updaten
                        classCopyOrMoveFilesAndFolders.value++;
                        dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                    }
                }

                // Ordner durchlaufen
                for (int i = 0; i < folderList.Count(); i++)
                {
                    // Wenn abbrechen gedrück wurde
                    if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                    {
                        cancelTokenAction.Cancel();
                    }

                    // Wenn Task noch nicht abgebrochen
                    if (!token.IsCancellationRequested)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }
                        // Ordner mit allen Dateien einfügen // Task
                        classCopyOrMoveFilesAndFolders = await mfPasteFolderAndFilesLokalToDropbox(path + "/" + sourceFolder.Name, folderList[i], dropboxToken, classCopyOrMoveFilesAndFolders, dWaitAbort, token);
                    }
                    // Wenn Task abgebrochen
                    else
                    {
                        break;
                    }
                }

                // Bei ausschneiden
                if (classCopyOrMoveFilesAndFolders.move & !token.IsCancellationRequested & classCopyOrMoveFilesAndFolders.deleteIfMoving)
                {
                    // Versuchen Ordner zu löschen
                    try
                    {
                        // Ordner löschen
                        await sourceFolder.DeleteAsync();
                    }
                    catch
                    {
                        // Wenn Nachricht ausgegeben wird
                        if (!classCopyOrMoveFilesAndFolders.allCantDeleteFolders)
                        {
                            // Nachricht ausgeben
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = resource.GetString("001_OrdnerNichtLöschen");
                            dQuery.Content = sourceFolder.Name;
                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                            dQuery.AddCheckbox(chBox);
                            DialogExButtonsSet btnSet = new DialogExButtonsSet();
                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            btnSet.AddButton(btn_1);
                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                            btnSet.AddButton(btn_2);
                            dQuery.AddButtonSet(btnSet);
                            // Neue Ausgabe erstellen
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                            dWaitAbort.ShowAsync(grMain);

                            // Antwort auslesen
                            string answer = dQuery.GetAnswer();

                            // Wenn für alle weiteren ausgeführt wird
                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                            {
                                classCopyOrMoveFilesAndFolders.allCantDeleteFolders = true;
                            }

                            // Wenn Abbrechen
                            if (answer == resource.GetString("001_Abbrechen"))
                            {
                                // Task abbrechen
                                cancelTokenAction.Cancel();
                            }
                        }
                    }
                }
            }

            // Wenn Dateien nicht ausgelesen werden können
            catch
            {
                // Wenn Nachricht ausgegeben wird
                if (!classCopyOrMoveFilesAndFolders.allCantOpenFolder)
                {
                    // Nachricht ausgeben
                    dQuery = new DialogEx();
                    dQuery.BorderBrush = scbXtrose;
                    dQuery.Title = resource.GetString("001_OrdnerNichÖffnen");
                    dQuery.Content = sourceFolder.Name;
                    DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                    dQuery.AddCheckbox(chBox);
                    DialogExButtonsSet btnSet = new DialogExButtonsSet();
                    DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    btnSet.AddButton(btn_1);
                    DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                    btnSet.AddButton(btn_2);
                    dQuery.AddButtonSet(btnSet);
                    // Neue Ausgabe erstellen
                    dWaitAbort.Hide();
                    await dQuery.ShowAsync(grMain);
                    dWaitAbort.ShowAsync(grMain);

                    // Nachricht auswerten
                    string answer = dQuery.GetAnswer();

                    // Wenn für alle weiteren ausgeführt wird
                    if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                    {
                        classCopyOrMoveFilesAndFolders.allCantOpenFolder = true;
                    }

                    // Wenn Abbrechen
                    if (answer == resource.GetString("001_Abbrechen"))
                    {
                        // Task abbrechen
                        cancelTokenAction.Cancel();
                    }
                }
            }
            // Value erhöhen und Updaten
            classCopyOrMoveFilesAndFolders.value++;
            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);

            // Rückgabe
            return classCopyOrMoveFilesAndFolders;
        }
#endregion
        // -----------------------------------------------------------------------





        // Dropbox --> Dropbox
        // -----------------------------------------------------------------------
#region Dropbox --> Dropbox

        // Task // Kopieren und Verschieben // Dropbox --> Dropbox
        private async Task<ClassCopyOrMoveFilesAndFolders> acCopyOrMoveDropboxToDropbox(List<Metadata> listItems, string path, string dropboxToken, ClassCopyOrMoveFilesAndFolders classCopyOrMoveFilesAndFolders, CancellationToken token)
        {
            // Wenn kein Pfad vorhanden
            if (path == null)
            {
                path = "";
            }

            // Lade Anzeige augeben
            DialogEx dWaitAbort = new DialogEx();
            DialogEx dQuery = new DialogEx();
            dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
            DialogExProgressRing progressRing = new DialogExProgressRing();
            dWaitAbort.AddProgressRing(progressRing);
            dWaitAbort.ShowAsync(grMain);

            // Parameter Dateien und Ordner setzen
            List<int> listFilesFolders = new List<int>();
            listFilesFolders.Add(0);
            listFilesFolders.Add(0);

            // Items durchlaufen
            for (int i = 0; i < listItems.Count(); i++)
            {
                // Wenn Ordner
                if (listItems[i].IsFolder)
                {
                    // Ordner hinzufügen
                    listFilesFolders[1]++;

                    // Datein in Ordner und Unterordner zählen // Task
                    listFilesFolders = await acGetFilesAndFoldersDropbox(listItems[i], dropboxToken, listFilesFolders);
                }
                // Wenn Datei
                else
                {
                    listFilesFolders[0]++;
                }
            }

            // Wenn Dateien vorhanden
            if (listFilesFolders[0] + listFilesFolders[1] > 0)
            {
                // Content erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");

                // Abfrage bei Drag and Drop
                if (classCopyOrMoveFilesAndFolders.dragAndDrop)
                {
                    // Wenn Drag and Drop Aktion // ask
                    if (setDdCopyOrMove == "ask")
                    {
                        // Nachricht ausgeben
                        dQuery = new DialogEx();
                        dQuery.BorderBrush = scbXtrose;
                        dQuery.Title = resource.GetString("001_KopierenVerschieben");
                        dQuery.Content = content;
                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Kopieren"));
                        btnSet.AddButton(btn_1);
                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Verschieben"));
                        btnSet.AddButton(btn_2);
                        DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        btnSet.AddButton(btn_3);
                        dQuery.AddButtonSet(btnSet);
                        dWaitAbort.Hide();
                        await dQuery.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dQuery.GetAnswer();

                        // Wenn kopieren
                        if (answer == resource.GetString("001_Kopieren"))
                        {
                            // Auf kopieren stellen
                            classCopyOrMoveFilesAndFolders.move = false;
                        }
                        // Wenn verschieben
                        else if (answer == resource.GetString("001_Verschieben"))
                        {
                            // Auf verschieben stellen
                            classCopyOrMoveFilesAndFolders.move = true;
                        }
                    }

                    // Wenn Drag and Drop Aktion // move // copy // standard
                    else
                    {
                        // Bestätigung erstellen
                        if (setDdConfirmCopyOrMove == "always" | (setDdConfirmCopyOrMove == "several" & ddListSourceIndexes.Count() > 1))
                        {
                            // Title erstellen 
                            string title = resource.GetString("001_Kopieren");

                            if (classCopyOrMoveFilesAndFolders.move)
                            {
                                title = resource.GetString("001_Verschieben");
                            }

                            // Nachricht ausgeben // Bestätigung löschen
                            dQuery = new DialogEx();
                            dQuery.BorderBrush = scbXtrose;
                            dQuery.Title = title;
                            dQuery.Content = content;
                            DialogExButtonsSet dialogExButtonsSet = new DialogExButtonsSet();
                            dialogExButtonsSet.HorizontalAlignment = HorizontalAlignment.Right;
                            DialogExButton button_1 = new DialogExButton(title);
                            dialogExButtonsSet.AddButton(button_1);
                            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dialogExButtonsSet.AddButton(button_2);
                            dQuery.AddButtonSet(dialogExButtonsSet);
                            dWaitAbort.Hide();
                            await dQuery.ShowAsync(grMain);
                        }
                    }
                }

                // Wenn nicht abgebrochen
                if (dQuery.GetAnswer() != resource.GetString("001_Abbrechen"))
                {
                    // Title erstellen 
                    string title = resource.GetString("001_Kopieren");

                    if (classCopyOrMoveFilesAndFolders.move)
                    {
                        title = resource.GetString("001_Verschieben");
                    }

                    // Lade Anzeige entfernen
                    dWaitAbort.Hide();

                    // Einfügen Ausgabe erstellen
                    dWaitAbort = new DialogEx();
                    dWaitAbort.Title = title;
                    dWaitAbort.Content = content;
                    progressRing = new DialogExProgressRing();
                    dWaitAbort.AddProgressRing(progressRing);
                    DialogExTextBlock tBlock = new DialogExTextBlock();
                    tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                    tBlock.FontSize = 10;
                    dWaitAbort.AddTextBlock(tBlock);
                    DialogExProgressBar proBar = new DialogExProgressBar();
                    proBar.Maximun = listItems.Count();
                    dWaitAbort.AddProgressBar(proBar);
                    DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                    DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                    dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                    dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                    dWaitAbort.ShowAsync(grMain);

                    // Dateien und Ordner einfügen
                    for (int i = 0; i < listItems.Count(); i++)
                    {
                        // Wenn abbrechen gedrück wurde
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            cancelTokenAction.Cancel();
                        }

                        // Wenn Task noch nicht abgebrochen
                        if (!token.IsCancellationRequested)
                        {
                            // DialogEx updaten
                            dWaitAbort.SetTextBoxTextByIndex(0, listItems[i].Name);

                            // Prüfen ob Datei oder Ordner bereits existiert
                            bool exists = true;
                            int ex = 0;
                            string filePath = path + "/" + listItems[i].Name;

                            // Schleife bis Dateiname oder Ordnername gefunden
                            while (exists)
                            {
                                // Wenn abbrechen gedrück wurde
                                if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                                {
                                    cancelTokenAction.Cancel();
                                    exists = false;
                                }

                                try
                                {
                                    // Versuchen Datei oder Ordner zu öffnen
                                    DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                    await dropboxClient.Files.GetMetadataAsync(filePath);

                                    // Nachricht ausgeben
                                    if (!classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles & ex == 0)
                                    {
                                        // Nachricht ausgeben
                                        dQuery = new DialogEx();
                                        dQuery.BorderBrush = scbXtrose;
                                        dQuery.Title = resource.GetString("001_DateiOrdnerBereitsVorhanden");
                                        dQuery.Content = listItems[i].Name;
                                        DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                        dQuery.AddCheckbox(chBox);
                                        DialogExButtonsSet btnSet = new DialogExButtonsSet();
                                        DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Überspringen"));
                                        btnSet.AddButton(btn_1);
                                        DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Umbenennen"));
                                        btnSet.AddButton(btn_2);
                                        dQuery.AddButtonSet(btnSet);
                                        // Neue Ausgabe erstellen
                                        dWaitAbort.Hide();
                                        await dQuery.ShowAsync(grMain);
                                        dWaitAbort.ShowAsync(grMain);

                                        // Nachricht auswerten
                                        string answer = dQuery.GetAnswer();

                                        // Wenn für alle weiteren ausgeführt wird
                                        if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                        {
                                            classCopyOrMoveFilesAndFolders.allNameCollisionOptionFiles = true;
                                        }

                                        // Bei umbenennen
                                        if (answer == resource.GetString("001_Umbenennen"))
                                        {
                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            filePath = path + "/" + listItems[i].Name + " (" + ex + ")";

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.GenerateUniqueName;
                                            }
                                        }

                                        // Bei überspringen
                                        else if (answer == resource.GetString("001_Überspringen"))
                                        {
                                            // Angeben das erstellt
                                            exists = false;

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles = NameCollisionOption.ReplaceExisting;
                                            }
                                        }
                                    }

                                    // Wenn keine Nachricht ausgegeben wird
                                    else
                                    {
                                        // Beim überspringen
                                        if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.ReplaceExisting)
                                        {
                                            exists = false;
                                        }

                                        // Beim umbenennen
                                        else if (classCopyOrMoveFilesAndFolders.nameCollisionOptionFiles == NameCollisionOption.GenerateUniqueName)
                                        {
                                            // Erweiterung erhöhen
                                            ex++;

                                            // Neuen Pfad erstellen
                                            filePath = path + "/" + listItems[i].Name + " (" + ex + ")";
                                        }
                                    }
                                }

                                // Wenn Datei oder Ordner noch nicht existiert
                                catch
                                {
                                    // Versuchen Datei oder Ordner zu kopieren
                                    try
                                    {
                                        // Datei oder Ordner verschieben
                                        if (classCopyOrMoveFilesAndFolders.move)
                                        {
                                            DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                            await dropboxClient.Files.MoveAsync(listItems[i].PathDisplay, filePath);
                                        }

                                        // Datei oder Ordner kopieren
                                        else
                                        {
                                            DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                                            await dropboxClient.Files.CopyAsync(listItems[i].PathDisplay, filePath);
                                        }
                                    }

                                    // Wenn Datei oder Ordner nicht Eingefügt werden konnte
                                    catch (Exception e)
                                    {
                                        // Wann Datei Ansonsten nicht kopiert oder verschoben werden kann
                                        if (!classCopyOrMoveFilesAndFolders.allCantPasteFiles)
                                        {
                                            // Nachricht erstellen
                                            dQuery = new DialogEx();
                                            dQuery.Title = resource.GetString("001_DateienOrdnerNichtEingefügt");
                                            dQuery.Content = listItems[i].Name;
                                            DialogExCheckBox chBox = new DialogExCheckBox(resource.GetString("001_FürAlle"));
                                            dQuery.AddCheckbox(chBox);
                                            DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                                            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                                            dQueryBtnSet.AddButton(btn_1);
                                            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_Überspringen"));
                                            dQueryBtnSet.AddButton(btn_2);
                                            dQuery.AddButtonSet(dQueryBtnSet);
                                            // Neue Ausgabe erstellen
                                            dWaitAbort.Hide();
                                            await dQuery.ShowAsync(grMain);
                                            dWaitAbort.ShowAsync(grMain);

                                            // Antwort auslesen
                                            string answer = dQuery.GetAnswer();

                                            // Wenn für alle weiteren ausgeführt wird
                                            if (dQuery.GetCheckboxByContent(resource.GetString("001_FürAlle")))
                                            {
                                                classCopyOrMoveFilesAndFolders.allCantPasteFiles = true;
                                            }

                                            // Wenn Abbrechen
                                            if (answer == resource.GetString("001_Abbrechen"))
                                            {
                                                // Task abbrechen
                                                cancelTokenAction.Cancel();
                                            }
                                        }
                                    }

                                    // Angeben das erstellt wurde
                                    exists = false;
                                }
                            }

                            // Value erhöhen und Updaten
                            classCopyOrMoveFilesAndFolders.value++;
                            dWaitAbort.SetProgressBarValueByIndex(0, classCopyOrMoveFilesAndFolders.value);
                        }
                        // Wenn Task abgebrochen
                        else
                        {
                            break;
                        }
                    }
                }
            }
            // Wenn keine Dateien vorhanden
            else
            {
                // Nachricht ausgeben
                dQuery = new DialogEx();
                dQuery.Title = resource.GetString("001_Einfügen");
                dQuery.Content = resource.GetString("001_DateienNichtVorhanden");
                DialogExButtonsSet dQueryBtnSet = new DialogExButtonsSet();
                DialogExButton dQueryBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                dQueryBtnSet.AddButton(dQueryBtn_1);
                dQuery.AddButtonSet(dQueryBtnSet);
                dWaitAbort.Hide();
                await dQuery.ShowAsync(grMain);
            }

            // Ausgabe entfernen
            dWaitAbort.Hide();

            // Drag and Drop beenden
            dd = false;

            // Ausgabe
            return classCopyOrMoveFilesAndFolders;
        }
#endregion
        // -----------------------------------------------------------------------
#endregion










#region Buttons Menüleiste
        // -----------------------------------------------------------------------
#region Menüleisten erstellen

        // Methode // Menüleisten erstellen
        private async void createAndShowMenu(string side)
        {
            // Links
            if (side == "left")
            {
                // Menü leeren
                spMenuLeft.Children.Clear();

                // Button Back hinzufügen
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canBack)
                {
                    BitmapImage img = await loadIcon("Back");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "left";
                    imgBtn.PointerReleased += BtnBack_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }

                // Button Selection Mode hinzufügen
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canMultiSelect)
                {
                    BitmapImage img = await loadIcon("MultiSelect");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "left";
                    imgBtn.PointerReleased += BtnSelectionMode_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }

                // Button Create Folder hinzufügen
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canCreateFolder)
                {
                    BitmapImage img = await loadIcon("NewFolder");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "left";
                    imgBtn.PointerReleased += BtnCreateFolder_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }

                // Button Sortieren hinzufügen
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canSort)
                {
                    BitmapImage img = await loadIcon("Sort");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "left";
                    imgBtn.PointerReleased += BtnSort_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }

                // Button Hinzufügen hinzufügen
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canSettings)
                {
                    BitmapImage img = await loadIcon("Add");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "left";
                    imgBtn.PointerReleased += BtnAdd_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }

                // Button Settings hinzufügen
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canSettings)
                {
                    BitmapImage img = await loadIcon("Settings");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "left";
                    imgBtn.PointerReleased += BtnSettings_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }

                // Button Help hinzufügen
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canSettings)
                {
                    BitmapImage img = await loadIcon("Help");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "left";
                    imgBtn.PointerReleased += BtnHelp_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }

                // Button About hinzufügen
                if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canSettings)
                {
                    BitmapImage img = await loadIcon("About");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "left";
                    imgBtn.PointerReleased += BtnAbout_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }

                // Button Size hinzufügen
                if (true)
                {
                    BitmapImage img = await loadIcon("Size");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "left";
                    imgBtn.PointerReleased += BtnSize_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }

                // Button Menü hinzufügen
                if (continuum & listFolderTreeLeft[listFolderTreeLeft.Count() - 1].canFlyout)
                {
                    BitmapImage img = await loadIcon("MenuTop");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "svLeft";
                    imgBtn.PointerReleased += BtnMenu_PointerReleased;
                    spMenuLeft.Children.Add(imgBtn);
                }
            }

            // Rechts
            else if (side == "right")
            {
                // Menü leeren
                spMenuRight.Children.Clear();

                // Button Back hinzufügen
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canBack)
                {
                    BitmapImage img = await loadIcon("Back");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "right";
                    imgBtn.PointerReleased += BtnBack_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }

                // Button Selection Mode hinzufügen
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canMultiSelect)
                {
                    BitmapImage img = await loadIcon("MultiSelect");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "right";
                    imgBtn.PointerReleased += BtnSelectionMode_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }

                // Button Create Folder hinzufügen
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canCreateFolder)
                {
                    BitmapImage img = await loadIcon("NewFolder");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "right";
                    imgBtn.PointerReleased += BtnCreateFolder_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }

                // Button Sortieren hinzufügen
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canSort)
                {
                    BitmapImage img = await loadIcon("Sort");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "right";
                    imgBtn.PointerReleased += BtnSort_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }

                // Button hinzufügen hinzufügen
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canSettings)
                {
                    BitmapImage img = await loadIcon("Add");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "right";
                    imgBtn.PointerReleased += BtnAdd_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }

                // Button Settings hinzufügen
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canSettings)
                {
                    BitmapImage img = await loadIcon("Settings");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "right";
                    imgBtn.PointerReleased += BtnSettings_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }

                // Button Help hinzufügen
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canSettings)
                {
                    BitmapImage img = await loadIcon("Help");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "right";
                    imgBtn.PointerReleased += BtnHelp_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }

                // Button About hinzufügen
                if (listFolderTreeRight[listFolderTreeRight.Count() - 1].canSettings)
                {
                    BitmapImage img = await loadIcon("About");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "right";
                    imgBtn.PointerReleased += BtnAbout_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }

                // Button Size hinzufügen
                if (true)
                {
                    BitmapImage img = await loadIcon("Size");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "right";
                    imgBtn.PointerReleased += BtnSize_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }

                // Button Menü hinzufügen
                if (continuum & listFolderTreeRight[listFolderTreeRight.Count() - 1].canFlyout)
                {
                    BitmapImage img = await loadIcon("MenuTop");
                    Windows.UI.Xaml.Controls.Image imgBtn = new Windows.UI.Xaml.Controls.Image();
                    imgBtn.Source = img;
                    imgBtn.Margin = new Thickness(8);
                    imgBtn.Tag = "svRight";
                    imgBtn.PointerReleased += BtnMenu_PointerReleased;
                    spMenuRight.Children.Add(imgBtn);
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Button Zurück
        // -----------------------------------------------------------------------
#region Button Zurück

        // Methode // Button Zurück
        private async void BtnBack_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {
                // Image kopieren
                Windows.UI.Xaml.Controls.Image image = (Windows.UI.Xaml.Controls.Image)sender;
                string side = image.Tag.ToString();

                // Wenn Links
                if (side == "left")
                {
                    // Ladeanzeige ausgeben
                    loadingShow(resource.GetString("001_EinenMoment") + "...", "left");

                    // Wenn aktueller Ordner // phone // sd // private
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "folderPhone" | listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "sd" | listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "private" | listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "temp")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenLeft != null)
                        {
                            cancelTokenLeft.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenLeft = new CancellationTokenSource();

                        // Ordner laden
                        await outputStart("left", cancelTokenLeft.Token);
                    }

                    // Wenn aktueller Ordner // Bilder // Musik // Video
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "folderPictures" | listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "folderMusic" | listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "folderVideo")
                    {
                        // Daten aus Ordnerbaum entfernen
                        listFolderTreeLeft.RemoveAt(listFolderTreeLeft.Count() - 1);
                        listFolderTreeLeft.RemoveAt(listFolderTreeLeft.Count() - 1);

                        // Vorherigen Task abbrechen
                        if (cancelTokenLeft != null)
                        {
                            cancelTokenLeft.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenLeft = new CancellationTokenSource();

                        // Phone Ordner laden
                        await outputPhone("left", cancelTokenLeft.Token);
                    }

                    // Wenn aktueller Ordner // Ordner
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "folder")
                    {
                        // Ordner auslesen
                        StorageFolder storageFolder = listFolderTreeLeft[listFolderTreeLeft.Count() - 2].storageFolder;
                        string descrition = listFolderTreeLeft[listFolderTreeLeft.Count() - 2].description;

                        // Daten aus Ordnerbaum entfernen
                        listFolderTreeLeft.RemoveAt(listFolderTreeLeft.Count() - 1);
                        listFolderTreeLeft.RemoveAt(listFolderTreeLeft.Count() - 1);

                        // Vorherigen Task abbrechen
                        if (cancelTokenLeft != null)
                        {
                            cancelTokenLeft.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenLeft = new CancellationTokenSource();

                        // Ordner ausgeben
                        await outputPhoneItems(listFolderTreeLeft[listFolderTreeLeft.Count() - 1].id, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType, storageFolder, descrition, "left", cancelTokenLeft.Token);
                    }

                    // Wenn aktueller Ordner // One Drive Folder
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "folderOneDrive")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenLeft != null)
                        {
                            cancelTokenLeft.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenLeft = new CancellationTokenSource();

                        // Wenn Start Ordner geladen wird
                        if (listFolderTreeLeft.Count() == 2)
                        {
                            await outputStart("left", cancelTokenLeft.Token);
                        }

                        // Wenn Pfad geladen wird
                        else
                        {
                            // Ordner auslesen
                            string path = listFolderTreeLeft[listFolderTreeLeft.Count() - 2].path;

                            // Daten aus Ordnerbaum entfernen
                            listFolderTreeLeft.RemoveAt(listFolderTreeLeft.Count() - 1);
                            listFolderTreeLeft.RemoveAt(listFolderTreeLeft.Count() - 1);

                            // Ordner ausgeben
                            await OneDriveLoadFolder(path, "left", cancelTokenLeft.Token);
                        }
                    }

                    // Wenn aktueller Ordner // Dropbox Folder
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].description == "folderDropbox")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenLeft != null)
                        {
                            cancelTokenLeft.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenLeft = new CancellationTokenSource();

                        // Wenn Start Ordner geladen wird
                        if (listFolderTreeLeft.Count() == 2)
                        {
                            await outputStart("left", cancelTokenLeft.Token);
                        }

                        // Wenn Pfad geladen wird
                        else
                        {
                            // Ordner auslesen
                            string path = listFolderTreeLeft[listFolderTreeLeft.Count() - 2].path;
                            string dropBoxToken = listFolderTreeLeft[listFolderTreeLeft.Count() - 2].token;

                            // Daten aus Ordnerbaum entfernen
                            listFolderTreeLeft.RemoveAt(listFolderTreeLeft.Count() - 1);
                            listFolderTreeLeft.RemoveAt(listFolderTreeLeft.Count() - 1);

                            // Ordner ausgeben
                            await DropboxLoadFolder(path, "left", dropBoxToken, cancelTokenLeft.Token);
                        }
                    }

                    // Ladeanzeigen verbergen
                    loadingHide("left");
                    prLeft.IsActive = false;
                }

                // Wenn Rechts
                else if (side == "right")
                {
                    // Ladeanzeige ausgeben
                    loadingShow(resource.GetString("001_EinenMoment") + "...", "right");

                    // Wenn aktueller Ordner // phone // sd
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "folderPhone" | listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "sd" | listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "private" | listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "temp")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenRight != null)
                        {
                            cancelTokenRight.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenRight = new CancellationTokenSource();

                        // Ordner laden
                        await outputStart("right", cancelTokenRight.Token);
                    }

                    // Wenn aktueler Ordner // Bilder // Musik // Video
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "folderPictures" | listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "folderMusic" | listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "folderVideo")
                    {
                        // Daten aus Ordnerbaum entfernen
                        listFolderTreeRight.RemoveAt(listFolderTreeRight.Count() - 1);
                        listFolderTreeRight.RemoveAt(listFolderTreeRight.Count() - 1);

                        // Vorherigen Task abbrechen
                        if (cancelTokenRight != null)
                        {
                            cancelTokenRight.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenRight = new CancellationTokenSource();

                        // Phone Ordner laden
                        await outputPhone("right", cancelTokenRight.Token);
                    }

                    // Wenn aktueller Ordner // Ordner
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "folder")
                    {
                        // Ordner auslesen
                        StorageFolder storageFolder = listFolderTreeRight[listFolderTreeRight.Count() - 2].storageFolder;
                        string descrition = listFolderTreeRight[listFolderTreeRight.Count() - 2].description;

                        // Daten aus Ordnerbaum entfernen
                        listFolderTreeRight.RemoveAt(listFolderTreeRight.Count() - 1);
                        listFolderTreeRight.RemoveAt(listFolderTreeRight.Count() - 1);

                        // Vorherigen Task abbrechen
                        if (cancelTokenRight != null)
                        {
                            cancelTokenRight.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenRight = new CancellationTokenSource();

                        // Ordner ausgeben
                        await outputPhoneItems(listFolderTreeRight[listFolderTreeRight.Count() - 1].id, listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType, storageFolder, descrition, "right", cancelTokenRight.Token);
                    }

                    // Wenn aktueller Ordner // One Drive Folder
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "folderOneDrive")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenRight != null)
                        {
                            cancelTokenRight.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenRight = new CancellationTokenSource();

                        // Wenn Start Ordner geladen wird
                        if (listFolderTreeRight.Count() == 2)
                        {
                            await outputStart("right", cancelTokenRight.Token);
                        }

                        // Wenn Pfad geladen wird
                        else
                        {
                            // Ordner auslesen
                            string path = listFolderTreeRight[listFolderTreeRight.Count() - 2].path;

                            // Daten aus Ordnerbaum entfernen
                            listFolderTreeRight.RemoveAt(listFolderTreeRight.Count() - 1);
                            listFolderTreeRight.RemoveAt(listFolderTreeRight.Count() - 1);

                            // Ordner ausgeben
                            await OneDriveLoadFolder(path, "right", cancelTokenRight.Token);
                        }
                    }

                    // Wenn aktueller Ordner // Dropbox Folder
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].description == "folderDropbox")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenRight != null)
                        {
                            cancelTokenRight.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenRight = new CancellationTokenSource();

                        // Wenn Start Ordner geladen wird
                        if (listFolderTreeRight.Count() == 2)
                        {
                            await outputStart("right", cancelTokenRight.Token);
                        }

                        // Wenn Pfad geladen wird
                        else
                        {
                            // Ordner auslesen
                            string path = listFolderTreeRight[listFolderTreeRight.Count() - 2].path;
                            string dropboxToken = listFolderTreeRight[listFolderTreeRight.Count() - 2].token;

                            // Daten aus Ordnerbaum entfernen
                            listFolderTreeRight.RemoveAt(listFolderTreeRight.Count() - 1);
                            listFolderTreeRight.RemoveAt(listFolderTreeRight.Count() - 1);

                            // Ordner ausgeben
                            await DropboxLoadFolder(path, "right", dropboxToken, cancelTokenRight.Token);
                        }
                    }

                    // Ladeanzeigen verbergen
                    loadingHide("right");
                    prRight.IsActive = false;
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Button Einzelauswahl und Mehrfachauswahl
        // -----------------------------------------------------------------------
#region Button Einzelauswahl und Mehrfachauswahl

        // Methode // Button Einzelauswahl und Mehrfachauswahl
        private async void BtnSelectionMode_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {
                // Image kopieren
                Windows.UI.Xaml.Controls.Image image = (Windows.UI.Xaml.Controls.Image)sender;
                string side = image.Tag.ToString();

                // Links
                if (side == "left")
                {
                    // Wenn Einzelauswahl
                    if (lbLeft.SelectionMode == SelectionMode.Single)
                    {
                        // ListBox auf Mehrfachauswahl stellen
                        lbLeft.SelectionMode = SelectionMode.Multiple;

                        // ListBox Items ändern
                        for (int i = 0; i < listFilesLeft.Count(); i++)
                        {
                            listFilesLeft[i].imgSelectedVisibility = Visibility.Visible;
                            listFilesLeft[i].unselectItem();
                        }

                        // Icon in Menüleiste ändern
                        image.Source = new BitmapImage(new Uri("ms-appx:/Images/MultiSelect.png", UriKind.RelativeOrAbsolute));
                        sender = image;
                    }

                    // Wenn Mehrfachauswahl
                    else
                    {
                        // ListBox auf Einzelauswahl stellen
                        lbLeft.SelectionMode = SelectionMode.Single;

                        // ListBox Items ändern
                        for (int i = 0; i < listFilesLeft.Count(); i++)
                        {
                            listFilesLeft[i].imgSelectedVisibility = Visibility.Collapsed;
                        }

                        // Icon in Menüleiste ändern
                        image.Source = new BitmapImage(new Uri("ms-appx:/Images/SingleSelect.png", UriKind.RelativeOrAbsolute));
                        sender = image;
                    }
                }

                // Rechts
                if (side == "right")
                {
                    // Wenn Einzelauswahl
                    if (lbRight.SelectionMode == SelectionMode.Single)
                    {
                        // ListBox auf Mehrfachauswahl stellen
                        lbRight.SelectionMode = SelectionMode.Multiple;

                        // ListBox Items ändern
                        for (int i = 0; i < listFilesRight.Count(); i++)
                        {
                            listFilesRight[i].imgSelectedVisibility = Visibility.Visible;
                            listFilesRight[i].unselectItem();
                        }

                        // Icon in Menüleiste ändern
                        image.Source = new BitmapImage(new Uri("ms-appx:/Images/MultiSelect.png", UriKind.RelativeOrAbsolute));
                        sender = image;
                    }

                    // Wenn Mehrfachauswahl
                    else
                    {
                        // ListBox auf Einzelauswahl stellen
                        lbRight.SelectionMode = SelectionMode.Single;

                        // ListBox Items ändern
                        for (int i = 0; i < listFilesRight.Count(); i++)
                        {
                            listFilesRight[i].imgSelectedVisibility = Visibility.Collapsed;
                        }

                        // Icon in Menüleiste ändern
                        image.Source = new BitmapImage(new Uri("ms-appx:/Images/SingleSelect.png", UriKind.RelativeOrAbsolute));
                        sender = image;
                    }
                }
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Button Ordner erstellen
        // -----------------------------------------------------------------------
#region Button Ordner erstellen

        // Methode // Button Ordner erstellen
        private async void BtnCreateFolder_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {
                // Image kopieren
                Windows.UI.Xaml.Controls.Image image = (Windows.UI.Xaml.Controls.Image)sender;
                string side = image.Tag.ToString();

                // Links
                if (side == "left")
                {
                    // Lokal
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder != null)
                    {
                        // Ordner erstellen
                        StorageFolder storageFolder = listFolderTreeLeft[listFolderTreeLeft.Count() - 1].storageFolder;
                        await CreateFolder(storageFolder, "left");
                    }

                    // One Drive
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                    {
                        // Ordner erstellen
                        await OneDriveCreateFolder(listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, "left");
                    }

                    // Dropbox
                    else if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                    {
                        // Ordner erstellen
                        await DropboxCreateFolder(listFolderTreeLeft[listFolderTreeLeft.Count() - 1].path, listFolderTreeLeft[listFolderTreeLeft.Count() - 1].token, "left");
                    }
                }

                // Rechts
                else if (side == "right")
                {
                    // Lokal
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder != null)
                    {
                        // Ordner erstellen
                        StorageFolder storageFolder = listFolderTreeRight[listFolderTreeRight.Count() - 1].storageFolder;
                        await CreateFolder(storageFolder, "right");
                    }

                    // One Drive
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                    {
                        // Ordner erstellen
                        await OneDriveCreateFolder(listFolderTreeRight[listFolderTreeRight.Count() - 1].path, "right");
                    }

                    // Dropbox
                    else if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                    {
                        // Ordner erstellen
                        await DropboxCreateFolder(listFolderTreeRight[listFolderTreeRight.Count() - 1].path, listFolderTreeRight[listFolderTreeRight.Count() - 1].token, "right");
                    }
                }
            }
        }



        // Ordner erstellen
        async Task CreateFolder(StorageFolder storageFolder, string side)
        {
            // Gibt an ob Ordner erstellt wurde
            bool isCreated = false;

            // Eingabeaufförderung erstellen
            DialogEx dialogEx = new DialogEx();
            dialogEx.BorderBrush = scbXtrose;
            dialogEx.Title = resource.GetString("001_NeuerOrdner");
            DialogExTextBox textBox_1 = new DialogExTextBox();
            textBox_1.Title = resource.GetString("001_Name");
            dialogEx.AddTextBox(textBox_1);
            DialogExButtonsSet buttonSet = new DialogExButtonsSet();
            DialogExButton button_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
            buttonSet.AddButton(button_1);
            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Hinzufügen"));
            buttonSet.AddButton(button_2);
            dialogEx.AddButtonSet(buttonSet);

            // Eingabeaufförderung ausgeben
            await dialogEx.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dialogEx.GetAnswer();

            // Wenn Ordner erstellt wird
            if (answer == resource.GetString("001_Hinzufügen"))
            {
                // Wenn Name vorhanden
                if (dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Name")) != "")
                {
                    // Versuchen Ordner zu erstellen
                    try
                    {
                        // Ordner erstellen
                        await storageFolder.CreateFolderAsync(dialogEx.GetTextBoxTextByTitle(resource.GetString("001_Name")), CreationCollisionOption.GenerateUniqueName);
                        // Angeben das Ordner erstelt wurde
                        isCreated = true;
                    }
                    catch (Exception exception)
                    {
                        // Bei Falschen Namen
                        if (exception.Message == "The filename, directory name, or volume label syntax is incorrect.\r\n" | exception.Message == "The parameter is incorrect.\r\n")
                        {
                            // Fehlermeldung
                            dialogEx = new DialogEx();
                            dialogEx.BorderBrush = scbXtrose;
                            dialogEx.Title = resource.GetString("001_OrdnerNichtErstellt");
                            dialogEx.Content = resource.GetString("001_NameFalsch");
                            buttonSet = new DialogExButtonsSet();
                            buttonSet.HorizontalAlignment = HorizontalAlignment.Right;
                            button_1 = new DialogExButton(resource.GetString("001_Schließen"));
                            buttonSet.AddButton(button_1);
                            dialogEx.AddButtonSet(buttonSet);
                            // Eingabeaufförderung ausgeben
                            await dialogEx.ShowAsync(grMain);
                        }

                        // Bei unbekanntem fehler
                        else
                        {
                            // Fehlermeldung
                            dialogEx = new DialogEx();
                            dialogEx.BorderBrush = scbXtrose;
                            dialogEx.Title = resource.GetString("001_OrdnerNichtErstellt");
                            buttonSet = new DialogExButtonsSet();
                            buttonSet.HorizontalAlignment = HorizontalAlignment.Right;
                            button_1 = new DialogExButton(resource.GetString("001_Schließen"));
                            buttonSet.AddButton(button_1);
                            dialogEx.AddButtonSet(buttonSet);
                            // Eingabeaufförderung ausgeben
                            await dialogEx.ShowAsync(grMain);
                        }
                    }
                }

                // Wenn kein Name vorhanden
                else
                {
                    // Fehlermeldung
                    dialogEx = new DialogEx();
                    dialogEx.BorderBrush = scbXtrose;
                    dialogEx.Title = resource.GetString("001_KeinName");
                    buttonSet = new DialogExButtonsSet();
                    buttonSet.HorizontalAlignment = HorizontalAlignment.Right;
                    button_1 = new DialogExButton(resource.GetString("001_Schließen"));
                    buttonSet.AddButton(button_1);
                    dialogEx.AddButtonSet(buttonSet);

                    // Eingabeaufförderung ausgeben
                    await dialogEx.ShowAsync(grMain);
                }
            }

            // Wenn Ordner erstellt wurde
            if (isCreated)
            {
                outputRefresh(side);
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Button sortieren
        // -----------------------------------------------------------------------
#region Button Sortieren

        // Methode // Button Sortieren
        private void BtnSort_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {
                // Alpha zum Abdecken einblenden
                grAlpha.Visibility = Visibility.Visible;

                // Image kopieren
                Windows.UI.Xaml.Controls.Image image = (Windows.UI.Xaml.Controls.Image)sender;
                string side = image.Tag.ToString();

                // Variablen
                bool isDropbox = false;

                // Gibt an was ausgegeben wird
                if (side == "left")
                {
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "dropbox")
                    {
                        isDropbox = true;
                    }
                }
                else if (side == "right")
                {
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "dropbox")
                    {
                        isDropbox = true;
                    }
                }

                // Flyout Menü erstellen
                menuFlyout = new MenuFlyout();
                menuFlyout.Closed += MenuFlyout_Closed;

                // Eintrag erstellen // Überschrift
                MenuFlyoutItem mfiHeader = new MenuFlyoutItem();
                mfiHeader.Text = resource.GetString("001_DateienSortieren");
                mfiHeader.FontWeight = FontWeights.Bold;
                mfiHeader.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiHeader);

                // Eintrag erstellen // Name aufsteigend
                MenuFlyoutItem mfiName = new MenuFlyoutItem();
                mfiName.Text = resource.GetString("001_Name") + " ↑";
                mfiName.Tag = "name↑~" + side;
                mfiName.Click += MfiSortFiles_Click;
                mfiName.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName);

                // Eintrag erstellen // Name absteigend
                MenuFlyoutItem mfiName2 = new MenuFlyoutItem();
                mfiName2.Text = resource.GetString("001_Name") + " ↓";
                mfiName2.Tag = "name↓~" + side;
                mfiName2.Click += MfiSortFiles_Click;
                mfiName2.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName2);

                // Eintrag erstellen // Datum aufsteigend
                MenuFlyoutItem mfiDate = new MenuFlyoutItem();
                mfiDate.Text = resource.GetString("001_Datum") + " ↑";
                mfiDate.Tag = "date↑~" + side;
                mfiDate.Click += MfiSortFiles_Click;
                mfiDate.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiDate);

                // Eintrag erstellen // Datum absteigend
                MenuFlyoutItem mfiDate2 = new MenuFlyoutItem();
                mfiDate2.Text = resource.GetString("001_Datum") + " ↓";
                mfiDate2.Tag = "date↓~" + side;
                mfiDate2.Click += MfiSortFiles_Click;
                mfiDate2.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiDate2);

                // Eintrag erstellen // Datei Typ aufsteigend
                MenuFlyoutItem mfiType = new MenuFlyoutItem();
                mfiType.Text = resource.GetString("001_FileType") + " ↑";
                mfiType.Tag = "type↑~" + side;
                mfiType.Click += MfiSortFiles_Click;
                mfiType.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiType);

                // Eintrag erstellen // Datei Typ absteigend
                MenuFlyoutItem mfiType2 = new MenuFlyoutItem();
                mfiType2.Text = resource.GetString("001_FileType") + " ↓";
                mfiType2.Tag = "type↓~" + side;
                mfiType2.Click += MfiSortFiles_Click;
                mfiType2.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiType2);

                // Trennlinie einfügen
                MenuFlyoutSeparator mfiSeperator = new MenuFlyoutSeparator();
                mfiSeperator.Margin = new Thickness(0, -3, 0, -3);
                menuFlyout.Items.Add(mfiSeperator);

                // Eintrag erstellen // Überschrift
                MenuFlyoutItem mfiHeader2 = new MenuFlyoutItem();
                mfiHeader2.Text = resource.GetString("001_OrdnerSortieren");
                mfiHeader2.FontWeight = FontWeights.Bold;
                mfiHeader2.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiHeader2);

                // Eintrag erstellen // Name aufsteigend
                MenuFlyoutItem mfiName3 = new MenuFlyoutItem();
                mfiName3.Text = resource.GetString("001_Name") + " ↑";
                mfiName3.Tag = "name↑~" + side;
                mfiName3.Click += MfiSortFolders_Click;
                mfiName3.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName3);

                // Eintrag erstellen // Name absteigend
                MenuFlyoutItem mfiName4 = new MenuFlyoutItem();
                mfiName4.Text = resource.GetString("001_Name") + " ↓";
                mfiName4.Tag = "name↓~" + side;
                mfiName4.Click += MfiSortFolders_Click;
                mfiName4.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName4);

                // Nicht bei Dropbox
                if (!isDropbox)
                {
                    // Eintrag erstellen // Datum aufsteigend
                    MenuFlyoutItem mfiDate3 = new MenuFlyoutItem();
                    mfiDate3.Text = resource.GetString("001_Datum") + " ↑";
                    mfiDate3.Tag = "date↑~" + side;
                    mfiDate3.Click += MfiSortFolders_Click;
                    mfiDate3.Margin = new Thickness(0, -6, 0, -6);
                    menuFlyout.Items.Add(mfiDate3);

                    // Eintrag erstellen // Datum absteigend
                    MenuFlyoutItem mfiDate4 = new MenuFlyoutItem();
                    mfiDate4.Text = resource.GetString("001_Datum") + " ↓";
                    mfiDate4.Tag = "date↓~" + side;
                    mfiDate4.Click += MfiSortFolders_Click;
                    mfiDate4.Margin = new Thickness(0, -6, 0, -6);
                    menuFlyout.Items.Add(mfiDate4);
                }

                // Menü ausgeben
                menuFlyout.ShowAt((FrameworkElement)sender);
            }
        }



        // Button Sort Files // Pointer Click
        private void MfiSortFiles_Click(object sender, RoutedEventArgs e)
        {
            // Kopie aus Sender erstellen
            MenuFlyoutItem mfiCopy = (MenuFlyoutItem)sender;
            string[] spTag = Regex.Split(mfiCopy.Tag.ToString(), "~");
            string sort = spTag[0];
            string side = spTag[1];

            // Links
            if (side == "left")
            {
                // Sortierung ändern
                setSortLeftFiles = sort;

                // Ausgabe erneuern
                outputRefresh("leftOnly");
            }

            // Rechts
            else if (side == "right")
            {
                // Sortierung ändern
                setSortRightFiles = sort;

                // Ausgabe erneuern
                outputRefresh("rightOnly");
            }

            // Alpha zum Abdecken ausblenden
            grAlpha.Visibility = Visibility.Collapsed;
        }



        // Button Sort Folders // Pointer Click
        private void MfiSortFolders_Click(object sender, RoutedEventArgs e)
        {
            // Kopie aus Sender erstellen
            MenuFlyoutItem mfiCopy = (MenuFlyoutItem)sender;
            string[] spTag = Regex.Split(mfiCopy.Tag.ToString(), "~");
            string sort = spTag[0];
            string side = spTag[1];

            // Links
            if (side == "left")
            {
                // Sortierung ändern
                setSortLeftFolders = sort;

                // Ausgabe erneuern
                outputRefresh("leftOnly");
            }

            // Rechts
            else if (side == "right")
            {
                // Sortierung ändern
                setSortRightFolders = sort;

                // Ausgabe erneuern
                outputRefresh("rightOnly");
            }

            // Alpha zum Abdecken ausblenden
            grAlpha.Visibility = Visibility.Collapsed;
        }
#endregion
        // -----------------------------------------------------------------------





        // Button hinzufügen
        // -----------------------------------------------------------------------
#region Button hinzufügen

        // Methode // Button hinzufügen
        private async void BtnAdd_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {
                // Zum Menü hinzufügen
                Frame.Navigate(typeof(Add));
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Button Einstellungen
        // -----------------------------------------------------------------------
#region Button Einstellungen

        // Methode // Button Einstellungen
        private void BtnSettings_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {
                // Zu Einstellungen wechseln
                Frame.Navigate(typeof(Settings));
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Button Hilfe
        // -----------------------------------------------------------------------
#region Button Hilfe

        // Methode // Button Einstellungen
        private async void BtnHelp_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {
                // Zu Einstellungen wechseln
                await Launcher.LaunchUriAsync(new Uri("http://www.xtrose.com/index.php?s=app&i=44&pa=pg&pgid=45"));
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Button Über
        // -----------------------------------------------------------------------
#region Button Über

        // Methode // Button Einstellungen
        private void BtnAbout_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {
                // Zu Einstellungen wechseln
                Frame.Navigate(typeof(About));
            }
        }
        #endregion
        // -----------------------------------------------------------------------





        // Button Größe
        // -----------------------------------------------------------------------
#region Button Größe

        // Methode // Button Große
        private void BtnSize_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {

                // Alpha zum Abdecken einblenden
                grAlpha.Visibility = Visibility.Visible;

                // Image kopieren
                Windows.UI.Xaml.Controls.Image image = (Windows.UI.Xaml.Controls.Image)sender;
                string side = image.Tag.ToString();

                // Flyout Menü erstellen
                menuFlyout = new MenuFlyout();
                menuFlyout.Closed += MenuFlyout_Closed;

                // Eintrag erstellen
                MenuFlyoutItem mfiName = new MenuFlyoutItem();
                mfiName.Text = "50%";
                mfiName.Tag = side + "~0.5";
                mfiName.Click += MfiSize_Click;
                mfiName.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName);

                // Eintrag erstellen
                MenuFlyoutItem mfiName2 = new MenuFlyoutItem();
                mfiName2.Text = "75%";
                mfiName2.Tag = side + "~0.75";
                mfiName2.Click += MfiSize_Click;
                mfiName2.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName2);

                // Eintrag erstellen
                MenuFlyoutItem mfiName3 = new MenuFlyoutItem();
                mfiName3.Text = "100%";
                mfiName3.Tag = side + "~1.0";
                mfiName3.Click += MfiSize_Click;
                mfiName3.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName3);

                // Eintrag erstellen
                MenuFlyoutItem mfiName4 = new MenuFlyoutItem();
                mfiName4.Text = "125%";
                mfiName4.Tag = side + "~1.25";
                mfiName4.Click += MfiSize_Click;
                mfiName4.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName4);

                // Eintrag erstellen
                MenuFlyoutItem mfiName5 = new MenuFlyoutItem();
                mfiName5.Text = "150%";
                mfiName5.Tag = side + "~1.5";
                mfiName5.Click += MfiSize_Click;
                mfiName5.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName5);

                // Eintrag erstellen
                MenuFlyoutItem mfiName6 = new MenuFlyoutItem();
                mfiName6.Text = "175%";
                mfiName6.Tag = side + "~1.75";
                mfiName6.Click += MfiSize_Click;
                mfiName6.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName6);

                // Eintrag erstellen
                MenuFlyoutItem mfiName7 = new MenuFlyoutItem();
                mfiName7.Text = "200%";
                mfiName7.Tag = side + "~2.0";
                mfiName7.Click += MfiSize_Click;
                mfiName7.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName7);

                // Eintrag erstellen
                MenuFlyoutItem mfiName8 = new MenuFlyoutItem();
                mfiName8.Text = "225%";
                mfiName8.Tag = side + "~2.25";
                mfiName8.Click += MfiSize_Click;
                mfiName8.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName8);

                // Eintrag erstellen
                MenuFlyoutItem mfiName9 = new MenuFlyoutItem();
                mfiName9.Text = "250%";
                mfiName9.Tag = side + "~2.5";
                mfiName9.Click += MfiSize_Click;
                mfiName9.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName9);

                // Eintrag erstellen
                MenuFlyoutItem mfiName10 = new MenuFlyoutItem();
                mfiName10.Text = "275%";
                mfiName10.Tag = side + "~2.75";
                mfiName10.Click += MfiSize_Click;
                mfiName10.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName10);

                // Eintrag erstellen
                MenuFlyoutItem mfiName11 = new MenuFlyoutItem();
                mfiName11.Text = "300%";
                mfiName11.Tag = side + "~3.0";
                mfiName11.Click += MfiSize_Click;
                mfiName11.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName11);

                // Eintrag erstellen
                MenuFlyoutItem mfiName12 = new MenuFlyoutItem();
                mfiName12.Text = "350%";
                mfiName12.Tag = side + "~3.5";
                mfiName12.Click += MfiSize_Click;
                mfiName12.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName12);

                // Eintrag erstellen
                MenuFlyoutItem mfiName13 = new MenuFlyoutItem();
                mfiName13.Text = "400%";
                mfiName13.Tag = side + "~4.0";
                mfiName13.Click += MfiSize_Click;
                mfiName13.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName13);

                // Eintrag erstellen
                MenuFlyoutItem mfiName14 = new MenuFlyoutItem();
                mfiName14.Text = "450%";
                mfiName14.Tag = side + "~4.5";
                mfiName14.Click += MfiSize_Click;
                mfiName14.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName14);

                // Eintrag erstellen
                MenuFlyoutItem mfiName15 = new MenuFlyoutItem();
                mfiName15.Text = "500%";
                mfiName15.Tag = side + "~5.0";
                mfiName15.Click += MfiSize_Click;
                mfiName15.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName15);

                // Eintrag erstellen
                MenuFlyoutItem mfiName16 = new MenuFlyoutItem();
                mfiName16.Text = "600%";
                mfiName16.Tag = side + "~6.0";
                mfiName16.Click += MfiSize_Click;
                mfiName16.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName16);

                // Eintrag erstellen
                MenuFlyoutItem mfiName17 = new MenuFlyoutItem();
                mfiName17.Text = "700%";
                mfiName17.Tag = side + "~7.0";
                mfiName17.Click += MfiSize_Click;
                mfiName17.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName17);

                // Eintrag erstellen
                MenuFlyoutItem mfiName18 = new MenuFlyoutItem();
                mfiName18.Text = "800%";
                mfiName18.Tag = side + "~8.0";
                mfiName18.Click += MfiSize_Click;
                mfiName18.Margin = new Thickness(0, -6, 0, -6);
                menuFlyout.Items.Add(mfiName18);

                // Menü ausgeben
                menuFlyout.ShowAt((FrameworkElement)sender);
            }
        }



        // Button Size // Pointer Click
        private void MfiSize_Click(object sender, RoutedEventArgs e)
        {
            // Kopie aus Sender erstellen
            MenuFlyoutItem mfiCopy = (MenuFlyoutItem)sender;
            string[] spTag = Regex.Split(mfiCopy.Tag.ToString(), "~");
            string side = spTag[0];
            double size = double.Parse(spTag[1], System.Globalization.CultureInfo.InvariantCulture);

            // Links
            if (side == "left")
            {
                // Icon Größe ändern
                setIconSizeLeft = size;

                // Ausgabe erneuern
                outputRefresh("leftOnly");
            }

            // Rechts
            else if (side == "right")
            {
                // Icon Größe ändern
                setIconSizeRight = size;

                // Ausgabe erneuern
                outputRefresh("rightOnly");
            }

            // Alpha zum Abdecken ausblenden
            grAlpha.Visibility = Visibility.Collapsed;
        }
        #endregion



        // Button Menü
        // -----------------------------------------------------------------------
#region Button Menü

        // Methode // Button Menü
        private void BtnMenu_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            // Wenn Drag and Drop nich aktiv
            if (!dd)
            {
                // Sender kopieren
                Windows.UI.Xaml.Controls.Image copy = (sender) as Windows.UI.Xaml.Controls.Image;
                mfSource = copy.Tag.ToString();

                // Alpha zum Abdecken einblenden
                grAlpha.Visibility = Visibility.Visible;

                // Flyout Menü ausgeben // Methode
                createAndShowFlyout(sender);
            }
        }
#endregion
        // -----------------------------------------------------------------------
#endregion










#region One Drive
        // -----------------------------------------------------------------------
        #region One Drive verbinden

        // Task // One Drive verbinden
        private async Task<string> OneDriveConnect()
        {
            // One Drive Daten
            string oneDriveclientId = "00000000481B6853";
            string[] oneDriveScopes = new[] { "wl.signin", "onedrive.appfolder", "onedrive.readwrite" };

            // Nachricht ausgeben
            DialogEx dQuery = new DialogEx();
            dQuery.BorderBrush = scbXtrose;
            dQuery.Title = resource.GetString("001_ArtDerVerbindung");
            DialogExButtonsSet btnSet = new DialogExButtonsSet();
            DialogExButton btn_1 = new DialogExButton(resource.GetString("001_VerknüpftesKonto"));
            btnSet.AddButton(btn_1);
            DialogExButton btn_2 = new DialogExButton(resource.GetString("001_AnderesKonto"));
            btnSet.AddButton(btn_2);
            DialogExButton btn_3 = new DialogExButton(resource.GetString("001_Abbrechen"));
            btnSet.AddButton(btn_3);
            dQuery.AddButtonSet(btnSet);
            await dQuery.ShowAsync(grMain);

            // Lade Anzeige augeben
            DialogEx dWaitAbort = new DialogEx();
            dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
            DialogExProgressRing progressRing = new DialogExProgressRing();
            dWaitAbort.AddProgressRing(progressRing);
            dWaitAbort.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dQuery.GetAnswer();

            // Verbinden mit webauthenticationbroker
            if (answer == resource.GetString("001_AnderesKonto"))
            {
                try
                {
                    oneDriveClient = OneDriveClientExtensions.GetClientUsingWebAuthenticationBroker(oneDriveclientId, oneDriveScopes);
                    var session = await oneDriveClient.AuthenticateAsync();
                    oneDriveToken = ($"Token: {session.AccessToken}");
                }
                catch { }
            }

            // Verbinden mit verknüpften Account
            if (answer == resource.GetString("001_VerknüpftesKonto"))
            {
                try
                {
                    // Verbinden
                    oneDriveClient = OneDriveClientExtensions.GetClientUsingOnlineIdAuthenticator(oneDriveScopes);
                    var session = await oneDriveClient.AuthenticateAsync();
                    oneDriveToken = ($"Token: {session.AccessToken}");
                }
                catch { }
            }

            // Ladeanzeige verbergen
            dWaitAbort.Hide();

            // Rückgabe
            return oneDriveToken;
        }
#endregion
        // -----------------------------------------------------------------------





        // One Drive Verzeichnis laden
        // -----------------------------------------------------------------------
#region One Drive Verzeichnis laden

        // Task // One Drive Verzeichnis laden
        private async Task OneDriveLoadFolder(string path, string side, CancellationToken token, bool addToFolderTree = true, bool createMenu = true)
        {
            // Wenn kein Token vorhanden
            if (oneDriveToken == "")
            {
                // One Drive verbinden
                oneDriveToken = await OneDriveConnect();
            }

            // Wenn token vorhanden
            if (oneDriveToken != "")
            {

                // Ob geladen wurde
                bool connect = true;

                // IChildrenCollectionPage erstellen
                IChildrenCollectionPage itemsLoad = null;

                // Versuchen Daten zu laden
                try
                {
                    // Ordner und Dateien laden // Root
                    if (path == null)
                    {
                        itemsLoad = await oneDriveClient.Drive.Root.Children.Request().GetAsync();
                    }

                    // Ordner und Dateien laden // Nach Pfad
                    else
                    {
                        itemsLoad = await oneDriveClient.Drive.Root.ItemWithPath(path).Children.Request().GetAsync();
                    }
                }
                catch
                {
                    // Gib an das nichts geladen wurde
                    connect = false;
                }

                // Wenn Daten geladen wurden
                if (connect)
                {
                    // Liste Items Dateien erstellen
                    List<Item> itemsFiles = new List<Item>();

                    // Liste Items Ordner erstellen
                    List<Item> itemsFolders = new List<Item>();

                    // Liste  Items erstellen
                    List<Item> items = new List<Item>();

                    // Dateien und Ordner trennen
                    for (int i = 0; i < itemsLoad.Count(); i++)
                    {
                        // Wenn Ordner
                        if (itemsLoad[i].Folder != null)
                        {
                            itemsFolders.Add(itemsLoad[i]);
                        }

                        // Wenn Datei
                        else
                        {
                            itemsFiles.Add(itemsLoad[i]);
                        }
                    }

                    // Links
                    if (side == "left")
                    {
                        // Dateien sortieren // Name aufsteigend
                        if (setSortLeftFiles == "name↑")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderBy((x) => x.Name);
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Name absteigend
                        else if (setSortLeftFiles == "name↓")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderByDescending((x) => x.Name);
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Datum aufsteigend
                        else if (setSortLeftFiles == "date↑")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderBy((x) => x.CreatedDateTime);
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Datum absteigend
                        else if (setSortLeftFiles == "date↓")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderByDescending((x) => x.CreatedDateTime);
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Type aufsteigend
                        else if (setSortLeftFiles == "type↑")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderBy((x) => x.GetType());
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Type absteigend
                        else if (setSortLeftFiles == "type↓")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderByDescending((x) => x.GetType());
                            itemsFiles = sort.ToList();
                        }

                        // Ordner sortieren // Name aufsteigend
                        if (setSortLeftFolders == "name↑")
                        {
                            IEnumerable<Item> sort = itemsFolders.OrderBy((x) => x.Name);
                            itemsFolders = sort.ToList();
                        }

                        // Ordner sortieren // Name absteigend
                        else if (setSortLeftFolders == "name↓")
                        {
                            IEnumerable<Item> sort = itemsFolders.OrderByDescending((x) => x.Name);
                            itemsFolders = sort.ToList();
                        }

                        // Ordner sortieren // Datum aufsteigend
                        else if (setSortLeftFolders == "date↑")
                        {
                            IEnumerable<Item> sort = itemsFolders.OrderBy((x) => x.CreatedDateTime);
                            itemsFolders = sort.ToList();
                        }

                        // Ordner sortieren // Datum absteigend
                        else if (setSortLeftFolders == "date↓")
                        {
                            IEnumerable<Item> sort = itemsFolders.OrderByDescending((x) => x.CreatedDateTime);
                            itemsFolders = sort.ToList();
                        }

                        // Items zusammenfügen
                        for (int i = 0; i < itemsFolders.Count(); i++)
                        {
                            items.Add(itemsFolders[i]);
                        }
                        for (int i = 0; i < itemsFiles.Count(); i++)
                        {
                            items.Add(itemsFiles[i]);
                        }

                        // Ordnerbaum hinzufügen
                        if (addToFolderTree)
                        {
                            listFolderTreeLeft.Add(new ClassFolderTree(-1, "oneDrive", null, null, path, "folderOneDrive", true, true, true, false, true, true, true, true));
                        }

                        // listFiles Leeren
                        listFilesLeft.Clear();

                        // MultiSelect entfernen
                        lbLeft.SelectionMode = SelectionMode.Single;

                        // Ladeanzeigen ändern
                        loadingHide("left");
                        prLeft.IsActive = true;

                        // Ordner durchlaufen
                        for (int i = 0; i < items.Count(); i++)
                        {

                            // Wenn Task nicht abgebrochen wurde
                            if (!token.IsCancellationRequested)
                            {
                                // Drop // Delete // Cut
                                bool canDrop = true;

                                // Drop // Delete // Cut prüfen
                                try
                                {
                                    if (items[i].Permissions.IsReadOnly)
                                    {
                                        canDrop = false;
                                    }
                                }
                                catch { }

                                // Erstellt Zeit erstellen
                                DateTimeOffset dTO = Convert.ToDateTime(Convert.ToString(items[i].CreatedDateTime));

                                // Wenn ein Ordner
                                if (items[i].File == null)
                                {

                                    // Wenn Task nicht abgebrochen wurde
                                    if (!token.IsCancellationRequested)
                                    {
                                        // In listFiles hinzufügen
                                        listFilesLeft.Add(new ClassFiles(i, "left", "oneDrive", "folder", items[i].Name, dTO, null, null, items[i], null, await loadIcon("folder"), true, canDrop, true, canDrop, true, canDrop, canDrop, true, false, false));
                                    }
                                }

                                // Wenn eine Datei
                                else
                                {
                                    // Bild Ausgabe erstellen
                                    string fileIcon;

                                    // Datei prüfen
                                    if (items[i].Audio != null)
                                    {
                                        fileIcon = "fileAudio";
                                    }
                                    if (items[i].Video != null)
                                    {
                                        fileIcon = "fileVideo";
                                    }
                                    if (items[i].Photo != null)
                                    {
                                        fileIcon = "filePhoto";
                                    }
                                    else
                                    {
                                        fileIcon = "file";
                                    }

                                    // Wenn Task nicht abgebrochen wurde
                                    if (!token.IsCancellationRequested)
                                    {
                                        // In ListFiles hinzufügen
                                        listFilesLeft.Add(new ClassFiles(i, "left", "oneDrive", "file", items[i].Name, dTO, null, null, items[i], null, await loadIcon(fileIcon), true, false, true, canDrop, true, canDrop, false, true, false, false));
                                    }
                                }
                            }

                            // Wenn Abgebrochen
                            else
                            {
                                break;
                            }
                        }

                        // Menü erstellen
                        if (createMenu)
                        {
                            createAndShowMenu("left");
                        }
                    }

                    // Rechts
                    else if (side == "right")
                    {
                        // Dateien sortieren // Name aufsteigend
                        if (setSortRightFiles == "name↑")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderBy((x) => x.Name);
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Name absteigend
                        else if (setSortRightFiles == "name↓")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderByDescending((x) => x.Name);
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Datum aufsteigend
                        else if (setSortRightFiles == "date↑")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderBy((x) => x.CreatedDateTime);
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Datum absteigend
                        else if (setSortRightFiles == "date↓")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderByDescending((x) => x.CreatedDateTime);
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Type aufsteigend
                        else if (setSortRightFiles == "type↑")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderBy((x) => x.GetType());
                            itemsFiles = sort.ToList();
                        }

                        // Dateien sortieren // Type absteigend
                        else if (setSortRightFiles == "type↓")
                        {
                            IEnumerable<Item> sort = itemsFiles.OrderByDescending((x) => x.GetType());
                            itemsFiles = sort.ToList();
                        }

                        // Ordner sortieren // Name aufsteigend
                        if (setSortRightFolders == "name↑")
                        {
                            IEnumerable<Item> sort = itemsFolders.OrderBy((x) => x.Name);
                            itemsFolders = sort.ToList();
                        }

                        // Ordner sortieren // Name absteigend
                        else if (setSortRightFolders == "name↓")
                        {
                            IEnumerable<Item> sort = itemsFolders.OrderByDescending((x) => x.Name);
                            itemsFolders = sort.ToList();
                        }

                        // Ordner sortieren // Datum aufsteigend
                        else if (setSortRightFolders == "date↑")
                        {
                            IEnumerable<Item> sort = itemsFolders.OrderBy((x) => x.CreatedDateTime);
                            itemsFolders = sort.ToList();
                        }

                        // Ordner sortieren // Datum absteigend
                        else if (setSortRightFolders == "date↓")
                        {
                            IEnumerable<Item> sort = itemsFolders.OrderByDescending((x) => x.CreatedDateTime);
                            itemsFolders = sort.ToList();
                        }

                        // Items zusammenfügen
                        for (int i = 0; i < itemsFolders.Count(); i++)
                        {
                            items.Add(itemsFolders[i]);
                        }
                        for (int i = 0; i < itemsFiles.Count(); i++)
                        {
                            items.Add(itemsFiles[i]);
                        }

                        // Ordnerbaum hinzufügen
                        if (addToFolderTree)
                        {
                            listFolderTreeRight.Add(new ClassFolderTree(-1, "oneDrive", null, null, path, "folderOneDrive", true, true, true, false, true, true, true, true));
                        }

                        // listFiles Leeren
                        listFilesRight.Clear();

                        // MultiSelect entfernen
                        lbRight.SelectionMode = SelectionMode.Single;

                        // Ladeanzeigen ändern
                        loadingHide("right");
                        prRight.IsActive = true;

                        // Ordner durchlaufen
                        for (int i = 0; i < items.Count(); i++)
                        {
                            // Wenn Task nicht abgebrochen wurde
                            if (!token.IsCancellationRequested)
                            {

                                // Drop // Delete // Cut
                                bool canDrop = true;

                                // Drop // Delete // Cut prüfen
                                try
                                {
                                    if (items[i].Permissions.IsReadOnly)
                                    {
                                        canDrop = false;
                                    }
                                }
                                catch { }

                                DateTimeOffset dTO = Convert.ToDateTime(Convert.ToString(items[i].CreatedDateTime));

                                // Wenn ein Ordner
                                if (items[i].File == null)
                                {
                                    // Wenn Task nicht abgebrochen wurde
                                    if (!token.IsCancellationRequested)
                                    {
                                        // In listFiles hinzufügen
                                        listFilesRight.Add(new ClassFiles(i, "right", "oneDrive", "folder", items[i].Name, dTO, null, null, items[i], null, await loadIcon("folder"), true, canDrop, true, canDrop, true, canDrop, canDrop, true, false, false));
                                    }
                                }

                                // Wenn eine Datei
                                else
                                {
                                    // Bild Ausgabe erstellen
                                    string fileIcon;

                                    // Datei prüfen
                                    if (items[i].Audio != null)
                                    {
                                        fileIcon = "fileAudio";
                                    }
                                    if (items[i].Video != null)
                                    {
                                        fileIcon = "fileVideo";
                                    }
                                    if (items[i].Photo != null)
                                    {
                                        fileIcon = "filePhoto";
                                    }
                                    else
                                    {
                                        fileIcon = "file";
                                    }

                                    // Wenn Task nicht abgebrochen wurde
                                    if (!token.IsCancellationRequested)
                                    {
                                        // In ListFiles hinzufügen
                                        listFilesRight.Add(new ClassFiles(i, "right", "oneDrive", "file", items[i].Name, dTO, null, null, items[i], null, await loadIcon(fileIcon), true, false, true, canDrop, true, canDrop, false, true, false, false));
                                    }
                                }
                            }

                            // Wenn Abgebrochen
                            else
                            {
                                break;
                            }
                        }

                        // Menü erstellen
                        if (createMenu)
                        {
                            createAndShowMenu("right");
                        }
                    }
                }

                // Wenn Verbindung fehlgeschlagen
                else
                {
                    // Token löschen
                    oneDriveToken = "";

                    // Nachricht ausgeben // Verbindung nicht möglich
                    DialogEx dEx = new DialogEx(resource.GetString("001_VerbindungNicht"));
                    DialogExButtonsSet dBSet = new DialogExButtonsSet();
                    DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                    dBSet.AddButton(dBtn);
                    dEx.AddButtonSet(dBSet);
                    await dEx.ShowAsync(grMain);

                    // Fenster aktuallisieren
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenLeft != null)
                        {
                            cancelTokenLeft.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenLeft = new CancellationTokenSource();

                        // Fenster links erneuern
                        await outputStart("left", cancelTokenLeft.Token);
                    }

                    // Fenster aktuallisieren
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenRight != null)
                        {
                            cancelTokenRight.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenRight = new CancellationTokenSource();

                        // Fenster links erneuern
                        await outputStart("right", cancelTokenLeft.Token);
                    }
                }
            }

            // Wenn kein Token vorhanden
            else
            {
                // Nachricht ausgeben // Verbindung nicht möglich
                DialogEx dEx = new DialogEx(resource.GetString("001_VerbindungNicht"));
                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                dBSet.AddButton(dBtn);
                dEx.AddButtonSet(dBSet);
                await dEx.ShowAsync(grMain);
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // One Drive Datei öffnen durch Download
        // -----------------------------------------------------------------------
#region One Drive Datei öffnen durch Download

        // Methode // One Drive Datei öffnen durch Download
        private async Task OneDriveOpenFileByDownload(Item item, DialogEx dWaitAbort)
        {
            // Pfad erstellen
            string path = item.ParentReference.Path;
            path = Regex.Replace(path, ":", "/");
            path += "/" + item.Name;
            path = Regex.Replace(path, "/drive/root/", "");

            // Ob Abbruch durch Benutzer
            bool abort = false;

            // Datei erstellen
            StorageFile file = await folderTemp.CreateFileAsync(item.Name, CreationCollisionOption.GenerateUniqueName);
            var streamFile = await file.OpenAsync(FileAccessMode.ReadWrite);

            // Datei herunterladen
            var builder = oneDriveClient.Drive.Root.ItemWithPath(path);

            // Filestream erstellen
            using (Stream fileStream = await builder.Content.Request().GetAsync())
            {
                // BinaryReader erstellen
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    // Puffer erstellen
                    byte[] buffer = new byte[setBufferSize];
                    int lenght;
                    int totalBytes = 0;

                    // Schleife Download
                    while ((lenght = binaryReader.Read(buffer, 0, setBufferSize)) > 0)
                    {
                        // Wenn Abbruch durch Benutzer
                        if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                        {
                            abort = true;
                            break;
                        }

                        // Datei als Stream herunterladen
                        await streamFile.WriteAsync(buffer.AsBuffer(0, lenght));
                        totalBytes += lenght;
                        dWaitAbort.SetProgressBarValueByIndex(0, totalBytes);
                    }
                }
            }

            // Datei öffnen
            if (!abort)
            {
                // Datei öffnen
                StorageFile fileToOpen = await folderTemp.GetFileAsync(file.Name);
                var options = new LauncherOptions();
                options.DisplayApplicationPicker = true;
                bool success = await Launcher.LaunchFileAsync(fileToOpen, options);
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // One Drive Ordner erstellen
        // -----------------------------------------------------------------------
#region One Drive Ordner erstellen

        // Methode // One Drive Ordner erstellen
        private async Task OneDriveCreateFolder(string path, string side)
        {
            // Gibt an ob Ordner erstellt wurde
            bool isCreated = false;

            // Eingabeaufförderung erstellen
            DialogEx dEx = new DialogEx();
            dEx.BorderBrush = scbXtrose;
            dEx.Title = resource.GetString("001_NeuerOrdner");
            DialogExTextBox textBox_1 = new DialogExTextBox();
            textBox_1.Title = resource.GetString("001_Name");
            dEx.AddTextBox(textBox_1);
            DialogExButtonsSet buttonSet = new DialogExButtonsSet();
            DialogExButton button_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
            buttonSet.AddButton(button_1);
            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Hinzufügen"));
            buttonSet.AddButton(button_2);
            dEx.AddButtonSet(buttonSet);

            // Eingabeaufförderung ausgeben
            await dEx.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dEx.GetAnswer();

            // Wenn Ordner erstellt wird
            if (answer == resource.GetString("001_Hinzufügen"))
            {
                // Wenn Name vorhanden
                if (dEx.GetTextBoxTextByTitle(resource.GetString("001_Name")) != "")
                {
                    // Versuchen Ordner zu erstellen
                    try
                    {
                        // Temponären Ordner erstellen und loschen
                        StorageFolder folderTest = await folderTemp.CreateFolderAsync(dEx.GetTextBoxTextByTitle(resource.GetString("001_Name")), CreationCollisionOption.ReplaceExisting);
                        await folderTest.DeleteAsync();

                        // Ordner auf One Drive erstellen
                        var newFolder = new Item
                        {
                            Name = dEx.GetTextBoxTextByTitle(resource.GetString("001_Name")),
                            Folder = new Folder(),
                        };

                        // Ordner erstellen // Root
                        if (path == null)
                        {
                            var newFolderCreated = await oneDriveClient.Drive.Root.Children.Request().AddAsync(newFolder);
                        }

                        // Ordner erstellen // Nach Pfad
                        else
                        {
                            var newFolderCreated = await oneDriveClient.Drive.Root.ItemWithPath(path).Children.Request().AddAsync(newFolder);
                        }

                        // Angeben das Ordner erstelt wurde
                        isCreated = true;
                    }
                    catch (Exception exception)
                    {
                        // Bei Falschen Namen
                        if (exception.Message == "The filename, directory name, or volume label syntax is incorrect.\r\n" | exception.Message == "The parameter is incorrect.\r\n")
                        {
                            // Fehlermeldung
                            dEx = new DialogEx();
                            dEx.BorderBrush = scbXtrose;
                            dEx.Title = resource.GetString("001_OrdnerNichtErstellt");
                            dEx.Content = resource.GetString("001_NameFalsch");
                            buttonSet = new DialogExButtonsSet();
                            buttonSet.HorizontalAlignment = HorizontalAlignment.Right;
                            button_1 = new DialogExButton(resource.GetString("001_Schließen"));
                            buttonSet.AddButton(button_1);
                            dEx.AddButtonSet(buttonSet);
                            // Eingabeaufförderung ausgeben
                            await dEx.ShowAsync(grMain);
                        }

                        // Bei unbekanntem fehler
                        else
                        {
                            // Fehlermeldung
                            dEx = new DialogEx();
                            dEx.BorderBrush = scbXtrose;
                            dEx.Title = resource.GetString("001_OrdnerNichtErstellt");
                            buttonSet = new DialogExButtonsSet();
                            buttonSet.HorizontalAlignment = HorizontalAlignment.Right;
                            button_1 = new DialogExButton(resource.GetString("001_Schließen"));
                            buttonSet.AddButton(button_1);
                            dEx.AddButtonSet(buttonSet);
                            // Eingabeaufförderung ausgeben
                            await dEx.ShowAsync(grMain);
                        }
                    }
                }

                // Wenn kein Name vorhanden
                else
                {
                    // Fehlermeldung
                    dEx = new DialogEx();
                    dEx.BorderBrush = scbXtrose;
                    dEx.Title = resource.GetString("001_KeinName");
                    buttonSet = new DialogExButtonsSet();
                    buttonSet.HorizontalAlignment = HorizontalAlignment.Right;
                    button_1 = new DialogExButton(resource.GetString("001_Schließen"));
                    buttonSet.AddButton(button_1);
                    dEx.AddButtonSet(buttonSet);

                    // Eingabeaufförderung ausgeben
                    await dEx.ShowAsync(grMain);
                }
            }

            // Wenn Ordner erstellt wurde
            if (isCreated)
            {
                outputRefresh(side);
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // One Drive abmelden
        // -----------------------------------------------------------------------
#region One Drive abmelden

        // Methode // One Drive Ordner erstellen
        private async Task OneDriveSignOut()
        {
            // Wenn Token vorhanden
            if (oneDriveToken != "")
            {
                // Versuchen abzumelden
                try
                {
                    // Abmelden
                    await oneDriveClient.SignOutAsync();

                    // Token löschen
                    oneDriveToken = "";

                    // Fenster aktuallisieren
                    if (listFolderTreeLeft[listFolderTreeLeft.Count() - 1].driveType == "oneDrive")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenLeft != null)
                        {
                            cancelTokenLeft.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenLeft = new CancellationTokenSource();

                        // Fenster links erneuern
                        await outputStart("left", cancelTokenLeft.Token);
                    }

                    // Fenster aktuallisieren
                    if (listFolderTreeRight[listFolderTreeRight.Count() - 1].driveType == "oneDrive")
                    {
                        // Vorherigen Task abbrechen
                        if (cancelTokenRight != null)
                        {
                            cancelTokenRight.Cancel();
                        }
                        // Neues Token erstellen
                        cancelTokenRight = new CancellationTokenSource();

                        // Fenster links erneuern
                        await outputStart("right", cancelTokenLeft.Token);
                    }
                }

                // Wenn verbindung fehlgeschlagen
                catch
                {
                    // Token löschen
                    oneDriveToken = "";

                    // Nachricht ausgeben // Verbindung nicht möglich
                    DialogEx dEx = new DialogEx(resource.GetString("001_VerbindungNicht"));
                    dEx.Content = resource.GetString("001_OneDriveÄndern");
                    DialogExButtonsSet dBSet = new DialogExButtonsSet();
                    DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                    dBSet.AddButton(dBtn);
                    dEx.AddButtonSet(dBSet);
                    await dEx.ShowAsync(grMain);
                }
            }

            // Wenn kein Token vorhanden
            else
            {
                // Nachricht ausgeben // Verbindung nicht möglich
                DialogEx dEx = new DialogEx(resource.GetString("001_VerbindungNicht"));
                dEx.Content = resource.GetString("001_OneDriveÄndern");
                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                dBSet.AddButton(dBtn);
                dEx.AddButtonSet(dBSet);
                await dEx.ShowAsync(grMain);
            }
        }
#endregion
        // -----------------------------------------------------------------------
#endregion










#region Dropbox

        // Dropbox Verzeichnis laden
        // -----------------------------------------------------------------------
#region Dropbox Verzeichnis laden

        // Task // One Drive Verzeichnis laden
        private async Task DropboxLoadFolder(string path, string side, string dropboxToken, CancellationToken token, bool addToFolderTree = true, bool createMenu = true)
        {
            // Pfad erstellen
            if (path == null)
            {
                path = string.Empty;
            }

            // Versuchen Dropbox Daten zu laden
            try
            {
                // Dropbox Dateien laden
                DropboxClient dropboxClient = new DropboxClient(dropboxToken);
                ListFolderResult folderResults = await dropboxClient.Files.ListFolderAsync(path);

                // Liste Items Dateien erstellen
                List<Metadata> itemsFiles = new List<Metadata>();

                // Liste Items Ordner erstellen
                List<Metadata> itemsFolders = new List<Metadata>();

                // Liste  Items erstellen
                List<Metadata> items = new List<Metadata>();

                // Dateien und Ordner trennen
                for (int i = 0; i < folderResults.Entries.Count(); i++)
                {
                    // Wenn Ordner
                    if (folderResults.Entries[i].IsFolder == true)
                    {
                        itemsFolders.Add(folderResults.Entries[i]);
                    }

                    // Wenn Datei
                    else
                    {
                        itemsFiles.Add(folderResults.Entries[i]);
                    }
                }

                // Links
                if (side == "left")
                {
                    // Dateien sortieren // Name aufsteigend
                    if (setSortLeftFiles == "name↑")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderBy((x) => x.Name);
                        itemsFiles = sort.ToList();
                    }

                    // Dateien sortieren // Name absteigend
                    else if (setSortLeftFiles == "name↓")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderByDescending((x) => x.Name);
                        itemsFiles = sort.ToList();
                    }
                    
                    // Dateien sortieren // Datum aufsteigend
                    else if (setSortLeftFiles == "date↑")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderBy((x) => x.AsFile.ClientModified);
                        itemsFiles = sort.ToList();
                    }

                    // Dateien sortieren // Datum absteigend
                    else if (setSortLeftFiles == "date↓")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderByDescending((x) => x.AsFile.ClientModified);
                        itemsFiles = sort.ToList();
                    }

                    // Dateien sortieren // Type aufsteigend
                    else if (setSortLeftFiles == "type↑")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderBy((x) => x.AsFile.GetType());
                        itemsFiles = sort.ToList();
                    }

                    // Dateien sortieren // Type absteigend
                    else if (setSortLeftFiles == "type↓")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderByDescending((x) => x.AsFile.GetType());
                        itemsFiles = sort.ToList();
                    }

                    // Ordner sortieren // Name aufsteigend
                    if (setSortLeftFolders == "name↑")
                    {
                        IEnumerable<Metadata> sort = itemsFolders.OrderBy((x) => x.Name);
                        itemsFolders = sort.ToList();
                    }

                    // Ordner sortieren // Name absteigend
                    else if (setSortLeftFolders != "name↑")
                    {
                        IEnumerable<Metadata> sort = itemsFolders.OrderByDescending((x) => x.Name);
                        itemsFolders = sort.ToList();
                    }

                    // Items zusammenfügen
                    for (int i = 0; i < itemsFolders.Count(); i++)
                    {
                        items.Add(itemsFolders[i]);
                    }
                    for (int i = 0; i < itemsFiles.Count(); i++)
                    {
                        items.Add(itemsFiles[i]);
                    }

                    // Ordnerbaum hinzufügen
                    if (addToFolderTree)
                    {
                        listFolderTreeLeft.Add(new ClassFolderTree(-1, "dropbox", null, dropboxToken, path, "folderDropbox", true, true, true, false, true, true, true, true));
                    }

                    // listFiles Leeren
                    listFilesLeft.Clear();

                    // MultiSelect entfernen
                    lbLeft.SelectionMode = SelectionMode.Single;

                    // Ladeanzeigen ändern
                    loadingHide("left");
                    prLeft.IsActive = true;

                    // Ordner durchlaufen
                    for (int i = 0; i < items.Count(); i++)
                    {

                        // Wenn Task nicht abgebrochen wurde
                        if (!token.IsCancellationRequested)
                        {
                            // Drop // Delete // Cut
                            bool canDrop = true;
                            DateTimeOffset dTO = new DateTimeOffset();

                            // Wenn Datei
                            if (items[i].IsFile)
                            {
                                // Drop // Delete // Cut prüfen
                                try
                                {
                                    if (items[i].AsFile.SharingInfo.ReadOnly)
                                    {
                                        canDrop = false;
                                    }
                                }
                                catch { }

                                // Erstellt Zeit erstellen
                                dTO = Convert.ToDateTime(Convert.ToString(items[i].AsFile.ClientModified));
                            }

                            // Wenn Ordner
                            else
                            {
                                // Drop // Delete // Cut prüfen
                                try
                                {
                                    if (items[i].AsFolder.SharingInfo.ReadOnly)
                                    {
                                        canDrop = false;
                                    }
                                }
                                catch { }
                            }

                            // Wenn ein Ordner
                            if (items[i].IsFolder)
                            {

                                // Wenn Task nicht abgebrochen wurde
                                if (!token.IsCancellationRequested)
                                {
                                    // In listFiles hinzufügen
                                    listFilesLeft.Add(new ClassFiles(i, "left", "dropbox", "folder", items[i].Name, dTO, null, null, null, items[i], await loadIcon("folder"), true, canDrop, true, canDrop, true, canDrop, canDrop, true, false, false));
                                }
                            }

                            // Wenn eine Datei
                            else
                            {
                                // Wenn Task nicht abgebrochen wurde
                                if (!token.IsCancellationRequested)
                                {
                                    // In ListFiles hinzufügen
                                    listFilesLeft.Add(new ClassFiles(i, "left", "dropbox", "file", items[i].Name, dTO, null, null, null, items[i], await loadIconByName(items[i].Name), true, false, true, canDrop, true, canDrop, false, true, false, false));
                                }
                            }
                        }

                        // Wenn Abgebrochen
                        else
                        {
                            break;
                        }
                    }

                    // Menü erstellen
                    if (createMenu)
                    {
                        createAndShowMenu("left");
                    }
                }

                // Rechts
                if (side == "right")
                {
                    // Dateien sortieren // Name aufsteigend
                    if (setSortRightFiles == "name↑")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderBy((x) => x.Name);
                        itemsFiles = sort.ToList();
                    }

                    // Dateien sortieren // Name absteigend
                    else if (setSortRightFiles == "name↓")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderByDescending((x) => x.Name);
                        itemsFiles = sort.ToList();
                    }

                    // Dateien sortieren // Datum aufsteigend
                    else if (setSortRightFiles == "date↑")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderBy((x) => x.AsFile.ClientModified);
                        itemsFiles = sort.ToList();
                    }

                    // Dateien sortieren // Datum absteigend
                    else if (setSortRightFiles == "date↓")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderByDescending((x) => x.AsFile.ClientModified);
                        itemsFiles = sort.ToList();
                    }

                    // Dateien sortieren // Type aufsteigend
                    else if (setSortRightFiles == "type↑")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderBy((x) => x.AsFile.GetType());
                        itemsFiles = sort.ToList();
                    }

                    // Dateien sortieren // Type absteigend
                    else if (setSortRightFiles == "type↓")
                    {
                        IEnumerable<Metadata> sort = itemsFiles.OrderByDescending((x) => x.AsFile.GetType());
                        itemsFiles = sort.ToList();
                    }

                    // Ordner sortieren // Name aufsteigend
                    if (setSortRightFolders == "name↑")
                    {
                        IEnumerable<Metadata> sort = itemsFolders.OrderBy((x) => x.Name);
                        itemsFolders = sort.ToList();
                    }

                    // Ordner sortieren // Name absteigend
                    else if (setSortRightFolders != "name↑")
                    {
                        IEnumerable<Metadata> sort = itemsFolders.OrderByDescending((x) => x.Name);
                        itemsFolders = sort.ToList();
                    }

                    // Items zusammenfügen
                    for (int i = 0; i < itemsFolders.Count(); i++)
                    {
                        items.Add(itemsFolders[i]);
                    }
                    for (int i = 0; i < itemsFiles.Count(); i++)
                    {
                        items.Add(itemsFiles[i]);
                    }

                    // Ordnerbaum hinzufügen
                    if (addToFolderTree)
                    {
                        listFolderTreeRight.Add(new ClassFolderTree(-1, "dropbox", null, dropboxToken, path, "folderDropbox", true, true, true, false, true, true, true, true));
                    }

                    // listFiles Leeren
                    listFilesRight.Clear();

                    // MultiSelect entfernen
                    lbRight.SelectionMode = SelectionMode.Single;

                    // Ladeanzeigen ändern
                    loadingHide("right");
                    prRight.IsActive = true;

                    // Ordner durchlaufen
                    for (int i = 0; i < items.Count(); i++)
                    {

                        // Wenn Task nicht abgebrochen wurde
                        if (!token.IsCancellationRequested)
                        {
                            // Drop // Delete // Cut
                            bool canDrop = true;
                            DateTimeOffset dTO = new DateTimeOffset();

                            // Wenn Datei
                            if (items[i].IsFile)
                            {
                                // Drop // Delete // Cut prüfen
                                try
                                {
                                    if (items[i].AsFile.SharingInfo.ReadOnly)
                                    {
                                        canDrop = false;
                                    }
                                }
                                catch { }

                                // Erstellt Zeit erstellen
                                dTO = Convert.ToDateTime(Convert.ToString(items[i].AsFile.ClientModified));
                            }

                            // Wenn Ordner
                            else
                            {
                                // Drop // Delete // Cut prüfen
                                try
                                {
                                    if (items[i].AsFolder.SharingInfo.ReadOnly)
                                    {
                                        canDrop = false;
                                    }
                                }
                                catch { }
                            }

                            // Wenn ein Ordner
                            if (items[i].IsFolder)
                            {

                                // Wenn Task nicht abgebrochen wurde
                                if (!token.IsCancellationRequested)
                                {
                                    // In listFiles hinzufügen
                                    listFilesRight.Add(new ClassFiles(i, "right", "dropbox", "folder", items[i].Name, dTO, null, null, null, items[i], await loadIcon("folder"), true, canDrop, true, canDrop, true, canDrop, canDrop, true, false, false));
                                }
                            }

                            // Wenn eine Datei
                            else
                            {
                                // Wenn Task nicht abgebrochen wurde
                                if (!token.IsCancellationRequested)
                                {
                                    // In ListFiles hinzufügen
                                    listFilesRight.Add(new ClassFiles(i, "right", "dropbox", "file", items[i].Name, dTO, null, null, null, items[i], await loadIconByName(items[i].Name), true, false, true, canDrop, true, canDrop, false, true, false, false));
                                }
                            }
                        }

                        // Wenn Abgebrochen
                        else
                        {
                            break;
                        }
                    }

                    // Menü erstellen
                    if (createMenu)
                    {
                        createAndShowMenu("right");
                    }
                }
            }

            // Wenn Verbindung nicht möglich
            catch (Exception e)
            {
                // Nachricht ausgeben // Verbindung nicht möglich
                DialogEx dEx = new DialogEx(resource.GetString("001_VerbindungNicht"));
                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                dBSet.AddButton(dBtn);
                dEx.AddButtonSet(dBSet);
                await dEx.ShowAsync(grMain);
            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Dropbox Datei öffnen durch Download
        // -----------------------------------------------------------------------
#region Dropbox Datei öffnen durch Download

        // Methode // One Drive Datei öffnen durch Download
        private async Task DropboxOpenFileByDownload(Metadata metadata, string token, DialogEx dWaitAbort)
        {
            // Pfad erstellen
            string path = metadata.PathDisplay;
            string name = metadata.Name;

            // Ob Abbruch durch Benutzer
            bool abort = false;

            // Datei herunterladen
            DropboxClient dropboxClient = new DropboxClient(token);

            // Datei erstellen
            StorageFile file = await folderTemp.CreateFileAsync(name, CreationCollisionOption.GenerateUniqueName);
            var streamFile = await file.OpenAsync(FileAccessMode.ReadWrite);
            var response = await dropboxClient.Files.DownloadAsync(path);

            // Puffer erstellen
            ulong fileSize = response.Response.Size;
            byte[] buffer = new byte[setBufferSize];
            int totalBytes = 0;
            int lenght;

            // Download erstellen
            using (var stream = await response.GetContentAsStreamAsync())
            {
                // Schleife Download
                lenght = stream.Read(buffer, 0, setBufferSize);
                while (lenght > 0)
                {
                    // Wenn Abbruch durch Benutzer
                    if (dWaitAbort.GetAnswer() == resource.GetString("001_Abbrechen"))
                    {
                        abort = true;
                        break;
                    }

                    // Datei als Stream herunterladen
                    await streamFile.WriteAsync(buffer.AsBuffer(0, lenght));
                    lenght = stream.Read(buffer, 0, setBufferSize);
                    totalBytes += lenght;
                    dWaitAbort.SetProgressBarValueByIndex(0, totalBytes);
                }
            }
            
            // Datei öffnen
            if (!abort)
            {
                // Datei öffnen
                StorageFile fileToOpen = await folderTemp.GetFileAsync(file.Name);
                var options = new LauncherOptions();
                options.DisplayApplicationPicker = true;
                bool success = await Launcher.LaunchFileAsync(fileToOpen, options);

            }
        }
#endregion
        // -----------------------------------------------------------------------





        // Dropbox Ordner erstellen
        // -----------------------------------------------------------------------
#region Dropbox Ordner erstellen

        // Methode // One Drive Ordner erstellen
        private async Task DropboxCreateFolder(string path, string token, string side)
        {
            // Gibt an ob Ordner erstellt wurde
            bool isCreated = false;

            // Eingabeaufförderung erstellen
            DialogEx dEx = new DialogEx();
            dEx.BorderBrush = scbXtrose;
            dEx.Title = resource.GetString("001_NeuerOrdner");
            DialogExTextBox textBox_1 = new DialogExTextBox();
            textBox_1.Title = resource.GetString("001_Name");
            dEx.AddTextBox(textBox_1);
            DialogExButtonsSet buttonSet = new DialogExButtonsSet();
            DialogExButton button_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
            buttonSet.AddButton(button_1);
            DialogExButton button_2 = new DialogExButton(resource.GetString("001_Hinzufügen"));
            buttonSet.AddButton(button_2);
            dEx.AddButtonSet(buttonSet);

            // Eingabeaufförderung ausgeben
            await dEx.ShowAsync(grMain);

            // Antwort auslesen
            string answer = dEx.GetAnswer();

            // Wenn Ordner erstellt wird
            if (answer == resource.GetString("001_Hinzufügen"))
            {
                // Wenn Name vorhanden
                if (dEx.GetTextBoxTextByTitle(resource.GetString("001_Name")) != "")
                {
                    // Versuchen Ordner zu erstellen
                    try
                    {
                        // Temponären Ordner erstellen und loschen
                        StorageFolder folderTest = await folderTemp.CreateFolderAsync(dEx.GetTextBoxTextByTitle(resource.GetString("001_Name")), CreationCollisionOption.ReplaceExisting);
                        await folderTest.DeleteAsync();

                        // Pfad erstellen
                        if(path == null)
                        {
                            path = "";
                        }
                        path = path += "/" + dEx.GetTextBoxTextByTitle(resource.GetString("001_Name"));

                        // Ordner auf Dropbox erstellen
                        DropboxClient dropboxClient = new DropboxClient(token);
                        await dropboxClient.Files.CreateFolderAsync(path);

                        // Angeben das Ordner erstelt wurde
                        isCreated = true;
                    }
                    catch
                    {
                        // Fehlermeldung
                        dEx = new DialogEx();
                        dEx.BorderBrush = scbXtrose;
                        dEx.Title = resource.GetString("001_OrdnerNichtErstellt");
                        buttonSet = new DialogExButtonsSet();
                        buttonSet.HorizontalAlignment = HorizontalAlignment.Right;
                        button_1 = new DialogExButton(resource.GetString("001_Schließen"));
                        buttonSet.AddButton(button_1);
                        dEx.AddButtonSet(buttonSet);
                        // Eingabeaufförderung ausgeben
                        await dEx.ShowAsync(grMain);
                    }
                }

                // Wenn kein Name vorhanden
                else
                {
                    // Fehlermeldung
                    dEx = new DialogEx();
                    dEx.BorderBrush = scbXtrose;
                    dEx.Title = resource.GetString("001_KeinName");
                    buttonSet = new DialogExButtonsSet();
                    buttonSet.HorizontalAlignment = HorizontalAlignment.Right;
                    button_1 = new DialogExButton(resource.GetString("001_Schließen"));
                    buttonSet.AddButton(button_1);
                    dEx.AddButtonSet(buttonSet);

                    // Eingabeaufförderung ausgeben
                    await dEx.ShowAsync(grMain);
                }
            }

            // Wenn Ordner erstellt wurde
            if (isCreated)
            {
                outputRefresh(side);
            }
        }
#endregion
        // -----------------------------------------------------------------------
#endregion










#region Methoden und Tasks
        // Privaten Bereich öffnen
        // -----------------------------------------------------------------------
        private async Task<bool> openPrivate ()
        {
            // Rückgabe
            bool open = false;

            // Wenn Vollversion
            if (setFullVersion)
            {
                // Prüfen ob Passwort eingegeben werden muss
                if (!setPrivateOpen)
                {
                    // Prüfen ob Passwort vorhanden
                    try
                    {
                        // Versuchen Passwort zu laden
                        StorageFile passFile = await folderSettings.GetFileAsync("Private.txt");
                        var passTemp = await FileIO.ReadTextAsync(passFile);
                        string pass1 = passTemp.ToString();

                        // Passwort abfragen
                        DialogEx dEx = new DialogEx(resource.GetString("001_Passwort"));
                        DialogExPasswordBox pb_1 = new DialogExPasswordBox(resource.GetString("001_Passwort"));
                        dEx.AddPasswordBox(pb_1);
                        DialogExButtonsSet dBSet = new DialogExButtonsSet();
                        DialogExButton dBtn_1 = new DialogExButton(resource.GetString("001_Weiter"));
                        dBSet.AddButton(dBtn_1);
                        DialogExButton dBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        dBSet.AddButton(dBtn_2);
                        dEx.AddButtonSet(dBSet);
                        await dEx.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dEx.GetAnswer();

                        // Wenn Antwort Weiter
                        if (answer == resource.GetString("001_Weiter"))
                        {
                            // Passwort auslesen
                            string pass0 = dEx.GetPasswordBoxTextByIndex(0);

                            // Wenn Passwört richtig
                            if (pass0 == pass1)
                            {
                                open = true;
                                setPrivateOpen = true;
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
                    }

                    // Wenn kein Passwort vorhanden
                    catch
                    {
                        // Warnung ausgeben
                        DialogEx dEx = new DialogEx(resource.GetString("001_Warnung"), resource.GetString("001_WarnungPrivate"));
                        dEx.ContentFontSize = 12;
                        DialogExButtonsSet dBSet = new DialogExButtonsSet();
                        DialogExButton dBtn_1 = new DialogExButton(resource.GetString("001_Weiter"));
                        dBSet.AddButton(dBtn_1);
                        DialogExButton dBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                        dBSet.AddButton(dBtn_2);
                        dEx.AddButtonSet(dBSet);
                        await dEx.ShowAsync(grMain);

                        // Antwort auslesen
                        string answer = dEx.GetAnswer();

                        // Wenn Antwort Weiter
                        if (answer == resource.GetString("001_Weiter"))
                        {
                            // Warnung ausgeben
                            dEx = new DialogEx(resource.GetString("001_PasswortErstellen"));
                            DialogExPasswordBox pb_1 = new DialogExPasswordBox(resource.GetString("001_Passwort"));
                            dEx.AddPasswordBox(pb_1);
                            DialogExPasswordBox pb_2 = new DialogExPasswordBox(resource.GetString("001_PasswortWiederholen"));
                            dEx.AddPasswordBox(pb_2);
                            dBSet = new DialogExButtonsSet();
                            dBtn_1 = new DialogExButton(resource.GetString("001_Weiter"));
                            dBSet.AddButton(dBtn_1);
                            dBtn_2 = new DialogExButton(resource.GetString("001_Abbrechen"));
                            dBSet.AddButton(dBtn_2);
                            dEx.AddButtonSet(dBSet);
                            await dEx.ShowAsync(grMain);

                            // Antwort auslesen
                            answer = dEx.GetAnswer();

                            // Wenn Antwort Weiter
                            if (answer == resource.GetString("001_Weiter"))
                            {
                                // Passworter laden
                                string pass0 = dEx.GetPasswordBoxTextByIndex(0);
                                string pass1 = dEx.GetPasswordBoxTextByIndex(1);

                                // Wenn Passworter nicht vorhanden
                                if (pass0.Length < 1 | pass1.Length < 1)
                                {
                                    //Fehlermeldung ausgeben
                                    dEx = new DialogEx(resource.GetString("001_Fehler"), resource.GetString("001_KeinPasswort"));
                                    dBSet = new DialogExButtonsSet();
                                    dBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                                    dBSet.AddButton(dBtn_1);
                                    dEx.AddButtonSet(dBSet);
                                    await dEx.ShowAsync(grMain);
                                }

                                // Wenn Passwort vorhanden
                                else
                                {
                                    // Wenn Passwörter unterschiedlich
                                    if (pass0 != pass1)
                                    {
                                        //Fehlermeldung ausgeben
                                        dEx = new DialogEx(resource.GetString("001_Fehler"), resource.GetString("001_PasswortUnterschied"));
                                        dBSet = new DialogExButtonsSet();
                                        dBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                                        dBSet.AddButton(dBtn_1);
                                        dEx.AddButtonSet(dBSet);
                                        await dEx.ShowAsync(grMain);
                                    }

                                    // Wenn Passwörter gleich
                                    else
                                    {
                                        // Versuchen ein Verzeichnis aus Passwort zu erstellen
                                        try
                                        {
                                            // Verzaichnis erstellen und löschen
                                            StorageFolder storageFolder = await folderTemp.CreateFolderAsync(pass0, CreationCollisionOption.FailIfExists);
                                            await storageFolder.DeleteAsync();

                                            // Passwort speichern
                                            StorageFile setPassword = await folderSettings.CreateFileAsync("Private.txt", CreationCollisionOption.OpenIfExists);
                                            await FileIO.WriteTextAsync(setPassword, pass0);

                                            // Angeben das Offen
                                            open = true;
                                        }
                                        // Wenn Passwort nicht verwendet werden kann
                                        catch
                                        {
                                            //Fehlermeldung ausgeben
                                            dEx = new DialogEx(resource.GetString("001_Fehler"), resource.GetString("001_PasswortNicht"));
                                            dBSet = new DialogExButtonsSet();
                                            dBtn_1 = new DialogExButton(resource.GetString("001_Schließen"));
                                            dBSet.AddButton(dBtn_1);
                                            dEx.AddButtonSet(dBSet);
                                            await dEx.ShowAsync(grMain);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Wenn keine Vollversion
            else
            {
                // Nachricht ausgeben // Nur in der Vollversion verfügbar
                DialogEx dEx = new DialogEx(resource.GetString("001_NurVollversion"));
                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Schließen"));
                dBSet.AddButton(dBtn);
                dEx.AddButtonSet(dBSet);
                await dEx.ShowAsync(grMain);
            }

            // Rückgabe
            return open;
        }
        // -----------------------------------------------------------------------





        // Ladeanzeigen erstellen
        // -----------------------------------------------------------------------
        // Parameter
        private string loadingButtonLeft;
        private string loadingButtonRight;



        // Methode // Ladeanzeige Ausgeben
        private void loadingShow(string content, string side, string button = null)
        {
            // Links
            if (side == "left")
            {
                // Button zurücksetzen
                loadingButtonLeft = "";

                // Ladeanzeige erstellen
                tbLoadingLeft.Text = content;
                if (button != null)
                {
                    btnLoadingLeft.Content = button;
                    btnLoadingLeft.Visibility = Visibility.Visible;
                }
                else
                {
                    btnLoadingLeft.Visibility = Visibility.Collapsed;
                }
                prLoadingLeft.IsActive = true;

                // Button Akiton erstellen
                btnLoadingLeft.Click += delegate
                {
                    loadingButtonLeft = btnLoadingLeft.Content.ToString();
                };

                // Ladeanzeige ausgeben
                grLoadingLeft_top.Visibility = Visibility.Visible;
                grLoadingLeft_bottom.Visibility = Visibility.Visible;
            }

            // Rechts
            else if (side == "right")
            {
                // Button zurücksetzen
                loadingButtonRight = "";

                // Ladeanzeige erstellen
                tbLoadingRight.Text = content;
                if (button != null)
                {
                    btnLoadingRight.Content = button;
                    btnLoadingRight.Visibility = Visibility.Visible;
                }
                else
                {
                    btnLoadingRight.Visibility = Visibility.Collapsed;
                }
                prLoadingRight.IsActive = true;

                // Button Akiton erstellen
                btnLoadingRight.Click += delegate
                {
                    loadingButtonRight = btnLoadingRight.Content.ToString();
                };

                // Ladeanzeige ausgeben
                grLoadingRight_top.Visibility = Visibility.Visible;
                grLoadingRight_bottom.Visibility = Visibility.Visible;
            }
        }



        // Ladeanzeige verbergen
        private void loadingHide(string side)
        {
            // Links
            if (side == "left")
            {
                prLoadingLeft.IsActive = false;
                grLoadingLeft_top.Visibility = Visibility.Collapsed;
                grLoadingLeft_bottom.Visibility = Visibility.Collapsed;
            }

            // Rechts
            else if (side == "right")
            {
                prLoadingRight.IsActive = false;
                grLoadingRight_top.Visibility = Visibility.Collapsed;
                grLoadingRight_bottom.Visibility = Visibility.Collapsed;
            }
        }
        // -----------------------------------------------------------------------





        // Funktion // Icons laden
        // -----------------------------------------------------------------------
        async Task<BitmapImage> loadIcon(string iconName)
        {
            // Wenn kein Icon geladen wird
            if (iconName == null)
            {
                // Ordner Icon laden
                iconName = "folder";
            }

            // Wenn Telefon Icon geladen wird 
            else if (iconName == "folderPhone")
            {
                // Wenn Desktop
                if (!IsMobile)
                {
                    iconName = "folderDesktop";
                }
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





        // Task // Icons nach Name laden
        // -----------------------------------------------------------------------
        private async Task<BitmapImage> loadIconByName(string name)
        {
            // String des Icons
            string icon = "";

            // String der Endungen
            string[] filePhoto = { "jpg", "JPG", "jpeg", "JPEG", "bmp", "BMP", "gif", "GIF", "tif", "TIF", "tiff", "TIFF", "ico", "ICO", "wmf", "WMF", "pcx", "PCX", "png", "PNG", "pic", "PIC" };
            string[] fileAudio = { "wav", "WAV", "ram", "RAM", "ra", "RA", "aif", "AIF", "aiff", "AIFF", "aifc", "AIFC", "mid", "MID", "midi", "MIDI", "au", "AU", "mp3", "MP3", "ogg", "OGG", "mod", "MOD", "rmi", "RMI", "snd", "SND", "wma", "WMA" };
            string[] fileVideo = { "avi", "AVI", "mpg", "MPG", "mpeg", "MPEG", "qt", "QT", "flv", "FLV", "wmv", "WMV", "mov", "MOV", "vob", "VOB", "asf", "ASF", "divx", "DIVX", "xvid", "XVID", "mp4", "MP4", "3gp", "3GP", "mkv", "MKV", "mka", "MKA", "mks", "MKS" };

            // Name aufteilen und Endung suchen
            string[] spName = name.Split(new char[] { '.' });

            // Endung auswählen
            for (int i = 0; i < filePhoto.Count(); i++)
            {
                if (spName[spName.Count() - 1] == filePhoto[i])
                {
                    icon = "filePhoto";
                    break;
                }
            }
            if (icon == "")
            {
                for (int i = 0; i < fileAudio.Count(); i++)
                {
                    if (spName[spName.Count() - 1] == fileAudio[i])
                    {
                        icon = "fileAudio";
                        break;
                    }
                }
            }
            if (icon == "")
            {
                for (int i = 0; i < fileVideo.Count(); i++)
                {
                    if (spName[spName.Count() - 1] == fileVideo[i])
                    {
                        icon = "fileVideo";
                        break;
                    }
                }
            }
            if (icon == "")
            {
                icon = "file";
            }

            // Rückgabe
            return await loadIcon(icon);
        }
        // -----------------------------------------------------------------------





        // Ordner und Dateien // Einzel oder Mehrzahl ausgeben
        // -----------------------------------------------------------------------
        public string filesFoldersOutput(int files, int folders, string separater = "\r\n")
        {
            // Return erstellen
            string stringReturn = "";

            // Ordner
            if (folders == 1)
            {
                stringReturn += folders.ToString() + " " + resource.GetString("001_Ordner");
            }
            else if (folders > 1)
            {
                stringReturn += folders.ToString() + " " + resource.GetString("001_OrdnerMehrzahl");
            }

            // Wenn Ordner und Dateien
            if (folders > 0 & files > 0)
            {
                stringReturn += separater;
            }

            // Dateien
            if (files == 1)
            {
                stringReturn += files.ToString() + " " + resource.GetString("001_Datei");
            }
            else if (files > 1)
            {
                stringReturn += files.ToString() + " " + resource.GetString("001_DateiMehrzahl");
            }

            // Return zurückgeben
            return stringReturn;
        }
        // -----------------------------------------------------------------------





        // Task // Datein in Ordner und Unterordner zählen // Lokal
        // -----------------------------------------------------------------------
        private async Task<List<int>> acGetFilesAndFoldersLokal(StorageFolder storageFolder, List<int> listFilesFolders)
        {
            // Versuchen Dateien auszulesen
            try
            {
                // Dateien auslesen
                IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
                IReadOnlyList<StorageFile> fileList = new List<StorageFile>();
                folderList = await storageFolder.GetFoldersAsync();
                fileList = await storageFolder.GetFilesAsync();

                // Dateien ergänzen
                listFilesFolders[0] += fileList.Count();

                // Ordner ergänzen
                for (int i = 0; i < folderList.Count(); i++)
                {
                    // Ordner ergänzen
                    listFilesFolders[1]++;
                    // Weitere Ordner und Dateien auslesen
                    listFilesFolders = await acGetFilesAndFoldersLokal(folderList[i], listFilesFolders);
                }
            }
            catch
            {
            }

            // Daten ausgeben
            return listFilesFolders;
        }
        // -----------------------------------------------------------------------




        // Task // Datein in Ordner und Unterordner zählen // One Drive
        // -----------------------------------------------------------------------
        private async Task<List<int>> acGetFilesAndFoldersOneDrive(Item item, List<int> listFilesFolders)
        {
            // Gibt an ob was geladen wurde
            bool connect = true;

            // Pfad erstellen
            string path = item.ParentReference.Path;

            // Wenn Pfad vorhanden
            if (path != null)
            {
                path = Regex.Replace(path, ":", "/");
                path += "/" + item.Name;
                path = Regex.Replace(path, "/drive/root/", "");
            }

            // IChildrenCollectionPage erstellen
            IChildrenCollectionPage items = null;

            // Versuchen Daten zu laden
            try
            {
                // Ordner und Dateien laden // Root
                if (path == null)
                {
                    items = await oneDriveClient.Drive.Root.Children.Request().GetAsync();
                }

                // Ordner und Dateien laden // Nach Pfad
                else
                {
                    items = await oneDriveClient.Drive.Root.ItemWithPath(path).Children.Request().GetAsync();
                }
            }
            catch
            {
                // Gib an das nichts geladen wurde
                connect = false;
            }

            // Dateien durchlaufen
            for (int i = 0; i < items.Count(); i++)
            {
                // Wenn Ordner
                if (items[i].Folder != null)
                {
                    // Ordner hinzufügen
                    listFilesFolders[1]++;

                    // Datein in Ordner und Unterordner zählen // Task
                    listFilesFolders = await acGetFilesAndFoldersOneDrive(items[i], listFilesFolders);
                }
                // Wenn Datei
                else
                {
                    listFilesFolders[0]++;
                }
            }

            // Daten ausgeben
            return listFilesFolders;
        }
        // -----------------------------------------------------------------------





        // Task // Datein in Ordner und Unterordner zählen // Dropbox
        // -----------------------------------------------------------------------
        private async Task<List<int>> acGetFilesAndFoldersDropbox(Metadata metadata, string token, List<int> listFilesFolders)
        {
            // Dropbox Dateien laden
            DropboxClient dropboxClient = new DropboxClient(token);
            ListFolderResult folderResults = await dropboxClient.Files.ListFolderAsync(metadata.PathDisplay);

            // Dateien und Ordner trennen
            for (int i = 0; i < folderResults.Entries.Count(); i++)
            {
                // Wenn Ordner
                if (folderResults.Entries[i].IsFolder == true)
                {
                    listFilesFolders[1]++;
                    listFilesFolders = await acGetFilesAndFoldersDropbox(folderResults.Entries[i], token, listFilesFolders);
                }

                // Wenn Datei
                else
                {
                    listFilesFolders[0]++;
                }
            }

            // Daten ausgeben
            return listFilesFolders;
        }
        // -----------------------------------------------------------------------





        // Temporäre Dateien löschen
        // -----------------------------------------------------------------------
        private async Task deleteTempFiles()
        {
            // Parameter // Liste Ordner // Liste Dateien
            IReadOnlyList<StorageFolder> folderList = new List<StorageFolder>();
            IReadOnlyList<StorageFile> fileList = new List<StorageFile>();

            // Dateien auslesen
            folderList = await folderTemp.GetFoldersAsync();
            fileList = await folderTemp.GetFilesAsync();

            // Wenn Dateien vorhenden
            if (folderList.Count() > 0 | fileList.Count() - 1 > 0)
            {
                // Lösch Klasse erstellen
                ClassDeleteFiles classDeleteFiles = new ClassDeleteFiles();
                CancellationTokenSource tokenTemp = new CancellationTokenSource();
                CancellationToken token = tokenTemp.Token;

                // Lade Anzeige augeben
                DialogEx dWaitAbort = new DialogEx();
                dWaitAbort.Title = resource.GetString("001_EinenMoment") + "...";
                DialogExProgressRing progressRing = new DialogExProgressRing();
                dWaitAbort.AddProgressRing(progressRing);
                dWaitAbort.ShowAsync(grMain);

                // Liste Dateien und Ordner
                List<int> listFilesFolders = new List<int>();
                listFilesFolders.Add(fileList.Count());
                listFilesFolders.Add(0);

                // Dateien und Ordner zählen
                for (int i = 0; i < folderList.Count(); i++)
                {
                    // Ordner hinzufügen
                    listFilesFolders[1]++;

                    // Datein in Ordner und Unterordner zählen // Task
                    listFilesFolders = await acGetFilesAndFoldersLokal(folderList[i], listFilesFolders);
                }

                // Nachricht erstellen
                string content = filesFoldersOutput(listFilesFolders[0], listFilesFolders[1], " | ");
                // Lade Anzeige entfernen
                dWaitAbort.Hide();

                // Löschen Ausgebe erstellen
                dWaitAbort = new DialogEx();
                dWaitAbort.Title = resource.GetString("001_TemporäreDateienLöschenTask");
                dWaitAbort.Content = content;
                progressRing = new DialogExProgressRing();
                dWaitAbort.AddProgressRing(progressRing);
                DialogExTextBlock tBlock = new DialogExTextBlock();
                tBlock.Foreground = new SolidColorBrush(Colors.Gray);
                tBlock.FontSize = 10;
                tBlock.Margin = new Thickness(0, 5, 0, -8);
                dWaitAbort.AddTextBlock(tBlock);
                DialogExProgressBar proBar = new DialogExProgressBar();
                proBar.Maximun = listFilesFolders[0] + listFilesFolders[1];
                dWaitAbort.AddProgressBar(proBar);
                DialogExButtonsSet dWaitAbortBtnSet = new DialogExButtonsSet();
                DialogExButton dWaitAbortBtn_1 = new DialogExButton(resource.GetString("001_Abbrechen"));
                dWaitAbortBtnSet.AddButton(dWaitAbortBtn_1);
                dWaitAbort.AddButtonSet(dWaitAbortBtnSet);
                dWaitAbort.ShowAsync(grMain);

                // Dateien durchlaufen
                for (int i = 0; i < fileList.Count(); i++)
                {
                    try
                    {
                        await fileList[i].DeleteAsync();
                    }
                    catch { }

                    classDeleteFiles.value++;
                    dWaitAbort.SetProgressBarValueByIndex(0, classDeleteFiles.value);
                }

                // Ordner durchlaufen
                for (int i = 0; i < folderList.Count(); i++)
                {
                    classDeleteFiles = await mfDeleteFolderAndFiles(folderList[i], classDeleteFiles, dWaitAbort, token, true);
                }
                    
                // Löschen Ausgabe entfernen
                dWaitAbort.Hide();
            }
        }
        // -----------------------------------------------------------------------





        // FilePicker
        // -----------------------------------------------------------------------
        private async void FilePicker()
        {
            // File Picker
            string[] filterTypes = { ".246", ".3g2", ".3gp", ".7z", ".7zip", ".aac", ".abm", ".accdb", ".ace", ".ai", ".amr", ".ajr", ".asp", ".aspx", ".asx", ".avi", ".bak", ".bik", ".bin", ".cab", ".ccd", ".cda", ".cdr", ".cfg", ".cnf", ".cr2", ".crw", ".css", ".daa", ".dao", ".dcr", ".djvu", ".dmg", ".doc", ".docm", ".docx", ".dot", ".dotm", ".dotx", ".drw", ".dwg", ".dxf", ".eml", ".eps", ".fax", ".fla", ".flac", ".flv", ".fpx", ".gif", ".gz", ".htm", ".html", ".ico", ".icon", ".iff", ".ifo", ".img", ".ind", ".indd", ".ini", ".iso", ".jar", ".jpg", ".jpeg", ".js", ".json", ".jsp", ".log", ".m3u", ".m4a", ".m4v", ".mdb", ".mdf", ".mht", ".midi", ".mkv", ".mobi", ".mov", ".mp3", ".mp4", ".mpc", ".mpg", ".mpeg", ".mrw", ".nfo", ".nrg", ".obj", ".odg", ".ods", ".odt", ".ogg", ".one", ".onepkg", ".pcd", ".pcm", ".pcx", ".pdf", ".php", ".php3", ".ply", ".png", ".potx", ".pps", ".ppsm", ".ppsx", ".ptt", ".pptm", ".pptx", ".ps", ".psb", ".pub", ".raw", ".rc", ".rss", ".rtf", ".set", ".shp", ".snd", ".srt", ".stf", ".sub", ".svg", ".swf", ".sxc", ".sxw", ".tao", ".tar", ".rar", ".tgz", ".thm", ".thmx", ".tif", ".tmp", ".torrent", ".txt", ".vcd", ".vob", ".vol", ".wav", ".wdb", ".wma", ".wmv", ".wpd", ".wpl", ".wps", ".wri", ".xls", ".xlsm", ".xlsx", ".xml", ".zip", ".zix", ".wmf", ".bmp", ".rm" };
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            // filePicker.SuggestedStartLocation = PickerLocationId.Unspecified;
            for (int i = 0; i < filterTypes.Count(); i++)
            {
                filePicker.FileTypeFilter.Add(filterTypes[i]);
            }
            IReadOnlyList<StorageFile> files = await filePicker.PickMultipleFilesAsync();

            // Wenn Dateien vorhanden
            if (files != null)
            {
                // Parameter zurücksetzen
                paste = true;
                this.cut = cut;
                cutSourceId = null;
                cutPath = null;
                cutToken = null;
                listPasteFoldersLocal.Clear();
                listPasteFilesLocal.Clear();
                listPasteItemsOneDrive.Clear();
                listPasteItemsDropbox.Clear();

                // Dateien in Kopie einsetzen
                for (int i = 0; i < files.Count(); i++)
                {
                    listPasteFilesLocal.Add(files[i]);
                }

                // Nachricht ausgeben
                DialogEx dialogEx = new DialogEx(resource.GetString("002_Dateiauswahl"), resource.GetString("002_DateiauswahlKopiert"));
                DialogExButtonsSet dxBtnSet = new DialogExButtonsSet();
                DialogExButton btn1 = new DialogExButton(resource.GetString("001_Weiter"));
                dxBtnSet.AddButton(btn1);
                dialogEx.AddButtonSet(dxBtnSet);
                await dialogEx.ShowAsync(grMain);
            }
        }
        // -----------------------------------------------------------------------
#endregion










#region Größen verändern
        // Methode // Wenn Größe von grMain geändert wird
        // -----------------------------------------------------------------------
        private void GrMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Nur wenn Mobile
            if (IsMobile)
            {
                // Prüft ob eine Maus angeschlossen wurde
                switch (UIViewSettings.GetForCurrentView().UserInteractionMode)
                {
                    // Wenn Maus aktiv ist
                    case UserInteractionMode.Mouse:
                        // Wenn Maus noch nicht verwendet wird
                        if (!continuum)
                        {
                            // Angeben das Maus verwendet wird
                            continuum = true;

                            // ScrollBar entfernen
                            grScrollLeft.Visibility = Visibility.Collapsed;
                            grScrollRight.Visibility = Visibility.Collapsed;

                            // Actionbar zurücksetzen um Größe neu zu errechnen
                            actionBarHeight = 0;
                        }
                        break;

                    // Wenn Touch aktiv ist
                    case UserInteractionMode.Touch:
                    default:
                        // Wenn Touch noch nicht verwendet wird
                        if (continuum)
                        {
                            continuum = false;
                            grScrollLeft.Visibility = Visibility.Visible;
                            grScrollRight.Visibility = Visibility.Visible;

                            // Actionbar zurücksetzen um Größe neu zu errechnen
                            actionBarHeight = 0;
                        }
                        break;
                }
            }

            // Neue Größe ermittleln
            frameHeight = Window.Current.CoreWindow.Bounds.Height;
            frameWidth = Window.Current.CoreWindow.Bounds.Width;
            frameLeftWidth = (frameWidth - rectLine.Width) / Convert.ToDouble(100) * percent;
            frameRightWidth = frameWidth - rectLine.Width - frameLeftWidth;

            // Größe Ändern
            ChangeWindowsSize();
        }
        // -----------------------------------------------------------------------





        // Größe des Fensters ändern // Durch ziehen der Trennlienie
        // -----------------------------------------------------------------------
        // Parameter und Argumente
        double scManiX = 0;
        bool scTick = true;


        // Methode // Beim starten der Bewegung
        private void rectLine_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            // Aktuelle Position festlegen
            scManiX = e.Position.X;

            // Timer starten
            scTimer.Start();
        }



        // Methode // Beim beenden der Bewegung
        private void rectLine_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            // Prozent errechnen
            percent = Convert.ToDouble(100) / frameWidth * (frameLeftWidth + (rectLine.Width / 2));

            // Timer stoppen und zurücksetzen
            scTimer.Stop();
            scTick = true;
        }



        // Methde // Während der Bewegung
        private void rectLine_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            // Wenn Position verändert wird
            if (scTick)
            {
                // Position verändern
                rectTransform.TranslateX += (e.Position.X - scManiX);


                // Neue Größen errechnen
                frameLeftWidth += e.Position.X;
                if (frameLeftWidth < 11)
                {
                    frameLeftWidth = 0;
                }
                if (frameLeftWidth > (frameWidth - rectLine.Width))
                {
                    frameLeftWidth = (frameWidth - rectLine.Width);
                }
                frameRightWidth = frameWidth - frameLeftWidth - rectLine.Width;

                // Manipulation zurücksetzen
                rectTransform.TranslateX = 0;

                // Größe ändern
                ChangeWindowsSize();

                // Timer auf warten stellen
                scTick = false;
            }
        }



        // Timer // Drosselt das Bewegen des Fensters
        private void ScTimer_Tick(object sender, object e)
        {
            // Timer auf ausführen stellen
            scTick = true;
        }
        // -----------------------------------------------------------------------





        // Fenster Größen ändern
        // -----------------------------------------------------------------------
        // Fenstergröße ändern
        public void ChangeWindowsSize()
        {
            // Wenn Mobil
            if (IsMobile)
            {
                // Wenn Handy
                if (!continuum)
                {
                    // Anpassungen die einmalig errechnet werden
                    if (actionBarHeight == 0)
                    {
                        // Hochformat
                        if (frameHeight > frameWidth)
                        {
                            actionBarHeight = frameWidth / 8;
                            grDdSubFromY = frameWidth / 6;
                            grScrollLeft.Margin = new Thickness(0, 0 - (frameWidth / 3.6), 0, 0 - (frameWidth / 3.6));
                            grScrollRight.Margin = new Thickness(0, 0 - (frameWidth / 3.6), 0, 0 - (frameWidth / 3.6));
                            prLeft.Width = (frameLeftWidth / 12) - 8;
                            prRight.Width = (frameLeftWidth / 12) - 8;
                            prLeft.Margin = new Thickness(0, 8, 4, 0);
                            prRight.Margin = new Thickness(0, 8, 4, 0);
                        }

                        // Querformat
                        else
                        {
                            actionBarHeight = frameHeight / 8;
                            grDdSubFromY = frameHeight / 6;
                            grScrollLeft.Margin = new Thickness(0, 0 - (frameHeight / 3.6), 0, 0 - (frameHeight / 3.6));
                            grScrollRight.Margin = new Thickness(0, 0 - (frameHeight / 3.6), 0, 0 - (frameHeight / 3.6));
                            prLeft.Width = (frameLeftWidth / 12) - 8;
                            prRight.Width = (frameLeftWidth / 12) - 8;
                            prLeft.Margin = new Thickness(0, 8, 4, 0);
                            prRight.Margin = new Thickness(0, 8, 4, 0);
                        }
                    }

                    // Anpassungen die immer errechnet werdem
                    frameLeft.Width = frameLeftWidth;
                    frameLeft.Height = frameHeight;
                    frameRight.Width = frameRightWidth;
                    frameRightHeight = frameHeight;
                    spFrameLeft.Height = actionBarHeight;
                    spMenuLeft.Height = spFrameLeft.Height - 3;
                    grLeft.MinHeight = frameHeight - actionBarHeight;
                    spFrameRight.Height = actionBarHeight;
                    spMenuRight.Height = spFrameLeft.Height - 3;
                    grRight.MinHeight = frameHeight - actionBarHeight;

                    // Hochformat
                    if (frameHeight > frameWidth)
                    {
                        grScrollLeft.Width = frameWidth / 12;
                        grScrollRight.Width = frameWidth / 12;
                        grLeft.Padding = new Thickness(0, 0, frameWidth / 12, 0);
                        grRight.Padding = new Thickness(0, 0, frameWidth / 12, 0);
                    }

                    // Querformat
                    else
                    {
                        grScrollLeft.Width = frameHeight / 12;
                        grScrollRight.Width = frameHeight / 12;
                        grLeft.Padding = new Thickness(0, 0, frameHeight / 12, 0);
                        grRight.Padding = new Thickness(0, 0, frameHeight / 12, 0);
                    }

                    // Teile ausrichten
                    rectLine.Margin = new Thickness(frameLeftWidth, 0, 0, 0);
                    ellipseLine.Margin = new Thickness((frameLeftWidth + (rectLine.Width / 2) - (ellipseLine.Width / 2)), 0, 0, 0);
                    frameRight.Margin = new Thickness(frameLeftWidth + rectLine.Width, 0, 0, 0);
                }

                // Wenn Continuum
                if (continuum)
                {
                    // Hochformat
                    if (frameHeight > frameWidth)
                    {
                        grDdSubFromY = frameWidth / 6;
                    }

                    // Querformat
                    else
                    {
                        grDdSubFromY = frameHeight / 6;
                    }

                    // Beide Formate
                    actionBarHeight = 60;
                    grScrollLeft.Visibility = Visibility.Collapsed;
                    grScrollRight.Visibility = Visibility.Collapsed;
                    prLeft.Width = 40;
                    prRight.Width = 40;
                    prLeft.Margin = new Thickness(0, 8, 4, 0);
                    prRight.Margin = new Thickness(0, 8, 4, 0);
                    setIconSizeLeft = 1.75;
                    setIconSizeRight = 1.75;

                    // Anpassungen die immer errechnet werdem
                    frameLeft.Width = frameLeftWidth;
                    frameLeft.Height = frameHeight;
                    frameRight.Width = frameRightWidth;
                    frameRightHeight = frameHeight;
                    spFrameLeft.Height = actionBarHeight;
                    spMenuLeft.Height = spFrameLeft.Height - 3;
                    grLeft.MinHeight = frameHeight - actionBarHeight;
                    spFrameRight.Height = actionBarHeight;
                    spMenuRight.Height = spFrameLeft.Height - 3;
                    grRight.MinHeight = frameHeight - actionBarHeight;

                    // Hochformat
                    if (frameHeight > frameWidth)
                    {
                        grScrollLeft.Width = frameWidth / 12;
                        grScrollRight.Width = frameWidth / 12;
                        grLeft.Padding = new Thickness(0, 0, frameWidth / 12, 0);
                        grRight.Padding = new Thickness(0, 0, frameWidth / 12, 0);
                    }

                    // Querformat
                    else
                    {
                        grScrollLeft.Width = frameHeight / 12;
                        grScrollRight.Width = frameHeight / 12;
                        grLeft.Padding = new Thickness(0, 0, frameHeight / 12, 0);
                        grRight.Padding = new Thickness(0, 0, frameHeight / 12, 0);
                    }

                    // Teile ausrichten
                    rectLine.Margin = new Thickness(frameLeftWidth, 0, 0, 0);
                    ellipseLine.Margin = new Thickness((frameLeftWidth + (rectLine.Width / 2) - (ellipseLine.Width / 2)), 0, 0, 0);
                    frameRight.Margin = new Thickness(frameLeftWidth + rectLine.Width, 0, 0, 0);
                }
            }

            // Wenn Desktop
            else
            {
                // Anpassungen die einmalig errechnet werden
                if (actionBarHeight == 0)
                {
                    // Hochformat
                    if (frameHeight > frameWidth)
                    {
                        grDdSubFromY = frameWidth / 6;
                    }

                    // Querformat
                    else
                    {
                        grDdSubFromY = frameHeight / 6;
                    }

                    // Beide Formate
                    actionBarHeight = 60;
                    grScrollLeft.Visibility = Visibility.Collapsed;
                    grScrollRight.Visibility = Visibility.Collapsed;
                    prLeft.Width = 40;
                    prRight.Width = 40;
                    prLeft.Margin = new Thickness(0, 8, 4, 0);
                    prRight.Margin = new Thickness(0, 8, 4, 0);
                    setIconSizeLeft = 1.75;
                    setIconSizeRight = 1.75;
                }

                // Anpassungen die immer errechnet werdem
                frameLeft.Width = frameLeftWidth;
                frameLeft.Height = frameHeight;
                frameRight.Width = frameRightWidth;
                frameRightHeight = frameHeight;
                spFrameLeft.Height = actionBarHeight;
                spMenuLeft.Height = spFrameLeft.Height - 3;
                grLeft.MinHeight = frameHeight - actionBarHeight;
                spFrameRight.Height = actionBarHeight;
                spMenuRight.Height = spFrameLeft.Height - 3;
                grRight.MinHeight = frameHeight - actionBarHeight;

                // Hochformat
                if (frameHeight > frameWidth)
                {
                    grScrollLeft.Width = frameWidth / 12;
                    grScrollRight.Width = frameWidth / 12;
                    grLeft.Padding = new Thickness(0, 0, frameWidth / 12, 0);
                    grRight.Padding = new Thickness(0, 0, frameWidth / 12, 0);
                }

                // Querformat
                else
                {
                    grScrollLeft.Width = frameHeight / 12;
                    grScrollRight.Width = frameHeight / 12;
                    grLeft.Padding = new Thickness(0, 0, frameHeight / 12, 0);
                    grRight.Padding = new Thickness(0, 0, frameHeight / 12, 0);
                }

                // Teile ausrichten
                rectLine.Margin = new Thickness(frameLeftWidth, 0, 0, 0);
                ellipseLine.Margin = new Thickness((frameLeftWidth + (rectLine.Width / 2) - (ellipseLine.Width / 2)), 0, 0, 0);
                frameRight.Margin = new Thickness(frameLeftWidth + rectLine.Width, 0, 0, 0);
            }
        }
        // -----------------------------------------------------------------------
#endregion










#region Animation Start
        // Animation Start
        // -----------------------------------------------------------------------
        // Bauteile
        DispatcherTimer timerStart;
        bool scrollbarRed = false;
        bool animationStartIsRunning = true;
        Grid grHelp;



        // Methode Animation Hilfe
        private async void animationStart()
        {
            // Timer einstellen
            timerStart = new DispatcherTimer();
            timerStart.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timerStart.Tick += TimerStart_Tick;
            timerStart.Start();

            // Bauteile erstellen
            Grid grHelp = new Grid
            {
                Name = "grStartAnimation",
                Background = new SolidColorBrush(Colors.Transparent)
            };

            // UI Blockieren
            grMain.Children.Add(grHelp);

            // Begrüßung erstellen
            DialogEx dialogEx = new DialogEx(resource.GetString("002_BegrüßungTitel"), resource.GetString("002_Bergrüßung"));
            DialogExCheckBox dxCb = new DialogExCheckBox(resource.GetString("002_NichtAnzeigen"));
            dialogEx.AddCheckbox(dxCb);
            DialogExButtonsSet dxBtnSet = new DialogExButtonsSet();
            DialogExButton btn1 = new DialogExButton(resource.GetString("001_Weiter"));
            dxBtnSet.AddButton(btn1);
            dialogEx.AddButtonSet(dxBtnSet);
            await dialogEx.ShowAsync(grMain);

            // Wenn nicht mehr angezeigt werden soll
            if (dialogEx.GetCheckboxByContent(resource.GetString("002_NichtAnzeigen")))
            {
                await folderSettings.CreateFileAsync("animationStart.txt");
            }

            // Animation zurückstellen
            animationStartIsRunning = false;
        }



        // Timer der Startanimation
        private void TimerStart_Tick(object sender, object e)
        {
            // Wenn Animation läuft
            if (animationStartIsRunning)
            {
                // Farbe der Scrollbar ändern
                if (scrollbarRed)
                {
                    grScrollLeft.Background = new SolidColorBrush(Color.FromArgb(255, 207, 40, 40));
                    grScrollRight.Background = new SolidColorBrush(Color.FromArgb(255, 207, 40, 40));
                    scrollbarRed = false;
                }
                else
                {
                    grScrollLeft.Background = scbScroll;
                    grScrollRight.Background = scbScroll;
                    scrollbarRed = true;
                }
            }

            // Wenn Animation zu Ende
            else
            {
                // Animation zurücksetzen und Timer stoppen
                grScrollLeft.Background = scbScroll;
                grScrollRight.Background = scbScroll;
                grMain.Children.Remove(grMain.FindName("grStartAnimation") as UIElement);
                timerStart.Stop();
            }
        }








        // -----------------------------------------------------------------------
        #endregion

        

    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    


    // Sprache
    public sealed partial class Language : Page
    {





        // Bauteile und Variablen
        // -----------------------------------------------------------------------
        // Resources // Für verschiedene Sprachen
        private ResourceLoader resource = new ResourceLoader();

        // IsoStore Variabeln
        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
        IsolatedStorageFileStream filestream;
        StreamReader sr;
        StreamWriter sw;

        // Seiten Variabeln
        string[] LangCodes = { "ms-MY", "cs-CZ", "da-DK", "de-DE", "en-US", "es-ES", "fr-FR", "hr-HR", "it-IT", "lt-LT", "lv-LV", "hu-HU", "nl-NL", "nb-NO", "pl-PL", "pt-PT", "ro-RO", "sk-SK", "sv-SE", "vi-VN", "tr-TR", "el-GR", "bg-BG", "ru-RU", "ar-SA", "fa-IR", "hi-IN", "th-TH", "ko-KR", "zh-CN", "zh-TW", "ja-JP", "uk-UA" };
        string[] LangNames = { "Behasa Melayu", "Čeština", "dansk", "deutsch", "English", "español", "Français", "hrvatski", "italiano", "Lietuvių", "Latviešu", "magyar", "Nederlands", "norsk", "polski", "português", "română", "Slovenský", "Svenska", "Tiếng Việt", "Türkçe", "Ελληνικά", "Български", "русский", "العربية", "فارسی", "हिंदी", "ไทย", "한국어", "简体中文", "繁體中文", "日本語", "Український" };

        // Index momentaner Sprache
        int LangID = -1;
        // -----------------------------------------------------------------------





        // Klasse erzeugen
        // -----------------------------------------------------------------------
        public Language()
        {
            // UI Komponenten initialisieren
            this.InitializeComponent();

            // Größe erstellen
            grHeader.Height = MainPage.actionBarHeight + 3;

            // Überschrift erstellen
            tbHeader.FontSize = MainPage.actionBarHeight / 8 * 3;

            // Bauteile erstellen
            tbHeader.Text = resource.GetString("002_SpracheÄndern");

            // Liste der Sprachen erstellen
            for(int i = 0; i < LangNames.Count(); i++)
            {
                lbLanguage.Items.Add(LangNames[i]);
            }

            // Aktuelle Sprache ermitteln
            for (int i = 0; i < LangCodes.Count(); i++)
            {
                if (MainPage.language == LangCodes[i])
                {
                    LangID = i;
                    try
                    {
                        lbLanguage.SelectedIndex = LangID;
                    }
                    catch
                    { }
                    break;
                }
            }
        }
        // -----------------------------------------------------------------------





        // Sprache auswählen
        // ---------------------------------------------------------------------------------------------------
        // Variabeln
        bool SelectLang = true;
        
        // Aktion
        private async void lbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbLanguage.SelectedIndex != LangID & SelectLang)
            {
                // Nachricht ausgeben // Kein Name eingegeben
                DialogEx dEx = new DialogEx(resource.GetString("002_SpracheÄndern"));
                DialogExButtonsSet dBSet = new DialogExButtonsSet();
                DialogExButton dBtn = new DialogExButton(resource.GetString("001_Ja"));
                dBSet.AddButton(dBtn);
                DialogExButton dBtn2 = new DialogExButton(resource.GetString("001_Nein"));
                dBSet.AddButton(dBtn2);
                dEx.AddButtonSet(dBSet);
                await dEx.ShowAsync(grMain);

                // Wenn Ja
                if (dEx.GetAnswer() == resource.GetString("001_Ja"))
                {
                    Language = LangCodes[lbLanguage.SelectedIndex];
                    // Neue Sprachdatei erstellen
                    StorageFolder folderSettings = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Settings", CreationCollisionOption.OpenIfExists);
                    var F_Language = await folderSettings.CreateFileAsync("Language.txt", CreationCollisionOption.OpenIfExists);
                    await FileIO.WriteTextAsync(F_Language, Language);
                    // Sprache festlegen
                    try
                    {
                        var culture = new CultureInfo(Language);
                        Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = culture.Name;
                        CultureInfo.DefaultThreadCurrentCulture = culture;
                        CultureInfo.DefaultThreadCurrentUICulture = culture;
                        Application.Current.Exit();
                    }
                    catch
                    { }
                }

                // Wenn Nein
                else
                {
                    // Listbox Auswahl zurücksetzen
                    SelectLang = false;
                    lbLanguage.SelectedIndex = LangID;
                    SelectLang = true;
                }
            }
        }
        // ---------------------------------------------------------------------------------------------------





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

using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.OneDrive.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;



// Klassen für Ordner und Dateien
namespace FileManager
{



    // Klasse // Verwaten // Start Ordner
    // -----------------------------------------------------------------------
    public class ClassStartFolders
    {
        // Parameter und Argumente 
        public int id { get; set; }
        public string driveType { get; set; }
        public string folderType { get; set; }
        public string name { get; set; }
        public string iconName { get; set; }
        public StorageFolder storageFolder { get; set; }
        public string token { get; set; }
        public bool canBack { get; set; }
        public bool canMultiSelect { get; set; }
        public bool canCreateFolder { get; set; }
        public bool canSettings { get; set; }
        public bool canDrag { get; set; }
        public bool canDrop { get; set; }
        public bool canFlyout { get; set; }
        public bool canDelete { get; set; }
        public bool canCopy { get; set; }
        public bool canCut { get; set; }
        public bool canPaste { get; set; }
        public bool canShare { get; set; }
        public bool canRemove { get; set; }
        public bool canSignOut { get; set; }

        // Klasse erzeugen
        public ClassStartFolders(int id, string driveType, string folderType, string name, string iconName, StorageFolder storageFolder, string token, bool canFlyout, bool canDrag, bool canDrop, bool canRemove, bool canSignOut)
        {
            // Argumente übernehmen
            this.id = id;
            this.driveType = driveType;
            this.folderType = folderType;
            this.name = name;
            this.iconName = iconName;
            this.storageFolder = storageFolder;
            this.canFlyout = canFlyout;
            this.canDrag = canDrag;
            this.canDrop = canDrop;
            this.canRemove = canRemove;
            this.canSignOut = canSignOut;
            this.token = token;

            // Argumente erstellen
            canBack = false;
            canMultiSelect = false;
            canCreateFolder = false;
            canSettings = true;
            canDelete = false;
            canCopy = false;
            canCut = false;
            canPaste = false;
            canShare = false;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse // Verwalten // Ordner Telefon
    // -----------------------------------------------------------------------
    public class ClassPhoneFolders
    {
        // Parameter und Argumente
        public int id { get; set; }
        public string driveType { get; set; }
        public string folderType { get; set; }
        public string name { get; set; }
        public StorageFolder storageFolder { get; set; }
        public string iconName { get; set; }
        public bool canBack { get; set; }
        public bool canMultiSelect { get; set; }
        public bool canCreateFolder { get; set; }
        public bool canSettings { get; set; }
        public bool canDrag { get; set; }
        public bool canDrop { get; set; }
        public bool canFlyout { get; set; }
        public bool canDelete { get; set; }
        public bool canCopy { get; set; }
        public bool canCut { get; set; }
        public bool canPaste { get; set; }
        public bool canShare { get; set; }
            
        // Klasse erzeugen
        public ClassPhoneFolders(string folderType, string name, StorageFolder storageFolder, string iconName, bool canDrag, bool canDrop)
        {
            // Argumente übernehmen
            this.folderType = folderType;
            this.name = name;
            this.storageFolder = storageFolder;
            this.iconName = iconName;
            this.canDrag = canDrag;
            this.canDrop = canDrop;
            // Argumente erstellen
            id = -1;
            driveType = "phone";
            canBack = true;
            canMultiSelect = false;
            canCreateFolder = false;
            canSettings = false;
            canFlyout = true;
            canDelete = false;
            canCopy = true;
            canCut = false;
            canPaste = true;
            canShare = false;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse // Verwaltet // Ordnerbäume
    // -----------------------------------------------------------------------
    public class ClassFolderTree
    {
        // Parameter und Argumente
        public int id { get; set; }
        public string driveType { get; set; }
        public StorageFolder storageFolder { get; set; }
        public string token { get; set; }
        public string path { get; set; }
        public string description { get; set; }
        public bool canBack { get; set; }
        public bool canMultiSelect { get; set; }
        public bool canCreateFolder { get; set; }
        public bool canDrop { get; set; }
        public bool canSettings { get; set; }
        public bool canFlyout { get; set; }
        public bool canPaste { get; set; }
        public bool canSort { get; set; }
            
        // Klasse erzeugen
        public ClassFolderTree(int id, string driveType, StorageFolder storageFolder, string token, string path, string description, bool canBack, bool canMultiSelect, bool canCreateFolder, bool canSettings, bool canSort, bool canDrop, bool canFlyout, bool canPaste)
        {
            // Argumente übernehmen
            this.id = id;
            this.driveType = driveType;
            this.storageFolder = storageFolder;
            this.path = path;
            this.description = description;
            this.canBack = canBack;
            this.canMultiSelect = canMultiSelect;
            this.canCreateFolder = canCreateFolder;
            this.canSettings = canSettings;
            this.canDrop = canDrop;
            this.canFlyout = canFlyout;
            this.canPaste = canPaste;
            this.canSort = canSort;
            this.token = token;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse // Verwalten // Dateien und ListBox Items
    // -----------------------------------------------------------------------
    public class ClassFiles : INotifyPropertyChanged
    {
        // Parameter und Argumente
        public int index { get; set; }
        public string side { get; set; }
        public string driveType { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public DateTimeOffset date { get; set; }
        public StorageFolder storageFolder { get; set; }
        public StorageFile storageFile { get; set; }
        public Item item { get; set; }
        public Metadata metadata { get; set; }
        public BitmapImage icon { get; set; }
        public bool canDrag { get; set; }
        public bool canDrop { get; set; }
        public bool canFlyout { get; set; }
        public bool canDelete { get; set; }
        public bool canCopy { get; set; }
        public bool canCut { get; set; }
        public bool canPaste { get; set; }
        public bool canShare { get; set; }
        public bool canRename { get; set; }
        public bool canPinToStart { get; set; }
        public bool canRemove { get; set; }
        public bool canSignOut { get; set; }

        // Parameter und Agumente // Binding
        public double grListWidth { get; set; }
        public double grListHeight { get; set; }
        public Thickness grListMargin { get; set; }
        public double grListFontSize { get; set; }
        private string _grListTag;
        public string grListTag
        {
            get { return _grListTag; }
            set
            {
                _grListTag = value;
                NotifyPropertyChanged("grListTag");
            }
        }
        private SolidColorBrush _background;
        public SolidColorBrush background
        {
            get { return _background; }
            set
            {
                _background = value;
                NotifyPropertyChanged("background");
            }
        }
        private BitmapImage _imgSelected;
        public BitmapImage imgSelected
        {
            get { return _imgSelected; }
            set
            {
                _imgSelected = value;
                NotifyPropertyChanged("imgSelected");
            }
        }
        private Visibility _imgSelectedVisibility;
        public Visibility imgSelectedVisibility
        {
            get { return _imgSelectedVisibility; }
            set
            {
                _imgSelectedVisibility = value;
                NotifyPropertyChanged("imgSelectedVisibility");
            }
        }
        private BitmapImage _imgMenu;
        public BitmapImage imgMenu
        {
            get { return _imgMenu; }
            set
            {
                _imgMenu = value;
                NotifyPropertyChanged("imgMenu");
            }
        }
        private Visibility _imgMenuVisibility;
        public Visibility imgMenuVisibility
        {
            get { return _imgMenuVisibility; }
            set
            {
                _imgMenuVisibility = value;
                NotifyPropertyChanged("imgMenuVisibility");
            }
        }
        private double _imgMenuWidth;
        public double imgMenuWidth
        {
            get { return _imgMenuWidth; }
            set
            {
                _imgMenuWidth = value;
                NotifyPropertyChanged("imgMenuWidth");
            }
        }
        private Thickness _imgMenuMargin;
        public Thickness imgMenuMargin
        {
            get { return _imgMenuMargin; }
            set
            {
                _imgMenuMargin = value;
                NotifyPropertyChanged("imgMenuMargin");
            }
        }

        // Klasse erzeugen
        public ClassFiles(int index, string side, string driveType, string type, string name, DateTimeOffset date, StorageFolder storageFolder, StorageFile storageFile, Item item, Metadata metadata, BitmapImage icon, bool canDrag, bool canDrop, bool canFlyout, bool canDelete, bool canCopy, bool canCut, bool canPaste, bool canShare, bool canRemove, bool canSignOut)
        {
            // Argumente übernehmen
            this.index = index;
            this.side = side;
            this.driveType = driveType;
            this.type = type;
            this.name = name;
            this.storageFolder = storageFolder;
            this.storageFile = storageFile;
            this.item = item;
            this.metadata = metadata;
            this.icon = icon;
            this.canDrag = canDrag;
            this.canDrop = canDrop;
            this.canFlyout = canFlyout;
            this.canDelete = canDelete;
            this.canCopy = canCopy;
            this.canCut = canCut;
            this.canPaste = canPaste;
            this.canShare = canShare;
            canRename = canDelete;
            this.canRemove = canRemove;
            this.canSignOut = canSignOut;
            canPinToStart = false;
            if (storageFolder != null)
            {
                //canPinToStart = true;
            }

            // Argumente erstellen
            double scaleFactor = 1;
            if (side == "left")
            {
                scaleFactor = MainPage.setIconSizeLeft;
            }
            else
            {
                scaleFactor = MainPage.setIconSizeRight;
            }
            grListWidth = 50 * scaleFactor;
            grListHeight = 100 * scaleFactor;
            grListMargin = new Thickness(4);
            grListFontSize = 8 * scaleFactor;

            // Argumente erstellen // Binding
            grListTag = index + "~" + side;
            background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            imgSelected = new BitmapImage(new Uri("ms-appx:/Images/MulitUnchecked.png", UriKind.RelativeOrAbsolute));
            imgSelectedVisibility = Visibility.Collapsed;

            // Menü Bild erstellen
            if (MainPage.continuum)
            {
                if (canFlyout)
                {
                    imgMenu = new BitmapImage
                    {
                        UriSource = new Uri("ms-appx:/Images/Menu.png", UriKind.RelativeOrAbsolute),
                    };
                    imgMenuVisibility = Visibility.Visible;
                    imgMenuWidth = 17 * scaleFactor;
                    imgMenuMargin = new Thickness(0, 0, 0, 50 * scaleFactor);
                }
            }
        }

        // Hintergrundfarbe ändern
        public void changeBackground(SolidColorBrush solidColorBrush)
        {
            background = solidColorBrush;
        }

        // Tag ändern
        public void changeTag(string tag)
        {
            grListTag = tag;
        }

        // Item auswählen
        public void selectItem()
        {
            imgSelected = new BitmapImage(new Uri("ms-appx:/Images/MulitChecked.png", UriKind.RelativeOrAbsolute));
        }

        // Item abwählen
        public void unselectItem()
        {
            imgSelected = new BitmapImage(new Uri("ms-appx:/Images/MulitUnchecked.png", UriKind.RelativeOrAbsolute));
        }

        // PropertyChanged Event, zum aktuallisieren
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    // -----------------------------------------------------------------------





    // Klasse // Wird Benötigt und Dateien und Ordner zu löschen
    // -----------------------------------------------------------------------
    public class ClassDeleteFiles
    {
        // Parameter und Argumente
        public int index { get; set; }
        public List<int> indexes { get; set; }
        public bool allCantOpenFolders { get; set; }
        public bool allCantDeleteFiles { get; set; }
        public bool allCantDeleteFolders { get; set; }
        public List<string> cantOpenFolders { get; set; }
        public List<string> cantDeleteFolders { get; set; }
        public List<string> cantDeleteFiles { get; set; }
        public List<int> deletedIndexes { get; set; }
        public int value = 0;

        // Klasse erzeugen
        public ClassDeleteFiles()
        {
            // Argumente erstellen
            index = 0;
            indexes = new List<int>();
            allCantOpenFolders = false;
            allCantDeleteFiles = false;
            allCantDeleteFolders = false;
            cantOpenFolders = new List<string>();
            cantDeleteFolders = new List<string>();
            cantDeleteFiles = new List<string>();
            deletedIndexes = new List<int>();
            value = 0;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse // Wird Benötigt und Dateien und Ordner einzufügen
    // -----------------------------------------------------------------------
    public class ClassCopyOrMoveFilesAndFolders
    {
        // Parameter und Argumente
        public bool dragAndDrop { get; set; }
        public bool move { get; set; }
        public bool allNameCollisionOptionFiles { get; set; }
        private NameCollisionOption _nameCollisionOptionFiles;
        public NameCollisionOption nameCollisionOptionFiles
        {
            get
            {
                return _nameCollisionOptionFiles;
            }
            set
            {
                _nameCollisionOptionFiles = value;
                if (value == NameCollisionOption.FailIfExists)
                {
                    nameCreationCollisionOptionFiles = CreationCollisionOption.FailIfExists;
                }
                else if (value == NameCollisionOption.GenerateUniqueName)
                {
                    nameCreationCollisionOptionFiles = CreationCollisionOption.GenerateUniqueName;
                }
                else if (value == NameCollisionOption.ReplaceExisting)
                {
                    nameCreationCollisionOptionFiles = CreationCollisionOption.ReplaceExisting;
                }
            }
        }
        public CreationCollisionOption nameCreationCollisionOptionFiles { get; set; }
        public bool allFileNotFound { get; set; }
        public bool allCantPasteFiles { get; set; }
        public bool allCantOpenFolder { get; set; }
        public bool allNameCollisionOptionFolders { get; set; }
        public CreationCollisionOption nameCollisionOptionsFoldes { get; set; }
        public bool allCantDeleteFolders { get; set; }
        public List<int> deletedIndexes { get; set; }
        public int value { get; set; }
        public bool errors { get; set; }
        public bool deleteIfMoving { get; set; }

        // Klasse erzeugen
        public ClassCopyOrMoveFilesAndFolders(bool dragAndDrop = false)
        {
            // Argumente erstellen
            this.dragAndDrop = dragAndDrop;
            move = false;
            allNameCollisionOptionFiles = false;
            nameCollisionOptionFiles = NameCollisionOption.FailIfExists;
            allFileNotFound = false;
            allCantPasteFiles = false;
            allCantOpenFolder = false;
            allNameCollisionOptionFolders = false;
            nameCollisionOptionsFoldes = CreationCollisionOption.FailIfExists;
            allCantDeleteFolders = false;
            deletedIndexes = new List<int>();
            value = 0;
            deleteIfMoving = true;
            errors = false;
        }
    }
    // -----------------------------------------------------------------------




    // Klasse // Wird Benötigt um Dateien und Ordner umzubenennen
    // -----------------------------------------------------------------------
    public class ClassRenameFiles
    {
        // Parameter und Argumente
        public int index { get; set; }
        public List<int> indexes { get; set; }
        public bool allCantRenameFilesAndFolders { get; set; }
        public int value = 0;

        // Klasse erzeugen
        public ClassRenameFiles()
        {
            // Argumente erstellen
            index = 0;
            indexes = new List<int>();
            allCantRenameFilesAndFolders = false;
            value = 0;
        }
    }
    // -----------------------------------------------------------------------


}



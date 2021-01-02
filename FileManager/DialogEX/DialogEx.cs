using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;



// Namespace
namespace Xtrose
{



    #region DialogEx
    // Klasse DialogEx
    // -----------------------------------------------------------------------
    public class DialogEx
    {
        // Parameter und Argumente
        public string Title { get; set; }
        public int TitleFontSize { get; set; }
        public FontWeight TitleFontWeight { get; set; }
        public Thickness TitleMargin { get; set; }
        public string Content { get; set; }
        public int ContentFontSize { get; set; }
        public FontWeight ContentFontWeight { get; set; }
        public Thickness ContentMargin { get; set; }
        public SolidColorBrush BackgroundBrush { get; set; }
        public SolidColorBrush DialogBackgroundBrush { get; set; }
        public SolidColorBrush BorderBrush { get; set; }
        public SolidColorBrush TitleForeground { get; set; }
        public SolidColorBrush ContentForeground { get; set; }
        public int MinHeight { get; set; }
        public bool BugFix { get; set; }
        public double Width { get; set; }
        
        // Verarbeitungs Parameter
        private List<TextBlock> dialogExTextBlocks = new List<TextBlock>();
        private List<string> buttonsContent = new List<string>();
        private List<string> checkBoxsContent = new List<string>();
        private List<bool> checkBoxsIsChecked = new List<bool>();
        private string isAnswer;
        private List<string> textBoxsTitle = new List<string>();
        private List<string> textBoxsText = new List<string>();
        private List<string> passwordBoxsTitle = new List<string>();
        private List<string> passwordBoxsPassword = new List<string>();
        private List<DialogExProgressBar> dialogExProgressBars = new List<DialogExProgressBar>();
        
        // Ergebnis Variablen
        private bool isCreated = false;
        private bool IsCompleted = false;
        
        // Bauteile
        private ContentDialog dialogEx = new ContentDialog();
        private StackPanel stackPanelContent = new StackPanel();
        private Grid targetGrid = null;
        private Grid _gridDialogEx = null;
        Grid grDialog = null;

        // Timer Animation
        private DispatcherTimer timer = new DispatcherTimer();
        string timerAction = "show";


        // Klasse erzeugen
        public DialogEx(string Title = null, string Content = null)
        {
            // Argumente übernehmen
            this.Title = Title;
            this.Content = Content;

            // Einstellungen <-- Standart Einstellungen zum Anpassen des Designs
            TitleForeground = null;
            TitleFontSize = 16;
            TitleFontWeight = FontWeights.Bold;
            TitleMargin = new Thickness(0, 6, 0, 0);
            ContentForeground = null;
            ContentFontSize = 14;
            ContentFontWeight = FontWeights.Normal;
            ContentMargin = new Thickness(0, 0, 0, 6);
            BorderBrush = new SolidColorBrush(Color.FromArgb(255, 207, 40, 40));
            BackgroundBrush = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0));
            DialogBackgroundBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            MinHeight = 120;
            Width = 0;
            BugFix = true;

            // Content Dialog einstellen <-- Darf nicht verändert werden
            dialogEx.Visibility = Visibility.Collapsed;

            // Timer einstellen
            timer.Stop();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timer.Tick += Timer_Tick;
        }



        // TextBlock hinzufügen
        public void AddTextBlock(DialogExTextBlock dialogExTextBlock)
        {
            // TextBox zusammentellen
            TextBlock textBlock = new TextBlock();
            textBlock.Text = dialogExTextBlock.Text;
            textBlock.FontSize = dialogExTextBlock.FontSize;
            textBlock.Foreground = dialogExTextBlock.Foreground;
            textBlock.Margin = dialogExTextBlock.Margin;
            textBlock.TextAlignment = dialogExTextBlock.textAlignment;
            textBlock.HorizontalAlignment = dialogExTextBlock.HorizontalAlignment;
            dialogExTextBlocks.Add(textBlock);

            // Textbox in StackPanel einfügen
            stackPanelContent.Children.Add(dialogExTextBlocks[dialogExTextBlocks.Count() - 1]);
        }



        // TextBox hinzufügen
        public void AddTextBox(DialogExTextBox dialogExTextBox)
        {
            // Title und Texte in Liste einfühen
            textBoxsTitle.Add(dialogExTextBox.Title);
            textBoxsText.Add(dialogExTextBox.Text);

            // TextBox zusammentellen
            TextBox textBox = new TextBox();
            if (dialogExTextBox.Title != null & dialogExTextBox.Title != "")
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = dialogExTextBox.FontSize;
                textBlock.Margin = dialogExTextBox.Margin;
                textBlock.HorizontalAlignment = dialogExTextBox.HorizontalAlignment;
                textBlock.Text = dialogExTextBox.Title;
                if (dialogExTextBox.Foreground != null)
                {
                    textBlock.Foreground = dialogExTextBox.Foreground;
                }
                stackPanelContent.Children.Add(textBlock);
            }
            else
            {
                textBox.Margin = dialogExTextBox.Margin;
            }
            textBox.Text = dialogExTextBox.Text;
            textBox.Tag = (textBoxsText.Count - 1).ToString();
            textBox.KeyDown += delegate (object sender, KeyRoutedEventArgs e)
            {
                int index = Convert.ToInt16(textBox.Tag);
                textBoxsText[index] = textBox.Text;
            };
            textBox.KeyUp += delegate (object sender, KeyRoutedEventArgs e)
            {
                int index = Convert.ToInt16(textBox.Tag);
                textBoxsText[index] = textBox.Text;
                if (e.Key == VirtualKey.Enter)
                {
                    (sender as TextBox).IsEnabled = false;
                    (sender as TextBox).IsEnabled = true;
                }
            };

            // Textbox in StackPanel einfügen
            stackPanelContent.Children.Add(textBox);
        }
        


        // PasswortBox hinzufügen
        public void AddPasswordBox(DialogExPasswordBox dialogExPasswordBox)
        {
            // Title und Passwort in Liste einfügen
            passwordBoxsTitle.Add(dialogExPasswordBox.Title);
            passwordBoxsPassword.Add(dialogExPasswordBox.Password);

            // PasswordBox zusammenstellen
            PasswordBox passwordBox = new PasswordBox();
            if (dialogExPasswordBox.Title != null & dialogExPasswordBox.Title != "")
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = dialogExPasswordBox.FontSize;
                textBlock.Margin = dialogExPasswordBox.Margin;
                textBlock.HorizontalAlignment = dialogExPasswordBox.HorizontalAlignment;
                textBlock.Text = dialogExPasswordBox.Title;
                if (dialogExPasswordBox.Foreground != null)
                {
                    textBlock.Foreground = dialogExPasswordBox.Foreground;
                }
                stackPanelContent.Children.Add(textBlock);
            }
            else
            {
                passwordBox.Margin = dialogExPasswordBox.Margin;
            }
            passwordBox.Password = dialogExPasswordBox.Password;
            passwordBox.Tag = (passwordBoxsPassword.Count - 1).ToString();
            passwordBox.KeyDown += delegate (object sender, KeyRoutedEventArgs e)
            {
                int index = Convert.ToInt16(passwordBox.Tag);
                passwordBoxsPassword[index] = passwordBox.Password;
            };
            passwordBox.KeyUp += delegate (object sender, KeyRoutedEventArgs e)
            {
                int index = Convert.ToInt16(passwordBox.Tag);
                passwordBoxsPassword[index] = passwordBox.Password;
                if (e.Key == VirtualKey.Enter)
                {
                    (sender as PasswordBox).IsEnabled = false;
                    (sender as PasswordBox).IsEnabled = true;
                }
            };

            // PasswordBox in StackPanel einfügen
            stackPanelContent.Children.Add(passwordBox);
        }
        


        // CheckBox hinzufügen
        public void AddCheckbox(DialogExCheckBox dialogExCheckBox)
        {
            // Content und Checked in Liste einfügen
            checkBoxsContent.Add(dialogExCheckBox.Content);
            checkBoxsIsChecked.Add(dialogExCheckBox.IsChecked);

            // CheckBox zusammenstellen
            CheckBox checkBox = new CheckBox();
            checkBox.Content = dialogExCheckBox.Content;
            checkBox.IsChecked = dialogExCheckBox.IsChecked;
            checkBox.FontSize = dialogExCheckBox.FontSize;
            checkBox.Margin = dialogExCheckBox.Margin;
            checkBox.HorizontalAlignment = dialogExCheckBox.HorizontalAlignment;
            if(dialogExCheckBox.Foreground != null)
            {
                checkBox.Foreground = dialogExCheckBox.Foreground;
            }
            checkBox.Tag = (checkBoxsContent.Count() - 1).ToString();
            checkBox.Checked += delegate
            {
                int index = Convert.ToInt16(checkBox.Tag);
                checkBoxsIsChecked[index] = true;
            };
            checkBox.Unchecked += delegate
            {
                int index = Convert.ToInt16(checkBox.Tag);
                checkBoxsIsChecked[index] = false;
            };

            // CheckBox in StackPanel einfügen
            stackPanelContent.Children.Add(checkBox);
        }
        


        // ButtonSet hinzufügen
        public async void AddButtonSet(DialogExButtonsSet dialogExButtonSet)
        {
            // WrapPanel erstellen
            WrapPanel wrapPanal = new WrapPanel();
            wrapPanal.Margin = dialogExButtonSet.Margin;
            wrapPanal.HorizontalAlignment = dialogExButtonSet.HorizontalAlignment;

            // Buttons durchlaufen und anpassen
            for (int i = 0; i < dialogExButtonSet.dialogExButtons.Count(); i++)
            {
                // Button Content in Liste einfügen
                buttonsContent.Add(dialogExButtonSet.dialogExButtons[i].Content);

                // Button Margin festlegen
                Thickness buttonMargin = new Thickness(0, 0, 6, 6);
                if (wrapPanal.HorizontalAlignment == HorizontalAlignment.Right)
                {
                    buttonMargin = new Thickness(6, 0, 0, 6);
                }

                // Standard Buttons
                if (!dialogExButtonSet.UseLinkButtons)
                {
                    Button button = new Button();
                    button.MinWidth = dialogExButtonSet.dialogExButtons[i].Width;
                    button.Margin = buttonMargin;
                    button.FontSize = dialogExButtonSet.dialogExButtons[i].FontSize;
                    if (dialogExButtonSet.dialogExButtons[i].Foreground != null)
                    {
                        button.Foreground = dialogExButtonSet.dialogExButtons[i].Foreground;
                    }
                    button.Content = dialogExButtonSet.dialogExButtons[i].Content;
                    button.Tag = (buttonsContent.Count() - 1).ToString();
                    button.Click += delegate
                    {
                        isAnswer = buttonsContent[Convert.ToInt32(button.Tag)];
                        IsCompleted = true;
                        Hide();
                    };
                    Grid grid = new Grid();
                    grid.Children.Add(button);
                    wrapPanal.Children.Add(grid);
                }

                // Link Buttons
                else
                {
                    TextBlock button = new TextBlock();
                    button.Margin = buttonMargin;
                    button.FontSize = dialogExButtonSet.dialogExButtons[i].FontSize;
                    if (dialogExButtonSet.dialogExButtons[i].Foreground != null)
                    {
                        button.Foreground = dialogExButtonSet.dialogExButtons[i].Foreground;
                    }
                    button.FontStyle = dialogExButtonSet.dialogExButtons[i].FontStyle;
                    button.Text = dialogExButtonSet.dialogExButtons[i].Content;
                    button.Tag = (buttonsContent.Count() - 1).ToString();
                    button.PointerReleased += delegate
                    {
                        isAnswer = buttonsContent[Convert.ToInt32(button.Tag)];
                        IsCompleted = true;
                        Hide();
                    };
                    wrapPanal.Children.Add(button);
                }
            }

            // WrapPanal Bug bei UWP beheben
            if (BugFix)
            {
                Grid gr = new Grid();
                gr.Width = 1;
                gr.Height = 1;
                wrapPanal.Children.Add(gr);
            }

            // ButtonSet in StackPanel einfügen
            stackPanelContent.Children.Add(wrapPanal);
        }
        


        // ProgressBar hinzufügen
        public void AddProgressBar(DialogExProgressBar dialogExProgressBar)
        {
            // ProgressBar zusammenstellen
            dialogExProgressBar.progressBar.Margin = dialogExProgressBar.Margin;
            dialogExProgressBar.progressBar.Minimum = dialogExProgressBar.Minimum;
            dialogExProgressBar.progressBar.Maximum = dialogExProgressBar.Maximun;
            dialogExProgressBars.Add(dialogExProgressBar);

            // ProgressBar in StackPanel einfügen
            stackPanelContent.Children.Add(dialogExProgressBars[dialogExProgressBars.Count() - 1].progressBar);
        }
        


        // ProgressRing 
        public void AddProgressRing(DialogExProgressRing dialogExProgressRing)
        {
            // ProgressRing zusammenstellen
            ProgressRing progressRing = new ProgressRing();
            progressRing.Margin = dialogExProgressRing.Margin;
            progressRing.IsActive = true;

            // ProgressRing in StackPanel einfügen
            stackPanelContent.Children.Add(progressRing);
        }



        // DialogEx zusammenstellen und ausgeben
        public async Task ShowAsync(Grid targetGrid)
        {
            // DialogEx zusammentellen // Wenn nicht bereits zusammengestellt
            if (!isCreated)
            {
                // Grid übernehmen
                this.targetGrid = targetGrid;

                // DialogEx erstellen
                _gridDialogEx = new Grid();
                _gridDialogEx.Name = "_gridDialogEx";
                _gridDialogEx.Background = BackgroundBrush;
                grDialog = new Grid();
                grDialog.BorderThickness = new Thickness(1);
                grDialog.BorderBrush = BorderBrush;
                grDialog.Background = DialogBackgroundBrush;
                if (Width == 0)
                {
                    double _width = Window.Current.CoreWindow.Bounds.Width;
                    double _height = Window.Current.CoreWindow.Bounds.Height;
                    Width = _width - 24;
                    if (_width > _height)
                    {
                        Width = _height - 24;
                    }
                }
                grDialog.Width = Width;
                grDialog.MinHeight = MinHeight;
                grDialog.VerticalAlignment = VerticalAlignment.Center;
                grDialog.HorizontalAlignment = HorizontalAlignment.Center;
                _gridDialogEx.Children.Add(grDialog);

                // StackPanelMain
                StackPanel stackPanelMain = new StackPanel();
                stackPanelMain.Margin = new Thickness(12);

                // Bauteile erstellen // Title
                TextBlock textBlockTitle = new TextBlock();
                textBlockTitle.Text = Title;
                textBlockTitle.FontWeight = TitleFontWeight;
                textBlockTitle.FontSize = TitleFontSize;
                if (TitleForeground != null)
                {
                    textBlockTitle.Foreground = TitleForeground;
                }
                textBlockTitle.Margin = TitleMargin;
                stackPanelMain.Children.Add(textBlockTitle);

                // Bauteile erstellen // Content
                if (Content != null)
                {
                    TextBlock textBlockContent = new TextBlock();
                    textBlockContent.Text = Content;
                    textBlockContent.TextWrapping = TextWrapping.Wrap;
                    textBlockContent.FontSize = ContentFontSize;
                    textBlockContent.FontWeight = ContentFontWeight;
                    if (ContentForeground != null)
                    {
                        textBlockContent.Foreground = ContentForeground;
                    }
                    textBlockContent.Margin = ContentMargin;
                    stackPanelContent.Children.Insert(0, textBlockContent);
                }

                // Bauteile in Content Dialog einfügen
                stackPanelMain.Children.Add(stackPanelContent);
                grDialog.Children.Add(stackPanelMain);

                // Angeben das bereits erstellt wurde
                isCreated = true;
            }

            // DialogEx ausgeben
            targetGrid.Children.Add(_gridDialogEx);
            _gridDialogEx.Opacity = 0.1;
            timerAction = "show";
            timer.Start();
            await dialogEx.ShowAsync();
        }
        
        // Animation
        private void Timer_Tick(object sender, object e)
        {
            // Einblenden
            if (timerAction == "show")
            {
                // Hintergrund Einblenden // Dialog Animieren
                if (_gridDialogEx.Opacity <= 1.0)
                {
                    _gridDialogEx.Opacity += 0.1;
                }
                else
                {
                    // Dialog ausrichten
                    _gridDialogEx.Opacity = 1.0;

                    // Timer stoppen
                    timer.Stop();
                }
            }
        }



        // Dialog Ex entfernen
        public void Hide()
        {
            // Dialog aus UI entfernen
            try
            {
                targetGrid.Children.Remove(targetGrid.FindName("_gridDialogEx") as UIElement);
            }
            catch { }

            // Dialog entfernen
            dialogEx.Hide();
        }
        


        // DialogBox Antwort zurückgeben
        public string GetAnswer()
        {
            return isAnswer;
        }
        


        // TextBox Text zurückgeben // Nach Title
        public string GetTextBoxTextByTitle(string title)
        {
            string _return = "";
            for (int i = 0; i < textBoxsTitle.Count(); i++)
            {
                if (title == textBoxsTitle[i])
                {
                    _return = textBoxsText[i];
                }
            }
            return _return;
        }
        


        // TextBox Text zurückgeben // Nach Index
        public string GetTextBoxTextByIndex(int index)
        {
            return textBoxsText[index];
        }



        // PasswortBox Text zurückgeben // Nach Title
        public string GetPasswordBoxTextByTitle(string title)
        {
            string _return = "";
            for (int i = 0; i < passwordBoxsTitle.Count(); i++)
            {
                if (title == passwordBoxsTitle[i])
                {
                    _return = passwordBoxsPassword[i];
                }
            }
            return _return;
        }



        // PasswordBox Text zurückgeben // Nach Index
        public string GetPasswordBoxTextByIndex(int index)
        {
            return passwordBoxsPassword[index];
        }



        // Checkbox IsChecked zurückgeben // Nach Content
        public bool GetCheckboxByContent(string content)
        {
            bool _return = false;
            for (int i = 0; i < checkBoxsContent.Count(); i++)
            {
                if (content == checkBoxsContent[i])
                {
                    _return = checkBoxsIsChecked[i];
                }
            }
            return _return;
        }



        // CheckBox Is Checked zurückgeben // Nach Index
        public bool GetCheckboxByIndex(int index)
        {
            return checkBoxsIsChecked[index];
        }



        // ProgressBar Value ändern // Nach Index
        public void SetProgressBarValueByIndex(int index, int value)
        {
            dialogExProgressBars[index].Value = value;
        }



        // ProgressBar Maximum ändern // Nach Index
        public void SetProgressBarMaximumByIndex(int index, int maximum)
        {
            dialogExProgressBars[index].Maximun = maximum;
        }



        // ProgressBar Visibility ändern // Nach Index
        public void SetProgressBarVisibilityByIndex(int index, Visibility visibility)
        {
            dialogExProgressBars[index].progressBar.Visibility = visibility;
        }



        // TextBox Text ändern // Nach Index
        public void SetTextBoxTextByIndex(int index, string text)
        {
            dialogExTextBlocks[index].Text = text;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse DialogExTextBlock
    public class DialogExTextBlock
    // -----------------------------------------------------------------------
    {
        // Parameter und Argumente
        public string Text { get; set; }
        public Thickness Margin { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public int FontSize { get; set; }
        public TextAlignment textAlignment { get; set; }

        // Klasse erzeugen
        public DialogExTextBlock(string Text = "")
        {
            // Argumente übernehmen
            this.Text = Text;
            // Argumente erstellen
            Margin = new Thickness(0, 6, 0, 0);
            Foreground = null;
            HorizontalAlignment = HorizontalAlignment.Left;
            FontSize = 14;
            textAlignment = TextAlignment.Left;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse DialogExTextBox
    public class DialogExTextBox
    // -----------------------------------------------------------------------
    {
        // Parameter und Argumente
        public string Title { get; set; }
        public string Text { get; set; }
        public Thickness Margin { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public int FontSize { get; set; }

        // Klasse erzeugen
        public DialogExTextBox(string Title = null, string Text = "")
        {
            // Argumente übernehmen
            this.Title = Title;
            this.Text = Text;
            // Argumente erstellen
            Margin = new Thickness(0, 6, 0, 0);
            Foreground = null;
            HorizontalAlignment = HorizontalAlignment.Left;
            FontSize = 14;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse DialogExPasswordBox
    public class DialogExPasswordBox
    // -----------------------------------------------------------------------
    {
        // Parameter und Argumente
        public string Title { get; set; }
        public string Password { get; set; }
        public Thickness Margin { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public int FontSize { get; set; }

        // Klasse erzeugen
        public DialogExPasswordBox(string Title = null, string Password = "")
        {
            // Argumente übernehmen
            this.Title = Title;
            this.Password = Password;
            Margin = new Thickness(0, 6, 0, 0);
            Foreground = null;
            HorizontalAlignment = HorizontalAlignment.Left;
            FontSize = 14;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse DialogExCheckBox
    public class DialogExCheckBox
    // -----------------------------------------------------------------------
    {
        // Parameter und Argumente
        public string Content { get; set; }
        public bool IsChecked { get; set; }
        public Thickness Margin { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public int FontSize { get; set; }

        // Klasse erzeugen
        public DialogExCheckBox(string Content, bool IsChecked = false)
        {
            // Argumente übernehmen
            this.Content = Content;
            this.IsChecked = IsChecked;
            // Argumente erstellen
            Margin = new Thickness(0, 6, 0, 0);
            Foreground = null;
            HorizontalAlignment = HorizontalAlignment.Left;
            FontSize = 13;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse DialogExButtonSet
    public class DialogExButtonsSet
    // -----------------------------------------------------------------------
    {
        // Parameter und Argumente
        public bool UseLinkButtons { get; set; }
        public Thickness Margin { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public HorizontalAlignment HorizontalAlignment { get; set; }
        public List<DialogExButton> dialogExButtons { get; set; }

        // Klasse erzeugen
        public DialogExButtonsSet()
        {
            // Argumente erstellen
            UseLinkButtons = false;
            Margin = new Thickness(0, 6, 0, 0);
            Foreground = null;
            HorizontalAlignment = HorizontalAlignment.Left;
            dialogExButtons = new List<DialogExButton>();
        }
        
        // Button hinzufügen
        public void AddButton(DialogExButton dialogExButton)
        {
            // Button der List hinzufügen
            dialogExButtons.Add(dialogExButton);
        }
    }
    // -----------------------------------------------------------------------





    // Klasse DialogExButton
    public class DialogExButton
    // -----------------------------------------------------------------------
    {
        // Parameter und Argumente
        public string Content { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public int FontSize { get; set; }
        public int Width { get; set; }
        public FontStyle FontStyle { get; set; }

        // Klasse erzeugen
        public DialogExButton(string Content)
        {
            // Argumente übernehmen
            this.Content = Content;
            // Argumente erstellen
            Foreground = null;
            FontSize = 13;
            Width = 80;
            FontStyle = FontStyle.Normal;
        }
    }
    // -----------------------------------------------------------------------





    // Klasse DialogExProgressBar
    public class DialogExProgressBar
    // -----------------------------------------------------------------------
    {
        // Paramenter und Argumente
        public ProgressBar progressBar { get; set; }
        public Thickness Margin { get; set; }
        public int Minimum { get; set; }
        public int Maximun { get; set; }
        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                progressBar.Value = value;
            }
        }

        // Klasse erzeugen
        public DialogExProgressBar()
        {
            // Argumente erstellen
            progressBar = new ProgressBar();
            Margin = new Thickness(0, 12, 0, 0);
            Minimum = 0;
            Maximun = 0;
            Value = 0;
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





    // Klasse DialogExProgressRing
    public class DialogExProgressRing
    // -----------------------------------------------------------------------
    {
        // Parameter und Argumente
        public Thickness Margin { get; set; }

        // Klasse erzeugen
        public DialogExProgressRing()
        {
            // Argumente erstellen
            Margin = new Thickness(0, 24, 0, 0);
        }
    }
    // -----------------------------------------------------------------------
    #endregion










    #region WrapPanal
    // -----------------------------------------------------------------------
    public partial class WrapPanel : Panel
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct OrientedSize
        {
            private Orientation orientation;
            private double direct;
            private double indirect;

            public Orientation Orientation
            {
                get { return orientation; }
            }

            public double Direct
            {
                get { return direct; }
                set { direct = value; }
            }

            public double Indirect
            {
                get { return indirect; }
                set { indirect = value; }
            }

            public double Width
            {
                get
                {
                    return (Orientation == Orientation.Horizontal) ? Direct : Indirect;
                }
                set
                {
                    if (Orientation == Orientation.Horizontal)
                    {
                        Direct = value;
                    }
                    else
                    {
                        Indirect = value;
                    }
                }
            }

            public double Height
            {
                get
                {
                    return (Orientation != Orientation.Horizontal) ? Direct : Indirect;
                }
                set
                {
                    if (Orientation != Orientation.Horizontal)
                    {
                        Direct = value;
                    }
                    else
                    {
                        Indirect = value;
                    }
                }
            }

            public OrientedSize(Orientation orientation)
                : this(orientation, 0.0, 0.0)
            {

            }

            public OrientedSize(Orientation orientation, double width, double height)
            {
                this.orientation = orientation;
                direct = 0.0;
                indirect = 0.0;
                this.Width = width;
                this.Height = height;
            }
        }

        private bool ignorePropertyChange;

        public static bool AreClose(double left, double right)
        {
            if (left == right)
            {
                return true;
            }
            double a = (Math.Abs(left) + Math.Abs(right) + 10.0) * 2.2204460492503131E-16;
            double b = left - right;
            return (-a < b) && (a > b);
        }

        public static bool IsGreaterThan(double left, double right)
        {
            return (left > right) && !AreClose(left, right);
        }

        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(WrapPanel),
                new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));

        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double), typeof(WrapPanel),
                new PropertyMetadata(double.NaN, OnItemHeightOrWidthPropertyChanged));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(WrapPanel),
                new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));

        private static void OnOrientationPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            WrapPanel source = (WrapPanel)d;
            Orientation value = (Orientation)e.NewValue;
            if (source.ignorePropertyChange)
            {
                source.ignorePropertyChange = false;
                return;
            }
            if ((value != Orientation.Horizontal) && (value != Orientation.Vertical))
            {
                source.ignorePropertyChange = true;
                source.SetValue(OrientationProperty, (Orientation)e.OldValue);
                throw new ArgumentException("OnOrientationPropertyChanged InvalidValue", "value");
            }
            source.InvalidateMeasure();
        }

        private static void OnItemHeightOrWidthPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            WrapPanel source = (WrapPanel)d;
            double value = (double)e.NewValue;
            if (source.ignorePropertyChange)
            {
                source.ignorePropertyChange = false;
                return;
            }
            if (!double.IsNaN(value) && ((value <= 0.0) || double.IsPositiveInfinity(value)))
            {
                source.ignorePropertyChange = true;
                source.SetValue(e.Property, (double)e.OldValue);
                throw new ArgumentException("OnItemHeightOrWidthPropertyChanged InvalidValue", "value");
            }
            source.InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Orientation o = Orientation;
            OrientedSize lineSize = new OrientedSize(o);
            OrientedSize totalSize = new OrientedSize(o);
            OrientedSize maximumSize = new OrientedSize(o, constraint.Width, constraint.Height);
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            Size itemSize = new Size(hasFixedWidth ? itemWidth : constraint.Width,
                hasFixedHeight ? itemHeight : constraint.Height);
            foreach (UIElement element in Children)
            {
                element.Measure(itemSize);
                OrientedSize elementSize = new OrientedSize(o,
                    hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                    hasFixedHeight ? itemHeight : element.DesiredSize.Height);
                if (IsGreaterThan(lineSize.Direct + elementSize.Direct, maximumSize.Direct))
                {
                    totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
                    totalSize.Indirect += lineSize.Indirect;
                    lineSize = elementSize;
                    if (IsGreaterThan(elementSize.Direct, maximumSize.Direct))
                    {
                        totalSize.Direct = Math.Max(elementSize.Direct, totalSize.Direct);
                        totalSize.Indirect += elementSize.Indirect;
                        lineSize = new OrientedSize(o);
                    }
                }
                else
                {
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }
            totalSize.Direct = Math.Max(lineSize.Direct, totalSize.Direct);
            totalSize.Indirect += lineSize.Indirect;
            return new Size(totalSize.Width, totalSize.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Orientation o = Orientation;
            OrientedSize lineSize = new OrientedSize(o);
            OrientedSize maximumSize = new OrientedSize(o, finalSize.Width, finalSize.Height);
            double itemWidth = ItemWidth;
            double itemHeight = ItemHeight;
            bool hasFixedWidth = !double.IsNaN(itemWidth);
            bool hasFixedHeight = !double.IsNaN(itemHeight);
            double indirectOffset = 0;
            double? directDelta = (o == Orientation.Horizontal) ?
                (hasFixedWidth ? (double?)itemWidth : null) :
                (hasFixedHeight ? (double?)itemHeight : null);
            UIElementCollection children = Children;
            int count = children.Count;
            int lineStart = 0;
            for (int lineEnd = 0; lineEnd < count; lineEnd++)
            {
                UIElement element = children[lineEnd];
                OrientedSize elementSize = new OrientedSize(o,
                    hasFixedWidth ? itemWidth : element.DesiredSize.Width,
                    hasFixedHeight ? itemHeight : element.DesiredSize.Height);
                if (IsGreaterThan(lineSize.Direct + elementSize.Direct, maximumSize.Direct))
                {
                    ArrangeLine(lineStart, lineEnd, directDelta, indirectOffset, lineSize.Indirect);
                    indirectOffset += lineSize.Indirect;
                    lineSize = elementSize;
                    if (IsGreaterThan(elementSize.Direct, maximumSize.Direct))
                    {
                        ArrangeLine(lineEnd, ++lineEnd, directDelta, indirectOffset, elementSize.Indirect);
                        indirectOffset += lineSize.Indirect;
                        lineSize = new OrientedSize(o);
                    }
                    lineStart = lineEnd;
                }
                else
                {
                    lineSize.Direct += elementSize.Direct;
                    lineSize.Indirect = Math.Max(lineSize.Indirect, elementSize.Indirect);
                }
            }
            if (lineStart < count)
            {
                ArrangeLine(lineStart, count, directDelta, indirectOffset, lineSize.Indirect);
            }
            return finalSize;
        }

        private void ArrangeLine(int lineStart, int lineEnd,
            double? directDelta, double indirectOffset, double indirectGrowth)
        {
            double directOffset = 0.0;
            Orientation o = Orientation;
            bool isHorizontal = o == Orientation.Horizontal;
            UIElementCollection children = Children;
            for (int index = lineStart; index < lineEnd; index++)
            {
                UIElement element = children[index];
                OrientedSize elementSize = new OrientedSize(o, element.DesiredSize.Width, element.DesiredSize.Height);
                double directGrowth = directDelta != null ? directDelta.Value : elementSize.Direct;
                Rect bounds = isHorizontal ?
                    new Rect(directOffset, indirectOffset, directGrowth, indirectGrowth) :
                    new Rect(indirectOffset, directOffset, indirectGrowth, directGrowth);
                element.Arrange(bounds);
                directOffset += directGrowth;
            }
        }
    }
    // -----------------------------------------------------------------------
    #endregion



}

#nullable enable

namespace MinimapAlert
{
    using System;
    using System.Collections.ObjectModel;
    using System.Timers;
    using System.Windows;
    using System.Windows.Controls;

    public partial class MainWindow : Window
    {
        public ObservableCollection<LogEntry> LogEntries { get; set; }

        private INodeFinder bobberFinder;
        private IPixelClassifier pixelClassifier;

        private NodeBot? bot;
        private Timer WindowSizeChangedTimer;
        private System.Threading.Thread? botThread;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = LogEntries = new ObservableCollection<LogEntry>();
            this.pixelClassifier = new PixelClassifier();

            this.bobberFinder = new SearchForNode(pixelClassifier);

            var imageProvider = bobberFinder as IImageProvider;
            if (imageProvider != null)
            {
                imageProvider.NodeEvent += ImageProvider_BitmapEvent;
            }

            this.WindowSizeChangedTimer = new Timer { AutoReset = false, Interval = 100 };
            this.Closing += (s, e) => botThread?.Abort();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Reset the timer so it only fires 100ms after the user stop dragging the window.
            WindowSizeChangedTimer.Stop();
            WindowSizeChangedTimer.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e) => bot?.Stop();

        private void Settings_Click(object sender, RoutedEventArgs e) => new ColourConfiguration(this.pixelClassifier).Show();

        private void SetImageVisibility(Image imageForVisible, Image imageForCollapsed, bool state)
        {
            imageForVisible.Visibility = state ? Visibility.Visible : Visibility.Collapsed;
            imageForCollapsed.Visibility = !state ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetButtonStates(bool isBotRunning)
        {
            Dispatch(() =>
            {
                this.Play.IsEnabled = isBotRunning;
                this.Stop.IsEnabled = !this.Play.IsEnabled;
                SetImageVisibility(this.PlayImage, this.PlayImage_Disabled, this.Play.IsEnabled);
                SetImageVisibility(this.StopImage, this.StopImage_Disabled, this.Stop.IsEnabled);
            });
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (bot == null)
            {
                SetButtonStates(false);
                botThread = new System.Threading.Thread(new System.Threading.ThreadStart(this.BotThread));
                botThread.Start();
            }
        }

        public void BotThread()
        {
            bot = new NodeBot(bobberFinder);
            bot.Start();

            bot = null;
            SetButtonStates(true);
        }

        private void ImageProvider_BitmapEvent(object sender, NodeEventArgs e)
        {
            var nodeFound = e.Point != null && e.Point.X > 0 && e.Point.Y > 0;

            Dispatch(() =>
            {
                using (var g = System.Drawing.Graphics.FromImage(e.Bitmap))
                {
                    using (var arialFont = new System.Drawing.Font("Arial", 10))
                    {
                        if (nodeFound)
                        {
                            g.DrawString(DateTime.Now.ToString("HH:mm:ss") + " Node found !", arialFont, System.Drawing.Brushes.Green, new System.Drawing.Point(5, 5)); // requires font, brush etc
                        }
                        else
                        {
                            g.DrawString(DateTime.Now.ToString("HH:mm:ss") + " Searching...", arialFont, System.Drawing.Brushes.Red, new System.Drawing.Point(5, 5)); // requires font, brush etc
                        }
                    }
                }

                var bitmapImage = e.Bitmap.ToBitmapImage();
                e.Bitmap.Dispose();
                this.Screenshot.Source = bitmapImage;
            });

            if (nodeFound)
            {
                Alert();
            }
        }

        public void Alert()
        {
            using (var audioFile = new NAudio.Wave.AudioFileReader(@"./Bensound-moose.wav"))
            using (var outputDevice = new NAudio.Wave.WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private void Dispatch(Action action)
        {
            Application.Current?.Dispatcher.BeginInvoke((Action)(() => action()));
            Application.Current?.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MinimapAlert
{
    public partial class ColourConfiguration : System.Windows.Window, INotifyPropertyChanged
    {
        private readonly IPixelClassifier pixelClassifier;

        private Bitmap ScreenCapture = new Bitmap(1, 1);

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public int YellowValue { get; set; }

        public int ScreenX
        {
            get
            {
                return WowScreen.X;
            }
            set
            {
                if (value > Screen.PrimaryScreen.Bounds.Width)
                {
                    value = Screen.PrimaryScreen.Bounds.Width;
                }

                WowScreen.X = value;
                Capture();
                OnPropertyChanged("ScreenX");
            }
        }

        public int ScreenY
        {
            get
            {
                return WowScreen.Y;
            }
            set
            {
                if (value > Screen.PrimaryScreen.Bounds.Height)
                {
                    value = Screen.PrimaryScreen.Bounds.Height;
                }

                WowScreen.Y = value;
                Capture();
                OnPropertyChanged("ScreenY");
            }
        }

        public int ScreenWidth
        {
            get
            {
                return WowScreen.Width;
            }
            set
            {
                if (value > Screen.PrimaryScreen.Bounds.Width)
                {
                    value = Screen.PrimaryScreen.Bounds.Width;
                }
                WowScreen.Width = value;
                Capture();
                OnPropertyChanged("ScreenWidth");
            }
        }

        public int ScreenHeight
        {
            get
            {
                return WowScreen.Height;
            }
            set
            {
                if (value > Screen.PrimaryScreen.Bounds.Height)
                {
                    value = Screen.PrimaryScreen.Bounds.Height;
                }
                WowScreen.Height = value;
                Capture();
                OnPropertyChanged("ScreenHeight");
            }
        }

        public int MaxBlue
        {
            get
            {
                return pixelClassifier.MaxBlue;
            }
            set
            {
                pixelClassifier.MaxBlue = value;
            }
        }

        public int MinRedGreen
        {
            get
            {
                return pixelClassifier.MinRedGreen;
            }
            set
            {
                pixelClassifier.MinRedGreen = value;
            }
        }

        public ColourConfiguration(IPixelClassifier pixelClassifier)
        {
            this.PropertyChanged += (s, e) => { };
            this.pixelClassifier = pixelClassifier;
            YellowValue = 100;

            InitializeComponent();

            this.DataContext = this;
        }

        private void RenderColour(bool renderMatchedArea)
        {
            var bitmap = new System.Drawing.Bitmap(256, 256);

            var points = new List<Point>();

            for (var b = 0; b < 256; b++)
            {
                for (var g = 0; g < 256; g++)
                {
                    if (pixelClassifier.IsMatch((byte)this.YellowValue, (byte)g, (byte)b))
                    {
                        points.Add(new Point(b, g));
                    }
                    bitmap.SetPixel(b, g, System.Drawing.Color.FromArgb(this.YellowValue, g, b));
                }
            }

            if (ScreenCapture == null)
            {
                ScreenCapture = WowScreen.GetCroppedBitmap(false);
                renderMatchedArea = true;
            }

            this.WowScreenshot.Source = ScreenCapture.ToBitmapImage();

            if (renderMatchedArea)
            {
                Dispatch(() =>
                {
                    Bitmap bmp = new Bitmap(ScreenCapture);
                    MarkYellowOnBitmap(bmp);
                    this.WowScreenshot.Source = bmp.ToBitmapImage();
                });
            }
        }

        private void MarkYellowOnBitmap(Bitmap bmp)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    if (this.pixelClassifier.IsMatch(pixel.R, pixel.G, pixel.B))
                    {
                        bmp.SetPixel(x, y, Color.Red);
                    }
                }
            }
        }

        //high red and green, low blue

        private static void MarkEdgeOfRedArea(Bitmap bitmap, List<Point> points)
        {
            foreach (var point in points)
            {
                //var pointsClose = points.Count(p => (p.X == point.X && (p.Y == point.Y - 1 || p.Y == point.Y + 1)) || (p.Y == point.Y && (p.X == point.X - 1 || p.X == point.X + 1)));
                //if (pointsClose < 4)
                //{
                bitmap.SetPixel(point.X, point.Y, Color.White);
                //}
            }
        }

        private void MaxBlue_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            this.MaxBlueText.Text = $"Blue must be less than {this.pixelClassifier.MaxBlue}.";
        }

        private void MinRedGreen_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            this.MinRedGreenText.Text = $"Red and Green must be more than: {this.pixelClassifier.MinRedGreen}";
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            RenderColour(true);
        }

        private void Capture_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Capture();
        }

        private void Capture()
        {
            ScreenCapture = WowScreen.GetCroppedBitmap(false);
            RenderColour(true);
        }

        private void Dispatch(Action action)
        {
            System.Windows.Application.Current?.Dispatcher.BeginInvoke((Action)(() => action()));
            System.Windows.Application.Current?.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
        }

        private void XMinus_Click(object sender, System.Windows.RoutedEventArgs e) => this.ScreenX--;

        private void XPlus_Click(object sender, System.Windows.RoutedEventArgs e) => this.ScreenX++;

        private void YMinus_Click(object sender, System.Windows.RoutedEventArgs e) => this.ScreenY--;

        private void YPlus_Click(object sender, System.Windows.RoutedEventArgs e) => this.ScreenY++;

        private void WidthMinus_Click(object sender, System.Windows.RoutedEventArgs e) => this.ScreenWidth--;

        private void WidthPlus_Click(object sender, System.Windows.RoutedEventArgs e) => this.ScreenWidth++;

        private void HeightMinus_Click(object sender, System.Windows.RoutedEventArgs e) => this.ScreenHeight--;

        private void HeightPlus_Click(object sender, System.Windows.RoutedEventArgs e) => this.ScreenHeight++;
    }
}
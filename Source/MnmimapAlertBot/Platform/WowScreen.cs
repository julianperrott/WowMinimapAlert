using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MinimapAlert
{
    public static class WowScreen
    {
        public static int X { get; set; }
        public static int Y { get; set; }
        public static int Width { get; set; }
        public static int Height { get; set; }

        static WowScreen()
        {
            Width = 155;
            Height = 155;
            X = 1730;
            Y = 38;
        }

        private static Bitmap CropImage(Bitmap img, bool highlight)
        {
            int x = img.Width / 2;
            int y = img.Height / 2;
            int r = Math.Min(x, y);

            var tmp = new Bitmap(2 * r, 2 * r);
            using (Graphics g = Graphics.FromImage(tmp))
            {
                if (highlight)
                {
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 0, 0)))
                    {
                        g.FillRectangle(brush, 0, 0, img.Width, img.Height);
                    }
                }

                g.SmoothingMode = SmoothingMode.None;
                g.TranslateTransform(tmp.Width / 2, tmp.Height / 2);
                var gp = new GraphicsPath();
                gp.AddEllipse(0 - r, 0 - r, 2 * r, 2 * r);
                g.SetClip(new Region(gp), CombineMode.Replace);
                var bmp = new Bitmap(img);

                g.DrawImage(bmp, new Rectangle(-r, -r, 2 * r, 2 * r), new Rectangle(x - r, y - r, 2 * r, 2 * r), GraphicsUnit.Pixel);
            }
            return tmp;
        }

        public static Color GetColorAt(Point pos, Bitmap bmp)
        {
            return bmp.GetPixel(pos.X, pos.Y);
        }

        public static Bitmap GetCroppedBitmap(bool highlight)
        {
            return CropImage(GetBitmap(), highlight);
        }

        private static Bitmap GetBitmap()
        {
            var bmpScreen = new Bitmap(Width, Height);
            var graphics = Graphics.FromImage(bmpScreen);
            graphics.CopyFromScreen(X, Y, 0, 0, bmpScreen.Size);
            graphics.Dispose();
            return bmpScreen;
        }

        public static Point GetScreenPositionFromBitmapPostion(Point pos)
        {
            return new Point(pos.X += Screen.PrimaryScreen.Bounds.Width / 4, pos.Y += Screen.PrimaryScreen.Bounds.Height / 4);
        }
    }
}
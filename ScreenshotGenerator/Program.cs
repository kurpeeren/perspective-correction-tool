using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

public class ScreenshotGenerator
{
    public static void Main()
    {
        int width = 800;
        int height = 600;

        using (var bitmap = new Bitmap(width, height))
        using (var g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Background
            g.Clear(Color.FromArgb(240, 240, 240));

            // Window
            var windowRect = new Rectangle(10, 10, width - 20, height - 20);
            g.FillRectangle(Brushes.White, windowRect);
            g.DrawRectangle(Pens.Gray, windowRect);

            // Title bar
            var titleBarRect = new Rectangle(11, 11, width - 22, 30);
            g.FillRectangle(Brushes.LightGray, titleBarRect);
            g.DrawString("Perspective Correction Tool", new Font("Segoe UI", 10), Brushes.Black, 15, 18);

            // Video area
            var videoRect = new Rectangle(25, 50, 750, 450);
            g.FillRectangle(Brushes.DarkGray, videoRect);
            g.DrawString("Camera Feed Placeholder", new Font("Segoe UI", 16), Brushes.White, 250, 250);

            // Draw a sample quadrilateral
            Point p1 = new Point(100, 100);
            Point p2 = new Point(600, 120);
            Point p3 = new Point(580, 400);
            Point p4 = new Point(120, 380);
            g.DrawPolygon(new Pen(Color.LightGreen, 3), new Point[] { p1, p2, p3, p4 });


            // Control panel
            var panelRect = new Rectangle(25, 510, 750, 70);
            g.FillRectangle(Brushes.WhiteSmoke, panelRect);
            g.DrawRectangle(Pens.LightGray, panelRect);

            // Controls
            g.DrawString("Camera:", new Font("Segoe UI", 9), Brushes.Black, 35, 535);
            g.FillRectangle(Brushes.White, 100, 530, 150, 25);
            g.DrawRectangle(Pens.Gray, 100, 530, 150, 25);
            g.DrawString("Default Camera", new Font("Segoe UI", 9), Brushes.Black, 105, 535);

            g.FillRectangle(Brushes.LightGray, 260, 530, 100, 25);
            g.DrawRectangle(Pens.Gray, 260, 530, 100, 25);
            g.DrawString("Start", new Font("Segoe UI", 9), Brushes.Black, 295, 535);

            g.FillRectangle(Brushes.LightGray, 370, 530, 150, 25);
            g.DrawRectangle(Pens.Gray, 370, 530, 150, 25);
            g.DrawString("Apply Perspective", new Font("Segoe UI", 9), Brushes.Black, 390, 535);


            bitmap.Save("screenshot.png", ImageFormat.Png);
            Console.WriteLine("Screenshot saved to screenshot.png");
        }
    }
}

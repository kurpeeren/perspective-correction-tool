using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Collections;
using System.Threading;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using AForge.Imaging.Filters;

namespace img_rect
{
    public partial class mainform : Form
    {
        public mainform()
        {
            InitializeComponent();
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            this.MinimumSize = new Size(600, 400);
            foreach (FilterInfo filterInfo in filterInfoCollection)
                cboDevice.Items.Add(filterInfo.Name);
            cboDevice.SelectedIndex = 0;
        }


        #region Değişkenler


        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;
        ArrayList arlist1 = new ArrayList();

        Bitmap line_layer;
        Bitmap bg_layer;
        Bitmap image;

        Bitmap newImage;
        Bitmap newImage1, newImage2, newImage4;
        int sayi = 0, click = 0;
        int a = 100;
        Thread t1, t2, t3, t4, t5, t6;
        Point point1, point2, point3, point4, point5;
        ResizeBilinear filter;
        Point locationss, Click_latestPoint, latestPoint, oneclick;
        //List<Point> corners = new List<Point>();
        List<AForge.IntPoint> corners = new List<AForge.IntPoint>();

        



        #endregion


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void sort2d(List<AForge.IntPoint> asd) {
            List<AForge.IntPoint> result = new List<AForge.IntPoint>
                { (new AForge.IntPoint(0,0)),(new AForge.IntPoint(0,0)),(new AForge.IntPoint(0,0)),(new AForge.IntPoint(0,0))};
            if (4 == asd.Count())
            {
                double mesafe, oldmesafe = pictureBox1.Width ;
                for(int i = 0; i < 4; i++)
                {    mesafe = Math.Sqrt(Math.Pow((0 - asd[i].Y), 2) + Math.Pow((0 - asd[i].X), 2));
                    if (mesafe < oldmesafe) { result[0] = asd[i]; oldmesafe = mesafe; }
                    }
                mesafe = 0; oldmesafe = pictureBox1.Width;
                for (int i = 0; i < 4; i++)
                {
                    mesafe = Math.Sqrt(Math.Pow((0 - asd[i].Y), 2) + Math.Pow((0 - asd[i].X), 2));
                    if (mesafe < oldmesafe) { result[1] = asd[i]; oldmesafe = mesafe; }
                }
                for (int i = 0; i < 4; i++)
                {
                    mesafe = Math.Sqrt(Math.Pow((0 - asd[i].Y), 2) + Math.Pow((0 - asd[i].X), 2));
                    if (mesafe < oldmesafe) { result[2] = asd[i]; oldmesafe = mesafe; }
                }
                for (int i = 0; i < 4; i++)
                {
                    mesafe = Math.Sqrt(Math.Pow((0 - asd[i].Y), 2) + Math.Pow((0 - asd[i].X), 2));
                    if (mesafe < oldmesafe) { result[3] = asd[i]; oldmesafe = mesafe; }
                }
            }
        }


        void persfek()
        {
            QuadrilateralTransformation filter =
            new QuadrilateralTransformation(corners);
            // apply the filter
            newImage4 = filter.Apply(newImage1);


        }

        private void btn_filt_Click(object sender, EventArgs e)
        {
            bit = false;
        }


        private void ProcessUsingLockbitsAndUnsafeAndParallel(Bitmap processedBitmap)
        {
            unsafe
            {
                BitmapData bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, processedBitmap.Width, processedBitmap.Height), ImageLockMode.ReadWrite, processedBitmap.PixelFormat);
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(processedBitmap.PixelFormat) / 8;
                int heightInPixels = bitmapData.Height;
                int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                Parallel.For(0, heightInPixels, y =>
                {
                    byte* currentLine = PtrFirstPixel + (y * bitmapData.Stride);
                    for (int x = 0; x < widthInBytes; x = x + bytesPerPixel)
                    {
                        int oldBlue = currentLine[x];
                        int oldGreen = currentLine[x + 1];
                        int oldRed = currentLine[x + 2];

                        currentLine[x] = (byte)oldBlue;
                        currentLine[x + 1] = (byte)oldGreen;
                        currentLine[x + 2] = (byte)oldRed;
                    }
                });
                processedBitmap.UnlockBits(bitmapData);
            }
        }


        private void btnStart_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ') point1 = point2 = point3 = point4 = point5 = Point.Empty; click = sayi = 0;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!(captureDevice == null))
            {
                if (captureDevice.IsRunning.Equals(true))
                {
                    captureDevice.Stop();
                }
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();

        }


        private Bitmap MergedBitmaps(Bitmap bmp1, Bitmap bmp2, float x = 0, float y = 0)
        {

            if (!(bmp1 == null || bmp2 == null))
            {
                Bitmap result = new Bitmap(Math.Max(bmp1.Width, bmp2.Width),
                                           Math.Max(bmp1.Height, bmp2.Height));
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.DrawImage(bmp2, Point.Empty);
                    g.DrawImage(bmp1, x, y);
                }

                return result;
            }
            else
            {
                return bmp2;
            }



        }

        private void layer_marger_task()
        {
            if (image != null)
            {
                #region //Arka Plan Hesabı
                int t = (image.Width - (image.Width / 100) * a);
                int s = (image.Height - (image.Height) / 100 * a);
                bg_layer = new Bitmap(image.Width + t, image.Height + s);
                using (Graphics g = Graphics.FromImage(bg_layer))
                    g.Clear(Color.Black);
                #endregion

                newImage = MergedBitmaps(image, bg_layer, t / 2, s / 2);
                t6 = new Thread(layer_draw_task);
                t6.Start();
                t6.Join();
                t5 = new Thread(filter_task);
                t5.Start();
                t5.Join();
                if (line_layer != null) newImage2 = MergedBitmaps(line_layer, newImage1);
                else { newImage2 = newImage1; }
            }
        }
        int x, y;
        private void filter_task()
        {
            filter = new ResizeBilinear(pictureBox1.Size.Width, pictureBox1.Size.Height);
            if (newImage != null)
            {
                newImage1 = filter.Apply(newImage);
                x = (pictureBox1.Width / newImage1.Width);
                y = (pictureBox1.Height / newImage1.Height);
            }

        }

        bool z1, z2, z3, z4, z5 = true;        
        private void layer_draw_task()
  

      {
            try
            {

                if ((newImage1 != null)) line_layer = new Bitmap(newImage1.Width, newImage1.Height);
                if ((line_layer != null))
                {

                    if (true)
                    {
                        using (var g = Graphics.FromImage(line_layer))
                      {
                            var pen = new Pen(Color.LightGreen,/*thickness = */ 3);
                            if ((oneclick != Point.Empty) && sayi < 4) g.DrawLine(pen, oneclick, locationss);
                            if ((click == 1) && (sayi == 1))
                            {
                                point1 = oneclick;
                                corners.Add(new AForge.IntPoint(point1.X, point1.Y));
                                click = 0;

                            }
                            if ((click == 1) && (sayi == 2))
                            {
                                point2 = locationss;
                                corners.Add(new AForge.IntPoint(point2.X, point2.Y));
                                click = 0;
                                oneclick = point2;
                            }
                            if ((click == 1) && (sayi == 3))
                            {
                                point3 = locationss;
                                corners.Add(new AForge.IntPoint(point3.X, point3.Y));
                                click = 0;
                                oneclick = point3;
                            }
                            if ((click == 1) && (sayi == 4))
                            {
                                point4 = locationss;
                                corners.Add(new AForge.IntPoint(point4.X, point4.Y));
                                click = 0;
                                oneclick = point4;
                            }
                            if ((click == 1) && (sayi == 5))
                            {
                                point5 = locationss;
                                click = 0;
                                oneclick = point5;
                            }




  
                            if (!(point1 == Point.Empty) && !(point2 == Point.Empty)) g.DrawLine(pen, point1, point2);


                      if (!(point2 == Point.Empty) && !(point3 == Point.Empty)) g.DrawLine(pen, point2, point3);
                            if (!(point3 == Point.Empty) && !(point4 == Point.Empty)) g.DrawLine(pen, point3, point4);
                            if (!(point4 == Point.Empty) && !(point5 == Point.Empty)) g.DrawLine(pen, point4, point1);      //    //g.DrawRectangle(pen, x1, y1, 399, 200);
                            pen.Dispose();
                            g.Dispose();

                        }
                    }

                }
            }
            catch { }
        }
        bool bit = true;

        

        private void CaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            t4 = new Thread(layer_marger_task);
            t4.Start();
            image = (Bitmap)
            eventArgs.Frame.Clone();
            t4.Join();
      

      if (bit)
            {
                pictureBox1.Image = newImage2;
            }
            else
            {
                t1 = new Thread(persfek);
                t1.Start();
                pictureBox1.Image = newImage4;
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();


        }









        private void btnStart_Click(object sender, EventArgs e)
        {
            #region Webcam Bağlantı


            if ((captureDevice == null))
            {
                captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString);
                captureDevice.NewFrame += CaptureDevice_NewFrame;
            }
            Thread t3 = new Thread(captureDevice.Start);
            t3.Start();
            Thread.Sleep(100);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            #endregion

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Click_latestPoint = latestPoint;
            oneclick = locationss;
            sayi++;
            click = 1;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (!(captureDevice == null || pictureBox1.Image == null))
            {
                label2.Text = e.Location.ToString();
                locationss.X = e.X * x; //);//*pictureBox1.Width*);
                locationss.Y = e.Y * y;//*pictureBox1.Height);
                try
                {
                    Bitmap b = new Bitmap(pictureBox1.Image);
                    Color colour = b.GetPixel(e.X, e.Y);
                    label3.Text = colour.ToString();
                }
                catch { }
            }



            //if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            //{
            //    using (Graphics g = Graphics.FromImage(line_layer))
            //    {
            //        // Draw next line and...
            //        Pen blackPen = new Pen(Color.Black, 3);
            //        g.DrawLine(blackPen, latestPoint, e.Location);

            //        // ... Remember the location
            //        latestPoint = e.Location;
            //    }
            //}
            //    Bitmap bmp = new Bitmap(pictureBox1.Image);

            //Rectangle asd = new Rectangle(0, 0, bmp.Width, bmp.Height);
            //bd = (RunImgProcess.detectionImg).LockBits(new Rectangle(0, 0, RunImgProcess.detectionImg.Width, RunImgProcess.detectionImg.Height), ImageLockMode.ReadOnly, RunImgProcess.detectionImg.PixelFormat);


            //unsafe
            //{
            //    byte* ptr = (byte*)bd.Scan0;
            //    int x = e.X * 3;
            //    int y = e.Y * bd.Stride;
            //    label2.Text = e.Location.ToString();
            //    label3.Text = ptr[x + y].ToString() + "," + ptr[x + y + 1].ToString() + "," + ptr[x + y + 2].ToString();
            //    GC.Collect();
            //    GC.WaitForPendingFinalizers();
            //}

            //(RunImgProcess.detectionImg).UnlockBits(bd);
        }
        private void GainBox_MouseDown(object sender, MouseEventArgs e)
        {
            //if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            //{
            //    // Remember the location where the button was pressed
            //    latestPoint = e.Location;
            //}
            ////pictureBox1.refresh();
        }
        private void MyMouseWheel(object sender, MouseEventArgs e)
        {
            a += e.Delta / 24;
            if (a > 100) a = 100;
            label1.Text = a.ToString();

        }
    }
}

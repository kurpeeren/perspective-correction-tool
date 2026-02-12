using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;

namespace PerspectiveCorrectionTool
{
    public partial class MainForm : Form
    {
        private FilterInfoCollection _videoDevices;
        private VideoCaptureDevice _videoSource;

        private readonly List<AForge.IntPoint> _sourceCorners = new List<AForge.IntPoint>();
        private Bitmap _sourceImage;
        private bool _showTransformed = false;
        private int _zoomPercentage = 100;

        // Used for drawing the selection rectangle and lines
        private Point _lastClickPoint;
        private Point _currentMouseLocation;
        private int _clickCount = 0;
        private bool _isClicked = false;


        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (_videoDevices.Count == 0)
            {
                MessageBox.Show("No video devices found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (FilterInfo device in _videoDevices)
            {
                cboDevice.Items.Add(device.Name);
            }
            cboDevice.SelectedIndex = 0;

            // Palette UX: Initial state guidance
            btnFilter.Text = "Select Point 1/4";
            btnFilter.Enabled = false;
            pictureBox1.Cursor = Cursors.Default;
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.WaitForStop();
                _videoSource = null;
                btnStart.Text = "Start";
                pictureBox1.Cursor = Cursors.Default;
            }
            else
            {
                _videoSource = new VideoCaptureDevice(_videoDevices[cboDevice.SelectedIndex].MonikerString);
                _videoSource.NewFrame += VideoSource_NewFrame;
                _videoSource.Start();
                btnStart.Text = "Stop";
                pictureBox1.Cursor = Cursors.Cross;
            }
        }

        private async void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Clone the frame to avoid issues with the source being disposed
            _sourceImage = (Bitmap)eventArgs.Frame.Clone();

            // Process the frame in the background
            Bitmap processedFrame = await Task.Run(() => ProcessFrame(_sourceImage));

            // Update the PictureBox on the UI thread
            pictureBox1.Image = processedFrame;
        }

        private Bitmap ProcessFrame(Bitmap source)
        {
            Bitmap resizedBitmap = new ResizeBilinear(pictureBox1.Width, pictureBox1.Height).Apply(source);

            if (_showTransformed && _sourceCorners.Count == 4)
            {
                try
                {
                    // Create the perspective transformation filter
                    QuadrilateralTransformation perspectiveFilter = new QuadrilateralTransformation(_sourceCorners, pictureBox1.Width, pictureBox1.Height);
                    return perspectiveFilter.Apply(source);
                }
                catch (Exception ex)
                {
                    // Log the exception, perhaps show an error to the user
                    Console.WriteLine($"Error during perspective transformation: {ex.Message}");
                    // Return the resized bitmap if transformation fails
                    return DrawOverlay(resizedBitmap);
                }
            }
            else
            {
                // Draw the selection overlay on the resized image
                return DrawOverlay(resizedBitmap);
            }
        }

        private Bitmap DrawOverlay(Bitmap baseImage)
        {
            // Clone the image to draw on it
            Bitmap overlayBitmap = (Bitmap)baseImage.Clone();

            using (Graphics g = Graphics.FromImage(overlayBitmap))
            {
                var pen = new Pen(Color.LightGreen, 2);

                // Draw existing lines between selected corners
                for (int i = 0; i < _sourceCorners.Count; i++)
                {
                    var p1 = new Point(_sourceCorners[i].X, _sourceCorners[i].Y);
                    var p2 = new Point(_sourceCorners[(i + 1) % _sourceCorners.Count].X, _sourceCorners[(i + 1) % _sourceCorners.Count].Y);
                    if (_sourceCorners.Count > 1 && i < _sourceCorners.Count -1)
                    {
                        g.DrawLine(pen, p1, p2);
                    }
                }

                // If we have 4 corners, close the shape
                if (_sourceCorners.Count == 4)
                {
                     g.DrawLine(pen, new Point(_sourceCorners[3].X, _sourceCorners[3].Y), new Point(_sourceCorners[0].X, _sourceCorners[0].Y));
                }


                // Draw a line from the last point to the current mouse position
                if (_sourceCorners.Count > 0 && _sourceCorners.Count < 4)
                {
                    g.DrawLine(pen, new Point(_sourceCorners.Last().X, _sourceCorners.Last().Y), _currentMouseLocation);
                }

                pen.Dispose();
            }

            return overlayBitmap;
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (_clickCount < 4)
            {
                // Scale the click point to the original image size if needed,
                // but for now, we assume the display and processing size are the same.
                _sourceCorners.Add(new AForge.IntPoint(e.Location.X, e.Location.Y));
                _clickCount++;

                // Palette UX: Update button text to guide user
                if (_clickCount < 4)
                {
                    btnFilter.Text = $"Select Point {_clickCount + 1}/4";
                    btnFilter.Enabled = false;
                }
                else
                {
                    btnFilter.Text = "Apply Perspective";
                    btnFilter.Enabled = true;
                }
            }
            else
            {
                // Reset selection
                _sourceCorners.Clear();
                _clickCount = 0;
                _showTransformed = false;

                // Palette UX: Reset guidance
                btnFilter.Text = "Select Point 1/4";
                btnFilter.Enabled = false;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            _currentMouseLocation = e.Location;
            // Update labels if needed
            label2.Text = e.Location.ToString();

            if (pictureBox1.Image != null)
            {
                try
                {
                    Bitmap b = new Bitmap(pictureBox1.Image);
                    Color colour = b.GetPixel(e.X, e.Y);
                    label3.Text = colour.ToString();
                    b.Dispose();
                }
                catch (Exception)
                {
                    // Ignore errors from getting pixel color (e.g. out of bounds)
                }
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            if (_sourceCorners.Count == 4)
            {
                _showTransformed = !_showTransformed;
                btnFilter.Text = _showTransformed ? "Show Original" : "Apply Perspective";
            }
            else
            {
                MessageBox.Show("Please select 4 points on the image first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.WaitForStop();
            }
        }

        // This is a rough implementation of sorting points.
        // A more robust solution should be used for a real application.
        private List<AForge.IntPoint> SortCorners(List<AForge.IntPoint> corners)
        {
            if (corners.Count != 4)
                return corners;

            // Sort by Y, then X
            var sorted = corners.OrderBy(p => p.Y).ThenBy(p => p.X).ToList();

            var topLeft = sorted[0];
            var topRight = sorted[1];
            if (topLeft.X > topRight.X)
            {
                var temp = topLeft;
                topLeft = topRight;
                topRight = temp;
            }

            var bottomLeft = sorted[2];
            var bottomRight = sorted[3];
            if (bottomLeft.X > bottomRight.X)
            {
                var temp = bottomLeft;
                bottomLeft = bottomRight;
                bottomRight = temp;
            }

            return new List<AForge.IntPoint> { topLeft, topRight, bottomRight, bottomLeft };
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using DlibDotNet;
using DlibDotNet.Extensions;
using Emgu.CV;
using GitHub.secile.Video;

namespace veBot_Operator.BotParts
{
    class FaceExpression
    {
        private System.Timers.Timer timer;
        private UsbCamera camera;
        private System.Windows.Controls.Image imgctrl;
        private FrontalFaceDetector fd;
        private ShapePredictor sp;
        public FaceExpression(System.Windows.Controls.Image img)
        {
            this.imgctrl = img;
            fd = Dlib.GetFrontalFaceDetector();
            sp = ShapePredictor.Deserialize("shape_predictor_68_face_landmarks.dat");
            
            }
        public void start_cam_stream()
        {
            string[] devices = UsbCamera.FindDevices();
            if (devices.Length == 0) Console.WriteLine("no camera");

            int cameraIndex = 0;
            UsbCamera.VideoFormat[] formats = UsbCamera.GetVideoFormat(cameraIndex);

            camera = new UsbCamera(cameraIndex, formats[0]);
            camera.Start();

      


            timer = new System.Timers.Timer(70);
            //timer.Elapsed += (s, ev) => pictureBox1.Image = camera.GetBitmap();
            timer.Elapsed += (s, ev) => detectfaces(camera.GetBitmap());

            timer.Start();




        }

        public void stop_camera()
        {
            timer.Stop();
            camera.Stop();
        }

        public void detectfaces(System.Drawing.Bitmap bmp)
        {
            
                var img = DlibDotNet.Extensions.BitmapExtensions.ToArray2D<RgbPixel>(bmp);

                // find all faces in the image
                var faces = fd.Operator(img);
                foreach (var face in faces)
                {
                
                    // find the landmark points for this face
                    var shape = sp.Detect(img, face);
                                     // draw the landmark points on the image
                    for (var i = 0; i < shape.Parts; i++)
                    {
                        var point = shape.GetPart((uint)i);
                        var rect = new Rectangle(point);
                        Dlib.DrawRectangle(img, rect, color: new RgbPixel(255, 255, 0), thickness: 20);
                    }
                }
                BitmapImage b = ToBitmapImage(BitmapExtensions.ToBitmap<RgbPixel>(img));
                this.imgctrl.Dispatcher.BeginInvoke(DispatcherPriority.Input, new ThreadStart(() =>
                {
                    this.imgctrl.Source = b;
            }));
            
        }
        public static BitmapImage ToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }
}

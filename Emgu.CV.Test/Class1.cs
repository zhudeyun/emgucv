using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.UI;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;

namespace Emgu.CV.Test
{
    [TestFixture]
    public class Tester
    {
        public void TestRotationMatrix2D()
        {
            RotationMatrix2D mat = new RotationMatrix2D(new Point2D<float>(1, 2), 30, 1);
            //Trace.WriteLine(Emgu.Utils.MatrixToString<float>(mat.Data, ", ", ";\r\n"));
        }

        public void GenerateLogo()
        {
            Image<Bgr, Byte> semgu = new Image<Bgr, byte>(160, 72, new Bgr(0, 0, 0));
            Image<Bgr, Byte> scv = new Image<Bgr, byte>(160, 72, new Bgr(0, 0, 0));
            MCvFont f1 = new MCvFont(CvEnum.FONT.CV_FONT_HERSHEY_TRIPLEX, 1.5, 1.5);
            MCvFont f2 = new MCvFont(CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX, 1.6, 2.2);
            semgu.Draw("Emgu", ref f1, new Point2D<int>(6, 50), new Bgr(55, 155, 255));
            semgu._Dilate(1);
            scv.Draw("CV", ref f2, new Point2D<int>(50, 60), new Bgr(255, 55, 255));
            scv._Dilate(2);
            Image<Bgr, Byte> logoBgr = semgu.Or(scv);
            Image<Gray, Byte> logoA = new Image<Gray, byte>(logoBgr.Width, logoBgr.Height);
            logoA.SetValue(255, logoBgr.Convert<Gray, Byte>());
            logoBgr._Not();
            logoA._Not();
            Image<Gray, Byte>[] channels = logoBgr.Split();
            channels = new Image<Gray,byte>[] { channels[0], channels[1], channels[2], new Image<Gray, Byte>(channels[0].Width, channels[0].Height, new Gray(255.0))};
            Image<Bgra, Byte> logoBgra = new Image<Bgra, byte>(channels);
            logoBgra.SetValue(new Bgra(0.0, 0.0, 0.0, 0.0), logoA);
            logoBgra.Save("EmguCVLogo.gif");
        }

        public void TestCvNamedWindow()
        {
            String win1 = "Test Window"; //The name of the window
            CvInvoke.cvNamedWindow(win1); //Create the window using the specific name

            using (Image<Bgr, Byte> img = new Image<Bgr, byte>(400, 200, new Bgr(255, 0, 0))) //Create an image of 400x200 of Blue color
            {
                MCvFont f = new MCvFont(CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX, 1.0, 1.0); //Create the font
                img.Draw("Hello, world", ref f, new Point2D<int>(10, 80), new Bgr(0, 255, 0)); //Draw "Hello, world." on the image using the specific font

                CvInvoke.cvShowImage(win1, img.Ptr); //Show the image
                CvInvoke.cvWaitKey(0);  //Wait for the key pressing event
                CvInvoke.cvDestroyWindow(win1); //Destory the window
            }
        }

        public void TestConvert()
        {
            Image<Gray, Single> g = new Image<Gray, Single>(80, 40);
            Image<Gray, Single> g2 = g.Convert<Single>(delegate(Single v, int x, int y) { return System.Convert.ToSingle(Math.Sqrt(0.0 + x * x + y * y)); });
            Application.Run(new ImageViewer(g2));
        }

        public void TestHorizontalLine()
        {
            Point2D<int> p1 = new Point2D<int>(10, 10);
            Point2D<int> p2 = new Point2D<int>(20, 10);
            LineSegment2D<int> l1 = new LineSegment2D<int>(p1, p2);
            Image<Bgr, Byte> img = new Image<Bgr, byte>(200, 400, new Bgr(255, 255, 255));
            img.Draw(l1, new Bgr(0.0, 0.0, 0.0), 1);
            Application.Run(new ImageViewer(img));
        }

        public void TestRectangle()
        {
            Point2D<double> p1 = new Point2D<double>(1.1, 2.2);
            Point2D<double> p2 = new Point2D<double>(2.2, 4.4);
            Rectangle<double> rect = new Rectangle<double>();
            rect.Center = p1;
            rect.Size = p2;

            Map<Gray, Byte> map = new Map<Gray, Byte>(new Rectangle<double>(new Point2D<double>(2.0, 4.0), new Point2D<double>(4.0, 8.0)), new Point2D<double>(0.1, 0.1), new Gray(255.0));
            map.Draw<double>(rect, new Gray(0.0), 1);

            Rectangle<double> roi = map.ROI;
            roi.Height /= 2.0;
            map.ROI = roi;

            Application.Run(new ImageViewer(map));
        }

        public void testEllipseFitting()
        {
            System.Random r = new Random();

            Image<Gray, byte> img = new Image<Gray, byte>(400, 400, new Gray(0.0));
            List<Point2D<int>> pts = new List<Point2D<int>>();
            for (int i = 0; i <= 100; i++)
            {
                int x = r.Next(100) + 20;
                int y = r.Next(300) + 50;
                img[y, x] = new Gray(255.0);
                pts.Add(new Point2D<int>(x, y));
            }

            Ellipse<float> e = PointCollection.LeastSquareEllipseFitting((IEnumerable<Point<int>>)pts.ToArray());

            img.Draw(e, new Gray(120.0), 1);
            Application.Run(new ImageViewer(img));
        }

        /*
        public void TestIpp()
        {
            Trace.WriteLine(String.Format("Ipp Used: {0}", Emgu.CV.Utils.IppUsed()));
        }*/

        /*
        [Test]
        public void TestRandom()
        {
            using (Image<Bgr, byte> img = new Image<Bgr, byte>(50, 50))
            {
                img._RandNormal(0xffffffff, new MCvScalar(0.0, 0.0, 0.0), new MCvScalar(50.0, 50.0, 50.0));
                Application.Run(new ImageViewer(img.ToBitmap()));
            }
        }*/

        public void CameraTest()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new TestCamera());
        }

        public void TestImageLoader()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Image<Bgr, Single> img = new Image<Bgr, Single>("stuff.jpg"))
            using (Image<Bgr, Single> img2 = img.Resize(100, 100, true))
            {
                Application.Run(new ImageViewer(img2));
                Rectangle<double> r = img2.ROI;
                r.Size = new Point2D<double>(r.Size.X / 2, r.Size.Y / 2);
                img2.ROI = r;
                Application.Run(new ImageViewer(img2));
            }
        }

        public void TestBgr()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Image<Bgr, Byte> img = new Image<Bgr, byte>(100, 100, new Bgr(0, 100, 200)))
            {
                Application.Run(new ImageViewer(img));
                Image<Gray, Byte>[] channels = img.Split();
                foreach (Image<Gray, Byte> i in channels)
                    Application.Run(new ImageViewer(i));
            }
        }

        public void TestBgra()
        {
            Image<Bgra, Byte> img = new Image<Bgra, byte>(100, 100);
            img.SetValue(new Bgra(255.0, 120.0, 0.0, 120.0));
            Image<Gray, Byte>[] channels = img.Split();
            foreach (Image<Gray, Byte> i in channels)
                Application.Run(new ImageViewer(i));
            Application.Run(new ImageViewer(img));
        }

        public void TestFont()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Image<Gray, Byte> img = new Image<Gray, Byte>(200, 300, new Gray()))
            {
                MCvFont f = new MCvFont(CvEnum.FONT.CV_FONT_HERSHEY_COMPLEX_SMALL, 1.0, 1.0);
                {
                    img.Draw("h.", ref f, new Point2D<int>(100, 10), new Gray(255.0));
                    img.Draw("a.", ref f, new Point2D<int>(100, 50), new Gray(255.0));
                }
                Application.Run(new ImageViewer(img));
            }
        }

        public void TestHistogram()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Image<Bgr, Byte> img = new Image<Bgr, byte>("stuff.jpg"))
            using (Image<Hsv, Byte> img2 = img.Convert<Hsv, Byte>())
            {
                Image<Gray, Byte>[] HSVs = img2.Split();

                using (Histogram h = new Histogram(new int[1] { 20 }, new float[1] { 0.0f }, new float[1] { 180.0f }))
                {
                    h.Accumulate(new Image<Gray, Byte>[1] { HSVs[0] });
                    using (Image<Gray, Byte> bpj = h.BackProject(new Image<Gray, Byte>[1] { HSVs[0] }))
                        Application.Run(new ImageViewer(bpj));
                }

                foreach (Image<Gray, Byte> i in HSVs) i.Dispose();
            }
        }

        public void TestGoodFeature()
        {
            using (Image<Gray, Byte> img = new Image<Gray, Byte>("stuff.jpg"))
            {
                Point2D<float>[][] pts = img.GoodFeaturesToTrack(100, 0.1, 10, 5, false, 0);
                foreach (Point2D<float> p in pts[0])
                    img.Draw<float>(new Circle<float>(p, 3.0f), new Gray(255.0), 1);
                Application.Run(new ImageViewer(img));
            }
        }

        public void TestSplitMerge()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (Image<Bgr, Byte> img = new Image<Bgr, byte>("stuff.jpg"))
            {
                using (Image<Hsv, Byte> imgHsv = img.Convert<Hsv, Byte>())
                {
                    Image<Gray, Byte>[] imgs = imgHsv.Split();
                    using (Image<Hsv, Byte> imgHsv2 = new Image<Hsv, Byte>(imgs))
                    {
                        using (Image<Bgr, Byte> imageRGB = imgHsv2.Convert<Bgr, Byte>())
                        {
                            LineSegment2D<int>[][] lines = imgHsv2.HughLines(
                                new Hsv(50.0, 50.0, 50.0), new Hsv(200.0, 200.0, 200.0),
                                1, Math.PI / 180.0, 50, 50, 10);

                            Circle<float>[][] circles = img.HughCircles(
                                new Bgr(200.0, 200.0, 200.0), new Bgr(100.0, 100.0, 100.0),
                                4.0, 1.0, 0, 0);

                            for (int i = 0; i < lines[0].Length; i++)
                            {
                                imageRGB.Draw(lines[0][i], new Bgr(255.0, 0.0, 0.0), 1);
                            }

                            for (int i = 0; i < lines[1].Length; i++)
                            {
                                imageRGB.Draw(lines[1][i], new Bgr(0.0, 255.0, 0.0), 1);
                            }

                            for (int i = 0; i < lines[2].Length; i++)
                            {
                                imageRGB.Draw(lines[2][i], new Bgr(0.0, 0.0, 255.0), 1);
                            }

                            foreach (Circle<float>[] cs in circles)
                                foreach (Circle<float> c in cs)
                                    imageRGB.Draw(c, new Bgr(0.0, 0.0, 0.0), 1);

                            Application.Run(new ImageViewer(imageRGB));

                            bool applied = false;
                            foreach (Circle<float>[] cs in circles)
                                foreach (Circle<float> c in cs)
                                {
                                    if (!applied)
                                    {
                                        Circle<float> cir = c;
                                        cir.Radius += 30;
                                        using (Image<Gray, Byte> mask = new Image<Gray, Byte>(imageRGB.Width, imageRGB.Height, new Gray(0.0)))
                                        {
                                            mask.Draw(cir, new Gray(255.0), -1);

                                            using (Image<Bgr, Byte> res = imageRGB.InPaint(mask, 50))
                                            {

                                            }
                                        }
                                        applied = true;
                                    }
                                }
                        }
                    }

                    foreach (Image<Gray, Byte> i in imgs)
                        i.Dispose();
                }
            }
        }

        public void TestFaceDetect()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (Image<Bgr, Byte> image = new Image<Bgr, byte>("lena.jpg"))
            using (Image<Bgr, Byte> smooth = image.GaussianSmooth(7))
            {
                DateTime t1 = DateTime.Now;

                FaceDetector fd = new FaceDetector();
                Face<Byte> f = fd.Detect(smooth)[0];
                TimeSpan ts = DateTime.Now.Subtract(t1);
                Trace.WriteLine(ts.TotalMilliseconds);

                Eye<Byte> e = f.DetectEye()[0];

                Application.Run(new ImageViewer(e.RGB));

                /*
                Image<Rgb, Byte> res = f.RGB.BlankClone();
                res.Draw(f.SkinContour, new Rgb(255.0, 255.0, 255.0), new Rgb(255.0, 255.0, 255.0), -1);
                Application.Run(new ImageViewer(res.ToBitmap()));
                */
            }

            #region old code
            /*
            using (HaarCascade h = new HaarCascade(".\\haarcascades\\haarcascade_frontalface_alt2.xml"))
            using (Image<Rgb> image = Emgu.CV.Utils.LoadRGBImage("lena.jpg"))
            using (Image<Gray> gray = image.ConvertColor<Gray>())
            using (Image<Gray> small = gray.Scale(gray.Width / 2, gray.Height / 2))
            using (Image<Rgb> colorSmall = image.Scale(gray.Width / 2, gray.Height / 2))
            {
                Rectangle<int>[][] objects = small.DetectHaarCascade(h);
                foreach (Rectangle<int> o in objects[0])
                {
                    using (Image<Gray> mask = small.BlankClone(new Gray(0.0)))
                    {
                        mask.Draw(o, new Gray(255.0), -1);
                        DateTime t2 = DateTime.Now;

                        small.ROI = o;
                        colorSmall.ROI = o;
                        using (Image<Gray> face = small.Copy())
                        using (Image<Rgb> colorFace = colorSmall.Copy())
                        using (Image<Hsv> hsvFace = colorFace.ConvertColor<Hsv>())
                        {
                            Image<Gray>[] hsvPlanes = hsvFace.Split();
                            using (Image<Gray> smallSatuation = hsvPlanes[1].Threshold(new Gray(120.0), new Gray(255.0), ThresholdType2.THRESH_BINART_INV))
                            using (Image<Gray> smallValue = hsvPlanes[1].Threshold(new Gray(100.0), new Gray(255.0), ThresholdType2.THRESH_BINART_INV))
                            using (Image<Gray> smallVS = smallSatuation & smallValue)
                            //using (Image<Gray> edge = face.Canny(new Gray(160.0), new Gray(100.0)))
                            {
                                //smallValue.DilateInPlace(1);

                                //edge.DilateInPlace(1);
                                //Contours cs = edge.FindContours(ContourRetrivalMode.LIST, ContourApproxMethod.CHAIN_APPROX_SIMPLE);
                                
                                int size = 60;
                                Histogram htg = new Histogram(new int[1] { size }, new float[1] { 0.0f }, new float[1] { 180.0f });
                                htg.Accumulate(new Image<Gray>[1] { hsvPlanes[0] });
                                
                                double[] arr = new double[size];
                                for (int i = 0; i < size; i++)
                                    arr[i] = htg.Query(new int[1] { i });
                                System.Array.Sort<double>(arr);
                                System.Array.Reverse(arr);
                                htg.Threshold(arr[2]);

                                using (Image<Gray> bpj = htg.BackProject(new Image<Gray>[1] { hsvPlanes[0] }))
                                {
                                    List<Seq> cList = bpj.FindContoursList( ContourApproxMethod.CHAIN_APPROX_SIMPLE);
                                     //= cs.Elements;

                                    using (Image<Gray> cImage = smallSatuation.BlankClone(new Gray(0.0)))
                                    {
                                        Seq maxAreaContour = cList[0];
                                        foreach (Seq ct in cList)
                                        {
                                            if (ct.Area > maxAreaContour.Area)
                                                maxAreaContour = ct;
                                        }

                                        Seq snake = face.Snake(maxAreaContour, 0.5f, 0.5f, 0.5f, new Point2D<int>(5, 5), new TermCriteria(10, 5.0));
                                        
                                        //Seq temp = maxAreaContour.ApproxPoly(maxAreaContour.Perimeter * 0.02);
                                        //if (temp.Area > 10)
                                        cImage.Draw(snake, new Gray(255.0), new Gray(120.0), -1);
                                        Application.Run(new ImageViewer(cImage.ToBitmap()));
                                    }
                                }
                            }
                        }
                        small.ROI = null;
                        image.ROI = null;
                        
                    }
                }
            }*/
            #endregion
        }

        public void TestCompression()
        {
            Image<Bgr, Byte> image = new Image<Bgr, byte>("lena.jpg");
            DateTime t1 = DateTime.Now;
            int l1 = image.Bytes.Length;
            DateTime t2 = DateTime.Now;
            image.SerializationCompressionRatio = 9;
            int l2 = image.Bytes.Length;
            DateTime t3 = DateTime.Now;
            TimeSpan ts1 = t2.Subtract(t1);
            TimeSpan ts2 = t3.Subtract(t2);
            Trace.WriteLine(String.Format("Image Size: {0} x {1}", image.Width, image.Height));
            Trace.WriteLine(String.Format("T1: {0}, T2: {1}, Delta: {2}", ts1.TotalMilliseconds, ts2.TotalMilliseconds, ts2.TotalMilliseconds - ts1.TotalMilliseconds));

            Trace.WriteLine(
                String.Format(
                "Original size: {0}; Compressed Size: {1}, Compression Ratio: {2}%",
                l1,
                l2,
                l2 * 100.0 / l1));
        }

        public void TestMarshalIplImage()
        {
            Image<Bgr, Single> image = new Image<Bgr, float>(2041, 1023);
            DateTime timeStart = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                MIplImage img = image.MIplImage;
            }
            TimeSpan timeSpan = DateTime.Now.Subtract(timeStart);
            Trace.WriteLine(String.Format("Time: {0} milliseconds", timeSpan.TotalMilliseconds));
        }

        public void TestReadImage()
        {
            Application.Run(new ImageViewer(new Image<Gray, Byte>("lena.jpg")));
            Application.Run(new ImageViewer(new Image<Bgr, Byte>("lena.jpg").Convert<Gray, Byte>()));
        }

        public void TestImageViewer()
        {
            Application.Run(new ImageViewer(null));
        }

        public void TestContour()
        {
            Image<Gray, Byte> img = new Image<Gray, byte>("stuff.jpg");
            img._GaussianSmooth(3);
            img = img.Canny(new Gray(80), new Gray(50));
            Image<Gray, Byte> res = img.CopyBlank();
            res.SetValue(255);

            Contour contour = img.FindContours();

            while (contour != null)
            {
                Contour approx = contour.ApproxPoly(contour.Perimeter * 0.05);

                if (approx.Convex && Math.Abs(approx.Area) > 20.0)
                {
                    MCvPoint[] vertices = approx.ToArray();

                    LineSegment2D<int>[] edges = PointCollection.PolyLine<int>(vertices, true);

                    res.DrawPolyline(vertices, true, new Gray(200), 1);
                }
                contour = contour.HNext;
            }
            Application.Run(new ImageViewer(res));
        }

        private void Testmap()
        {
            Point2D<double> center = new Point2D<double>(-110032, -110032);

            Map<Gray, Byte> map = new Map<Gray, byte>(new Rectangle<double>(center, 10000, 12000), new Point2D<double>(100, 100));
            Point2D<float>[] pts = new Point2D<float>[]
            {
                new Point2D<float>( (float)center.X + 3120,(float) center.Y + 2310),
                new Point2D<float>((float)center.X -220, (float) center.Y-4120)
            };
            map.DrawPolyline<float>(pts, false, new Gray(255.0), 1);
            Triangle2D<float> tri = new Triangle2D<float>(
                new Point2D<float>((float)center.X - 1000.0f, (float) center.Y+200.0f),
                new Point2D<float>((float)center.X - 3000.0f, (float) center.Y+200.0f),
                new Point2D<float>((float)center.X -700f, (float) center.Y+800.0f));
            map.Draw(tri, new Gray(80), 0);
            map.Draw(tri, new Gray(255), 1);
            Application.Run(new Emgu.CV.UI.ImageViewer(map));
        }

        private void TestConvexHull()
        {
            Random r = new Random();
            Point2D<float>[] pts = new Point2D<float>[40];
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = new Point2D<float>((float)(r.NextDouble() * 600), (float)(r.NextDouble() * 600));
            }

            MCvPoint2D32f[] hull = PointCollection.ConvexHull(Emgu.Utils.IEnumConvertor<Point2D<float>, Point<float>>(pts, delegate(Point2D<float> p) { return (Point<float>)p; }), Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);

            Image<Bgr, Byte> img = new Image<Bgr, byte>(600, 600);
            foreach (Point2D<float> p in pts)
            {
                img.Draw(new Circle<float>(p, 3), new Bgr(255.0, 255.0, 255.0), 1);
            }

            MCvPoint[] convexHull = Array.ConvertAll<MCvPoint2D32f, MCvPoint>(hull, delegate(MCvPoint2D32f p) { return new MCvPoint((int)p.x, (int)p.y); });
            img.DrawPolyline(
                convexHull,
                true, new Bgr(255.0, 0.0, 0.0), 1);

            Application.Run(new ImageViewer(img));

        }
    }
}

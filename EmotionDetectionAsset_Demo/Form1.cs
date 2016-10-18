//using dlibclr;
namespace dlib_csharp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using AssetManagerPackage;
    using AssetPackage;
    using Accord.Video.FFMPEG;
    using System.Windows.Forms.DataVisualization.Charting;

    public partial class Form1 : Form
    {
        #region Fields

        const String databse = @"C:\Users\wvd_v\Documents\Visual Studio 2015\Projects\EmotionDetectionAsset\EmotionDetectionAsset_Test\bin\Debug\shape_predictor_68_face_landmarks.dat";
        const String face1 = @"C:\Users\wvd_v\Documents\Visual Studio 2015\Projects\dlib-master\examples\dlib-csharp\franck_02159.bmp";
        const String face2 = @"C:\Users\wvd_v\Documents\Visual Studio 2015\Projects\dlib-master\examples\dlib-csharp\franck_02159m.bmp";
        const String wrapper = @"C:\Users\wvd_v\Documents\Visual Studio 2015\Projects\dlib-master\examples\build\Release\dlibwrapper.dll";

        List<RECT> Faces = new List<RECT>();
        List<POINT> Landmarks = new List<POINT>();

        /// <summary>
        /// The dlib supported PixelFormats for load_bmp() in image_loader.h.
        /// </summary>
        PixelFormat[] supported =
            {
            PixelFormat.Format1bppIndexed,
            PixelFormat.Format4bppIndexed,
            PixelFormat.Format8bppIndexed,
            PixelFormat.Format24bppRgb
        };
        Stopwatch sw = new Stopwatch();

        #endregion Fields

        #region Constructors

        public Form1()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Detect faces.
        /// </summary>
        ///
        /// <param name="img">          The image. </param>
        /// <param name="length">       The length. </param>
        /// <param name="faces">        [out] The faces. </param>
        /// <param name="facecount">    [out] The facecount. </param>
        [DllImport(wrapper,
            CharSet = CharSet.Auto,
            EntryPoint = "DetectFaces",
            SetLastError = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void DetectFaces(
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] img,
            Int32 length,
            out IntPtr faces,
            out int facecount);

        /// <summary>
        /// Detect landmarks.
        /// </summary>
        ///
        /// <param name="face">         The face. </param>
        /// <param name="landmarks">    [out] The landmarks. </param>
        /// <param name="markcount">    [out] The markcount. </param>
        [DllImport(wrapper,
            CharSet = CharSet.Auto,
            EntryPoint = "DetectLandmarks",
            SetLastError = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void DetectLandmarks(RECT face, out IntPtr landmarks, out int markcount);

        /// <summary>
        /// Init database.
        /// </summary>
        ///
        /// <param name="lpFileName">   Filename of the file. </param>
        ///
        /// <returns>
        /// An int.
        /// </returns>
        [DllImport(wrapper,
            CharSet = CharSet.Auto,
            EntryPoint = "InitDatabase",
            SetLastError = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern int InitDatabase([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        /// <summary>
        /// Init detector.
        /// </summary>
        [DllImport(wrapper,
            CharSet = CharSet.Auto,
            EntryPoint = "InitDetector",
            SetLastError = true,
            CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitDetector();

        /// <summary>
        /// Event handler. Called by button1 for click events.
        /// </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        private void button1_Click(object sender, EventArgs e)
        {
            Debug.Print(String.Empty);

            using (Timing t = new Timing("Initialize"))
            {
                InitDetector();

                InitDatabase(databse);

                InitDatabase(databse);

                label1.Text = t.ElapsedMsg;
            }

            //! Has to be a BMP as dlib only supports reading BMP from a stream.
            //
        }

        /// <summary>
        /// Event handler. Called by button2 for click events.
        /// </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)Bitmap.FromFile(face2);

            pictureBox1.Image = bmp;

            DetectFacesInBitmap((Bitmap)pictureBox1.Image);
        }

        /// <summary>
        /// Event handler. Called by button3 for click events.
        /// </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        private void button3_Click(object sender, EventArgs e)
        {
            DetectLandmarksInFaces();
        }

        /// <summary>
        /// (This method is obsolete) detect landmarks in faces.
        /// </summary>
        [Obsolete()]
        private void DetectLandmarksInFaces()
        {
            foreach (RECT r in Faces)
            {
                using (Timing t = new Timing("DetectFaces"))
                {
                    int markcount = 0;
                    IntPtr landmarks = IntPtr.Zero;

                    DetectLandmarks(r, out landmarks, out markcount);

                    //Debug.Print("Detected: {0} Landmark Part(s)", markcount);
                    //Debug.Print("SizeOf(POINT)={0}", Marshal.SizeOf(new POINT()));

                    Landmarks.Clear();

                    //POINT[] ManagedPointArray = new POINT[markcount];

                    IntPtr[] pIntPtrArray = new IntPtr[markcount];

                    Marshal.Copy(landmarks, pIntPtrArray, 0, markcount);

                    for (Int32 i = 0; i < markcount; i++)
                    {
                        Landmarks.Add((POINT)Marshal.PtrToStructure(pIntPtrArray[i], typeof(POINT)));

                        Marshal.FreeCoTaskMem(pIntPtrArray[i]);

                        // Debug.Print("Landmark Point {0} @ {1}", i, Landmarks[i].ToString());
                    }

                    Marshal.FreeCoTaskMem(landmarks);

                    label1.Text = t.ElapsedMsg;
                }

                using (Graphics g1 = Graphics.FromImage(pictureBox1.Image))
                {
                    foreach (Point p in Landmarks)
                    {
                        g1.FillRectangle(new SolidBrush(Color.Red), new Rectangle(p.X - 1, p.Y - 1, 3, 3));
                    }
                }
            }

            this.Refresh();
        }

        EmotionDetectionAsset eda = new EmotionDetectionAsset();

        /// <summary>
        /// Init chart.
        /// </summary>
        private void InitChart()
        {
            chart1.Series.Clear();

            chart1.ChartAreas[0].AxisX.Maximum = 100;

            foreach (String emotion in eda.Emotions)
            {
                chart1.Series.Add(emotion).ChartType = SeriesChartType.FastLine;

                //for (Int32 i = 0; i < 100; i++)
                //{
                //    chart1.Series[emotion].Points.AddY(0.1);
                //}
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            InitChart();

            // change the capture time frame
            this.webCamCapture1.TimeToCapture_milliseconds = 20;

            // start the video capture. let the control handle the
            // frame numbers.
            this.webCamCapture1.Start(0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile("Kiavash1.jpg");

            ProcessImageIntoEmotions(pictureBox1.Image, true);
        }

        /// <summary>
        /// Detect faces in bitmap.
        /// </summary>
        ///
        /// <param name="bmp">  The bitmap. </param>
        private void DetectFacesInBitmap(Bitmap bmp)
        {
            Debug.Print("{0}", pictureBox1.Image.PixelFormat);

            //Bitmap clone = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //using (Graphics gr = Graphics.FromImage(clone))
            //{
            //    gr.DrawImage(pictureBox1.Image, new Rectangle(0, 0, clone.Width, clone.Height));
            //}

            //! This is code to make sure dlib accepts the image. 
            //! Dlib's image_load.h / load_bmp() only supports 1,4,8 or 24 bits images (so no 16 or 32 bit ones).
            // 
            Byte[] raw = (!supported.Contains(bmp.PixelFormat)) ?
                bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format24bppRgb).ToByteArray() :
                bmp.ToByteArray();

            //http://stackoverflow.com/questions/209258/dllimport-int-how-to-do-this-if-it-can-be-done
            //http://www.codeproject.com/Questions/187293/Interoperation-C-and-native-Win-C-code-arrays-of

            using (Timing t = new Timing("DetectFaces"))
            {
                int facecount = 0;
                IntPtr faces = IntPtr.Zero;

                DetectFaces(raw, raw.Length, out faces, out facecount);

                //Debug.Print("Detected: {0} face(s)", facecount);
                //Debug.Print("SizeOf(RECT)={0}", Marshal.SizeOf(new RECT()));

                Faces.Clear();

                //RECT[] ManagedRectArray = new RECT[facecount];

                if (facecount != 0)
                {
                    IntPtr[] pIntPtrArray = new IntPtr[facecount];

                    Marshal.Copy(faces, pIntPtrArray, 0, facecount);

                    for (Int32 i = 0; i < facecount; i++)
                    {
                        Faces.Add((RECT)Marshal.PtrToStructure(pIntPtrArray[i], typeof(RECT)));

                        Marshal.FreeCoTaskMem(pIntPtrArray[i]);

                        Debug.Print("Face {0} @ {1}", i, Faces[i].ToString());
                    }

                    Marshal.FreeCoTaskMem(faces);
                }

                label1.Text = t.ElapsedMsg;
            }

            using (Graphics g1 = Graphics.FromImage(pictureBox1.Image))
            {
                foreach (RECT r in Faces)
                {
                    g1.DrawRectangle(new Pen(new SolidBrush(Color.Black)), new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top));
                }
            }

            this.Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // stop the video capture
            this.webCamCapture1.Stop();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // set the image capture size
            this.webCamCapture1.CaptureHeight = this.pictureBox1.Height;
            this.webCamCapture1.CaptureWidth = this.pictureBox1.Width;

            AssetManager.Instance.Bridge = new Bridge();

            eda.Initialize(@".\shape_predictor_68_face_landmarks.dat");

            eda.ParseRules(File.ReadAllLines(@".\FURIA Fuzzy Logic Rules.txt"));

            //listView1.SetDoubleBuffered(true);
        }

        /// <summary>
        /// Draw faces and landmarks.
        /// </summary>
        public void DrawFacesAndLandmarks(Image img)
        {
            using (Graphics g = Graphics.FromImage(img))
            {
                Int32 cnt = 0;

                foreach (KeyValuePair<EmotionDetectionAsset.RECT, List<EmotionDetectionAsset.POINT>> kvp in eda.Faces)
                {
                    //! Faces
                    // 
                    g.DrawRectangle(
                        new Pen(new SolidBrush(Color.Black)),
                        new Rectangle(kvp.Key.Left, kvp.Key.Top, kvp.Key.Right - kvp.Key.Left, kvp.Key.Bottom - kvp.Key.Top));

                    //! Face Id
                    // .
                    g.DrawString(
                        String.Format("face: {0}", cnt++),
                        new Font(FontFamily.GenericSansSerif, 10.0f, FontStyle.Bold),
                        new SolidBrush(Color.Yellow),
                        new PointF(kvp.Key.Left, kvp.Key.Top));

                    //! Angles.
                    // 
                    foreach (EmotionDetectionAsset.POINT vector in eda.Vectors)
                    {
                        g.DrawLine(new Pen(
                            new SolidBrush(Color.Blue)),
                            kvp.Value[vector.X].X, kvp.Value[vector.X].Y,
                            kvp.Value[vector.Y].X, kvp.Value[vector.Y].Y);
                    }

                    //! Landmarks.
                    // 
                    foreach (EmotionDetectionAsset.POINT p in kvp.Value)
                    {
                        g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(p.X - 1, p.Y - 1, 3, 3));
                    }
                }
            }

            this.Refresh();
        }

        Int32 counter = 0;

        private void webCamCapture1_ImageCaptured(object source, WebCam_Capture.WebcamEventArgs e)
        {
            this.pictureBox1.Image = e.WebCamImage;

            ProcessImageIntoEmotions(this.pictureBox1.Image, true);

            //this.webCamCapture1.Stop();
        }

        Boolean facedetected = false;


        private void ProcessImageIntoEmotions(Image img, Boolean redetect)
        {
            //! Skipping does not seem to work properly
            // 
            if (redetect)
            {
                facedetected = eda.ProcessImage(img);

                if (facedetected)
                {
                    counter++;

                    //Debug.WriteLine(String.Format("{0} Face(s detected.", eda.Faces.Count));

                    //! Process detect faces.
                    // 
                    if (eda.ProcessFaces())
                    {
                        //! Show Detection Results.
                        // 
                        DrawFacesAndLandmarks(img);

                        UpdateOutputTable();
                    }

                    UpdateChart();
                }
            }
        }

        //Dictionary<String, List<Double>> History = new Dictionary<String, List<Double>>();

        /// <summary>
        /// Updates the output table.
        /// </summary>
        private void UpdateOutputTable()
        {
            listView1.BeginUpdate();
            {
                if (eda.ProcessLandmarks())
                {
                    Boolean first = true;

                    foreach (KeyValuePair<EmotionDetectionAsset.RECT, List<EmotionDetectionAsset.POINT>> kvp in eda.Faces)
                    {
                        //! Process detected landmarks into emotions.
                        // 

                        while (listView1.Columns.Count - 1 > eda.Faces.Count)
                        {
                            listView1.Columns.RemoveAt(1);
                        }

                        while (listView1.Columns.Count - 1 < eda.Faces.Count)
                        {
                            listView1.Columns.Add(String.Empty).Width = 80;
                        }

                        for (Int32 i = 1; i < listView1.Columns.Count; i++)
                        {
                            listView1.Columns[i].Text = String.Format("Face_{0}", i);
                        }
                    }

                    if (listView1.Items.Count == 0)
                    {
                        foreach (String emotion in eda.Emotions)
                        {
                            listView1.Items.Add(emotion);//.SubItems.Clear();
                        }
                    }

                    //! Show Result for First Detected Face.
                    // 
                    Int32 ndx = 0;

                    foreach (String emotion in eda.Emotions)
                    {
                        for (Int32 i = 0; i < eda.Faces.Count; i++)
                        {
                            String emo = String.Format("{0:0.00}", eda[i, emotion]);

                            if (listView1.Items[ndx].SubItems.Count < i + 2)
                            {
                                listView1.Items[ndx].SubItems.Add(emo);
                            }
                            else
                            {
                                if (emo != listView1.Items[ndx].SubItems[i + 1].Text)
                                {
                                    listView1.Items[ndx].SubItems[i + 1].Text = emo;
                                }
                            }
                        }

                        ndx++;
                    }
                }
            }

            listView1.EndUpdate();
        }

        /// <summary>
        /// Updates the chart.
        /// </summary>
        private void UpdateChart()
        {
            //! Plot averaged data...
            // 
            foreach (String emotion in eda.Emotions)
            {
                chart1.Series[emotion].Points.AddY(eda[0, emotion]);

                chart1.ChartAreas[0].AxisY.Maximum = Double.NaN;

                if (chart1.Series[emotion].Points.Count > 100)
                {
                    chart1.Series[emotion].Points.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Event handler. Called by button6 for click events.
        /// </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        private void button6_Click(object sender, EventArgs e)
        {
            //! Create instance of video reader
            VideoFileReader reader = new VideoFileReader();

            //! Open video file
            reader.Open(@"C:\Virtual Machines\X-HRL-A10430\Videos\Wiskunde Academie\Bewijzen - Waarom gaan de zwaartelijnen door 1 punt, het zwaartepunt_ - WiskundeAcademie.mp4");

            Image img;

            // Read 100 video frames out of it
            for (Int32 i = 0; i < reader.FrameCount; i++)
            {
                img = reader.ReadVideoFrame();

                if (img != null)
                {
                    pictureBox1.Image = img;

                    //pictureBox1.Refresh();

                    ProcessImageIntoEmotions(pictureBox1.Image, i % 10 == 0);

                    //! Dispose the frame when it is no longer required
                    img.Dispose();
                }
            }

            reader.Close();
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// A point.
        /// 
        /// <see cref="http://www.pinvoke.net/default.aspx/Structures/RECT.html"/>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            #region Constructors

            /// <summary>
            /// Constructor.
            /// </summary>
            ///
            /// <param name="x">    The x coordinate. </param>
            /// <param name="y">    The y coordinate. </param>
            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            /// <summary>
            /// Constructor.
            /// </summary>
            ///
            /// <param name="pt">   The point. </param>
            public POINT(System.Drawing.Point pt)
                : this(pt.X, pt.Y)
            {
                //
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Implicit cast that converts the given POINT to a Point.
            /// </summary>
            ///
            /// <param name="p">    The Point to process. </param>
            ///
            /// <returns>
            /// The result of the operation.
            /// </returns>
            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            /// <summary>
            /// Implicit cast that converts the given System.Drawing.Point to a POINT.
            /// </summary>
            ///
            /// <param name="p">    The Point to process. </param>
            ///
            /// <returns>
            /// The result of the operation.
            /// </returns>
            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }

            /// <summary>
            /// Returns the fully qualified type name of this instance.
            /// </summary>
            ///
            /// <returns>
            /// A <see cref="T:System.String" />
            ///  containing a fully qualified type name.
            /// </returns>
            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{X={0},Y={1}}}", X, Y);
            }

            #endregion Methods
        }

        /// <summary>
        /// A rectangle.
        /// 
        /// <see cref="http://www.pinvoke.net/default.aspx/Structures/RECT.html"/>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            #region Constructors

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r)
                : this(r.Left, r.Top, r.Right, r.Bottom)
            {
            }

            #endregion Constructors

            #region Methods

            //public int X
            //{
            //    get { return Left; }
            //    set { Right -= (Left - value); Left = value; }
            //}
            //public int Y
            //{
            //    get { return Top; }
            //    set { Bottom -= (Top - value); Top = value; }
            //}
            ///// <summary>
            ///// Gets or sets the height.
            ///// </summary>
            /////
            ///// <value>
            ///// The height.
            ///// </value>
            //public int Height
            //{
            //    get { return Bottom - Top; }
            //    set { Bottom = value + Top; }
            //}
            ///// <summary>
            ///// Gets or sets the width.
            ///// </summary>
            /////
            ///// <value>
            ///// The width.
            ///// </value>
            //public long Width
            //{
            //    get { return Right - Left; }
            //    set { Right = value + Left; }
            //}
            ///// <summary>
            ///// Gets or sets the location.
            ///// </summary>
            /////
            ///// <value>
            ///// The location.
            ///// </value>
            //public System.Drawing.Point Location
            //{
            //    get { return new System.Drawing.Point(Left, Top); }
            //    set { X = value.X; Y = value.Y; }
            //}
            ///// <summary>
            ///// Gets or sets the size.
            ///// </summary>
            /////
            ///// <value>
            ///// The size.
            ///// </value>
            //public System.Drawing.Size Size
            //{
            //    get { return new System.Drawing.Size(Width, Height); }
            //    set { Width = value.Width; Height = value.Height; }
            //}
            /// <summary>
            /// Implicit cast that converts the given RECT to a Rectangle.
            /// </summary>
            ///
            /// <param name="r">    The rectangle to compare to this object. </param>
            ///
            /// <returns>
            /// The result of the operation.
            /// </returns>
            public static implicit operator System.Drawing.Rectangle(RECT r)
            {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top);
            }

            /// <summary>
            /// Implicit cast that converts the given System.Drawing.Rectangle to a RECT.
            /// </summary>
            ///
            /// <param name="r">    The rectangle to compare to this object. </param>
            ///
            /// <returns>
            /// The result of the operation.
            /// </returns>
            public static implicit operator RECT(System.Drawing.Rectangle r)
            {
                return new RECT(r);
            }

            ///// <summary>
            ///// Equality operator.
            ///// </summary>
            /////
            ///// <param name="r1">   The first RECT. </param>
            ///// <param name="r2">   The second RECT. </param>
            /////
            ///// <returns>
            ///// The result of the operation.
            ///// </returns>
            //public static bool operator ==(RECT r1, RECT r2)
            //{
            //    return r1.Equals(r2);
            //}
            ///// <summary>
            ///// Inequality operator.
            ///// </summary>
            /////
            ///// <param name="r1">   The first RECT. </param>
            ///// <param name="r2">   The second RECT. </param>
            /////
            ///// <returns>
            ///// The result of the operation.
            ///// </returns>
            //public static bool operator !=(RECT r1, RECT r2)
            //{
            //    return !r1.Equals(r2);
            //}
            ///// <summary>
            ///// Indicates whether this instance and a specified object are equal.
            ///// </summary>
            /////
            ///// <param name="r">    The rectangle to compare to this object. </param>
            /////
            ///// <returns>
            ///// true if <paramref name="obj" />
            /////  and this instance are the same type and represent the same value; otherwise, false.
            ///// </returns>
            //public bool Equals(RECT r)
            //{
            //    return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            //}
            ///// <summary>
            ///// Indicates whether this instance and a specified object are equal.
            ///// </summary>
            /////
            ///// <param name="obj">  The object to compare with the current instance. </param>
            /////
            ///// <returns>
            ///// true if <paramref name="obj" />
            /////  and this instance are the same type and represent the same value; otherwise, false.
            ///// </returns>
            //public override bool Equals(object obj)
            //{
            //    if (obj is RECT)
            //        return Equals((RECT)obj);
            //    else if (obj is System.Drawing.Rectangle)
            //        return Equals(new RECT((System.Drawing.Rectangle)obj));
            //    return false;
            //}
            ///// <summary>
            ///// Returns the hash code for this instance.
            ///// </summary>
            /////
            ///// <returns>
            ///// A 32-bit signed integer that is the hash code for this instance.
            ///// </returns>
            //public override int GetHashCode()
            //{
            //    return ((System.Drawing.Rectangle)this).GetHashCode();
            //}
            /// <summary>
            /// Returns the fully qualified type name of this instance.
            /// </summary>
            ///
            /// <returns>
            /// A <see cref="T:System.String" />
            ///  containing a fully qualified type name.
            /// </returns>
            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }

            #endregion Methods
        }

        public class Timing : IDisposable
        {
            #region Fields

            private bool disposedValue = false; // To detect redundant calls
            String msg = String.Empty;
            Stopwatch sw = new Stopwatch();

            #endregion Fields

            #region Constructors

            public Timing(String msg)
            {
                this.msg = msg;

                sw.Reset();
                sw.Start();
            }

            #endregion Constructors

            #region Properties

            public String ElapsedMsg
            {
                get
                {
                    return String.Format("{0}: {1} ms", msg, sw.ElapsedMilliseconds);
                }
            }

            #endregion Properties

            #region Methods

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~Timing() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }
            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        sw.Stop();
                        Debug.Print(ElapsedMsg);
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    disposedValue = true;
                }
            }

            #endregion Methods
        }

        #endregion Nested Types

    }
}
/*
 * Copyright 2017 Open University of the Netherlands (OUNL)
 *
 * Authors: Kiavash Bahreini, Wim van der Vegt.
 * Organization: Open University of the Netherlands (OUNL).
 * Project: The RAGE project
 * Project URL: http://rageproject.eu.
 * Task: T2.3 of the RAGE project; Development of assets for emotion detection. 
 * 
 * For any questions please contact: 
 *
 * Kiavash Bahreini via kiavash.bahreini [AT] ou [DOT] nl
 * and/or
 * Wim van der Vegt via wim.vandervegt [AT] ou [DOT] nl
 *
 * Cite this work as:
 * Bahreini, K., van der Vegt, W. & Westera, W. Multimedia Tools and Applications (2019). https://doi.org/10.1007/s11042-019-7250-z
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union’s Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace AssetPackage
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using AssetManagerPackage;

    //! TODO Dynamically Load DLL (now it should be on the path).
    //! TODO Add scaling factor.
    //! TODO Add Emotion (strip)chart in Demo.
    //! TODO Read Vectors from file (as it should match the rules input file).
    //!
    //! DONE DlibWrapper.dll Path should be a (static) property.
    //! DONE Grayscale support.
    //! DONE Database Path should be a (static) property.
    //! DONE Output emotions for all faces in Demo.
    /// <summary>
    /// The detected emotions.
    /// 
    /// The Key is the name of the emotion.
    /// 
    /// The Value is the fuzzy outcome for the emtion.
    /// </summary>
    using DetectedEmotions = System.Collections.Generic.Dictionary<System.String, System.Double>;

    /// <summary>
    /// The detected face.
    /// 
    /// The Key is the detection rectangle.
    /// 
    /// The Value is a list of 68 landmark points.
    /// </summary>
    using DetectedFace = System.Collections.Generic.KeyValuePair<EmotionDetectionAsset.RECT, System.Collections.Generic.List<EmotionDetectionAsset.POINT>>;

    /// <summary>
    /// The detected faces.
    /// 
    /// The Key is the detection rectangle.
    /// 
    /// The Value is a list of 68 landmark points.
    /// </summary>
    using DetectedFaces = System.Collections.Generic.Dictionary<EmotionDetectionAsset.RECT, System.Collections.Generic.List<EmotionDetectionAsset.POINT>>;

    /// <summary>
    /// An asset.
    /// </summary>
    public class EmotionDetectionAsset : BaseAsset
    {
        #region Fields

        /// <summary>
        /// The names of detectable Emotions.
        /// </summary>
        public List<String> Emotions = new List<String>();

        /// <summary>
        /// The list of Fuzzy expressions parsed.
        /// </summary>
        ///
        /// <remarks>
        /// Each expression is a single line from the FURIA output.
        /// </remarks>
        private FuzzyExpressions Expressions = new FuzzyExpressions();

        /// <summary>
        /// The detected faces and their landmarks.
        /// </summary>
        public DetectedFaces Faces = new DetectedFaces();

        /// <summary>
        /// The vectors used to calculate the angles.
        /// 
        /// <remark>This need to be read from a config file</remark>
        /// </summary>
        public List<POINT> Vectors = new List<POINT>()
        {
          	//! Left eyebrow to the left eye.
          	//
            new POINT(17,36),   // 0
            new POINT(17,39),   // 1
            new POINT(36,39),   // 2

            new POINT(19,36),   // 3
            new POINT(19,39),   // 4
            new POINT(36,39),   //! equal to [2]

            new POINT(21,36),   // 5
            new POINT(21,39),   // 6
            new POINT(36,39),   //! equal to [2]

            //! Right eyebrow to the right eye.
            //
            new POINT(22,42),   // 7
            new POINT(22,45),   // 8
            new POINT(42,45),   // 9

            new POINT(24,42),   // 10
            new POINT(24,45),   // 11
            new POINT(24,45),   //! equal to [11]

            new POINT(26,42),   // 13
            new POINT(26,45),   // 14
            new POINT(24,45),   //! equal to [11]

            //! Left eye.
            //
            new POINT(37,40),   // 16
            new POINT(37,41),   // 17
            new POINT(40,41),   // 20

            new POINT(38,40),   // 21
            new POINT(38,41),   // 22
            new POINT(40,41),   //! equal to [20]

            //! Right eye?
            //
            new POINT(43,46),   // 24
            new POINT(43,47),   // 25
            new POINT(46,47),   // 26

            new POINT(44,46),   // 27
            new POINT(44,47),   // 28
            new POINT(46,47),   //! equal to [26]

            //! Top of the mouth.
            //
            new POINT(48,51),   // 30
            new POINT(51,54),   // 31
            new POINT(48,54),   // 32

            //! Bottom of the mouth.
            //
            new POINT(48,57),   // 33
            new POINT(54,57),   // 34
            new POINT(48,54),   //! equal to [32]

            //! Left eyebrow.
            //
            new POINT(17,19),   // 36
            new POINT(19,21),   // 37
            new POINT(17,21),   // 38

            //! Right eyebrow.
            //
            new POINT(22,24),   // 39
            new POINT(24,26),   // 40
            new POINT(22,26),   // 41

            //! Eyebrows to the top nose.
            //
            new POINT(21,27),   // 42
            new POINT(22,27),   // 43
            new POINT(21,22),   // 44

            //! Left eye to the mouth.
            //
            new POINT(36,60),   // 45
            new POINT(48,60),   // 46
            new POINT(36,48),   // 47

            //! Right eye to the mouth.
            //
            new POINT(45,64),   // 48
            new POINT(54,64),   // 49
            new POINT(45,54),   // 50

            //! Mouth to eyes.
            //
            new POINT(39,51),   // 51
            new POINT(42,51),   // 52
            new POINT(39,42),   // 53
           };

        /// <summary>
        /// The 64 bits wrapper.
        /// </summary>
        public static String wrapper64 = @"dlibwrapper_x64.dll";

        /// <summary>
        /// The 32 bits wrapper.
        /// </summary>
        public static String wrapper32 = @"dlibwrapper_x86.dll";

        /// <summary>
        /// The first regex used to parse the left part of FURIA fuzzy rules.
        /// </summary>
        private static Regex rg1 = new Regex(@"\((?<var>[a-zA-Z][0-9]+) in \[(?<lsb>[\-\w\.]+), (?<lst>[\-\w\.]+), (?<rst>[\w\.]+), (?<rsb>[\w\.]+)\]\)");

        /// <summary>
        /// The second regex used to parse the right part of FURIA fuzzy rules.
        /// </summary>
        private static Regex rg2 = new Regex(@"Emotions=(?<emotion>[A-Za-z]*) \(CF = (?<cf>[0-9\.]+)\)");

        /// <summary>
        /// The history of emotions.
        /// 
        /// The Key is the number of the face.
        /// 
        /// The value is a list of detected emotions fot this face.
        /// </summary>
        private Dictionary<Int32, List<DetectedEmotions>> EmotionsHistory = new Dictionary<Int32, List<DetectedEmotions>>();

        /// <summary>
        /// Options for controlling the operation.
        /// </summary>
        private EmotionDetectionAssetSettings settings = null;

        /// <summary>
        /// The dlib supported PixelFormats for load_bmp() in image_loader.h.
        /// </summary>
        PixelFormat[] supported =
            {
            PixelFormat.Format1bppIndexed,
            PixelFormat.Format4bppIndexed,
            PixelFormat.Format8bppIndexed,

            //! Dlib lack support for Format16bppRgb is missing.

            PixelFormat.Format24bppRgb

            //! Dlib lack support for Format32bppRgb (my webcam's default).
        };

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the EmotionDetectionAsset.Asset class.
        /// </summary>
        public EmotionDetectionAsset()
            : base()
        {
            //! Create Settings and let it's BaseSettings class assign Defaultvalues where it can.
            //
            settings = new EmotionDetectionAssetSettings();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets options for controlling the operation.
        /// </summary>
        ///
        /// <remarks>   Besides the toXml() and fromXml() methods, we never use this property but use
        ///                it's correctly typed backing field 'settings' instead. </remarks>
        /// <remarks> This property should go into each asset having Settings of its own. </remarks>
        /// <remarks>   The actual class used should be derived from BaseAsset (and not directly from
        ///             ISetting). </remarks>
        ///
        /// <value>
        /// The settings.
        /// </value>
        public override ISettings Settings
        {
            get
            {
                return settings;
            }
            set
            {
                settings = (value as EmotionDetectionAssetSettings);
            }
        }

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        ///
        /// <remarks>
        /// This is a moving average filtered value. 
        /// Set Settings.Average to 1 and Settings.SuppressSpikes to false to disable.
        /// </remarks>
        ///
        /// <param name="face"> The face. </param>
        ///
        /// <returns>
        /// The indexed item.
        /// </returns>
        public List<DetectedEmotions> this[Int32 face]
        {
            get
            {
                if (EmotionsHistory.ContainsKey(face))
                {
                    return EmotionsHistory[face];
                }

                return null;
            }
        }

        /// <summary>
        /// Indexer to get items within this collection using array index syntax.
        /// </summary>
        ///
        /// <remarks>
        /// This is a moving average filtered value. 
        /// Set Settings.Average to 1 and Settings.SuppressSpikes to false to disable.
        /// </remarks>
        ///
        /// <param name="face">     The face. </param>
        /// <param name="emotion">  The emotion. </param>
        ///
        /// <returns>
        /// The indexed item.
        /// </returns>
        public Double this[Int32 face, String emotion]
        {
            get
            {
                //! Check if there are enough points to average.
                // 
                if (EmotionsHistory.ContainsKey(face) && EmotionsHistory[face].Count > (settings.SuppressSpikes ? 2 : 0))
                {
                    if (settings.SuppressSpikes)
                    {
                        //! When Suppressing Spikes, ignore the last two entries as they are used for filtering and not stable yet.
                        // 
                        return EmotionsHistory[face].Select(p => p[emotion]).Take(EmotionsHistory[face].Count - 2).Average();
                    }
                    else
                    {
                        //Log(Severity.Verbose, "Avg Count: {0}", EmotionsHistory[face].Select(p => p[emotion]).Count());
                        return EmotionsHistory[face].Select(p => p[emotion]).Average();
                    }
                }

                return 0;
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Detect emotions in landmarks.
        /// </summary>
        public Boolean DetectEmotionsInLandmarks()
        {
            Int32 ndx = 0;

            foreach (DetectedFace kvp in Faces)
            {
                DetectedEmotions DetectedEmotions = new DetectedEmotions();

                List<Double> EuclideanDistances = new List<Double>();
                List<Double> Cosines = new List<Double>();
                List<Double> ArcCosines = new List<Double>();

                //! 1) Calculate Euclidean Distances from Landmarks.
                //
                foreach (POINT p in Vectors)
                {
                    EuclideanDistances.Add(EuclideanDistance(kvp.Value[p.X], kvp.Value[p.Y]));
                }

                //! 2) Calculate Cosines from Euclidean Distances.
                //
                for (Int32 i = 0; i < EuclideanDistances.Count; i += 3)
                {
                    Cosines.Add(Cosine(EuclideanDistances[i + 0], EuclideanDistances[i + 1], EuclideanDistances[i + 2]));
                    Cosines.Add(Cosine(EuclideanDistances[i + 1], EuclideanDistances[i + 2], EuclideanDistances[i + 0]));
                    Cosines.Add(Cosine(EuclideanDistances[i + 2], EuclideanDistances[i + 0], EuclideanDistances[i + 1]));
                }

                //! Calculate ArcCosines from Cosines.
                //
                for (Int32 i = 0; i < Cosines.Count; i++)
                {
                    ArcCosines.Add(ArcCosine(Cosines[i]));
                }

                //! Evaluate FURIA Fuzzy Rules with ArCosines as Input.
                //
                foreach (IGrouping<String, FuzzyExpression> emotion in Expressions.GroupBy(p => p.Emotion))
                {
                    Double orresult = 0;    //! Classic Fuzzy Logic: Double.MinValue;

                    foreach (FuzzyExpression expression in emotion)
                    {
                        Double andresult = Double.NaN;  //! Classic Fuzzy Logic: Double.MaxValue;

                        foreach (FuzzyPart part in expression)
                        {
                            //! Normal Fuzzy And is just take the min of both operands.
                            //
                            andresult = Double.IsNaN(andresult) ? part.Result(ArcCosines) : andresult * part.Result(ArcCosines); //! Classic Fuzzy Logic: Math.Min(andresult, part.Result(ArcCosines));
                        }

                        //! Normal Fuzzy Or is just take the max of the operands.
                        //! We multiply the part first with the Certainty Factor (CF).
                        //

                        orresult += expression.CF * andresult; //! Classic Fuzzy Logic: orresult = Math.Max(orresult, expression.CF * andresult);
                    }

                    DetectedEmotions[emotion.Key] = orresult;
                }

                //! Build some history so we can average.
                //
                if (!EmotionsHistory.ContainsKey(ndx))
                {
                    EmotionsHistory.Add(ndx, new List<DetectedEmotions>());
                }

                //! NEW CODE SPIKE FILTERING
                // 
                if (settings.SuppressSpikes && EmotionsHistory[ndx].Count >= settings.Average + 2)
                {
                    Double SpikeAmplitude = settings.SpikeAmplitude;

                    Double v;
                    Double lv;
                    Double nv;

                    //foreach (Int32 face in EmotionsHistory.Keys)
                    //{
                    Int32 cnt = EmotionsHistory[ndx].Count;

                    foreach (String emotion in Emotions)
                    {
                        lv = EmotionsHistory[ndx][cnt - 2][emotion];
                        v = EmotionsHistory[ndx][cnt - 1][emotion];
                        nv = DetectedEmotions[emotion];

                        if (((v >= lv + SpikeAmplitude) && (v >= nv + SpikeAmplitude)) ||
                          ((v <= lv - SpikeAmplitude) && (v <= nv - SpikeAmplitude)))
                        {
                            EmotionsHistory[ndx][cnt - 1][emotion] = (lv + nv) / 2;
                        }
                    }
                    //}
                }

                EmotionsHistory[ndx].Add(DetectedEmotions);

                if (EmotionsHistory[ndx].Count > settings.Average + (settings.SuppressSpikes ? 2 : 0))
                {
                    EmotionsHistory[ndx].RemoveAt(0);
                }

                //! Broadcast Emotions.
                // 
                foreach (String emotion in Emotions)
                {
                    Messages.broadcast(emotion, new EmotionEventArgs()
                    {
                        face = ndx,
                        value = this[ndx, emotion]
                    });
                }

                ndx++;
            }

            return ndx != 0;
        }

        /// <summary>
        /// Initializes the dlib wrapper and face detection.
        /// </summary>
        ///
        /// <param name="dllPath">  Full pathname of the DLL file. </param>
        /// <param name="database"> The database. </param>
        public void Initialize(String dllPath, String database)
        {
            // Load the appropriate dll (x86 or x64) depending on the CPU/OS version. 
            // 
            DlibWrapper.LoadWrapper(this, dllPath);

            // Init the Dlib Face Detection. 
            // 
            DlibWrapper.InitDetector();

            //! Init the DLib Landmark Detection twice or we do not see any faces detected. Reason not known, need to debug this.
            //
            DlibWrapper.InitDatabase(database);
            DlibWrapper.InitDatabase(database);
        }

        /// <summary>
        /// Parse a FURIA Fuzzy Rule.
        /// </summary>
        ///
        /// <param name="rule"> The expression. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public Boolean ParseRule(String rule)
        {
            if (!String.IsNullOrEmpty(rule.Trim()))
            {
                FuzzyExpression expression = new FuzzyExpression();

                String[] fsplit = rule
                    .Split(new string[] { "=>" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToArray();

                if (fsplit.Length == 2)
                {
                    String logic = fsplit[0];
                    String score = fsplit[1];

                    if (rg2.IsMatch(score))
                    {
                        foreach (Match m in rg2.Matches(score))
                        {
                            if (m.Success)
                            {
                                expression.Emotion = m.Groups["emotion"].Value;
                                expression.CF = Double.Parse(m.Groups["cf"].Value, CultureInfo.InvariantCulture);
                            }
                        }
                    }

                    String[] parts = logic
                        .Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(p => p.Trim())
                        .ToArray();

                    foreach (String part in parts)
                    {
                        // (V30 in [159.608, 160.424, inf, inf])
                        //

                        if (rg1.IsMatch(part))
                        {
                            foreach (Match m in rg1.Matches(part))
                            {
                                if (m.Success)
                                {
                                    FuzzyPart fuzzy = new FuzzyPart();

                                    // lsb is Left Shoulder Bottom Location
                                    // lst is Left Shoulder Top Location
                                    // rst is Right Shoulder Top Location
                                    // rsb is Right Shoulder Bottom Location

                                    //! ____/----\____

                                    fuzzy.var = Int32.Parse(m.Groups["var"].Value.TrimStart('V'), CultureInfo.InvariantCulture);
                                    fuzzy.lsb = ParseNumber(m, "lsb", Double.NegativeInfinity);
                                    fuzzy.lst = ParseNumber(m, "lst", Double.NegativeInfinity);
                                    fuzzy.rst = ParseNumber(m, "rst", Double.PositiveInfinity);
                                    fuzzy.rsb = ParseNumber(m, "rsb", Double.PositiveInfinity);

                                    expression.Add(fuzzy);
                                }
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                Expressions.Add(expression);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Parse rules.
        /// </summary>
        ///
        /// <param name="rules">    The rules. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public Boolean ParseRules(String[] rules)
        {
            Expressions.Clear();
            Emotions.Clear();

            foreach (String rule in rules)
            {
                if (!ParseRule(rule) && rule.StartsWith("(V"))
                {
                    //! Parsing error on a rule.
                    //
                    return false;
                }
            }

            Emotions = Expressions.Select(p => p.Emotion).Distinct().ToList();

            foreach (String emotion in Emotions)
            {
                if (!Messages.define(emotion))
                {
                    Log(Severity.Warning, "Error defining {0} message", emotion);
                }
            }

            return true;
        }

        /// <summary>
        /// Process the faces into 68 landmarks.
        /// </summary>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public Boolean ProcessFaces()
        {
            DetectLandmarksInFaces();

            foreach (KeyValuePair<RECT, List<POINT>> kvp in Faces)
            {
                if (kvp.Value.Count != 0)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Process the image described by bmp into zero or more faces.
        /// </summary>
        ///
        /// <param name="bmp">      The bitmap. </param>
        /// <param name="width">    The width. </param>
        /// <param name="height">   The height. </param>
        /// <param name="flip">     (Optional) True to flip. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public Boolean ProcessImage(Byte[] bmp, Int32 width, Int32 height, Boolean flip = false)
        {
            Faces.Clear();

#warning add additional checks on format & size.

            if (DlibWrapper.SetImageToRGBA(bmp, width, height, flip))
            {
                DetectFacesInImage();
            }

            return Faces.Count != 0;
        }

        /// <summary>
        /// Process the image described by bmp into zero or more faces.
        /// </summary>
        ///
        /// <param name="bmp">  The bitmap. </param>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public Boolean ProcessImage(Image bmp)
        {
            Faces.Clear();

            if (!supported.Contains(bmp.PixelFormat))
            {
                Log(Severity.Warning, "Unsupported PixelFormat {0}, will need conversion", bmp.PixelFormat);
            }

            //Bitmap clone = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //using (Graphics gr = Graphics.FromImage(clone))
            //{
            //    gr.DrawImage(pictureBox1.Image, new Rectangle(0, 0, clone.Width, clone.Height));
            //}

            //! This is code to make sure dlib accepts the image.
            //! Dlib's image_load.h / load_bmp() only supports 1,4,8 or 24 bits images (so no 16 or 32 bit ones).
            Boolean gray = ((EmotionDetectionAssetSettings)settings).GrayScale;

            // GrayScale() returns PixelFormat.Format32bppArgb
            bmp = gray ? bmp.GrayScale() : bmp;

            Byte[] raw = !supported.Contains(bmp.PixelFormat)
                ? ((Bitmap)bmp).Clone(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    PixelFormat.Format24bppRgb).ToByteArray()
                : ((Bitmap)bmp).ToByteArray();
            /*
            #warning TEST CODE New Exported Methods

                        // See https://msdn.microsoft.com/en-us/library/5ey6h79d(v=vs.110).aspx

                        //! 1) Lock the bitmap's bits.  
                        Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                        BitmapData bmpData =
                         ((Bitmap)bmp).LockBits(rect, ImageLockMode.ReadOnly,
                            bmp.PixelFormat);

                        //! 2) Get the address of the first line.
                        IntPtr ptr = bmpData.Scan0;

                        //! 3) Declare an array to hold the bytes of the bitmap.
                        int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
                        raw = new byte[bytes];

                        //! 4) Copy the RGB values into the array.
                        Marshal.Copy(ptr, raw, 0, bytes);

                        //! 5) Unlock the bits.
                        ((Bitmap)bmp).UnlockBits(bmpData);

            #warning DEBUG CODE

                        Color pixel = ((Bitmap)bmp).GetPixel(0, 0);

                        // ARGB [255] 229 198 154 Format24bppRgb
                        Log(Severity.Verbose, "ARGB [{0}] {1} {2} {3} {4}", pixel.A, pixel.R, pixel.G, pixel.B, bmp.PixelFormat.ToString());
            */
            //DlibWrapper.SetImageToBmp(raw, raw.Length);

#warning This code must be more versatile to support more formats

            // See https://msdn.microsoft.com/en-us/library/system.drawing.imaging.pixelformat(v=vs.110).aspx

            //! No need to test pixelformat any further as raw contains either a supported pixel depth or is a to 24 bits converted image.
            // 
            if (DlibWrapper.SetImageToBmp(raw, raw.Length))
            {
                DetectFacesInImage();
            }

            //if (bmp.PixelFormat == PixelFormat.Format32bppPArgb || bmp.PixelFormat == PixelFormat.Format32bppArgb)
            //{
            // No need to flip Image.
            // 
            //if (
            //    //DlibWrapper.SetImageToRGBA(raw, bmp.Width, bmp.Height, false)
            //    DlibWrapper.SetImageToBmp(raw, raw.Length))
            //{
            //    DetectFacesInImage();
            //}
            //}
            //else
            //{
            //    // No need to flip Image.
            //    // 
            //    if (DlibWrapper.SetImageToRGB(raw, bmp.Width, bmp.Height, false))
            //    {
            //        DetectFacesInImage();
            //    }
            //}

            return Faces.Count != 0;
        }

        /// <summary>
        /// Process the landmarks into emotions usingg fuzzy logic.
        /// </summary>
        ///
        /// <returns>
        /// true if it succeeds, false if it fails.
        /// </returns>
        public Boolean ProcessLandmarks()
        {
            return DetectEmotionsInLandmarks();
        }

        /// <summary>
        /// Parse number.
        /// </summary>
        ///
        /// <param name="m">    The Match to process. </param>
        /// <param name="grp">  The group. </param>
        /// <param name="def">  The definition. </param>
        ///
        /// <returns>
        /// A Double.
        /// </returns>
        private static Double ParseNumber(Match m, String grp, Double def)
        {
            return m.Groups[grp].Value.EndsWith("inf") ? def : Double.Parse(m.Groups[grp].Value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// General Solution: https://www.mathsisfun.com/algebra/trig-solving-triangles.html Solution for
        /// this specific problem: https://www.mathsisfun.com/algebra/trig-solving-sss-triangles.html
        /// 
        /// To solve three sides of the triangle (called an SSS triangle) : Use The Law of Cosines first
        /// to calculate one of the angles then use The Law of Cosines again to find another angle and
        /// finally use angles of a triangle add to 180° to find the last angle.
        ///  We use the "angle" version of the Law of Cosines :
        /// 
        /// cos(C) = (a² + b² − c²)/2ab  
        /// cos(A) = (b² + c² − a²)/2bc  
        /// cos(B) = (c² + a² − b²)/2ca.
        /// </summary>
        ///
        /// <param name="a">    The POINT to process. </param>
        ///
        /// <returns>
        /// A double.
        /// </returns>
        private double ArcCosine(double a)
        {
            return (Math.Acos(a) * 180.0 / Math.PI);
        }

        /// <summary>
        /// General Solution: https://www.mathsisfun.com/algebra/trig-solving-triangles.html Solution for
        /// this specific problem: https://www.mathsisfun.com/algebra/trig-solving-sss-triangles.html
        /// 
        /// To solve three sides of the triangle (called an SSS triangle) :
        /// 
        /// Use The Law of Cosines first to calculate one of the angles then use The Law of Cosines again
        /// to find another angle and finally use angles of a triangle add to 180° to find the last angle.
        /// 
        /// We use the "angle" version of the Law of Cosines :
        /// 
        /// cos(C) = (a² + b² − c²)/2ab  
        /// cos(A) = (b² + c² − a²)/2bc  
        /// cos(B) = (c² + a² − b²)/2ca.
        /// </summary>
        ///
        /// <param name="a">    The POINT to process. </param>
        /// <param name="b">    The POINT to process. </param>
        /// <param name="c">    The double to process. </param>
        ///
        /// <returns>
        /// A double.
        /// </returns>
        private double Cosine(double a, double b, double c)
        {
#warning Possibility of a divide by zero!
            return ((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b));
        }

        private void DetectFacesInImage()
        {
            int facecount = 0;
            IntPtr faces = IntPtr.Zero;

            DlibWrapper.DetectFaces(out faces, out facecount);

            Faces.Clear();

            if (facecount != 0)
            {
                IntPtr[] pIntPtrArray = new IntPtr[facecount];

                Marshal.Copy(faces, pIntPtrArray, 0, facecount);

                for (Int32 i = 0; i < facecount; i++)
                {
                    Faces.Add((RECT)Marshal.PtrToStructure(pIntPtrArray[i], typeof(RECT)), new List<POINT>());

                    Marshal.FreeCoTaskMem(pIntPtrArray[i]);
                }

                Marshal.FreeCoTaskMem(faces);
            }
        }

        /// <summary>
        /// Detect landmarks in faces.
        /// </summary>
        private void DetectLandmarksInFaces()
        {
            foreach (KeyValuePair<RECT, List<POINT>> kvp in Faces)
            {
                int markcount = 0;
                IntPtr landmarks = IntPtr.Zero;

                DlibWrapper.DetectLandmarks(kvp.Key, out landmarks, out markcount);

                kvp.Value.Clear();

                IntPtr[] pIntPtrArray = new IntPtr[markcount];

                Marshal.Copy(landmarks, pIntPtrArray, 0, markcount);

                for (Int32 i = 0; i < markcount; i++)
                {
                    kvp.Value.Add((POINT)Marshal.PtrToStructure(pIntPtrArray[i], typeof(POINT)));

                    Marshal.FreeCoTaskMem(pIntPtrArray[i]);
                }

                Marshal.FreeCoTaskMem(landmarks);
            }
        }

        /// <summary>
        /// Solution: http://www.cut-the-knot.org/pythagoras/DistanceFormula.shtml
        /// 
        /// dist((x, y), (a, b)) = √(x - a)² + (y - b)².
        /// </summary>
        ///
        /// <param name="a">    The POINT to process. </param>
        /// <param name="b">    The POINT to process. </param>
        ///
        /// <returns>
        /// A double.
        /// </returns>
        private double EuclideanDistance(POINT a, POINT b)
        {
            return (Math.Sqrt(
                Math.Pow(a.X - b.X, 2) +
                Math.Pow(a.Y - b.Y, 2)));
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// A point (to bridge the gap between C++ and C#).
        /// 
        /// <see cref="http://www.pinvoke.net/default.aspx/Structures/RECT.html"/>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>
            /// The X coordinate.
            /// </summary>
            public int X;

            /// <summary>
            /// The Y coordinate.
            /// </summary>
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

            #endregion Constructors

            #region Methods

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
                return string.Format("{{X={0},Y={1}}}", X, Y);
            }

            #endregion Methods
        }

        /// <summary>
        /// A rectangle (to bridge the gap between C++ and C#).
        /// 
        /// <see cref="http://www.pinvoke.net/default.aspx/Structures/RECT.html"/>
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// The left.
            /// </summary>
            public int Left;

            /// <summary>
            /// The top.
            /// </summary>
            public int Top;

            /// <summary>
            /// The right.
            /// </summary>
            public int Right;

            /// <summary>
            /// The bottom.
            /// </summary>
            public int Bottom;

            #region Constructors

            /// <summary>
            /// Constructor.
            /// </summary>
            ///
            /// <param name="left">     The left. </param>
            /// <param name="top">      The top. </param>
            /// <param name="right">    The right. </param>
            /// <param name="bottom">   The bottom. </param>
            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Convert this object into a string representation.
            /// </summary>
            ///
            /// <returns>
            /// A string that represents this object.
            /// </returns>
            public override string ToString()
            {
                return string.Format("{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }

            #endregion Methods
        }

        /// <summary>
        /// A fuzzy expression.
        /// </summary>
        public class FuzzyExpression : List<FuzzyPart>
        {
            #region Fields

            /// <summary>
            /// The Certainty Factor (CF).
            /// </summary>
            public Double CF;

            /// <summary>
            /// The emotion.
            /// </summary>
            public String Emotion;

            #endregion Fields
        }

        /// <summary>
        /// Executes the input needed action.
        /// </summary>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        The EventArgs&lt;Int32&gt; to process. </param>
        ///
        /// <returns>
        /// A Double.
        /// </returns>
        public class FuzzyExpressions : List<FuzzyExpression>
        {
            #region Constructors

            /// <summary>
            /// Static constructor.
            /// </summary>
            ///
            /// <remarks>
            /// Example: "(V30 in [159.608, 160.424, inf, inf]) and (V35 in [30.0655, 30.2536, inf, inf]) =>
            /// Emotions=Happy (CF = 0.97)";
            /// 
            /// Where Vnn are ArcCosine input parameters and the [] bracketed parts are the fuzzy operands
            /// (trapeziums) the input is held against.
            /// 
            /// Fuzzy and means taking the max of the both operands.
            /// </remarks>
            static FuzzyExpressions()
            {
                //
            }

            #endregion Constructors
        }

        /// <summary>
        /// A fuzzy part.
        /// </summary>
        public class FuzzyPart
        {
            #region Fields

            /// <summary>
            /// The Left Shoulder Bottom.
            /// </summary>
            public Double lsb;

            /// <summary>
            /// The Left Shoulder Top.
            /// </summary>
            public Double lst;

            /// <summary>
            /// The Right Shoulder Bottom.
            /// </summary>
            public Double rsb;

            /// <summary>
            /// The Right Shoulder Top.
            /// </summary>
            public Double rst;

            /// <summary>
            /// The variable.
            /// </summary>
            public Int32 var;

            #endregion Fields

            #region Methods

            /// <summary>
            /// Result given the input (.
            /// </summary>
            ///
            /// <param name="Input">    The input. </param>
            ///
            /// <returns>
            /// A Double.
            /// </returns>
            public Double Result(List<Double> Input)
            {
                Double value = Input[var];

                if (value >= lsb && value <= rsb)
                {
                    if (value < lst)
                    {
                        //! partially valid
                        return (value - lsb) / (lst - lsb);
                    }
                    else if (value >= lst && value <= rst)
                    {
                        //! valid
                        return 1;
                    }
                    else if (value > rst)
                    {
                        //! partially valid
                        return (rsb - value) / (rsb - rst);
                    }
                }

                //! invalid (value < lsb) or (value > rsb)
                return 0;
            }

            /// <summary>
            /// Convert this object into a string representation.
            /// </summary>
            ///
            /// <returns>
            /// A string that represents this object.
            /// </returns>
            public override string ToString()
            {
                return String.Format("({0} in [{1}, {2}, {3},{4}])",
                    var,
                    Double.IsNegativeInfinity(lsb) ? "-inf" : lsb.ToString(),
                    Double.IsNegativeInfinity(lsb) ? "-inf" : lst.ToString(),
                    Double.IsPositiveInfinity(rst) ? "+inf" : rst.ToString(),
                    Double.IsPositiveInfinity(rsb) ? "+inf" : rsb.ToString()
                    );
            }

            #endregion Methods
        }

        /// <summary>
        /// A dlib wrapper.
        /// </summary>
        protected static class DlibWrapper
        {
            /// <summary>
            /// Loads a library.
            /// </summary>
            ///
            /// <param name="fileName"> Filename of the file. </param>
            ///
            /// <returns>
            /// The library.
            /// </returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
            private static extern IntPtr LoadLibrary(string fileName);

            /// <summary>
            /// Gets proc address.
            /// </summary>
            ///
            /// <param name="hModule">  The module. </param>
            /// <param name="procName"> Name of the proc. </param>
            ///
            /// <returns>
            /// The proc address.
            /// </returns>
            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            /// <summary>
            /// The detect landmarks.
            /// </summary>
            internal static DetectLandmarksDelegate DetectLandmarks = null;

            /// <summary>
            /// The set image to bitmap.
            /// </summary>
            internal static SetImageToBmpDelegate SetImageToBmp = null;

            /// <summary>
            /// The set image to RGB.
            /// </summary>
            internal static SetImageToRGBDelegate SetImageToRGB = null;

            /// <summary>
            /// The set image to RGBA.
            /// </summary>
            internal static SetImageToRGBADelegate SetImageToRGBA = null;

            /// <summary>
            /// The detect faces.
            /// </summary>
            internal static DetectFacesDelegate DetectFaces = null;

            /// <summary>
            /// The detect faces.
            /// </summary>
            [Obsolete]
            internal static DetectFacesOldDelegate DetectFacesOld = null;

            /// <summary>
            /// The init database.
            /// </summary>
            internal static InitDatabaseDelegate InitDatabase = null;

            /// <summary>
            /// The init detector.
            /// </summary>
            internal static InitDetectorDelagate InitDetector = null;

            /// <summary>
            /// Handle of the wrapper DLL.
            /// </summary>
            private static IntPtr wrapperDllHandle = IntPtr.Zero;

            /// <summary>
            /// Gets a value indicating whether this object is 64 bit process.
            /// </summary>
            ///
            /// <value>
            /// True if this object is 64 bit process, false if not.
            /// </value>
            private static Boolean Is64BitProcess
            {
                get
                {
                    // Supresses dead code warning and Environment.Is64BitProcess is not supported by .Net 3.5. 
                    // 
                    return IntPtr.Size == 8;
                }
            }

            /// <summary>
            /// Loads a wrapper.
            /// </summary>
            ///
            /// <param name="eda">      The eda. </param>
            /// <param name="dllPath">  Full pathname of the DLL file. </param>
            internal static void LoadWrapper(EmotionDetectionAsset eda, String dllPath)
            {
                if (wrapperDllHandle != IntPtr.Zero)
                {
                    eda.Log(Severity.Warning, "Wrapper dll is already loaded!");

                    return;
                }

                String dllName = String.Empty;

                switch (Is64BitProcess)
                {
                    case true:
                        eda.Log(Severity.Verbose, "Running on a 64 bit Operating System");

                        dllPath = Path.Combine(dllPath, wrapper64);

                        break;

                    case false:
                        eda.Log(Severity.Verbose, "Running on a 32 bit Operating System");

                        dllPath = Path.Combine(dllPath, wrapper32);

                        break;
                }

                wrapperDllHandle = LoadLibrary(dllPath);

                if (wrapperDllHandle != IntPtr.Zero)
                {
                    eda.Log(Severity.Verbose, "Loaded {0} as wrapper", dllPath);

                    //! 1
                    InitDetector = (InitDetectorDelagate)GetDelegate(eda, "InitDetector", typeof(InitDetectorDelagate));

                    //! 2
                    InitDatabase = (InitDatabaseDelegate)GetDelegate(eda, "InitDatabase", typeof(InitDatabaseDelegate));

                    //! 3
                    SetImageToBmp = (SetImageToBmpDelegate)GetDelegate(eda, "SetImageToBmp", typeof(SetImageToBmpDelegate));

                    //! 4
                    SetImageToRGB = (SetImageToRGBDelegate)GetDelegate(eda, "SetImageToRGB", typeof(SetImageToRGBDelegate));

                    //! 5
                    SetImageToRGBA = (SetImageToRGBADelegate)GetDelegate(eda, "SetImageToRGBA", typeof(SetImageToRGBADelegate));

                    //! 6
                    DetectFaces = (DetectFacesDelegate)GetDelegate(eda, "DetectFaces", typeof(DetectFacesDelegate));

                    //! 7
                    DetectLandmarks = (DetectLandmarksDelegate)GetDelegate(eda, "DetectLandmarks", typeof(DetectLandmarksDelegate));

                    //! 8
                    DetectFacesOld = (DetectFacesOldDelegate)GetDelegate(eda, "DetectFacesOld", typeof(DetectFacesOldDelegate));
                }
            }

            /// <summary>
            /// Gets a delegate.
            /// </summary>
            ///
            /// <exception cref="EntryPointNotFoundException">
            /// Thrown when an Entry Point Not Found error
            /// condition occurs. </exception>
            ///
            /// <param name="eda">          The eda. </param>
            /// <param name="procName">     Name of the proc. </param>
            /// <param name="delegateType"> Type of the delegate. </param>
            private static Delegate GetDelegate(EmotionDetectionAsset eda, string procName, Type delegateType)
            {
                IntPtr procAdress = GetProcAddress(wrapperDllHandle, procName);

                if (procAdress == IntPtr.Zero)
                {
                    throw new EntryPointNotFoundException("Wrapper function: " + procName);
                }

                try
                {
                    return Marshal.GetDelegateForFunctionPointer(procAdress, delegateType);
                }
                catch (ArgumentNullException e)
                {
                    eda.Log(Severity.Warning, e.Message);
                }
                catch (ArgumentException e)
                {
                    eda.Log(Severity.Warning, e.Message);
                }

                return null;
            }

            #region Methods

            /// <summary>
            /// Sets Image to Bitmap.
            /// </summary>
            ///
            /// <param name="img">      The image. </param>
            /// <param name="length">   The length. </param>
            ///
            /// <returns>
            /// A bool.
            /// </returns>
            internal delegate bool SetImageToBmpDelegate(
                [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] img,
                Int32 length);

            /// <summary>
            /// Sets Image to RGB Array.
            /// </summary>
            ///
            /// <param name="img">      The image. </param>
            /// <param name="width">    The length. </param>
            /// <param name="height">   The height. </param>
            /// <param name="flip">     True to flip. </param>
            ///
            /// <returns>
            /// A bool.
            /// </returns>
            internal delegate bool SetImageToRGBDelegate(
                [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] img,
                Int32 width,
                Int32 height,
                Boolean flip);

            /// <summary>
            /// Sets Image to RGBA Array.
            /// </summary>
            ///
            /// <param name="img">      The image. </param>
            /// <param name="width">    The length. </param>
            /// <param name="height">   The height. </param>
            /// <param name="flip">     True to flip. </param>
            ///
            /// <returns>
            /// A bool.
            /// </returns>
            internal delegate bool SetImageToRGBADelegate(
                [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U1)] byte[] img,
                Int32 width,
                Int32 height,
                Boolean flip);

            /// <summary>
            /// Detect faces.
            /// </summary>
            ///
            /// <param name="faces">        [out] The faces. </param>
            /// <param name="facecount">    [out] The facecount. </param>
            internal delegate void DetectFacesDelegate(
                out IntPtr faces,
                out int facecount);

            /// <summary>
            /// Detect faces.
            /// </summary>
            ///
            /// <param name="img">          The image. </param>
            /// <param name="length">       The length. </param>
            /// <param name="faces">        [out] The faces. </param>
            /// <param name="facecount">    [out] The facecount. </param>
            internal delegate void DetectFacesOldDelegate(
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
            //[DllImport(wrapper,
            internal delegate void DetectLandmarksDelegate(RECT face, out IntPtr landmarks, out int markcount);

            /// <summary>
            /// Init database.
            /// </summary>
            ///
            /// <param name="lpFileName">   Filename of the file. </param>
            ///
            /// <returns>
            /// An int.
            /// </returns>
            internal delegate int InitDatabaseDelegate([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

            /// <summary>
            /// Init detector.
            /// </summary>
            internal delegate void InitDetectorDelagate();

            #endregion Methods
        }

        /// <summary>
        /// Additional information for emotion events.
        /// </summary>
        public class EmotionEventArgs
        {
            public Int32 face;
            public Double value;
        }

        #endregion Nested Types
    }
}

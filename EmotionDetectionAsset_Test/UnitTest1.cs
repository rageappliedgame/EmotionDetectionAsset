using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using AssetManagerPackage;
using AssetPackage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static AssetPackage.EmotionDetectionAsset;

namespace EmotionDetectionAsset_Test
{
    [TestClass]
    public class UnitTest1
    {
        String expr = "(V30 in [159.608, 160.424, inf, inf]) and (V35 in [30.0655, 30.2536, inf, inf]) => Emotions=Happy (CF = 0.97)";

        public class Bridge : IBridge, ILog
        {
            public void Log(Severity severity, string msg)
            {
                Debug.WriteLine(msg);
            }
        }

        [TestInitialize]
        public void Setup()
        {
            Debug.WriteLine(String.Empty);

            AssetManager.Instance.Bridge = new Bridge();
        }

        [TestCleanup]
        public void Cleanup()
        {
            //
        }

        [TestMethod]
        [TestCategory("Parsing")]
        public void TestMethod3()
        {
            Debug.WriteLine("[TestMethod3]");

            EmotionDetectionAsset eda = new EmotionDetectionAsset();

            foreach (String line in File.ReadAllLines(@"..\..\..\database\FURIA Fuzzy Logic Rules.txt"))
            {
                if (eda.ParseRule(line))
                {

                    // Output:
                    //Emotions = Happy(CF = 0.97)
                    //(V30 in [159.608, 160.424, inf, inf])
                    //(V35 in [30.0655, 30.2536, inf, inf])

                    //eda.CheckValues(eda.expressions.Skip(0).First(), "V30", new Double[] { 20, 160, 180 });
                    //eda.CheckValues(eda.expressions.Skip(0).First(), "V35", new Double[] { 20, 30.0655, 30.0656, 30.1, 30.2535, 30.2536, 32 });
                }
            }

            Debug.Print("{0} Expressions", eda.expressions.Count);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Debug.WriteLine("[TestMethod4]");

            EmotionDetectionAsset eda = new EmotionDetectionAsset();
            // http://stackoverflow.com/questions/13605013/pass-bitmap-from-c-sharp-to-c
            // https://msdn.microsoft.com/en-us/library/vs/alm/dd183402(v=vs.85).aspx
            // 

            eda.Initialize(@".\shape_predictor_68_face_landmarks.dat");

            eda.ParseRules(File.ReadAllLines(@".\FURIA Fuzzy Logic Rules.txt"));

            if (eda.ProcessImage((Bitmap)Bitmap.FromFile(@".\Kiavash1.jpg")))
            {
                Debug.WriteLine(String.Format("{0} Face(s detected.", eda.Faces.Count));

                if (eda.ProcessFaces())
                {
                    Int32 i = 1;
                    foreach (KeyValuePair<RECT, List<POINT>> kvp in eda.Faces)
                    {
                        Debug.WriteLine(String.Format("{0} Landmark(s) detected in Face {1} at {2}.", kvp.Value.Count, i++, kvp.Key));
                    }

                    if (eda.ProcessLandmarks())
                    {

                        //! Not coded yet.
                    }
                }
            }
        }
    }
}
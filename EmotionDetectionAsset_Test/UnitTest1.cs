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

namespace EmotionDetectionAsset_Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using AssetManagerPackage;
    using AssetPackage;

    using static AssetPackage.EmotionDetectionAsset;

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

            if (eda.ParseRules(File.ReadAllLines(@"..\..\..\database\FURIA Fuzzy Logic Rules.txt")))
            {

                // Output:
                //Emotions = Happy(CF = 0.97)
                //(V30 in [159.608, 160.424, inf, inf])
                //(V35 in [30.0655, 30.2536, inf, inf])

                //eda.CheckValues(eda.expressions.Skip(0).First(), "V30", new Double[] { 20, 160, 180 });
                //eda.CheckValues(eda.expressions.Skip(0).First(), "V35", new Double[] { 20, 30.0655, 30.0656, 30.1, 30.2535, 30.2536, 32 });
            }

            Debug.Print("{0} Emotions found in Rules", eda.Emotions.Count);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Debug.WriteLine("[TestMethod4]");

            EmotionDetectionAsset eda = new EmotionDetectionAsset();
            // http://stackoverflow.com/questions/13605013/pass-bitmap-from-c-sharp-to-c
            // https://msdn.microsoft.com/en-us/library/vs/alm/dd183402(v=vs.85).aspx
            // 

            eda.Initialize(@".", "shape_predictor_68_face_landmarks.dat");

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
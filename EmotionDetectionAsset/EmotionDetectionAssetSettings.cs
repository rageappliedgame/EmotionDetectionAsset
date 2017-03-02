/*
 * Copyright 2016 Open University of the Netherlands
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * This project has received funding from the European Union�s Horizon
 * 2020 research and innovation programme under grant agreement No 644187.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
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
    using System.ComponentModel;

    /// <summary>
    /// An asset settings.
    /// 
    /// BaseSettings contains the (de-)serialization methods.
    /// </summary>
    public class EmotionDetectionAssetSettings : BaseSettings
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the EmotionDetectionAsset.AssetSettings class.
        /// </summary>
        public EmotionDetectionAssetSettings()
            : base()
        {
            // Set Default values here.
            Database = @"shape_predictor_68_face_landmarks.dat";
            Rules = @"FURIA Fuzzy Logic Rules.txt";
            GrayScale = false;
            Average = 5;
            SuppressSpikes = false;
            SpikeAmplitude = 0.25;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        ///
        /// <value>
        /// The database.
        /// </value>
        [Description("The DLib landmarks database.")]
        [Category("Setup")]
        public String Database
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the rules.
        /// </summary>
        ///
        /// <value>
        /// The rules.
        /// </value>
        [Description("The FURIA rules to be parsed.")]
        [Category("Setup")]
        public String Rules
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the gray scale.
        /// </summary>
        ///
        /// <value>
        /// true if gray scale, false if not.
        /// </value>
        [Description("If true, input images are converted to graysscale before further processing.")]
        [Category("Config")]
        [DefaultValue(false)]
        public Boolean GrayScale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of detections to average.
        /// </summary>
        ///
        /// <remarks>
        /// The minumum value is 1.
        /// </remarks>
        ///
        /// <value>
        /// The number of emotions to average.
        /// </value>
        [Description("The number of detections to average. The minumum value is 1.")]
        [Category("Config")]
        [DefaultValue(5)]
        public Int32 Average
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the suppress spikes.
        /// </summary>
        ///
        /// <remarks>
        /// If set to true, two extra sets of historical data are added to the history to be able to
        /// perform filtering outside averaged data in the EmotionDetectionAsset [face,emotion] indexer.
        /// </remarks>
        ///
        /// <value>
        /// true if remove spikes, false if not.
        /// </value>
        [Description("Enable or disable Spike Suppression ")]
        [Category("Config")]
        [DefaultValue(false)]
        public bool SuppressSpikes
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the spike amplitude.
        /// </summary>
        ///
        /// <value>
        /// The spike amplitude.
        /// </value>
        [Description("The Amplitude to conside a value a spike ")]
        [Category("Config")]
        [DefaultValue(0.25)]
        public double SpikeAmplitude
        {
            get;
            set;
        }

//#warning FIR parameters.

#warning Dlib wrapper filename (if we dynload it).

#warning Scaled etcs (basic image pre-processing).

        #endregion Properties
    }
}

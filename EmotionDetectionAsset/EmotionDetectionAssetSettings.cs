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

#warning FIR paramaters.

#warning Dlib wrapper filename (if we dynload it).

#warning Scaled etcs (basic image pre-processing).

        #endregion Properties
    }
}

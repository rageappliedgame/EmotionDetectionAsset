namespace AssetPackage
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

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
        }

        #endregion Constructors

        #region Properties

#warning FIR paramaters.

#warning FURIA rules filename.

#warning Dlib database filename.

#warning Dlib wrapper filename (if we dynload it).

#warning EMOTION SETTINGS.

#warning Grayscale, Scaled etcs (basic image pre-processing).

        #endregion Properties
    }
}

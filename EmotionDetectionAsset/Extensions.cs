namespace AssetPackage
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// An extension class.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// A Bitmap extension method that converts an image to a byte array.
        /// </summary>
        ///
        /// <param name="image">    The image to act on. </param>
        ///
        /// <returns>
        /// image as a byte[].
        /// </returns>
        public static byte[] ToByteArray(this Bitmap image)
        {
            using (var ms = new MemoryStream())
            {
                //! Save and convert.
                //! 
                //! Bitmap is currently one of the few supported format by dlib.
                //! 
                //! Dlib's supports the following PixelFormat values:
                //! 
                //! Format1bppIndexed, Format4bppIndexed, Format8bppIndexed and Format24bppRgb. 
                image.Save(ms, ImageFormat.Bmp);

                //! this also works.
                //image.Save(ms, image.RawFormat);

                return ms.ToArray();
            }
        }
    }
}

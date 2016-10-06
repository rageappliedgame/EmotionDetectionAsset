namespace AssetPackage
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// An extensions.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Static constructor.
        /// </summary>
        static Extensions()
        {
            //! Set the color matrix attribute.
            // 
            ia.SetColorMatrix(cm);
        }

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
                image.Save(ms, ImageFormat.Bmp);

                //! Also works.
                //image.Save(ms, image.RawFormat);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Create the grayscale ColorMatrix.
        /// </summary>
        private static ColorMatrix cm = new ColorMatrix(
                new float[][]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

        /// <summary>
        /// ! Image attributes to be set with the colorMatrix.
        /// </summary>
        private static ImageAttributes ia = new ImageAttributes();

        /// <summary>
        /// Convert to GrayScale.
        /// </summary>
        ///
        /// <remarks>
        /// Adapted from: http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-
        /// to-grayscale.
        /// </remarks>
        ///
        /// <param name="img">  The image. </param>
        ///
        /// <returns>
        /// A Bitmap.
        /// </returns>
        public static Bitmap GrayScale(this Image img)
        {
            //! Create a blank bitmap the same size as original.
            // 
            Bitmap newBitmap = new Bitmap(img.Width, img.Height);

            //! Get a graphics object from the new image.
            // 
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                //! Draw the original image on the new image using the grayscale color matrix.
                // 
                g.DrawImage(
                    img,
                    new Rectangle(0, 0, img.Width, img.Height),
                    0, 0, img.Width, img.Height,
                    GraphicsUnit.Pixel, ia);
            }

            return newBitmap;
        }
    }
}

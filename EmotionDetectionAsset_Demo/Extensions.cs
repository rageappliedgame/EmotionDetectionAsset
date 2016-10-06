using System;
namespace dlib_csharp
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// An extensions.
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
                image.Save(ms, ImageFormat.Bmp);
                //image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Sets double buffered.
        /// </summary>
        ///
        /// <param name="control">  The control. </param>
        /// <param name="value">    true to value. </param>
        public static void SetDoubleBuffered(this Control control, Boolean value)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { value });
        }
    }
}

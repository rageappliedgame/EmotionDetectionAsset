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

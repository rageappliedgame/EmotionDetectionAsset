
namespace AssetPackage
{
    using System;
    using System.Reflection;
    using System.Windows.Forms;

    /// <summary>
    /// An extensions.
    /// </summary>
    public static class Extensions
    {
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

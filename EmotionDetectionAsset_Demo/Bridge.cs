namespace dlib_csharp
{
    using System.Diagnostics;
    using AssetPackage;

    /// <summary>
    /// A bridge.
    /// </summary>
    public class Bridge : IBridge, ILog
    {
        public void Log(Severity severity, string msg)
        {
            Debug.WriteLine(msg);
        }
    }

}

using System;

namespace GoogleMaps.Net.Clustering.Utility
{
    internal static class ExceptionUtil
    {
        public static string GetException(Exception ex)
        {
            return String.Format("Msg:{0}\nStacktrace:{1}\nInnerExc:{2}", ex.Message, ex.StackTrace, ex.InnerException);
        }
    }
}

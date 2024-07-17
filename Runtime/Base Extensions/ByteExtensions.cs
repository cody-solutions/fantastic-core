using System;

namespace FantasticCore.Runtime.Base_Extensions
{
    public static class ByteExtensions
    {
        /// <summary>
        /// Format bytes to string with relevant size
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string FormatDownloadBytes(this long bytes)
        {
            const int scale = 1024;
            string[] orders = { "Gb", "Mb", "Kb", "B" };
            long max = (long)Math.Pow(scale, orders.Length - 1);
            foreach (string order in orders)
            {
                if (bytes > max)
                {
                    return $"{decimal.Divide(bytes, max):##.##} {order}";
                }

                max /= scale;
            }
            
            return "0 B";
        }
    }
}
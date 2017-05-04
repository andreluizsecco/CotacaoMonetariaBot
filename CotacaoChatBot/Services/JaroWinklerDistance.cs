using System;
using System.Linq;

namespace CotacaoChatBot.Services
{
    /// <summary>
    /// Código retirado de: http://lucenenet.apache.org/docs/3.0.3/db/d12/_jaro_winkler_distance_8cs_source.html
    /// </summary>
    [Serializable]
    public class JaroWinklerDistance
    {
        private static float threshold = 0.7f;

        private static int[] Matches(string s1, string s2)
        {
            string Max, Min;

            if (s1.Length > s2.Length)
            {
                Max = s1;
                Min = s2;
            }
            else
            {
                Max = s2;
                Min = s1;
            }

            var range = Math.Max(Max.Length / 2 - 1, 0);
            var matchIndexes = new int[Min.Length];

            for (var i = 0; i < matchIndexes.Length; i++)
                matchIndexes[i] = -1;

            var matchFlags = new bool[Max.Length];
            var matches = 0;

            for (var mi = 0; mi < Min.Length; mi++)
            {
                var c1 = Min[mi];
                for (int xi = Math.Max(mi - range, 0),
                xn = Math.Min(mi + range + 1, Max.Length); xi < xn; xi++)
                {
                    if (matchFlags[xi] || c1 != Max[xi]) continue;

                    matchIndexes[mi] = xi;
                    matchFlags[xi] = true;
                    matches++;
                    break;
                }
            }

            var ms1 = new char[matches];
            var ms2 = new char[matches];

            for (int i = 0, si = 0; i < Min.Length; i++)
            {
                if (matchIndexes[i] != -1)
                {
                    ms1[si] = Min[i];
                    si++;
                }
            }

            for (int i = 0, si = 0; i < Max.Length; i++)
            {
                if (matchFlags[i])
                {
                    ms2[si] = Max[i];
                    si++;
                }
            }

            var transpositions = ms1.Where((t, mi) => t != ms2[mi]).Count();

            var prefix = 0;
            for (var mi = 0; mi < Min.Length; mi++)
            {
                if (s1[mi] == s2[mi])
                    prefix++;
                else
                    break;
            }

            return new int[] { matches, transpositions / 2, prefix, Max.Length };
        }

        public static float GetDistance(string s1, string s2)
        {
            var mtp = Matches(s1, s2);
            var m = (float)mtp[0];

            if (m == 0)
                return 0f;

            float j = ((m / s1.Length + m / s2.Length + (m - mtp[1]) / m)) / 3;
            float jw = j < Threshold ? j : j + Math.Min(0.1f, 1f / mtp[3]) * mtp[2] * (1 - j);
            return jw;
        }

        /// <summary>
        /// Gets or sets the current value of the threshold used for adding the Winkler bonus.
        /// Set to a negative value to get the Jaro distance. The default value is 0.7.
        /// </summary>
        public static float Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }
    }
}
﻿using System;

namespace Maths
{
    public static class StringDistance
    {

        public static int Compute(String first, String second)
        {
            if (first == null || first.Length == 0 )
            {
                return 0;
            }

            if (second == null || second.Length == 0)
            {
                return 0;
            }

            var d = new int[first.Length + 1, second.Length + 1];
            for (var i = 0; i <= first.Length; i++)
            {
                d[i, 0] = 0;
            }

            for (var j = 0; j <= second.Length; j++)
            {
                d[0, j] = 0;
            }

            for (var i = 1; i <= first.Length; i++)
            {
                for (var j = 1; j <= second.Length; j++)
                {
                    var cost = (second[j - 1] == first[i - 1]) ? 1 : 0;
                    d[i, j] = Max(
                         d[i - 1, j],
                         d[i, j - 1],
                         d[i - 1, j - 1] + cost
                    );
                }
            }
            return d[first.Length, second.Length];
        }

        private static int Max(int e1, int e2, int e3)
        {
            return Math.Max(Math.Max(e1, e2), e3);
        }
        
     
       
    }
}

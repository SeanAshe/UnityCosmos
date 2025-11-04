using System;

namespace Cosmos.Math
{
    public static class MathUtility
    {
        public static double FindRootBisection(Func<double, double> function, double lowerBound, double upperBound, double tolerance = 1e-7, int maxIterations = 100)
        {
            double fLower = function(lowerBound);
            double fUpper = function(upperBound);

            if (fLower * fUpper >= 0)
            {
                return (lowerBound + upperBound) / 2; // 返回一个大概的值
            }

            double midpoint = 0;
            for (int i = 0; i < maxIterations; i++)
            {
                midpoint = (lowerBound + upperBound) / 2.0;
                double fMid = function(midpoint);

                // 检查是否足够接近0
                if (global::System.Math.Abs(fMid) < tolerance || (upperBound - lowerBound) / 2.0 < tolerance)
                {
                    return midpoint;
                }

                // 根据中点值的符号，缩小搜索区间
                if (global::System.Math.Sign(fMid) == global::System.Math.Sign(fLower))
                {
                    lowerBound = midpoint;
                    fLower = fMid;
                }
                else
                {
                    upperBound = midpoint;
                }
            }

            return midpoint; // 返回当前找到的最佳近似值
        }
    }
}

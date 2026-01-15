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

        /// <summary>
        /// 使用牛顿迭代法求解 f(x) = 0
        /// 不需要区间两端符号相反，只需要一个初始猜测值
        /// </summary>
        /// <param name="f">目标函数</param>
        /// <param name="initialGuess">初始猜测值 (例如 0.5)</param>
        /// <param name="tolerance">目标精度 (例如 1e-9)</param>
        /// <param name="maxIterations">最大迭代次数</param>
        /// <returns>方程的根</returns>
        public static double FindRootNewton(Func<double, double> f, double initialGuess, double tolerance = 1e-9, int maxIterations = 100)
        {
            double x = initialGuess;

            for (int i = 0; i < maxIterations; i++)
            {
                double y = f(x);

                // 1. 检查是否已经找到根
                if (global::System.Math.Abs(y) < tolerance)
                {
                    return x;
                }

                // 2. 计算数值导数 (斜率)
                // f'(x) ≈ [f(x + h) - f(x - h)] / 2h
                double h = 1e-5; // 微小的步长
                double slope = (f(x + h) - f(x - h)) / (2 * h);

                // 3. 安全检查：如果斜率太小 (接近水平)，导数不仅不可靠，还会导致除以0飞出天际
                if (global::System.Math.Abs(slope) < 1e-12)
                {
                    x += 1.0; // 强制跳跃一下，防止死循环
                    continue;
                }

                // 4. 牛顿迭代公式： x_new = x_old - f(x) / f'(x)
                double nextX = x - (y / slope);

                // (可选) 如果你想限制 x 不跑出特定范围（比如概率不能是负数），可以在这里加 Clamp
                // if (nextX < 0) nextX = 0.0001;
                // if (nextX > 1) nextX = 0.9999;

                // 5. 检查变化量是否极小 (收敛了)
                if (global::System.Math.Abs(nextX - x) < tolerance)
                {
                    return nextX;
                }

                x = nextX;
            }
            return x;
        }
    }
}

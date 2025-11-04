using System;
using System.Collections.Generic;
using Cosmos.System;


namespace Cosmos.Math
{
    //动态概率
    public class PRDCalculator
    {
        private readonly float tagetRate;
        private readonly float C; // 核心常数
        private readonly Random random;
        private int attackCounter;
        public float CurrentRate => global::System.Math.Min(C * attackCounter, 1.0f);
        public PRDCalculator(float targetProbability, int? seed = null)
        {
            if (targetProbability <= 0 || targetProbability >= 1) return;

            tagetRate = targetProbability;

            int index = (int)tagetRate * 100;
            if (tagetRate * 100 - index != 0 || !preciseValues.IsIndexValid(index))
                C = CalculateCForProbability(targetProbability);
            else
                C = (float)preciseValues[index];

            attackCounter = 1;
            random = seed == null ? new Random() : new Random(seed.Value);
        }
        public bool Roll()
        {
            if (random.NextDouble() < CurrentRate)
            {
                Reset();
                return true;
            }
            else
            {
                attackCounter++;
                return false;
            }
        }
        public void Reset()
        {
            attackCounter = 1;
        }

        private static readonly List<double> preciseValues = new()
        {
            0.0001560315, 0.0006201128, 0.001386151, 0.002448543, 0.003801686, 0.005440097, 0.00735865, 0.009552456, 0.01201639, 0.0147458,
            0.01773628, 0.02098319, 0.02448245, 0.02822967, 0.03222091, 0.03645224, 0.04091996, 0.04562014, 0.05054932, 0.05570405,
            0.06108086, 0.06667642, 0.0724875, 0.07851115, 0.08474412, 0.09118344, 0.09782637, 0.1046702, 0.1117117, 0.1189492,
            0.1263794, 0.1340009, 0.1418053, 0.1498101, 0.1579831, 0.1663288, 0.1749093, 0.1836247, 0.192486, 0.2015474,
            0.21092, 0.2203646, 0.2298987, 0.2395401, 0.249307, 0.2598723, 0.2704529, 0.2810076, 0.2915523, 0.302103,
            0.3126766, 0.3232906, 0.3341199, 0.34737, 0.3603978, 0.3732168, 0.3858396, 0.3982783, 0.4105446, 0.4226497,
            0.4346045, 0.4464193, 0.4581044, 0.4696699, 0.4811254, 0.4924808, 0.5074627, 0.5294118, 0.5507247, 0.5714285,
            0.5915493, 0.6111111, 0.6301371, 0.6486486, 0.6666666, 0.6842105, 0.7012987, 0.7179487, 0.7341772, 0.75,
            0.7654321, 0.7804878, 0.7951807, 0.8095238, 0.8235294, 0.8372094, 0.8505747, 0.8636364, 0.8764044, 0.888889,
            0.901099, 0.9130436, 0.9247311, 0.9361702, 0.9473684, 0.9583333, 0.9690722, 0.9795918, 0.9898989
        };
        private static float CalculateCForProbability(float targetProbability)
        {
            double targetHits = 1.0 / targetProbability;

            // 定义一个函数，其根是我们想要找的 C 值
            // f(c) = GetExpectedHits(c) - targetHits
            double func(double c) => GetExpectedHits(c) - targetHits;

            // 使用二分法在 (0, 1] 区间内寻找根
            // 返回值需要从 double 转回 float
            return (float)MathUtility.FindRootBisection(func, 0.000001, 1.0);
        }
        private static double GetExpectedHits(double c)
        {
            double expectedValue = 0;
            double probNotCritYet = 1.0;

            // 设定一个足够大的计算上限，避免无限循环
            int maxAttempts = (int)global::System.Math.Ceiling(2f / c) + 5;

            for (int n = 1; n < maxAttempts; n++)
            {
                double critChanceAtN = global::System.Math.Min(c * n, 1.0);

                // 第一次暴击恰好发生在第 n 次的概率
                double probFirstCritAtN = probNotCritYet * critChanceAtN;

                // 累加到期望值中
                expectedValue += n * probFirstCritAtN;

                // 更新“至今未暴击”的概率
                probNotCritYet *= 1.0 - critChanceAtN;

                // 如果概率已经低到可以忽略，提前退出以提高效率
                if (probNotCritYet < 1e-12)
                {
                    break;
                }
            }
            return expectedValue;
        }
    }

    public class PRDCalculatorNonLinear
    {
        private readonly float tagetRate;
        private readonly float curveCenter; // N_half
        private readonly float k; // 陡峭度参数
        private readonly Random random;
        private int attackCounter;

        /// <summary>
        /// 获取下一次攻击的实际暴击率（遵循S型曲线）。
        /// </summary>
        public float CurrentRate
        {
            get
            {
                // P(N) = 1 / (1 + exp(-k * (N - N_half)))
                double value = 1.0 / (1.0 + global::System.Math.Exp(-k * (attackCounter - curveCenter)));
                return (float)value;
            }
        }

        /// <summary>
        /// 使用目标概率和曲线中心点来初始化非线性计算器。
        /// </summary>
        /// <param name="targetProbability">期望的平均概率 (0, 1)。</param>
        /// <param name="curveCenterPoint">S型曲线的中心点 (N_half)，即概率达到50%时的攻击次数。</param>
        public PRDCalculatorNonLinear(float targetProbability, float curveCenterPoint, int? seed = null)
        {
            if (targetProbability <= 0 || targetProbability >= 1)
                throw new ArgumentOutOfRangeException(nameof(targetProbability), "目标概率必须在 (0, 1) 之间。");
            if (curveCenterPoint <= 0)
                throw new ArgumentOutOfRangeException(nameof(curveCenterPoint), "曲线中心点必须大于0。");

            tagetRate = targetProbability;
            curveCenter = curveCenterPoint;

            // 核心步骤：求解参数 k
            k = CalculateSteepness(targetProbability, curveCenterPoint);
            attackCounter = 1;
            random = seed == null ? new Random() : new Random(seed.Value);

        }

        public bool Roll()
        {
            if (random.NextDouble() < CurrentRate)
            {
                Reset();
                return true;
            }
            else
            {
                attackCounter++;
                return false;
            }
        }
        public void Reset()
        {
            attackCounter = 1;
        }

        private static float CalculateSteepness(float targetProbability, float n_half)
        {
            double targetHits = 1.0 / targetProbability;

            // 目标函数: f(k) = GetExpectedHits(k, n_half) - targetHits
            double func(double k) => GetExpectedHits(k, n_half) - targetHits;

            // 使用二分法寻找陡峭度 k。k 的合理范围可以从一个很小的值到5或10。
            return (float)MathUtility.FindRootBisection(func, 0.01, 10.0);
        }

        private static double GetExpectedHits(double k, double n_half)
        {
            double expectedValue = 0;
            double probNotCritYet = 1.0;

            // S型曲线收敛到1的速度可能较慢，需要足够大的上限
            int maxAttempts = (int)global::System.Math.Ceiling(n_half) * 2 + 20;

            for (int n = 1; n < maxAttempts; n++)
            {
                // 使用Sigmoid函数计算当前概率
                double critChanceAtN = 1.0 / (1.0 + global::System.Math.Exp(-k * (n - n_half)));

                double probFirstCritAtN = probNotCritYet * critChanceAtN;
                expectedValue += n * probFirstCritAtN;
                probNotCritYet *= 1.0 - critChanceAtN;

                if (probNotCritYet < 1e-12) break;
            }
            return expectedValue;
        }
    }
}
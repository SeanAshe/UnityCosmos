using System;
using System.Collections.Generic;
using Cosmos.System;


namespace Cosmos.Math
{
    public class PRDPlusCalculator
    {
        private readonly float tagetRate;
        private readonly int max;
        private readonly int startPos;
        public readonly float baseProb;
        private readonly Random random;
        private readonly double[] probs;
        private int counter;
        public PRDPlusCalculator(float targetProbability, int n, int start, int? seed = null)
        {
            if (targetProbability <= 0 || targetProbability >= 1) return;
            tagetRate = targetProbability;
            max = n;
            startPos = start;
            counter = 0;
            random = seed == null ? new Random() : new Random(seed.Value);
            baseProb = CalculateBaseProb(tagetRate, max, startPos);
            probs = GenerateProbabilityTable(max, baseProb, startPos);
        }

        public float CurrentRate => (float)global::System.Math.Min(probs[counter], 1.0);
        public bool Roll()
        {
            if (random.NextDouble() < CurrentRate)
            {
                Reset();
                return true;
            }
            else
            {
                counter++;
                return false;
            }
        }
        public void Reset()
        {
            counter = 0;
        }

        private static float CalculateBaseProb(float tagetRate, int max, int startPos)
        {
            double func(double x)
            {
                var probs = GenerateProbabilityTable(max, x, startPos);
                return CalculateOverallAndVerify(probs) - tagetRate;
            }
            // 返回值需要从 double 转回 float
            return (float)MathUtility.FindRootNewton(func, 0.0001, 1e-09, 1000);
        }
        public static double[] GenerateProbabilityTable(int n, double baseProb, int startPos)
        {
            double[] probs = new double[n];

            // 线性增长区间的长度
            // 例如 n=80, start=61. 区间是 61..80，共 20步
            double rampLength = n - startPos + 1;

            for (int i = 0; i < n; i++)
            {
                int currentStep = i + 1; // 转换为 1-based 用于逻辑判断

                if (currentStep < startPos)
                {
                    // 平稳期
                    probs[i] = baseProb;
                }
                else
                {
                    // 线性增长期
                    // 已经是最后一次吗？直接设为1防止浮点误差
                    if (currentStep == n)
                    {
                        probs[i] = 1.0;
                    }
                    else
                    {
                        // 线性插值公式
                        // 进度从 1 到 rampLength
                        double progress = currentStep - startPos + 1;

                        // 我们希望在 rampLength (即第n次) 时达到 1.0
                        // 这是一个简单的线性函数： y = kx + b
                        // 起点 (0进度) 实际上是 baseProb，终点是 1.0
                        // 这里的插值逻辑：
                        // 当 progress = 1 (刚进入起涨点), 概率稍微提升一点，或者保持 baseProb?
                        // 通常做法：均匀切分 (1 - baseProb) 为 rampLength 份

                        double increment = (1.0 - baseProb) / rampLength;
                        probs[i] = baseProb + (increment * progress);

                        // 确保不越界
                        if (probs[i] > 1.0) probs[i] = 1.0;
                    }
                }
            }
            return probs;
        }
        public static double CalculateOverallAndVerify(double[] probs)
        {
            double expectedValue = 0.0;
            double survivalRate = 1.0; // 还没抽到的概率

            for (int i = 0; i < probs.Length; i++)
            {
                double p = probs[i];
                double probabilityOfWinningHere = survivalRate * p;

                // 期望步数 += 当前步数 * 在当前步获胜的绝对概率
                expectedValue += (i + 1) * probabilityOfWinningHere;

                survivalRate *= 1.0 - p;

                if (survivalRate < 1e-12) break;
            }
            return 1.0 / expectedValue;
        }
    }
}
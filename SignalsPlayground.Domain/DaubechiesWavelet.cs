using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace SignalsPlayground.Domain
{
    public class DaubechiesWavelet
    {
        /// <summary>
        /// Gets the Debauchies scaling function
        /// </summary>
        /// <param name="x">Array of x-coordinates to map to scaling function</param>
        /// <param name="levels">Levels of approximatation</param>
        /// <returns></returns>
        public IEnumerable<Vector2[]> GetScalingLevels(int levels, WaveletKind wavelet)
        {
            if (levels < 0)
                throw new ArgumentOutOfRangeException($"{nameof(GetScalingLevels)} parameter {nameof(levels)} ({levels}) must be 0 or larger");

            var coefficients = WaveletCoefficients.GetScalingCoefficients(wavelet).ToArray();
            var previous = WaveletCoefficients.GetInitialScalingValues(wavelet).ToArray().Select((x, index) => new Vector2(index, x)).ToArray();

            float maxRange = coefficients.Length - 1f;

            var current = previous;
            yield return previous;

            for (int level = 1; level <= levels; level++)
            {
                current = new Vector2[PointsAtLevel(coefficients.Length, level)];

                for (int j = 0; j < current.Length; j += 2)
                    current[j] = previous[j / 2];

                for (int j = 1; j < current.Length; j += 2)
                {
                    float sum = 0;
                    float r = j / (float)(1 << level);

                    for (int k = 0; k < coefficients.Length; k++)
                    {
                        float distancePerPoint = maxRange / (current.Length - 1);
                        float rprev = 2 * r - k;
                        int rIndex = (int)MathF.Round(rprev / distancePerPoint);

                        if (rprev >= 0 && rprev <= maxRange)
                            sum += coefficients[k] * current[rIndex].Y;
                    }
                    current[j] = new Vector2(r, sum);
                }

                yield return current;
                previous = current;
            }
        }

        /// <summary>
        /// Gets the Debauchies scaling function
        /// </summary>
        /// <param name="levels">Levels of approximatation</param>
        /// <param name="waveletKind">Wavelet kind to calculate</param>
        /// <returns></returns>
        public Vector2[] GetWaveletLevel(int levels, WaveletKind waveletKind)
        {
            if (levels < 0)
                throw new ArgumentOutOfRangeException($"{nameof(GetScalingLevels)} parameter {nameof(levels)} ({levels}) must be 0 or larger");

            var scaling = GetScalingLevels(levels, waveletKind).Last().ToList();

            var coefficients = WaveletCoefficients.GetWaveletCoefficients(waveletKind);

            var current = new Vector2[PointsAtLevel(coefficients.Length, levels)];

            float maxRange = coefficients.Length - 1f;
            float distancePerPoint = maxRange / (current.Length - 1);

            for (int j = 0; j < current.Length; j++)
            {
                float sum = 0;
                float r = j * distancePerPoint;

                for (int k = 0; k < coefficients.Length; k++)
                {
                    float rprev = 2 * r - k;
                    int rIndex = (int)MathF.Round(rprev / distancePerPoint);

                    if (rprev >= 0 && rprev <= maxRange)
                        sum += coefficients[k] * scaling[rIndex].Y;
                }
                current[j] = new Vector2(r, sum);
            }

            return current;
        }

        public int GetMaxRange(WaveletKind waveletKind) =>
            WaveletCoefficients.GetScalingCoefficients(waveletKind).Length - 1;

        private int PointsAtLevel(int zeroLevelPoints, int level)
        {
            if (level < 0)
                throw new ArgumentOutOfRangeException($"{nameof(PointsAtLevel)} was called with {nameof(level)} ({level})");
            else if (level == 0)
                return zeroLevelPoints;
            else
                return zeroLevelPoints * (1 << level) - ((1 << level) - 1);
        }
    }
}

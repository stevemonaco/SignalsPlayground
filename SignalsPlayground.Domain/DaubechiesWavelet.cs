using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SignalsPlayground.Domain
{
    public class DaubechiesWavelet
    {
        /// <summary>
        /// Gets the Debauchies scaling function
        /// </summary>
        /// <param name="x">Array of x-coordinates to map to scaling function</param>
        /// <param name="levels">Levels of approximatation</param>
        /// <returns>The scaling function at successive levels of approximation</returns>
        public IEnumerable<Vector2[]> GetScalingFunction(int levels, WaveletKind wavelet)
        {
            if (levels < 0)
                throw new ArgumentOutOfRangeException($"{nameof(GetScalingFunction)} parameter {nameof(levels)} ({levels}) must be 0 or larger");

            var coefficients = WaveletCoefficients.GetScalingCoefficients(wavelet).ToArray();
            var previous = WaveletCoefficients.GetInitialScalingValues(wavelet).ToArray().Select((x, index) => new Vector2(index, x)).ToArray();

            float maxRange = coefficients.Length - 1f;

            var current = previous;
            yield return previous;

            for (int level = 1; level <= levels; level++)
            {
                current = GetScalingLevel(level, previous, coefficients);

                yield return current;
                previous = current;
            }

            Vector2[] GetScalingLevel(int level, Vector2[] previous, float[] scalingCoefs)
            {
                var scaling = new Vector2[PointsAtLevel(scalingCoefs.Length, level)];
                float maxRange = scalingCoefs.Length - 1f;
                float distancePerIndex = maxRange / (scaling.Length - 1);

                for (int n = 0; n < scaling.Length; n += 2)
                    scaling[n] = previous[n / 2];

                for (int n = 1; n < scaling.Length; n += 2)
                {
                    float sum = 0;
                    float r = n / (float)(1 << level);

                    for (int k = 0; k < scalingCoefs.Length; k++)
                    {
                        float rPrevious = 2 * r - k;
                        int rIndex = (int)MathF.Round(rPrevious / distancePerIndex);

                        if (rPrevious >= 0 && rPrevious <= maxRange)
                            sum += scalingCoefs[k] * scaling[rIndex].Y;
                    }
                    scaling[n] = new Vector2(r, sum);
                }

                return scaling;
            }
        }

        /// <summary>
        /// Gets the Debauchies scaling function
        /// </summary>
        /// <param name="levels">Levels of approximatation</param>
        /// <param name="waveletKind">Wavelet kind to calculate</param>
        /// <returns>The wavelet function at the specified level of approximation</returns>
        public Vector2[] GetWaveletFunction(int levels, WaveletKind waveletKind)
        {
            if (levels < 0)
                throw new ArgumentOutOfRangeException($"{nameof(GetScalingFunction)} parameter {nameof(levels)} ({levels}) must be 0 or larger");

            var scaling = GetScalingFunction(levels, waveletKind).Last().ToArray();
            var coefficients = WaveletCoefficients.GetWaveletCoefficients(waveletKind).ToArray();

            return GetWaveletLevel(scaling, coefficients);

            Vector2[] GetWaveletLevel(Vector2[] scaling, float[] waveletCoefs)
            {
                var wavelet = new Vector2[scaling.Length];

                float maxRange = waveletCoefs.Length - 1f;
                float distancePerPoint = maxRange / (wavelet.Length - 1);

                for (int n = 0; n < wavelet.Length; n++)
                {
                    float sum = 0;
                    float r = n * distancePerPoint;

                    for (int k = 0; k < waveletCoefs.Length; k++)
                    {
                        float rPrevious = 2 * r - k;
                        int rIndex = (int)MathF.Round(rPrevious / distancePerPoint);

                        if (rPrevious >= 0 && rPrevious <= maxRange)
                            sum += waveletCoefs[k] * scaling[rIndex].Y;
                    }
                    wavelet[n] = new Vector2(r, sum);
                }

                return wavelet;
            }
        }

        public int GetMaxRange(WaveletKind waveletKind) =>
            WaveletCoefficients.GetScalingCoefficients(waveletKind).Length - 1;

        private int PointsAtLevel(int zeroLevelPoints, int level)
        {
            if (level < 0)
                throw new ArgumentOutOfRangeException($"{nameof(PointsAtLevel)} was called with {nameof(level)} ({level})");
            else
                return zeroLevelPoints * (1 << level) - ((1 << level) - 1);
        }
    }
}

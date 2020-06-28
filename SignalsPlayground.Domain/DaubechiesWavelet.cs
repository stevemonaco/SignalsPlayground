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
                //current = new Vector2[PointsAtLevel(coefficients.Length, level)];
                //float distancePerIndex = maxRange / (current.Length - 1);

                //for (int j = 0; j < current.Length; j += 2)
                //    current[j] = previous[j / 2];

                //for (int j = 1; j < current.Length; j += 2)
                //{
                //    float sum = 0;
                //    float r = j / (float)(1 << level);

                //    for (int k = 0; k < coefficients.Length; k++)
                //    {
                //        float rprev = 2 * r - k;
                //        int rIndex = (int)MathF.Round(rprev / distancePerIndex);

                //        if (rprev >= 0 && rprev <= maxRange)
                //            sum += coefficients[k] * current[rIndex].Y;
                //    }
                //    current[j] = new Vector2(r, sum);
                //}

                yield return current;
                previous = current;
            }

            Vector2[] GetScalingLevel(int level, Vector2[] previous, float[] scalingCoefs)
            {
                var scaling = new Vector2[PointsAtLevel(scalingCoefs.Length, level)];
                float maxRange = scalingCoefs.Length - 1f;
                float distancePerIndex = maxRange / (scaling.Length - 1);

                for (int j = 0; j < scaling.Length; j += 2)
                    scaling[j] = previous[j / 2];

                for (int j = 1; j < scaling.Length; j += 2)
                {
                    float sum = 0;
                    float r = j / (float)(1 << level);

                    for (int k = 0; k < scalingCoefs.Length; k++)
                    {
                        float rPrevious = 2 * r - k;
                        int rIndex = (int)MathF.Round(rPrevious / distancePerIndex);

                        if (rPrevious >= 0 && rPrevious <= maxRange)
                            sum += scalingCoefs[k] * scaling[rIndex].Y;
                    }
                    scaling[j] = new Vector2(r, sum);
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

                for (int j = 0; j < wavelet.Length; j++)
                {
                    float sum = 0;
                    float r = j * distancePerPoint;

                    for (int k = 0; k < waveletCoefs.Length; k++)
                    {
                        float rprev = 2 * r - k;
                        int rIndex = (int)MathF.Round(rprev / distancePerPoint);

                        if (rprev >= 0 && rprev <= maxRange)
                            sum += waveletCoefs[k] * scaling[rIndex].Y;
                    }
                    wavelet[j] = new Vector2(r, sum);
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

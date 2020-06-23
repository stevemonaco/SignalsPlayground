using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalsPlayground.Domain
{
    public class WaveletCoefficients
    {
        private static readonly float[] db1Scaling = { 1, 1 };
        private static readonly float[] db2Scaling = { 0.6830127f, 1.1830127f, 0.3169873f, -0.1830127f };
        private static readonly float[] db3Scaling = { 0.47046721f, 1.14111692f, 0.650365f, -0.19093442f, -0.12083221f, 0.0498175f };
        private static readonly float[] db4Scaling = { 0.32580343f, 1.01094572f, 0.89220014f, -0.03957503f, -0.26450717f, 0.0436163f, 0.0465036f, -0.01498699f };
        private static readonly float[] db5Scaling = { 0.22641898f, 0.85394354f, 1.02432694f, 0.19576696f, -0.34265671f, -0.04560113f, 0.10970265f, -0.00882680f, -0.01779187f, 0.00471742793f };
        private static readonly float[] db6Scaling = { 0.15774243f, 0.69950381f, 1.06226376f, 0.44583132f, -0.31998660f, -0.18351806f, 0.13788809f, 0.03892321f, -0.04466375f, 0.000783251152f, 0.00675606236f, -0.00152353381f };

        private static readonly float[] db1Initial = { };
        private static readonly float[] db2Initial = { 0, 1.36602544f, -0.36602544f, 0 };
        private static readonly float[] db3Initial = { };
        private static readonly float[] db4Initial = { 0, 1.00716450f, -3.38492330e-2f, 3.96088947e-2f, -1.17337313e-2f, -1.25087227e-3f,  6.04419105e-5f };
        private static readonly float[] db5Initial = { };
        private static readonly float[] db6Initial = { };

        public static ReadOnlySpan<float> GetWaveletCoefficients(WaveletKind waveletKind)
        {
            return waveletKind switch
            {
                WaveletKind.db1 => ToWaveletCoefficients(db1Scaling).ToArray(),
                WaveletKind.db2 => ToWaveletCoefficients(db2Scaling).ToArray(),
                WaveletKind.db3 => ToWaveletCoefficients(db3Scaling).ToArray(),
                WaveletKind.db4 => ToWaveletCoefficients(db4Scaling).ToArray(),
                WaveletKind.db5 => ToWaveletCoefficients(db5Scaling).ToArray(),
                WaveletKind.db6 => ToWaveletCoefficients(db6Scaling).ToArray(),
                _ => throw new ArgumentException($"{nameof(GetWaveletCoefficients)} parameter '{waveletKind}' is invalid")
            };
        }

        public static ReadOnlySpan<float> GetScalingCoefficients(WaveletKind waveletKind)
        {
            return waveletKind switch
            {
                WaveletKind.db1 => db1Scaling,
                WaveletKind.db2 => db2Scaling,
                WaveletKind.db3 => db3Scaling,
                WaveletKind.db4 => db4Scaling,
                WaveletKind.db5 => db5Scaling,
                WaveletKind.db6 => db6Scaling,
                _ => throw new ArgumentException($"{nameof(GetScalingCoefficients)} parameter '{waveletKind}' is invalid")
            };
        }

        public static ReadOnlySpan<float> GetInitialScalingValues(WaveletKind waveletKind)
        {
            return waveletKind switch
            {
                WaveletKind.db1 => db1Initial,
                WaveletKind.db2 => db2Initial,
                WaveletKind.db3 => db3Initial,
                WaveletKind.db4 => db4Initial,
                WaveletKind.db5 => db5Initial,
                WaveletKind.db6 => db6Initial,
                _ => throw new ArgumentException($"{nameof(GetScalingCoefficients)} parameter '{waveletKind}' is invalid")
            };
        }

        /// <summary>
        /// Reverses the Quadrature Mirror Filter
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static IEnumerable<float> ToWaveletCoefficients(IEnumerable<float> items) =>
            items.Select((x, index) => index % 2 == 0 ? -x : x).Reverse();
    }
}

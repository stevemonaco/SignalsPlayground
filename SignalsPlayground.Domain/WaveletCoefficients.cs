using System;
using System.Collections.Generic;
using System.Linq;

namespace SignalsPlayground.Domain
{
    public class WaveletCoefficients
    {
        // Scaling coefficients from https://en.wikipedia.org/wiki/Daubechies_wavelet#The_scaling_sequences_of_lowest_approximation_order
        private static readonly float[] db2Scaling = { 0.6830127f, 1.1830127f, 0.3169873f, -0.1830127f };
        private static readonly float[] db3Scaling = { 0.47046721f, 1.14111692f, 0.650365f, -0.19093442f, -0.12083221f, 0.0498175f };
        private static readonly float[] db4Scaling = { 0.32580343f, 1.01094572f, 0.89220014f, -0.03957503f, -0.26450717f, 0.0436163f, 0.0465036f, -0.01498699f };
        private static readonly float[] db5Scaling = { 0.22641898f, 0.85394354f, 1.02432694f, 0.19576696f, -0.34265671f, -0.04560113f, 0.10970265f, -0.00882680f, -0.01779187f, 0.00471742793f };
        private static readonly float[] db6Scaling = { 0.15774243f, 0.69950381f, 1.06226376f, 0.44583132f, -0.31998660f, -0.18351806f, 0.13788809f, 0.03892321f, -0.04466375f, 0.000783251152f, 0.00675606236f, -0.00152353381f };
        private static readonly float[] db7Scaling = { 0.11009943f, 0.56079128f, 1.03114849f, 0.66437248f, -0.20351382f, -0.31683501f, 0.1008467f, 0.11400345f, -0.05378245f, -0.02343994f, 0.01774979f, 6.07514995e-4f, -2.54790472e-3f, 5.00226853e-4f };
        private static readonly float[] db8Scaling = { 0.07695562f, 0.44246725f, 0.95548615f, 0.82781653f, -0.02238574f, -0.40165863f, 6.68194092e-4f, 0.18207636f, -0.02456390f, -0.06235021f, 0.01977216f, 0.01236884f, -6.88771926e-3f, -5.54004549e-4f, 9.55229711e-4f, -1.66137261e-4f };
        private static readonly float[] db9Scaling = { 0.05385035f, 0.34483430f, 0.85534906f, 0.92954571f, 0.18836955f, -0.41475176f, -0.13695355f, 0.21006834f, 0.043452675f, -0.09564726f, 3.54892813e-4f, 0.03162417f, -6.67962023e-3f, -6.05496058e-3f, 2.61296728e-3f, 3.25814671e-4f, -3.56329759e-4f, 5.5645514e-5f };
        private static readonly float[] db10Scaling = { 0.03771716f, 0.26612218f, 0.74557507f, 0.97362811f, 0.39763774f, -0.35333620f, -0.27710988f, 0.18012745f, 0.13160299f, -0.10096657f, -0.04165925f, 0.04696981f, 5.10043697e-3f, -0.01517900f, 1.97332536e-3f, 2.81768659e-3f, -9.69947840e-4f, -1.64709006e-4f, 1.32354367e-4f, -1.875841e-5f, };

        // Initial scaling values derived from db_initial_values.py
        private static readonly float[] db2Initial  = { 0, 1.36602544f, -0.36602544f, 0 };
        private static readonly float[] db3Initial  = { 0, 1.28653824f, -0.38600335f, 0.09525722f, 0.00420788f, 0 };
        private static readonly float[] db4Initial  = { 0, 1.00716450f, -3.38492330e-2f, 3.96088947e-2f, -1.17337313e-2f, -1.25087227e-3f,  6.04419105e-5f, 0 };
        private static readonly float[] db5Initial  = { 0, 6.96581952e-1f, 4.48937292e-1f, -1.82376501e-1f, 3.71700319e-2f, 1.46173743e-3f, -1.72676487e-3f, -9.22021251e-6f, -3.85275626e-5f, 0 };
        private static readonly float[] db6Initial  = { 0, 4.36587765e-1f, 8.32415060e-1f, -3.84521860e-1f, 1.42839932e-1f, -2.54773902e-2f, -3.57531959e-3f,  1.91016751e-3f, -1.10661322e-4f, -2.25974137e-5f, -4.50963101e-5f, 0 };
        private static readonly float[] db7Initial  = { 0, 2.53176262e-1f, 1.01001486f, -3.92364768e-1f, 1.84666057e-1f, -6.69130292e-2f, 1.06144460e-2f, 1.37177938e-3f, -5.07771756e-4f,  1.53516516e-5f, 1.07593726e-4f, -4.55972386e-5f, -1.35180833e-4f, 0 };
        private static readonly float[] db8Initial  = { 0, 1.37000734e-1f, 9.91912754e-1f, -1.68983598e-1f, 7.11450154e-2f, -4.90054741e-2f, 2.35716063e-2f, -6.27419915e-3f, 7.81445865e-4f, -1.43491763e-4f, 2.40767243e-5f, -6.04642158e-5f, -4.20163080e-6f, 7.16157238e-6f, 2.86356112e-5f, 0 };
        private static readonly float[] db9Initial  = { 0, 7.00238649e-2f, 8.48016140e-1f, 1.91746375e-1f, -1.61539948e-1f, 6.22122513e-2f, -6.38616197e-3f, -8.02645550e-3f, 5.45690991e-3f, -1.52196195e-3f, -1.42990994e-4f, -2.46778588e-4f, 1.37316571e-4f,  2.95254176e-4f,  6.69555476e-5f, -8.34085913e-5f, -7.36113108e-6f, 0 };
        private static readonly float[] db10Initial = { 0, 3.35022410e-2f, 6.52441286e-1f, 5.55187364e-1f, -3.80546580e-1f, 2.02254237e-1f, -8.02421839e-2f,  1.70954422e-2f, 2.17394433e-3f, -2.21139656e-3f,  2.66167187e-4f, -7.60169294e-5f, 1.53443445e-5f,  1.66660591e-4f, -1.20277572e-4f,  1.02015014e-4f, 2.28888562e-4f, -2.04018674e-4f, -3.31164517e-5f, 0 };

        public static ReadOnlySpan<float> GetWaveletCoefficients(WaveletKind waveletKind)
        {
            return waveletKind switch
            {
                WaveletKind.db2 => ToWaveletCoefficients(db2Scaling).ToArray(),
                WaveletKind.db3 => ToWaveletCoefficients(db3Scaling).ToArray(),
                WaveletKind.db4 => ToWaveletCoefficients(db4Scaling).ToArray(),
                WaveletKind.db5 => ToWaveletCoefficients(db5Scaling).ToArray(),
                WaveletKind.db6 => ToWaveletCoefficients(db6Scaling).ToArray(),
                WaveletKind.db7 => ToWaveletCoefficients(db7Scaling).ToArray(),
                WaveletKind.db8 => ToWaveletCoefficients(db8Scaling).ToArray(),
                WaveletKind.db9 => ToWaveletCoefficients(db9Scaling).ToArray(),
                WaveletKind.db10 => ToWaveletCoefficients(db10Scaling).ToArray(),
                _ => throw new ArgumentException($"{nameof(GetWaveletCoefficients)} parameter '{waveletKind}' is invalid")
            };
        }

        public static ReadOnlySpan<float> GetScalingCoefficients(WaveletKind waveletKind)
        {
            return waveletKind switch
            {
                WaveletKind.db2 => db2Scaling,
                WaveletKind.db3 => db3Scaling,
                WaveletKind.db4 => db4Scaling,
                WaveletKind.db5 => db5Scaling,
                WaveletKind.db6 => db6Scaling,
                WaveletKind.db7 => db7Scaling,
                WaveletKind.db8 => db8Scaling,
                WaveletKind.db9 => db9Scaling,
                WaveletKind.db10 => db10Scaling,
                _ => throw new ArgumentException($"{nameof(GetScalingCoefficients)} parameter '{waveletKind}' is invalid")
            };
        }

        public static ReadOnlySpan<float> GetInitialScalingValues(WaveletKind waveletKind)
        {
            return waveletKind switch
            {
                WaveletKind.db2 => db2Initial,
                WaveletKind.db3 => db3Initial,
                WaveletKind.db4 => db4Initial,
                WaveletKind.db5 => db5Initial,
                WaveletKind.db6 => db6Initial,
                WaveletKind.db7 => db7Initial,
                WaveletKind.db8 => db8Initial,
                WaveletKind.db9 => db9Initial,
                WaveletKind.db10 => db10Initial,
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

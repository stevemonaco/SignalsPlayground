using System;

namespace SignalsPlayground.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintD4Coefficients();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// https://en.wikipedia.org/wiki/Daubechies_wavelet#The_scaling_sequences_of_lowest_approximation_order
        /// </remarks>
        static void PrintD4Coefficients()
        {
            Console.WriteLine("D4 Quadrature Mirror Filter Coefficients");
            var c0 = (1 + Math.Sqrt(3)) / (4 * Math.Sqrt(2));
            var c1 = (3 + Math.Sqrt(3)) / (4 * Math.Sqrt(2));
            var c2 = (3 - Math.Sqrt(3)) / (4 * Math.Sqrt(2));
            var c3 = (1 - Math.Sqrt(3)) / (4 * Math.Sqrt(2));

            Console.WriteLine($"c0: {c0}");
            Console.WriteLine($"c1: {c1}");
            Console.WriteLine($"c2: {c2}");
            Console.WriteLine($"c3: {c3}");

            var normFactor = 2 / (c0 + c1 + c2 + c3);

            c0 = c0 * normFactor * Math.Pow(-1, 0);
            c1 = -c1 * normFactor * Math.Pow(-1, 1);
            c2 = c2 * normFactor * Math.Pow(-1, 2);
            c3 = -c3 * normFactor * Math.Pow(-1, 3);

            Console.WriteLine("D4 Quadrature Mirror Filter Coefficients (Normalized to sum of 2)");

            Console.WriteLine($"c0: {c0}");
            Console.WriteLine($"c1: {c1}");
            Console.WriteLine($"c2: {c2}");
            Console.WriteLine($"c3: {c3}");
        }
    }
}

using System.Collections.Generic;
using System;

namespace Tests
{
    /// <summary>
    /// Generates a predictable number. Used to mock a random number generator.
    /// </summary>
    internal class PredictableNumberGenerator
    {
        private IList<int> Sequence { get; }
        private int Index { get; set; }
        
        /// <summary>
        /// Define an amount that ports are shifted to avoid possible conflicts with
        /// other tests.
        /// </summary>
        private const int PortShift = 1000;

        /// <summary>
        /// Instantiates the instance with the given sequence of numbers.
        /// </summary>
        /// <param name="sequence">The sequence of numbers to return.</param>
        public PredictableNumberGenerator(params int[] sequence)
        {
            Sequence = new List<int>(sequence);
        }

        /// <summary>
        /// Returns the next value in the sequence.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the number to be
        /// returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the number to be
        /// returned.</param>
        /// <returns>The next number in the sequence.</returns>
        public int Next(int minValue, int maxValue)
        {
            var next = minValue + Sequence[Index] - PortShift;
            Index += 1;

            if (next >= maxValue)
                throw new InvalidOperationException("Exceeded allowable range");

            return next;
        }
    }
}

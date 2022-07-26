namespace ProceduralLevelGeneration.EvolutionaryComputing
{
    using Data;
    using UnityEngine;
    using Utility;
    
    public static class Genetics
    {
        /// <summary>
        /// Operates a crossover between two chromosomes
        /// </summary>
        /// <param name="c1">The first parent</param>
        /// <param name="c2">The second parent</param>
        /// <param name="p">
        /// The probability to take a gene from either parent
        /// <br />
        /// 0 = Only genes from the first parent
        /// <br />
        /// 1 = Only genes from the second parent
        /// </param>
        /// <returns>The child born from those parents</returns>
        public static Chromosome Crossover(Chromosome c1, Chromosome c2, float p) {
            var child = new Chromosome();
            for (var index = 0; index < Shortest(c1, c2); index++) child.Add(RandUtils.PickOne(c1, c2, p)[index]);
            return child;
        }

        /// <summary>
        /// Operates a random mutation on a chromosome
        /// </summary>
        /// <param name="c">The parent</param>
        /// <param name="p">
        /// The probability of a gene mutating
        /// <br />
        /// 0 = No mutation
        /// <br />
        /// 1 = Every gene mutates
        /// </param>
        /// <param name="mutate">The function which creates a new, random room</param>
        /// <returns>The potentially mutated child</returns>
        public static Chromosome Mutation(Chromosome c, float p, Room.Mutate mutate) {
            var child = new Chromosome();
            for (var index = 0; index < c.Count; index++) child.Add(RandUtils.PickOne(mutate(), c[index], p));
            return child;
        }

        private static int Shortest(Chromosome c1, Chromosome c2) { return Mathf.Min(c1.Count, c2.Count); }
    }
}
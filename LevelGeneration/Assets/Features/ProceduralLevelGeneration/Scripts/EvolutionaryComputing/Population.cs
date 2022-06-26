using System.Linq;
using DataTypes;
using UnityEngine;

namespace ProceduralLevelGeneration.EvolutionaryComputing
{
    using Chromosome;
    using Room;
    
    public class Population
    {
        public delegate float Fitness(Chromosome chromosome);

        public int evolutionLevel;

        private readonly Fitness _fitnessFunction;

        private readonly Room.Mutate _mutate;

        private readonly Chromosome[] _chromosomes;

        public Population(int populationSize, int geneCount, Room.Mutate mutate, Fitness fitnessFunction)
        {
            evolutionLevel = 0;
            _chromosomes = new Chromosome[populationSize];
            _fitnessFunction = fitnessFunction;
            _mutate = mutate;
            PopulateChromosomes(geneCount);
        }

        public int Evolve(float crossoverProbability, float mutationProbability)
        {
            var fittest = FindFittest();
            CreateNewGeneration(fittest, crossoverProbability, mutationProbability);
            return evolutionLevel;
        }

        public Mesh BestLayout()
        {
            var fittest = FindFittest().first;
            Debug.Log($"Best level with a surface area of {fittest.Area()} :\n{fittest}");
            return fittest.ToMesh();
        }

        #region Chromosome population

        private void PopulateChromosomes(int geneCount) { for (var i = 0; i < _chromosomes.Length; i++) { _chromosomes[i] = PopulateChromosome(geneCount); } }

        private Chromosome PopulateChromosome(int geneCount)
        {
            var chromosome = new Chromosome();
            for (var index = 0; index < geneCount; index++) { chromosome.Add(_mutate()); }
            return chromosome;
        }

        #endregion

        #region Fittest calculation

        private Pair<Chromosome, Chromosome> FindFittest()
        {
            var fitnessRatings = _chromosomes.Select(GetFitness).ToList();
            var sorted = fitnessRatings.OrderByDescending(v => v.second).ToArray();
            return new Pair<Chromosome, Chromosome>(sorted[0].first, sorted[1].first);
        }

        private Pair<Chromosome, float> GetFitness(Chromosome chromosome) { return new Pair<Chromosome, float>(chromosome, _fitnessFunction(chromosome)); }

        #endregion

        #region Next-gen creation

        private void CreateNewGeneration(Pair<Chromosome, Chromosome> fittest, float crossoverProbability, float mutationProbability)
        {
            Crossover(fittest, crossoverProbability);
            Mutation(mutationProbability);
            evolutionLevel++;
        }

        private void Crossover(Pair<Chromosome, Chromosome> fittest, float crossoverProbability)
        {
            for (var index = 0; index < _chromosomes.Length; index++)
            {
                var child = Genetics.Crossover(fittest.first, fittest.second, crossoverProbability);
                _chromosomes[index] = child;
            }
        }

        private void Mutation(float mutationProbability)
        {
            for (var index = 0; index < _chromosomes.Length; index++)
            {
                var mutant = Genetics.Mutation(_chromosomes[index], mutationProbability, _mutate);
                _chromosomes[index] = mutant;
            }
        }

        #endregion

        #region Fitness functions

        public static float MaximizeArea(Chromosome chromosome) { return chromosome.Area(); }

        public static float MinimizeArea(Chromosome chromosome) { return 1000 - chromosome.Area(); }
        
        public static float CorridorPenalty(Chromosome chromosome) { return 1 / ((1 + chromosome.NarrowCount) * Mathf.Pow(10, chromosome.TinyCount)); }

        #endregion
    }
}
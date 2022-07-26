namespace ProceduralLevelGeneration.EvolutionaryComputing {
    using Data;
    using System;
    using UnityEngine;
    using Utility;

    public enum EFitness {
        MaximizeArea,
        MinimizeArea,
        CorridorPenalty,
    }
    
    public class ProceduralLayoutGenerator : MonoBehaviour {
        public bool standaloneRun;
        
        [Header("Room settings")]
        [SerializeField] private Vector2Int rangeL = new Vector2Int(5, 15);
        [SerializeField] private Vector2Int rangeW = new Vector2Int(5, 15);
        
        [Space(5)]
        
        [SerializeField] private Vector2Int rangeX = new Vector2Int(-10, 10);
        [SerializeField] private Vector2Int rangeY = new Vector2Int(-10, 10);
        
        [Header("Chromosome settings")]
        [SerializeField, Range(2, 100)] private int chromosomesInPopulation = 10;
        [SerializeField, Range(5, 10)] private int roomsInChromosome = 5;
        
        [Header("Evolutionary computing")]
        [Tooltip("0 = Clone of the first parent\n1 = Clone of the second parent")]
        [SerializeField, Range(0, 1)] private float crossoverProbability = .5f;
        
        [Tooltip("0 = No mutation\n1 = A new chromosome is created")]
        [SerializeField, Range(0, 1)] private float mutationProbability = .1f;

        [SerializeField] private EFitness fitnessFunction;
        
        [SerializeField, Range(1, 100)] private int iterations = 30;

        private Population _population;

        private void Start() {
            _population = new Population(chromosomesInPopulation, roomsInChromosome, Mutate, GetFunction());

            if (standaloneRun) {
                var gob = new GameObject("Level layout", typeof(MeshFilter), typeof(MeshRenderer));
                gob.GetComponent<MeshFilter>().mesh = Run();
            }
        }

        public Mesh Run() {
            var runtime = Time.realtimeSinceStartup;
            var iterationCount = 0;
            while (iterationCount < iterations) iterationCount = _population.Evolve(crossoverProbability, mutationProbability);
            Debug.Log($"Time spent : {Time.realtimeSinceStartup - runtime} seconds");
            return _population.BestLayout();
        }

        private Room Mutate() { return new Room(RandInt(rangeX), RandInt(rangeY), RandInt(rangeL), RandInt(rangeW)); }

        private Population.Fitness GetFunction() {
            return fitnessFunction switch
            {
                EFitness.MaximizeArea => Population.MaximizeArea,
                EFitness.MinimizeArea => Population.MinimizeArea,
                EFitness.CorridorPenalty => Population.CorridorPenalty,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static int RandInt(Vector2Int range) { return RandUtils.RandInt(range.x, range.y); }

        private void OnValidate() {
            if (rangeL.x <= 0) rangeL.x = 1;
            if (rangeL.y < rangeL.x) rangeL.y = rangeL.x;
            if (rangeW.x <= 0) rangeW.x = 1;
            if (rangeW.y < rangeW.x) rangeW.y = rangeW.x;
            if (rangeX.x < 0) rangeX.x = 0;
            if (rangeX.y < rangeX.x) rangeX.y = rangeX.x;
            if (rangeY.x < 0) rangeY.x = 0;
            if (rangeY.y < rangeY.x) rangeY.y = rangeY.x;
        }
    }
}
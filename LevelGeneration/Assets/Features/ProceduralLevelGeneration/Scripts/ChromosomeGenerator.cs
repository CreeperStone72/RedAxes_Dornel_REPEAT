using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ProceduralLevelGeneration
{
    public class ChromosomeGenerator : MonoBehaviour
    {
        public Chromosome chromosome;
        
        private void Start()
        {
            var meshes = chromosome.ToMeshes();

            for (var i = 0; i < meshes.Count; i++)
            {
                var go = new GameObject($"Room {i}", typeof(MeshFilter), typeof(MeshRenderer));
                go.GetComponent<MeshFilter>().mesh = meshes[i];
                Debug.Log(chromosome.GetRoomArea(i));
            }
        }
    }
}
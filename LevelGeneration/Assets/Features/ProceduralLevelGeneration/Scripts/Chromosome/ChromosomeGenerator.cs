using UnityEngine;

namespace ProceduralLevelGeneration.Chromosome
{
    public class ChromosomeGenerator : MonoBehaviour
    {
        public bool loadGlobalMesh;
        
        public Chromosome chromosome;
        
        private void Start()
        {
            if (loadGlobalMesh)
            {
                var gob = new GameObject($"Max surface", typeof(MeshFilter), typeof(MeshRenderer));
                gob.GetComponent<MeshFilter>().mesh = chromosome.ToMesh();
            }
            else
            {
                var meshes = chromosome.ToMeshes();
            
                for (var i = 0; i < chromosome.Count; i++)
                {
                    var go = new GameObject($"Room {i}", typeof(MeshFilter), typeof(MeshRenderer));
                    go.GetComponent<MeshFilter>().mesh = meshes[i];
                    go.GetComponent<MeshRenderer>().material.color = Random.ColorHSV();
                }
            }
        }
    }
}
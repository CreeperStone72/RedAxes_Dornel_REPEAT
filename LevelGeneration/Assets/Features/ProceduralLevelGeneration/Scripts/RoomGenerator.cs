using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ProceduralLevelGeneration
{
    public class RoomGenerator : MonoBehaviour
    {
        [SerializeField] private int x, y, w, h;
        [SerializeField] private Room.PlacementType t;
        
        private void Start()
        {
            var room = new Room(x, y, w, h, t);

            var go = new GameObject("Room", typeof(MeshFilter), typeof(MeshRenderer));
            go.GetComponent<MeshFilter>().mesh = room.ToMesh();
        }
    }
}
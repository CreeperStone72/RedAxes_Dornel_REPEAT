using UnityEngine;

namespace ProceduralLevelGeneration.Room
{
    public class RoomGenerator : MonoBehaviour
    {
        [SerializeField] private int x, y, l, w;
        
        private void Start()
        {
            var room = new Room(x, y, l, w);

            var go = new GameObject("Room", typeof(MeshFilter), typeof(MeshRenderer));
            go.GetComponent<MeshFilter>().mesh = room.ToMesh();
        }
    }
}
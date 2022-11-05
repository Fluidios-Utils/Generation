using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.Tilemap.Hexagonal
{
    public class HexaTile : Tile
    {
        [SerializeField] private MeshFilter _meshFilter;

        public void Setup(Mesh mesh, Vector3 position)
        {
            transform.position = position;
            _meshFilter.sharedMesh = mesh;
        }
    }
}

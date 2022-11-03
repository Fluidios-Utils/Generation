using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluidiousUtils.Generation.NoiseMath;
using UnityEngine.UIElements;

namespace FluidiousUtils.Generation.Tilemap.Hexagonal
{
    [CreateAssetMenu(fileName = "Hexagonal Tilemap Settings", menuName = "FUtils/Generation/Tilemap/Hexagonal/TilemapSettings")]
    public class HexagonalTilemapSettings : TileMapSettings
    {
        [Header("Map"),SerializeField] private int _mapSize = 5;
        public int MapSize { get { return _mapSize; } }
        public Vector3 MapCenter
        {
            get 
            {
                Vector3 position;
                float x = Mathf.Round(_mapSize / 2);
                position.x = x * InnerTileRadius * 2f;
                position.y = 0f;
                position.z = x * OuterTileRadius * 1.5f;
                return position; 
            }
        }
        [SerializeField] private NoiseLayer[] _noiseLayers;
        public NoiseLayer[] NoiseLayers { get { return _noiseLayers; } }
        [SerializeField] private Gradient _locationDistribution;
        public Gradient LocationDistribution { get { return _locationDistribution; } }
        [Header("Tiles"), SerializeField] private HexaTile _basicTile;
        public HexaTile BasicTile { get { return _basicTile; } }
        [SerializeField] private float _outerTileRadius = 1;
        public float OuterTileRadius { get { return _outerTileRadius; } }
        public float InnerTileRadius { get { return _outerTileRadius * Mathf.Sqrt(3) / 2; } }
        [SerializeField] private float _tileBevel = 0.1f;
        public float TileBevel { get { return _tileBevel; } }
        [SerializeField] private float _tileHeightStep = 2;
        public float TileHeightStep { get { return _tileHeightStep; } }
    }
}
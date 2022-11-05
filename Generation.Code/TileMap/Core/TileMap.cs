using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.Tilemap
{
    public abstract class TileMap : MonoBehaviour
    {
        [SerializeField] private TileMapSettings _settings;
        [SerializeField] private TileMapGenerator _mapGenerator;
        public Tile[] Tiles { get; private set; }

        protected void Generate()
        {
            _mapGenerator.Generate(_settings);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.Tilemap
{
    public abstract class TileMapGenerator : MonoBehaviour
    {
        public abstract Tile[] Generate(TileMapSettings mapSettings);
    }
}

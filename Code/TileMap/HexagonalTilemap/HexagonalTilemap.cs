using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.Tilemap.Hexagonal
{
    public class HexagonalTilemap : TileMap
    {
        private void Awake()
        {
            Generate();
        }
    }
}

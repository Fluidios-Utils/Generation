using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.Tilemap
{
    public abstract class TileMapSettings : ScriptableObject
    {
        [SerializeField] private int _seed;
        public int Seed { get { return _seed; } }
    }
}

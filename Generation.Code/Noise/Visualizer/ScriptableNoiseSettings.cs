using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.NoiseMath
{
    internal abstract class ScriptableNoiseSettings : ScriptableObject
    {
        public abstract void ResetNoise(int seed, Vector3 newOffset);
        public abstract float CalculateAtPoint(Vector3 point);
    }

}
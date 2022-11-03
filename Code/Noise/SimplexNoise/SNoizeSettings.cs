using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.NoiseMath
{
    [CreateAssetMenu(fileName = "SimplexNoise", menuName = "FUtils/Generation/NoiseMath/SimplexNoiseSettings")]
    internal class SNoizeSettings : ScriptableNoiseSettings
    {
        [SerializeField] private List<Noise.Layer> _layers;
        public override float CalculateAtPoint(Vector3 point)
        {
            return Noise.GenerateValue(_layers, point);
        }

        public override void ResetNoise(int seed, Vector3 newOffset)
        {
            foreach (var item in _layers)
            {
                item.generator = new SimplexNoise(seed);
                item.offset = newOffset;
            }
        }
    }
}

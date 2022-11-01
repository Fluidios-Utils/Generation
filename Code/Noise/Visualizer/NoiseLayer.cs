using FluidiousUtils.Generation.NoiseMath;
using System;
using UnityEngine;

namespace FluidiousUtils.Generation.NoiseMath
{
    [Serializable]
    public class NoiseLayer
    {
        [SerializeField] private bool _enabled;
        [SerializeField] private bool _usePreviousAsMask;
        public bool Enabled { get { return _enabled; } }
        public bool UsePreviousAsMask { get { return _usePreviousAsMask; } }
        [SerializeField] private ScriptableNoiseSettings _noiseSettings;

        public float CalculateAtPoint(Vector3 point)
        {
            return _noiseSettings.CalculateAtPoint(point);
        }
    }
}
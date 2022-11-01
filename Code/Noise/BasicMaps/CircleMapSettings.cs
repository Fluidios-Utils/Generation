using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.NoiseMath
{
    [CreateAssetMenu(fileName = "CircleMap", menuName = "FUtils/Generation/NoiseMath/CircleMapSettings")]
    internal class CircleMapSettings : ScriptableNoiseSettings
    {
        [SerializeField] private Vector3 _center;
        [SerializeField] private float _radius = 1;
        [SerializeField] private float _blend = 0.2f;
        [SerializeField] private bool _invert;

        public override float CalculateAtPoint(Vector3 point)
        {
            float distance = Vector3.Distance(point, _center);
            float value = distance > _radius ? (Mathf.Lerp(0, 1, (_radius + _blend - distance)/_blend)) : 1;
            return _invert ? (1 - value) : value;
        }
    }
}

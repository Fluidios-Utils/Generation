using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.NoiseMath
{
    [RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
    public class NoiseVisualizer : MonoBehaviour
    {
        [SerializeField] private int _mapScale = 1024;
        [SerializeField] private float _noiseScale = 1;
        [SerializeField] private bool _autoUpdate;
        public bool AutoUpdate { get { return _autoUpdate; } }
        [SerializeField] private List<NoiseLayer> _settings = new List<NoiseLayer>();
        internal List<NoiseLayer> Settings { get { return _settings; } }
       
        public void Regenerate()
        {
            float noiseValue;
            Vector3 position;
            Texture2D texture = new Texture2D(_mapScale, _mapScale);
            Color[] colorMap = new Color[_mapScale * _mapScale];
            for (int y = 0; y < _mapScale; y++)
            {
                for (int x = 0; x < _mapScale; x++)
                {
                    position = new Vector3(x, 0, y) * _noiseScale;
                    noiseValue = CalculateNoiseAtPoint(position);
                    colorMap[y * _mapScale + x] = Color.Lerp(Color.black, Color.white, noiseValue);
                }
            }
            texture.SetPixels(colorMap);
            texture.Apply();

            Renderer renderer = GetComponent<Renderer>();
            renderer.sharedMaterial = new Material(Shader.Find("Unlit/Texture"));
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.sharedMaterial.mainTexture = texture;
            renderer.transform.localScale = new Vector3(_mapScale, 1, _mapScale);
        }

        private float CalculateNoiseAtPoint(Vector3 point)
        {
            if (_settings.Count == 0) return default;


            float previousLayer = _settings[0].CalculateAtPoint(point);
            float noiseValue = _settings[0].Enabled ? previousLayer : 0;

            for (int i = 1; i < _settings.Count && _settings[i].Enabled; i++)
            {
                float maskValue = (_settings[i].UsePreviousAsMask) ? previousLayer : 1; //save prev layer value if needed
                previousLayer = _settings[i].CalculateAtPoint(point); //calculate
                noiseValue += previousLayer * maskValue; //add with mask effect 
            }

            return noiseValue;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FluidiousUtils.Generation.Tilemap
{
    public abstract class Tile : MonoBehaviour
    {
        [SerializeField] List<Tile> _neigbours = new List<Tile>();
        public List<Tile> Neighbours { get { return _neigbours; } }
        [SerializeField] private bool _debug;

        private void OnDrawGizmosSelected()
        {
            if (_debug)
            {
                Gizmos.color = Color.black;
                foreach (var item in _neigbours)
                {
                    Gizmos.DrawLine(transform.position + Vector3.up, item.transform.position + Vector3.up);
                }
            }
        }
    }
}

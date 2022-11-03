using FluidiousUtils.Generation.NoiseMath;
using FluidiousUtils.Generation.Meshes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace FluidiousUtils.Generation.Tilemap.Hexagonal
{
    public class HexmapGenerator : TileMapGenerator
    {
        private Mesh[] _locationMeshes;

        public override Tile[] Generate(TileMapSettings mapSettings)
        {
            List<Tile> tiles = new List<Tile>();
            HexagonalTilemapSettings settings = mapSettings as HexagonalTilemapSettings;
            _locationMeshes = PrepareMeshes(settings);
            Vector3 position;
            float height;
            ResetNoiseOffset(settings);
            for (int z = 0; z < settings.MapSize; z++)
            {
                for (int x = 0; x < settings.MapSize; x++)
                {
                    position = CalculatePosition(x, z, settings);

                    height = CalculateHeight(position, settings.NoiseLayers);

                    tiles.Add(CreateTile(settings, height, position, tiles.Count));

                    ConnectTileWithNeighbours(tiles.Count - 1, tiles, settings.MapSize);
                }
            }
            return tiles.ToArray();
        }
        private Mesh[] PrepareMeshes(HexagonalTilemapSettings settings)
        {
            int length = settings.LocationDistribution.colorKeys.Length;
            Mesh[] meshes = new Mesh[length];
            for (int i = 0; i < length; i++)
            {
                meshes[i] = MeshGenerator.NgonShape
                        (
                               6,
                                settings.OuterTileRadius,
                                (i + 1) * settings.TileHeightStep,
                                settings.TileBevel,
                                30,
                                settings.LocationDistribution.colorKeys[i].color,
                                "Hexagon " + (i + 1).ToString(),
                                MeshGenerator.BevelType.AllEdges,
                                MeshGenerator.ShapeType.WithoutBottomSide
                        );
            }
            return meshes;
        }
        private void ResetNoiseOffset(HexagonalTilemapSettings settings)
        {
            Vector3 mapCenter = settings.MapCenter;
            foreach (var item in settings.NoiseLayers)
            {
                item.ResetNoise(settings.Seed, mapCenter);
            }
        }
        private Vector3 CalculatePosition(int x, int z, HexagonalTilemapSettings settings)
        {
            return new Vector3
                (
                    (x + z * 0.5f - z / 2) * (settings.InnerTileRadius * 2f),
                    0,
                    z * (settings.OuterTileRadius * 1.5f)
                );
        }
        private float CalculateHeight(Vector3 point, NoiseLayer[] noiseLayers)
        {
            if (noiseLayers.Length == 0) return default;


            float previousLayer = noiseLayers[0].CalculateAtPoint(point);
            float noiseValue = noiseLayers[0].Enabled ? previousLayer : 0;

            for (int i = 1; i < noiseLayers.Length && noiseLayers[i].Enabled; i++)
            {
                float maskValue = (noiseLayers[i].UsePreviousAsMask) ? previousLayer : 1; //save prev layer value if needed
                previousLayer = noiseLayers[i].CalculateAtPoint(point); //calculate
                noiseValue += previousLayer * maskValue; //add with mask effect 
            }

            return noiseValue;
        }
        private Tile CreateTile(HexagonalTilemapSettings settings, float height, Vector3 position, int tileNum)
        {
            HexaTile tile = Instantiate(settings.BasicTile, transform);
            tile.gameObject.name = tileNum.ToString();
            tile.Setup
            (
                        GetMeshFromNoiseHeight(height, settings, out var tileHeight),
                        position + Vector3.up * tileHeight / 2
            );

            return tile;
        }
        private Mesh GetMeshFromNoiseHeight(float noiseHeight, HexagonalTilemapSettings settings, out float meshHeight)
        {
            int length = settings.LocationDistribution.colorKeys.Length;
            int id = 0;
            for (int i = 0; i < length; i++)
            {
                if (noiseHeight > settings.LocationDistribution.colorKeys[i].time)
                    id = i;
            }
            meshHeight = (id + 1) * settings.TileHeightStep;
            return _locationMeshes[id];
        }
        private void ConnectTileWithNeighbours(int tileId, List<Tile> tiles, int mapSize)
        {
            Tile main = tiles[tileId];

            if(tileId % mapSize > 0)
            {
                main.Neighbours.Add(tiles[tileId - 1]);
                tiles[tileId - 1].Neighbours.Add(main);
            }

            if(tileId >= mapSize)
            {
                int offset = (tileId / mapSize) % 2 > 0 ? 0 : -1;
                int row = tileId / mapSize;
                if (row % 2 > 0 || tileId % mapSize > 0)
                {
                    main.Neighbours.Add(tiles[tileId - mapSize + offset]);
                    tiles[tileId - mapSize + offset].Neighbours.Add(main);
                }
                if ((tileId + 1) % mapSize > 0 || row % 2 == 0)
                {
                    main.Neighbours.Add(tiles[tileId - mapSize + offset + 1]);
                    tiles[tileId - mapSize + offset + 1].Neighbours.Add(main);
                }
            }
        }
    }
}

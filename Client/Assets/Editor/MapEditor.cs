using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor
{
#if UNITY_EDITOR
    /// <summary>
    /// Inspects the map's collision coordinates and makes them into a file.
    /// </summary>
    [MenuItem("Tools/GenerateMap")]
    private static void GenerateMap()
    {
        GenerateByPath("Assets/Resources/Map/");
        GenerateByPath("../Common/MapData/");
    }

    private static void GenerateByPath(string pathPrefix)
    {
        GameObject[] _gameoObjects = Resources.LoadAll<GameObject>("Prefabs/Map");
        if (_gameoObjects.Length == 0)
            return;

        for (int i = 0; i < _gameoObjects.Length; i++)
        {
            string _mapName = _gameoObjects[i].name;
            string _collisionName = "Tilemap_Collision";
            string _baseMapName = "Tilemap_Base";
            // Extract the collision tilemap from the map grid.
            Tilemap _tileMapBase = Util.FindChild<Tilemap>(_gameoObjects[i], _baseMapName, true);
            Tilemap _tileMap = Util.FindChild<Tilemap>(_gameoObjects[i], _collisionName, true);

            // Create a file with the collision coordinates to send to the server.
            using (var writer = File.CreateText($"{pathPrefix}{_mapName}.txt"))
            {
                // Minimam and maximam size of Map
                writer.WriteLine(_tileMapBase.cellBounds.xMin);
                writer.WriteLine(_tileMapBase.cellBounds.xMax);
                writer.WriteLine(_tileMapBase.cellBounds.yMin);
                writer.WriteLine(_tileMapBase.cellBounds.yMax);

                for (int y = _tileMapBase.cellBounds.yMax; y >= _tileMapBase.cellBounds.yMin; y--)
                {
                    for (int x = _tileMapBase.cellBounds.xMin; x <= _tileMapBase.cellBounds.xMax; x++)
                    {
                        TileBase _tile = _tileMap.GetTile(new Vector3Int(x, y, 0));
                        if (_tile != null)
                            writer.Write("1");
                        else
                            writer.Write("0");
                    }
                    writer.WriteLine();
                }

                Debug.Log($"Completed [{_mapName}] information file!!");
            }
        }
    }


#endif
}

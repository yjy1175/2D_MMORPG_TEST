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
        GameObject[] _gameoObjects = Resources.LoadAll<GameObject>("Prefabs/Map");
        if (_gameoObjects.Length == 0)
            return;

        for(int i = 0; i < _gameoObjects.Length; i++)
        {
            string _mapName = _gameoObjects[i].name;
            string _collisionName = "Tilemap_Collision";
            // Extract the collision tilemap from the map grid.
            Tilemap _tileMap = Util.FindChild<Tilemap>(_gameoObjects[i], _collisionName, true);

            // Create a file with the collision coordinates to send to the server.
            using (var writer = File.CreateText($"Assets/Resources/Map/{_mapName}.txt"))
            {
                // Minimam and maximam size of Map
                writer.WriteLine(_tileMap.cellBounds.xMin);
                writer.WriteLine(_tileMap.cellBounds.xMax);
                writer.WriteLine(_tileMap.cellBounds.yMin);
                writer.WriteLine(_tileMap.cellBounds.yMax);

                for (int y = _tileMap.cellBounds.yMax; y >= _tileMap.cellBounds.yMin; y--)
                {
                    for (int x = _tileMap.cellBounds.xMin; x <= _tileMap.cellBounds.xMax; x++)
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

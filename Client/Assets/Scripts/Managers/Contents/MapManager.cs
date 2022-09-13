using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager
{
    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }
    bool[,] collision;

    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x > MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y > MaxY)
            return false;

        return !collision[MaxY - cellPos.y, cellPos.x - MinX];
    }
    public void LoadMap(int mapId)
    {
        DestoryMap();

        string _mapName = $"Map_{mapId.ToString("000")}";
        GameObject _go = Managers.Resource.Instantiate($"Map/{_mapName}");
        _go.name = "Map";

        GameObject _col = Util.FindChild(_go, "Tilemap_Collision", true);
        if (_col != null)
            _col.SetActive(false);

        CurrentGrid = _go.GetComponent<Grid>();

        // Collision 관련 파일
        TextAsset _txt = Managers.Resource.Load<TextAsset>($"Map/{_mapName}");
        StringReader _reader = new StringReader(_txt.text);

        MinX = int.Parse(_reader.ReadLine());
        MaxX = int.Parse(_reader.ReadLine());
        MinY = int.Parse(_reader.ReadLine());
        MaxY = int.Parse(_reader.ReadLine());

        int _xCount = MaxX - MinX + 1;
        int _yCount = MaxY - MinY + 1;

        collision = new bool[_yCount, _xCount];
        for(int y = 0; y < _yCount; y++)
        {
            string _line = _reader.ReadLine();
            for(int x = 0; x < _xCount; x++)
            {
                collision[y, x] = _line[x] == '1' ? true : false;
            }
        }
    }

    public void DestoryMap()
    {
        GameObject _map = GameObject.Find("Map");
        if (_map != null)
        {
            GameObject.Destroy(_map);
            CurrentGrid = null;
        }
    }
}

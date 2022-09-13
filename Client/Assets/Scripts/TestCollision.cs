using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestCollision : MonoBehaviour
{
    public Tilemap tileMap;
    public TileBase testTileBase;

    // Start is called before the first frame update
    void Start()
    {
        tileMap.SetTile(new Vector3Int(0, 0, 0), testTileBase);
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3Int> blocked = new List<Vector3Int>();

        foreach (Vector3Int pos in tileMap.cellBounds.allPositionsWithin)
        {
            TileBase _tile = tileMap.GetTile(pos);
            if (_tile != null)
                blocked.Add(pos);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>();

    public void Add(PlayerInfo info, bool myPlayer = false)
    {
        if (myPlayer)
        {
            GameObject _go = Managers.Resource.Instantiate("Creature/MyPlayer");
            _go.name = info.Name;
            objects.Add(info.PlayerId, _go);

            MyPlayer = _go.GetComponent<MyPlayerController>();
            MyPlayer.id = info.PlayerId;
            MyPlayer.PosInfo = info.PosInfo;
        }
        else
        {
            GameObject _go = Managers.Resource.Instantiate("Creature/Player");
            _go.name = info.Name;
            objects.Add(info.PlayerId, _go);

            PlayerController _player = _go.GetComponent<PlayerController>();
            _player.id = info.PlayerId;
            _player.PosInfo = info.PosInfo;
        }
    }
    public void Remove(int id)
    {
        GameObject go = Find(id);
        if (go == null)
            return;

        objects.Remove(id);
        Managers.Resource.Destroy(go);
    }

    public void RemoveMyPlayer()
    {
        if (MyPlayer = null)
            return;

        objects.Remove(MyPlayer.id);
        MyPlayer = null;
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach(GameObject obj in objects.Values)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            if (cc.CellPosition == cellPos)
                return obj;
        }

        return null;
    }

    public List<GameObject> Find(List<Vector3Int> cellPositions)
    {
        List<GameObject> objs = new List<GameObject>();
        foreach (GameObject obj in objects.Values)
        {
            CreatureController cc = obj.GetComponent<CreatureController>();
            if (cc == null)
                continue;

            foreach(Vector3Int pos in cellPositions)
            {
                if (cc.CellPosition == pos)
                    objs.Add(obj);
            }
        }

        return objs;
    }

    public GameObject Find(int id)
    {
        GameObject go = null;
        objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject obj in objects.Values)
        {
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        foreach (GameObject obj in objects.Values)
        {
            Managers.Resource.Destroy(obj);
        }
        objects.Clear();
    }
}

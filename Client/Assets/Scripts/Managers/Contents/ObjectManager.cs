using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager
{
    //Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>();
    List<GameObject> objects = new List<GameObject>();

    public void Add(GameObject go)
    {
        objects.Add(go);
    }

    public void Remove(GameObject go)
    {
        objects.Remove(go);
    }

    public GameObject Find(Vector3Int cellPos)
    {
        foreach(GameObject obj in objects)
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
        foreach (GameObject obj in objects)
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

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach (GameObject obj in objects)
        {
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        objects.Clear();
    }
}

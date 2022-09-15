﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance; // 유일성이 보장된다
    static Managers Instance { get { Init(); return s_instance; } } // 유일한 매니저를 갖고온다

    #region Contents
    MapManager map = new MapManager();
    ObjectManager obj = new ObjectManager();
    NetworkManager network = new NetworkManager();
    
    public static MapManager Map { get { return Instance.map; } }

    public static ObjectManager Obj { get { return Instance.obj; } }
    public static NetworkManager NetWork { get { return Instance.network; } }
	#endregion

	#region Core
	DataManager data = new DataManager();
    PoolManager pool = new PoolManager();
    ResourceManager resource = new ResourceManager();
    SceneManagerEx scene = new SceneManagerEx();
    SoundManager sound = new SoundManager();
    UIManager ui = new UIManager();

    public static DataManager Data { get { return Instance.data; } }
    public static PoolManager Pool { get { return Instance.pool; } }
    public static ResourceManager Resource { get { return Instance.resource; } }
    public static SceneManagerEx Scene { get { return Instance.scene; } }
    public static SoundManager Sound { get { return Instance.sound; } }
    public static UIManager UI { get { return Instance.ui; } }
	#endregion

	void Start()
    {
        Init();
	}

    void Update()
    {
        network.Update();
    }

    static void Init()
    {
        if (s_instance == null)
        {
			GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance.network.Init();
            s_instance.data.Init();
            s_instance.pool.Init();
            s_instance.sound.Init();
        }		
	}

    public static void Clear()
    {
        Sound.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}

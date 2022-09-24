using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        Managers.Map.LoadMap(1);

        Screen.SetResolution(640, 480, false);

        //GameObject _player = Managers.Resource.Instantiate("Creature/Player");
        //_player.name = "Player";
        //Managers.Obj.Add(_player);

        //for(int i = 0; i < 10; i++)
        //{
        //    GameObject _monster = Managers.Resource.Instantiate("Creature/Monster");
        //    _monster.name = $"Monster_{(i+1).ToString("000")}";

        //    // 랜덤 위치 스폰
        //    Vector3Int _pos = new Vector3Int()
        //    {
        //        x = Random.Range(-20, 20),
        //        y = Random.Range(-10, 10)
        //    };

        //    MonsterController _mc = _monster.GetComponent<MonsterController>();
        //    _mc.CellPosition = _pos;

        //    Managers.Obj.Add(_monster);
        //}

        //Managers.UI.ShowSceneUI<UI_Inven>();
        //Dictionary<int, Data.Stat> dict = Managers.Data.StatDict;
        //gameObject.GetOrAddComponent<CursorController>();

        //GameObject player = Managers.Game.Spawn(Define.WorldObject.Player, "UnityChan");
        //Camera.main.gameObject.GetOrAddComponent<CameraController>().SetPlayer(player);

        ////Managers.Game.Spawn(Define.WorldObject.Monster, "Knight");
        //GameObject go = new GameObject { name = "SpawningPool" };
        //SpawningPool pool = go.GetOrAddComponent<SpawningPool>();
        //pool.SetKeepMonsterCount(2);
    }

    public override void Clear()
    {
        
    }
}

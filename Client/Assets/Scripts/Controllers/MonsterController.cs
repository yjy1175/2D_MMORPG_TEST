using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    Coroutine coPatrol;
    Coroutine coSearch;
    Coroutine coSkill;

    Vector3Int destCellPos;

    GameObject target;

    float searchRange = 5f;
    float skillRange = 1f;

    bool rangeSkill = false;
    public override CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            base.State = value;

            if (coPatrol != null)
            {
                StopCoroutine(coPatrol);
                coPatrol = null;
            }

            if (coSearch != null)
            {
                StopCoroutine(coSearch);
                coSearch = null;
            }
        }
    }
    protected override void Init()
    {
        base.Init();

        State = CreatureState.Idle;
        Dir = MoveDirection.Down;
        speed = 3f;
        rangeSkill = Random.Range(0, 2) == 0 ? true : false;

        if (rangeSkill)
            skillRange = 10f;
        else
            skillRange = 1f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if(coPatrol == null)
        {
            coPatrol = StartCoroutine("CoPatrol");
        }

        if (coSearch == null)
        {
            coSearch = StartCoroutine("CoSearch");
        }
    }

    protected override void MoveToNextPos()
    {
        Vector3Int _destPos = destCellPos;
        if(target != null)
        {
            _destPos = target.GetComponent<CreatureController>().CellPosition;

            Vector3Int _dir = _destPos - CellPosition;
            if(_dir.magnitude <= skillRange && (_dir.x == 0 || _dir.y == 0))
            {
                Dir = GetDirFromVector(_dir);
                State = CreatureState.Skill;

                if(rangeSkill)
                    coSkill = StartCoroutine("CoStartShootArrow");
                else
                    coSkill = StartCoroutine("CoStartPunch");

                return;
            }
        }

        List<Vector3Int> _path = Managers.Map.FindPath(CellPosition, _destPos, ignoreDestCollision: true);

        // target이 너무 멀어졌거나 길을 찾지 못하는 경우
        if(_path.Count < 2 || (target != null &&_path.Count > 10))
        {
            target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int _nextPos = _path[1];


        // TODO : 길찾기
        Vector3Int _moveCellDir = _nextPos - CellPosition;

        Dir = GetDirFromVector(_moveCellDir);

        if (Managers.Map.CanGo(_nextPos) && Managers.Obj.Find(_nextPos) == null)
        {
            CellPosition = _nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public override void OnDamaged()
    {
        GameObject _effect = Managers.Resource.Instantiate("Effect/DieEffect");
        _effect.transform.position = transform.position;
        _effect.GetComponent<Animator>().Play("start");
        Managers.Resource.Destroy(_effect, 0.5f);
        Managers.Obj.Remove(id);
        Managers.Resource.Destroy(gameObject);
    }

    IEnumerator CoPatrol()
    {
        int _watiSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(_watiSeconds);

        for(int i = 0; i < 10; i++)
        {
            int _xRange = Random.Range(-5, 6);
            int _yRange = Random.Range(-5, 6);
            Vector3Int _randPos = CellPosition + new Vector3Int(_xRange, _yRange, 0);

            if(Managers.Map.CanGo(_randPos) && Managers.Obj.Find(_randPos) == null)
            {
                destCellPos = _randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle;
    }

    IEnumerator CoSearch()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);

            if (target != null)
                continue;

            target = Managers.Obj.Find((go) =>
            {
                PlayerController _pc = go.GetComponent<PlayerController>();
                if (_pc == null)
                    return false;

                Vector3Int _dir = _pc.CellPosition - CellPosition;
                if (_dir.magnitude > searchRange)
                    return false;

                return true;
            });
        }
    }

    IEnumerator CoStartPunch()
    {
        // 피격판정
        List<GameObject> _gos = Managers.Obj.Find(GetFrontCellPosition(1, 1));
        if (_gos.Count != 0)
        {
            foreach (GameObject go in _gos)
            {
                CreatureController _cc = go.GetComponent<CreatureController>();
                if (_cc != null)
                    _cc.OnDamaged();
            }
        }
        // 대기시간
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        coSkill = null;
    }

    IEnumerator CoStartShootArrow()
    {
        // 화살 발사
        GameObject _go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController _ac = _go.GetComponent<ArrowController>();
        _ac.Dir = Dir;
        _ac.CellPosition = CellPosition;

        // 대기시간
        rangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Moving;
        coSkill = null;
    }
}

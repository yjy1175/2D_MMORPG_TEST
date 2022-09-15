using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    Coroutine coSkill;
    bool rangeSkill = false;
    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }
        
        base.UpdateController();
    }

    private void LateUpdate()
    {

        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    protected override void UpdateIdle()
    {
        if(Dir != MoveDirection.None)
        {
            State = CreatureState.Moving;
            return;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            State = CreatureState.Skill;
            coSkill = StartCoroutine("CoStartPunch");
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            State = CreatureState.Skill;
            coSkill = StartCoroutine("CoStartShootArrow");
        }
    }

    protected override void UpdateAnimation()
    {
        if (state == CreatureState.Idle)
        {
            switch (lastDir)
            {
                case MoveDirection.Up:
                    anim.Play("idle_back");
                    sprite.flipX = false;
                    break;
                case MoveDirection.Down:
                    anim.Play("idle_front");
                    sprite.flipX = false;
                    break;
                case MoveDirection.Left:
                    anim.Play("idle_side");
                    sprite.flipX = true;
                    break;
                case MoveDirection.Right:
                    anim.Play("idle_side");
                    sprite.flipX = false;
                    break;
            }
        }
        else if (state == CreatureState.Moving)
        {
            switch (dir)
            {
                case MoveDirection.Up:
                    anim.Play("walk_back");
                    sprite.flipX = false;
                    break;
                case MoveDirection.Down:
                    anim.Play("walk_front");
                    sprite.flipX = false;
                    break;
                case MoveDirection.Left:
                    sprite.flipX = true;
                    anim.Play("walk_side");
                    break;
                case MoveDirection.Right:
                    anim.Play("walk_side");
                    sprite.flipX = false;
                    break;
            }
        }
        else if (state == CreatureState.Skill)
        {
            switch (lastDir)
            {
                case MoveDirection.Up:
                    anim.Play(rangeSkill ? "attack_weapon_back" : "attack_back");
                    sprite.flipX = false;
                    break;
                case MoveDirection.Down:
                    anim.Play(rangeSkill ? "attack_weapon_front" : "attack_front");
                    sprite.flipX = false;
                    break;
                case MoveDirection.Left:
                    sprite.flipX = true;
                    anim.Play(rangeSkill ? "attack_weapon_side" : "attack_side");
                    break;
                case MoveDirection.Right:
                    anim.Play(rangeSkill ? "attack_weapon_side" : "attack_side");
                    sprite.flipX = false;
                    break;
            }
        }
        else
        {

        }
    }

    // 키보드 입력
    private void GetDirInput()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Dir = MoveDirection.Up;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Dir = MoveDirection.Down;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Dir = MoveDirection.Left;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Dir = MoveDirection.Right;
        }
        else
        {
            Dir = MoveDirection.None;
        }
    }

    IEnumerator CoStartPunch()
    {
        // 피격판정
        List<GameObject> _gos = Managers.Obj.Find(GetFrontCellPosition(1, 1));
        if (_gos.Count != 0)
        {
            foreach(GameObject go in _gos)
            {
                CreatureController _cc = go.GetComponent<CreatureController>();
                if (_cc != null)
                    _cc.OnDamaged();
            }   
        }
        // 대기시간
        rangeSkill = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        coSkill = null;
    }

    IEnumerator CoStartShootArrow()
    {
        // 화살 발사
        GameObject _go = Managers.Resource.Instantiate("Creature/Arrow");
        ArrowController _ac = _go.GetComponent<ArrowController>();
        _ac.Dir = lastDir;
        _ac.CellPosition = CellPosition;

        // 대기시간
        rangeSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        coSkill = null;
    }

    public override void OnDamaged()
    {
        Debug.Log("Player Hit!");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    protected Coroutine coSkill;
    protected bool rangeSkill = false;
    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {   
        base.UpdateController();
    }

    protected override void UpdateAnimation()
    {
        if (sprite == null || anim == null)
            return;
        if (State == CreatureState.Idle)
        {
            switch (Dir)
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
        else if (State == CreatureState.Moving)
        {
            switch (Dir)
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
        else if (State == CreatureState.Skill)
        {
            switch (Dir)
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

    public void UseSkill(int skillId)
    {
        if(skillId == 1)
        {
            coSkill = StartCoroutine("CoStartPunch");
        }
        else if (skillId == 2)
        {
            coSkill = StartCoroutine("CoStartShootArrow");
        }
    }

    protected virtual void CheckUpdatedFlag()
    {

    }

    IEnumerator CoStartPunch()
    {
        // 대기시간
        rangeSkill = false;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        coSkill = null;
        CheckUpdatedFlag();
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
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        coSkill = null;
        CheckUpdatedFlag();
    }

    public override void OnDamaged()
    {
        Debug.Log("Player Hit!");
    }
}

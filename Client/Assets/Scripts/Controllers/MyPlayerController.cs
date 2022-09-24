using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
    bool moveKeyPressed = false;
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

    protected override void UpdateIdle()
    {
        if (moveKeyPressed)
        {
            State = CreatureState.Moving;
            return;
        }

        if (coSkillCoolTime == null && Input.GetKey(KeyCode.LeftControl))
        {
            C_Skill skill = new C_Skill() { Info = new SkillInfo() };
            skill.Info.SkillId = 1;

            Managers.NetWork.Send(skill);

            coSkillCoolTime = StartCoroutine("CoSkillCoolTime", 0.2f);
        }
        else if (coSkillCoolTime == null && Input.GetKey(KeyCode.LeftShift))
        {
            C_Skill skill = new C_Skill() { Info = new SkillInfo() };
            skill.Info.SkillId = 2;

            Managers.NetWork.Send(skill);

            coSkillCoolTime = StartCoroutine("CoSkillCoolTime", 0.2f);
        }
    }

    Coroutine coSkillCoolTime;
    IEnumerator CoSkillCoolTime(float time)
    {
        yield return new WaitForSeconds(time);
        coSkillCoolTime = null;
    }

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    private void GetDirInput()
    {
        moveKeyPressed = true;

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
            moveKeyPressed = false;
        }
    }

    protected override void MoveToNextPos()
    {
        if (moveKeyPressed == false)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();
            return;
        }

        Vector3Int destPos = CellPosition;
        switch (Dir)
        {
            case MoveDirection.Up:
                destPos += Vector3Int.up;
                break;
            case MoveDirection.Down:
                destPos += Vector3Int.down;
                break;
            case MoveDirection.Left:
                destPos += Vector3Int.left;
                break;
            case MoveDirection.Right:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Obj.Find(destPos) == null)
            {
                CellPosition = destPos;
            }
        }

        CheckUpdatedFlag();
    }

    protected override void CheckUpdatedFlag()
    {
        if (updated)
        {
            C_Move movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.NetWork.Send(movePacket);
            updated = false;
        }
    }
}

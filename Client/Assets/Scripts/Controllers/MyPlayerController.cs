using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
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
        if (Dir != MoveDirection.None)
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

    private void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

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
}

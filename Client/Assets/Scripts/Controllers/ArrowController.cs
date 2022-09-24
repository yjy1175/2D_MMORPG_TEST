using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class ArrowController : CreatureController
{
    protected override void Init()
    {
        // TODO : 방향 설정
        switch (lastDir)
        {
            case MoveDirection.Up:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            case MoveDirection.Down:
                transform.rotation = Quaternion.Euler(0, 0, -180);
                break;
            case MoveDirection.Left:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case MoveDirection.Right:
                transform.rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        State = CreatureState.Moving;
        speed = 15.0f;

        base.Init();
    }

    protected override void UpdateAnimation()
    {
        
    }

    protected override void MoveToNextPos()
    {
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
            GameObject _go = Managers.Obj.Find(destPos);
            if (_go == null)
            {
                CellPosition = destPos;
            }
            else
            {
                CreatureController _cc = _go.GetComponent<CreatureController>();
                if(_cc != null)
                    _cc.OnDamaged();
                Managers.Resource.Destroy(gameObject);
            }
        }
        else
        {
            Managers.Resource.Destroy(gameObject);
        }
    }
}

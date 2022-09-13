using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;

    Vector3Int cellPosition = Vector3Int.zero;

    MoveDirection dir = MoveDirection.Down;
    bool isMoving = false;

    Animator anim;

    public MoveDirection Dir
    {
        get { return dir; }
        set 
        {
            if (dir == value)
                return;

            switch (value)
            {
                case MoveDirection.Up:
                    anim.Play("walk_back");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case MoveDirection.Down:
                    anim.Play("walk_front");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case MoveDirection.Left:
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                    anim.Play("walk_side");
                    break;
                case MoveDirection.Right:
                    anim.Play("walk_side");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case MoveDirection.None:
                    if (dir == MoveDirection.Up)
                    {
                        anim.Play("idle_back");
                        transform.localScale = new Vector3(1f, 1f, 1f);
                    } 
                    else if (dir == MoveDirection.Down)
                    {
                        anim.Play("idle_front");
                        transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                        
                    else if(dir == MoveDirection.Left)
                    {
                        anim.Play("idle_side");
                        transform.localScale = new Vector3(-1f, 1f, 1f);
                    }   
                    else
                    {
                        anim.Play("idle_side");
                        transform.localScale = new Vector3(1f, 1f, 1f);
                    }
                    break;
            }

            dir = value;
        }
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        Vector3 _pos = Managers.Map.CurrentGrid.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, 0);
        transform.position = _pos;
    }

    private void Update()
    {
        GetDirInput();
        UpdatePosition();
        UpdateMoving();
    }

    private void LateUpdate()
    {

        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
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
    // 실질적인 이동
    private void UpdateMoving()
    {
        if (!isMoving && dir != MoveDirection.None)
        {
            Vector3Int destPos = cellPosition;
            switch (dir)
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
                cellPosition = destPos;
                isMoving = true;
            }
        }
    }
    // 이동 가능할 상태일때, 실제 좌표로 이동한다.
    private void UpdatePosition()
    {
        if (!isMoving)
            return;

        Vector3 _desPos = Managers.Map.CurrentGrid.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, 0);
        Vector3 _moveDir = _desPos - transform.position;

        // 도착 여부 체크
        float _dist = _moveDir.magnitude;
        if(_dist < speed * Time.deltaTime)
        {
            transform.position = _desPos;
            isMoving = false;
        }
        else
        {
            transform.position += _moveDir.normalized * speed * Time.deltaTime;
            isMoving = true;
        }
    }

}

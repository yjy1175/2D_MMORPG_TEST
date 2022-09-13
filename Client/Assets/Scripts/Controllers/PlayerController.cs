using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    public Grid grid;
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
        Vector3 _pos = grid.CellToWorld(cellPosition);
        transform.position = _pos;
    }

    void Update()
    {
        GetDirInput();
        UpdatePosition();
        UpdateMoving();
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
        if (!isMoving)
        {
            switch (dir)
            {
                case MoveDirection.Up:
                    cellPosition += Vector3Int.up;
                    isMoving = true;
                    break;
                case MoveDirection.Down:
                    cellPosition += Vector3Int.down;
                    isMoving = true;
                    break;
                case MoveDirection.Left:
                    cellPosition += Vector3Int.left;
                    isMoving = true;
                    break;
                case MoveDirection.Right:
                    cellPosition += Vector3Int.right;
                    isMoving = true;
                    break;
            }
        }
    }
    // 이동 가능할 상태일때, 실제 좌표로 이동한다.
    private void UpdatePosition()
    {
        if (!isMoving)
            return;

        Vector3 _desPos = grid.CellToWorld(cellPosition);
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

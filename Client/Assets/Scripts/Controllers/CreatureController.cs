using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    public int id { get; set; }
    public float speed = 5.0f;

    protected bool updated = false;

    PositionInfo positionInfo = new PositionInfo();
    public PositionInfo PosInfo
    {
        get { return positionInfo; }
        set
        {
            if (positionInfo.Equals(value))
                return;

            CellPosition = new Vector3Int(value.PosX, value.PosY, 0);
            State = value.State;
            Dir = value.MoveDir;
        }
    }

    public Vector3Int CellPosition 
    {
        get
        {
            return new Vector3Int(PosInfo.PosX, PosInfo.PosY, 0);
        }
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            updated = true;
        }
    }
    protected Animator anim;
    protected SpriteRenderer sprite;

    public virtual CreatureState State
    {
        get { return PosInfo.State; }
        set
        {
            if (PosInfo.State == value)
                return;

            PosInfo.State = value;
            UpdateAnimation();
            updated = true;
        }
    }

    protected MoveDirection lastDir = MoveDirection.Down;
    public MoveDirection Dir
    {
        get { return PosInfo.MoveDir; }
        set
        {
            if (PosInfo.MoveDir == value)
                return;
            PosInfo.MoveDir = value;
            if (value != MoveDirection.None)
                lastDir = value;

            UpdateAnimation();
            updated = true;
        }
    }

    public MoveDirection GetDirFromVector(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDirection.Right;
        else if (dir.x < 0)
            return MoveDirection.Left;
        else if (dir.y > 0)
            return MoveDirection.Up;
        else if (dir.y < 0)
            return MoveDirection.Down;
        else
            return MoveDirection.None;
    }

    public List<Vector3Int> GetFrontCellPosition(int xRange, int yRange)
    {
        List<Vector3Int> _cellPositions = new List<Vector3Int>();
        Vector3Int _cellPos = CellPosition;

        for(int i = 1; i <= xRange; i++)
        {
            switch (lastDir)
            {
                case MoveDirection.Up:
                    _cellPos += Vector3Int.up;
                    break;
                case MoveDirection.Down:
                    _cellPos += Vector3Int.down;
                    break;
                case MoveDirection.Left:
                    _cellPos += Vector3Int.left;
                    break;
                case MoveDirection.Right:
                    _cellPos += Vector3Int.right;
                    break;
            }

            _cellPositions.Add(_cellPos);
        }


        return _cellPositions;
    }

    protected virtual void UpdateAnimation()
    {
        if(State == CreatureState.Idle)
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
        else if(State == CreatureState.Moving)
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
        else if(State == CreatureState.Skill)
        {
            switch (lastDir)
            {
                case MoveDirection.Up:
                    anim.Play("attack_back");
                    sprite.flipX = false;
                    break;
                case MoveDirection.Down:
                    anim.Play("attack_front");
                    sprite.flipX = false;
                    break;
                case MoveDirection.Left:
                    sprite.flipX = true;
                    anim.Play("attack_side");
                    break;
                case MoveDirection.Right:
                    anim.Play("attack_side");
                    sprite.flipX = false;
                    break;
            }
        }
        else
        {

        }
    }

    void Start()
    {
        Init();
    }

    private void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        Vector3 _pos = Managers.Map.CurrentGrid.CellToWorld(CellPosition) + new Vector3(0.5f, 0.5f, 0);
        transform.position = _pos;

        State = CreatureState.Idle;
        Dir = MoveDirection.None;
        CellPosition = new Vector3Int(0, 0, 0);
        UpdateAnimation();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }

        
    }

    protected virtual void UpdateIdle()
    {

    }

    // 스르륵 이동하는 것을 처리
    protected virtual void UpdateMoving()
    {
        Vector3 _desPos = Managers.Map.CurrentGrid.CellToWorld(CellPosition) + new Vector3(0.5f, 0.5f, 0);
        Vector3 _moveDir = _desPos - transform.position;

        // 도착 여부 체크
        float _dist = _moveDir.magnitude;
        if (_dist < speed * Time.deltaTime)
        {
            transform.position = _desPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += _moveDir.normalized * speed * Time.deltaTime;
            State = CreatureState.Moving;
        }
    }

    protected virtual void MoveToNextPos()
    {

    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }

    public virtual void OnDamaged()
    {

    }

}

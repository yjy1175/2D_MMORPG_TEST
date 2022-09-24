using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Player
    {
        public PlayerInfo Info { get; set; }
            = new PlayerInfo() { PosInfo = new PositionInfo() };
        public GameRoom Room { get; set; }
        public ClientSession Session { get; set; }

        public Vector2Int CellPos
        {
            get { return new Vector2Int(Info.PosInfo.PosX, Info.PosInfo.PosY); }
            set
            {
                Info.PosInfo.PosX = value.x;
                Info.PosInfo.PosY = value.y;
            }
        }

        public List<Vector2Int> GetFrontCellPosition(MoveDirection dir, int xRange = 1, int yRange = 1)
        {
            List<Vector2Int> _cellPositions = new List<Vector2Int>();
            Vector2Int _cellPos = CellPos;

            for (int i = 1; i <= xRange; i++)
            {
                switch (dir)
                {
                    case MoveDirection.Up:
                        _cellPos += Vector2Int.up;
                        break;
                    case MoveDirection.Down:
                        _cellPos += Vector2Int.down;
                        break;
                    case MoveDirection.Left:
                        _cellPos += Vector2Int.left;
                        break;
                    case MoveDirection.Right:
                        _cellPos += Vector2Int.right;
                        break;
                }
                _cellPositions.Add(_cellPos);
            }

            return _cellPositions;
        }

        public void OnDamaged()
        {
            Console.WriteLine($"Hit => [{Info.PlayerId}]Player");
        }
    }
}

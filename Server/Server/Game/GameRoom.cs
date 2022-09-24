using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        Dictionary<int, Player> _players = new Dictionary<int, Player>();

        Map _map = new Map();

        public void Init(int mapId)
        {
            _map.LoadMap(mapId);
        }

        // 입장
        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock (_lock)
            {
                _players.Add(newPlayer.Info.PlayerId, newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                {
                    // 본인 정보 전송
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);

                    // 본인을 제외한 유저들 정보 전송
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach(Player p in _players.Values)
                    {
                        if(p != newPlayer)
                            spawnPacket.Players.Add(p.Info);
                    }
                    newPlayer.Session.Send(spawnPacket);
                }

                // 타인에게 본인 정보 전송
                {
                    S_Spawn spawnPacket = new S_Spawn();
                    spawnPacket.Players.Add(newPlayer.Info);
                    foreach (Player p in _players.Values)
                    {
                        if (p != newPlayer)
                            p.Session.Send(spawnPacket);
                    }
                }
            }
        }
        // 퇴장
        public void LeaveGame(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.Remove(playerId, out player) == false)
                    return;

                player.Room = null;

                // 본인에게 정보 전송
                {
                    S_LeaveGame leavePacket = new S_LeaveGame();
                    player.Session.Send(leavePacket);
                }

                // 타인에게 정보 전송
                {
                    S_Despawn despawnPacket = new S_Despawn();
                    despawnPacket.PlayerId.Add(player.Info.PlayerId);

                    foreach(Player p in _players.Values)
                    {
                        if (p != player)
                            p.Session.Send(despawnPacket);
                    }
                }
            }
        }
        // 뿌려주기
        public void Broadcast(IMessage packet)
        {
            lock (_lock)
            {
                foreach (Player p in _players.Values)
                {
                    p.Session.Send(packet);
                }
            }
        }
        // 이동 동기화
        public void HandleMove(Player player, C_Move movePacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                // 이동 검증
                PositionInfo movePosInfo = movePacket.PosInfo;
                PlayerInfo info = player.Info;

                // 이동할 경우 갈 수 있는 곳 인지 체크
                if (info.PosInfo.PosX != movePosInfo.PosX || info.PosInfo.PosY != movePosInfo.PosY)
                {
                    if (!_map.CanGo(new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)))
                        return;
                }

                info.PosInfo.State = movePosInfo.State;
                info.PosInfo.MoveDir = movePosInfo.MoveDir;
                if (!_map.ApplyMove(player, new Vector2Int(movePosInfo.PosX, movePosInfo.PosY)))
                    return;

                // 브로드캐스팅
                S_Move resMovePacket = new S_Move();
                resMovePacket.PlayerId = player.Info.PlayerId;
                resMovePacket.PosInfo = movePacket.PosInfo;

                Broadcast(resMovePacket);
            }
        }
        // 스킬 동기화
        public void HandleSkill(Player player, C_Skill skillPacket)
        {
            if (player == null)
                return;

            lock (_lock)
            {
                PlayerInfo info = player.Info;
                if (info.PosInfo.State != CreatureState.Idle)
                    return;

                // TODO: 스킬 가능 여부 체크

                // 통과
                info.PosInfo.State = CreatureState.Skill;

                S_Skill resSkillPacket = new S_Skill() { Info = new SkillInfo() };
                resSkillPacket.PlayerId = info.PlayerId;
                resSkillPacket.Info.SkillId = skillPacket.Info.SkillId;

                Broadcast(resSkillPacket);

                // TODO: 데미지 판정
                List<Vector2Int> skillPos = player.GetFrontCellPosition(info.PosInfo.MoveDir);
                foreach(Vector2Int pos in skillPos)
                {
                    Player target = _map.FindByPos(pos);
                    if (target != null)
                        target.OnDamaged();
                }
            }
        }
    }
}

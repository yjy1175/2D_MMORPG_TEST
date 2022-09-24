﻿using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class GameRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        List<Player> _players = new List<Player>();

        // 입장
        public void EnterGame(Player newPlayer)
        {
            if (newPlayer == null)
                return;

            lock (_lock)
            {
                _players.Add(newPlayer);
                newPlayer.Room = this;

                // 본인한테 정보 전송
                {
                    // 본인 정보 전송
                    S_EnterGame enterPacket = new S_EnterGame();
                    enterPacket.Player = newPlayer.Info;
                    newPlayer.Session.Send(enterPacket);

                    // 본인을 제외한 유저들 정보 전송
                    S_Spawn spawnPacket = new S_Spawn();
                    foreach(Player p in _players)
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
                    foreach (Player p in _players)
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
                Player player = _players.Find(p => p.Info.PlayerId == playerId);
                if (player == null)
                    return;

                _players.Remove(player);
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

                    foreach(Player p in _players)
                    {
                        if (p != player)
                            p.Session.Send(despawnPacket);
                    }
                }
            }
        }
    }
}

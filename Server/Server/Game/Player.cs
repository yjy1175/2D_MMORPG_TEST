using System;
using System.Collections.Generic;
using System.Text;
using Google.Protobuf.Protocol;

namespace Server.Game
{
    public class Player
    {
        public PlayerInfo Info { get; set; } = new PlayerInfo();
        public GameRoom Room { get; set; }
        public ClientSession Session { get; set; }
        
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public class PlayerManager
    {
        public static PlayerManager Instance { get; } = new PlayerManager();

        object _lock = new object();
        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        int _playerId = 1; // TODO

        public Player Add()
        {
            Player newPlayer = new Player();

            lock (_lock)
            {
                newPlayer.Info.PlayerId = _playerId;
                _players.Add(_playerId, newPlayer);
                _playerId++;
            }

            return newPlayer;

        }

        public bool Remove(int playerId)
        {
            lock (_lock)
            {
                return _players.Remove(playerId);
            }
        }

        public Player Find(int playerId)
        {
            lock (_lock)
            {
                Player player = null;
                if (_players.TryGetValue(playerId, out player))
                    return player;

                return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Moko
{
    public class WorldGameSessionManager : MonoBehaviour
    {
        public static WorldGameSessionManager instance;
        
        [Header("Active Players In Session")] 
        public List<PlayerManager> players = new List<PlayerManager>();

        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);
        }

        public void AddPlayerToActivePlayersList(PlayerManager newPlayer)
        {
            if (!players.Contains(newPlayer))
            {
                players.Add(newPlayer);
            }

            // check the list for null slots, and remove the null slots
            for (int i = players.Count - 1; i >= 0; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }

        public void RemovePlayerFromActivePlayersList(PlayerManager player)
        {
            if (players.Contains(player))
            {
                players.Remove(player);
            }
            
            // check the list for null slots, and remove the null slots
            for (int i = players.Count - 1; i >= 0; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
    }
}

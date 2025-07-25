using System;
using SKC.Cards;
using UnityEngine;

namespace SKC.Events
{
    public static class EventBroker
    {
        #region Flow
        public static Action onGameStart;
        public static void OnGameStart()
        {
            onGameStart?.Invoke();
        }
        
        public static Action onGameEnd;
        public static void OnGameEnd()
        {
            onGameEnd?.Invoke();
        }

        public static Action onGameRestart;
        public static void OnGameRestart()
        {
            onGameRestart?.Invoke();
        }

        public static Action onGameSave;
        public static void OnGameSave()
        {
            onGameSave?.Invoke();
        }
        #endregion
        
        #region In-Game

        public static Action<int> onMatch;
        public static void OnMatch(int combo)
        {
            onMatch?.Invoke(combo);
        }
        
        public static Action<int> onMissMatch;
        public static void OnMissMatch(int combo)
        {
            onMissMatch?.Invoke(combo);
        }

        #endregion
    }
}

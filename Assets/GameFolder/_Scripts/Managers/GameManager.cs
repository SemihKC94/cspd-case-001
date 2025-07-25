using System;
using UnityEngine;
using SKC.Events;

namespace SKC.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CardManager cardManager;
        [SerializeField] private LevelManager levelManager;
        [SerializeField] private UIManager uiManager;
        
        //Privates
        private float _currentScore;
        private float _addedScore;
        
        // Singleton Instance
        public static GameManager Instance { get; private set; }
        
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject); 
            }
        }
        
        private void OnEnable()
        {
            EventBroker.onGameStart += InitializeGame;
            EventBroker.onGameEnd += GameEnd;
            EventBroker.onGameSave += SaveGame;
            EventBroker.onGameRestart += GameRestart;
            
            EventBroker.onMatch += GetScore;
        }

        private void OnDisable()
        {
            EventBroker.onGameStart -= InitializeGame;
            EventBroker.onGameEnd -= GameEnd;
            EventBroker.onGameSave -= SaveGame;
            EventBroker.onGameRestart -= GameRestart;

            EventBroker.onMatch -= GetScore;
        }

        public void InitializeGame()
        {
            _currentScore = 0;
            _addedScore = 0;
            uiManager.SetBackgroundColor(levelManager.GetCurrentLevel().backgroundColor);
            cardManager.InitializeGameLayout(levelManager.GetCurrentLevel());
        }

        private void GameEnd()
        {
            SaveManager.Instance.CheckHighScore(_currentScore);
            SaveManager.Instance.CurrentGameData.level++;
            SaveGame();
            
            GameRestart();
        }

        private void SaveGame()
        {
            SaveManager.Instance.SaveGame();
        }

        private void GameRestart()
        {
            _currentScore = 0;
            _addedScore = 0;
            cardManager.ResetGame();
        }

        public void GetScore(int combo)
        {
            if (combo >= 5) combo = 5;
            
            _addedScore = combo * 5;
            _currentScore += _addedScore;
            
            uiManager.UpdateScore(_currentScore,_addedScore,combo);
            
        }

        [ContextMenu("Next Level")]
        private void NextLevelTest()
        {
            EventBroker.OnGameEnd();
        }
    }
}

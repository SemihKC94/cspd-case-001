using UnityEngine;
using System.IO;
using SKC.SaveSystem; 

namespace SKC.Managers
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private string savePath;
        private const string SAVE_FILE_NAME = "gameData.json";

        public SaveData CurrentGameData { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
            }
            
            savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

            LoadGame();
        }
        
        public void SaveGame()
        {
            string jsonData = JsonUtility.ToJson(CurrentGameData, true);
            File.WriteAllText(savePath, jsonData);
#if UNITY_EDITOR
            Debug.Log($"Game saved to: {savePath}");
#endif
        }
        
        public void LoadGame()
        {
            if (File.Exists(savePath))
            {
                string jsonData = File.ReadAllText(savePath);
                CurrentGameData = JsonUtility.FromJson<SaveData>(jsonData);
#if UNITY_EDITOR
                Debug.Log($"Game loaded from: {savePath}");
#endif
            }
            else
            {
                CurrentGameData = new SaveData();
#if UNITY_EDITOR
                Debug.Log("No save file found. Creating new game data.");
#endif
            }
        }

        public void CheckHighScore(float score)
        {
            if (score >= CurrentGameData.highestPoint)
            {
                CurrentGameData.highestPoint = score;
#if UNITY_EDITOR
                Debug.Log($"Highest point updated: {CurrentGameData.highestPoint}");
#endif
            }
        }
        
        public void ResetGameData()
        {
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
#if UNITY_EDITOR
                Debug.Log("Game data file deleted.");
#endif
            }
            CurrentGameData = new SaveData();
            SaveGame();
        }
        
        // For Test
        
        [ContextMenu("Test Save Game")]
        void TestSave()
        {
            CurrentGameData.level += 1;
            CurrentGameData.highestPoint += 500;
            CurrentGameData.totalMatchesFound += 10;
            CurrentGameData.totalMovesMade += 50;
            CurrentGameData.gameVolume = 0.7f;
            CurrentGameData.sfxEnabled = false;
            SaveGame();
        }

        [ContextMenu("Test Load Game")]
        void TestLoad()
        {
            LoadGame();
            Debug.Log($"Loaded Data: Level={CurrentGameData.level}, Matches={CurrentGameData.totalMatchesFound}");
        }

        [ContextMenu("Test Reset Game Data")]
        void TestReset()
        {
            ResetGameData();
        }
    }
}
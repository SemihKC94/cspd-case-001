using System.Collections;
using System.Collections.Generic;
using SKC.Level;
using UnityEngine;

namespace SKC.Managers
{
    public class LevelManager : MonoBehaviour
    {
        [Header("Level Datas")]
        [SerializeField] private LevelData[] allLevels;

        public LevelData GetCurrentLevel()
        {
            if (SaveManager.Instance.CurrentGameData.level >= allLevels.Length)
            {
                return allLevels[Random.Range(0, allLevels.Length)];
            }
            
            return allLevels[SaveManager.Instance.CurrentGameData.level];
        }
    }
}

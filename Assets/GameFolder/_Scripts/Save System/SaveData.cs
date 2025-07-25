using System;
using System.Collections.Generic;

namespace SKC.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public int level;
        public float highestPoint;
        public int totalMatchesFound;  
        public int totalMovesMade;     
        public float gameVolume;       
        public bool sfxEnabled;        
        
        public SaveData()
        {
            level = 0;
            highestPoint = 0;
            totalMatchesFound = 0;
            totalMovesMade = 0;
            gameVolume = 1.0f;
            sfxEnabled = true;
        }
    }
}
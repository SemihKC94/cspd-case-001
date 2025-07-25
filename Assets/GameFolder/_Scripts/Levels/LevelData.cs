using System.Collections;
using System.Collections.Generic;
using SKC.Cards;
using UnityEngine;

namespace SKC.Level
{
    [CreateAssetMenu(fileName = nameof(LevelData), menuName = nameof(SKC) + "/" + nameof(Level) + "Level Data")]
    public class LevelData : ScriptableObject
    {
        public int cardCount;
        public CardPool cardPool;
        public string levelName;
        public float cardSpacing = 10f;
        public float xPadding = 10f;
        public float yPadding = 10f;
        public Color backgroundColor = Color.black;
    }
}

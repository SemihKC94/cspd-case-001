using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SKC.Cards
{
    [CreateAssetMenu(fileName = nameof(CardData), menuName = nameof(SKC) + "/" + "New " + nameof(CardData))]
    public class CardData : ScriptableObject
    {
        public int cardID;
        public Sprite cardIconSprite;
    }
}
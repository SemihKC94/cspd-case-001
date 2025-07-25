using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace SKC.Cards
{
    [CreateAssetMenu(fileName = nameof(CardPool), menuName = nameof(SKC) + "/" + "New " + nameof(CardPool))]
    public class CardPool : ScriptableObject
    {
        public List<CardData> cardPool;
    }
}
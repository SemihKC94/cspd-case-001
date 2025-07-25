using System;
using System.Collections;
using System.Collections.Generic;
using SKC.Cards;
using SKC.Events;
using SKC.Helper;
using SKC.Level;
using UnityEngine;
using UnityEngine.UI;

namespace SKC.Managers
{
    public class CardManager : MonoBehaviour
    {
        [Header("Assigns")]
        [SerializeField] private Vector2 cardSize = new Vector2(180, 300);
        [SerializeField] private Card cardPrefab; 
        [SerializeField] private RectTransform cardGridParentRect;
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        [SerializeField] private List<CardData> allCardDataList;

        // Privates
        private List<Card> allCards = new List<Card>();
        private List<Card> flippedCards = new List<Card>();
        private int matchesFound = 0;
        private int totalPairs;

        private bool isChecking = false;
        
        private int movesCount = 0;
        private int comboCount = 0;
        private int totalGameCards = 12;

        
        public int ComboCount { get { return comboCount; } }
        
        public void InitializeGameLayout(LevelData levelData)
        {
            ClearCards();
            allCardDataList = new List<CardData>();
            allCardDataList = levelData.cardPool.cardPool;
            
            totalGameCards = levelData.cardCount;
            
            Tuple<int, int> gridDimensions = GridHelper.CalculateGridDimensionsForFixedSize(totalGameCards, cardGridParentRect, 
                   cardSize.x,cardSize.y, levelData.cardSpacing, levelData.xPadding,
                   levelData.yPadding);
            int calculatedRows = gridDimensions.Item1;
            int calculatedColumns = gridDimensions.Item2;
            
            GridHelper.SetGridLayoutWithFixedSize(cardSize.x,cardSize.y,levelData.cardSpacing,calculatedRows, calculatedColumns, gridLayoutGroup, cardGridParentRect);

            totalPairs = totalGameCards / 2; 

            GenerateCards();
        }

        void ClearCards()
        {
            foreach (Card child in allCards)
            {
                PoolManager.Instance.ReturnObject(child.gameObject);
            }
            allCards.Clear();
            flippedCards.Clear();
            matchesFound = 0;
            movesCount = 0;
            comboCount = 0;
        }
        
        void GenerateCards()
        {
            ListExtension.Shuffle(allCardDataList);
            List<CardData> gameCardData = new List<CardData>();
            for (int i = 0; i < totalPairs; i++)
            {
                gameCardData.Add(allCardDataList[i]);
                gameCardData.Add(allCardDataList[i]);
            }
            
            ListExtension.Shuffle(gameCardData);

            for (int i = 0; i < gameCardData.Count; i++)
            {
                GameObject newCardGO = PoolManager.Instance.GetObject(cardPrefab.name, gridLayoutGroup.transform);
                Card newCard = newCardGO.GetComponent<Card>();
                newCard.Initialize(gameCardData[i], this);
                allCards.Add(newCard);
            }

            StartCoroutine(RevealAllCard());
        }

        private IEnumerator RevealAllCard()
        {
            yield return new WaitForSeconds(0.50f);
            foreach (Card item in allCards)
            {
                item.FlipCard();
            }
            
            yield return new WaitForSeconds(1.50f);
            
            foreach (Card item in allCards)
            {
                item.UnflipCard();
            }
        }
        

        public void CardClicked(Card card)
        {
            if (isChecking) return;

            card.FlipCard();
            flippedCards.Add(card);
            
            SoundManager.Instance.Play("Flip");

            if (flippedCards.Count == 2)
            {
                isChecking = true;
                movesCount++;
                StartCoroutine(CheckForMatchRoutine());
            }
        }

        IEnumerator CheckForMatchRoutine()
        {
            yield return new WaitForSeconds(0.30f);

            Card card1 = flippedCards[0];
            Card card2 = flippedCards[1];
            
            if (card1.GetCardID() == card2.GetCardID())
            {
                card1.SetMatched();
                card2.SetMatched();
                matchesFound++;
                
                comboCount++;
                EventBroker.OnMatch(comboCount);
                

                if (matchesFound == totalPairs)
                {
                    EventBroker.OnGameEnd();
                    SoundManager.Instance.Play("Win");
                }
                else
                {
                    SoundManager.Instance.Play("Match");
                }
            }
            else
            {
                card1.UnflipCard();
                card2.UnflipCard();

                comboCount = 0;
                EventBroker.OnMissMatch(comboCount);
                
                SoundManager.Instance.Play("MissMatch");
            }
            
            yield return new WaitForSeconds(0.70f);
            
            flippedCards.Clear();
            isChecking = false;
        }

        public void ResetGame()
        {
            ClearCards();
        }

        public bool IsChecking()
        {
            return isChecking;
        }
    }
}

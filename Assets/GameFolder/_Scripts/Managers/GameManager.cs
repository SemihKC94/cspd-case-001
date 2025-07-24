using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using SKC.Helper; 
using SKC.Cards;

namespace SKC.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Assigns")]
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
        public int totalGameCards = 12;
        public float cardSpacing = 10f;
        public float targetCardAspectRatio = 9f / 16f; 
        
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

        void Start()
        {
            InitializeGameLayout(totalGameCards); 
        }

        public void InitializeGameLayout(int totalCardsCount)
        {
            ClearCards();
            
            Tuple<int, int> gridDimensions = GridHelper.CalculateOptimalGrid(totalCardsCount, cardGridParentRect, targetCardAspectRatio);
            int calculatedRows = gridDimensions.Item1;
            int calculatedColumns = gridDimensions.Item2;
            
            GridHelper.SetGridLayout(calculatedRows, calculatedColumns, cardSpacing, gridLayoutGroup, cardGridParentRect, targetCardAspectRatio);

            totalPairs = totalCardsCount / 2; 

            GenerateCards();
            //UpdateMovesText();
        }

        void ClearCards()
        {
            foreach (Transform child in gridLayoutGroup.transform)
            {
                PoolManager.Instance.ReturnObject(child.gameObject);
            }
            allCards.Clear();
            flippedCards.Clear();
            matchesFound = 0;
            movesCount = 0;
            //UpdateMovesText();
        }

        // void UpdateMovesText()
        // {
        //     if (movesText != null)
        //     {
        //         movesText.text = "Hamle: " + movesCount;
        //     }
        // }

        void GenerateCards()
        {
            ShuffleList(allCardDataList);
            List<CardData> gameCardData = new List<CardData>();
            for (int i = 0; i < totalPairs; i++)
            {
                gameCardData.Add(allCardDataList[i]);
                gameCardData.Add(allCardDataList[i]);
            }
            
            gameCardData = ShuffleList(gameCardData);

            for (int i = 0; i < gameCardData.Count; i++)
            {
                GameObject newCardGO = PoolManager.Instance.GetObject(cardPrefab.name, gridLayoutGroup.transform);
                Card newCard = newCardGO.GetComponent<Card>();
                newCard.Initialize(gameCardData[i]);
                allCards.Add(newCard);
            }

            StartCoroutine(RevealAllCard());
        }

        private IEnumerator RevealAllCard()
        {
            foreach (Card item in allCards)
            {
                item.FlipCard();
            }
            
            yield return new WaitForSeconds(2.00f);
            
            foreach (Card item in allCards)
            {
                item.UnflipCard();
            }
        }

        List<T> ShuffleList<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
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
                //UpdateMovesText();
                StartCoroutine(CheckForMatchRoutine());
            }
        }

        IEnumerator CheckForMatchRoutine()
        {
            yield return new WaitForSeconds(0.50f);

            Card card1 = flippedCards[0];
            Card card2 = flippedCards[1];
            
            if (card1.GetCardID() == card2.GetCardID())
            {
                card1.SetMatched();
                card2.SetMatched();
                matchesFound++;
                comboCount++;

                if (matchesFound == totalPairs)
                {
                    // Oyun bitiş ekranı, yeni seviyeye geçiş vb.
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
                
                SoundManager.Instance.Play("MissMatch");
            }
            
            yield return new WaitForSeconds(0.20f);
            
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

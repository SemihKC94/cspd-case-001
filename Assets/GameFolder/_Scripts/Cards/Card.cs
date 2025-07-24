using System.Collections;
using System.Collections.Generic;
using SKC.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace SKC.Cards
{ 
    public class Card : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject cardVisualContainer;
        [SerializeField] private Sprite cardBackgroundSprite;
        [SerializeField] private Sprite cardFrontSprite;
        [SerializeField] private Image cardBackground;
        [SerializeField] private Image cardImage;
        [SerializeField] private Button cardButton;

        // Privates
        private CardData cardData;
        //private GameManager gameManager;
        private bool isFlipped = false;
        private bool isMatched = false;

        void Awake()
        {
            cardButton.onClick.AddListener(() => OnCardClicked());
        }
        
        public void Initialize(CardData data)
        {
            cardData = data;
            cardVisualContainer.gameObject.SetActive(true);
            cardBackground.sprite = cardFrontSprite;
            cardImage.sprite = data.cardIconSprite;
            cardImage.gameObject.SetActive(false);
            isFlipped = false;
            isMatched = false;
            cardButton.interactable = true;
            
        }

        void OnCardClicked()
        {
            if (!isFlipped && !isMatched && !GameManager.Instance.IsChecking())
            {
                GameManager.Instance.CardClicked(this);
            }
        }

        public void FlipCard()
        {
            if (!isFlipped)
            {
                cardImage.gameObject.SetActive(true);
                cardBackground.sprite = cardBackgroundSprite;
                isFlipped = true;
                cardButton.interactable = false;
            }
        }

        public void UnflipCard()
        {
            StartCoroutine(UnflipRoutine());
        }

        IEnumerator UnflipRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            cardImage.gameObject.SetActive(false);
            cardBackground.sprite = cardFrontSprite;
            isFlipped = false;
            if (!isMatched)
            {
                cardButton.interactable = true;
            }
        }

        public void SetMatched()
        {
            isMatched = true;
            cardButton.interactable = false;
            cardVisualContainer.gameObject.SetActive(false);
        }

        public bool IsFlipped()
        {
            return isFlipped;
        }

        public bool IsMatched()
        {
            return isMatched;
        }

        public int GetCardID()
        {
            return cardData.cardID;
        }
    }
}

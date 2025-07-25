using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
        private CardManager cardManager;
        private bool isFlipped = false;
        private bool isMatched = false;

        void Awake()
        {
            cardButton.onClick.AddListener(() => OnCardClicked());
        }
        
        public void Initialize(CardData data, CardManager cardManager)
        {
            this.cardManager = cardManager;
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
            if (!isFlipped && !isMatched && !cardManager.IsChecking())
            {
                cardManager.CardClicked(this);
            }
        }

        public void FlipCard()
        {
            if (!isFlipped)
            {
                transform.DOKill(this.gameObject);
                transform.DORotate(new Vector3(0, 90, 0), .1f)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        cardImage.gameObject.SetActive(true);
                        cardBackground.sprite = cardBackgroundSprite;
                        
                        transform.DORotate(new Vector3(0, 180, 0), .1f)
                            .SetEase(Ease.Linear)
                            .OnComplete(() =>
                            {
                                isFlipped = true;
                                cardButton.interactable = false;
                            });
                    });
            }
        }

        public void UnflipCard()
        {
            StartCoroutine(UnflipRoutine());
        }

        IEnumerator UnflipRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            
            transform.DORotate(new Vector3(0, 90, 0), .10f)
                .SetEase(Ease.Linear)
                .SetDelay(0.8f)
                .OnComplete(() =>
                {
                    cardImage.gameObject.SetActive(false);
                    cardBackground.sprite = cardFrontSprite;
                    
                    transform.DORotate(new Vector3(0, 0, 0), .10f) 
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            isFlipped = false;
                            if (!isMatched)
                            {
                                cardButton.interactable = true;
                            }

                            transform.localEulerAngles = Vector3.zero;
                        });
                });
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

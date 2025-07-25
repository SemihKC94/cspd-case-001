using DG.Tweening;
using SKC.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SKC.Managers
{
    public class UIManager : MonoBehaviour
    {
        [Space,Header("UI References")]
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button winButton;
        [SerializeField] private Button mainButton;
        [SerializeField] private Button mainButtonInSettings;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button soundButton;
        [SerializeField] private Button deleteSaveButton;
        
        [Space,Header("Panels")]
        [SerializeField] private CanvasGroup mainPanel;
        [SerializeField] private CanvasGroup gamePanel;
        [SerializeField] private CanvasGroup winPanel;
        [SerializeField] private CanvasGroup settingsPanel;
        
        [Space,Header("Image References")]
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image soundSettingsImage;
        
        [Space,Header("Sprite References")]
        [SerializeField] private Sprite soundOnSprite;
        [SerializeField] private Sprite soundOffSprite;
        
        [Space,Header("Texts")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI currentScoreText;
        [SerializeField] private TextMeshProUGUI addedScoreText;
        
        // Privates
        private CanvasGroup _currentActivePanel;
        private bool sfxEnabled = true;

        private void Awake()
        {
            SetImmediateCanvasGroups(mainPanel,true);
            SetImmediateCanvasGroups(gamePanel,false);
            SetImmediateCanvasGroups(winPanel,false);
            SetImmediateCanvasGroups(settingsPanel,false);
            
            _currentActivePanel = mainPanel;
            
            playButton.onClick.AddListener(() => PlayGame());
            winButton.onClick.AddListener(() => MainMenu());
            mainButton.onClick.AddListener(() => MainMenu());
            mainButtonInSettings.onClick.AddListener(() => MainMenu());
            settingsButton.onClick.AddListener(() => Settings());
            soundButton.onClick.AddListener(() => SoundToggle());
            deleteSaveButton.onClick.AddListener(() => DeleteSave());
        }

        private void Start()
        {
            UpdateHighScore();

            sfxEnabled = SaveManager.Instance.CurrentGameData.sfxEnabled;
        }

        private void OnEnable()
        {
            EventBroker.onGameEnd += WinGame;
        }

        private void OnDisable()
        {
            EventBroker.onGameEnd -= WinGame;
        }

        private void PlayGame()
        {
            EventBroker.OnGameStart();
            currentScoreText.text = $"SCORE: 0";
            levelText.text =  $"LEVEL {SaveManager.Instance.CurrentGameData.level + 1}";
            SetTweenCanvasGroups(_currentActivePanel,false);
            _currentActivePanel = gamePanel;
            SetTweenCanvasGroups(_currentActivePanel,true);
        }

        private void WinGame()
        {
            _currentActivePanel = winPanel;
            SetTweenCanvasGroups(_currentActivePanel,true);
        }

        private void MainMenu()
        {
            EventBroker.OnGameRestart();
            UpdateHighScore();
            SetTweenCanvasGroups(gamePanel,false);
            SetTweenCanvasGroups(winPanel,false);
            SetTweenCanvasGroups(settingsPanel,false);
            _currentActivePanel = mainPanel;
            SetTweenCanvasGroups(_currentActivePanel,true);
        }

        private void Settings()
        {
            sfxEnabled = SaveManager.Instance.CurrentGameData.sfxEnabled;
            
            if(sfxEnabled) soundSettingsImage.sprite = soundOnSprite;
            else soundSettingsImage.sprite = soundOffSprite;
            
            SetTweenCanvasGroups(_currentActivePanel,false);
            _currentActivePanel = settingsPanel;
            SetTweenCanvasGroups(_currentActivePanel,true);
        }

        private void UpdateHighScore()
        {
            if (SaveManager.Instance.CurrentGameData.highestPoint != 0)
            {
                highScoreText.text = "HIGH SCORE: " + SaveManager.Instance.CurrentGameData.highestPoint.ToString();
            }
            else
            {

                highScoreText.text = "";
            }
        }

        public void UpdateScore(float score, float addedScore, int combo)
        {
            DOTween.Kill(addedScoreText.gameObject);
            addedScoreText.alpha = 0.0f;
            if (combo > 1)
            {
                float t = Mathf.Clamp01((float)(combo - 1) / (float)(4));
                Color interpolatedColor = Color.Lerp(Color.yellow, Color.red, t);
                string hexColor = ColorUtility.ToHtmlStringRGB(interpolatedColor);
                addedScoreText.text = $"<color=white>+{addedScore}</color>  <color=#{hexColor}>x{combo}</color>";
            }
            else
            {
                addedScoreText.text = $"<color=white>+{addedScore}</color>";
            }
            
            addedScoreText.DOFade(1.0f, 0.3f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo);
            currentScoreText.text = $"SCORE: {score}";

        }

        private void SoundToggle()
        {
            sfxEnabled = !sfxEnabled;
            
            SaveManager.Instance.CurrentGameData.sfxEnabled = sfxEnabled;
            
            if(sfxEnabled) soundSettingsImage.sprite = soundOnSprite;
            else soundSettingsImage.sprite = soundOffSprite;
            
            SaveManager.Instance.SaveGame();
        }

        private void DeleteSave()
        {
            SaveManager.Instance.ResetGameData();
        }

        public void SetBackgroundColor(Color color)
        {
            backgroundImage.color = color;
        }

        private void SetTweenCanvasGroups(CanvasGroup canvasGroup, bool active)
        {
            if (active)
            {
                canvasGroup.DOFade(1.0f, .25f).OnComplete(() =>
                {
                    canvasGroup.alpha = 1.0f;
                    canvasGroup.blocksRaycasts = true;
                    canvasGroup.interactable = true;
                });
                
                return;
            }
            
            canvasGroup.DOFade(0.0f, .25f).OnComplete(() =>
            {
                canvasGroup.alpha = 0.0f;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            });
        }
        
        private void SetImmediateCanvasGroups(CanvasGroup canvasGroup, bool active)
        {
            if (active)
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                
                return;
            }
            
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}

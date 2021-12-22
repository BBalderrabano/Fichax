using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private PlayerData pd;
    private LevelSystem animLevelSystem;

    public GameObject sliderComponent;
    public GameObject levelComponent;
    public GameObject scoreComponent;
    public GameObject xpComponent;
    public GameObject levelPanelComponent;

    private Slider slider;
    private TextMeshProUGUI levelDisplay;
    private TextMeshProUGUI scoreDisplay;
    private TextMeshProUGUI xpDisplay;
    private CanvasGroup levelPanelCanvasGroup;

    private float startScore;

    private bool animating = false;
    
    private Vector3 levelOriginalScale;

    void Start()
    {
        pd = PlayerData.singleton;
        animLevelSystem = new LevelSystem(pd.start_max_xp, pd.max_xp_increment);

        slider = sliderComponent.GetComponent<Slider>();
        levelDisplay = levelComponent.GetComponentInChildren<TextMeshProUGUI>();
        scoreDisplay = scoreComponent.GetComponent<TextMeshProUGUI>();
        xpDisplay = xpComponent.GetComponent<TextMeshProUGUI>();
        levelPanelCanvasGroup = levelPanelComponent.GetComponent<CanvasGroup>();

        slider.value = 0;
        levelDisplay.text = "1";
        scoreDisplay.text = "Score: 0";
        xpDisplay.text = "0/10";

        levelOriginalScale = levelComponent.transform.localScale;
    }

    int levelAnimationTweenId = -1;
    int scoreAnimationTweenId = -1;
    int finalAnimationTweenId = -1;

    void IncreaseScoreAndXp(float amount) {
        animLevelSystem.AddScore(amount);

        xpDisplay.text = Mathf.FloorToInt(animLevelSystem.GetXp()) + "/" + Mathf.FloorToInt(animLevelSystem.GetXpToLevel());
        levelDisplay.text = animLevelSystem.GetLevel() + "";

        slider.value = (animLevelSystem.GetXp() / animLevelSystem.GetXpToLevel()) * 100f;
    }

    public void AddScore(int amount) {
        pd.playerLevelData.AddScore(amount);

        if (!animating) {
            startScore = Mathf.FloorToInt(animLevelSystem.GetScore());
            animating = true;
        }

        if (finalAnimationTweenId > 0)
            LeanTween.cancel(finalAnimationTweenId);

        scoreDisplay.text = "+" + (pd.playerLevelData.GetScore() - startScore);

        PulsateLevelStar();

        AnimateScore();
    }

    void PulsateLevelStar() {

        if (levelAnimationTweenId > 0)
            LeanTween.cancel(levelAnimationTweenId);

        levelAnimationTweenId = LeanTween.scale(levelComponent, levelOriginalScale * 1.3f, 0.8f)
                                .setFrom(levelOriginalScale).setEasePunch()
                                .setOnComplete(() => {
                                    levelAnimationTweenId = -1;
                                }).id;
    }

    void AnimateScore() {
        if (scoreAnimationTweenId > 0)
            LeanTween.cancel(scoreAnimationTweenId);
            
        float currentScore = animLevelSystem.GetScore();

        float lastTick = currentScore;

        scoreAnimationTweenId = LeanTween.value(currentScore, pd.playerLevelData.GetScore(), 1f)
                                    .setOnUpdate((float n) => {
                                        IncreaseScoreAndXp(Mathf.Abs(n - lastTick));
                                        lastTick = n;
                                    })
                                    .setOnComplete(()=> {
                                        scoreAnimationTweenId = -1;
                                        FinishAnimation();
                                    }).setEaseOutCubic().id;
    }

    void FinishAnimation() {
        if (finalAnimationTweenId > 0)
            LeanTween.cancel(finalAnimationTweenId);

        finalAnimationTweenId = LeanTween.value(startScore, pd.playerLevelData.GetScore(), 1f)
                    .setOnUpdate((float n) => {
                        animating = false;

                        scoreDisplay.text = "Score: " + Mathf.FloorToInt(n);
                    })
                    .setOnComplete(() => {
                        finalAnimationTweenId = -1;

                        animLevelSystem.SetScore(pd.playerLevelData.GetScore());
                    }).setDelay(2).id;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameObject sliderComponent;
    public GameObject levelComponent;
    public GameObject scoreComponent;
    public GameObject xpComponent;

    private Slider slider;
    private TextMeshProUGUI levelDisplay;
    private TextMeshProUGUI scoreDisplay;
    private TextMeshProUGUI xpDisplay;

    private int currentScore;

    private bool animating = false;

    // Start is called before the first frame update
    void Start()
    {
        slider = sliderComponent.GetComponent<Slider>();
        levelDisplay = levelComponent.GetComponentInChildren<TextMeshProUGUI>();
        scoreDisplay = scoreComponent.GetComponent<TextMeshProUGUI>();
        xpDisplay = xpComponent.GetComponent<TextMeshProUGUI>();

        slider.value = 0;
        levelDisplay.text = "1";
        scoreDisplay.text = "Score: 0";
        xpDisplay.text = "0/10";
    }

    void Update()
    {
        if (animating)
        {

        }
    }

    public void AddScore(int amount) {
        PlayerData.singleton.AddScore(amount);

        animating = true;
    }
}

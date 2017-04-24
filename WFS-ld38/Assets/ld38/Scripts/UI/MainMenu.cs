using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    const string SliderString = "Players: {0}";

    void Awake()
    {
        ui.SetActive(false);
    }

    public Slider playerSlider;
    public Text sliderText;
    public Generate planet;
    public GameController controller;
    public GameObject ui;

    public void UpdateSlider(float players)
    {
        sliderText.text = string.Format(SliderString, (int)players);
    }

    public void StartClicked()
    {
        int players = Mathf.Clamp((int)playerSlider.value, 2, 8);
        controller.numPlayers = players;
        ui.SetActive(true);
        gameObject.SetActive(false);
        controller.GeneratePlanet();
    }
}

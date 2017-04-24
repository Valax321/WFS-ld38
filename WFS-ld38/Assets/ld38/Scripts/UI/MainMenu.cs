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
    public GameObject menu;
    public GameObject loadText;

    public void UpdateSlider(float players)
    {
        sliderText.text = string.Format(SliderString, (int)players);
    }

    IEnumerator UI()
    {
        menu.SetActive(false);
        loadText.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        controller.GeneratePlanet();
        yield return new WaitForSeconds(0.5f);
        loadText.SetActive(false);
        ui.SetActive(true);
    }

    public void StartClicked()
    {
        int players = Mathf.Clamp((int)playerSlider.value, 2, 8);
        controller.numPlayers = players;
        StartCoroutine(UI());
    }
}

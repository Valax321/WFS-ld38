using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets;

[AddComponentMenu("UI/Controller")]
public class UIController : MonoBehaviour
{
    const string CurrencyString = "{0}: {1} ({2} per turn)";
    const string HealthString = "HP: {0}";
    const string DescriptionString = "Help: {0}";
    const string BiomeString = "Biome: {0}";
    const string TileCurrencyString = "{0}: {1}";
    const string UnitAbilityPanelString = "{0}\n({1} moves left)";

    public GameObject topBar;
    public GameObject historyPanel;
    public GameObject createPanel;
    public GameObject descriptionPanel;
    public GameObject abilityPanel;
    public GameObject abilityLayoutGroup;
    public GameObject tileInfo;
    public GameObject mainScreen;
    public GameObject pauseScreen;

    public History history;

    public static UIController instance { get { return uiInstance; } }
    static UIController uiInstance;

    void Awake()
    {
        if (uiInstance == null)
        {
            uiInstance = this;
        }
    }

    public void UpdateCurrency(string currencyName, long currency, long perTurn)
    {
        if (topBar != null)
        {
            topBar.transform.FindChild("Currency").GetComponent<Text>().text = string.Format(CurrencyString, currencyName, currency, perTurn);
        }
    }

    public void UpdateHP(int hp)
    {
        if (topBar != null)
        {
            topBar.transform.FindChild("Health").GetComponent<Text>().text = string.Format(HealthString, hp);
        }
    }

    public void UpdateDescription(string name, string description)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.transform.FindChild("Title").GetComponent<Text>().text = string.Format(DescriptionString, name);
            descriptionPanel.transform.FindChild("Text").GetComponent<Text>().text = description;
        }
    }

    public void UpdateDescriptionEnabled(bool enabled)
    {
        if (descriptionPanel != null)
        {
            descriptionPanel.SetActive(enabled);
        }
    }

    public void UpdateInfoBiome(VoronoiTile.Biomes biome)
    {
        if (tileInfo != null)
        {
            var biomeString = Enum.GetName(typeof(VoronoiTile.Biomes), biome);
            tileInfo.transform.FindChild("Biome").GetComponent<Text>().text = string.Format(BiomeString, biomeString);
        }
    }

    public void UpdateInfoCurrencyValue(string currency, int value)
    {
        if (tileInfo != null)
        {
            tileInfo.transform.FindChild("Currency").GetComponent<Text>().text = string.Format(TileCurrencyString, currency, value);
        }
    }

    public void UpdateInfoPanel(VoronoiTile.Biomes biome, string currency, int value)
    {
        UpdateInfoBiome(biome);
        UpdateInfoCurrencyValue(currency, value);
    }

    public void UpdateInfoPanelEnabled(bool enabled)
    {
        if (tileInfo != null)
        {
            tileInfo.SetActive(enabled);
        }
    }

    public void UpdateAbilityPanelEnabled(bool enabled)
    {
        if (abilityPanel != null)
        {
            abilityPanel.SetActive(enabled);
        }
    }

    public void UpdateAbilityPanelName(string name, int moves)
    {
        if (abilityPanel != null)
        {
            abilityPanel.transform.FindChild("Unit Name Label").GetComponent<Text>().text = string.Format(UnitAbilityPanelString, name, moves - 1);
        }
    }

    public void UpdateAbility1Name(string name)
    {
        if (abilityPanel != null)
        {
            abilityLayoutGroup.transform.FindChild("Ability 1").GetComponentInChildren<Text>().text = name;
        }
    }

    public void UpdateAbility1Enabled(bool enabled)
    {
        if (abilityPanel != null)
        {
            abilityLayoutGroup.transform.FindChild("Ability 1").gameObject.SetActive(enabled);
        }
    }

    public void UpdateAbility2Name(string name)
    {
        if (abilityPanel != null)
        {
            abilityLayoutGroup.transform.FindChild("Ability 2").GetComponentInChildren<Text>().text = name;
        }
    }

    public void UpdateAbility2Enabled(bool enabled)
    {
        if (abilityPanel != null)
        {
            abilityLayoutGroup.transform.FindChild("Ability 2").gameObject.SetActive(enabled);
        }
    }

    public void UpdateMoveButtonEnabled(bool enabled)
    {
        if (abilityPanel != null)
        {
            abilityLayoutGroup.transform.FindChild("Move Button").gameObject.SetActive(enabled);
        }
    }

    public void UpdatePause(bool isPaused)
    {
        pauseScreen.SetActive(isPaused);
        mainScreen.SetActive(!isPaused);
    }

    public void PushNotification(string msg)
    {
        history.AddMessage(msg);
    }
}

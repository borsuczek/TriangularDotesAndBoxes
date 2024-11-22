using ObjectDefinition;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    /// <summary>
    /// Handles the selection of the game mode and adjusts the settings panel accordingly.
    /// </summary>
    /// <param name="mode">The selected game mode (0: PvP, 1: PvE, 2: EvE).</param>
    public void ModeSelect(int mode)
    {
        GameManager.currentMode = mode;
        Transform settingsPanelTransform = GameManager.Instance.SettingsPanel.transform;
        GameObject firstPlayerObject = settingsPanelTransform.Find("FirstNameInput").gameObject;
        GameObject secondPlayerObject = settingsPanelTransform.Find("SecondNameInput").gameObject;
        GameObject firstBotObject = settingsPanelTransform.Find("FirstBotDifficulty").gameObject;
        GameObject secondBotObject = settingsPanelTransform.Find("SecondBotDifficulty").gameObject;

        firstPlayerObject.GetComponentInChildren<TMP_InputField>().text = "";
        secondPlayerObject.GetComponentInChildren<TMP_InputField>().text = "";
        firstBotObject.GetComponentsInChildren<Toggle>()[1].isOn = false;
        secondBotObject.GetComponentsInChildren<Toggle>()[1].isOn = false;

        if (mode == 0)
        {
            firstPlayerObject.SetActive(true);
            secondPlayerObject.SetActive(true);
            firstBotObject.SetActive(false);
            secondBotObject.SetActive(false);
        }
        else if (mode == 1)
        {
            firstPlayerObject.SetActive(true);
            secondPlayerObject.SetActive(false);
            firstBotObject.SetActive(false);
            secondBotObject.SetActive(true);
        }
        else
        {
            firstPlayerObject.SetActive(false);
            secondPlayerObject.SetActive(false);
            firstBotObject.SetActive(true);
            secondBotObject.SetActive(true);
        }

        GameManager.Instance.PanelControl.OpenSettingsPanel();
        GameManager.Instance.PanelControl.CloseMenuPanel();
    }

    /// <summary>
    /// Sets player settings based on the input from the settings panel and opens the size menu.
    /// </summary>
    public void SetPlayerSettings()
    {
        Transform settingsPanelTransform = GameManager.Instance.SettingsPanel.transform;
        GameObject firstPlayerObject = settingsPanelTransform.Find("FirstNameInput").gameObject;
        GameObject secondPlayerObject = settingsPanelTransform.Find("SecondNameInput").gameObject;
        GameObject firstBotObject = settingsPanelTransform.Find("FirstBotDifficulty").gameObject;
        GameObject secondBotObject = settingsPanelTransform.Find("SecondBotDifficulty").gameObject;

        if (firstPlayerObject.activeSelf)
        {
            GameManager.firstPlayerName = firstPlayerObject.GetComponentInChildren<TMP_InputField>().text;
        }
        if (secondPlayerObject.activeSelf)
        {
            GameManager.secondPlayerName = secondPlayerObject.GetComponentInChildren<TMP_InputField>().text;
        }

        if(GameManager.currentMode  == 0)
        {
            if (GameManager.firstPlayerName == "")
                GameManager.firstPlayerName = "Player1";
            if (GameManager.secondPlayerName == "")
                GameManager.secondPlayerName = "Player2";
        }
        else if(GameManager.currentMode == 1)
        {
            if (GameManager.firstPlayerName == "")
                GameManager.firstPlayerName = "Player";

            GameManager.secondPlayerName = "Computer";
        }
        else if(GameManager.currentMode == 2)
        {
            GameManager.firstPlayerName = "Computer1";
            GameManager.secondPlayerName = "Computer2";
        }

        if (firstBotObject.GetComponentInChildren<Toggle>().isOn)
            GameManager.bot1Level = (int)Level.Easy;
        else
            GameManager.bot1Level = (int)Level.Hard;

        if (secondBotObject.GetComponentInChildren<Toggle>().isOn)
            GameManager.bot2Level = (int)Level.Easy;
        else
            GameManager.bot2Level = (int)Level.Hard;

        GameManager.Instance.PanelControl.OpenSizeMenu();
        GameManager.Instance.PanelControl.CloseSettingsPanel();
    }

    /// <summary>
    /// Initiates the game by setting the number of rows and columns, opening the game panel, and creating dots and players.
    /// </summary>
    public void GameStart()
    {
        GameManager.numberOfRows = int.Parse(GameManager.Instance.SliderRowsValue.text);
        GameManager.numberOfColumns = int.Parse(GameManager.Instance.SliderColumnsValue.text);
        GameManager.Instance.PanelControl.OpenGamePanel();
        GameManager.Instance.PanelControl.CloseSizeMenu();
        GameManager.Instance.BoardControl.CreateDots();
        GameManager.Instance.PlayerControl.CreatePlayers();
    }

    /// <summary>
    /// Navigates back to the mode selection panel from the size selection panel.
    /// </summary>
    public void BackToModeSelect()
    {
        GameManager.Instance.PanelControl.CloseSizeMenu();
        GameManager.Instance.PanelControl.OpenMenuPanel();
    }

    /// <summary>
    /// Updates the displayed value of the columns slider.
    /// </summary>
    /// <param name="value">The current value of the columns slider.</param>
    public void ChangeSliderColumnsValue(float value)
    {
        GameManager.Instance.SliderColumnsValue.text = value.ToString();
    }

    /// <summary>
    /// Updates the displayed value of the rows slider.
    /// </summary>
    /// <param name="value">The current value of the rows slider.</param>
    public void ChangeSliderRowsValue(float value)
    {
        GameManager.Instance.SliderRowsValue.text = value.ToString();
    }
}

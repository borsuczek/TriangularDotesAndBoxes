using ObjectDefinition;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static UnityEngine.GraphicsBuffer;
using System.Collections;
using System;
using System.Numerics;
using System.Data;
using System.Collections.Generic;

public class PlayerControl : MonoBehaviour
{
    /// <summary>
    /// Creates players based on the current game mode.
    /// </summary>
    public void CreatePlayers()
    {
        if (GameManager.currentMode == (int)Mode.PvP)
        {
            GameManager.firstPlayer = new Player();
            GameManager.secondPlayer = new Player();
        }
        else if (GameManager.currentMode == (int)Mode.PvE)
        {
            GameManager.firstPlayer = new Player();
            GameManager.secondPlayer = new Player(GameManager.bot2Level);
        }
        else if (GameManager.currentMode == (int)Mode.EvE)
        {
            GameManager.firstPlayer = new Player(GameManager.bot1Level);
            GameManager.secondPlayer = new Player(GameManager.bot2Level);
        }
        GameManager.firstPlayer.Material = GameManager.Instance.Player1Material;
        GameManager.firstPlayer.PlayerLayout = GameManager.Instance.Player1Layout;
        GameManager.secondPlayer.Material = GameManager.Instance.Player2Material;
        GameManager.secondPlayer.PlayerLayout = GameManager.Instance.Player2Layout;
        GameManager.firstPlayer.PlayerLayout.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = GameManager.firstPlayerName;
        GameManager.secondPlayer.PlayerLayout.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>().text = GameManager.secondPlayerName;

        GameManager.currentPlayer = GameManager.firstPlayer;
        if(GameManager.currentMode == (int)Mode.EvE)
        {
            GameManager.Instance.BoardControl.ChooseDot();
        }
    }

    /// <summary>
    /// Changes the current player, updates visual cues, and handles the end of the game.
    /// </summary>
    public void ChangePlayer()
    {
        GameManager.currentPlayer.UnboldText();
        if (GameManager.currentPlayer == GameManager.firstPlayer)
        {
            GameManager.currentPlayer = GameManager.secondPlayer;
        }
        else
        {
            GameManager.currentPlayer = GameManager.firstPlayer;
        }
        GameManager.currentPlayer.BoldText();
    }
}

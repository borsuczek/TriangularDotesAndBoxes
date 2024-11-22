using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelControl : MonoBehaviour
{
    /// <summary>
    /// Opens the main menu panel.
    /// </summary>
    public void OpenMenuPanel()
    {
        GameManager.Instance.MenuPanel.SetActive(true);
    }

    /// <summary>
    /// Closes the main menu panel.
    /// </summary>
    public void CloseMenuPanel()
    {
        GameManager.Instance.MenuPanel.SetActive(false);
    }

    /// <summary>
    /// Opens the settings panel.
    /// </summary>
    public void OpenSettingsPanel()
    {
        GameManager.Instance.SettingsPanel.SetActive(true);
    }

    /// <summary>
    /// Closes the settings panel.
    /// </summary>
    public void CloseSettingsPanel()
    {
        GameManager.Instance.SettingsPanel.SetActive(false);
    }

    /// <summary>
    /// Opens the size selection menu.
    /// </summary>
    public void OpenSizeMenu()
    {
        GameManager.Instance.SizeSelectionPanel.SetActive(true);
    }


    /// <summary>
    /// Closes the size selection menu.
    /// </summary>
    public void CloseSizeMenu()
    {
        GameManager.Instance.SizeSelectionPanel.SetActive(false);
    }

    /// <summary>
    /// Opens the main game panel.
    /// </summary>
    public void OpenGamePanel()
    {
        GameManager.Instance.GamePanel.SetActive(true);
    }

    /// <summary>
    /// Closes the main game panel.
    /// </summary>
    public void CloseGamePanel()
    {
        GameManager.Instance.GamePanel.SetActive(false);
    }

    /// <summary>
    /// Opens the block panel.
    /// </summary>
    public void OpenBlockPanel()
    {
        GameManager.Instance.BlockPanel.SetActive(true);
    }

    /// <summary>
    /// Closes the block panel.
    /// </summary>
    public void CloseBlockPanel()
    {
        GameManager.Instance.BlockPanel.SetActive(false);
    }

    /// <summary>
    /// Opens the finish panel.
    /// </summary>
    public void OpenFinishPanel()
    {
        GameManager.Instance.FinishPanel.SetActive(true);
    }

    /// <summary>
    /// Closes the finish panel.
    /// </summary>
    public void CloseFinishPanel()
    {
        GameManager.Instance.FinishPanel.SetActive(false);
    }
}

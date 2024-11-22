using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ObjectDefinition;
using UnityEditor;
using System.Data;

public class GameManager : MonoBehaviour
{
    public static ObjectDefinition.Dot[] dotArray;
    public static bool[,] connectionArray;
    public static int[,] costArray;
    public static int currentMode = (int)Mode.EvE;
    public static int bot1Level = (int)Level.Hard;
    public static int bot2Level = (int)Level.Hard;
    public const int impossible = -1000000;
    public const int possible = 0;
    public const double smallestTriangleArea = 0.5;
    public static int numberOfDots;
    public static int numberOfRows;
    public static int numberOfColumns;
    public static bool finished = false;
    public static Player currentPlayer = new Player();
    public static Player firstPlayer;
    public static Player secondPlayer;
    public static string firstPlayerName;
    public static string secondPlayerName;

    public TextMeshProUGUI SliderColumnsValue;
    public TextMeshProUGUI SliderRowsValue;

    public GameObject dotPrefab;
    public GameObject meshPrefab;
    public GameObject lineRendererPrefab;

    public GameObject GamePanel;
    public GameObject SizeSelectionPanel;
    public GameObject SettingsPanel;
    public GameObject MenuPanel;
    public GameObject BlockPanel;
    public GameObject FinishPanel;

    public GameObject GridLayout;
    public GameObject LineLayout;
    public GameObject TriangleLayout;
    public GameObject Player1Layout;
    public GameObject Player2Layout;

    public Material Player1Material;
    public Material Player2Material;

    public BoardControl BoardControl;
    public PanelControl PanelControl;
    public PlayerControl PlayerControl;
    public DrawingControl DrawingControl;
    public FinishControl FinishControl;

    /// <summary>
    /// Singleton instance of the GameManager.
    /// </summary>
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

}

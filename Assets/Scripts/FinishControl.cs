using ObjectDefinition;
using TMPro;
using UnityEngine;

public class FinishControl : MonoBehaviour
{
    private const string winText = " wins!!";
    private const string drawText = "There is a draw!";

    /// <summary>
    /// Handles the end of the game by displaying the winner or a draw message and opens the finish panel.
    /// </summary>
    public void GameEnd()
    {
        GameManager.Instance.PanelControl.OpenBlockPanel();
        TextMeshProUGUI playerTextMesh;
        TextMeshProUGUI wonTextMesh = GameManager.Instance.FinishPanel.transform.Find("WonText").GetComponent<TextMeshProUGUI>();
        if (GameManager.firstPlayer.points == GameManager.secondPlayer.points)
        {
            wonTextMesh.text = drawText;
            wonTextMesh.color = Color.gray;
        }
        else
        {
            if (GameManager.firstPlayer.points > GameManager.secondPlayer.points)
                playerTextMesh = GameManager.firstPlayer.PlayerLayout.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>();
            else
                playerTextMesh = GameManager.secondPlayer.PlayerLayout.transform.Find("PlayerNameText").GetComponent<TextMeshProUGUI>();
            wonTextMesh.text = playerTextMesh.text + winText;
            wonTextMesh.color = playerTextMesh.color;
        }
        GameManager.Instance.PanelControl.OpenFinishPanel();
    }

    /// <summary>
    /// Initiates a rematch by resetting players, tables, and necessary game elements.
    /// </summary>
    public void Rematch()
    {
        GameManager.finished = false;
        GameManager.Instance.PanelControl.CloseFinishPanel();
        ResetPlayers();
        ResetTables();
        DestroyLines();
        DestroyTriangles();
        if (GameManager.currentPlayer.Bot != null)
            GameManager.Instance.BoardControl.ChooseDot();
        else
            GameManager.Instance.PanelControl.CloseBlockPanel();
    }

    /// <summary>
    /// Starts a new game by resetting players, lines, triangles, and dots, and opens the menu panel.
    /// </summary>
    public void NewGame()
    {
        GameManager.finished = false;
        GameManager.Instance.PanelControl.CloseFinishPanel();
        ResetPlayers();
        DestroyLines();
        DestroyTriangles();
        DestroyDots();
        GameManager.Instance.PanelControl.OpenMenuPanel();
        GameManager.Instance.PanelControl.CloseBlockPanel();
        GameManager.Instance.PanelControl.CloseGamePanel();
    }

    /// <summary>
    /// Resets player scores and other related attributes.
    /// </summary>
    private void ResetPlayers()
    {
        GameManager.firstPlayer.points = 0;
        GameManager.secondPlayer.points = 0;
        GameManager.firstPlayer.ChangeScore();
        GameManager.secondPlayer.ChangeScore();
        GameManager.currentPlayer = GameManager.firstPlayer;
        GameManager.firstPlayer.BoldText();
        GameManager.secondPlayer.UnboldText();
    }

    /// <summary>
    /// Resets tables by setting arrays, marking impossible connections, and enabling all dots.
    /// </summary>
    private void ResetTables()
    {
        GameManager.Instance.BoardControl.SetArrays();
        GameManager.Instance.BoardControl.SetImpossibleInRowsColsDiags();
        foreach(Dot dot in GameManager.dotArray)
        {
            dot.SetEnabled();
        }
    }

    /// <summary>
    /// Destroys all line game objects in the line layout.
    /// </summary>
    private void DestroyLines()
    {
        Transform lineLayoutTransform = GameManager.Instance.LineLayout.transform;
        int numberOfLines = lineLayoutTransform.childCount;
        for (int numberOfLine = 0; numberOfLine < numberOfLines; numberOfLine++)
        {
            Destroy(lineLayoutTransform.GetChild(numberOfLine).gameObject);
            numberOfLines = lineLayoutTransform.childCount;
        }
    }

    /// <summary>
    /// Destroys all triangle game objects in the triangle layout.
    /// </summary>
    private void DestroyTriangles()
    {
        Transform triangleLayoutTransform = GameManager.Instance.TriangleLayout.transform;
        int numberOfTriangles = triangleLayoutTransform.childCount;
        for (int numberOfTriangle = 0; numberOfTriangle < numberOfTriangles; numberOfTriangle++)
        {
            Destroy(triangleLayoutTransform.GetChild(numberOfTriangle).gameObject);
            numberOfTriangles = triangleLayoutTransform.childCount;
        }
    }

    /// <summary>
    /// Destroys all dot game objects in the grid layout.
    /// </summary>
    private void DestroyDots()
    {
        Transform gridLayoutTransform = GameManager.Instance.GridLayout.transform;
        int numberOfDots = gridLayoutTransform.childCount;
        for (int numberOfDot = 0; numberOfDot < numberOfDots; numberOfDot++)
        {
            Destroy(gridLayoutTransform.GetChild(numberOfDot).gameObject);
            numberOfDots = gridLayoutTransform.childCount;
        }
    }
}

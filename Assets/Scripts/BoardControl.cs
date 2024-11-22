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
using UnityEngine.U2D;
using Unity.PlasticSCM.Editor.WebApi;
using System.Data.Common;
using System.Threading;
using System.Security.Cryptography;

public class BoardControl : MonoBehaviour
{
    private Button firstClickedDot;
    private Button secondClickedDot;
    private bool drawingLine;

    private int rows;
    private int columns;
    private int numberOfDots;
    private int numberOfTriangles = 0;
    private int maxNumberOfTriangles;

    /// <summary>
    /// Creates dots on the game board based on the specified number of rows and columns.
    /// </summary>
    public void CreateDots()
    {
        rows = GameManager.numberOfRows; 
        columns = GameManager.numberOfColumns;

        numberOfDots = rows * columns;
        maxNumberOfTriangles = (rows - 1) * (columns - 1) * 2;

        GameManager.numberOfDots = numberOfDots;
        GameManager.dotArray = new Dot[numberOfDots];
        GameManager.connectionArray = new bool[numberOfDots, numberOfDots];
        GameManager.costArray = new int[numberOfDots, numberOfDots];

        int id = 0;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject newButton = Instantiate(GameManager.Instance.dotPrefab, GameManager.Instance.GridLayout.transform);
                newButton.GetComponent<Button>().onClick.AddListener(() => OnDotClicked(newButton.GetComponent<Button>()));
                newButton.name = id.ToString();
                Dot dot = new Dot(id, new Position(x, y), newButton);
                GameManager.dotArray[id] = dot;
                id++;
            }
        }

        SetArrays();
        SetImpossibleInRowsColsDiags();

        GameManager.Instance.GridLayout.GetComponent<GridLayoutGroup>().constraintCount = rows;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GameManager.Instance.GridLayout.GetComponent<GridLayoutGroup>().GetComponent<RectTransform>());
    }

    /// <summary>
    /// Initializes connection and cost arrays for dots on the game board.
    /// </summary>
    public void SetArrays()
    {
        for (int i = 0; i < numberOfDots; i++)
        {
            for (int j = 0; j < numberOfDots; j++)
            {
                if (i == j)
                {
                    GameManager.connectionArray[i, i] = true;
                    GameManager.costArray[i, i] = GameManager.impossible;
                }
                else
                {
                    GameManager.connectionArray[i, j] = false;
                    GameManager.costArray[i, i] = GameManager.possible;
                }
            }
        }
    }

    /// <summary>
    /// Initiates the dot selection process for the computer player.
    /// </summary>
    public void ChooseDot()
    {
        GameManager.Instance.PanelControl.OpenBlockPanel();
        (int firstDotId, int secondDotId) = GameManager.currentPlayer.Bot.Tactic();
        Debug.Log(firstDotId);
        Debug.Log(secondDotId);
        Dot firstDot = GameManager.dotArray[firstDotId];
        Dot secondDot = GameManager.dotArray[secondDotId];

        SetLine(firstDot, secondDot);

        if (!GameManager.finished)
        {
            if (GameManager.currentPlayer.Bot == null)
            {
                GameManager.Instance.PanelControl.CloseBlockPanel();
            }
            else
            {
                StartCoroutine(WaitForNextComputer());
            }
        }
    }

    /// <summary>
    /// Waits for a short duration before selecting the next dot for the computer player.
    /// </summary>
    /// <returns>An enumerator for coroutine.</returns>
    IEnumerator WaitForNextComputer()
    {
        yield return new WaitForSeconds(0.1f); //wait for 0.5 seconds
        ChooseDot();
    }

    /// <summary>
    /// Sets cost values in the cost array to indicate the impossibility of creating lines between certain dots.
    /// </summary>
    public void SetImpossibleInRowsColsDiags()
    {
        for (int firstDotId = 0; firstDotId < numberOfDots; firstDotId++)
        {
            for (int secondDotId = firstDotId; secondDotId < numberOfDots; secondDotId++)
            {
                Dot firstDot = GameManager.dotArray[firstDotId];
                Dot secondDot = GameManager.dotArray[secondDotId];

                GameManager.costArray[firstDotId, secondDotId] = GameManager.impossible;
                GameManager.costArray[secondDotId, firstDotId] = GameManager.impossible;

                if ((firstDot.Position.X == secondDot.Position.X && (firstDot.Position.Y == secondDot.Position.Y - 1 || firstDot.Position.Y == secondDot.Position.Y + 1))
                    || (firstDot.Position.Y == secondDot.Position.Y && (firstDot.Position.X == secondDot.Position.X - 1 || firstDot.Position.X == secondDot.Position.X + 1)))
                {
                    GameManager.costArray[firstDotId, secondDotId] = GameManager.possible;
                    GameManager.costArray[secondDotId, firstDotId] = GameManager.possible;
                }
                else if ((int)BigInteger.GreatestCommonDivisor(Math.Abs(secondDot.Position.X - firstDot.Position.X), Math.Abs(secondDot.Position.Y - firstDot.Position.Y)) == 1)
                {
                    GameManager.costArray[firstDotId, secondDotId] = GameManager.possible;
                    GameManager.costArray[secondDotId, firstDotId] = GameManager.possible;
                }
            }
        }
    }

    /// <summary>
    /// Handles the event when a dot is clicked.
    /// </summary>
    /// <param name="clickedDot">The clicked dot's button.</param>
    public void OnDotClicked(Button clickedDot)
    {
        if (!drawingLine)
        {
            firstClickedDot = clickedDot;
            drawingLine = true;
            print(clickedDot.gameObject.name);
            ShowAvaiableDots(int.Parse(clickedDot.gameObject.name));
        }
        else if (clickedDot != firstClickedDot)
        {

            secondClickedDot = clickedDot;
            SetLine(GameManager.dotArray[int.Parse(firstClickedDot.gameObject.name)], GameManager.dotArray[int.Parse(secondClickedDot.gameObject.name)]);

            foreach (Dot dot in GameManager.dotArray)
            {
                dot.SetActive();
            }

            if (GameManager.currentPlayer.Bot != null)
            {
                print(GameManager.currentPlayer.Bot);
                ChooseDot();
            }
        }
        else
        {
            drawingLine = false;

            foreach (Dot dot in GameManager.dotArray)
            {
                dot.SetActive();
            }
        }
    }

    /// <summary>
    /// Shows available dots that can be connected to the clicked dot.
    /// </summary>
    /// <param name="clickedId">The ID of the clicked dot.</param>
    public void ShowAvaiableDots(int clickedId)
    {
        Dot clickedDot = GameManager.dotArray[clickedId];
        clickedDot.SetFirstClicked();

        for (int dotId = 0; dotId < numberOfDots; dotId++)
        {
            if (clickedId != dotId)
            {
                Dot dot = GameManager.dotArray[dotId];
                bool canConnect = false;

                if (GameManager.costArray[clickedId, dotId] > GameManager.impossible && !GameManager.connectionArray[dotId, clickedId])
                {
                    if (!isIntersecting(clickedDot, dot))
                    {
                        canConnect = true;
                    }
                }
                if (!canConnect)
                {
                    dot.SetUnactive();
                }
            }
        }
    }

    /// <summary>
    /// Checks if a line between two dots intersects with any existing lines.
    /// </summary>
    private bool isIntersecting(Dot dotA, Dot dotB)
    {
        for (int dotCId = 0; dotCId < numberOfDots; dotCId++)
        {
            Dot dotC = GameManager.dotArray[dotCId];

            if (dotC.ID != dotA.ID && dotC.ID != dotB.ID)
            {
                for (int dotDId = 0; dotDId < numberOfDots; dotDId++)
                {
                    Dot dotD = GameManager.dotArray[dotDId];

                    if (dotD.ID != dotA.ID && dotD.ID != dotB.ID && dotD.ID != dotC.ID
                        && GameManager.connectionArray[dotC.ID, dotD.ID] == true)
                    {
                        int xa = dotA.Position.X, ya = dotA.Position.Y;
                        int xb = dotB.Position.X, yb = dotB.Position.Y;
                        int xc = dotC.Position.X, yc = dotC.Position.Y;
                        int xd = dotD.Position.X, yd = dotD.Position.Y;

                        if (xb - xa != 0)
                        {
                            double s = ((xa * (yb - ya) - ya * (xb - xa) + yc * (xb - xa) - xc * (yb - ya)) * 1.0 / ((xd - xc) * (yb - ya) - (yd - yc) * (xb - xa)) * 1.0);
                            double t = ((xc + s * (xd - xc) - xa)) * 1.0 / (xb - xa) * 1.0;
                            if (s > 0.0 && t > 0.0 && s < 1.0 && t < 1.0)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (xd - xc != 0)
                            {
                                double s = (xa - xc) * 1.0 / (xd - xc) * 1.0;
                                double t = (yc + s * (yd - yc) - ya) * 1.0 / (yb - ya) * 1.0;
                                if (s > 0.0 && t > 0.0 && s < 1.0 && t < 1.0)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Sets a line between two dots, updates the connection array, and checks for completed triangles.
    /// </summary>
    /// <param name="firstDot">The first dot.</param>
    /// <param name="secondDot">The second dot.</param>
    public void SetLine(Dot firstDot, Dot secondDot)
    {
        GameManager.Instance.DrawingControl.DrawLineBetweenDots(firstDot.Button.GetComponent<Button>(), secondDot.Button.GetComponent<Button>());
        drawingLine = false;

        GameManager.connectionArray[firstDot.ID, secondDot.ID] = true;
        GameManager.connectionArray[secondDot.ID, firstDot.ID] = true;

        UpdateCostArray(GameManager.costArray, GameManager.connectionArray);
        DisableDots();
        CheckIfSmallestTriangle(firstDot.ID, secondDot.ID);
    }

    /// <summary>
    /// Updates the cost array based on the current connections and intersections.
    /// </summary>
    /// <param name="costArray">The cost array to be updated.</param>
    /// <param name="connectionArray">The connection array indicating connected dots.</param>
    public void UpdateCostArray(int[,] costArray, bool[,] connectionArray)
    {
        for (int dotAId = 0; dotAId < numberOfDots; dotAId++)
        {
            for (int dotBId = dotAId; dotBId < numberOfDots; dotBId++)
            {
                Dot dotA = GameManager.dotArray[dotAId];
                Dot dotB = GameManager.dotArray[dotBId];

                if (costArray[dotA.ID, dotB.ID] > GameManager.impossible)
                {
                    if (isIntersecting(dotA, dotB) || connectionArray[dotA.ID, dotB.ID])
                    {
                        costArray[dotA.ID, dotB.ID] = GameManager.impossible;
                        costArray[dotB.ID, dotA.ID] = GameManager.impossible;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Disables dots that can no longer be selected due to connections with other dots.
    /// </summary>
    public void DisableDots()
    {
        for (int dotAId = 0; dotAId < numberOfDots; dotAId++)
        {
            bool disable = true;
            for (int dotBId = 0; dotBId < numberOfDots; dotBId++)
            {
                if (dotAId != dotBId && GameManager.costArray[dotAId, dotBId] > GameManager.impossible)
                {
                    disable = false;
                    break;
                }
            }
            if (disable)
            {
                GameManager.dotArray[dotAId].SetDisabled();
            }
        }
    }

    /// <summary>
    /// Checks if the selected line completes the smallest possible triangle and updates scores accordingly.
    /// </summary>
    /// <param name="firstDotId">The ID of the first dot.</param>
    /// <param name="secondDotId">The ID of the second dot.</param>
    public void CheckIfSmallestTriangle(int firstDotId, int secondDotId)
    {
        bool pointsAdded = false;

        for (int thirdDotId = 0; thirdDotId < numberOfDots; thirdDotId++)
        {
            if (GameManager.connectionArray[firstDotId, secondDotId] && GameManager.connectionArray[secondDotId, thirdDotId] && GameManager.connectionArray[thirdDotId, firstDotId]  
                && thirdDotId != firstDotId && thirdDotId != secondDotId)
            {
                double area = TriangleAreaFromIds(firstDotId, secondDotId, thirdDotId);

                if (area == GameManager.smallestTriangleArea)
                {
                    pointsAdded = true;
                    GameManager.Instance.DrawingControl.DrawTriangle(GameManager.dotArray[firstDotId], GameManager.dotArray[secondDotId], GameManager.dotArray[thirdDotId]);
                    GameManager.currentPlayer.points++;
                    numberOfTriangles++;
                }
            }
        }

        if (!pointsAdded)
        {
            GameManager.Instance.PlayerControl.ChangePlayer();
        }
        else
        {
            GameManager.currentPlayer.ChangeScore();
            if(numberOfTriangles == maxNumberOfTriangles)
            {
                GameManager.finished = true;
                GameManager.Instance.FinishControl.GameEnd();
                numberOfTriangles = 0;
            }
        }
    }

    /// <summary>
    /// Calculates the area of a triangle formed by three dots based on their IDs.
    /// </summary>
    /// <param name="dotIdA">ID of the first dot.</param>
    /// <param name="dotIdB">ID of the second dot.</param>
    /// <param name="dotIdC">ID of the third dot.</param>
    /// <returns>The area of the triangle.</returns>
    public double TriangleAreaFromIds(int dotIdA, int dotIdB, int dotIdC)
    {
        int xa = GameManager.dotArray[dotIdA].Position.X, ya = GameManager.dotArray[dotIdA].Position.Y;
        int xb = GameManager.dotArray[dotIdB].Position.X, yb = GameManager.dotArray[dotIdB].Position.Y;
        int xc = GameManager.dotArray[dotIdC].Position.X, yc = GameManager.dotArray[dotIdC].Position.Y;
        return (Math.Abs((xb - xa) * (yc - ya) - (yb - ya) * (xc - xa))) / 2.0;
    }
}
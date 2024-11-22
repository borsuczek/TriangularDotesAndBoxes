using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;
using Random = System.Random;

namespace ObjectDefinition
{
    /// <summary>
    /// Represents a dot in the game.
    /// </summary>
    [Serializable]
    public class Dot
    {
        private static Color activeColor = Color.white;
        private static Color unactiveColor = Color.black;
        private static Color disabledColor = Color.gray;
        private static Color firstClickedColor = Color.red;
        public int ID { get; set; }
        public Position Position { get; set; }
        public GameObject Button { get; set; }
        public bool Disabled { get; set; } = false;
        public Dot(int id, Position position, GameObject button)
        {
            ID = id;
            Position = position;
            Button = button;
        }

        /// <summary>
        /// Sets the dot to an active state in GUI.
        /// </summary>
        public void SetActive()
        {
            if (!Disabled)
            {
                Button.GetComponentInChildren<Button>().enabled = true;
                Button.GetComponentInChildren<Image>().color = activeColor;
            }
            else
            {
                Button.GetComponentInChildren<Image>().color = disabledColor;
            }
        }

        /// <summary>
        /// Sets the dot to an inactive state in GUI.
        /// </summary>
        public void SetUnactive()
        {
            Button.GetComponentInChildren<Button>().enabled = false;
            Button.GetComponentInChildren<Image>().color = unactiveColor;
        }

        /// <summary>
        /// Sets the dot to a state indicating it was the first clicked dot.
        /// </summary>
        public void SetFirstClicked()
        {
            Button.GetComponentInChildren<Image>().color = firstClickedColor;
        }

        /// <summary>
        /// Sets the dot to a disabled state when it can't be clicked no more.
        /// </summary>
        public void SetDisabled()
        {
            Disabled = true;
            Button.GetComponentInChildren<Button>().enabled = false;
            Button.GetComponentInChildren<Image>().color = disabledColor;
        }

        /// <summary>
        /// Sets the dot to an enabled state.
        /// </summary>
        public void SetEnabled()
        {
            Disabled = false;
            Button.GetComponentInChildren<Button>().enabled = true;
            Button.GetComponentInChildren<Image>().color = activeColor;
        }
    }


    /// <summary>
    /// Represents the position of a dot.
    /// </summary>
    [Serializable]
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }
    }


    /// <summary>
    /// Represents a computer player in the game.
    /// </summary>
    [Serializable]
    public class Bot
    {
        public int level { get; set; }
        public Func <(int, int)> Tactic;

        public Bot(int level)
        {
            this.level = level;
            if (level == (int)Level.Easy)
            {
                Tactic = EasyMode;
            }
            else if (level == (int)Level.Hard)
            {
                Tactic = HardMode;
            }
        }

        /// <summary>
        /// Tactic for the Easy difficulty level.
        /// </summary>
        /// <returns>A tuple representing a chosen connection.</returns>
        public (int,int) EasyMode()
        {
            var possibleConnections = new List<(int, int)>();

            for (int firstDotId = 0; firstDotId < GameManager.numberOfDots; firstDotId++)
            {
                for(int secondDotId = firstDotId; secondDotId < GameManager.numberOfDots; secondDotId++)
                {
                    if (GameManager.costArray[firstDotId, secondDotId] > GameManager.impossible)
                    {
                        possibleConnections.Add((firstDotId, secondDotId));
                    }
                }
            }
            return RandFromConnections(possibleConnections);
        }

        /// <summary>
        /// Tactic for the Hard difficulty level.
        /// </summary>
        /// <returns>A tuple representing a chosen connection.</returns>
        public (int, int) HardMode()
        {
            const int ourMoveCost = 5;
            const int theirMoveCost = -5;
            int[,] costArray = (int[,])GameManager.costArray.Clone();
            int max = AddCosts(costArray, GameManager.connectionArray, ourMoveCost);
            var possibleConnections = FindMaxConnections(costArray, max);

            if (max != 0)
            {
                return RandFromConnections(possibleConnections);
            }

            int minTriangles = int.MaxValue;
            foreach ((int firstId, int secondId) in possibleConnections)
            {
                int[,] tempCostArray = (int[,])costArray.Clone();
                bool[,] tempConnectionArray = (bool[,])GameManager.connectionArray.Clone();
                int triangles = 0;
                Recursive(firstId, secondId, tempConnectionArray, tempCostArray, ref triangles, minTriangles);

                if (triangles < minTriangles)
                {
                    minTriangles = triangles;
                }
                if(triangles > 0)
                {
                    int totalCost = theirMoveCost * triangles;
                    costArray[firstId, secondId] += totalCost;
                }
            }
            var maxCostConnections = FindMaxConnections(costArray, minTriangles*theirMoveCost);
            return RandFromConnections(maxCostConnections);
        }

        /// <summary>
        /// Selects a random connection from a list of possible connections.
        /// </summary>
        /// <param name="possibleConnections">List of possible connections.</param>
        /// <returns>A tuple representing a randomly selected connection.</returns>
        private (int, int) RandFromConnections(List<(int, int)> possibleConnections)
        {
            Random rand = new Random();

            // Select a random index from the collected indices
            int randomIndex = rand.Next(0, possibleConnections.Count);

            // Access the array element at the selected index
            (int row, int col) selectedIndices = possibleConnections[randomIndex];
            return (selectedIndices);
        }

        /// <summary>
        /// Finds and returns a list of connections with the maximum cost in the costArray.
        /// </summary>
        /// <param name="costArray">The array containing the costs of connections between dots.</param>
        /// <param name="max">The maximum cost to search for.</param>
        /// <returns>List of connections with the maximum cost.</returns>
        private List<(int, int)> FindMaxConnections(int[,] costArray, int max)
        {
            var maxConnections = new List<(int, int)>();
            for (int firstDotId = 0; firstDotId < GameManager.numberOfDots; firstDotId++)
            {
                for (int secondDotId = firstDotId; secondDotId < GameManager.numberOfDots; secondDotId++)
                {
                    if (costArray[firstDotId, secondDotId] == max)
                    {
                        maxConnections.Add((firstDotId, secondDotId));
                    }
                }
            }
            return maxConnections;
        }

        /// <summary>
        /// Adds costs to the costArray based on the number of triangles formed by connections.
        /// </summary>
        /// <param name="costArray">The array containing the costs of connection between dots.</param>
        /// <param name="connectionArray">The array indicating connections between dots.</param>
        /// <param name="ourMoveCost">The cost associated with each move.</param>
        /// <returns>Total cost added to the costArray.</returns>
        private int AddCosts(int[,] costArray, bool[,] connectionArray, int ourMoveCost)
        {
            int cost = 0;
            for (int firstDotId = 0; firstDotId < GameManager.numberOfDots; firstDotId++)
            {
                for (int secondDotId = firstDotId; secondDotId < GameManager.numberOfDots; secondDotId++)
                {
                    if (costArray[firstDotId, secondDotId] > GameManager.impossible)
                    {
                        int triangles = CountTriangles(firstDotId, secondDotId, connectionArray);
                        if (triangles > 0)
                        {
                            cost = triangles * ourMoveCost;
                            costArray[firstDotId, secondDotId] += cost;
                            costArray[secondDotId, firstDotId] += cost;
                        }
                    }
                }
            }
            return cost;
        }

        /// <summary>
        /// Counts the number of triangles formed by a pair of connected dots.
        /// </summary>
        /// <param name="firstDotId">The ID of the first dot in the pair.</param>
        /// <param name="secondDotId">The ID of the second dot in the pair.</param>
        /// <param name="connectionArray">The array indicating connections between dots.</param>
        /// <returns>The number of triangles formed by the pair of connected dots.</returns>
        private int CountTriangles(int firstDotId, int secondDotId, bool[,] connectionArray)
        {
            int numberOfTriagles = 0;

            for (int thirdDotId = 0; thirdDotId < GameManager.numberOfDots; thirdDotId++)
            {
                if (connectionArray[secondDotId, thirdDotId] && connectionArray[thirdDotId, firstDotId]
                    && thirdDotId != firstDotId && thirdDotId != secondDotId)
                {
                    double area = GameManager.Instance.BoardControl.TriangleAreaFromIds(firstDotId, secondDotId, thirdDotId);
                    if (area == GameManager.smallestTriangleArea)
                    {
                        numberOfTriagles++;
                    }
                }
            }
            return numberOfTriagles;
        }

        /// <summary>
        /// Searches for a pair of connected dots forming triangles with the least cost.
        /// </summary>
        /// <param name="costArray">The array containing the costs between dots.</param>
        /// <param name="connectionArray">The array indicating connections between dots.</param>
        /// <returns>A tuple containing the number of triangles and the indices of the connected dots.</returns>
        private (int, (int, int)) SearchTriangles(int[,] costArray, bool[,] connectionArray)
        {
            int triangles = 0;
            for (int firstDotId = 0; firstDotId < GameManager.numberOfDots; firstDotId++)
            {
                for (int secondDotId = firstDotId; secondDotId < GameManager.numberOfDots; secondDotId++)
                {
                    if (costArray[firstDotId, secondDotId] > GameManager.impossible)
                    {
                        triangles = CountTriangles(firstDotId, secondDotId, connectionArray);
                        if (triangles > 0)
                            return (triangles, (firstDotId, secondDotId));
                    }
                }
            }
            return (0, (0, 0));
        }

        /// <summary>
        /// Performs a recursive search for connected dots forming triangles with the least cost.
        /// </summary>
        /// <param name="firstId">The ID of the first dot in the connected pair.</param>
        /// <param name="secondId">The ID of the second dot in the connected pair.</param>
        /// <param name="connectionArray">The array indicating connections between dots.</param>
        /// <param name="costArray">The array containing the costs between dots.</param>
        /// <param name="numberOfTriangles">The current count of triangles.</param>
        /// <param name="minTriangles">The minimum number of triangles to search for.</param>
        private void Recursive(int firstId, int secondId, bool[,] connectionArray, int[,] costArray, ref int numberOfTriangles, int minTriangles)
        {
            connectionArray[firstId, secondId] = true;
            connectionArray[secondId, firstId] = true;
            GameManager.Instance.BoardControl.UpdateCostArray(costArray, connectionArray);
            int[,] tempCostArray = (int[,])costArray.Clone();
            (int triangles, (int, int) index) = SearchTriangles(tempCostArray, connectionArray);
            if (triangles != 0)
            {
                numberOfTriangles += triangles;
                if (numberOfTriangles <= minTriangles)
                    Recursive(index.Item1, index.Item2, connectionArray, costArray, ref numberOfTriangles, minTriangles);
            }
        }
    }


    /// <summary>
    /// Represents a human or bot player in the game.
    /// </summary>
    [Serializable]
    public class Player
    {
        public int points = 0;
        public Material Material { get; set; }
        public GameObject PlayerLayout { get; set; }
        public Bot Bot{ get; set; }
        public Player(int level)
        {
            this.Bot = new Bot(level);
        }
        public Player() { this.Bot = null; }

        /// <summary>
        /// Bolds text in the player's UI layout.
        /// </summary>
        public void BoldText()
        {
            foreach (TextMeshProUGUI text in PlayerLayout.transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                text.fontStyle = FontStyles.Bold;
            }
        }

        /// <summary>
        /// Unbolds text in the player's UI layout.
        /// </summary>
        public void UnboldText()
        {
            foreach (TextMeshProUGUI text in PlayerLayout.transform.GetComponentsInChildren<TextMeshProUGUI>())
            {
                if(text.fontStyle  == FontStyles.Bold)
                    text.fontStyle -= FontStyles.Bold;
            }
        }

        /// <summary>
        /// Updates the player's score in the UI.
        /// </summary>
        public void ChangeScore()
        {
            PlayerLayout.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = points.ToString();
        }
    }

    /// <summary>
    /// Enumeration for different game modes.
    /// </summary>
    enum Mode
    {
        PvP,
        PvE,
        EvE
    }

    /// <summary>
    /// Enumeration for different bot difficulty levels.
    /// </summary>
    enum Level
    {
        Easy,
        Medium,
        Hard
    }
}

using ObjectDefinition;
using UnityEngine;
using UnityEngine.UI;

public class DrawingControl : MonoBehaviour
{
    /// <summary>
    /// Draws a triangle in the game world using the provided dots as vertices.
    /// </summary>
    /// <param name="dotA">The first dot of the triangle.</param>
    /// <param name="dotB">The second dot of the triangle.</param>
    /// <param name="dotC">The third dot of the triangle.</param>
    public void DrawTriangle(Dot dotA, Dot dotB, Dot dotC)
    {
        GameObject newMesh = Instantiate(GameManager.Instance.meshPrefab, GameManager.Instance.TriangleLayout.transform);
        newMesh.name = $"{dotA.ID}_{dotB.ID}_{dotC.ID}";

        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[] { dotA.Button.transform.position, dotB.Button.transform.position, dotC.Button.transform.position };
        mesh.triangles = new int[] { 0, 1, 2 };

        newMesh.GetComponent<MeshFilter>().mesh = mesh;

        newMesh.GetComponent<MeshRenderer>().material = GameManager.currentPlayer.Material;
    }

    /// <summary>
    /// Draws a line between two dots in the game world.
    /// </summary>
    /// <param name="dotAButton">The button associated with the first dot.</param>
    /// <param name="dotBButton">The button associated with the second dot.</param>
    public void DrawLineBetweenDots(Button dotAButton, Button dotBButton)
    {
        Vector3[] linePositions = { dotAButton.transform.position, dotBButton.transform.position };
        GameObject newLineRenderer = Instantiate(GameManager.Instance.lineRendererPrefab, GameManager.Instance.LineLayout.transform);
        newLineRenderer.name = dotAButton.gameObject.name + "_" + dotBButton.name;
        LineRenderer lineRenderer = newLineRenderer.GetComponent<LineRenderer>();
        lineRenderer.material = GameManager.currentPlayer.Material;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(linePositions);
    }
}

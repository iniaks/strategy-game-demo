using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridLayout : MonoBehaviour
{
    [Header("Grid Settings")]
    // public Vector2Int gridSize;
    public int mapSize;

    [Header("Tile Settings")]
    public float outerSize = 1f;
    public float innerSize = 0f;
    public float hexHeight = 1f;
    public bool isFlatTopped;
    public Material w_material;
    public Material b_material;
    
    private void OnEnable() {
        LayoutGrid();
    }

    // private void OnValidate() {
    //     if (Application.isPlaying) {
    //         LayoutGrid();
    //     }
    // }

    private void LayoutGrid () {
        int mapLimit = Mathf.RoundToInt(mapSize / 2);
        // Debug.Log(mapLimit);
        for (int r = -mapLimit; r <= mapLimit; r++) {
            int start = r >= 0 ? -mapLimit : -mapLimit - r;
            int upper = r >= 0 ? mapLimit - r : mapLimit;
            for (int q = start; q <= upper; q++) {
                // Debug.Log($"r={r},q={q}");
                GameObject tile = new GameObject($"Hex {q},{r},{0-q-r}", typeof(HexRenderer));
                tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(q, r));

                HexRenderer hexRenderer = tile.GetComponent<HexRenderer>();
                hexRenderer.isFlatTopped = isFlatTopped;
                hexRenderer.outerSize = outerSize;
                hexRenderer.innerSize = innerSize;
                hexRenderer.height = hexHeight;
                hexRenderer.coordinate = new Vector2Int(q, r);
                tile.GetComponent<MeshRenderer>().material = w_material;
                hexRenderer.DrawMesh();
                tile.transform.SetParent(transform, true);
            }
        }
    }

    public Vector3 GetPositionForHexFromCoordinate(Vector2Int coordinate) {
        int column = isFlatTopped ? coordinate.x : coordinate.x + (coordinate.y - (coordinate.y&1)) / 2;
        int row = isFlatTopped ? coordinate.y + (coordinate.x - (coordinate.x&1)) / 2 : coordinate.y;

        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float size = outerSize;

        if (!isFlatTopped) {
            shouldOffset = (row % 2) == 0;
            width = Mathf.Sqrt(3f) * size;
            height = 2f * size;

            horizontalDistance = width;
            verticalDistance = height * (3f / 4f);

            offset = (shouldOffset) ? width / 2 : 0;

            xPosition = (column * (horizontalDistance)) + offset;
            yPosition = (row * verticalDistance);
        } else {
            shouldOffset = (column % 2) == 0;
            height = Mathf.Sqrt(3f) * size;
            width = 2f * size;

            horizontalDistance = width * (3f / 4f);
            verticalDistance = height;

            offset = (shouldOffset) ? height / 2 : 0;

            xPosition = (column * (horizontalDistance)) ;
            yPosition = (row * verticalDistance) - offset;
        }

        return new Vector3(xPosition, 0, -yPosition);
    }
}

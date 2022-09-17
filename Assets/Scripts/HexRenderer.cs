using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public struct Face {
    public List<Vector3> vertices {get; private set;}
    public List<int> triangles {get; private set;}
    public List<Vector2> uvs {get; private set;}
    public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs) {
        this.vertices = vertices;
        this.triangles = triangles;
        this.uvs = uvs;
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(EventTrigger))]
[RequireComponent(typeof(BoxCollider))]
public class HexRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    private Mesh m_mesh;
    private BoxCollider m_collider;
    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    public Vector2Int coordinate;
    public Material material;

    public float innerSize;
    public float outerSize;
    public float height;

    private void Awake() {
        m_collider = GetComponent<BoxCollider>();
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();

        m_mesh = new Mesh();
        m_mesh.name = "Hex";

        m_meshFilter.mesh = m_mesh;
        m_meshRenderer.material = material;
        triggerSelect();
    }

    private void OnEnable() {
        DrawMesh();
    }

    private void OnValidate() {
        if (Application.isPlaying) {
            DrawMesh();
        }
    }

    public void DrawMesh() {
        DrawFaces();
        CombineFaces();
    }

    private List<Face> m_faces;
    private void DrawFaces() {
        m_faces = new List<Face>();
        for (int point = 0; point < 6; point++) {
            m_faces.Add(CreateFace(innerSize, outerSize, height / 2f, height / 2f, point));
        }
        for (int point = 0; point < 6; point++) {
            m_faces.Add(CreateFace(innerSize, outerSize, -height / 2f, -height / 2f, point, true));
        }
        for (int point = 0; point < 6; point++) {
            m_faces.Add(CreateFace(outerSize, outerSize, height / 2f, -height / 2f, point, true));
        }
        for (int point = 0; point < 6; point++) {
            m_faces.Add(CreateFace(innerSize, innerSize, height / 2f, -height / 2f, point, false));
        }
    }

    private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false) {
        Vector3 pointA = GetPoint(innerRad, heightB, point);
        Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point+1 : 0);
        Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point+1 : 0);
        Vector3 pointD = GetPoint(outerRad, heightA, point);

        List<Vector3> vertices = new List<Vector3>() {pointA, pointB, pointC, pointD};
        List<int> triangles = new List<int>() {0, 1, 2, 2, 3, 0};
        List<Vector2> uvs = new List<Vector2>() {new Vector2(0,0), new Vector2(1,0), new Vector2(1,1), new Vector2(0,1)};
        if (reverse) {
            vertices.Reverse();
        }
        return new Face(vertices, triangles, uvs);
    }

    public bool isFlatTopped;
    protected Vector3 GetPoint(float size, float height, int index) {
        float angle_deg = isFlatTopped ? 60 * index : 60 * index - 30;
        float angle_rad = Mathf.PI / 180f * angle_deg;
        return new Vector3((size * Mathf.Cos(angle_rad)), height, size * Mathf.Sin(angle_rad));
    }

    private void CombineFaces() {
        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < m_faces.Count; i++) {
            vertices.AddRange(m_faces[i].vertices);
            uvs.AddRange(m_faces[i].uvs);

            int Offset = (4*i);
            foreach (int triangle in m_faces[i].triangles) {
                tris.Add(triangle + Offset);
            }
        }

        m_mesh.vertices = vertices.ToArray();
        m_mesh.triangles = tris.ToArray();
        m_mesh.uv = uvs.ToArray();
        m_mesh.RecalculateNormals();
        m_collider.size = new Vector3(outerSize + outerSize * 3f/4f, height, outerSize + outerSize * 3f/4f);
    }

    private void triggerSelect () {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback = new EventTrigger.TriggerEvent();
        UnityEngine.Events.UnityAction<BaseEventData> callback = new UnityEngine.Events.UnityAction<BaseEventData>(selectHex);
        entry.callback.AddListener(callback);
        GetComponent<EventTrigger>().triggers.Add(entry);
    }

    public void selectHex (BaseEventData e) {
        Debug.Log(this.coordinate);
    }
}

using System;
using UnityEngine;

public class Planet : MonoBehaviour
{

    public int subdivisions = 0;
    public float testOffset = 0;

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    ShapeGenerator shapeGenerator;

    [SerializeField, HideInInspector]
    MeshFilter[,] meshFilters;
    TerrainFace[,] terrainFaces;

    private int _subdivisions;
    private int _divisions;


    void Initialize()
    {
        _divisions = (int) Mathf.Pow(4, subdivisions);

        shapeGenerator = new ShapeGenerator(shapeSettings);
        if (meshFilters == null || meshFilters.Length == 0 || _subdivisions != subdivisions) {
            meshFilters = new MeshFilter[6, _divisions];
        }

        _subdivisions = subdivisions;

        terrainFaces = new TerrainFace[6, _divisions];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        Vector2[] offsets = {
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, 0)
        };

        for (int d = 0; d < 6; d++)
        {
            // Remember arrays count 0 as a index. That is why we subtract 1 from divisions.
            GenerateFaces(directions, offsets, d, _divisions - 1);
        }
    }

    private void GenerateFaces(Vector3[] directions, Vector2[] offsets, int d, int divisions) {
        if (divisions < 0) {
            return;
        }

        if (meshFilters[d, divisions] == null)
        {
            GameObject meshObj = new GameObject("Chunk");
            meshObj.transform.parent = transform;

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
            meshFilters[d, divisions] = meshObj.AddComponent<MeshFilter>();
            meshFilters[d, divisions].sharedMesh = new Mesh();
        }

        // Size of 0.125 has offset of 6.5f, 6.5f
        // Size of 0.25 has offset of 2.5f, 2.5f
        // Size of 0.5 has offset of 0.5f, 0.5f
        // Size of 1 has offset of -0.5f, -0.5f
        // Size of 2 has offset of -1f, -1f
        Vector2 offset = offsets[divisions % 4] + new Vector2(-1.5f + Mathf.Sqrt(_divisions) / 2, -1.5f + Mathf.Sqrt(_divisions) / 2);

        Mesh mesh = meshFilters[d, divisions].sharedMesh;
        
        //float size = 0.125f;
        float size = 2.0f / Mathf.Sqrt(_divisions);

        TerrainFace terrainFace = new TerrainFace(shapeGenerator, mesh, resolution, directions[d], offset, size);
        terrainFaces[d, divisions] = terrainFace;

        GenerateFaces(directions, offsets, d, divisions - 1);
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenerateColours()
    {
        foreach (MeshFilter m in meshFilters)
        {
            if (m != null)
                m.GetComponent<MeshRenderer>().sharedMaterial.color = colourSettings.planetColour;
        }
    }
}

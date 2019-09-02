using UnityEngine;

public class Planet : MonoBehaviour
{

    public int subdivisions = 0;
    public float offset = 0;

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
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;


    void Initialize()
    {
        shapeGenerator = new ShapeGenerator(shapeSettings);
        if (meshFilters == null || meshFilters.Length == 0)
            meshFilters = new MeshFilter[6 * 4];
        terrainFaces = new TerrainFace[6 * 4];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        Vector2[] offsets = {
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, 0)
        };

        int a = 0;
        for (int d = 0; d < 6; d++)
        {
            for (int i = 0; i < 4; i++)
            {
                if (meshFilters[i + a] == null)
                {
                    GameObject meshObj = new GameObject("Chunk");
                    meshObj.transform.parent = transform;

                    meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                    meshFilters[i + a] = meshObj.AddComponent<MeshFilter>();
                    meshFilters[i + a].sharedMesh = new Mesh();
                }

                terrainFaces[i + a] = new TerrainFace(shapeGenerator, meshFilters[i + a].sharedMesh, resolution, directions[d], offsets[i] - new Vector2(-offset, -offset) * 0, 1f);
            }
            a += 4;
        }
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
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colourSettings.planetColour;
        }
    }
}

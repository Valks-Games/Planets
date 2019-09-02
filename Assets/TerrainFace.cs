using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace {

    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;
    Vector2 offset;
    float faceSize;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp, Vector2 offset, float faceSize)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;
        this.offset = offset;
        this.faceSize = faceSize;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
        int triIndex = 0;

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);

                Vector3 pointOnUnitCube = localUp + (percent.x - offset.x) * faceSize * axisA + (percent.y - offset.y) * faceSize * axisB;

                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitCube);
                //vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                if (x != resolution - 1 && y != resolution - 1)
                {
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;

                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;
                    triIndex += 6;
                }
            }
        }
        
        mesh.Clear();
        mesh.name = "Chunk";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = vertices;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float fov {get; private set;}
    public int rayCount = 2;
    public float distance = 30f;
    Mesh mesh;
    float dirAngle;
    Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        fov = 90;
        float angle = dirAngle;
        float angleIncrease = fov/rayCount;

        mesh = new Mesh();    
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;
        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex = origin + FindDirWithAngle(angle) * distance;
            vertices[vertexIndex] = vertex;

            if(i > 0){
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
            
                triangleIndex += 3;
            }
            vertexIndex++;
            angle -= angleIncrease;
        }


        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
    private void LateUpdate() {
        UpdateMesh();
    }
    public static Vector3 FindDirWithAngle(float angle){
        angle *= Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public void UpdateMesh(){
        
        float angle = dirAngle;
        float angleIncrease = fov / rayCount;
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];

        vertices[0] = origin;
        for (int i = 1; i <= rayCount+1; i++)
        {
            Vector3 vertex;
            RaycastHit2D hit2D = Physics2D.Raycast(origin, FindDirWithAngle(angle), distance);
            if(hit2D.collider == null){
                vertex = origin + FindDirWithAngle(angle) * distance;
            }else
            {
                vertex = hit2D.point;
            }

            vertices[i] = vertex;
            angle -= angleIncrease;
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    public void SetOrigin(Vector3 origin){
        this.origin = origin;
    }

    public void SetDir(float dirAngle){
        this.dirAngle = dirAngle;
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Transform head;
    public Transform trail;
 public Vector3 heading;
 new CircleCollider2D collider;
 

 Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        heading = new Vector3(0.5f, 0.2f, 0); // TODO Randomise
        heading.Normalize();

        collider = GetComponent<CircleCollider2D>();

    }

    public void Kill()
    {
        GetComponent<Snake>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {

    }

    private void OnCollisionEnter2D(Collision2D other) {
        print("COLLISION");
    }

    public float speed = 3;
    public float rotSpeed = 3;

    public float trailWidth = 5;
    public int nMod = 3;

    void Move(){

        head.position += speed * head.up;

        // Calculate new heading
        float hInput = Input.GetAxis("Horizontal");
        int sign = 
            hInput == 0 ? 0 : hInput < 0 ? -1 : 1;


        // Quaternion q = Quaternion.Euler(0,0, -sign * rotSpeed);
        // heading = q * heading;
        float d = -sign * rotSpeed * Time.deltaTime;
        head.Rotate(new Vector3(0, 0, d));
    }


    void Trail(){

        if(Time.frameCount % nMod != 0) return;

        mesh = new Mesh();

        Vector3 p1 = head.localPosition;
        p1 -= head.right * trailWidth;
        
        Vector3 p2 = head.localPosition;
        p2 += head.right * trailWidth;

        vertList.Add(p1);
        vertList.Add(p2);

        if(vertList.Count <= 2) return;

        Vector3[] vertices = new Vector3[vertList.Count];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[(((vertices.Length / 2) - 1) * 6)];

        int triangleIndex = 0;

        for (int i = 1; i < (vertices.Length / 2); i++)
        {
            int i1 = (i-1)*2;
            int i2 = (i-1)*2 + 1;
            int i3 = i*2;
            int i4 = i*2 + 1;

            Color c = new Color(((float)i) / (vertices.Length/2),1,1);
            Debug.DrawLine(vertList[i1], vertList[i2], c);

            vertices[i1] = vertList[i1];
            vertices[i2] = vertList[i2];
            vertices[i3] = vertList[i3];
            vertices[i4] = vertList[i4];

            triangles[triangleIndex] = i3;
            triangles[triangleIndex + 1] = i2;
            triangles[triangleIndex + 2] = i1;

            triangles[triangleIndex + 3] = i2;
            triangles[triangleIndex + 4] = i3;
            triangles[triangleIndex + 5] = i4;

            triangleIndex += 6;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        trail.GetComponent<MeshFilter>().mesh = mesh;

        // trail.GetComponent<MeshCollider>().sharedMesh = null;
        // trail.GetComponent<MeshCollider>().sharedMesh = mesh;

        vertices = vertices.Where((n,i) => i < vertices.Length - 2).ToArray();
        triangles = triangles.Where((n,i) => i < triangles.Length - 6).ToArray();
        SetCollider(vertices, triangles);
    }

    void SetCollider(Vector3[] vertices, int[] triangles){
        PolygonCollider2D polygonCollider2D = trail.GetComponent<PolygonCollider2D>();
        polygonCollider2D.pathCount = 1;

        var boundaryPath = EdgeHelpers.GetEdges(triangles).FindBoundary().SortEdges();

        Vector3[] yourVectors = new Vector3[boundaryPath.Count];
        for(int i = 0; i < boundaryPath.Count; i++)
        {
            yourVectors[i] = vertices[ boundaryPath[i].v1 ];
        }
        List<Vector2> newColliderVertices = new List<Vector2>();

        for(int i=0; i < yourVectors.Length; i++)
        {
            newColliderVertices.Add(new Vector2(yourVectors[i].x, yourVectors[i].y));
        }

        Vector2[] newPoints = newColliderVertices.Distinct().ToArray();

        EditorUtility.SetDirty(polygonCollider2D);

        polygonCollider2D.SetPath(0, newPoints);
    }

    List<Vector3> vertList = new List<Vector3>();

    // Update is called once per frame
    void Update()
    {
        Move();

        Trail();

    }
}

public static class EdgeHelpers
 {
     public struct Edge
     {
         public int v1;
         public int v2;
         public int triangleIndex;
         public Edge(int aV1, int aV2, int aIndex)
         {
             v1 = aV1;
             v2 = aV2;
             triangleIndex = aIndex;
         }
     }
 
     public static List<Edge> GetEdges(int[] aIndices)
     {
         List<Edge> result = new List<Edge>();
         for (int i = 0; i < aIndices.Length; i += 3)
         {
             int v1 = aIndices[i];
             int v2 = aIndices[i + 1];
             int v3 = aIndices[i + 2];
             result.Add(new Edge(v1, v2, i));
             result.Add(new Edge(v2, v3, i));
             result.Add(new Edge(v3, v1, i));
         }
         return result;
     }
 
     public static List<Edge> FindBoundary(this List<Edge> aEdges)
     {
         List<Edge> result = new List<Edge>(aEdges);
         for (int i = result.Count-1; i > 0; i--)
         {
             for (int n = i - 1; n >= 0; n--)
             {
                 if (result[i].v1 == result[n].v2 && result[i].v2 == result[n].v1)
                 {
                     // shared edge so remove both
                     result.RemoveAt(i);
                     result.RemoveAt(n);
                     i--;
                     break;
                 }
             }
         }
         return result;
     }
     public static List<Edge> SortEdges(this List<Edge> aEdges)
     {
         List<Edge> result = new List<Edge>(aEdges);
         for (int i = 0; i < result.Count-2; i++)
         {
             Edge E = result[i];
             for(int n = i+1; n < result.Count; n++)
             {
                 Edge a = result[n];
                 if (E.v2 == a.v1)
                 {
                     // in this case they are already in order so just continoue with the next one
                     if (n == i+1)
                         break;
                     // if we found a match, swap them with the next one after "i"
                     result[n] = result[i + 1];
                     result[i + 1] = a;
                     break;
                 }
             }
         }
         return result;
     }
 }

using System.Collections.Generic;
using UnityEngine;

public class Snake2 : MonoBehaviour
{
 public Vector3 heading;
 CircleCollider2D collider;
public MeshFilter filter;

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

    public float speed = 3;
    public float rotSpeed = 3;

    public float trailWidth = 5;

    void Move(){

        transform.position += speed * transform.up;

        // Calculate new heading
        float hInput = Input.GetAxis("Horizontal");
        int sign = 
            hInput == 0 ? 0 : hInput < 0 ? -1 : 1;


        // Quaternion q = Quaternion.Euler(0,0, -sign * rotSpeed);
        // heading = q * heading;

        transform.Rotate(new Vector3(0, 0, -sign * rotSpeed * Time.deltaTime));
    }

    public int nMod = 3;

    void Trail(){

        if(Time.frameCount % nMod != 0) return;

        mesh = new Mesh();
        filter.mesh = mesh;

        Vector3 p1 = transform.position;
        p1 -= transform.right * trailWidth;
        
        Vector3 p2 = transform.position;
        p2 += transform.right * trailWidth;

        vertList.Add(p1);
        vertList.Add(p2);

        if(vertList.Count <= 2) return;

        Vector3[] vertices = new Vector3[vertList.Count];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[((vertices.Length / 2) - 1) * 6];

        int triangleIndex = 0;

        for (int i = 1; i < vertices.Length / 2; i++)
        {
            int i1 = (i-1)*2;
            int i2 = (i-1)*2 + 1;
            int i3 = i*2;
            int i4 = i*2 + 1;

            Debug.DrawLine(vertList[i1], vertList[i2]);

            vertices[i1] = vertList[i1];
            vertices[i2] = vertList[i2];
            vertices[i1] = vertList[i3];
            vertices[i2] = vertList[i4];

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
    }

    List<Vector3> vertList = new List<Vector3>();

    // Update is called once per frame
    void Update()
    {
        Move();

        Trail();

    }
}

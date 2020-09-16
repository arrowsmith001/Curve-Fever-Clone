using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public Vector3 heading;

    // Start is called before the first frame update
    void Start()
    {
        heading = new Vector3(0.5f, 0.2f, 0); // TODO Randomise
        heading.Normalize();
    }

    public void Kill()
    {
        GetComponent<Snake>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        other.GetComponent<Trail>().CheckIn(this.GetComponent<Snake>());
    }

    public float speed = 3;
    public float rotSpeed = 3;

    public GameObject trailPart;

    // Update is called once per frame
    void Update()
    {
        // Calculate new heading
        float hInput = Input.GetAxis("Horizontal");
        int sign = 
            hInput == 0 ? 0 : hInput < 0 ? -1 : 1;

        heading = Quaternion.Euler(0,0, -sign * rotSpeed * Time.deltaTime) * heading;
        heading.Normalize();

        transform.position += speed * heading;


        GameObject trail = Instantiate(trailPart, transform.position, Quaternion.identity);
        trail.transform.localScale = transform.localScale;

        trail.GetComponent<Trail>().PassParent(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    Snake parent;
    
    private void Awake() {
        parent = transform.parent.GetComponent<Snake>();
    }

    // private void OnCollisionEnter(Collision other) {
    //     print("COLLISION");
    // }

    private void OnTriggerEnter2D(Collider2D other) {

        if(other.gameObject.GetComponent<Trail>() != null){
            
            Trail trail = other.gameObject.GetComponent<Trail>();
            trail.CheckIn(this.transform.parent.GetComponent<Snake>());

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

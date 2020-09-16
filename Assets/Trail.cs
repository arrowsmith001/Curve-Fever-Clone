using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    int triggerCount = 0;

    // Start is called before the first frame update
    public void CheckIn(Snake snake)
    {
        if(snake != parent) {
            snake.Kill();
            return;
        }
        
        triggerCount++;
        if(triggerCount == 2) snake.Kill();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Snake parent;
    public void PassParent(Snake snake)
    {
        this.parent = snake;
    }
}

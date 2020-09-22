using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Trail : MonoBehaviour
    {
        Snake parent;

        private void Awake() {
            parent = transform.parent.GetComponent<Snake>();
        }
        
        int triggerCount = 0;

        // Start is called before the first frame update
        public void CheckIn(Snake snake)
        {
                snake.Kill();
     
            
            // triggerCount++;
            // if(triggerCount == 50) snake.Kill();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

    }








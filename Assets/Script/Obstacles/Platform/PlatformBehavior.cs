using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatFormBehavior : Machine
{
    #region Script Parameters
    public bool loop;
    public float delay;

    public List<GameObject> points = new List<GameObject>();
    private int currentIndex = 0;
    private float timer; 

    bool isTimerOn = false;
    #endregion

    #region Methods
    public override void MachineBehaviour()
    {
        if (isTimerOn)
        {
            if (timer < delay)
                timer += Time.deltaTime;
            else
            {
                timer = 0.0f;
                isTimerOn = false;
            }
        }

        if (activeGenerator == requiredGeneratorsCount && !isTimerOn)
        { 
            MovePlatform();
        }
    }

    private void MovePlatform()
    {
        if (points.Count != 0)
        {
            Vector3 targetPosition = points[currentIndex].transform.position;

            Vector3 direction = targetPosition - machineObject.transform.position;
            float distance = direction.magnitude;

            Vector3 moveVector = direction.normalized * Mathf.Min(speed * Time.deltaTime, distance);
            machineObject.transform.position += moveVector;

            if (distance < 0.1f)
            {
                isTimerOn = true; 
                currentIndex++;

                if (currentIndex >= points.Count)
                {
                    if (loop)
                    {
                        currentIndex = 0;
                    }
                    else
                    {
                        Disable();
                        return;
                    }
                }
            }
        }
    }
    #endregion
}

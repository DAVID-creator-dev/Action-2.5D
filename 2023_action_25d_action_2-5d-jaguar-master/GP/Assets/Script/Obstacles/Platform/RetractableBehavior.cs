using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RetractableBehavior : Machine
{
    #region Script Parameters
    [SerializeField] private List<GameObject> points = new List<GameObject>();
    [SerializeField] private float delay;

    public AudioSource audioSource;
    public bool isDoor; 

    private bool hasPlaySound = true;
    private float timer;
    private int targetIndex = 0;
    #endregion
    public override void MachineBehaviour()
    {
        if (activeGenerator == requiredGeneratorsCount)
        {
            if (hasPlaySound)
            {
                if (isDoor)
                    PlaySound("Open_Door");
                else
                    PlaySound("Reractable_Plateform_On"); 

                hasPlaySound = false;
            }
            MovePlatform(points[1].transform.position); 
        }
        else
        {
            if (timer < delay)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if (!hasPlaySound)
                {
                    if (isDoor)
                        PlaySound("Close_Door");
                    else
                        PlaySound("Reractable_Plateform_Off");

                    hasPlaySound = true;
                }
                MovePlatform(points[0].transform.position); 
            }
        }
    }

    private void PlaySound(string name)
    {
        GameManager.Get.OnSFXMachinesUpdate.Invoke(name, audioSource, false);
    }

    private void MovePlatform(Vector3 targetPosition)
    {
        if (points.Count != 0)
        {
            Vector3 direction = targetPosition - machineObject.transform.position;
            float distance = direction.magnitude;

            Vector3 moveVector = direction.normalized * Mathf.Min(speed * Time.deltaTime, distance);
            machineObject.transform.position += moveVector;

            if (distance < 0.1f)
            {
                timer = 0f; 
                targetIndex = (targetIndex + 1) % points.Count; 
            }
        }
    }
}

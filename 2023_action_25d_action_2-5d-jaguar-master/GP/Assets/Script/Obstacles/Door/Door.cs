using System.Collections;
using UnityEngine;

public class Door : Machine
{
    #region Script Paramaters
    private bool isInAnimation = false; 
    private Coroutine currentAnimation; 

    public float offsetY;
    public float animationDuration; 
    #endregion

    #region Unity Methods
    public override void InitMachine()
    {
        initialPosition = machineObject.transform.position;
        targetPosition = new Vector3(machineObject.transform.position.x, initialPosition.y + offsetY, machineObject.transform.position.z);
    }
    #endregion

    #region Methods
    public override void CheckGenerator()
    {
        if (!isInAnimation) 
        {
            if (activeGenerator == requiredGeneratorsCount)
            {
                currentAnimation = StartCoroutine(OpenCloseDoor(targetPosition));
            }
            else
            {
                currentAnimation = StartCoroutine(OpenCloseDoor(initialPosition));
            }
        }
        else 
        {
            isInAnimation = false;
            StopCoroutine(currentAnimation);
            CheckGenerator(); 
        }
    }

    private IEnumerator OpenCloseDoor(Vector3 target)
    {
        isInAnimation = true;
        float elapsedTime = 0.0f;
        Vector3 startingPosition = machineObject.transform.position; 

        while (elapsedTime < animationDuration)
        {
            float animationProgress = elapsedTime / animationDuration;
            Vector3 newPosition = Vector3.Lerp(startingPosition, target, animationProgress);
            machineObject.transform.position = newPosition;
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        machineObject.transform.position = target;
        isInAnimation = false;
    }
    #endregion
}

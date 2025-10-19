using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    #region Script Parameters
    [Header("zAxis Parameters")]
    [SerializeField] private bool turnLeftZAxis;
    [SerializeField] private bool turnRightZAxis;

    [Header("xAxis Parameters")]
    [SerializeField] private bool turnLeftXAxis;
    [SerializeField] private bool turnRightXAxis;

    private Vector3 xAxis = new Vector3(1, 0, 0);
    private Vector3 zAxis = new Vector3(0, 0, 1);

    private bool oneTime = false; 
    
    private PlayerController playerController;
    #endregion

    #region Methods Unity
    private void Start()
    {
        playerController = GameManager.Get.playerController;
    }
    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        if (!oneTime)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Get.isInCinematic = true;
                playerController.moveOnZAxis = playerController.moveOnZAxis ? false : true;

                if (playerController != null)
                {
                    if (turnLeftZAxis || turnRightZAxis)
                    {
                        MovePlayer();
                        playerController.lastDirection = (turnLeftZAxis ? zAxis : -zAxis);
                        playerController.Camera();
                    }
                    else if (turnLeftXAxis || turnRightXAxis)
                    {
                        MovePlayer();
                        playerController.lastDirection = (turnLeftXAxis ? -xAxis : xAxis);
                        playerController.Camera();
                    }
                }
                else
                    Debug.LogWarning("Missing Player"); 
            }
        }
    }

    private void MovePlayer()
    {
        oneTime = true; 
        StartCoroutine(MovePlayerCoroutine());
    }

    private IEnumerator MovePlayerCoroutine()
    {
        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsedTime / duration);
            playerController.transform.position = Vector3.Lerp(playerController.transform.position, new Vector3(transform.position.x, playerController.transform.position.y, transform.position.z), t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        GameManager.Get.isInCinematic = false;
    }
    #endregion
}

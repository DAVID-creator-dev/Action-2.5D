using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    #region Script Paramaters
    public int id;
    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (id >= GameManager.Get.lastVisitedCheckpointID)
            {
                GameManager.Get.lastVisitedCheckpointID = id;
                GameManager.Get.playerController.respawnPos = transform.position;
            }
            else
            {
                Debug.Log("Vous ne pouvez pas revenir en arri�re !");
            }
        }
    }
    #endregion
}

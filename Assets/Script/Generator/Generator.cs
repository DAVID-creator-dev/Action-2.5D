using System.Collections;
using UnityEngine;

public class Generator : MonoBehaviour
{
    #region Script Parameters
    private bool wasPlayerColliding;

    private float activationDelay = 0.5f; 
    private float elapsedTime = 0.0f;
    #endregion

    #region Methods
    private void Update()
    {
        if (gameObject.transform.parent != gameObject.CompareTag("Player"))
        {
            elapsedTime = 0f; 
            return;
        }

        elapsedTime += Time.deltaTime;

        if (elapsedTime >= activationDelay)
        {
            CollisionBox();
        }
    }

    private void CollisionBox()
    {
        Vector3 boxCenter = transform.position + Vector3.up * 0.5f;
        Vector3 boxSize = new Vector3(0.25f, 0.2f, 0.25f);

        Collider[] boxColliders = Physics.OverlapBox(boxCenter, boxSize, Quaternion.identity);

        bool isPlayerColliding = false;

        foreach (Collider collider in boxColliders)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("test"); 
                isPlayerColliding = true;

                Vector3 playerPosition = GameManager.Get.playerController.transform.position;
                Vector3 influenceDirection = (new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z) - playerPosition);

                GameManager.Get.playerController.externalDirectionInfluence = influenceDirection * Time.deltaTime;
            }
        }

        if (!wasPlayerColliding && isPlayerColliding)
        {
            wasPlayerColliding = true;
        }
        else if (wasPlayerColliding && !isPlayerColliding)
        {
            GameManager.Get.playerController.externalDirectionInfluence = Vector3.zero;
            wasPlayerColliding = false;
        }
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    #region Script Parameters 
    private string[] footstepNames = { "FootStep1", "FootStep2", "FootStep3", "FootStep4" };
    #endregion

    #region Methods
    private void PlayFootStepSound()
    {
        int randomIndex = Random.Range(0, footstepNames.Length);
        string randomFootstepName = footstepNames[randomIndex];

        GameManager.Get.OnSFXPlayerUpdate.Invoke(randomFootstepName);
    }
    #endregion
}

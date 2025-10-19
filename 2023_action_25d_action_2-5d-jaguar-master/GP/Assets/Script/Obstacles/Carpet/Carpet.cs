using UnityEngine;

public class Carpet : Machine
{
    #region Script Parameters
    [HideInInspector] public Vector3 directionCarpet;
    public bool isForwardDir;
    public GameObject forward;
    public GameObject backward;
    [HideInInspector] public GameObject currentParticle;

    public AudioSource audioSource;
    private bool hasPlaySound = true;
    #endregion

    public override void InitMachine()
    {
        if (isForwardDir)
        {
            directionCarpet = transform.forward;
            if (forward == null )
                Debug.LogWarning("Missing particles reference"); 
            else
                currentParticle = backward;
        }
        else
        {
            directionCarpet = -transform.forward;
            if (backward == null)
                Debug.LogWarning("Missing particles reference");
            else
                currentParticle = forward;
        }
    }
    #region Methods
    private void OnTriggerStay(Collider other)
    {
        if (activeGenerator == requiredGeneratorsCount)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Get.playerController.externalDirectionInfluence = directionCarpet * (speed / 5) * Time.deltaTime;
            }
            else if (other.CompareTag("Generator"))
            {
                other.gameObject.transform.position += directionCarpet * speed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GameManager.Get.playerController.externalDirectionInfluence = Vector3.zero;  
    }
    public override void MachineBehaviour()
    {
        if (currentParticle == null)
        {
            Debug.LogWarning("Missing particles reference");
        }
        else
        {
            if (activeGenerator == requiredGeneratorsCount)
            {
                if (hasPlaySound)
                {
                    PlaySound("Treadmill_Start", false);
                    PlaySound("Treadmill_Ongoing", true); 
                    hasPlaySound = false;
                }
                currentParticle.SetActive(true);
            }
            else
            {
                if (!hasPlaySound)
                {
                    PlaySound("Treadmill_End", false);
                    hasPlaySound = true;

                }
                currentParticle.SetActive(false);
            }
        }
    }

    private void PlaySound(string name, bool loop)
    {
        GameManager.Get.OnSFXMachinesUpdate.Invoke(name, audioSource, loop);
    }
    #endregion
}

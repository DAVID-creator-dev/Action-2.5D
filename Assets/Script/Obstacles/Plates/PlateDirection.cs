using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateDirection : MonoBehaviour
{
    #region Script Parameters
    [SerializeField] private Carpet carpet;
    [SerializeField] private Material enableCable;
    [SerializeField] private Material disableCable;
    [SerializeField] private Material enablePlate;
    [SerializeField] private Material disablePlate;
    [SerializeField] private MeshRenderer[] cables;
    [SerializeField] private AudioSource audioSource; 
    private Renderer plateRendrer;
    #endregion

    #region Methods Unity
    private void Start()
    {
        plateRendrer = transform.gameObject.GetComponent<Renderer>();
    }
    #endregion

    #region Methods
    private void OnTriggerEnter(Collider other)
    {
        ToggleDirection(other);
        if (other.gameObject.tag == "Generator")
        {
            GameManager.Get.OnSFXMachinesUpdate.Invoke("Drop_Generator_Ground", audioSource, false);

            plateRendrer.material = enableCable;

            if (cables.Length > 0)
            {
                for (int i = 0; i < cables.Length; i++)
                {
                    if (cables[i] != null && enableCable != null)
                        cables[i].material = enableCable;
                    else
                        Debug.LogWarning("Cable material or array element " + i + " is null. Bind something to the array or delete.");
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        ToggleDirection(other);

        if (other.gameObject.tag == "Generator")
        {
            plateRendrer.material = disableCable;

            if (cables.Length > 0)
            {
                for (int i = 0; i < cables.Length; i++)
                {
                    if (cables[i] != null && disableCable != null)
                        cables[i].material = disableCable;
                    else
                        Debug.LogWarning("Cable material or array element " + i + " is null. Bind something to the array or delete.");
                }
            }
        }
    }
    private void ToggleDirection(Collider other)
    {
        if (other.gameObject.tag == "Generator")
        {
            if (carpet != null)
            {
                carpet.directionCarpet = -carpet.directionCarpet;

                if (carpet.directionCarpet.x < 0.0f)
                    SwitchParticle(carpet.forward);
                else
                    SwitchParticle(carpet.backward);
            }
            else
                Debug.LogWarning("Missing carpet"); 
        }
    }
    private void SwitchParticle(GameObject particle)
    {
        if (carpet != null)
        {
            if (carpet.activeGenerator == carpet.requiredGeneratorsCount)
            {
                carpet.currentParticle.SetActive(false);
                carpet.currentParticle = particle;
                carpet.currentParticle.SetActive(true);
            }
        }
        else
            Debug.LogWarning("Missing carpet");
    }
    #endregion
}
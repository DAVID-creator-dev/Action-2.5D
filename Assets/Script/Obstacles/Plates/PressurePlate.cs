using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    #region Script Paramaters
    [SerializeField] private bool isInvert;
    [SerializeField] private MeshRenderer[] cables;
    [SerializeField] private bool isTriggerVolume;
    [SerializeField] private Machine[] machine;
    [SerializeField] private Material enableCable;
    [SerializeField] private Material disableCable;
    [SerializeField] private Material enablePlate;
    [SerializeField] private Material disablePlate;
    [SerializeField] private AudioSource audioSource; 

    private Renderer plateRenderer; 
    #endregion

    #region Methods
    public void Start()
    {
        plateRenderer = gameObject.transform.GetComponent<Renderer>();

        if (isInvert)
        {
            if (machine != null)
            {
                for (int i = 0; i < machine.Length; i++)
                    machine[i].CheckGenerator();
            }
            else
                Debug.LogWarning("You need to bind machines"); 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && isTriggerVolume)
        {
            if (isInvert)
            {
                DisableSystem();
            }
            else
            {
                EnableSystem();
            }
            return;
        }

        if (other.gameObject.tag == "Generator")
        {
            GameManager.Get.OnSFXMachinesUpdate.Invoke("Drop_Generator_Ground", audioSource, false);

            if (!isInvert)
                EnableSystem();
            else
                DisableSystem();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Generator")
        {
            if (!isInvert)
                DisableSystem();
            else
                EnableSystem();
        }
    }
    private void EnableMachines()
    {
        if (machine != null)
        {
            for (int i = 0; i < machine.Length; i++)
                machine[i].Enable();
        }
    }
    private void DisableMachines()
    {
        if (machine != null)
        {
            for (int i = 0; i < machine.Length; i++)
                machine[i].Disable();
        }


    }
    private void DisableMaterial()
    {
        if (plateRenderer != null && disablePlate != null)
            plateRenderer.material = disablePlate;
        
        if (cables != null && cables.Length > 0)
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


    private void EnableMaterial()
    {
        if (plateRenderer != null && enablePlate != null)
            plateRenderer.material = enablePlate;

        if (cables.Length > 0)
        {
            for (int i = 0; i < cables.Length; i++)
            {
                if (cables[i] != null && disableCable != null)
                    cables[i].material = enableCable;
                else
                    Debug.LogWarning("Cable material or array element " + i + " is null. Bind something to the array or delete.");
            }
        }
    }

    private void EnableSystem()
    {
        EnableMachines();
        EnableMaterial();
    }
    private void DisableSystem()
    {
        DisableMachines();
        DisableMaterial();
    }
    #endregion
}
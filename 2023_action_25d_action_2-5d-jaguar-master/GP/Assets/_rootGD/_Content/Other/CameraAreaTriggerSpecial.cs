using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraAreaTriggerSpecial : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cinemachineVirtualCamera.Priority = 15;
        }
       
    }

}
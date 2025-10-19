using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraAreaTrigger : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [SerializeField] private bool _lookAtPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cinemachineVirtualCamera.Priority = 15;
            if (_lookAtPlayer)
            {
                _cinemachineVirtualCamera.LookAt = other.transform;
            }
            if (isSpecialEnd)
            {
                _cinemachineVirtualCamera.Follow = other.transform;
            }
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (isSpecialEnd) { return; }

        if (other.CompareTag("Player"))
        {
            _cinemachineVirtualCamera.Priority = 5;
        }
    }


    public bool isSpecialEnd;

}
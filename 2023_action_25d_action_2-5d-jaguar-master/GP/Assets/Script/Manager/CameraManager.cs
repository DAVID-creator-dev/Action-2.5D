using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    #region Structure
    public class CameraData
    {
        public CinemachineVirtualCamera cameras;
        public int priority; 

        public CameraData(CinemachineVirtualCamera virtualCameras, int virtualPriority)
        {
            cameras = virtualCameras;
            priority = virtualPriority; 
        }
    }
    #endregion

    #region Script Parameters
    public List<CameraData> virtualCameras = new List<CameraData>(); 
    #endregion

    #region Unity Methods
    private void Start()
    {
        CinemachineVirtualCamera[] obj = GameObject.FindObjectsOfType<CinemachineVirtualCamera>();

        foreach (CinemachineVirtualCamera virtualCamera in obj)
        {
            virtualCameras.Add(new(virtualCamera, virtualCamera.Priority)); 
        }
    }
    #endregion

    #region Methods
    public void ResetPriority()
    {
        if (virtualCameras.Count > 0)
        {
            foreach (CameraData obj in virtualCameras)
            {
                obj.cameras.Priority = obj.priority;
            }
        }
    }
    #endregion
}

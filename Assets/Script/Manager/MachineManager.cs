using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Machine : MonoBehaviour
{
    public GameObject machineObject;
    [HideInInspector]public Vector3 targetPosition;
    [HideInInspector]public Vector3 initialPosition;
    public int activeGenerator;
    public int requiredGeneratorsCount;
    public float speed;
    public virtual void Enable()
    {
        ++activeGenerator;
        CheckGenerator();
    }
    public virtual void Disable()
    {
        --activeGenerator;
        CheckGenerator();
    }
    public virtual void InitMachine() { }
    public virtual void MachineBehaviour() { }
    public virtual void CheckGenerator() { }
}
public class MachineManager : MonoBehaviour
{
    public Machine[] machine;
    void Awake()
    {
        if (machine.Length != 0)
        {
            for (int i = 0; i < machine.Length; i++)
                machine[i].InitMachine();
        }
    }

    private void Update()
    {
        if (machine.Length != 0)
        {
            for (int i = 0; i < machine.Length; i++)
                machine[i].MachineBehaviour();
        }
    }
}
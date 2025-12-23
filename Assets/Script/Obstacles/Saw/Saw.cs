using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : Machine
{
    public PlayerController player;
    public GameObject limit1;
    public GameObject limit2;
    private Vector3 dir;
    public override void InitMachine()
    {
        dir = limit2.transform.position - limit1.transform.position; 
    }
    public override void MachineBehaviour()
    {
        machineObject.transform.position += dir * Time.deltaTime;
        if (CheckDistance(limit1) < 0.5f || CheckDistance(limit2) < 0.5f)
            dir = -dir;
    }
    private float CheckDistance(GameObject limit)
    {
        return Vector3.Distance(machineObject.transform.position, limit.transform.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
            player.DropGenerator(-player.transform.forward);
    }
    public override void CheckGenerator()
    {
        if (activeGenerator == requiredGeneratorsCount)
            dir = limit2.transform.position - limit1.transform.position;
        else
            dir = Vector3.zero;
    }
}

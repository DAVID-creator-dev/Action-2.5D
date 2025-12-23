using UnityEngine;

public class Piston : Machine
{
    #region Script Parameters
    public PlayerController player;
    public float ejectForce = 10f;
    private bool isElevating = false;
    private Vector3 ejectDir;
    public bool horizontal;
    public float offset = 10.0f;
    public float frequency = 5.0f;
    private float timer = 0.0f;
    private float Gravity = -9.81f; 
    #endregion

    #region Methods
    public override void InitMachine()
    {
        initialPosition = machineObject.transform.position;
        SetAnimDir();
        SetEjectDir();
    }
    private void SetAnimDir()
    {
        if (horizontal)
            targetPosition = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z + offset);
        else
            targetPosition = new Vector3(initialPosition.x, offset, initialPosition.z);
    }
    private void SetEjectDir()
    {
        if (horizontal)
            ejectDir = Vector3.forward;
        else
            ejectDir = Vector3.up;
    }
    private void OnTriggerStay(Collider other)
    {
        if (isElevating && other.gameObject.CompareTag("Player"))
        {
            float verticalVelocity = Mathf.Sqrt(ejectForce * -2 * Gravity);

            GameManager.Get.playerController.externalDirectionInfluence = new Vector3(0, verticalVelocity, 0);
        }
    }
    private void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.CompareTag("Player"))
            player.health = 0;
    }
    public void SetElevating(bool state)
    {
        isElevating = state;
    }
    private void PistonAnimation()
    {
        timer += Time.deltaTime;
        if (timer >= frequency)
        {
            machineObject.transform.position = Vector3.Lerp(machineObject.transform.position, targetPosition, 0.05f);
            timer = 0.0f;
        }
        else
        {
            machineObject.transform.position = Vector3.Lerp(machineObject.transform.position, initialPosition, 0.01f);
            if (timer > frequency * 0.99)
            {
                SetElevating(true);
            }
            else
            {
                SetElevating(false);
            }
        }
    }
    public override void MachineBehaviour()
    {
        if (activeGenerator == requiredGeneratorsCount)
            PistonAnimation();
    }
    #endregion
}

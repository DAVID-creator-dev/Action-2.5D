using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElevatorBehavior : Machine
{
    #region Script Parameters
    public bool loop;
    public float delay;
    public AudioSource audioSource; 

    public List<GameObject> points = new List<GameObject>();
    private int currentIndex = 0;
    private float timer;
    private bool hasPlaySound = true; 

    bool isTimerOn = false;
    bool orientation;
    bool playerDetected; 

    Bounds bounds;
    #endregion

    #region Methods
    private void Start()
    {
        if (GameManager.Get != null && GameManager.Get.playerController != null)
        {
            Renderer renderer = GameManager.Get.playerController.GetComponent<Renderer>();
            if (renderer != null)
            {
                bounds = renderer.bounds;
            }
            else
            {
                Debug.LogError("Renderer component not found on player controller.");
            }
        }
        else
        {
            Debug.LogError("GameManager or playerController is null.");
        }
    }

    public override void MachineBehaviour()
    {
        if (WaitForPlayerEnter && !PlayerIsInside) { return; }

        if (isTimerOn)
        {
            if (timer < delay)
                timer += Time.deltaTime;
            else
            {
                timer = 0.0f;
                isTimerOn = false;
            }
        }

        StopElevator();

        if (activeGenerator == requiredGeneratorsCount && !isTimerOn)
        {
            if (hasPlaySound)
            {
                PlaySound("Elevator_Ongoing", true);
                hasPlaySound = false;
            }
            MovePlatform();
        }
        else
        {
            if (!hasPlaySound)
            {
                PlaySound("Stop", false);
                hasPlaySound = true;
            }
        }
    }

    private void MovePlatform()
    {
        if (points.Count != 0)
        {
            Vector3 targetPosition = points[currentIndex].transform.position;

            Vector3 direction = targetPosition - machineObject.transform.position;
            float distance = direction.magnitude;

            Vector3 moveVector = direction.normalized * Mathf.Min(speed * Time.deltaTime, distance);
            machineObject.GetComponent<Rigidbody>().AddForce(moveVector);
            machineObject.transform.position += moveVector;

            orientation = direction.y > 0;

            if (distance < 0.1f)
            {
                if (currentIndex == 0)
                    PlaySound("Elevator_Touch_Ground", false);
                else
                    PlaySound("Elevator_Ding", false);

                isTimerOn = true;
                currentIndex++;

                if (currentIndex >= points.Count)
                {
                    if (loop)
                    {
                        currentIndex = 0;
                    }
                    else
                    {
                        Disable();
                        return;
                    }
                }
            }
        }
    }

    private void StopElevator()
    {
        Vector3 initialPosition = transform.position;
        RaycastHit hit;

        if (!orientation)
        {
            if (Physics.Raycast(initialPosition, Vector3.down, out hit, 0.2f))
            {
                if (hit.transform.gameObject.CompareTag("BackGround"))
                {
                    Debug.Log("Do nothing"); 
                }
                else
                {
                    currentIndex = 1;
                    orientation = false;
                }
            }

            Debug.DrawRay(initialPosition, Vector3.down * 0.2f, Color.red);
        }
        else
        {
            playerDetected = false;

            if (Physics.Raycast(initialPosition, Vector3.up, out hit, 0.2f))
            {
                if (hit.transform.gameObject.CompareTag("Player"))
                {
                    initialPosition += new Vector3(0.0f, bounds.size.y, 0.0f);
                    if (Physics.Raycast(initialPosition, Vector3.up, out hit, 1))
                    {
                        playerDetected = true;
                    }
                }
            }

            if (playerDetected)
            {
                if (hit.transform.gameObject.CompareTag("BackGround"))
                {
                    Debug.Log("Do nothing");
                }
                else
                {
                    currentIndex = 0;
                    orientation = true;
                }
            }
            else 
            {
                if (Physics.Raycast(initialPosition, Vector3.up, out hit, 0.2f))
                {
                    if (hit.transform.gameObject.CompareTag("Generator") || hit.transform.gameObject.CompareTag("BackGround"))
                    {
                        Debug.Log("Do nothing");
                    }
                    else
                    {
                        currentIndex = 0;
                        orientation = true;
                    }
                }
            }

            Debug.DrawRay(initialPosition, Vector3.up * 0.2f, Color.red);
        }
    }

    private void PlaySound(string name, bool loop)
    {
        GameManager.Get.OnSFXMachinesUpdate.Invoke(name, audioSource, loop); 
    }

    public bool WaitForPlayerEnter;
    private bool PlayerIsInside;
    private void OnTriggerEnter(Collider other)
    {
        if (WaitForPlayerEnter && other.transform.CompareTag("Player"))
        {
            PlayerIsInside = true;
            activeGenerator = requiredGeneratorsCount;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (WaitForPlayerEnter && other.transform.CompareTag("Player"))
        {
            PlayerIsInside = false;
            activeGenerator = 0;
        }
    }
    #endregion
}
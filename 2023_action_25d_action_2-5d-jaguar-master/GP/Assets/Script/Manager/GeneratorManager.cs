using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class GeneratorManager : MonoBehaviour
{
    #region Structure
    public class GeneratorData
    {
        public GameObject generator;
        public Vector3 startPosition;
        public Transform parent;
        public Rigidbody rigidbody;

        public GeneratorData(GameObject generator, Vector3 startPosition, Transform parent, Rigidbody rigidbody)
        {
            this.generator = generator;
            this.startPosition = startPosition;
            this.parent = parent; 
            this.rigidbody = rigidbody;
        }
    }

    #endregion

    #region Script Parameters
    public List<GeneratorData> generators = new List<GeneratorData>();
    private PlayerController player; 
    float test; 
    #endregion

    #region Unity Methods
    private void Start()
    {
        player = GameManager.Get.playerController; 
        
        GameObject[] generatorObjects = GameObject.FindGameObjectsWithTag("Generator");

        foreach (GameObject generatorObject in generatorObjects)
        {
            Vector3 startPosition = generatorObject.transform.position;
            Transform parentTransform = generatorObject.transform.parent;
            Rigidbody rigidbody = generatorObject.gameObject.GetComponent<Rigidbody>();


            if (parentTransform != null && parentTransform.CompareTag("Player"))
            {
                generators.Add(new GeneratorData(generatorObject, startPosition, parentTransform, rigidbody));
            }
            else
            {
                generators.Add(new GeneratorData(generatorObject, startPosition, null, rigidbody));
            }
        }
    }

    #endregion

    #region Methods
    public void ResetPosition()
    {
        if (generators.Count > 0)
        {
            foreach (GeneratorData obj in generators)
            {
                obj.generator.transform.parent = null;
                obj.generator.transform.position = obj.startPosition;
                obj.generator.GetComponent<Rigidbody>().isKinematic = false;

                if (player != null)
                {
                    if (obj.parent != null)
                    {
                        player.generator = obj.generator;
                        player.rigidbodyGen = obj.rigidbody;
                        obj.generator.transform.position = player.generatorEmplacement.transform.position;
                        obj.rigidbody.isKinematic = true;
                        obj.generator.transform.parent = obj.parent;
                    }
                }
                else
                    Debug.LogWarning("Missing Player"); 
            }
        }

    }

    #endregion
}
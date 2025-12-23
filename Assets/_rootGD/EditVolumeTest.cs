
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class EditVolumeTest : MonoBehaviour
{
    // Start is called before the first frame update

    public Volume volume;
    public MinFloatParameter minFloatParameter;
    private PhysicallyBasedSky sky;
    private VolumeProfile profile;

    void Start()
    {
        profile = volume.sharedProfile;
        if (!profile.TryGet<PhysicallyBasedSky>(out var sky))
        {
            sky = profile.Add<PhysicallyBasedSky>(false); // null
        }
        Debug.Log(sky.name);
    }

    // Update is called once per frame
    void Update()
    {
        sky.spaceEmissionMultiplier = minFloatParameter;
    }
}
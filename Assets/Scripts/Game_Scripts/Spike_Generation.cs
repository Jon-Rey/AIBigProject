using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike_Generation : MonoBehaviour
{
    //The object we want to spawn
    public GameObject SpikeObject;

    public GameObject MaxSpawnLocObj;
    public GameObject MinSpawnLocObj;
    public float MinSpacingBetween;
    public float MaxSpacingBetween;
    public int AmountofSpikes;

    private Vector3 MinSpawnLocation;
    private Vector3 MaxSpawnLocation;
    // Start is called before the first frame update
    void Start()
    {
        //get the min and max vectors of where to spawn the spikes
        MinSpawnLocation = MinSpawnLocObj.transform.position;
        MaxSpawnLocation = MaxSpawnLocObj.transform.position;
        //the location the spike obj will spawn at
        Vector3 SpawnLoc = MinSpawnLocation;
        //do this for how ever many spikes we want
        for (int i =0; i < AmountofSpikes; i++)
        {
            int NumberSpikes = 0;
            //if our spikes z position value goes past the max then we spawn one more spike and then stop spawning them
            if (SpawnLoc.z > MaxSpawnLocation.z)
            {
                SpawnLoc.z = MaxSpawnLocation.z;
                Instantiate(SpikeObject, SpawnLoc, SpikeObject.transform.rotation);
                break;
            }
            NumberSpikes = Random.Range(1, 3);// get either 1 or 2
            //if we are spawning two spikes together then we spawn the first one andd the offset fo the second one
            //else we just spawn one spike at the location
            if (NumberSpikes == 2)
            {
                Instantiate(SpikeObject, SpawnLoc, SpikeObject.transform.rotation);//spawn the spike obj at certain location and rotation
                SpawnLoc.z += 1.0f;
            }
            Instantiate(SpikeObject, SpawnLoc, SpikeObject.transform.rotation);//spawn the spike obj at certain location and rotation
            SpawnLoc.z += Random.Range(MinSpacingBetween, MaxSpacingBetween);//offset the next spike location by a random amount
        }
    }
}

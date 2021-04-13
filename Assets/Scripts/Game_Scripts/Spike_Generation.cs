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
    public float AmountofSpikes;

    private Vector3 MinSpawnLocation;
    private Vector3 MaxSpawnLocation;
    // Start is called before the first frame update
    void Start()
    {
        MinSpawnLocation = MinSpawnLocObj.transform.position;
        MaxSpawnLocation = MaxSpawnLocObj.transform.position;
        Vector3 SpawnLoc = MinSpawnLocation;
        for (int i =0; i < AmountofSpikes; i++)
        {
            if (SpawnLoc.z > MaxSpawnLocation.z)
            {
                SpawnLoc.z = MaxSpawnLocation.z;
                Instantiate(SpikeObject, SpawnLoc, SpikeObject.transform.rotation);
                break;
            }
            Instantiate(SpikeObject, SpawnLoc, SpikeObject.transform.rotation);
            SpawnLoc.z += Random.Range(MinSpacingBetween, MaxSpacingBetween);
        }
    }
}

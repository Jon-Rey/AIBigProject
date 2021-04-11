using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpossibleAIManager : MonoBehaviour
{

    public int populationSize = 10;
    public Transform SpawnPoint;
    public GameObject playerPrefab;
    public GameObject Camera; 
    [HideInInspector]
    public Dictionary<GameObject, PlayerAI> population = new Dictionary<GameObject, PlayerAI>();
    

    /// <summary>
    /// Generate population
    /// </summary>
    public void Start()
    {
        GeneratePopulation();
    }

    public void GeneratePopulation()
    {
        for(int i = 0; i < populationSize; i++)
        {
            GameObject new_player = Instantiate(playerPrefab, SpawnPoint);
            new_player.GetComponent<PlayerScript>().Spawn = SpawnPoint.gameObject;
            PlayerAI playerAI = new_player.GetComponent<PlayerAI>();
            playerAI.StartPlayerAI(new List<float>());
            population[new_player] = playerAI;
        }

        foreach(var child in population)
        {
            Collider child_coll1 = child.Key.GetComponent<Collider>();
            foreach(var child2 in population)
            {
                if(child.Key != child2.Key)
                {
                    Physics.IgnoreCollision(child_coll1, child2.Key.GetComponent<Collider>());
                }
            }
        }
    }
}

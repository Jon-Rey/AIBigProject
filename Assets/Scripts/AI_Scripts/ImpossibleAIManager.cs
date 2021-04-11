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

    public void Update()
    {
        SetCameraOnFarthestChild();
    }

    public void SetCameraOnFarthestChild()
    {
        KeyValuePair<GameObject, PlayerAI> farthest_child = new KeyValuePair<GameObject, PlayerAI>(null, null);
        foreach (var child in population)
        {
            if (farthest_child.Key == null)
            {
                farthest_child = child;
            }
            else if (Mathf.Abs(child.Key.transform.position.x) > Mathf.Abs(farthest_child.Key.transform.position.x))
            {
                farthest_child = child;
            }
        }
        Camera.transform.SetParent(farthest_child.Key.transform);
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

    }

    //TODO: Crossover function

    //TODO: mutation function

    //TODO: fitness function

    //TODO: survival selection
}

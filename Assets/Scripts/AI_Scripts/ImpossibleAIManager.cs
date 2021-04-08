using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpossibleAIManager : MonoBehaviour
{
    public int populationSize = 10;
    public GameObject playerPrefab;
    [HideInInspector]
    public List<PlayerAI> population = new List<PlayerAI>();

    /// <summary>
    /// Generate population
    /// </summary>
    public void Start()
    {
        
    }
}

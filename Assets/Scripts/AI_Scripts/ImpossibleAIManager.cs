using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ImpossibleAIManager : MonoBehaviour
{

    public int populationSize = 10;
    public Transform SpawnPoint;
    public GameObject playerPrefab;
    public GameObject Camera;
    public float MutationRate = 0.01f;

    private bool IsFirstRun = true;     // TODO: implement the 1st run
    [SerializeField]
    private int Generation;
    private List<PlayerAI> Population = new List<PlayerAI>();
    
    private System.Random random;

    private PlayerAI BestSoFar = null;

    /// <summary>
    /// Generate population
    /// </summary>
    public void Start()
    {
        Generation = 1;
        random = new System.Random();
        GeneratePopulation();
    }

    private void GeneratePopulation()
    {
        if (!IsFirstRun)
        {
            for (int i = 0; i < populationSize; i++)
            {
                // make a new player ai with a randomized chromosome
                var randomChromosome = new List<int>(BestSoFar.ChromosomeLength);
                for (int j = 0; j < BestSoFar.ChromosomeLength; j++)
                {
                    randomChromosome.Add(random.NextDouble() < 0.5 ? 0 : 1); 
                }
                
                PlayerAI playerAI = MakeNewPlayerAI();
                playerAI.StartPlayerAI(randomChromosome);
                Population.Add(playerAI);
            }
        }
        else
        {
            BestSoFar = MakeNewPlayerAI();
            BestSoFar.StartPlayerAI_testRun();
        }
    }

    /// <summary>
    /// Select the best individuals in the current generation as parents. These will be used to produce offspring. 
    /// </summary>
    /// <returns>
    /// A list of PlayerAI objects (parents) that can be used for reproducing.
    /// </returns>
    List<PlayerAI> SelectMatingPool()
    {
        var matingPool = new List<PlayerAI>();
        List<PlayerAI> sortedList = Population.OrderByDescending(p => p.fitness).ToList();
        // top half of the population gets to reproduce
        for (int i = 0; i < Mathf.FloorToInt(sortedList.Count / 2.0f); i++)
        {
            matingPool.Add(sortedList[i]);
        }
        return matingPool;
    }


    public float Fitness(PlayerAI child)
    {
        float return_fit = 0;

        //one distance factor playing into the solution
        return_fit += child.ChromosomeLength / BestSoFar.ChromosomeLength;

        return return_fit;
    }

    /// <summary>
    /// Randomly selects 2 parents from the mating pool, crosses them over using
    /// Uniform crossover and returns a list of resulting 2 children.
    /// </summary>
    /// <returns></returns>
    List<PlayerAI> Crossover()
    {
        var allParents = SelectMatingPool();
        var children = new List<PlayerAI>()
        {
            MakeNewPlayerAI(), MakeNewPlayerAI()
        };

        // randomly select 2 parents to cross 
        var idxToRemove = Random.Range(0, allParents.Count);
        var p1 = allParents[idxToRemove];
        allParents.RemoveAt(idxToRemove);
        
        idxToRemove = Random.Range(0, allParents.Count);
        var p2 = allParents[idxToRemove];
        allParents.RemoveAt(idxToRemove);

        foreach (PlayerAI child in children)
        {
            for (int i = 0; i < p1.Chromosome.Count; i++)
            {
                child.Chromosome[i] = random.NextDouble() < 0.5 ? p1.Chromosome[i] : p2.Chromosome[i];
            }
        }
        return children;
    }

    PlayerAI MakeNewPlayerAI()
    {
        GameObject new_player = Instantiate(playerPrefab, SpawnPoint);
        new_player.GetComponent<PlayerScript>().Spawn = SpawnPoint.gameObject;
        return new_player.GetComponent<PlayerAI>();
    }
    
    public void Update()
    {
        // SetCameraOnFarthestChild();
        CheckChildrenState();
    }

    void CheckChildrenState()
    {
        if (IsFirstRun && BestSoFar)
        {
            if (BestSoFar.currState == PlayerAI.STATE.FINISH)
            {
                IsFirstRun = false;
                BestSoFar.gameObject.SetActive(false);
                GeneratePopulation();
            }
        }
        
        int dead = 0;
        foreach(var child in Population)
        {
            if(child.currState == PlayerAI.STATE.DEAD)
            {
                dead += 1;
            }
            else if (child.currState == PlayerAI.STATE.FINISH)
            {
                BestSoFar = child;
                break;
            }
        }
        if (BestSoFar == null)
        {
            if (dead == populationSize - 1)
            {
                //Survival selection!
            }
        }
        else
        {
            //solution achieved, stop simulation and print out successful child or let successful child continue running the course alone till done. 
        }
    }

    public void SetCameraOnFarthestChild()
    {
        PlayerAI farthest_child = null;
        foreach (var child in Population)
        {
            if (farthest_child == null)
            {
                farthest_child = child;
            }
            else if (Mathf.Abs(child.transform.position.x) > Mathf.Abs(farthest_child.transform.position.x))
            {
                farthest_child = child;
            }
        }
        Camera.transform.SetParent(farthest_child.transform);
    }

    
    //TODO: mutation function
    public List<int> Mutation(List<int> gene, int jumpFrames)
    {
        List<int> copygene = gene;
        List<int> changes = new List<int>();
        int randinter = Random.Range(0,gene.Count);//gets amount of elements in the list we are changing
        int i = 0;
        while (i < randinter)
        {
            int change = Random.Range(0,gene.Count-1);
            if (!changes.Contains(change))
            {
                changes.Add(change);
                i += 1;
            }
        }
        foreach(int c in changes)
        {
            if (copygene[c] == 0)
            {
                copygene[c] = 1;
            }
            else
            {
                copygene[c] = 0;
            }
        }
        return copygene;
    }

    //TODO: fitness function
    //TODO: survival selection
}

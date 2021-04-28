using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public class ImpossibleAIManager : MonoBehaviour
{

    public int populationSize = 10;
    public Transform SpawnPoint;
    public GameObject playerPrefab;
    public GameObject Camera;
    public float MutationRate = 0.1f;

    private bool IsFirstRun = true;     // TODO: implement the 1st run
    [SerializeField]
    private int Generation;
    private List<PlayerAI> Population = new List<PlayerAI>();
    private List<PlayerAI> SortedPopulation;
    private bool AllChildrenDead = false;
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
            BestSoFar.ChromosomeLength = BestSoFar.CurrentChromosomeIndex;
            Debug.Log($"Chromosome Length={BestSoFar.ChromosomeLength}");
            for (int i = 0; i < populationSize; i++)
            {
                PlayerAI playerAI = MakeNewPlayerAI();
                Population.Add(playerAI);
            }
            SortedPopulation = Population.OrderByDescending(p => p.fitness).ToList();
            foreach (var pop in Population)
            {
                pop.StartPlayerAI();
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
        //SortedPopulation = Population.OrderByDescending(p => p.fitness).ToList();
        // top half of the population gets to reproduce
        for (int i = 0; i < Mathf.FloorToInt(SortedPopulation.Count / 2.0f); i++)
        {
            matingPool.Add(SortedPopulation[i]);
        }
        return matingPool;
    }

    /// <summary>
    /// Given a pool of Player AI's, selects k individuals from the pool at random.
    /// Performs tournament selection between those random individuals and returns a
    /// pair of Player AI parents.
    /// </summary>
    /// <param name="pool"></param>
    /// <param name="k"></param>
    /// <returns></returns>
    PlayerAI[] TournamentSelection(List<PlayerAI> pool, int k)
    {
        List<PlayerAI> temp = new List<PlayerAI>();
        PlayerAI[] parentPair = new PlayerAI[2];
        for (int i = 0; i < 2; i++)
        {
            temp.Clear();
            for (int j = 0; j < k; j++)
            {
                temp.Add(pool[random.Next(pool.Count)]);
            }
            // tournament selection part: 
            temp = temp.OrderByDescending(p => p.fitness).ToList();
            parentPair[i] = temp[0];
        }
        return parentPair;
    }


    public float Fitness(PlayerAI child)
    {
        float return_fit = 0;

        //one distance factor playing into the solution
        return_fit += (float)child.ChromosomeLength / BestSoFar.ChromosomeLength;

        return return_fit;
    }
    
    public float Fitness(List<int> chromo)
    {
        float return_fit = 0;

        //one distance factor playing into the solution
        return_fit += (float)chromo.Count / BestSoFar.ChromosomeLength;

        return return_fit;
    }

    /// <summary>
    /// Randomly selects 2 parents from the mating pool, crosses them over using
    /// Uniform crossover and returns a list of resulting 2 children.
    /// </summary>
    /// <returns></returns>
    PlayerAI[] Crossover(PlayerAI[] parents)
    {
        if (parents.Length != 2)
            throw new IndexOutOfRangeException("Need 2 parents to make them babies ;)");

        var children = new PlayerAI[2]
        {
            MakeNewPlayerAI(), MakeNewPlayerAI()
        };

        for (int i = 0; i < children.Length; i++)
        {
            for (int j = 0; j < children[i].Chromosome.Count; j++)
            {
                children[i].Chromosome[j] =
                    random.NextDouble() < 0.5 ? parents[0].Chromosome[i] : parents[1].Chromosome[i];
            }
        }
        return children;
    }

    PlayerAI MakeNewPlayerAI()
    {
        if (!IsFirstRun)
        {
            // make a new player ai with a randomized chromosome
            var randomChromosome = new List<int>(BestSoFar.ChromosomeLength);
            for (int j = 0; j < BestSoFar.ChromosomeLength; j++)
            {
                randomChromosome.Add(random.NextDouble() < 0.5 ? 0 : 1);
            }
            GameObject new_player = Instantiate(playerPrefab, SpawnPoint);
            PlayerAI playerAI = new_player.GetComponent<PlayerAI>();
            playerAI.fitness = Fitness(randomChromosome);
            playerAI.Chromosome = randomChromosome;
            new_player.GetComponent<PlayerScript>().Spawn = SpawnPoint.gameObject;
            return playerAI;
        }
        else
        {
            GameObject new_player = Instantiate(playerPrefab, SpawnPoint);
            PlayerAI playerAI = new_player.GetComponent<PlayerAI>();
            new_player.GetComponent<PlayerScript>().Spawn = SpawnPoint.gameObject;
            return playerAI;
        }
    }
    
    public void Update()
    {
        CheckChildrenState();
    }

    void CheckChildrenState()
    {
        if (!IsFirstRun)
        {
            foreach (var child in Population)
            {
                if(child.currState == PlayerAI.STATE.ACTIVE)
                {
                    AllChildrenDead = false;
                    break;
                }
                else
                {
                    AllChildrenDead = true;
                }
            }
            
            if (AllChildrenDead)
            {
                Debug.Log("all dead");
                SurvivalSelection();
            }
            else
            {
                Debug.Log("found best solution");
            }
        }
        else
        {
            if (BestSoFar.currState != PlayerAI.STATE.FINISH) return;
            IsFirstRun = false;
            BestSoFar.gameObject.SetActive(false);
            Debug.Log("gen pop");
            GeneratePopulation();
        }
    }

    void SurvivalSelection()
    {
        //Survival selection!
        Generation += 1;
                
        // n = calc how many children to remove from pop (always even)
        // remove n worst members of the pop
        // let's go with quarter pop removal
        var quarterPop = SortedPopulation.Count / 4;
        SortedPopulation.RemoveRange(quarterPop * 3, quarterPop);
        Population = SortedPopulation;

        List<PlayerAI> tempChildren = new List<PlayerAI>();
        // select parents from the remaining pool and make n children
        for (int i = 0; i < quarterPop / 2; i++)
        {
            PlayerAI[] parents = TournamentSelection(SortedPopulation, SortedPopulation.Count / 2);
            PlayerAI[] children = Crossover(parents);

            foreach (var child in children)
            {
                // should we mutate?
                if (random.NextDouble() <= MutationRate)
                {
                    child.Chromosome = Mutation(child.Chromosome);
                }
                child.fitness = Fitness(child);
                        
                // removed children get replaced with the new children
                tempChildren.Add(child);
            }
        }

        foreach(var child in tempChildren)
        {
            Population.Add(child);
        }
        Debug.Log("survival selected");
    }
    
    List<int> Mutation(List<int> gene)
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
}

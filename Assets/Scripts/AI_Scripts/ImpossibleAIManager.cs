using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ImpossibleAIManager : MonoBehaviour
{
    public enum GAMESTATE
    {
        SETUP,
        FIRSTRUN,
        RUNNING,
        EVOLVING
    }
    public GAMESTATE GameState = GAMESTATE.FIRSTRUN;
    
    public int populationSize = 10;
    public Transform SpawnPoint;
    public GameObject playerPrefab;
    public float MutationRate = 0.1f;
    public int Generation { get; private set; }
    
    private List<PlayerAI> Population = new List<PlayerAI>();
    private List<PlayerAI> DestroyPop = new List<PlayerAI>();
    private bool AllChildrenDead = false;
    private PlayerAI BestSoFar = null;
    private System.Random random;


    /// <summary>
    /// Generate population
    /// </summary>
    public void Start()
    {
        Generation = 1;
        random = new System.Random();
        DoTestRun();
    }

    public void Update()
    {
        if (GameState == GAMESTATE.FIRSTRUN)
            CheckTestRun();
        else if (GameState == GAMESTATE.RUNNING)
            CheckChildrenState();
    }


    void CheckTestRun()
    {
        if (BestSoFar)
        {
            if (BestSoFar.currState == PlayerAI.STATE.FINISH)
            {
                BestSoFar.gameObject.SetActive(false);
                GameState = GAMESTATE.SETUP;
                GeneratePopulation();
            }
        }
    }


    void CheckChildrenState()
    {
        foreach (var child in Population)
        {
            switch (child.currState)
            {
                case PlayerAI.STATE.DEAD:
                    AllChildrenDead = true;
                    break;
                case PlayerAI.STATE.ACTIVE:
                    AllChildrenDead = false;
                    break;
                case PlayerAI.STATE.INACTIVE:
                    AllChildrenDead = false;
                    break;
                case PlayerAI.STATE.FINISH:
                    AllChildrenDead = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (AllChildrenDead == false)
            {
                break;
            }
        }
        
        if (AllChildrenDead)
        {
            Debug.Log("all dead");
            GameState = GAMESTATE.EVOLVING;
            AllChildrenDead = false;
            SurvivalSelection();

        }
    }

    #region GEN_FUNCS

    private void GeneratePopulation()
    {
        BestSoFar.ChromosomeLength = BestSoFar.CurrentChromosomeIndex;
        Debug.Log($"Chromosome Length={BestSoFar.ChromosomeLength}");
        for (int i = 0; i < populationSize; i++)
        {
            PlayerAI playerAI = MakeNewPlayerAI(RandChromo());
            Population.Add(playerAI);
        }
        Population = Population.OrderByDescending(p => p.fitness).ToList();

        GameState = GAMESTATE.RUNNING;

        foreach (var pop in Population)
        {
            pop.StartPlayerAI();
        }
    }

    private void DoTestRun()
    {
        GameState = GAMESTATE.FIRSTRUN;
        BestSoFar = MakeNewPlayerAI();
        BestSoFar.StartPlayerAI_testRun();
    }

    void SurvivalSelection()
    {
        ResetAllChildren();
        //Survival selection!
        Generation += 1;

        // n = calc how many children to remove from pop (always even)
        // remove n worst members of the pop
        // let's go with quarter pop removal
        var quarterPop = Population.Count / 4;
        for (int i = quarterPop*3; i < quarterPop*3+quarterPop; i++)
        {
            PlayerAI tempPlayer = null;
            tempPlayer = Population[i];
            Population.Remove(tempPlayer);
            DestroyPop.Add(tempPlayer);
        }


        List<PlayerAI> tempChildren = new List<PlayerAI>();
        // select parents from the remaining pool and make n children
        for (int i = 0; i < quarterPop / 2; i++)
        {
            PlayerAI[] parents = TournamentSelection(Population, Population.Count / 2);
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

        foreach (var child in tempChildren)
        {
            Population.Add(child);
        }
        Population = Population.OrderByDescending(p => p.fitness).ToList();

        GameState = GAMESTATE.RUNNING;
        //Done with Survival Selection
        foreach(var child in Population)
        {
            child.StartPlayerAI();
        }
    }

    public void ResetAllChildren()
    {
        foreach (var child in Population)
        {
            child.ResetAi();
        }
    }
    #endregion

    #region HELPERS

    /// <summary>
    /// Select the best individuals in the current generation as parents. These will be used to produce offspring. 
    /// </summary>
    /// <returns>
    /// A list of PlayerAI objects (parents) that can be used for reproducing.
    /// </returns>
    List<PlayerAI> SelectMatingPool()
    {
        var matingPool = new List<PlayerAI>();
        //Population = Population.OrderByDescending(p => p.fitness).ToList();
        // top half of the population gets to reproduce
        for (int i = 0; i < Mathf.FloorToInt(Population.Count / 2.0f); i++)
        {
            matingPool.Add(Population[i]);
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
            throw new IndexOutOfRangeException("Need 2 parents to make new babies ;)");

        var children = new PlayerAI[2];
        List<int> tempChromo = new List<int>(parents[0].ChromosomeLength);
        for (int i = 0; i < children.Length; i++)
        {
            tempChromo.Clear();
            for (int j = 0; j < tempChromo.Count; j++)
            {
                tempChromo[j] = 
                    random.NextDouble() < 0.5 ? parents[0].Chromosome[i] : parents[1].Chromosome[i];
            }
            // TODO: need to instantiate the children to be able to do anything like this.
            children[i] = MakeNewPlayerAI(tempChromo);

        }
        return children;
    }

    private List<int> RandChromo()
    {
        var randomChromosome = new List<int>(BestSoFar.ChromosomeLength);
        for (int j = 0; j < BestSoFar.ChromosomeLength; j++)
        {
            randomChromosome.Add(random.NextDouble() < 0.5 ? 0 : 1);
        }
        return randomChromosome;
    }

    PlayerAI MakeNewPlayerAI(List<int> Chromo)
    {
        GameObject new_player = Instantiate(playerPrefab, SpawnPoint);
        PlayerAI playerAI = new_player.GetComponent<PlayerAI>();
        playerAI.fitness = Fitness(Chromo);
        playerAI.Chromosome = Chromo;
        new_player.GetComponent<PlayerScript>().Spawn = SpawnPoint.gameObject;
        return playerAI;
    }

    PlayerAI MakeNewPlayerAI()
    {
        GameObject new_player = Instantiate(playerPrefab, SpawnPoint);
        PlayerAI playerAI = new_player.GetComponent<PlayerAI>();
        new_player.GetComponent<PlayerScript>().Spawn = SpawnPoint.gameObject;
        return playerAI;
    }


    List<int> Mutation(List<int> gene)
    {
        List<int> copygene = gene;
        List<int> changes = new List<int>();
        int randinter = Random.Range(0, gene.Count);//gets amount of elements in the list we are changing
        int i = 0;
        while (i < randinter)
        {
            int change = Random.Range(0, gene.Count - 1);
            if (!changes.Contains(change))
            {
                changes.Add(change);
                i += 1;
            }
        }
        foreach (int c in changes)
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

    void DeadChildGarbageCollection()
    {
        var quarterPop = Population.Count / 4;


        // 
    }
    #endregion
}

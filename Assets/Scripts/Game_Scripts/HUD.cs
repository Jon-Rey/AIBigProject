using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public ImpossibleAIManager Manager;
    public Text GenerationText;
    public Text PopulationSizeText;
    public Text MutationRateText;
    
    // Update is called once per frame
    void Update()
    {
        if (Manager.GameState == ImpossibleAIManager.GAMESTATE.FIRSTRUN)
        {
            GenerationText.text = "Current Generation: TEST RUN";
            PopulationSizeText.text = "Population Size: 0";
            MutationRateText.text = "Mutation Rate: 0%";
        }
        else
        {
            GenerationText.text = $"Current Generation: {Manager.Generation}";
            PopulationSizeText.text = $"Population Size: {Manager.populationSize}";
            MutationRateText.text = $"Mutation Rate: {Manager.MutationRate * 100}%";
        }
    }
}

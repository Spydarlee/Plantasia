using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public static TutorialManager   Instance = null;
    public List<string>             HintLocIds = new List<string>();
    public string                   NoMoreHintsLocId = "Hint_00";
    private List<int>               mUsedHintIds = new List<int>();

    // -------------------------------------------------------------------------------

    public Plant FirstPlant { get; set; }
    public TutorialStages CurrentStage
    {
        get { return mCurrentStage; }
        set
        {
            mCurrentStage = value;
            SaveGameManager.Instance.UpdateTutorialStage(mCurrentStage);
        }
    }

    // -------------------------------------------------------------------------------

    private TutorialStages  mCurrentStage = TutorialStages.NewGame;
    private UIManager       mUIManager = null;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;
    }

    // -------------------------------------------------------------------------------

    private void Start()
    {
        mUIManager = UIManager.Instance;

        Debug.Log("TutorialManager Start: " + mCurrentStage);

        var saveGameManager = SaveGameManager.Instance;
        if (saveGameManager.IsLoaded)
        {
            Initialise(saveGameManager.SaveGameData);
            Debug.Log("TutorialManager Start (saveGameManager.IsLoaded): " + mCurrentStage);
        }
        else
        {
            SaveGameManager.OnSaveGameLoaded += Initialise;
        }
    }

    // -------------------------------------------------------------------------------

    private void Initialise(GameData gameData)
    {
        mCurrentStage = gameData.TutorialStage;
        Debug.Log("TutorialManager Start (Initialise): " + mCurrentStage);

        var homePlanetoidPlants = Universe.Instance.CurrentHomePlanetoid.Plants;
        FirstPlant = (homePlanetoidPlants != null && homePlanetoidPlants.Count > 0) ? homePlanetoidPlants[0] : FirstPlant;
    }

    // -------------------------------------------------------------------------------

    public void SpawnFirstPlant()
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(new Vector3(0f, 10.0f, 0f), Vector3.down, out raycastHit))
        {
            var currentHomePlanetoid = Universe.Instance.CurrentHomePlanetoid;

            FirstPlant = PlantManager.Instance.SpawnPlant(PlantTypes.Flower, raycastHit.point, currentHomePlanetoid, raycastHit.normal, true);
            Universe.Instance.CurrentPlanetoid.AddPlant(FirstPlant);
        }
        else
        {
            Debug.LogError("Ray to find First Plant spawn position failed to hit anything!");
        }
    }

    // -------------------------------------------------------------------------------

    public void ShowHint()
    {
        if (HintLocIds.Count > 0)
        {
            string hintLocId = NoMoreHintsLocId;

            if (mUsedHintIds.Count == HintLocIds.Count)
            {
                mUsedHintIds.Clear();
            }
            else
            {
                var randomHintIndex = Random.Range(0, HintLocIds.Count);
                while (mUsedHintIds.Contains(randomHintIndex))
                {
                    randomHintIndex = Random.Range(0, HintLocIds.Count);
                }

                hintLocId = HintLocIds[randomHintIndex];
                mUsedHintIds.Add(randomHintIndex);
            }

            mUIManager.MessageBoxUI.Show("", hintLocId);
        }
    }

    // -------------------------------------------------------------------------------
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActionGraph;

public class GameManager : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public static GameManager   Instance;

    // -------------------------------------------------------------------------------

    [Header("State Data")]
    public Graph                GameplayStateGraph = null;
    public GameplayStates       CurrentGameplayState = GameplayStates.Planetoid;
    public bool IsTransitioning { get { return GameplayStateGraph != null && GameplayStateGraph.IsTransitioning; } }

    [Header("Global References")]
    public SeedShip             SeedShip = null;
    public CameraController     CameraController = null;
    public Planetoid            CurrentPlanetoid { get { return Universe.CurrentPlanetoid; } set { Universe.CurrentPlanetoid = value; } }
    public PlanetoidProxy       SelectedPlanetoidProxy = null;
    public Universe             Universe = null;
    public LayerMask            XZPlaneCollisionMask;
    public Sun                  TheSun { get { return Universe.TheSun; } }
    public Orbitable            TheMoon { get { return Universe.TheMoon; } }

    [Header("Prefabs")]
    public GameObject           SeedCollectParticleEffect = null;

    // -------------------------------------------------------------------------------

    public delegate void GameStateChangedAction(GameplayStates prevState, GameplayStates newState);
    public static event GameStateChangedAction OnGameStateChanged = null;

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            Debug.LogWarning("More than one GameManager found! Destroying one...");
        }

        Instance = this;
    }

    // -------------------------------------------------------------------------------

    private void Start()
    {
        if (SeedShip == null)
        {
            SeedShip = GameObject.FindObjectOfType<SeedShip>();
        }

        if (CameraController == null)
        {
            CameraController = GameObject.FindObjectOfType<CameraController>();
        }

        if (Universe == null)
        {
            Universe = GameObject.FindObjectOfType<Universe>();
        }
    }

    // -------------------------------------------------------------------------------

    public void ChangeGameplayState(GameplayStates newGameplayState)
    {
        if (CurrentGameplayState != newGameplayState)
        {
            if (OnGameStateChanged != null)
            {
                OnGameStateChanged.Invoke(CurrentGameplayState, newGameplayState);
            }

            CurrentGameplayState = newGameplayState;
        }
    }

    // -------------------------------------------------------------------------------

    public bool IsCurrentGameplayState(GameplayStates state, bool falseifTransitioning = true)
    {
        return (!falseifTransitioning || !IsTransitioning) && CurrentGameplayState == state;
    }

    // -------------------------------------------------------------------------------
}

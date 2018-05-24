using UnityEngine;

// -------------------------------------------------------------------------------

public abstract class PlantGrowthRequirement : ScriptableObject
{
    // -------------------------------------------------------------------------------

    public Sprite UISprite = null;

    // -------------------------------------------------------------------------------

    public abstract bool    CheckRequirement(Plant plant);
    public abstract float   GetStatusAsPercentage(Plant plant);
}

// -------------------------------------------------------------------------------
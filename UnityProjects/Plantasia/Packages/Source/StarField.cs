using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarField : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    public ParticleSystem   ParticleSystem = null;

    public float            ShowDuration = 0.5f;
    public float            HideDuration = 1.5f;
    public float            MinStartSize = 0.3f;
    public float            MaxStartSize = 50.0f;

    // -------------------------------------------------------------------------------

    void Start()
    {
        if (ParticleSystem == null)
        {
            ParticleSystem = GetComponent<ParticleSystem>();
        }

        SetEnabled(false, true);
	}

    // -------------------------------------------------------------------------------

    public void SetEnabled(bool enabled, bool instant = false)
    {
        if (instant)
        {
            gameObject.SetActive(enabled);
        }
        else
        {
            gameObject.SetActive(true);

            var fromStartSize = (enabled) ? MaxStartSize : MinStartSize;
            var targetStartSize = (enabled) ? MinStartSize : MaxStartSize;
            var duration = (enabled) ? ShowDuration : HideDuration;

            LeanTween.value(fromStartSize, targetStartSize, duration).setOnUpdate((float value) =>
            {
                var main = ParticleSystem.main;
                main.startSize = value;
            }).setOnComplete(() =>
            {
                if (!enabled)
                {
                    gameObject.SetActive(false);
                }
            });
        }
    }

    // -------------------------------------------------------------------------------
}

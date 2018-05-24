using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuriedObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // -------------------------------------------------------------------------------

    public int              NumTapsToUnbury = 10;
    public GameObject       ObjectToBury = null;
    public float            BuryDepth = -1.0f;
    public float            UnburiedHeight = 1.0f;
    public ParticleSystem   HintParticles = null;

    // -------------------------------------------------------------------------------

    public float Radius { get { return mCapsuleCollider.radius; } }

    // -------------------------------------------------------------------------------

    private int             mNumTapsTapped = 0;
    private CapsuleCollider mCapsuleCollider = null;

    // -------------------------------------------------------------------------------

    protected virtual void Awake()
    {
        mCapsuleCollider = GetComponent<CapsuleCollider>();
        Debug.Assert(mCapsuleCollider != null, "BuriedObjects need capsule colliders to LIVE, man.", this);
    }

    // -------------------------------------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        // Doesn't even do anything, mate, just here so we can get OnPointerUp
    }

    // -------------------------------------------------------------------------------

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ObjectToBury != null && mNumTapsTapped < NumTapsToUnbury)
        {
            ++mNumTapsTapped;

            var t = ((float)mNumTapsTapped / (float)NumTapsToUnbury);
            var buryDepth = Mathf.Lerp(BuryDepth, UnburiedHeight, t);

            ObjectToBury.transform.position = transform.position + (transform.up * buryDepth);

            if (mNumTapsTapped == NumTapsToUnbury)
            {
                OnUnburied();
            }
        }
    }

    // -------------------------------------------------------------------------------

    public void OnEnable()
    {
        Initialise();        
    }

    // -------------------------------------------------------------------------------

    protected virtual void OnUnburied()
    {
        HintParticles.Stop();
        gameObject.SetActive(false);
    }

    // -------------------------------------------------------------------------------

    protected virtual void Initialise()
    {
        if (ObjectToBury != null)
        {
            ObjectToBury.transform.SetParent(transform);
            ObjectToBury.transform.position = transform.position + (transform.up * BuryDepth);
        }

        mNumTapsTapped = 0;
    }

    // -------------------------------------------------------------------------------
}

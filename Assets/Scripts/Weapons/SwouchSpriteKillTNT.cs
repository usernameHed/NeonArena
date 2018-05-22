using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class SwouchSpriteKillTNT : MonoBehaviour
{
    #region variable
    public GameObject overlapKill;
    public float rangeOverlap = 3;
    #endregion

    #region  initialisation
    //references
    private void Awake()
    {

    }

    //initialisation
    private void Start()
    {
		SoundManager.Instance.PlaySound (SoundManager.Instance.TNTSound);
    }
    #endregion

    #region core script
    public void activeOverlap()
    {
        if (overlapKill)
        {
            overlapKill.SetActive(true);
            overlapKill.GetComponent<OverlapDestroy>().startKill(rangeOverlap, rangeOverlap, null, 0, null);
        }
    }
    #endregion

    #region unity fonction and ending
    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public void destroyThis()
    {
		Destroy(gameObject);
    }
    #endregion
}

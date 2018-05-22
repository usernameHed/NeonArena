using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class animController : MonoBehaviour
{
    #region variable
    private Animator anim;

    private float timeToGo;

    [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)]  private float timeOpti = 0.1f;

    #endregion

    #region  initialisation
    //references
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    //initialisation
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                                   //setup le temps
    }
    #endregion

    #region core script

    /// <summary>
    /// set jump à false
    /// </summary>
    public void setJumpFalse()
    {
        anim.SetBool("Jump", false);
    }

    /// <summary>
    /// set jump à false
    /// </summary>
    public void setFloorFalse()
    {
        anim.SetBool("OnFloor", false);
    }

    /// <summary>
    /// set jump à false
    /// </summary>
    public void setWallFalse()
    {
        anim.SetBool("OnWall", false);
    }
    #endregion

    #region unity fonction and ending
    //à chaque frames
    private void Update()
    {
        if (Time.fixedTime >= timeToGo)                                         //effectué à chaque opti frame
        {

            timeToGo = Time.fixedTime + timeOpti;
        }
    }


    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public void destroyThis()
    {
        Destroy(gameObject);
    }
    #endregion
}

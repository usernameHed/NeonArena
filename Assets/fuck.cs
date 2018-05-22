using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class fuck : MonoBehaviour
{
    #region variable
    
    private float timeToGo;

    [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)]  private float timeOpti = 0.1f;


    #endregion

    #region  initialisation
    //references
    private void Awake()
    {

    }

    //initialisation
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                                   //setup le temps
        gameObject.GetComponent<Button>().Select();
    }
    #endregion

    #region core script

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

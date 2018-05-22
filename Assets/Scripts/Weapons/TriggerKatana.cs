using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class TriggerKatana : MonoBehaviour
{
    #region variable

    private float timeToGo;
    private Katana kata;

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
    }
    #endregion

    #region core script
    public void setActive(Katana katana)
    {
        kata = katana;
        gameObject.GetComponent<SphereCollider>().enabled = true;
    }
    #endregion

    #region unity fonction and ending
    //à chaque frames
    private void OnTriggerEnter(Collider other)
    {
        if (other != kata.PC.gameObject && kata.isDashing)
        {
            kata.StartHit();
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

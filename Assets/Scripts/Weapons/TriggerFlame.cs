using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class TriggerFlame : MonoBehaviour
{
    #region variable

    private float timeToGo;
    private GameObject refPlayer;
    private PlayerController PC;

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
    public void setActive(GameObject player)
    {
        refPlayer = player;
        PC = refPlayer.GetComponent<PlayerController>();
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }
    #endregion

    #region unity fonction and ending
    //à chaque frames
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && other.GetInstanceID() != refPlayer.GetInstanceID())
        {
            PC.createWeapon(other.GetComponent<PlayerController>().weaponId);
            
            if (!other.GetComponent<PlayerController>().playerIsDead)
                PC.Score++;
            other.GetComponent<PlayerController>().RpcKill();
        }
        /*else if (other.CompareTag("Tiles"))
        {
            other.GetComponent<TilesManager>().destroyThis();
        }*/
    }

    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public void destroyThis()
    {
        Destroy(gameObject);
    }
    #endregion
}

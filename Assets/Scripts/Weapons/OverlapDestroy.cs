using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class OverlapDestroy : MonoBehaviour
{
    #region variable
    [Tooltip("Range Radius du player, et des blockes")]
    public float[] rangeRadius;


    [Tooltip("destroyPlayer")]
    public bool destroyPlayer = true;
    [Tooltip("destroyTiles")]
    public bool destroyTiles = true;


    [HideInInspector]
    public GameObject playerRef;      //le player qui a tiré la rocket

    private GameObject savePlayerRef;

    private float timeToGo;
    private Collider[] gravityColliders;
    private int maxTab = 30;                    //max objectà trigger

    [Tooltip("Optimisation des fps")]
    [SerializeField]
    [Range(0, 10.0f)]
    private float timeOpti = 0.1f;

    #endregion

    #region  initialisation
    //reference
    private void Awake()
    {
        gravityColliders = new Collider[maxTab];
    }

    //initialisation
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                                   //setup le temps
    }

    /// <summary>
    /// détruit
    /// </summary>
    public void startKill(float rangePlayer, float rangeTiled, GameObject refplayer, float time, GameObject saveRef)
    {
        rangeRadius[0] = rangePlayer;
        rangeRadius[1] = rangeTiled;
        playerRef = refplayer;
        if (saveRef)
            savePlayerRef = saveRef;
        Invoke("Overlap", time);                                        //détruit tout après un temps
    }

    #endregion

    #region core script
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeRadius[0]);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, rangeRadius[1]);
    }

    /// <summary>
    /// détruit
    /// </summary>
    private void Overlap()
    {
        Vector3 here = transform.position; // get player position...

        //la liste des ranges à faire...
        for (int i = 0; i < rangeRadius.Length; i++)
        {

            int otherObject = Physics.OverlapSphereNonAlloc(here, rangeRadius[i], gravityColliders);
            if (otherObject > maxTab)
                otherObject = maxTab;

            for (int j = 0; j < otherObject; j++)
            {
                Collider hitCollider = gravityColliders[j];
                if (i == 0 && hitCollider.CompareTag("Player") && destroyPlayer && (!playerRef || hitCollider.gameObject.GetInstanceID() != playerRef.GetInstanceID()))   //c'est un player
                {
                    if (savePlayerRef)
                    {
                        savePlayerRef.GetComponent<PlayerController>().createWeapon(hitCollider.GetComponent<PlayerController>().weaponId);
                        if (!hitCollider.GetComponent<PlayerController>().playerIsDead)
    						savePlayerRef.GetComponent<PlayerController> ().Score++;
                    }
                    hitCollider.gameObject.GetComponent<PlayerController>().RpcKill();
                }
                else if (i == 1 && hitCollider.CompareTag("Tiles") && destroyTiles) //si c'est un tiles...
                {
                    hitCollider.gameObject.GetComponent<TilesManager>().destroyThis();
                }
            }
        }
        destroyThis();
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

using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class Rocket : MonoBehaviour
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [FoldoutGroup("Gameplay")] [Tooltip("speed de la rocket")] public float speed = 5f;
    [FoldoutGroup("Gameplay")] [Tooltip("temps d'attente avant que la rocket tue aussi le player")] public float timeWaitForKillOwner = 1f;
    [FoldoutGroup("Gameplay")] [Tooltip("prefabs overlapDestroy")] public GameObject prefabsOverlapDestroy;
    [FoldoutGroup("Gameplay")] [Tooltip("radius d'explosion pour les players")] public float radiusPlayer = 1f;
    [FoldoutGroup("Gameplay")] [Tooltip("radius d'explosion pour les tiles")] public float radiusTiles = 3f;
    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    [HideInInspector] public GameObject playerRef;      //le player qui a tiré la rocket
    private GameObject savePlayerRef;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private float timeToGo;
    private Rigidbody Rb;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [FoldoutGroup("Debug")] [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)] private float timeOpti = 0.1f;
    [FoldoutGroup("Debug")] [Tooltip("prefabs de la particule de mort")] public GameObject prefabsDeadObject;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        Rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                               //setup le temps
        Rb.velocity = transform.up * speed;
    }
    #endregion

    #region core script
    /// <summary>
    /// active le collider de la rocket !
    /// </summary>
    public void activeRocket(GameObject player)
    {
        playerRef = player;
        savePlayerRef = playerRef;
		CapsuleCollider[] sc = gameObject.GetComponents<CapsuleCollider> ();
		sc[0].enabled = true;
		//sc[1].enabled = true;
        Invoke("killReferencePlayer", timeWaitForKillOwner);
    }

    private void killReferencePlayer()
    {
        playerRef = null;
    }
    #endregion

    #region unity fonction and ending

    /// <summary>
    /// effectué à chaque frame
    /// </summary>
    private void Update()
    {
        //effectué à chaque opti frame
        if (Time.fixedTime >= timeToGo)
        {
            //ici action optimisé

            timeToGo = Time.fixedTime + timeOpti;
        }
    }

    private void FixedUpdate()
    {
        //Rb.MovePosition(transform.position + transform.up * speed * Time.deltaTime);
    }

    /// <summary>
    /// collision avec des objets
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter(Collider collide)
    {
        GameObject other = collide.gameObject;
		bool isValidPlayer = other.CompareTag("Player") && (playerRef == null || (playerRef != null && other.GetInstanceID() != playerRef.GetInstanceID()));
		if (isValidPlayer || other.CompareTag("Tiles") || other.CompareTag("Rocket"))
        {
            GameObject deadOverlap = Instantiate(prefabsOverlapDestroy, gameObject.transform.position, Quaternion.identity) as GameObject;
			deadOverlap.GetComponent<OverlapDestroy>().startKill(radiusPlayer, radiusTiles, playerRef, 0.1f, savePlayerRef);

			destroyThis();
        }
    }

    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public void destroyThis()
    {
        GameObject deadParticle = Instantiate(prefabsDeadObject, gameObject.transform.position, Quaternion.identity) as GameObject;
        Destroy(gameObject);
    }
    #endregion
}

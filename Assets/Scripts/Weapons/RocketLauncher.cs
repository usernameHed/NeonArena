using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Networking;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class RocketLauncher : Weapon
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [FoldoutGroup("Gameplay")] [Tooltip("la vitesse de turnRate du weapon")] [SerializeField] private float turnRate = 2000f;
    [FoldoutGroup("Gameplay")] [Tooltip("Impulsion du joueur lors du tir")] [SerializeField] private float forceImpulse = 10f;
    [FoldoutGroup("Gameplay")] [Tooltip("cree la rocket un peu devant ?")] [SerializeField] private float forwardPoint = 0f;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
	private Rigidbody playerBody;
    private float timeToGo;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [FoldoutGroup("Debug")] [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)] private float timeOpti = 0.1f;
    [FoldoutGroup("Debug")] [Tooltip("rocket prefabs")] [SerializeField] private GameObject prefabsRocket;

    #endregion
    
    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {

    }

    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                               //setup le temps
		playerBody = PC.GetComponent<Rigidbody>();
    }
    #endregion

    #region core script
    /// <summary>
    /// functionTest
    /// </summary>
	protected override void Shoot(float rotation)
    {
        SoundManager.Instance.PlaySound (SoundManager.Instance.RocketLaunchSound);

		Debug.Log("RocketLauncher");
		transform.rotation = Quaternion.Euler (new Vector3(0.0F, 0.0F, rotation));
        GameObject rocket = Instantiate(prefabsRocket, transform.position + (transform.up * forwardPoint), transform.rotation);
        Rocket rockScript = rocket.GetComponent<Rocket>();
        rockScript.activeRocket(PC.gameObject); //active la rocket
      
		playerBody.AddForce(transform.up * -forceImpulse, ForceMode.Impulse);
    }

    /// <summary>
    /// visé avec le joueur
    /// </summary>
    public override void Dir(float horizMove, float vertiMove)
    {
        float heading = Mathf.Atan2(horizMove * turnRate * Time.deltaTime, vertiMove * turnRate * Time.deltaTime);

        Quaternion _targetRotation = Quaternion.identity;

        _targetRotation = Quaternion.Euler(0f, 0f, -heading * Mathf.Rad2Deg);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, turnRate * Time.deltaTime);
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

    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public void destroyThis()
    {
		SoundManager.Instance.PlaySound (SoundManager.Instance.RocketSound);
        Destroy(gameObject);
    }
    #endregion
}

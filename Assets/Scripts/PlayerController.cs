using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// description
/// <summary>

public class PlayerController : MonoBehaviour
{
	public delegate void ScoreUpdate(int score);
	public event ScoreUpdate ScoreUpdateEvent;

    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [FoldoutGroup("Movement")] [Tooltip("l'id du joueur (par rapport à ses contrôles)")] 
	private int idPlayerInOrder = 0;


    [FoldoutGroup("Movement")] [Tooltip("l'id du joueur (par rapport à ses contrôles)")] 
	private int idPlayer = 1;


    [FoldoutGroup("Movement")] [Tooltip("vitesse de déplacement")]
	public float speed = 5f;
    [FoldoutGroup("Movement")] [Tooltip("Ajoute une force lorsqu'on se déplace dans le sens inverse")]
	public float addSpeedInverse = 2f;
    

    [FoldoutGroup("Gameplay")] [Tooltip("le temps d'attente quand on meurt avant de détruire complètement")]
	public float timeBeforeDie = 1.0f;
    [FoldoutGroup("Gameplay")] [Tooltip("prefabs de la particule de mort")]
	public GameObject prefabsDeadPlayer;

     [FoldoutGroup("Gameplay")] [Tooltip("prefabs de bump")]
	public GameObject prefabsBump;
    [FoldoutGroup("Gameplay")] [Tooltip("prefabs de bump")]
	public GameObject prefabsNormalBump;

    [FoldoutGroup("Gameplay")] [Tooltip("prefabs respanwn")]
	public GameObject prefabsRespawn;

    [FoldoutGroup("Gameplay")] [Tooltip("prefabs changeWeapon")]
	public GameObject prefabsChangeWeapon;
    
    [FoldoutGroup("Debug")]     [Tooltip("La liste des weapons")] [SerializeField]
	private List<Weapon> listWeapons;

    [FoldoutGroup("Debug")]    [Tooltip("La liste des prefabs Animator")] [SerializeField]
	private List<GameObject> listPrefabsDisplay;

    [FoldoutGroup("Debug")]    [Tooltip("La liste des weapons")] [SerializeField]
	private Transform weaponParent;

    [FoldoutGroup("Debug")]    [Tooltip("le point de camera")] [SerializeField]
	private GameObject pointCam;

    [FoldoutGroup("Debug")]    [Tooltip("display du joueur")] [SerializeField]
	private GameObject display;

    [FoldoutGroup("Debug")]    [Tooltip("display du joueur")]
	public Animator animatorPlayer;

    private bool totalyDesactivePlayer = false;
    #endregion

    #region private variable

    // Components
    private PlayerConnected playerConnected;
	public Rigidbody rigidbodyPlayer;                   //le rigidbody du joueur

    private bool isWalking = false;         //le joueur est-il en train de marcher ?
	public bool IsWalking
	{
		get { return isWalking; }
	}
    public bool playerIsDead = false;      //le joueur est-il mort ?
	private int direction = 1;
	public int Direction
	{
		get { return direction; }
	}

	private Camera playerCamera;                         //main camera du jeu
	private CameraController cameraController;                //camera controller du jeu
	public Weapon weaponScript;
    public int weaponId = -1;

    public bool stopMove = false;
	public bool jetpack = false;
    private GameObject canvasTmpPlayer;

    public GameObject canvasInGameOfPlayer;

	private int score = 0;
	public int Score
	{
		get { return score; }
		set { score = value; if (ScoreUpdateEvent != null)
                ScoreUpdateEvent (score);
        }
	}

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        playerConnected = PlayerConnected.getSingularity();                              //récupère les infos inputs
        rigidbodyPlayer = GetComponent<Rigidbody>();                                     //récupère le rigidbody
        
        playerCamera = Camera.main;
        cameraController = playerCamera.gameObject.GetComponent<CameraController>();        
    }

    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        startEverything();                                                  //reset toute les variable et references

        transform.position = MapManager.Instance.GetRandomSpawnPoint ();

		cameraController.addToCam(pointCam);              //ajoute à la caméra une target
    }

    

    /// <summary>
    /// reset variable au début du spawn du player
    /// </summary>
    private void startEverything()
    {
        Debug.Log("start playerCOntroller");
        // Random spawn position
        playerIsDead = false;               //le joueur est vivant !
        rigidbodyPlayer.isKinematic = false;
        stopMove = false;
		jetpack = false;

        createWeapon();
    }

    /// <summary>
    /// cree la weapon du joueur
    /// </summary>
    public void createWeapon(int id = -1)
    {
        weaponParent.transform.ClearChild();                    //clear le parent contenant les weapons
        weaponParent.gameObject.SetActive(true);
        if (id == -1)
            weaponId = Random.Range(0, listWeapons.Count - 1);      //id du weapon à prendre dans la liste des weapons prefabs
        else
            weaponId = id;

        Instantiate(prefabsChangeWeapon, transform.position, Quaternion.identity);

        //canvasInGameOfPlayer.GetComponent<PanelPlayer>().changeWeaponId(weaponId);
        GameManagerGame.getSingularity().updateWeaponGUI(this);

        weaponScript = Instantiate(listWeapons[weaponId], transform.position, Quaternion.identity, weaponParent) as Weapon;
		weaponScript.PC = this;

		jetpack = false;
    }
    #endregion

    #region core script
    
    public void setPlayerID(int id)
    {
        idPlayer = id;
    }

    public void setInOrderPlayerID(int idOrder)
    {
        Debug.Log("id??? " + idOrder);
        GameObject playerAnim = Instantiate(listPrefabsDisplay[idOrder - 1], transform.position, Quaternion.identity, display.transform);
        animatorPlayer = playerAnim.transform.GetChild(0).GetComponent<Animator>();
        idPlayerInOrder = idOrder;
    }

    public int getInOrderPlayerID()
    {
        return (idPlayerInOrder);
    }

    public int getPlayerID()
    {
        return (idPlayer);
    }
    
    /// <summary>
    /// Déplace le cube
    /// </summary>
    private void Move()
    {
        float horizMove = playerConnected.getPlayer(idPlayer).GetAxis("Move Horizontal");
        float vertiMove = playerConnected.getPlayer(idPlayer).GetAxis("Move Vertical");

        //on est en train de marcher
        if (horizMove + vertiMove != 0)
        {
            weaponScript.Dir(horizMove, vertiMove);

			if (horizMove != 0)
			{
				direction = (int) Mathf.Sign (horizMove);
			}
            if ((direction == 1 && animatorPlayer.transform.localScale.x < 0) || (direction == -1 && animatorPlayer.transform.localScale.x > 0))
            {
                Vector3 locScale = animatorPlayer.transform.localScale;
                animatorPlayer.transform.localScale = new Vector3(locScale.x * -1, locScale.y, locScale.z);
            }
                

			// Changement de direction
			if ((rigidbodyPlayer.velocity.x < 0 && horizMove > 0) || (rigidbodyPlayer.velocity.x > 0 && horizMove < 0))
            {
                horizMove *= addSpeedInverse;
            }                

			Vector3 movement = new Vector3(horizMove, 0, vertiMove) * speed;
            
			if(!jetpack)    
            	rigidbodyPlayer.AddForce(movement * Time.deltaTime);

            //si on vient de commencer à marcher, activer le son
            if (!isWalking)
            {
                isWalking = true;
            }
        }
        //on est pas en train de marcher
        else
        {
            //si on était en trian de marcher, et qu'on s'est arreté, arreté le son
            if (isWalking)
            {
                isWalking = false;
            }
        }
    }

    /// <summary>
    /// tire
    /// </summary>
    private void Fire()
    {
        bool shallFire = playerConnected.getPlayer(idPlayer).GetButtonDown("FireX") || (weaponId == 1 && playerConnected.getPlayer(idPlayer).GetButton("FireX"));

        if (shallFire)
        {
            float zRotation = weaponScript.transform.rotation.eulerAngles.z;
            weaponScript.TryShoot(zRotation);
        }

		if (playerConnected.getPlayer (idPlayer).GetButtonUp ("FireX"))
		{
			weaponScript.OnShootRelease ();
		}
    }

    /// <summary>
    ///
    /// </summary>
    private void totalyDesactive()
    {
        totalyDesactivePlayer = true;
        weaponParent.gameObject.SetActive(false);
        canvasTmpPlayer.SetActive(false);
    }

    public void totalyActive()
    {
        totalyDesactivePlayer = false;
        weaponParent.gameObject.SetActive(true);
        display.SetActive(true);
        canvasTmpPlayer.SetActive(true);
        Respawn();
    }
    #endregion

    #region unity fonction and ending

    /// <summary>
    /// effectué à chaque frame
    /// </summary>
    private void Update()
    {
		if (playerIsDead || totalyDesactivePlayer)
		{
			return;
		}

        if (!stopMove)
            Move();                                     //déplace le player

        Fire();

		
    }

    /// <summary>
    /// attend X seconde avant de détruire l'objet
    /// cela permet à la caméra de rester focus un temps avant que le
    /// joueur ne meurt totalement
    /// </summary>
    /// <returns></returns>
    private IEnumerator waitBeforeDie()
    {
        display.SetActive(false);
        weaponParent.gameObject.SetActive(false);
        GameObject deadParticle = Instantiate(prefabsDeadPlayer, gameObject.transform.position, Quaternion.identity, gameObject.transform) as GameObject;
        deadParticle.SetActive(true);

        yield return new WaitForSeconds(timeBeforeDie); //wait X seconde

        //totalyDesactive();
        Respawn();
    }
		
	void Respawn()
	{
        
		transform.position = MapManager.Instance.GetRandomSpawnPoint ();
		rigidbodyPlayer.velocity = rigidbodyPlayer.angularVelocity = Vector3.zero;
        Instantiate(prefabsRespawn, transform.position, Quaternion.identity);
        startEverything();
        display.SetActive(true);
	}

    [FoldoutGroup("Debug")]
    [Button("RpcKill")]
    public void RpcKill()
    {
        if (playerIsDead)
            return;

		SoundManager.Instance.PlaySound (SoundManager.Instance.DeathSound);

		Score = (int) Mathf.Max(score - 1, 0);
        rigidbodyPlayer.isKinematic = true;
        playerIsDead = true;
        StartCoroutine(waitBeforeDie());                            //attend X seconde avant de détruire totalement le joueur
    }
		
    [FoldoutGroup("Debug")]
    [Button("changeWeapon")]
    public void changeWeapon()
    {
        startEverything();
    }
    #endregion
}

using UnityEngine;
using Sirenix.OdinInspector;
using Rewired;
using System.Collections.Generic;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class PlayerConnected : MonoBehaviour
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    // Set vibration in all Joysticks assigned to the Player
    [FoldoutGroup("Vibration")] [Tooltip("Active les vibrations")] public bool enabledVibration = true;
    [FoldoutGroup("Vibration")] [Tooltip("the first motor")] public int motorIndex = 0; 
    [FoldoutGroup("Vibration")] [Tooltip("full motor speed")] public float motorLevel = 1.0f;
    [FoldoutGroup("Vibration")] [Tooltip("durée de la vibration")] public float duration = 2.0f;

    public bool[] playerConnected;                      //tableau d'état des controller connecté
    public short playerNumber = 5;                     //size fixe de joueurs (0 = clavier, 1-4 = manette)

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>

    private Player [] playersRewired;                 //tableau des class player (rewired)

    private float timeToGo;
    private static PlayerConnected SS;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [FoldoutGroup("Debug")]
    [ValidateInput("debugTimeOpti", "optimisation supérieur à 0", InfoMessageType.Warning)]
    [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)] private float timeOpti = 0.1f;

    [FoldoutGroup("Debug")]
    [Tooltip("Activation de la singularité du script")]
    [OnValueChanged("testSingularity")]
    [SerializeField]
    private bool enableSingularity = false;

    [FoldoutGroup("Debug")]
    [Tooltip("l'objet est-il global inter-scène ?")]
    [SerializeField]
    private bool dontDestroyOnLoad = false;

    #endregion

    #region fonction debug variables
    /// <summary>
    /// retourne une erreur si le timeOpti est inférieur ou égal à 0.
    /// </summary>
    /// <returns></returns>
    private bool debugTimeOpti(float timeOpti)
    {
        if (timeOpti <= 0)
            return (false);
        return (true);
    }

    /// <summary>
    /// test si on met le script en UNIQUE
    /// </summary>
    private void testSingularity()
    {
        if (!enableSingularity)
            return;

        if (SS == null)
            SS = this;
        else if (SS != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// récupère la singularité (si ok par le script)
    /// </summary>
    /// <returns></returns>
    static public PlayerConnected getSingularity()
    {
        if (!SS || !SS.enableSingularity)
        {
            Debug.LogError("impossible de récupéré la singularité");
            return (null);
        }
        return (SS);
    }

    /// <summary>
    /// set l'objet en dontDestroyOnLoad();
    /// </summary>
    private void setDontDestroyOnLoad()
    {
		if (dontDestroyOnLoad && transform.parent == null)
            DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        testSingularity();                                                  //set le script en unique ?
        setDontDestroyOnLoad();                                             //set le script Inter-scène ?

        playerConnected = new bool[playerNumber];                           //initialise 
        playersRewired = new Player[playerNumber];
        initPlayerRewired();                                                //initialise les event rewired
        initController();                                                   //initialise les controllers rewired
        testMoreThanOneJoypad();                                            //init clavier ou pas...        
    }

    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                               //setup le temps 
        //GameManagerGame.getSingularity().startPlayer();
        //GameManagerGame.getSingularity().updatePlayer();
    }

    /// <summary>
    /// initialise les players
    /// </summary>
    private void initPlayerRewired()
    {
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;

        //parcourt les X players...
        for (int i = 0; i < playerNumber; i++)
        {
            playersRewired[i] = ReInput.players.GetPlayer(i);       //get la class rewired du player X
            playerConnected[i] = false;                             //set son état à false par défault
        }
    }

    /// <summary>
    /// initialise les players (manettes)
    /// </summary>
    private void initController()
    {
        foreach (Player player in ReInput.players.GetPlayers(true))
        {
            foreach (Joystick j in player.controllers.Joysticks)
            {
                setPlayerController(player.id, true);
                break;
            }
        }
    }
    #endregion

    #region core script

    /// <summary>
    /// test combien il y a de manette
    /// </summary>
    private void testMoreThanOneJoypad()
    {
        int numberJoypad = 0;
        for (int i = 1; i < playerConnected.Length; i++)
        {
            if (playerConnected[i])
            {
                numberJoypad++;
            }
        }
        if (numberJoypad < 2)
        {
            playerConnected[0] = true;                             //set son état à false par défault
        }
        else
            playerConnected[0] = false;                             //set son état à false par défault
    }

    /// <summary>
    /// actualise le player ID si il est connecté ou déconnecté
    /// </summary>
    /// <param name="id">id du joueur</param>
    /// <param name="isConnected">statue de connection du joystick</param>
    private void setPlayerController(int id, bool isConnected)
    {
        playerConnected[id] = isConnected;        
    }

    private void updatePlayerController(int id, bool isConnected)
    {
        playerConnected[id] = isConnected;

        testMoreThanOneJoypad();

        if (GameManagerGame.getSingularity())
            GameManagerGame.getSingularity().updatePlayer();
    }

    /// <summary>
    /// get id of player
    /// </summary>
    /// <param name="id"></param>
    public Player getPlayer(int id)
    {
        if (id == -1)
            return (ReInput.players.GetSystemPlayer());
        else if (id >= 0 && id < playerNumber)
            return (playersRewired[id]);
        Debug.LogError("problème d'id");
        return (null);
    }

    /// <summary>
    /// set les vibrations du gamepad
    /// </summary>
    /// <param name="id">l'id du joueur</param>
    public void setVibrationPlayer(int id)
    {
        if (!enabledVibration)
            return;
        getPlayer(id).SetVibration(motorIndex, motorLevel, duration);
    }


    #endregion

    #region unity fonction and ending

    /// <summary>
    /// a controller is connected
    /// </summary>
    /// <param name="args"></param>
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller was connected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        updatePlayerController(args.controllerId + 1, true);
    }

    /// <summary>
    /// a controller is disconnected
    /// </summary>
    void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
    {
        Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        updatePlayerController(args.controllerId + 1, false);
    }

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
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        ReInput.ControllerConnectedEvent -= OnControllerConnected;
        ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
    }
    #endregion
}

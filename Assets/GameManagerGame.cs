using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class GameManagerGame : MonoBehaviour
{
    #region variable
    public GameObject CanvasGameOver;
    public List<Button> listButtonGameOver;
    public List<GameObject> playerList;
    public List<GameObject> playerCanvasInGame;
    public GameObject prefabsPlayer;

    private static GameManagerGame SS; //singleton privé
    private PlayerConnected playerConnected;

    private float timeToGo;
    private bool gameIsOver = false;

    [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)]  private float timeOpti = 0.1f;

    #endregion

    #region  initialisation
    //references
    private void Awake()
    {
        setSingularity();
        playerConnected = PlayerConnected.getSingularity();                              //récupère les infos inputs

        startPlayer();
        updatePlayer();
    }

    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                                   //setup le temps
        
    }

    private void setSingularity()
    {
        if (SS == null)
            SS = this;
        else if (SS != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// récupère la singularité (si ok par le script)
    /// </summary>
    /// <returns></returns>
    static public GameManagerGame getSingularity()
    {
        if (!SS)
        {
            Debug.LogError("impossible de récupéré la singularité");
            return (null);
        }
        return (SS);
    }

    /// <summary>
    /// game over appelé quand un player est mort...
    /// </summary>
    public void gameOver(bool active)
    {
        if (active)
        {
            CanvasGameOver.SetActive(true);
            gameIsOver = true;
        }
        else
        {
            gameIsOver = false;
            CanvasGameOver.SetActive(false);
        }
    }

    /// <summary>
    /// initialise la liste des playrrs
    /// </summary>
    public void startPlayer()
    {
        for (int i = 0; i < playerConnected.playerConnected.Length; i++)
        {
            playerList.Add(null);
        }
    }

    /// <summary>
    /// remplis la liste des players
    /// </summary>
    public void updatePlayer()
    {
        for (int i = 0; i < playerList.Count; i++)
        {


            if (playerConnected.playerConnected[i] && !playerList[i])
            {

                GameObject newPlayer = Instantiate(prefabsPlayer);              //cree un player
                playerList[i] = newPlayer;                                      //set ce player à la list du gameManager

                PlayerController PCtmp = newPlayer.GetComponent<PlayerController>();
                PCtmp.setPlayerID(i);      //set l'id du player

            }

            else if (!playerConnected.playerConnected[i] && playerList[i])
            {
                playerCanvasInGame[i].SetActive(false);                          //active le canvas lié au player
                Destroy(playerList[i]);
            }
        }

        updateInGameCanvasOfPlayer();
    }

    private void updateInGameCanvasOfPlayer()
    {
        int idOrder = 1;
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerConnected.playerConnected[i])
            {
                Debug.Log(idOrder);
                PlayerController PCtmp = playerList[i].GetComponent<PlayerController>();
                GameObject panelPlayer = playerCanvasInGame[idOrder - 1];

                PCtmp.setInOrderPlayerID(idOrder);
                panelPlayer.SetActive(true);                          //active le canvas lié au player
                PCtmp.canvasInGameOfPlayer = panelPlayer;    //set le canvasInGame au player
				panelPlayer.GetComponent<PanelPlayer>().PC = PCtmp;
                PCtmp.ScoreUpdateEvent += panelPlayer.GetComponent<PanelPlayer>().changeScorePlayer;
                panelPlayer.GetComponent<PanelPlayer>().changeWeaponId(PCtmp.weaponId);
                idOrder++;
            }
        }
        for (int i = idOrder; i < 5; i++)
        {
            Debug.Log("false: " + i);
            GameObject panelPlayer = playerCanvasInGame[i - 1];
            panelPlayer.SetActive(false);                          //active le canvas lié au player
        }
    }

    public void updateWeaponGUI(PlayerController PCtmp)
    {
        PCtmp.canvasInGameOfPlayer.GetComponent<PanelPlayer>().changeWeaponId(PCtmp.weaponId);
        /*for (int i = 0; i < playerList.Count; i++)
        {
            if (playerConnected.playerConnected[i])
            {
                PlayerController PCtmp = playerList[i].GetComponent<PlayerController>();
                PCtmp.canvasInGameOfPlayer.GetComponent<PanelPlayer>().changeWeaponId(PCtmp.weaponId);
            }
        }*/
    }

    #endregion

    #region core script

    #endregion

    #region unity fonction and ending
    //à chaque frames
    private void Update()
    {
        if (gameIsOver && (playerConnected.getPlayer(0).GetButtonDown("FireA") || playerConnected.getPlayer(1).GetButtonDown("FireA")))
        {
            gameOver(false);
        }
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

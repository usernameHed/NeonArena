using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class MenuManager : MonoBehaviour
{
    #region variable

    public GameObject credits;
    public GameObject tutoObj;

    private float timeToGo;
    private PlayerConnected PC;
    private bool credit = false;
    private bool tuto = false;

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
        PC = PlayerConnected.getSingularity();
    }
    #endregion

    #region core script
    public void jumpToScene(string scene = "1_MainMenu")
    {
        SceneManager.LoadScene(scene);
    }
    #endregion

    #region unity fonction and ending
    //à chaque frames
    private void Update()
    {
        if (tuto)
        {
            if (PC.getPlayer(0).GetButtonDown("UISubmit") || PC.getPlayer(1).GetButtonDown("UISubmit"))
            {
                jumpToScene("scene1");
            }
            else if (PC.getPlayer(0).GetButtonDown("UICancel") || PC.getPlayer(1).GetButtonDown("UICancel"))
            {
                tuto = false;
                tutoObj.SetActive(false);
            }
        }
        else
        {
            if (PC.getPlayer(0).GetButtonDown("UISubmit") || PC.getPlayer(1).GetButtonDown("UISubmit"))
            {
                tutoObj.SetActive(true);
                tuto = true;
                //jumpToScene("scene1");
            }
            else if ((PC.getPlayer(0).GetButtonDown("UICancel") || PC.getPlayer(1).GetButtonDown("UICancel")) && !credit)
            {
                Quit();
            }
            else if ((PC.getPlayer(0).GetButtonDown("UICancel") || PC.getPlayer(1).GetButtonDown("UICancel")) && credit)
            {
                //retour au menu
                credits.SetActive(false);
                credit = false;
            }
            else if ((PC.getPlayer(0).GetButtonDown("FireX") || PC.getPlayer(1).GetButtonDown("FireX")) && !credit)
            {
                credits.SetActive(true);
                credit = true;
            }
            else if ((PC.getPlayer(0).GetButtonDown("FireX") || PC.getPlayer(1).GetButtonDown("FireX")) && !credit)
            {
                credits.SetActive(false);
                credit = true;
            }
        }

    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public void destroyThis()
    {
        Destroy(gameObject);
    }
    #endregion
}

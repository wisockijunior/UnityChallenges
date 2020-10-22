using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SidiaUnityChallenge;

public class MainMenuActions : MonoBehaviour
{
    private void Awake()
    {
        if (GameMgr == null)
        {
            GameMgr = new BejeweledGameMgr();
            GameMgr.PivotChessboardGO = this.PivotChessboardGO;
            GameMgr.GameMgrGo = this.PivotChessboardGO;
            GameMgr.Reset();
        }
    }


    //Renato - 22/10/2020
    //TODO: move public references to the correct place!
    // public references
    public GameObject PivotChessboardGO;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
        if (GameMgr != null)
        {
            // game running?
            if (GameMgr.GetIsGameRunning())
            {
                // handle game mouse/finger user interactions
                GameMgr.HandleMouseClick();
            }
        }
    }

    public BejeweledGameMgr GameMgr;

    public void OnUserClick_NewGame()
    {
        GameMgr.StartNewGame();
    }


    public void OnUserClick_Normal()
    {

    }


    public void OnUserClick_TimeTrial()
    {

    }


    public void OnUserClick_Options()
    {

    }

    public void OnUserClick_QuitGame()
    {
        GameMgr.StopGame();
    }

}

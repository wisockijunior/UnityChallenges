using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SidiaUnityChallenge;

public class MainMenuActions : MonoBehaviour
{
    private void Awake()
    {
        if (GameMgr == null)
            GameMgr = new BejeweledGameMgr();

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMgr != null)
        {
            GameMgr.CheckForMouseClick();
        }
    }

    

    public BejeweledGameMgr GameMgr;

    public void OnUserClick_NewGame()
    {
        // new game
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

    }

}

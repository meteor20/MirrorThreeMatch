using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public Text titleText;
    public GameObject GameState;
    public GameObject GameOver;
    public GameObject GameOverEffect;
    bool hasGameOver;
    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        GameState.gameObject.SetActive (true );
        GameOver.gameObject.SetActive(false);
        titleText.text ="第"+ (PlayerPrefs.GetInt(GameData.Level) + 1).ToString()+"关";
    }


    // Update is called once per frame
    void Update()
    {
        if (PieceBoard.instance!=null &&PieceBoard.instance .PlayState==PlayState.GameOver && hasGameOver==false )
        {
            SetGameOverEffect();
         int level=   PlayerPrefs.GetInt(GameData.Level)+1;
            PlayerPrefs.SetInt(GameData.Level, level);
            hasGameOver = true;
        }
    }

    public void SetGameOverEffect()
    {
        GameState.gameObject.SetActive(false );
        GameOver.gameObject.SetActive(transform );
        Instantiate(GameOverEffect);
    }


    public void loadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}

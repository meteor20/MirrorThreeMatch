using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public Text titleText;
    public Slider timeslider; 
    public GameObject GameState;
    public GameObject GameOver;
    public Text gameLoseTips;
    public GameObject GameLosePanel;
    public GameObject GameOverEffect;

    [Header("GameOver")]
    public Text overContext;
    bool hasGameOver;


    private float m_SinceStartGame;
    private int m_SinceCheckNumber;
    // Start is called before the first frame update
    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        GameState.gameObject.SetActive (true );
        GameOver.gameObject.SetActive(false);
        GameLosePanel .gameObject.SetActive(false);
        titleText.text ="第"+ (PlayerPrefs.GetInt(GameData.Level) + 1).ToString()+"关";


        if (PieceBoard.instance != null)
        {
            PieceBoard.instance.male.OnCharacterDeath += GameLose;
            PieceBoard.instance.womma .OnCharacterDeath += GameLose;
        }

        m_SinceStartGame = Time.time;
        }


    // Update is called once per frame
    void Update()
    {
        if (PieceBoard.instance!=null &&PieceBoard.instance .PlayState==PlayState.GameOver && hasGameOver==false )
        {
#if !UNITY_EDITOR
 TapDB.onResume();
#endif

            SetGameOverEffect();
         int level=   PlayerPrefs.GetInt(GameData.Level)+1;
            PieceBoard.instance.womma.GetComponent<Animator>().SetBool ("run",false );
            PieceBoard.instance.male .GetComponent<Animator>().SetBool("run", false);
            PlayerPrefs.SetInt(GameData.Level, level);
            hasGameOver = true;
           // AnalyticsEvent.Custom("Level Progress", new Dictionary<string, object>
    //{
    //    { "Level10", level },
    //    { "Level5", level },
    //    { "Level15", level },

    //});
        }

        if ((Time.time - m_SinceStartGame) / 60 >m_SinceCheckNumber )
        {
            m_SinceCheckNumber= (int ) ((Time.time - m_SinceStartGame) / 60);
        }

        if (CheckGame1())
        {
            GameLose();
            gameLoseTips.text = "时间到了";
            Debug.LogError("GameoverLLLLLLLLLL");
        }
       // Debug.LogError(CheckGame());
    }

    public void SetGameOverEffect()
    {
        if (FindObjectOfType<ThemeMusic>())

            FindObjectOfType<ThemeMusic>().audioSource.Stop();
        GameState.gameObject.SetActive(false );
        GameOver.gameObject.SetActive(transform );
        overContext.text = "用时"+((int )(Time.time - m_SinceStartGame)).ToString()+"秒";
        Instantiate(GameOverEffect);
    }


    public void loadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene ().name );
    }

    public void GameLose()
    {
      //  Debug.LogError("gisog;iesrhnrg;i");
    GameLosePanel.SetActive(true );
        gameLoseTips.text = "注意躲避陷阱";
      if (FindObjectOfType<ThemeMusic>())

            FindObjectOfType<ThemeMusic>().audioSource.Stop();
    }

    public void LoadNextLevel()
    {
        int level = PlayerPrefs.GetInt(GameData.Level) + 1;
        SceneManager.LoadScene("Level"+level .ToString ());

    }

    public bool    CheckGame1()
    {
        if (Time.time  -  PieceBoard .instance .sinceTouchTime>160)
        {
            return true;
        }

        timeslider.value = 1-(Time.time - PieceBoard.instance.sinceTouchTime) / 160;
        return false;
    }
    public bool  CheckGame()
    {
        PieceSlot[,] pieceSlots = PieceBoard.instance.pieceSlots;
        for (int i = 0; i < PieceBoard.instance.topBoardWith  ; i++)
        {
            for (int j  = 1; j  <= PieceBoard.instance.topBoardHeight+ PieceBoard.instance.bottomBoardHeight  ; j ++)
            {
                // int count = 1;

                PieceSlot sampleSlot = pieceSlots[i, j];
                List<Piece> pieces = new List<Piece>();
                int index = i;
                index = index + 1;
                if (index < PieceBoard .instance .topBoardWith )
                {
                    int y = j - 1;
                    if (y >= 1)
                    {
                        if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[index, y].Piece != null && pieceSlots[index, y].Piece.PieceType == sampleSlot.Piece.PieceType  &&!pieces.Contains (pieceSlots[index, y].Piece))
                        {
                            pieces.Add(pieceSlots[index, y].Piece);
                        }
                    }
                    y = j + 1;
                    if (y <= PieceBoard.instance.topBoardHeight + PieceBoard.instance.bottomBoardHeight)
                    {
                        if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[index, y].Piece != null && pieceSlots[index, y].Piece.PieceType == sampleSlot.Piece.PieceType   && !pieces.Contains (pieceSlots[index, y].Piece))
                        {
                            pieces.Add(pieceSlots[index, y].Piece);
                        }
                    }
                }
                index = index - 1;
                while (index > 0)
                {
                    index--;
                    if (pieceSlots[index, j].Piece == null || sampleSlot.Piece == null)
                    {
                        //Debug.LogError(pieceSlots[index, j].position.x + "-" + pieceSlots[index, j].position.z);
                        //Debug.LogError(sampleSlot.position.x + "-" + sampleSlot.position.z);
                    }
                    if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[index, j].Piece != null && pieceSlots[index, j].Piece.PieceType == sampleSlot.Piece.PieceType  && !pieces.Contains (pieceSlots[index, j].Piece))
                    {
                        pieces.Add(pieceSlots[index, j].Piece);
                    }
                    else
                    {
                        break;
                    }
                }
               
                if (pieces.Count >= 1)
                {
                    if (index - 1 >= 0)
                    {
                        if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[index - 1, j].Piece != null && pieceSlots[index - 1, j].Piece.PieceType == sampleSlot.Piece.PieceType && !pieces.Contains(pieceSlots[index - 1, j].Piece))
                        {
                            pieces.Add(pieceSlots[index - 1, j].Piece);
                        }
                    }


                    //count = 1;
                    if (index  >= 0 && pieceSlots[index, j].Piece !=null && pieceSlots [index ,j].Piece.GetComponent<IImmoveable>() == null)
                    {
                       // index -= 1;
                        int y = j - 1;
                        if (y >= 1)
                        {
                            if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[index, y].Piece != null && pieceSlots[index, y].Piece.PieceType == sampleSlot.Piece.PieceType  && !pieces.Contains(pieceSlots[index, y].Piece))
                            {
                                pieces.Add(pieceSlots[index, y].Piece);
                            }
                        }
                        y = j + 1;
                        if (y<= PieceBoard.instance.topBoardHeight + PieceBoard.instance.bottomBoardHeight)
                        {
                            if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[index, y].Piece != null && pieceSlots[index, y].Piece.PieceType == sampleSlot.Piece.PieceType  && !pieces.Contains (pieceSlots[index, y].Piece))
                            {
                                pieces.Add(pieceSlots[index, y].Piece);
                            }
                        }

                    }

                    if (pieces.Count >=3)
                    {
                        Debug.LogError(pieces.Count);
                        return true;
                    }
                    
                    pieces.Clear();
                }




                //vertical
                pieces.Clear();
                index = j;
                index += 1;
                if (index<= PieceBoard.instance.topBoardHeight + PieceBoard.instance.bottomBoardHeight)
                {
                    int x = i - 1;
                    if (x >= 0)
                    {
                        if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[x, index].Piece != null && pieceSlots[x, index].Piece.PieceType == sampleSlot.Piece.PieceType && !pieces.Contains(pieceSlots[x, index].Piece))
                        {
                            pieces.Add(pieceSlots[x, index].Piece);
                        }
                    }
                    x = i + 1;
                    if (x < PieceBoard.instance.topBoardWith)
                    {
                        if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[x, index].Piece != null && pieceSlots[x, index].Piece.PieceType == sampleSlot.Piece.PieceType  && !pieces.Contains (pieceSlots[x, index].Piece))
                        {
                            pieces.Add(pieceSlots[x, index].Piece);
                        }
                    }
                }
                index -= 1;
                while (index > 1)
                {
                    index--;

                    if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[i, index].Piece != null && pieceSlots[i, index].Piece.PieceType == sampleSlot.Piece.PieceType&&!pieces.Contains(pieceSlots[i, index].Piece))
                    {
                        pieces.Add(pieceSlots[i, index].Piece);
                    }
                    else
                    {
                        break;
                    }
                }

                if (pieces.Count >= 1)
                {
                    if (index >= 1 && pieceSlots[i, index].Piece !=null && pieceSlots[i, index  ].Piece.GetComponent<IImmoveable>() == null)
                    {
                        int x = i - 1;
                        if (x >= 0)
                        {
                            if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[x, index].Piece != null && pieceSlots[x, index].Piece.PieceType == sampleSlot.Piece.PieceType&& !pieces.Contains (pieceSlots[x, index].Piece))
                            {
                                pieces.Add(pieceSlots[x, index].Piece);
                            }
                        }
                        x = i + 1;
                        if (x < PieceBoard.instance.topBoardWith)
                        {
                            if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IImmoveable>() == null && pieceSlots[x, index].Piece != null && pieceSlots[x, index].Piece.PieceType == sampleSlot.Piece.PieceType&&!pieces.Contains(pieceSlots[x, index].Piece))
                            {
                                pieces.Add(pieceSlots[x, index].Piece);
                            }
                        }


                    }

                    if (pieces.Count >= 3)
                    {
                        Debug.LogError(pieces.Count);
                        return true;
                    }
                  
                    pieces.Clear();

                }
            }
        }
     
        return false;

    }
}

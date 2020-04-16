using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public Text displayText;
    public GameObject noGameLevelTips;
    public int maxLevel = 4;
    // Start is called before the first frame update
    void Start()
    {
        displayText.text = "你的成就：成功找到了" + PlayerPrefs.GetInt(GameData.Level)+"对镜像世界的人";
       // PlayerPrefs.SetInt(GameData.Level, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void  loadLevel(string level)
    {
        SceneManager.LoadScene(level );
    }

    public void PlayGame()
    {
      int level=  PlayerPrefs.GetInt(GameData.Level )+1;
        
        if (maxLevel<level || SceneManager.GetSceneByName("Level" + level) == null)
            noGameLevelTips.SetActive(true );
        else 
        SceneManager.LoadScene("Level"+level );
    }
}

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
    public   LevelData[] levelDatas;

    void Start()
    {
        displayText.text = "你的成就：成功找到了" + PlayerPrefs.GetInt(GameData.Level)+"对镜像世界的人";
        // PlayerPrefs.SetInt(GameData.Level, 0);
#if !UNITY_EDITOR
        TapDB.onStart("039xb742honceuwk", "taptap", "");
#endif

    }


    public void  loadLevel(string level)
    {
        SceneManager.LoadScene(level );
    }

    public void PlayGame()
    {
      int level=  PlayerPrefs.GetInt(GameData.Level )+1;
        
        if (maxLevel  < level || SceneManager.GetSceneByName("Level" + level) == null)
            noGameLevelTips.SetActive(true );
        else 
        SceneManager.LoadScene("Level"+level );
    }
    void OnApplicationQuit()
    {
#if !UNITY_EDITOR
           TapDB.onStop();
        Debug.Log("OnApplicationQuit");
#endif


    }


    public static void ToJeson()
    {
        //for (int i = 0; i < levelDatas.Length ; i++)
        //{
        //    JsonUtility.ToJson(levelDatas[i]);
        //}
       
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThemeMusic : MonoBehaviour
{
    [HideInInspector ]
    public  AudioSource audioSource;
    public AudioClip[] audioClips;
    float m_SinceMusicStartedTime;
    int index;
    float singleTime;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource >();
        if (audioSource ==null )
        {
            Debug.LogError("audio is null ");
        }
         index = Random.Range(0,audioClips .Length );
        audioSource.clip =(audioClips[index ]);
        m_SinceMusicStartedTime = Time.time;
        singleTime = audioClips[index].length;
        audioSource.Play();
    
    }

    // Update is called once per frame
    void Update()
    {
        if (Time .time -m_SinceMusicStartedTime > singleTime)
        {
            index = (index + 1) % audioClips.Length;
            audioSource.clip = (audioClips[index]);
            singleTime = audioClips[index].length;
            audioSource.Play();
            m_SinceMusicStartedTime = Time.time;

        }
    }
}

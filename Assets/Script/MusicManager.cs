using UnityEngine;
using System.Collections;
using System;

public class MusicManager : MonoBehaviour
{
    [Header("流すためのAudioSource")]
    public AudioSource AudioSource;
    
    [Header("ボス戦が始まる前のBGM")]
    public AudioClip BossPrevious;
    [Range(0f,1f)]public float BossPreviousVolume = 1f;
    
    [Header("ボス戦のBGM")]
    public AudioClip Boss;
    [Range(0f, 1f)] public float BossVolume = 1f;
    
    [Header("ボス戦が終わった後のBGM")]
    public AudioClip BossClear;
    [Range(0f, 1f)] public float BossClearVolume = 1f;

    [Header("様々なフラグ")]
    public bool BossStartFlag = false;
    public bool BossClearFlag = false;

    [Header("ステージが始まってから最初のBGMが流れるまでの時間")]
    public float StartSeconds = 2.0f;//ステージが始まってから最初のBGMが流れるまでの時間
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.clip = BossPrevious;
        AudioSource.volume = BossPreviousVolume;
        StartCoroutine(Waitseconds());
    }

    // Update is called once per frame
    void Update()
    {
        if (BossStartFlag)
        {
            //AudioSource.volume -= FadeOutSeconds * Time.deltaTime;
            //AudioSource.Stop();
            AudioSource.clip = Boss;
            AudioSource.volume = BossVolume;
            AudioSource.Play();
            BossStartFlag = false;
        }
        if (BossClearFlag)
        {
            //AudioSource.volume -= FadeOutSeconds * Time.deltaTime;
            //AudioSource.Stop();
            AudioSource.clip = BossClear;
            AudioSource.volume = BossClearVolume;
            AudioSource.Play();
            BossClearFlag = false;
        }

    }
    private IEnumerator Waitseconds()
    {
        yield return new WaitForSeconds(StartSeconds);
        AudioSource.Play();
    }

}

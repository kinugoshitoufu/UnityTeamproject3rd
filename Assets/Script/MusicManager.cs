using UnityEngine;
using System.Collections;
using System;

public class MusicManager : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip BossPrevious;
    public AudioClip Boss;
    public AudioClip BossClear;
    public bool BossStartFlag = false;
    public bool BossClearFlag = false;
    [Header("ステージが始まってから最初のBGMが流れるまでの時間")]
    public float StartSeconds = 2.0f;//ステージが始まってから最初のBGMが流れるまでの時間
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        AudioSource.clip = BossPrevious;
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
            AudioSource.Play();
            BossStartFlag = false;
        }
        if (BossClearFlag)
        {
            //AudioSource.volume -= FadeOutSeconds * Time.deltaTime;
            //AudioSource.Stop();
            AudioSource.clip = BossClear;
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

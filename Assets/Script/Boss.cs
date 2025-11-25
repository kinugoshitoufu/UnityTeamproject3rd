using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // ========== SE用 =============
    [Header("SE関連")]
    [Tooltip("seClipsを追加してオーディオファイルを追加")]
    public AudioClip[] seClips;
    private AudioSource[] seAudios;
    [Tooltip("seClipsの最大数指定")]
    public int maxSeAudio = 10;

    // ========== ゲーム用 ==========
    [Header("ゲーム関連")]
    public float HP = 100;//HP
    public float waitTime = 0.9f;//待機時間
    public Rigidbody2D rb;

    private float startHP;

    protected float ratioHP=100;//HPの割合
    protected bool waitComplete = false;//最初の待機


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        StartCoroutine(Wait());//待機処理を呼び出す
        StartInit();//変数初期化
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!waitComplete) return;
        ratioHP = HP / startHP;
    }

    //初期化処理
    void StartInit()
    {
        //ゲーム変数関連の初期化
        startHP = HP;
        ratioHP = 100;

        //SE関連の初期化
        seAudios = new AudioSource[maxSeAudio];
        for (int i = 0; i < maxSeAudio; i++)
        {
            seAudios[i] = gameObject.AddComponent<AudioSource>();
            seAudios[i].loop = false; // ループ再生を無効化
            seAudios[i].playOnAwake = false;// 自動再生無効化
        }
    }

    //最初に呼び出される待機処理
    IEnumerator Wait()
    {
        Debug.Log(waitTime + "秒間の待機を開始します。");
        //指定したフレーム分、待機
        yield return new WaitForSeconds(waitTime);
        waitComplete = true;
        Debug.Log("待機処理が完了しました。");
    }

    //SEを再生
    public void PlaySE(int index)
    {
        if (index < 0) return; // indexが0未満なら何もしない
        if (index >= seClips.Length) return;// indexが範囲外なら何もしない
        for (int i = 0; i < maxSeAudio; i++)
        {  // 再生中ではないプレイヤーを探す
            if (seAudios[i].isPlaying) continue;// 再生中なら次へ
            seAudios[i].PlayOneShot(seClips[index]);// SEを再生
            break;// SEを鳴らしたらfor文を抜ける
        }
    }


}


using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using System.Runtime.InteropServices.WindowsRuntime;


public class Boss : MonoBehaviour
{

    // ========== ゲーム用 ==========
    [Header("ゲーム関連")]
    public float HP = 100;//HP
    [Header("ボスの最大HP")]
    public float MaxHP = 100;//ボスの最大HP
    public float Hp { get { return HP; } }
    public float waitTime = 0.9f;//待機時間

    private float beforeMaxHP = 100;
    private float beforeHP = 100;

    //[Header("攻撃パラメータ(待機、余韻など)")]
    //public List<attackParameters> attackParm = new List<attackParameters>();

    // ========== コンポーネント ==========
    public Rigidbody2D rb;
    public Transform PlayerPos;
    public bool Invincible { get { return invincible; }}//無敵状態かどうか

    // ========== 参照用 ==========
    private int direction = -1;//(右が1,左が-1)
    //private float startHP;
    private float distanceX;
    private bool invincible = false;//無敵状態かどうか?

    private float beforeNum = 0;

    private Vector2 rightEdge;//画面右端
    private Vector2 leftEdge; //画面左端


    protected float ratioHP = 100;//HPの割合

    //protected float ratioHP { get { return HP / startHP; } set { } }//HPの割合
    protected int Direction { get { return direction; } }//向きの取得
    
    protected bool waitComplete = false;//最初の待機
    protected Vector2 RightEdge { get { return rightEdge; }}
    protected Vector2 LeftEdge { get {return leftEdge; }}

    // ========== SE用 =============
    [Header("SE関連")]
    [Tooltip("seClipsを追加してオーディオファイルを追加")]
    public AudioClip[] seClips;
    private AudioSource[] seAudios;
    [Tooltip("seClipsの最大数指定")]
    public int maxSeAudio = 10;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        StartCoroutine(Wait());//待機処理を呼び出す
        StartInit();//変数初期化
    }

    // Update is called once per frame
    protected void Update()
    {

        //プレイヤーとの距離から向きを計算
        if (PlayerPos != null)
        {
            distanceX = PlayerPos.position.x - transform.position.x;
            if (distanceX != 0) direction = (int)(distanceX / Mathf.Abs(distanceX));//ゼロ除算対策
        }
        
        if (!waitComplete) return;
        ratioHP = HP / MaxHP;

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    Debug.Log("HPを半分にします");
        //    if (ratioHP > 0.5f) HP = MaxHP * 0.49f;
        //    else HP = 0;
        //}

        //HPもしくはMaxHPが変更されたら
        if ((beforeMaxHP != MaxHP)||(beforeHP!=HP))
        {
            ShowHP();
            beforeHP = HP;
            beforeMaxHP = MaxHP;
        }

        //体力を変更するデバッグモード
        DebugHPMode();

    }

    

    //押したキーの体力に変更
    void DebugHPMode()
    {
        float num = beforeNum;

        for(int i = 1; i <= 9; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                num = i * 0.1f;
                Debug.Log(i + "キーが押されました");
            }
        }

        //0キーが押されたら体力を100%にする
        if (Input.GetKeyDown(KeyCode.Alpha0)) num = 1;

        //違うボタンが押されたら採用
        if (beforeNum == num) return;
        //if (MaxHP * num == HP) return;
        beforeNum = num;

        //押されたキーの割合に体力を変更する
        ChangeDebugHP(MaxHP * num);
    }

    void ChangeDebugHP(float num)
    {
        Debug.Log("ボスのHPを" + HP +"/"+MaxHP+"から" + num + "/" + MaxHP + "から" + "に変更しました");
        HP = num;
        //ShowHP();
    }
    //HPを表示する
    void ShowHP()
    {
        Debug.Log("<ボス>「最大HP=" + MaxHP + ",現在のHP=" + HP + ",HP割合=" + (HP / MaxHP) * 100 + "%」");
    }

    //初期化処理
    void StartInit()
    {
        //ゲーム変数関連の初期化-------------
        ratioHP = HP/MaxHP;

        beforeHP = HP;
        beforeMaxHP= MaxHP;

        //画面端の座標を取得(ボスが画面外に出ないように大きさの半分引いておく)
        rightEdge = Camera.main.ViewportToWorldPoint(Vector2.one);
        leftEdge = Camera.main.ViewportToWorldPoint(Vector2.zero);

        rightEdge.x -= transform.lossyScale.x/2;
        leftEdge.x += transform.lossyScale.x/2;

        //SE関連の初期化----------------------
        seAudios = new AudioSource[maxSeAudio];
        for (int i = 0; i < maxSeAudio; i++)
        {
            seAudios[i] = gameObject.AddComponent<AudioSource>();
            seAudios[i].loop = false; // ループ再生を無効化
            seAudios[i].playOnAwake = false;// 自動再生無効化
        }
    }

    //無敵状態にする
    public void OnInvincible(bool flag)
    {
        invincible = flag;
    }

    //ボスの体力が50%以上かどうか
    public bool CheckHP()
    {
        ratioHP = HP / MaxHP;
        return (ratioHP >= 0.5f) ? true : false;
    }


    //ボスが死亡しているかどうか?
    public bool CheckDeath()
    {
        return (HP <= 0) ? true : false;
    }


    //最初に呼び出される待機処理
    public IEnumerator Wait()
    {
        waitComplete = false;
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


    //攻撃受けた際のコード(テスト)
    //protected virtual void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Bullet"))
    //    {
    //        Debug.Log("ボスが攻撃を喰らいました");
    //    }
    //}

    //protected virtual void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Bullet"))
    //    {
    //        Debug.Log("ボスが攻撃を喰らいました2");
    //    }
    //}



}

[System.Serializable]
//攻撃のパラメータ
public class attackParameters
{
    [Header("処理時間")]
    public processTime proTime;
    [Header("アニメーション時間")]
    public animationTime animeTime;
}

[System.Serializable]
//処理時間
public class processTime
{
    [Header("準備時間")]
    public float preparationTime = 0;
    [Header("攻撃時間")]
    public float attackTime = 0;
    [Header("攻撃余韻")]
    public float afterglowTime = 0;

}
[System.Serializable]
//アニメーションの再生時間
public class animationTime
{
    [Header("準備時間")]
    public float preparationTime = 0;
    [Header("攻撃時間")]
    public float attackTime = 0;
    [Header("攻撃余韻")]
    public float afterglowTime = 0;

}



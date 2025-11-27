using UnityEngine;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using System.Threading;
using System.ComponentModel;

/// <summary>
/// プレイヤーの各フレームの行動を記録するデータクラス
/// スーパータイムフォースウルトラのようなクローン再生システムで使用
/// </summary>
[System.Serializable]
public class PlayerAction
{
    public float time;              // 記録開始からの経過時間
    public Vector2 position;        // その時点でのプレイヤーの位置
    public Vector2 velocity;        // その時点でのプレイヤーの速度（物理演算用）
    public bool jumpInput;          // ジャンプボタンが押されたかどうか
    public float horizontalInput;   // 左右の入力値（-1.0 ～ 1.0）
    public bool shotInput;          // 弾を発射したかどうか（追加）
}

/// <summary>
/// プレイヤーキャラクターの移動・ジャンプ・行動記録を管理するスクリプト
/// プレイヤーの全ての行動を記録し、指定タイミングでクローンを生成する
/// </summary>
public class PlayerScript : MonoBehaviour
{
    // ========== 様々な機能に必要な変数 ==========
    private bool flicflag = false;
    public static PlayerScript instance;
    private Animator animator;

    // ========== 移動関連のパラメータ ==========
    [Header("移動設定")]
    [Tooltip("左右の移動速度")]
    public float moveSpeed = 5f;

    [Tooltip("ジャンプの強さ（上方向の初速度）")]
    public float jumpForce = 7f;

    // ========== コンポーネント参照 ==========
    private Rigidbody2D rb;          // 物理演算用のRigidbody2D
    public bool isGrounded;          // 地面に接地しているかどうか

    // ========== 記録システム関連 ==========
    [Header("記録設定")]
    [Tooltip("記録された全ての行動データ")]
    private List<PlayerAction> recordedActions = new List<PlayerAction>();

    [Tooltip("現在記録中かどうか")]
    private bool isRecording = true;

    [Tooltip("記録開始からの経過時間")]
    private float recordingTime = 0f;

    // ========== クローン生成用 ==========
    [Header("クローン設定")]
    [Tooltip("生成するクローンのプレハブ（Inspectorで設定必須）")]
    public GameObject clonePrefab;

    // ========== 弾生成用 =============
    [Header("弾生成用のプレハブ")]
    [Tooltip("発射する弾のプレハブ")]
    public GameObject Bullet;

    [Header("発射位置（ShotPoint）")]
    [Tooltip("弾が発射される位置（Transform）")]
    public Transform shotPoint;

    // ========== SE用 =============
    [Header("SE関連")]
    [Tooltip("seClipsを追加してオーディオファイルを追加")]
    public AudioClip[] seClips;
    private AudioSource[] seAudios;
    [Tooltip("seClipsの最大数指定")]
    public int maxSeAudio = 10;
    /// <summary>
    /// 初期化処理
    /// Rigidbody2Dコンポーネントを取得
    /// </summary>
    void Start()
    {
        // Rigidbody2Dコンポーネントを取得（物理演算に必要）
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        //audioSource = GetComponent<AudioSource>();
        instance = this;
        seAudios = new AudioSource[maxSeAudio];
        for (int i = 0; i < maxSeAudio; i++)
        {
            seAudios[i] = gameObject.AddComponent<AudioSource>();
            seAudios[i].loop = false; // ループ再生を無効化
            seAudios[i].playOnAwake = false;// 自動再生無効化
        }

        // クローンプレハブが設定されていない場合は警告を出す
        if (clonePrefab == null)
        {
            Debug.LogWarning("ClonePrefabが設定されていません！Inspectorで設定してください。");
        }

        // 弾のプレハブが設定されていない場合は警告を出す
        if (Bullet == null)
        {
            Debug.LogWarning("Bulletプレハブが設定されていません！Inspectorで設定してください。");
        }

        // ShotPointが設定されていない場合は警告を出す
        if (shotPoint == null)
        {
            Debug.LogWarning("ShotPointが設定されていません！Inspectorで設定してください。");
        }

        // 最初は記録を停止状態で開始
        isRecording = false;
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新処理
    /// 記録中は入力を記録し、条件を満たしたらクローン生成
    /// </summary>
    void Update()
    {
        // 左右の入力を取得（-1.0 ～ 1.0 の範囲）
        float horizontal = Input.GetAxis("Horizontal");

        // ジャンプボタン（スペースキー）が押されたかを取得
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        // 記録を開始するための条件チェック
        // 何か入力があれば記録開始
        if (Mathf.Abs(horizontal) >= 0.01f || jumpPressed)
        {
            isRecording = true;
        }

        // 記録中の場合、プレイヤーの入力と状態を記録
        if (isRecording)
        {
            RecordPlayerInput();
        }

        // プレイヤーが完全に停止したらクローンを生成
        // 速度が0で、かつ入力もない状態
        if (rb.linearVelocity == Vector2.zero && Mathf.Abs(horizontal) == 0.0f)
        {
            // 記録データがある場合のみクローンを生成
            if (recordedActions.Count != 0)
            {
                Debug.Log("クローン生成条件を満たしました");
                StopRecordingAndSpawnClone();

                // 記録を停止
                if (isRecording)
                {
                    isRecording = false;
                }
            }
        }

        // Rキーでシーンをリセット（やり直し機能）
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.SceneManagement.Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        // 右クリックで弾を発射
        // ※この処理はRecordPlayerInput内で記録されます
        if (Input.GetMouseButtonDown(1))
        {
            Shot();
        }
    }

    /// <summary>
    /// プレイヤーの入力と状態を記録し、実際の移動処理も行う
    /// </summary>
    void RecordPlayerInput()
    {
        // 左右の入力を取得（-1.0 ～ 1.0 の範囲）
        float horizontal = Input.GetAxis("Horizontal");

        // ジャンプボタン（スペースキー）が押されたかを取得
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);

        // 右クリック（弾の発射）が押されたかを取得
        bool shotPressed = Input.GetMouseButtonDown(1);

        // ========== 現在の状態を記録 ==========
        PlayerAction action = new PlayerAction
        {
            time = recordingTime,                    // 現在の記録時間
            position = transform.position,           // 現在の位置
            velocity = rb.linearVelocity,            // 現在の速度（物理演算の速度）
            jumpInput = jumpPressed,                 // ジャンプボタンの入力
            horizontalInput = horizontal,            // 左右の入力値
            shotInput = shotPressed                  // 弾の発射入力（追加）
        };
        recordedActions.Add(action);  // 記録リストに追加

        // 記録時間を進める
        recordingTime += Time.deltaTime;

        // ========== 実際の移動処理 ==========

        if (horizontal <= -0.01f && !flicflag)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            flicflag = true;
        }
        if (horizontal >= 0.01f && flicflag)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            flicflag = false;
        }

        // 左右の入力がある場合、横方向の速度を設定
        if (Mathf.Abs(horizontal) >= 0.01f)
        {
            animator.SetBool("MoveBool",true);
            rb.linearVelocityX = horizontal * moveSpeed;
        }
        else
        {
            // 入力がない場合は横方向の速度を0にする（滑り続けないように）
            animator.SetBool("MoveBool", false);
            rb.linearVelocityX = 0f;
        }

        // 地面に接地していてジャンプボタンが押された場合
        if (isGrounded && jumpPressed)
        {
            // 記録が停止していた場合は再開
            if (!isRecording)
            {
                isRecording = true;
            }

            // Y方向に力を加えてジャンプ（X方向の速度は維持）
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    /// <summary>
    /// 記録を停止してクローンを生成し、新しい記録を開始する
    /// </summary>
    void StopRecordingAndSpawnClone()
    {
        // 記録データがない場合は何もしない
        if (recordedActions.Count == 0)
        {
            Debug.LogWarning("記録データがありません。クローンを生成できません。");
            return;
        }

        // クローンプレハブが設定されていない場合は生成できない
        if (clonePrefab == null)
        {
            Debug.LogError("ClonePrefabが設定されていません！");
            return;
        }

        // 記録を一時停止
        isRecording = false;

        // ========== クローンの生成 ==========
        // 最初の記録位置にクローンを生成
        GameObject clone = Instantiate(clonePrefab, recordedActions[0].position, Quaternion.identity);

        // クローンのコントローラーを取得
        CloneController cloneController = clone.GetComponent<CloneController>();

        if (cloneController != null)
        {
            // クローンに記録データを渡す（新しいListを作成してコピー）
            cloneController.SetRecordedActions(new List<PlayerAction>(recordedActions));
        }
        else
        {
            Debug.LogError("ClonePrefabにCloneControllerがアタッチされていません！");
        }

        // ========== 新しい記録を開始 ==========
        recordedActions.Clear();  // 記録データをクリア
        recordingTime = 0f;       // 記録時間をリセット
        isRecording = true;       // 記録を再開

        Debug.Log("クローンを生成しました！新しい記録を開始します。");
    }

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

    /// <summary>
    /// 弾を発射する処理
    /// 右クリック時に呼ばれる
    /// </summary>
    void Shot()
    {
        // 弾のプレハブとShotPointが設定されているか確認
        if (Bullet == null)
        {
            Debug.LogWarning("Bulletプレハブが設定されていません！");
            return;
        }

        if (shotPoint == null)
        {
            Debug.LogWarning("ShotPointが設定されていません！");
            return;
        }

        // ShotPointの位置と回転で弾を生成
        GameObject bullet = Instantiate(Bullet, shotPoint.position, shotPoint.rotation);
        PlaySE(0);
        Debug.Log("弾を発射しました");
    }

    public bool Getflicflag()
    {
        return flicflag;
    }

    /// <summary>
    /// 他のコライダーと衝突した瞬間に呼ばれる
    /// 地面との接触を検知して接地状態を更新
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトが"Ground"タグを持っている場合
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;  // 接地状態をtrueに
        }
    }

    /// <summary>
    /// 他のコライダーと接触し続けている間呼ばれる
    /// 地面との接触を維持
    /// </summary>
    private void OnCollisionStay2D(Collision2D collision)
    {
        // 衝突したオブジェクトが"Ground"タグを持っている場合
        if (collision.collider.CompareTag("Ground"))
        {
            animator.SetBool("JumpBool", false);
            isGrounded = true;  // 接地状態をtrueに
        }
    }

    /// <summary>
    /// 他のコライダーから離れた瞬間に呼ばれる
    /// 地面から離れたことを検知して接地状態を更新
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 離れたオブジェクトが"Ground"タグを持っている場合
        if (collision.collider.CompareTag("Ground"))
        {
            animator.SetBool("JumpBool", true);
            isGrounded = false;  // 接地状態をfalseに
        }
    }

    /// <summary>
    /// トリガーコライダーに入った瞬間に呼ばれる
    /// クローンの上に乗ることができるようにする
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // クローンタグを持つオブジェクトの場合
        if (collision.CompareTag("Clone"))
        {
            isGrounded = true;  // 接地状態をtrueに
        }
    }

    /// <summary>
    /// トリガーコライダー内にいる間呼ばれる
    /// クローンの上に乗り続けている状態
    /// </summary>
    private void OnTriggerStay2D(Collider2D collision)
    {
        // クローンタグを持つオブジェクトの場合
        if (collision.CompareTag("Clone"))
        {
            isGrounded = true;  // 接地状態をtrueに
        }
    }

    /// <summary>
    /// トリガーコライダーから出た瞬間に呼ばれる
    /// クローンから離れた状態
    /// </summary>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // クローンタグを持つオブジェクトの場合
        if (collision.CompareTag("Clone"))
        {
            isGrounded = false;  // 接地状態をfalseに
        }
    }
}
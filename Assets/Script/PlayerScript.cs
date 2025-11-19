using UnityEngine;
using System.Collections.Generic;

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
}

/// <summary>
/// プレイヤーキャラクターの移動・ジャンプ・行動記録を管理するスクリプト
/// プレイヤーの全ての行動を記録し、指定タイミングでクローンを生成する
/// </summary>
public class PlayerScript : MonoBehaviour
{
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
    public GameObject Bullet;

    [Header("発射位置（ShotPoint）")]
    public Transform shotPoint;

    /// <summary>
    /// 初期化処理
    /// Rigidbody2Dコンポーネントを取得
    /// </summary>
    void Start()
    {
        // Rigidbody2Dコンポーネントを取得（物理演算に必要）
        rb = GetComponent<Rigidbody2D>();

        // クローンプレハブが設定されていない場合は警告を出す
        if (clonePrefab == null)
        {
            Debug.LogWarning("ClonePrefabが設定されていません！Inspectorで設定してください。");
        }
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新処理
    /// 記録中は入力を記録し、Rキーでクローン生成
    /// </summary>
    void Update()
    {
        // 記録中の場合、プレイヤーの入力と状態を記録
        if (isRecording)
        {
            RecordPlayerInput();
        }

        // Rキーが押されたらクローンを生成
        // ※ここは好きなタイミングに変更可能（死亡時、タイムアウト時など）
        if (Input.GetKeyDown(KeyCode.R))
        {
            StopRecordingAndSpawnClone();
        }

        // 右クリックが押されたらクローンを生成
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

        // ========== 現在の状態を記録 ==========
        PlayerAction action = new PlayerAction
        {
            time = recordingTime,                    // 現在の記録時間
            position = transform.position,           // 現在の位置
            velocity = rb.linearVelocity,            // 現在の速度（物理演算の速度）
            jumpInput = jumpPressed,                 // ジャンプボタンの入力
            horizontalInput = horizontal             // 左右の入力値
        };
        recordedActions.Add(action);  // 記録リストに追加

        // 記録時間を進める
        recordingTime += Time.deltaTime;

        // ========== 実際の移動処理 ==========
        // 左右の入力がある場合、横方向の速度を設定
        if (Mathf.Abs(horizontal) >= 0.01f)
        {
            rb.linearVelocityX = horizontal * moveSpeed;
        }
        else
        {
            // 入力がない場合は横方向の速度を0にする（滑り続けないように）
            rb.linearVelocityX = 0f;
        }

        // 地面に接地していてジャンプボタンが押された場合
        if (isGrounded && jumpPressed)
        {
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

    void Shot()
    {
        // 弾を生成
        GameObject bullet = Instantiate(Bullet, shotPoint.position, shotPoint.rotation);
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
    /// 他のコライダーから離れた瞬間に呼ばれる
    /// 地面から離れたことを検知して接地状態を更新
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 離れたオブジェクトが"Ground"タグを持っている場合
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;  // 接地状態をfalseに
        }
    }
}
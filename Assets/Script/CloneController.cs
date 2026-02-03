using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

/// <summary>
/// クローンの行動を制御するスクリプト
/// 記録された行動データを再生し、ループさせる
/// スーパータイムフォースウルトラのクローンシステムを実現
/// </summary>
public class CloneController : MonoBehaviour
{
    // ========== 様々な機能に必要な変数 ==========
    private bool flicflag = false;
    private float DefCloneScale;
    private float CloneScale;
    public static CloneController instance;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private UnityEngine.Color color;
    private bool movestop = false;

    // ========== 記録データ関連 ==========
    [Tooltip("再生する行動データのリスト")]
    public List<PlayerAction> recordedActions;

    // ========== コンポーネント参照 ==========
    public Rigidbody2D rb;          // 物理演算用のRigidbody2D
    public bool isGrounded;         // 地面に接地しているかどうか

    // ========== 再生状態管理 ==========
    [Tooltip("現在再生中の行動データのインデックス")]
    public int currentActionIndex = 0;

    [Tooltip("再生開始からの経過時間")]
    public float playbackTime = 0f;

    [Tooltip("記録全体の長さ（秒）")]
    public float totalRecordingTime = 0f;

    // ========== 再生モード設定 ==========
    [Header("再生設定")]
    [Tooltip("位置補間を使用するかどうか（trueでスムーズな動き）")]
    public bool useInterpolation = true;

    // ========== 弾発射用 ==========
    [Header("弾生成用のプレハブ")]
    [Tooltip("クローンが発射する弾のプレハブ（Inspectorで設定必須）")]
    public GameObject Bullet;

    [Header("発射位置（ShotPoint）")]
    [Tooltip("弾が発射される位置（Transform）")]
    public Transform shotPoint;

    [Header("ダメージヒット時")]
    [Tooltip("ノックバックの強さ")]
    public Vector2 knockpower;
    [Tooltip("ダメージを食らってから消えるまでの時間")]
    public float Deadspeed = 10.0f;
    // 前フレームで弾を撃ったかどうかを記憶（連続発射防止用）
    private bool previousShotInput = false;

    /// <summary>
    /// 初期化処理
    /// Rigidbody2Dコンポーネントを取得
    /// </summary>
    void Start()
    {

        // Rigidbody2Dコンポーネントを取得（物理演算に必要）
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
        DefCloneScale = transform.localScale.x;
        instance = this;
        // クローンは重力の影響を受けないようにする（記録通りに動かすため）
        // ※物理演算と記録再生を併用する場合はコメントアウト
        if (rb != null)
        {
            rb.gravityScale = 0f;  // 重力を無効化
        }

        // 弾のプレハブが設定されていない場合は警告を出す
        if (Bullet == null)
        {
            Debug.LogWarning("CloneのBulletプレハブが設定されていません！Inspectorで設定してください。");
        }

        // ShotPointが設定されていない場合は警告を出す
        if (shotPoint == null)
        {
            Debug.LogWarning("CloneのShotPointが設定されていません！Inspectorで設定してください。");
        }
    }

    /// <summary>
    /// 外部から記録データを設定する
    /// PlayerScriptからクローン生成時に呼ばれる
    /// </summary>
    /// <param name="actions">再生する行動データのリスト</param>
    public void SetRecordedActions(List<PlayerAction> actions)
    {
        // 記録データを受け取る
        recordedActions = actions;

        // 記録データが存在する場合、初期設定を行う
        if (recordedActions != null && recordedActions.Count > 0)
        {
            // 記録全体の長さを計算（最後の記録の時間）
            totalRecordingTime = recordedActions[recordedActions.Count - 1].time;

            // クローンを最初の記録位置に配置
            transform.position = recordedActions[0].position;

            // 速度を初期化
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            Debug.Log($"クローンが{recordedActions.Count}個の行動データを受け取りました（長さ: {totalRecordingTime:F2}秒）");
        }
        else
        {
            Debug.LogWarning("記録データが空です！");
        }
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新処理
    /// 記録された行動を再生し、ループさせる
    /// </summary>
    void Update()
    {
        CloneScale = transform.localScale.x;
        //GameObject Clone = GameObject.FindWithTag("clone");
        //if (Clone != null)
        //{
        //    Collider2D clonecoll = Clone.GetComponent<Collider2D>();
        //    Collider2D mycoll = GetComponent<Collider2D>();
        //    Physics2D.IgnoreCollision(mycoll, clonecoll, true);
        //}
        // 記録データがない、または空の場合は何もしない
        if (recordedActions == null || recordedActions.Count == 0)
        {
            return;
        }

        // 再生時間を進める
        if (!movestop)
        {
            playbackTime += Time.deltaTime;
        }
        

        // ========== ループ処理 ==========
        // 記録時間を超えたら最初に戻る
        if (playbackTime > totalRecordingTime && !movestop)
        {
            // 再生時間をリセット（少しオーバーした分は考慮）
            playbackTime = playbackTime - totalRecordingTime;

            // インデックスをリセット
            currentActionIndex = 0;

            // 最初の位置に瞬時に戻る
            transform.position = recordedActions[0].position;

            // 速度をリセット
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // 前フレームの発射状態もリセット
            previousShotInput = false;

            //Debug.Log("クローンがループしました");
        }

        // 現在の時刻に対応する行動を再生
        if (!movestop)
        {
            ReplayActions();
        }

        if (movestop)
        {
            rb.linearVelocity = Vector2.zero;
            color.a -= Deadspeed * Time.deltaTime;
            spriteRenderer.color = color;
        }

        if (color.a <= 0.0f)
        {
            Destroy(gameObject);
        }

        if (Mathf.Abs(rb.linearVelocityX) >= 0.01f)
        {
            animator.SetBool("MoveBool", true);
        }
        else
        {
            animator.SetBool("MoveBool", false);
        }

        if (rb.linearVelocityY >= 0.01f)
        {
            animator.SetBool("JumpBool", true);
        }

        if (rb.linearVelocityX <= -0.01f && !flicflag)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            flicflag = true;
        }
        if (rb.linearVelocityX >= 0.01f && flicflag)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            flicflag = false;
        }

        if (animator.GetBool("FlicBool") != flicflag)
        {
            animator.SetBool("FlicBool", true);
        }
        if (animator.GetBool("FlicBool") != flicflag)
        {
            animator.SetBool("FlicBool", false);
        }

        bool RightJump = animator.GetCurrentAnimatorStateInfo(0).IsName("CloneJumpRight");
        bool LeftJump = animator.GetCurrentAnimatorStateInfo(0).IsName("CloneJumpLeft");
        if (animator.GetBool("JumpBool") && !isGrounded)
        {

            if (animator.GetBool("FlicBool") == true && RightJump == true)
            {
                animator.Play("CloneJumpLeft", 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }
            if (animator.GetBool("FlicBool") == false && LeftJump == true)
            {
                animator.Play("CloneJumpRight", 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }
        }

        bool RightAttack = animator.GetCurrentAnimatorStateInfo(0).IsName("CloneAttackRight");
        bool LeftAttack = animator.GetCurrentAnimatorStateInfo(0).IsName("CloneAttackLeft");

        if (animator.GetBool("FlicBool") == true && RightAttack == true)
        {
            animator.Play("CloneAttackLeft", 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        if (animator.GetBool("FlicBool") == false && LeftAttack == true)
        {
            animator.Play("CloneAttackRight", 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }

        bool JumpRightAttack = animator.GetCurrentAnimatorStateInfo(0).IsName("CloneJumpAttackRight");
        bool JumpLeftAttack = animator.GetCurrentAnimatorStateInfo(0).IsName("CloneJumpAttackLeft");

        if (animator.GetBool("FlicBool") == true && JumpRightAttack == true)
        {
            animator.Play("CloneJumpAttackLeft", 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }
        if (animator.GetBool("FlicBool") == false && JumpLeftAttack == true)
        {
            animator.Play("CloneJumpAttackRight", 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }

    }

    /// <summary>
    /// 現在の再生時刻に対応する行動データを見つけて再生する
    /// </summary>
    void ReplayActions()
    {
        // ========== 現在の時刻に対応するアクションを探す ==========
        // 次のアクションの時刻が現在の再生時刻を超えるまでインデックスを進める
        while (currentActionIndex < recordedActions.Count - 1 &&
               recordedActions[currentActionIndex + 1].time <= playbackTime)
        {
            currentActionIndex++;
        }

        // インデックスが範囲内であることを確認
        if (currentActionIndex < recordedActions.Count)
        {
            // 現在のアクションを取得
            PlayerAction currentAction = recordedActions[currentActionIndex];

            if (currentAction.shotInput && !previousShotInput)
            {
                Shot();  // 弾を発射
            }

            // 現在の発射状態を記憶（次のフレームで使用）
            previousShotInput = currentAction.shotInput;

            // ========== 位置と速度の適用 ==========
            if (useInterpolation && currentActionIndex < recordedActions.Count - 1)
            {
                // 【補間モード】次のアクションとの間を滑らかに補間
                PlayerAction nextAction = recordedActions[currentActionIndex + 1];

                // 現在のアクションと次のアクションの間での進行度を計算（0.0 ～ 1.0）
                float timeDiff = nextAction.time - currentAction.time;

                // 0除算を防ぐ
                if (timeDiff > 0.0001f)
                {
                    float t = (playbackTime - currentAction.time) / timeDiff;
                    // tを0～1の範囲にクランプ
                    t = Mathf.Clamp01(t);

                    // 位置を線形補間（スムーズな動き）
                    Vector2 interpolatedPosition = Vector2.Lerp(
                        currentAction.position,
                        nextAction.position,
                        t
                    );
                    transform.position = interpolatedPosition;

                    // 速度も補間して適用
                    if (rb != null)
                    {
                        Vector2 interpolatedVelocity = Vector2.Lerp(
                            currentAction.velocity,
                            nextAction.velocity,
                            t
                        );
                        rb.linearVelocity = interpolatedVelocity;
                    }
                }
                else
                {
                    // 時間差がほぼない場合は現在のアクションをそのまま使用
                    transform.position = currentAction.position;
                    if (rb != null)
                    {
                        rb.linearVelocity = currentAction.velocity;
                    }
                }
            }
            else
            {
                // 【非補間モード】記録された位置と速度をそのまま適用
                transform.position = currentAction.position;

                if (rb != null)
                {
                    rb.linearVelocity = currentAction.velocity;
                }
            }
        }
    }

    void EndAnimAttackClone()
    {
        animator.SetBool("AttackBool", false);
    }

    void Shot()
    {
        //animator.SetBool("AttackBool", true);
        // 弾のプレハブとShotPointが設定されているか確認
        if (Bullet == null)
        {
            Debug.LogWarning("クローンのBulletプレハブが設定されていません！");
            return;
        }

        if (shotPoint == null)
        {
            Debug.LogWarning("クローンのShotPointが設定されていません！");
            return;
        }

        // ShotPointの位置と回転で弾を生成
        GameObject bullet = Instantiate(Bullet, shotPoint.position, shotPoint.rotation);

        if (animator.GetBool("FlicBool") == true && animator.GetBool("MoveBool") == false && animator.GetBool("JumpBool") == false)
        {
            animator.Play("CloneAttackLeft", 0, 0.0f);
        }
        else if (animator.GetBool("FlicBool") == false && animator.GetBool("MoveBool") == false && animator.GetBool("JumpBool") == false)
        {
            animator.Play("CloneAttackRight", 0, 0.0f);
        }
        else if (animator.GetBool("FlicBool") == true && animator.GetBool("MoveBool") == true && animator.GetBool("JumpBool") == false)
        {
            animator.Play("CloneMoveAttackLeft", 0, 0.0f);
        }
        else if (animator.GetBool("FlicBool") == false && animator.GetBool("MoveBool") == true && animator.GetBool("JumpBool") == false)
        {
            animator.Play("CloneMoveAttackRight", 0, 0.0f);
        }
        else if (animator.GetBool("FlicBool") == false && animator.GetBool("JumpBool") == true)
        {
            animator.Play("CloneJumpAttackRight", 0, 0.0f);
        }
        else if (animator.GetBool("FlicBool") == true && animator.GetBool("JumpBool") == true)
        {
            animator.Play("CloneJumpAttackLeft", 0, 0.0f);
        }

        Debug.Log("クローンが弾を発射しました");
    }

    public void EndAnimAttackBool()
    {
        animator.SetBool("AttackBool", false);
    }
    public void EndAnimMoveRightAttackBool()
    {
        animator.Play("CloneMoveRight", 0, 0.6f);
    }
    public void EndAnimMoveLeftAttackBool()
    {
        animator.Play("CloneMoveLeft", 0, 0.6f);
    }
    public void EndAnimJumpRightAttackBool()
    {
        animator.Play("CloneJumpRight", 0, 0.5f);
    }
    public void EndAnimJumpLeftAttackBool()
    {
        animator.Play("CloneJumpLeft", 0, 0.5f);
    }

    public float GetDefCloneScaleX()
    {
        return DefCloneScale;
    }
    public float GetCloneScaleX()
    {
        return CloneScale;
    }
    /// <summary>
    /// 他のコライダーと衝突した瞬間に呼ばれる
    /// 地面との接触を検知（将来的な拡張用）
    /// </summary>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトが"Ground"タグを持っている場合
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;  // 接地状態をtrueに
            if (animator.GetBool("JumpBool") == true)
            {
                animator.SetBool("JumpBool", false);
            }
        }
        if (collision.collider.CompareTag("Bullet"))
        {
            rb.linearVelocity = Vector2.zero;
            Vector2 distination = (transform.position - collision.transform.position).normalized;
            rb.AddForce(Vector2.up * knockpower.y, ForceMode2D.Force);
            rb.AddForce(distination * knockpower, ForceMode2D.Impulse);
            animator.SetBool("DamageBool", true);
            if (animator.GetBool("FlicBool") == true && animator.GetBool("DamageBool") == true)
            {
                animator.Play("CloneDamageLeft");
            }
            else if (animator.GetBool("FlicBool") == false && animator.GetBool("DamageBool") == true)
            {
                animator.Play("CloneDamageRight");
            }
            movestop = true;
            //Destroy(gameObject);
        }
        if (collision.collider.CompareTag("Enemy"))
        {
            rb.linearVelocity = Vector2.zero;
            Vector2 distination = (transform.position - collision.transform.position).normalized;
            rb.AddForce(Vector2.up * knockpower.y, ForceMode2D.Impulse);
            rb.AddForce(distination * knockpower, ForceMode2D.Impulse);
            animator.SetBool("DamageBool", true);
            if (animator.GetBool("FlicBool") == true && animator.GetBool("DamageBool") == true)
            {
                animator.Play("CloneDamageLeft");
            }
            else if (animator.GetBool("FlicBool") == false && animator.GetBool("DamageBool") == true)
            {
                animator.Play("CloneDamageRight");
            }
            movestop = true;
            //Destroy(gameObject);
        }
    }

    /// <summary>
    /// 他のコライダーから離れた瞬間に呼ばれる
    /// 地面から離れたことを検知（将来的な拡張用）
    /// </summary>
    private void OnCollisionExit2D(Collision2D collision)
    {
        // 離れたオブジェクトが"Ground"タグを持っている場合
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;  // 接地状態をfalseに
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
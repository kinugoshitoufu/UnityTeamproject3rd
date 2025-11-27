using UnityEngine;
using UnityEngine.EventSystems;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float Destroytimer = 1.0f;
    private Vector2 moveDirection;
    private float timer = 0.0f;
    private bool isFrozen = false;
    private Collider2D coll;
    void Start()
    {
        coll = GetComponent<Collider2D>();
        if (PlayerScript.instance.Getflicflag() == false)
        {
            moveDirection = Vector2.right;
        }
        else
        {
            moveDirection = Vector2.left;
        }
    }
    void Update()
    {
        // 凍結されていない時だけ移動
        if (!isFrozen)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        // Ground にぶつかった後の削除タイマー
        if (isFrozen)
        {
            timer += Time.deltaTime;
            if (timer >= Destroytimer)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isFrozen = true;
            moveSpeed = 0f;   // 停止
            coll.isTrigger = true;
        }

        if (collision.gameObject.CompareTag("Clone"))
        {
            Destroy(gameObject);
        }
    }
}

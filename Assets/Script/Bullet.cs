using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float Destroytimer = 1.0f;
    private float timer = 0.0f;
    private bool isFrozen = false;
    private Collider2D coll;
    void Start()
    {
        coll = GetComponent<Collider2D>();
    }
    void Update()
    {
        // 凍結されていない時だけ移動
        if (!isFrozen)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime, Space.World);
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
    }
}

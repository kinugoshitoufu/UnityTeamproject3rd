using UnityEngine;

public class CloneBullet : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float Destroytimer = 1.0f;
    public Vector2 moveDirection;
    public float timer = 0.0f;
    public bool isFrozen = false;
    public GameObject Clone;
    private Collider2D coll;
    void Start()
    {
        coll = GetComponent<Collider2D>();
        Clone = FetchNearObjectWithTag("Clone");
        if (Clone.transform.localScale.x >= 0.0f)
        {
            moveDirection = Vector2.right;
        }
        else if (Clone.transform.localScale.x <= 0.0f)
        {
            moveDirection = Vector2.left;
        }
    }
    void Update()
    {
        // 凍結されていない時だけ移動
        if (!isFrozen)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime,Space.World);
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

    private GameObject FetchNearObjectWithTag(string tagName)
    {
        // 該当タグが1つしか無い場合はそれを返す
        var targets = GameObject.FindGameObjectsWithTag(tagName);
        if (targets.Length == 1) return targets[0];

        GameObject result = null;
        var minTargetDistance = float.MaxValue;
        foreach (var target in targets)
        {
            // 前回計測したオブジェクトよりも近くにあれば記録
            var targetDistance = Vector3.Distance(transform.position, target.transform.position);
            if (!(targetDistance < minTargetDistance)) continue;
            minTargetDistance = targetDistance;
            result = target.transform.gameObject;
        }

        // 最後に記録されたオブジェクトを返す
        return result;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isFrozen = true;
            moveSpeed = 0f;   // 停止
            coll.isTrigger = true;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}

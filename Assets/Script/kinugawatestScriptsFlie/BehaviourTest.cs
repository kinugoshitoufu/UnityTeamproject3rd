using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTest : MonoBehaviour
{
    public static BehaviourTest Instance;
    public float moveSpeed = 1.5f;
    public GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveUp()
    {
        transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
    }
    public void MoveDown()
    {
        transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
    }
    public void MoveLeft()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }
    public void MoveRight()
    {
        transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
    }
    public bool WherePlayer()
    {
        if (Player.transform.position.x > gameObject.transform.position.x)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

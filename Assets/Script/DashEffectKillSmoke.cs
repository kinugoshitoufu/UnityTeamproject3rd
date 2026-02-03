using UnityEngine;

public class DashEffectKillSmoke : MonoBehaviour
{
    void Destroyme()
    {
        
        Destroy(gameObject);
        ScreenManager.instance.Close();
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    void DestroyPlayer()
    {
        PlayerScript.instance.GetComponent<SpriteRenderer>().enabled = false;
    }
}

using UnityEngine;

public class DashEffectKillSmoke : MonoBehaviour
{
    [SerializeField] private GameObject deathPlayer;
    Vector3 floorPos = new Vector3(-6, -3.64f, 0);

    void Destroyme()
    {
        
        Destroy(gameObject);
        StartCoroutine(ScreenManager.instance.Close());
    }

    void DestroyMe()
    {
        Destroy(gameObject);
    }

    void AppearancePlayer()
    {
        Instantiate(deathPlayer, floorPos, Quaternion.identity);
    }

    void DestroyPlayer()
    {
        PlayerScript.instance.GetComponent<SpriteRenderer>().enabled = false;
    }
}

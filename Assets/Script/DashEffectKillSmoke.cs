using UnityEngine;

public class DashEffectKillSmoke : MonoBehaviour
{
    [SerializeField] private GameObject deathPlayer;
    [SerializeField] private GameObject clearPlayer;
    [SerializeField] private GameObject deathBoss;


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

    void AppearanceClearPlayer()
    {
        Instantiate(clearPlayer, floorPos, Quaternion.identity);
    }

    void AppearanceBoss()
    {
        Instantiate(deathBoss, new Vector3(-floorPos.x,floorPos.y,floorPos.z), Quaternion.identity);
    }


    void DestroyPlayer()
    {
        PlayerScript.instance.GetComponent<SpriteRenderer>().enabled = false;
    }
}

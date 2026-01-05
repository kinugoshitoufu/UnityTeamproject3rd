using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyBulletPrefab;
    public Transform spawnPointLeft;
    public Transform spawnPointRight;

    private Elephant elephant;
    public GameObject otherObject; // elephantスクリプトをアタッチしているオブジェクト

    public float timer=1.0f;

    private void Start()
    {
        //Debug.Log("EnemySpawnerは正常です!!!");
        elephant = otherObject.GetComponent<Elephant>();
    }
    private void Update()
    {
        if (elephant.balljump)
        {
            //Debug.Log("balljumpがtrueになったのを感知");
            timer += Time.deltaTime;
            if (timer > 1) 
            { 
                SpawnBullet();
                elephant.balljump=false;
                timer = 0;
            }
        }
    }
    public void SpawnBullet()
    {
        Transform spawnPoint = (Random.value < 0.5f) ? spawnPointLeft : spawnPointRight;
        Instantiate(enemyBulletPrefab, spawnPoint.position, Quaternion.identity);
    }
}

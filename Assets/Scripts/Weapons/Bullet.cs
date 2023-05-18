using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public GameObject splashText;
    public LayerMask enemyLayer;
    [SerializeField] bool enemyBullet;

    bool hitEnemy = false;

    private void Start() {
        Destroy(this.gameObject, 10);
    }

    private void OnCollisionEnter(Collision other) {
        if(((1<<other.gameObject.layer) & enemyLayer) != 0 && !hitEnemy && !enemyBullet) {
            hitEnemy = true;
            // Instantiate(splashText, other.transform.position + new Vector3(Random.Range(-1f, 1f), .5f, Random.Range(-1f, 1f)), Quaternion.Euler(0, -180, 0));
            
            Transform tempT = other.transform;

            while (!tempT.GetComponent<Enemy>()) {
                tempT = tempT.parent;
            }

            Enemy enemy = tempT.GetComponent<Enemy>();
            enemy.TakeDamage(damage, GunData.gunTypes.projectile);
        }
        if (!hitEnemy && enemyBullet) { // If hit player
            if (!other.transform.CompareTag("Player")) return;
            if (other.transform.GetComponent<player_move>().godMode) return;
            DeathManager deathManager = GameObject.FindObjectOfType<DeathManager>();
            deathManager.StartCoroutine(deathManager.Death());
        }
    }
}

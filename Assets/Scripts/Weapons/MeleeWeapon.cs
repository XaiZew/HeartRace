using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public LayerMask enemyLayer;
    bool hit;
    RaycastHit raycastHit;

    public Transform cam;
    public GameObject splashText;

    public MeleeData meleeData;

    float attackCoolddown;
    float attackTimer;

    [SerializeField] AudioSource bonkSource;

    private void Start() {
        
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && attackTimer >= attackCoolddown) {
            Attack();
        }

        attackTimer += 1 * Time.deltaTime;
    }

    void Attack() {
        bonkSource.Play();
        attackTimer = 0;
        hit = Physics.Raycast(cam.position, cam.forward, out raycastHit, meleeData.range, enemyLayer);
        if (hit) {
            Transform tempT = raycastHit.transform;

            while (!tempT.GetComponent<Enemy>()) {
                tempT = tempT.parent;
            }

            Enemy enemy = tempT.GetComponent<Enemy>();
            enemy.TakeDamage(meleeData.damage, isMelee:true);

        }
    }
    
}

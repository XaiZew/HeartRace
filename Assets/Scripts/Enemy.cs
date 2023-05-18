using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    GameManager                 gameManager;
    
    CameraShake                 cameraShake;
    public EnemyData            data;
    float                       health;
    public LayerMask            playerLayer;
    public GameObject           bulletPrefab;
    
    [SerializeField] Transform  orientation;
    Transform                   player;
    [SerializeField] GameObject splashText;
    [SerializeField] Transform  head;
    [SerializeField] Transform headAnchor;

    bool playerInRange = false;
    bool isDead = false;

    float shootCooldownTimer;

    [SerializeField] AudioSource hitSource;

    private void Start() {
        shootCooldownTimer = Random.Range(0, data.shootCooldown * 100) / 100;
        gameManager = GameObject.FindObjectOfType<GameManager>();
        cameraShake = FindObjectOfType<camera_move>().transform.Find("CameraShake").GetComponent<CameraShake>();
        health = data.totalHealth;

        SwitchKinematic(transform, true);
        // StartCoroutine(CheckForPlayer());
    }

    void SwitchKinematic(Transform transf, bool bo) {
        foreach(Transform t in transf) { // foreach child
            if (t.GetComponent<Rigidbody>())
                t.GetComponent<Rigidbody>().isKinematic = bo;
            
            if (t.childCount > 0) {
                SwitchKinematic(t, bo);
            }
        }
    }

    public void TakeDamage(float amount, GunData.gunTypes gunType = GunData.gunTypes.projectile, bool isMelee = false) {
        if (health <= 0) return;
        hitSource.Play();
        GameObject splashTextTemp = Instantiate(splashText, transform.position + new Vector3(Random.Range(-0.5f, .5f), Random.Range(-0.5f, .5f), Random.Range(-0.5f, .5f)), Quaternion.Euler(0, -180, 0));
        SplashText splashTextTempScript = splashTextTemp.GetComponent<SplashText>();
        
        if (isMelee) {
            splashTextTempScript.splashTextSprite.sprite = splashTextTempScript.splashTextSprites[3];
        }
        else {
            switch (gunType) {
                case GunData.gunTypes.raycast:
                    splashTextTempScript.splashTextSprite.sprite = splashTextTempScript.splashTextSprites[0];
                    break;
                case GunData.gunTypes.projectile:
                    splashTextTempScript.splashTextSprite.sprite = splashTextTempScript.splashTextSprites[1];
                    break;
                case GunData.gunTypes.laser:
                    splashTextTempScript.splashTextSprite.sprite = splashTextTempScript.splashTextSprites[2];
                    break;
            }
        }

        cameraShake.StartCoroutine(cameraShake.ShakeCamera(.2f, .8f));
        health -= amount;
        if (health <= 0) {
            StartCoroutine(Death());
        }
    }

    public IEnumerator Death() {
        isDead = true;
        SwitchKinematic(transform, false);
        cameraShake.StartCoroutine(cameraShake.ShakeCamera(.25f, .8f));
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
        yield return null;
    }

    IEnumerator CheckForPlayer() {
        if (gameManager.isActive) {
            playerInRange = false;
            Collider[] col = Physics.OverlapSphere(transform.position, data.playerRadius, playerLayer);
            foreach(Collider c in col) {
                if (c.CompareTag("Player")) {
                    RaycastHit obstructionHit;
                    if (Physics.Raycast(orientation.position, (c.transform.position - transform.position).normalized, out obstructionHit)) {
                        Debug.DrawLine(transform.position, obstructionHit.transform.position, Color.black, 10);
                        if (((1<<obstructionHit.transform.gameObject.layer) & playerLayer) == 0) {
                            Debug.Log("notplayer");
                            break;
                        }
                    }
                    playerInRange = true;
                    player = c.transform;
                    ShootAtPlayer();
                }
            }
        }
        yield return new WaitForSeconds(2f);
        StartCoroutine(CheckForPlayer());
    }

    void ShootAtPlayer() {
        if (gameManager.isActive) {
            Collider[] col = Physics.OverlapSphere(transform.position, data.playerRadius, playerLayer);
            foreach(Collider c in col) {
                if (c.CompareTag("Player")) {
                    RaycastHit obstructionHit;
                    if (Physics.Raycast(orientation.position, (c.transform.position - transform.position).normalized, out obstructionHit)) {
                        if (((1<<obstructionHit.transform.gameObject.layer) & playerLayer) != 0) {
                            playerInRange = true;
                            player = c.transform;
                            break;
                        }
                        else {
                            playerInRange = false;
                        }
                    }
                }
            }
        }

        int randomInt = Random.Range(0, 9);
        if (shootCooldownTimer >= data.shootCooldown && playerInRange && randomInt == 4) {
            if (bulletPrefab != null) {
                GameObject bullet = Instantiate(bulletPrefab, orientation.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody>().AddForce(orientation.forward * 56, ForceMode.Impulse);
            }
            shootCooldownTimer = 0;
        }
    }

    private void Update() {
        if (!gameManager.isActive || isDead) return;
        if (shootCooldownTimer < data.shootCooldown && playerInRange) {
            shootCooldownTimer += 1 * Time.deltaTime;
        }

        ShootAtPlayer();

        if (playerInRange) {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            headAnchor?.LookAt(new Vector3(0, player.position.y, 0));
            head.localRotation = Quaternion.Euler(head.localRotation.x, head.localEulerAngles.y, -headAnchor.localEulerAngles.x);
        }
    }
}

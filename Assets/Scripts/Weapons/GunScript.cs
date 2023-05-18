using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GunScript : MonoBehaviour
{
    public GameManager gameManager;

    public GunData gunData;
    RaycastHit weaponRaycast;
    Transform weaponHit;
    Transform cam;
    [SerializeField] LayerMask enemyLayer;
    GameObject splashText;
    WeaponHolder weaponHolder;

    public float shootTimer = 0;
    public float laserDelayTimer;

    // Projectiles
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletPoint;

    // Plasma
    LineRenderer lr;
    float lrSegments;
    
    // Weapon Sprite for HUD
    public Sprite weaponSprite;

    [SerializeField] AudioSource plasmaCharge;
    [SerializeField] AudioSource plasmaShoot;

    private void Start() {
        weaponHolder = transform.parent.GetComponent<WeaponHolder>();
        cam = weaponHolder.cam;
        splashText = weaponHolder.splashText;
        lr = GetComponent<LineRenderer>();
        if (lr)
            lrSegments = lr.positionCount;
    }

    private void Update() {
        if (SceneManager.sceneCount > 1) return;
        Shooting();
        if (gunData.gunType == GunData.gunTypes.laser)
            bulletPoint.LookAt(cam.position + (cam.forward * gunData.laserRange));
        else if (gunData.gunType == GunData.gunTypes.projectile)
            bulletPoint.LookAt(cam.position + (cam.forward * 20));
    }

    bool firstFire = false;
    void Shooting() {
        if (gunData.gunType == GunData.gunTypes.laser) {
            if (Input.GetMouseButton(0)) {
                if (Physics.SphereCast(cam.position, .4f, cam.forward, out weaponRaycast, gunData.laserRange) && shootTimer >= gunData.chargeTime && laserDelayTimer >= gunData.laserInterval && ((1<<weaponRaycast.transform.gameObject.layer) & enemyLayer) != 0) {
                    Transform tempT = weaponRaycast.transform;

                    while (!tempT.GetComponent<Enemy>()) {
                        tempT = tempT.parent;
                    }

                    Enemy enemy = tempT.GetComponent<Enemy>();
                    enemy.TakeDamage(gunData.damage, gunData.gunType);

                }
                if (shootTimer < gunData.chargeTime)
                    shootTimer += 1 * Time.deltaTime;

                if (laserDelayTimer < gunData.laserInterval) {
                    laserDelayTimer += 1 * Time.deltaTime;
                }
                else {
                    laserDelayTimer = 0;
                }

                // Knockback
                // If been shooting for less than .2 seconds then add knockback;
                if (shootTimer <= laserDelayTimer) {
                    if (laserDelayTimer < .1f) {
                        if (firstFire) {
                            Debug.Log("palsma");
                            if (!plasmaCharge.isPlaying)
                                plasmaCharge.Play();
                            weaponHolder.playerRB.AddForce(-cam.forward * gunData.laserInitialKnockback, ForceMode.Impulse);
                        }
                    }
                    else {
                        firstFire = false;
                    }
                }
                if (shootTimer >= gunData.chargeTime) {
                    weaponHolder.playerRB.AddForce(-cam.forward * gunData.laserKnockback, ForceMode.Force);
                    if (!plasmaShoot.isPlaying)
                        plasmaShoot.Play();
                }

                // Plasma Line Renderer
                if (shootTimer >= gunData.chargeTime && weaponRaycast.point != null) {
                    float newDistance = Vector3.Distance(bulletPoint.position, weaponRaycast.point);
                    lr.enabled = true;
                    // lr.SetPosition(0, bulletPoint.position);
                    // Vector3 lrTargetPos = Vector3.Lerp(lr.GetPosition(1), cam.position + (-transform.forward * gunData.laserRange), 1 * Time.deltaTime * (gunData.laserDrag * lrSegments));
                    // lr.SetPosition(1, bulletPoint.position + (-transform.forward * 6));
                    
                    for (int i = (int)lrSegments - 1; i >= 0; i--) {
                        Vector3 lrTargetPos = Vector3.Lerp(lr.GetPosition(i), bulletPoint.position + (bulletPoint.forward * (newDistance * (i / lrSegments))), (1 * Time.deltaTime * (newDistance * (((lrSegments - i) + gunData.laserDragMultiplier) / lrSegments))) / (Mathf.Ceil(newDistance) * tempFloatTest));
                        lr.SetPosition(i, lrTargetPos);
                    }

                    // need to link i value to how close it is to cams transform forward + laserrange etc

                    // lr.SetPosition(1, lrTargetPos);
                }
            }
            else {
                if (plasmaShoot.isPlaying)
                    plasmaShoot.Stop();
                shootTimer = 0;
                laserDelayTimer = 0;
                firstFire = true;
                lr.enabled = false;
            }

            Debug.Log(plasmaCharge.isPlaying);
        }
        else if (gunData.gunType == GunData.gunTypes.squirtmachine) {
            if (Input.GetMouseButton(0)) {
                GameObject newbullet = Instantiate(bullet, bulletPoint);
                Rigidbody newbulletrb = newbullet.GetComponent<Rigidbody>();
                newbulletrb.AddForce(bulletPoint.forward * gunData.projectileForce, ForceMode.Impulse);
                Destroy(newbullet, 1.2f);
            }
        }
        else if (gunData.gunType == GunData.gunTypes.projectile) {
            if (gunData.auto) {
                if (Input.GetMouseButton(0) && shootTimer >= gunData.projectileInterval) {
                    GameObject newbullet = Instantiate(bullet, bulletPoint);
                    Rigidbody newbulletrb = newbullet.GetComponent<Rigidbody>();
                    newbullet.transform.parent = null;
                    newbulletrb.AddForce(bulletPoint.forward * gunData.projectileForce, ForceMode.Impulse);
                    Destroy(newbullet, 1.2f);
                    shootTimer = 0;
                    Recoil();
                }
            }
            else {
                if (Input.GetMouseButtonDown(0)) {
                    GameObject newbullet = Instantiate(bullet, bulletPoint);
                    Rigidbody newbulletrb = newbullet.GetComponent<Rigidbody>();
                    newbullet.transform.parent = null;
                    newbulletrb.AddForce(bulletPoint.forward * gunData.projectileForce, ForceMode.Impulse);
                    Bullet newbulletScript = newbullet.GetComponent<Bullet>();
                    newbulletScript.damage = gunData.damage;
                    newbulletScript.splashText = splashText;
                    Destroy(newbullet, 1.2f);
                    Recoil();
                }
            }

            if (Input.GetMouseButtonUp(0)) {
                shootTimer = gunData.projectileInterval;
            }

            if (shootTimer < gunData.projectileInterval) {
                shootTimer += 1 * Time.deltaTime;
            }
        }
    }

    public float tempFloatTest;

    void Recoil() {
        cam.parent.localRotation = Quaternion.Euler(cam.localEulerAngles.x + Random.Range(0, gunData.recoilX), cam.localEulerAngles.y + Random.Range(0, gunData.recoilY), cam.localEulerAngles.z);
    }
}

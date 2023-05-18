using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneBomb : MonoBehaviour
{
    SphereCollider sphCol;
    Rigidbody rb;
    public GameObject splashText;

    public float damage;
    public float radius;
    public float explosionForce;

    CameraShake cameraShake;
    [SerializeField] AudioSource explosionSource;
    [SerializeField] AudioSource clickSource;

    private void Start() {
        cameraShake = FindObjectOfType<CameraShake>();
        sphCol = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();

        splashText.GetComponent<SplashText>().splashTextSprite.sprite = splashText.GetComponent<SplashText>().splashTextSprites[2];

        StartCoroutine(Explosion());
    }

    IEnumerator Explosion() {
        clickSource.Play();
        yield return new WaitForSeconds(1f);
        Instantiate(splashText, transform);
        clickSource.Play();
        yield return new WaitForSeconds(.5f);
        Instantiate(splashText, transform);
        clickSource.Play();
        yield return new WaitForSeconds(.5f);

        Collider[] cols = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in cols) {
            
            if (!col.CompareTag("Enemy")) continue;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, (col.transform.position - transform.position).normalized, out hit)) {
                if (!hit.transform.CompareTag("Enemy")) continue;
            }

            Transform tempT = col.transform;
            while (!tempT.GetComponent<Enemy>()) {
                tempT = tempT.parent;
            }
            tempT.GetComponent<Enemy>().TakeDamage(damage);

        }

        explosionSource.Play();
        cameraShake.StartCoroutine(cameraShake.ShakeCamera(.2f, .8f));
        rb.AddExplosionForce(explosionForce, transform.position - new Vector3(0, .5f, 0), radius);
        Destroy(this.gameObject, .4f);
        //sphCol.enabled = true;
    }
}

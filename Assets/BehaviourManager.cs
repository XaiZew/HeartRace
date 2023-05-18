using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourManager : MonoBehaviour
{

    public bool BehavePhase2 = false;
    public float shootTime = 0.5f;
    public float BurstTime = 0.05f;

    bool ReadyToShoot = true;

    public GameObject Player;

    public LayerMask playerLayer;
    public GameObject BulletPrefab;


    // Start is called before the first frame update
    void Start()
    {
        BehavePhase2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        Phase1Behaviour();
    }

    public void ChangePhase()
    {
        BehavePhase2 = true;
    }

    void Phase1Behaviour()
    {
        RaycastHit Hit; 
        transform.LookAt(Player.transform);
        if(ReadyToShoot == true)
        {
            Physics.Raycast(transform.position, transform.forward, out Hit);
            if (((1<<Hit.transform.gameObject.layer) & playerLayer) != 0) {

                StartCoroutine(Shootwait());

            }
        }

    }

    IEnumerator Shootwait()
    {
        ReadyToShoot = false;
        if(BehavePhase2 == true)
        {
            for (int x = 0; x < 3; x++)
                {
                    ShootProjectile();
                    yield return new WaitForSeconds(BurstTime);
                }
            yield return new WaitForSeconds(shootTime);
            ReadyToShoot = true;
        }
        else
        {
            ShootProjectile();
            yield return new WaitForSeconds(shootTime);
            ReadyToShoot = true;
        }
        
    }
    

    void ShootProjectile()
    {
        GameObject bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 56, ForceMode.Impulse);
    }


}

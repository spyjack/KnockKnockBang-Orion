using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField]
    EnemyController enemyBrains = null;

    [SerializeField]
    GunGraphics enemyGun = null;

    [SerializeField]
    float enemyAccuracy = 0.25f;

    [SerializeField]
    float enemyDamage = 1f;

    [SerializeField]
    float enemyCooldown = 0.5f;

    [SerializeField]
    float scanFrequency = 0.25f;

    [SerializeField]
    float fieldOfViewAngle = 90f;

    [SerializeField]
    float rotationSpeed = 2f;

    bool isAngry = false;
    bool canShoot = true;
    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(CheckFieldOfViewRepeated());
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyBrains.Dead)
        {
            StopAllCoroutines();
            return;
        }

        if (isAngry && !enemyBrains.Ragdolled)
        {
            RotateTowardsPlayer();

            if (CheckAngle(45f))
            {
                ShootAtPlayer();
            }
        }


    }

    void RotateTowardsPlayer()
    {
        Vector3 playerDirection = enemyBrains.Target.transform.position - transform.position;
        playerDirection.y = 0;
        Quaternion rotationTowardsPlayer = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotationTowardsPlayer, rotationSpeed * Time.deltaTime);
    }

    void ShootAtPlayer()
    {
        if (!canShoot)
            return;

        RaycastHit hit;
        Vector3 castDirection = transform.TransformDirection(new Vector3(Random.Range(-enemyAccuracy, enemyAccuracy), Random.Range(-enemyAccuracy, enemyAccuracy), 1));
        if (Physics.Raycast(enemyGun.transform.position, castDirection, out hit, 100f))
        {
            if (hit.transform.tag == "Player")
            {
                hit.transform.GetComponentInParent<PlayerController>().TakeDamage(enemyDamage);
            }
            enemyGun.TraceBullet(hit.point);
        }
        else
        {
            enemyGun.TraceBullet(castDirection * 100f);
        }
        enemyGun.DoMuzzleFlash();
        enemyBrains.Animator.SetTrigger("Shoot");
        canShoot = false;
        StartCoroutine(EnemyShootCooldown());
    }

    bool CheckAngle(float _maxAngle)
    {
        Vector3 _direction = Vector3.Normalize(enemyBrains.Target.transform.position - this.transform.position);
        float _angle = Vector3.Angle(_direction, transform.forward);

        if (_angle <= _maxAngle)
        {
            return true;
        }
        return false;
    }

    void ScanFieldOfView()
    {
        if (CheckAngle(fieldOfViewAngle))
        {
            isAngry = true;
        }
    }

    public void GetAngry()
    {
        isAngry = true;
    }

    IEnumerator CheckFieldOfViewRepeated()
    {
        while(true)
        {
            ScanFieldOfView();
            yield return new WaitForSeconds(scanFrequency);
            if (isAngry)
                yield break;
        }
    }

    IEnumerator EnemyShootCooldown()
    {
        yield return new WaitForSeconds(enemyCooldown);
        canShoot = true;
        yield break;
    }
}

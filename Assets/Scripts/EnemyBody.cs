using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    [SerializeField]
    EnemyController enemyBrains = null;

    [SerializeField]
    Rigidbody enemyTorsoBody = null;

    [SerializeField]
    Rigidbody[] enemyBodyParts;

    [SerializeField]
    Vector3 desiredPosition = Vector3.zero;

    bool doGetBackUp = false;
    bool doGetBackUpTimer = false;
    bool ragdolling = false;

    public float rotationspeed = 0.1F;
    public float movementspeed = 0.1F;

    // Start is called before the first frame update
    void Start()
    {
        desiredPosition = this.transform.position;
        DisableGravity();
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.relativeVelocity.magnitude);
        if ((collision.transform.tag == "Kickable" || collision.transform.gameObject.tag == "Door") && enemyBrains.ImpluseResistence < collision.relativeVelocity.magnitude)
        {
            enemyBrains.EnabledRagdoll();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyBrains.Dead)
            return;

        if (doGetBackUp && Vector3.Distance(transform.localPosition, transform.TransformVector(Vector3.zero)) > 0.1f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, transform.TransformVector(Vector3.zero), Time.deltaTime * movementspeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0,0,0), Time.deltaTime * rotationspeed);
        }else if (doGetBackUp && Vector3.Distance(transform.localPosition, transform.TransformVector(Vector3.zero)) <= 0.1f)
        {
            doGetBackUp = false;
            enemyBrains.DisabledRagdoll();
        }
    }

    public void GetBackUpStarter()
    {
        if (enemyBrains.Dead)
            return;

        doGetBackUp = false;
        enemyBrains.EnabledRagdoll();

        if (doGetBackUpTimer)
        {
            StopCoroutine(GetBackUpTimer());
        }
        else if (!doGetBackUpTimer)
        {
            StartCoroutine(GetBackUpTimer());
        }
    }

    public void EnableGravity()
    {
        ragdolling = true;

        enemyTorsoBody.useGravity = true;
        enemyTorsoBody.isKinematic = false;

        if (enemyBodyParts.Length > 0)
        {
            foreach (Rigidbody body in enemyBodyParts)
            {
                body.useGravity = true;
                body.isKinematic = false;
            }
        }
    }

    public void DisableGravity()
    {
        ragdolling = false;

        enemyTorsoBody.useGravity = false;
        enemyTorsoBody.isKinematic = true;

        if (enemyBodyParts.Length > 0)
        {
            foreach (Rigidbody body in enemyBodyParts)
            {
                body.useGravity = false;
                body.isKinematic = true;
            }
        }
    }

    IEnumerator GetBackUpTimer()
    {
        doGetBackUpTimer = true;
        yield return new WaitForSeconds(5f);

        if (enemyBrains.Dead)
            yield break;

        doGetBackUp = true;
        doGetBackUpTimer = false;
        enemyTorsoBody.useGravity = false;
        enemyTorsoBody.isKinematic = true;
        yield break;
    }
}

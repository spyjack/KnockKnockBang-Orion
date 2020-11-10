using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChildColliders : MonoBehaviour
{
    [SerializeField]
    EnemyController enemyBrains = null;

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.relativeVelocity.magnitude);
        if ((collision.transform.tag == "Kickable" || collision.transform.gameObject.tag == "Door") && enemyBrains.ImpluseResistence < collision.relativeVelocity.magnitude)
        {
            enemyBrains.EnabledRagdoll();
        }
    }
}

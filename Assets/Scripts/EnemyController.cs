using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    Animator enemyAnimator = null;
    [SerializeField]
    EnemyBody enemyBody = null;
    [SerializeField]
    EnemyCombat enemyCombat = null;
    [SerializeField]
    PlayerController playerTarget = null;

    [SerializeField]
    float healthMax = 10f;
    [SerializeField]
    float currentHealth = 10f;

    [SerializeField]
    float impulseResistence = 50f;

    bool isDead = false;
    bool isRagdolled = false;

    public Animator Animator
    {
        get { return enemyAnimator; }
    }

    public float ImpluseResistence
    {
        get { return impulseResistence; }
    }

    public bool Dead
    {
        get { return isDead; }
    }
    public bool Ragdolled
    {
        get { return isRagdolled; }
    }
    public PlayerController Target
    {
        get { return playerTarget; }
    }
    // Start is called before the first frame update
    void OnAwake()
    {
        playerTarget = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void TakeDamage(float _damage)
    {
        if (isDead)
            return;

        currentHealth -= _damage;
        enemyCombat.GetAngry();
        if (currentHealth <= 0)
            Died();
    }

    void Died()
    {
        isDead = true;
        EnabledRagdoll();
        playerTarget.UpdateKillsLeft(1);
    }

    public void EnabledRagdoll()
    {
        enemyAnimator.enabled = false;
        isRagdolled = true;
        enemyBody.EnableGravity();
    }

    public void DisabledRagdoll()
    {
        enemyAnimator.enabled = true;
        isRagdolled = false;
        enemyBody.DisableGravity();
    }
}

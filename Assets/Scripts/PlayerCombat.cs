using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController = null;

    [SerializeField]
    float slowMoTimer = 0.5f;

    [SerializeField]
    float slowMoScale = 0.25f;

    [SerializeField]
    float kickDamage = 0.5f;

    [SerializeField]
    float kickStrength = 100f;

    [SerializeField]
    float kickRange = 2f;

    [SerializeField]
    float gunDamage = 3f;

    [SerializeField]
    float gunStrength = 200f;

    [SerializeField]
    float gunRange = 100f;

    [SerializeField]
    float gunAccuracy = 0.25f;

    [SerializeField]
    float cooldown = 0.2f;

    [SerializeField]
    float sustainedFireResetTime = 3;

    float sustainedFirePoints = 1;
    float sustainedFireTime = 0;

    [SerializeField]
    PostProcessProfile effectProfile = null;
    Vignette vignetteSetting = null;
    ChromaticAberration chromaticAberrationSetting = null;

    bool[] canShoot = new bool[2] { true, true };

    [SerializeField]
    GunGraphics[] gunGraphics = new GunGraphics[2];

    // Start is called before the first frame update
    void Start()
    {
        effectProfile.TryGetSettings(out vignetteSetting);
        effectProfile.TryGetSettings(out chromaticAberrationSetting);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.Dead)
            return;

        if (Input.GetButton("Fire1"))
            FireGun(0);

        if (Input.GetButton("Fire2"))
            FireGun(1);

        if (Input.GetButtonDown("Jump"))
            Kick();

        DoSustainedFireTimer();
    }

    void DoSustainedFireTimer()
    {
        if (sustainedFireTime > 0)
        {
            sustainedFireTime -= Time.deltaTime;
        }else if (sustainedFireTime <= 0 && sustainedFirePoints > 1)
        {
            sustainedFireTime = 0;
            sustainedFirePoints = 1;
            
            print("Reset Accuracy Bonus");
        }

        if (vignetteSetting != null && vignetteSetting.intensity.value > 0 && sustainedFireTime <= 0)
            vignetteSetting.intensity.value = Mathf.Lerp(vignetteSetting.intensity.value, 0, Time.deltaTime * 2f);
        if (chromaticAberrationSetting != null && chromaticAberrationSetting.intensity.value > 0 && sustainedFireTime <= 0)
            chromaticAberrationSetting.intensity.value = Mathf.Lerp(chromaticAberrationSetting.intensity.value, 0, Time.deltaTime * 2f);
    }

    void Kick()
    {
        playerController.Animator.SetTrigger("Kick");
        Vector3 castDirection = playerController.Looker.transform.TransformDirection(Vector3.forward);
        RaycastHit[] hits = Physics.BoxCastAll(transform.position, Vector3.one, castDirection, transform.rotation, kickRange);
        foreach (RaycastHit collision in hits)
        {
            if (collision.transform.tag == "Kickable")
            {
                collision.transform.GetComponent<Rigidbody>().AddForceAtPosition(transform.TransformDirection(Vector3.forward * kickStrength), collision.point);
            }else if (collision.transform.tag == "Door")
            {
                Rigidbody doorBody = collision.transform.GetComponent<Rigidbody>();
                if (doorBody.isKinematic == true)
                {
                    AddFireFocus(10);

                    StartCoroutine(SlowTime());
                }

                doorBody.isKinematic = false;
                doorBody.useGravity = true;
                doorBody.AddForceAtPosition(transform.TransformDirection(Vector3.forward * kickStrength), collision.point);
            }else if (collision.transform.tag == "EnemyTorso")
            {
                print("Kicking Enemy");
                collision.transform.GetComponentInParent<EnemyController>().TakeDamage(kickDamage);
                collision.transform.GetComponent<EnemyBody>().GetBackUpStarter();
                collision.transform.GetComponent<Rigidbody>().AddForceAtPosition(transform.TransformDirection(Vector3.forward * kickStrength * 3), collision.point);
            }
        }
    }

    void FireGun(int _index)
    {
        if (canShoot[_index] == false)
            return;
            
        canShoot[_index] = false;
        if (_index == 1)
        {
            playerController.Animator.SetTrigger("ShootRight");
            StartCoroutine(RightCooldown());
        }else
        {
            playerController.Animator.SetTrigger("ShootLeft");
            StartCoroutine(LeftCooldown());
        }
        gunGraphics[_index].DoMuzzleFlash();
        RaycastHit hit;
        Vector3 castDirection = playerController.Looker.transform.TransformDirection(new Vector3(Random.Range(-gunAccuracy, gunAccuracy) * 1 / sustainedFirePoints, Random.Range(-gunAccuracy, gunAccuracy) * 1 / sustainedFirePoints, 1));
        if (Physics.Raycast(playerController.Looker.transform.position, castDirection, out hit, gunRange))
        {
            if (hit.transform.tag == "Kickable")
            {
                hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(transform.TransformDirection(Vector3.forward * gunStrength), hit.point);
            }
            else if (hit.transform.tag == "Enemy" || hit.transform.tag == "EnemyTorso")
            {
                hit.transform.GetComponentInParent<EnemyController>().TakeDamage(gunDamage);
                hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(transform.TransformDirection(Vector3.forward * gunStrength), hit.point);
            }
        }
        AddFireFocus(1);
        gunGraphics[_index].TraceBullet(playerController.Looker.transform.position + castDirection * gunRange);
    }

    void AddFireFocus(float _amount)
    {
        if (sustainedFireTime < sustainedFireResetTime)
            sustainedFireTime++;
        sustainedFirePoints += _amount;
        if (vignetteSetting != null)
            vignetteSetting.intensity.value = Mathf.Clamp(sustainedFirePoints / 30f, 0f, 0.75f);
        if (chromaticAberrationSetting != null)
            chromaticAberrationSetting.intensity.value = Mathf.Clamp(sustainedFirePoints / 30f, 0f, 2f);
    }

    IEnumerator LeftCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot[0] = true;
        yield break;
    }

    IEnumerator RightCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        canShoot[1] = true;
        yield break;
    }

    IEnumerator SlowTime()
    {
        Time.timeScale = slowMoScale;
        yield return new WaitForSeconds(slowMoTimer * slowMoScale);
        Time.timeScale = 1;
    }
}

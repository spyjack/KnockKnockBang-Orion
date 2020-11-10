using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunGraphics : MonoBehaviour
{
    [SerializeField]
    LineRenderer bulletRenderer = null;

    [SerializeField]
    ParticleSystem muzzleFlashParticle = null;

    [SerializeField]
    Light muzzleLight = null;

    [SerializeField]
    float trailDuration = 0.2f;

    public void TraceBullet(Vector3 endPosition)
    {
        bulletRenderer.enabled = true;
        bulletRenderer.SetPosition(0, bulletRenderer.transform.position);
        bulletRenderer.SetPosition(1, endPosition);
        StartCoroutine(FadeBulletTrail());
    }

    public void DoMuzzleFlash()
    {
        StartCoroutine(MuzzleLightToggle());
        muzzleFlashParticle.Play();
    }

    IEnumerator MuzzleLightToggle()
    {
        muzzleLight.enabled = true;
        yield return new WaitForSeconds(0.1f);
        muzzleLight.enabled = false;
        yield break;
    }

    IEnumerator FadeBulletTrail()
    {
        yield return new WaitForSeconds(trailDuration);
        bulletRenderer.enabled = false;
        yield break;
    }
}

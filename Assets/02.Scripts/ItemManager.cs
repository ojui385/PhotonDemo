using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject collFx;

    private new AudioSource audio;
    public AudioClip collSFX;

    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        // Item Destroy
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        Destroy(this.gameObject, 3.0f);

        // 이펙트
        GameObject tempFx = Instantiate(collFx, this.transform.position, Quaternion.LookRotation(Vector3.up));
        Destroy(tempFx, 2.0f);

        // 소리
        audio.PlayOneShot(collSFX);
    }
}

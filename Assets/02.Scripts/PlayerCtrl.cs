using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityStandardAssets.Utility;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviourPunCallbacks
{
    private new Rigidbody rigidbody;
    private PhotonView pv;

    private float v;
    private float h;
    private float r;

    [Header("이동 및 회전 속도")]
    public float moveSpeed = 8.0f;
    public float turnSpeed = 200.0f;
    public float jumpPower = 5.0f;
    private float turnSpeedValue = 0.0f;

    private RaycastHit hit;

    public Material[] playerMt;
    private int idxMt = -1;

    IEnumerator Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();

        turnSpeedValue = turnSpeed;
        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.5f);

        if (pv.IsMine)
        {
            Camera.main.GetComponent<SmoothFollow>().target = transform.Find("CamPivot").transform;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }


        turnSpeed = turnSpeedValue;
    }

    // 키 입력
    void Update()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        r = Input.GetAxis("Mouse X");

        Debug.DrawRay(transform.position, -transform.up * 0.6f, Color.green);
        if (Input.GetKeyDown("space"))
        {
            if (Physics.Raycast(transform.position, -transform.up, out hit, 0.6f))
            {
                rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }

    }

    // 물리적 처리
    void FixedUpdate()
    {
        if (pv.IsMine)
        {
            Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
            transform.Translate(dir.normalized * Time.deltaTime * moveSpeed, Space.Self);
            transform.Rotate(Vector3.up * Time.smoothDeltaTime * turnSpeed * r);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        string coll = other.gameObject.name;
        switch (coll)
        {
            case "Item_1":
                idxMt = 0;
                pv.RPC(nameof(SetMt), RpcTarget.AllViaServer, idxMt);
                break;
            case "Item_2":
                idxMt = 1;
                pv.RPC(nameof(SetMt), RpcTarget.AllViaServer, idxMt);
                break;
            case "Item_3":
                idxMt = 2;
                pv.RPC(nameof(SetMt), RpcTarget.AllViaServer, idxMt);
                break;
        }

    }

    [PunRPC]
    private void SetMt(int idx)
    {
        GameManager.instance.GetScore(pv.ViewID / 1000);
        GetComponent<Renderer>().material = playerMt[idx];
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (pv.IsMine && idxMt != -1)
        {
            pv.RPC(nameof(SetMt), newPlayer, idxMt);
        }
    }

}
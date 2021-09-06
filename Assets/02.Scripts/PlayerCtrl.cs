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

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }

    IEnumerator Start()
    {

        turnSpeedValue = turnSpeed;
        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.5f);

        if (pv.IsMine)
        {
            Camera.main.GetComponent<SmoothFollow>().target = transform.Find("CamPivot").transform;
        }
        else
        {
            // GetComponent<Rigidbody>().isKinematic = true;
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
        if (!pv.IsMine)
        {
            return;
        }
        string coll = other.gameObject.name;
        switch (coll)
        {
            case "Item_1":
                ScorePoint(0);
                break;
            case "Item_2":
                ScorePoint(1);
                break;
            case "Item_3":
                ScorePoint(2);
                break;
        }

    }

    public void InitColor(int num)
    {
        pv.RPC(nameof(SetMt), RpcTarget.AllViaServer, num);
    }

    [PunRPC]
    public void SetMt(int idx)
    {
        GetComponent<Renderer>().material = playerMt[idx];
        idxMt = idx;
    }

    public void ScorePoint(int idx)
    {
        pv.RPC(nameof(SetMt), RpcTarget.AllViaServer, idx);
        GameManager.instance.GetScore(pv.ViewID / 1000);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (pv.IsMine && idxMt != -1)
        {
            pv.RPC(nameof(SetMt), newPlayer, idxMt);
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public PhotonView pv;

    public bool isConnect = false;
    public Transform[] spawnPoints;

    public Text scorePlayer1Text;
    public Text scorePlayer2Text;

    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        pv = GetComponent<PhotonView>();
        StartCoroutine(CreatePlayer());
    }

    void Update()
    {


    }

    IEnumerator CreatePlayer()
    {
        yield return new WaitUntil(() => isConnect);

        spawnPoints = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();

        Vector3 pos = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].position;
        Quaternion rot = spawnPoints[PhotonNetwork.CurrentRoom.PlayerCount].rotation;

        GameObject playerTemp = PhotonNetwork.Instantiate("Player", pos, rot, 0);
    }

    public void GetScore(int playerNum)
    {
        pv.RPC(nameof(SetScore), RpcTarget.AllViaServer, playerNum);
    }

    [PunRPC]
    public void SetScore(int playerNum)
    {
        if (playerNum == 1)
        {
            scorePlayer1++;
            scorePlayer1Text.text = $"{scorePlayer1:00}";
        }
        else
        {
            scorePlayer2++;
            scorePlayer2Text.text = $"{scorePlayer2:00}";
        }
    }
}

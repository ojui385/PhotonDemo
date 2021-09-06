using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0";
    private string userId = "Ojui";

    private void Awake()
    {
        // 방장이 혼자 씬을 로딩하면, 나머지 사람들은 자동으로 싱크가 됨
        PhotonNetwork.AutomaticallySyncScene = true;

        // 게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;

        // 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    void Start()
    {
        Debug.Log("00. 포톤 매니저 시작");
        PhotonNetwork.NickName = userId;
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("01. 포톤 서버에 접속");

    }

    public void OnStartBtn()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("02. 랜덤 룸 접속 실패");

        // 룸 속성 설정
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 30;


        // 룸을 생성 > 자동 입장됨
        PhotonNetwork.CreateRoom("room_1", ro);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("03. 방 생성 완료");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("04. 방 입장 완료");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Level_1");
        }
    }

}
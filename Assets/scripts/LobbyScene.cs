using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using ExitGames.Client.Photon;

public class LobbyScene : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerCountText; // TextMeshProUGUIの参照
    public Button startButton; // スタートボタンの参照

    private void Start()
    {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CustomRoomProperties = new Hashtable { { "canJoin", true } }; // カスタムプロパティを設定
        PhotonNetwork.JoinOrCreateRoom("Room", roomOptions, TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        // カスタムプロパティ "canJoin" を確認
        bool canJoin = (bool)PhotonNetwork.CurrentRoom.CustomProperties["canJoin"];
        if (!canJoin)
        {
            // ルームに入室できない場合、退出する
            PhotonNetwork.LeaveRoom();
            return;
        }

        UpdatePlayerCountText();

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.interactable = true;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCountText();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCountText();
        if (PhotonNetwork.IsMasterClient)
        {
            startButton.interactable = true;
        }
    }

    private void UpdatePlayerCountText()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        playerCountText.text = "Players in Room: " + playerCount;
    }

    public void SwitchNextScene()
    {
        // カスタムプロパティ "canJoin" を false に設定
        Hashtable customProperties = new Hashtable { { "canJoin", false } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);

        photonView.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}

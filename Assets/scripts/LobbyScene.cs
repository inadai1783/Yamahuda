using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshProUGUIを使用するためのディレクティブ


// MonoBehaviourPunCallbacksを継承して、PUNのコールバックを受け取れるようにする
public class LobbyScene : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerCountText; // TextMeshProUGUIの参照
    private void Start() {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster() {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom() {
        UpdatePlayerCountText();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCountText();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCountText();
    }

    private void UpdatePlayerCountText()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        playerCountText.text = "Players: " + playerCount;
    }

    public void SwitchNextScene()
    {
        photonView.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
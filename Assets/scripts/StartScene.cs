using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using ExitGames.Client.Photon;

public class StartScene : MonoBehaviourPunCallbacks
{
    private bool isMyCardFlip = true;
    private bool canJoin = true;
    public TextMeshProUGUI playerCountText; // TextMeshProUGUIの参照
    public Button startButton; // スタートボタンの参照
    public Toggle toggleTRUE;
    public Toggle toggleFALSE;

    private void Start()
    {
        // PhotonServerSettingsの設定内容を使ってマスターサーバーへ接続する
        PhotonNetwork.ConnectUsingSettings();
    }

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        // "Room"という名前のルームに参加する（ルームが存在しなければ作成して参加する）
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions(), TypedLobby.Default);
    }

    // ゲームサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            UpdataCustomProp();
            interactableAllButton();
        }
        else
        {
            // カスタムプロパティ "canJoin" を確認
            bool canJoin = (bool)PhotonNetwork.CurrentRoom.CustomProperties["canJoin"];
            if (!canJoin)
            {
                // ルームに入室できない場合、退出する
                PhotonNetwork.LeaveRoom();
                return;
            }
        }

        UpdatePlayerCountText();
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
            interactableAllButton();
        }
    }

    private void UpdatePlayerCountText()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        playerCountText.text = "Players in Room: " + playerCount;
    }

    public void ToggleChanged()
    {
        if (toggleTRUE.isOn)
        {
            isMyCardFlip = true; // カードを裏にする
            UpdataCustomProp();
        }
        else
        {
            isMyCardFlip = false; // カードを表にする
            UpdataCustomProp();
        }
    }


    public void SwitchNextScene()
    {
        // カスタムプロパティ "canJoin" を false に設定
        canJoin = false;
        UpdataCustomProp();
        photonView.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void UpdataCustomProp()
    {
        Hashtable customProperties = new Hashtable { { "canJoin", canJoin }, { "isMyCardFlip", isMyCardFlip} };
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);

    }

    public void interactableAllButton()
    {
        startButton.interactable = true;
        toggleTRUE.interactable = true;
        toggleFALSE.interactable = true;

    }
}

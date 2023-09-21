using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CardManager : MonoBehaviourPunCallbacks
{
    private int myPlayerID; //自身のプレイヤーID
    private int cardNum = 10; //カードの枚数
    private int members; //プレイヤーの人数
    private bool isDraw = false; //一枚引いたか
    private bool isMyCardFlip;
    private bool finishDealing = false;
    private List<GameObject> flippedCards = new List<GameObject>(); //　裏を向いているカードのリスト
    public GameObject Deck;
    public Button restartButton;
    public Button finishButton;
    public List<int> deck; // 共通の山札
    public GameObject CardPrefab; //引いたカード
    public Dictionary<int, int> playerCards; // プレイヤーIDとカード番号の対応

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            restartButton.interactable = true;
            finishButton.interactable  =true;
        }
        isMyCardFlip = (bool)PhotonNetwork.CurrentRoom.CustomProperties["isMyCardFlip"];
        myPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
        members = PhotonNetwork.CurrentRoom.PlayerCount;
        playerCards = new Dictionary<int, int>();
        InitializeDeck(); // 山札を初期化
    }

    void InitializeDeck() //山札の初期化
    {
        deck = new List<int>();
        for (int i = 1; i <= cardNum; i++)
        {
            deck.Add(i);
        }
        CreateDeck();
    }

    void CreateDeck()
    {
        for(int i = 0; i < cardNum; i++)
        {
            Vector3 position = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), -i * 0.1f);
            GameObject card = Instantiate(CardPrefab, position, Quaternion.identity, Deck.transform);
            card.transform.Rotate(0f, 180f, 0f);
        }
    }

    public void OnDeckClicked()
    {
        if(!isDraw)
        {
            photonView.RPC("DealCard", RpcTarget.MasterClient, myPlayerID); // マスタークライアントにカードを引いてもらう
            isDraw = true;
        }
    }

    public void OnRestartClicked()
    {
        photonView.RPC("Restart", RpcTarget.All);
    }

    [PunRPC]
    void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void OnFinishClicked()
    {
        if(finishDealing)
        {
            photonView.RPC("Finish", RpcTarget.All);
            finishButton.interactable  = false;
        }
    }

    [PunRPC]
    void Finish()
    {
        foreach(var cardInstance in flippedCards)
        {
            Animator anime = cardInstance.GetComponent<Animator>();
            anime.SetBool("isFlipped", true);
        }
    }

    [PunRPC]
    void DealCard(int playerID)
    {
        int number = getCard(); //ランダムなインデックス獲得
        photonView.RPC("SendCardInfo", RpcTarget.All, playerID, number);
    }

    int getCard()
    {
        int randomIndex = Random.Range(0, deck.Count); // ランダムな位置からカードを選択
        int cardNumber = deck[randomIndex]; // 選択したカードを取得
        deck.RemoveAt(randomIndex); // カードをリストから削除
        return cardNumber;
    }

    [PunRPC]
    void makeCardInstance(Vector3 position, int playerID, bool isMine)
    {
        // カードプレハブをインスタンス化して画面上に表示
        GameObject cardInstance = Instantiate(CardPrefab, position, Quaternion.identity);
        // カードの値をTextMeshProに設定
        TextMeshProUGUI cardText = cardInstance.GetComponentInChildren<TextMeshProUGUI>();
        cardText.text = playerCards[playerID].ToString();

        // isFlipp が true の場合、オブジェクトをy軸で180度回転させる
        if (isMine) //自分のカード=山札から引く(ismycardflipがfalseなら引いてから裏返す)
        {
            cardInstance.transform.Rotate(0f, 180f, 0f); //裏返しの状態にする
            Animator anime = cardInstance.GetComponent<Animator>();
            anime.SetBool("isClicked", true);
            if(!isMyCardFlip) //裏向きじゃない=表向き
            {
                anime.SetBool("isFlipped", true);
            }else
            {
                flippedCards.Add(cardInstance);
            }
        }else //その他のカード
        {
            cardInstance.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            if(!isMyCardFlip) //自分のカードを表向き=その他のカードは裏向き
            {
                cardInstance.transform.Rotate(0f, 180f, 0f);
                flippedCards.Add(cardInstance);
            }
        }
    }

    [PunRPC]
    private void SendCardInfo(int playerID, int cardNumber)
    {
        playerCards[playerID] = cardNumber;
        if(myPlayerID == playerID) //もし自分のカードの更新であればインスタンス生成
        {
            // makeCardInstance(new Vector3(0f,-7f,0f), myPlayerID, true, isMyCardFlip);
            makeCardInstance(new Vector3(0f,0f,0f), myPlayerID, true);
        }
        if(members == playerCards.Count)
        {
            ViewOtherCards();
            finishDealing = true;
        }
    }
    void ViewOtherCards()
    {
        float space = 20f / members;
        Vector3 position = new Vector3(space - 10f, 7f, 0f); // カードの表示開始位置

        foreach (var kvp in playerCards)
        {
            int playerID = kvp.Key;
            int cardNumber = kvp.Value;

            if (playerID != myPlayerID)
            {
                makeCardInstance(position, playerID, false);
                // カードの位置を調整して横並びに表示
                position.x += space;
            }
        }
    }

    // マスタークライアントがルームを抜けたときに呼び出されるコールバック
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // 新しいマスタークライアントが自分の場合、Finish ボタンと Restart ボタンを有効にする
        if (newMasterClient.ActorNumber == myPlayerID)
        {
            restartButton.interactable = true;
            finishButton.interactable = true;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CardManager : MonoBehaviourPun
{
    private int myPlayerID; //自信のプレイヤーID
    private int cardNum = 10; //カードの枚数
    private int members; //プレイヤーの人数
    private bool isDraw;
    public List<int> deck; // 共通の山札
    public GameObject CardPrefab; //引いたカード
    public Dictionary<int, int> playerCards; // プレイヤーIDとカード番号の対応

    void Start()
    {
        isDraw = false;
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
    }

    public void OnDeckClicked()
    {
        if(!isDraw)
        {
            photonView.RPC("DealCard", RpcTarget.MasterClient, myPlayerID); // マスタークライアントにカードを引いてもらう
            isDraw = true;
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
    void makeCardInstance(Vector3 posion, int playerID, bool isFlipp)
    {
        // カードプレハブをインスタンス化して画面上に表示
        GameObject cardInstance = Instantiate(CardPrefab, posion, Quaternion.identity);
        // カードの値をTextMeshProに設定
        TextMeshProUGUI cardText = cardInstance.GetComponentInChildren<TextMeshProUGUI>();
        cardText.text = playerCards[playerID].ToString();

        // isFlipp が true の場合、オブジェクトをy軸で180度回転させる
        if (isFlipp)
        {
            cardInstance.transform.Rotate(0f, 180f, 0f);
            cardInstance.transform.localScale = new Vector3(0.5f,0.5f,1f);
        }

    }

    [PunRPC]
    private void SendCardInfo(int playerID, int cardNumber)
    {
        playerCards[playerID] = cardNumber;
        if(myPlayerID == playerID) //もし自分のカードの更新であればインスタンス生成
        {
            makeCardInstance(new Vector3(0f,-7f,0f), myPlayerID, false);
        }
        if(members == playerCards.Count)
        {
            ViewOtherCards();
        }
    }
    void ViewOtherCards()
    {
        float space = 20f / members - 10f;
        Vector3 posion = new Vector3(space, 7f, 0f); // カードの表示開始位置

        foreach (var kvp in playerCards)
        {
            int playerID = kvp.Key;
            int cardNumber = kvp.Value;

            if (playerID != myPlayerID)
            {
                makeCardInstance(posion, playerID, true);
                // カードの位置を調整して横並びに表示
                posion.x += space;
            }
        }
    }
}
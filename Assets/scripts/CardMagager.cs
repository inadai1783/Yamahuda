using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class CardManager : MonoBehaviourPun
{
    public List<int> deck; // 共通の山札
    private int cardNum = 10; //カードの枚数
    private int member = 5; //プレイヤーの人数
    public GameObject CardPrefab; //引いたカード
    public Vector3 cardPosion;

    void Start()
    {
        InitializeDeck(); // 山札を初期化
        DealCards(); // カードをプレイヤーに配布
    }

    void InitializeDeck() //山札の初期化
    {
        deck = new List<int>();
        for (int i = 1; i <= cardNum; i++)
        {
            deck.Add(i);
        }
    }

    void DealCards()
    {
            cardPosion = new Vector3(0f, 0f, 0f);
            int cardNum = getCard();
            makeCardInstance(cardNum, cardPosion, false);
            Debug.Log("Player received card: " + cardNum);
    }
    
        //    cardPosion.x = 20f * (float)i / (float)member - 10f; その他のカードを並べるときの座標参考
        //    cardPosion.y = 5f;


    int getCard()
    {
        int randomIndex = Random.Range(0, deck.Count); // ランダムな位置からカードを選択
        int cardNum = deck[randomIndex]; // 選択したカードを取得
        deck.RemoveAt(randomIndex); // カードをリストから削除
        //SendDeckData(); //山札の同期
        return cardNum;
    }

    void makeCardInstance(int cardNum, Vector3 cardPosion, bool isFlipp)
    {
        // カードプレハブをインスタンス化して画面上に表示
        GameObject cardInstance = PhotonNetwork.Instantiate("Card", cardPosion, Quaternion.identity);
        // カードの値をTextMeshProに設定
        TextMeshProUGUI cardText = cardInstance.GetComponentInChildren<TextMeshProUGUI>();
        cardText.text = cardNum.ToString();

            // isFlipp が true の場合、オブジェクトをy軸で180度回転させる
        if (isFlipp)
        {
            cardInstance.transform.Rotate(0f, 180f, 0f);
            cardInstance.transform.localScale = new Vector3(0.5f,0.5f,1f);
        }
    }

    // public void SendDeckData()
    // {
    //    photonView.RPC("SyncDeck", RpcTarget.Others, deck); 
    // }

    // [PunRPC]
    // public void SyncDeck(List<int> newDeck)
    // {
    //     deck = newDeck;
    // }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardManager : MonoBehaviour
{
    public List<int> deck; // 共通の山札
    private int cardNum = 10; //カードの枚数
    private int member = 5; //プレイヤーの人数
    public GameObject Card; //引いたカード
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
        // 各プレイヤーにカードを配布するロジックを実装
        for (int i = 0; i < member; i++)
        {
            setPosion(i);
            int cardNum = getCard();
            makeCardInstance(cardNum, cardPosion, i == 0);
            Debug.Log("Player " + (i + 1) + " received card: " + cardNum);
        }
    }

    void setPosion(int i){
        if(i == 0)
        {
            cardPosion = new Vector3(0f, -5f);
        }else
        {
           cardPosion.x = 20f * (float)i / (float)member - 10f;
           cardPosion.y = 5f;
        }
    }

    int getCard()
    {
        int randomIndex = Random.Range(0, deck.Count); // ランダムな位置からカードを選択
        int cardNum = deck[randomIndex]; // 選択したカードを取得
        deck.RemoveAt(randomIndex); // カードをリストから削除
        return cardNum;
    }

    void makeCardInstance(int cardNum, Vector3 cardPosion, bool isMine)
    {
        // カードプレハブをインスタンス化して画面上に表示
        GameObject cardInstance = Instantiate(Card, cardPosion, Quaternion.identity);
        // カードの値をTextMeshProに設定
        TextMeshProUGUI cardText = cardInstance.GetComponentInChildren<TextMeshProUGUI>();
        cardText.text = cardNum.ToString();

            // isFlipp が true の場合、オブジェクトをy軸で180度回転させる
        if (!isMine)
        {
            cardInstance.transform.Rotate(0f, 180f, 0f);
            cardInstance.transform.localScale = new Vector3(0.5f,0.5f,1f);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardManager : MonoBehaviour
{
    public List<int> deck; // 共通の山札
    private int cardNum = 10; //カードの枚数
    private int member = 1; //プレイヤーの人数
    public GameObject Card; //引いたカード

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
        int numPlayers = GetNumberOfPlayers(); // プレイヤー数を取得
        for (int i = 0; i < numPlayers; i++)
        {
            int randomIndex = Random.Range(0, deck.Count); // ランダムな位置からカードを選択
            int card = deck[randomIndex]; // 選択したカードを取得
            deck.RemoveAt(randomIndex); // カードをリストから削除

           // カードプレハブをインスタンス化して画面上に表示
            GameObject cardInstance = Instantiate(Card, Vector3.zero, Quaternion.identity);

            // カードの値をTextMeshProに設定
            TextMeshProUGUI cardText = cardInstance.GetComponentInChildren<TextMeshProUGUI>();
            cardText.text = card.ToString();

            // カードをプレイヤーに送信する処理をここに追加
            Debug.Log("Player " + (i + 1) + " received card: " + card);
        }
    }

    int GetNumberOfPlayers()
    {
        // プレイヤー数を取得するロジックを実装
        // 例: return PhotonNetwork.PlayerList.Length;
        return member; // ダミーの値、実際のプレイヤー数に合わせて調整
    }
}

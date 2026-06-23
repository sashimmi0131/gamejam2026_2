using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class selector : MonoBehaviour
{ 
    
     //インスペクターで操作できるようにする
        public Sprite imageA;
        public Sprite imageB;

        private SpriteRenderer sr;
        private int select = 0;

        void Start()
        {
           
            sr = GetComponent<SpriteRenderer>();
            // 初期状態を設定
            sr.sprite = imageA;
        }

        // マウスが乗っている間呼ばれ続ける
        void OnMouseOver()
        {
            select = 1;
            sr.sprite = imageB; // 画像をBに切り替え
        }

        // マウスが離れた瞬間に呼ばれる
        void OnMouseExit()
        {
            select = 0;
            sr.sprite = imageA; // 画像をAに戻す
        }
}


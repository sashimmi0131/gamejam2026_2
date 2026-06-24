using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneBase : MonoBehaviour
{

    public static SceneID beforeScene = SceneID.Title;       //ひとつ前のシーンが何だったか記憶する
    public static SceneID nowScene = SceneID.Title;          //現在どのシーンに居るかを記憶する
    protected void SceneChange(SceneID id)      //(SceneID id)どのシーンに変えたいかという情報を受け取る窓口
    {
        beforeScene = nowScene;                 //現在のシーンを前のシーンとして記憶する
        nowScene = id;                          //nowSceneの中身をこれから移動するシーン(ID)に上書きする

        SceneManager.LoadScene(id.ToString());  //SceneManagerを実行してUnityの機能で実際に画面をロードする
    }

    protected void SceneBack()
    {
        SceneChange(beforeScene);
    }

}

public enum SceneID      //ゲーム内のに存在するシーンの名前をリスト化したもの
{
    Title,
    MainScene,
    HappyEND,
    BadEND,
    SinnyuuEND,
}
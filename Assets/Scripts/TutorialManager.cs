using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    public GameObject Arrow = default;
    [SerializeField]
    private Text OffenceText = default;
    private PunTurnManager turnManager;
    private int[,] board = new int[5, 5];
    [SerializeField]
    private CanvasGroup canvasbutton = default;
    private int previous = 0;
    private int localgravitydir = 4;
    private int remotegravitydir = 4;
    

    private int surrender = 0;//勝ちは1,負けは2


    private float[] vectorJ = { -309.4f, -154.9f, 0f, 154f, 310f };
    private float[] vectorI = { 409f, 256f, 101.5f, -53f, -208f };

    private int localSelection;
    private int remoteSelection;
    private int state = 1;

    bool stlocalallow = true;
    bool stremoteallow = true;

    int turn = 1;
    bool turnonce = true;//バグの収拾
    bool turnonceEmu = true;

    public List<GameObject> stones = new List<GameObject>();//盤面上に存在するオブジェクト群
    public Dictionary<int, GameObject> stoneplace = new Dictionary<int, GameObject>();//stoneの座標と、その座標にあるobjectを羅列
    public Dictionary<int, int> changestones = new Dictionary<int, int>();//元の座標と新しい座標をintで羅列
    public Dictionary<int, GameObject> temporary = new Dictionary<int, GameObject>();//stoneplaceの交換用

    private List<GameObject> temporarylocalstones = new List<GameObject>();//自分の色の石の配列
    private List<GameObject> temporaryremotestones = new List<GameObject>();//相手の色の石の配列

    public AudioClip Tap;
    public AudioClip Erase;
    public AudioClip Gravitysound;
    AudioSource audioSource;

    [SerializeField]
    public GameObject canvas;//キャンバス
    [SerializeField]
    public Text exp;//キャンバス
    [SerializeField]
    public GameObject Nodescontainer;
    [SerializeField]
    public GameObject returnbutton;
    [SerializeField]
    public GameObject black;
    [SerializeField]
    public GameObject blackcircle;
    [SerializeField]
    public GameObject white;
    [SerializeField]
    public GameObject whitecircle;
    [SerializeField]
    public Image gravity3;//キャンバス
    [SerializeField]
    public Image gravity2;
    [SerializeField]
    public Image gravity1;
    bool gravitytime = false;//もしかしなくてもboolでよかった
    [SerializeField, Range(0, 10)]
    float Starttime = 0;
    private float startTime;

    GameObject state121;
    GameObject state123;
    GameObject state124;
    GameObject state221;
    GameObject state223;
    GameObject state224;
    GameObject state321;
    GameObject state322;
    GameObject state323;
    GameObject state324;

    GameObject select1;
    GameObject select2;
    GameObject select2opponent;
    GameObject select3;
    int select3dir = 100;

    public void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Debug.Log("Let's start tutorial");
        audioSource = GetComponent<AudioSource>();
        black = (GameObject)Resources.Load("black");
        blackcircle = (GameObject)Resources.Load("blackcircle");
        white = (GameObject)Resources.Load("white");
        whitecircle = (GameObject)Resources.Load("whitecircle");
        returnbutton.SetActive(true);
        canvasbutton.interactable = false;
        Arrow.SetActive(false);

        Debug.Log("startfirst");
        StartCoroutine("firstStory");
    }
    String S;
    IEnumerator firstStory()
    {
        yield return new WaitForSeconds(1f);
        write("やあ。");
        yield return new WaitForSeconds(1f);
        write("Nodesの世界にようこそ。");
        yield return new WaitForSeconds(1.5f);
        write("ルールを説明する。");
        yield return new WaitForSeconds(1.5f);
        write("自分の色の石を4つ揃えれば君の勝ちだ。");
        yield return new WaitForSeconds(2f);
        write("縦横斜め、方向はどれでもいい。");
        yield return new WaitForSeconds(1f);
        state121 = Instantiate(black, new Vector3(vectorJ[1], vectorI[2], 0f), Quaternion.identity) as GameObject;
        state123 = Instantiate(black, new Vector3(vectorJ[3], vectorI[2], 0f), Quaternion.identity) as GameObject;
        state124 = Instantiate(black, new Vector3(vectorJ[4], vectorI[2], 0f), Quaternion.identity) as GameObject;
        state121.transform.SetParent(Nodescontainer.transform, false);
        state123.transform.SetParent(Nodescontainer.transform, false);
        state124.transform.SetParent(Nodescontainer.transform, false);
        yield return new WaitForSeconds(0.6f);
        write("やってみよう。君は黒だ。");
        canvasbutton.interactable = true;                         
    }
    private void state1invoke(int num)
    {
        if(num==12) StartCoroutine("secondStory");
    }
    IEnumerator secondStory()
    {
        state = 2;
        write("そう、やるじゃないか。");
        yield return new WaitForSeconds(1f);
        Destroy(state121);
        Destroy(state123);
        Destroy(state124);
        Destroy(select1);
        yield return new WaitForSeconds(0.5f);
        write("では、次に行こう。");
        yield return new WaitForSeconds(1.5f);
        write("相手のターンには、相手が石を起く場所を予測しろ。");
        yield return new WaitForSeconds(1.5f);
        write("毎ターン、予測して、その場所をタップするんだ。");
        yield return new WaitForSeconds(1.5f);
        write("予測が当たれば、相手の石を消すことができる。");
        yield return new WaitForSeconds(0.5f);
        state221 = Instantiate(white, new Vector3(vectorJ[1], vectorI[2], 0f), Quaternion.identity) as GameObject;
        state223 = Instantiate(white, new Vector3(vectorJ[3], vectorI[2], 0f), Quaternion.identity) as GameObject;
        state224 = Instantiate(white, new Vector3(vectorJ[4], vectorI[2], 0f), Quaternion.identity) as GameObject;
        state221.transform.SetParent(Nodescontainer.transform, false);
        state223.transform.SetParent(Nodescontainer.transform, false);
        state224.transform.SetParent(Nodescontainer.transform, false);
        yield return new WaitForSeconds(1f);
        write("これもやってみよう。負けないように動け。");
        canvasbutton.interactable = true;
    }
    private void state2invoke(int num)
    {
        if (num == 12)
        {
            select2 = Instantiate(blackcircle, new Vector3(vectorJ[2], vectorI[2], 0f), Quaternion.identity) as GameObject;
            select2.transform.SetParent(Nodescontainer.transform, false);
            select2opponent = Instantiate(white, new Vector3(vectorJ[2], vectorI[2], 0f), Quaternion.identity) as GameObject;
            select2opponent.transform.SetParent(Nodescontainer.transform, false);
            StartCoroutine("thirdStory");
        }
    }
    IEnumerator thirdStory()
    {
        yield return new WaitForSeconds(1f);
        audioSource.PlayOneShot(Erase);
        Destroy(select2);
        Destroy(select2opponent);
        write("いいぞ。");
        yield return new WaitForSeconds(1f);
        Destroy(state221);
        Destroy(state223);
        Destroy(state224);
        yield return new WaitForSeconds(0.8f);
        write("最後に、このゲームを決着する最強の手段を教えよう。");
        yield return new WaitForSeconds(0.8f);
        yield return new WaitForSeconds(0.8f);
        Arrow.SetActive(true);
        write("この矢印を見ろ。");
        yield return new WaitForSeconds(1f);
        write("押せば、矢印の方向に全ての石が動く。");
        yield return new WaitForSeconds(1f);
        state321 = Instantiate(black, new Vector3(vectorJ[1], vectorI[2], 0f), Quaternion.identity) as GameObject;
        state322 = Instantiate(white, new Vector3(vectorJ[2], vectorI[2], 0f), Quaternion.identity) as GameObject;
        state323 = Instantiate(black, new Vector3(vectorJ[3], vectorI[2], 0f), Quaternion.identity) as GameObject;
        state324 = Instantiate(black, new Vector3(vectorJ[4], vectorI[2], 0f), Quaternion.identity) as GameObject;
        select3 = Instantiate(black, new Vector3(vectorJ[2], vectorI[3], 0f), Quaternion.identity) as GameObject;
        state321.transform.SetParent(Nodescontainer.transform, false);
        state323.transform.SetParent(Nodescontainer.transform, false);
        state324.transform.SetParent(Nodescontainer.transform, false);
        state322.transform.SetParent(Nodescontainer.transform, false);
        select3.transform.SetParent(Nodescontainer.transform, false);
        changestones.Add(11, 21);
        changestones.Add(12, 17);
        changestones.Add(13, 23);
        changestones.Add(14, 24);
        changestones.Add(17, 22);
        stoneplace.Add(11,state321);
        stoneplace.Add(12, state322);
        stoneplace.Add(13, state323);
        stoneplace.Add(14, state324);
        stoneplace.Add(17,select3);
        yield return new WaitForSeconds(0.5f);
        write("最後だ。これを使って勝利しろ。");
        canvasbutton.interactable = false;
    }
    private void state3invoke()
    {
        StartCoroutine("lastStory");
    }
    IEnumerator lastStory()
    {

        gravitytime = true;
        audioSource.PlayOneShot(Gravitysound);
        write("");
        Starttime = Time.timeSinceLevelLoad;
        yield return new WaitForSeconds(1.5f);
        gravitytime = false;
        write("見事だ。");
        yield return new WaitForSeconds(1f);
        write("右下のマークはGravityを使える回数を表している。");
        yield return new WaitForSeconds(1f);
        write("同じターンに石を置いて、かつGravityを使用することも可能だ。");
        yield return new WaitForSeconds(2f);
        write("相手のターンにGravityを使って、防御手段とすることもできる。");
        yield return new WaitForSeconds(2f);
        write("全ては話された。");
        yield return new WaitForSeconds(1f);
        write("さあ、始めよう。");
        yield return new WaitForSeconds(2f);
        returnmenu();
    }
    public void OnClick(int number)
    {
        if (number == 100) arrowpushed();
        else if (board[number / 5, number % 5] != 0) Debug.Log("CannotPush");
        else
        {
            audioSource.PlayOneShot(Tap);
            Debug.Log("I am a born of my sword");
            if (state == 1)
            {
                if (number == 12)
                {
                    canvasbutton.interactable = false;
                    select1 = Instantiate(black, new Vector3(vectorJ[number % 5], vectorI[number / 5], 0f), Quaternion.identity) as GameObject;
                    select1.transform.SetParent(Nodescontainer.transform, false);
                    state1invoke(number);
                }
                else write("そこじゃないみたいだ。");
            }
            else if (state == 2)
            {
                if (number == 12)
                {
                    canvasbutton.interactable = false;
                    state2invoke(number);
                }
                else write("そこじゃないみたいだ。");
            }
        }
    }

    bool appearok = false;
    void write(string s)
    {
        S = s;
        exp.text = "";
        appearok = true;
    }
    void oneword()
    {
        if (exp.text.Length < S.Length)
        {
            exp.text += S[exp.text.Length];
        }
        else
        {
            appearok = false;
        }
    }
    void Update()
    {
        if (appearok)
        {
            oneword();
        }
        int now = Rotate();
        if (now != previous)
        {
            Arrow.transform.Rotate(new Vector3(0, 0, (now - previous) * 90));
            previous = now;
        }
        if (gravitytime)
        {
            foreach (KeyValuePair<int, int> pair in changestones)
            {//changestonesのkeyからvalueの座標にkeyでstoneplaceから引っ張れるオブジェクトを移動
             //Debug.Log(pair.Key + " " + pair.Value);
                Vector3 startposition = new Vector3(vectorJ[pair.Key % 5], vectorI[pair.Key / 5], 0f); 
                Vector3 endposition = new Vector3(vectorJ[pair.Value % 5], vectorI[pair.Value / 5], 0f);
                //stoneplace[pair.Key].transform.position = Vector3.Lerp(stoneplace[pair.Key].transform.position, endposition, 2f);
                if (Vector3.Distance(stoneplace[pair.Key].transform.localPosition, endposition) > 2)
                    stoneplace[pair.Key].transform.localPosition += (endposition - startposition) / 30;
            }
        }
    }
    int Rotate()
    {
        var dir = Vector3.zero;
        dir.y = Input.acceleration.y;
        dir.z = Input.acceleration.z;
        int rotate = 0;
        if (Math.Abs(dir.z) >= Math.Abs(dir.y) * 4) return 2;
        else if (dir.y <= 0) rotate = 0;
        else rotate = 2;
        return rotate;
    }
    public void returnmenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    void arrowpushed()
    {//arrowが押された時に呼ばれる処理
        Debug.Log("Arrowpushed");
        Arrow.SetActive(false);
        select3dir = Rotate();
        if (select3dir == 0)
        {
            Debug.Log("colorchange");
            Color usedcolor = new Color(55 / 255f, 55 / 255f, 55 / 255f, 1f);
            gravity3.color = usedcolor;
            Debug.Log("colorchangeEnd");
            state3invoke();
        }
        else
        {
            write("方向が違う。");
            Arrow.SetActive(true);
        }
    }

    private class WaitForseconds
    {
        private float v;

        public WaitForseconds(float v)
        {
            this.v = v;
        }
    }
}

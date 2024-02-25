using Photon.Realtime;
using Photon; // 注意
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class TurnManager : PunBehaviour, IPunTurnManagerCallbacks
{
    [SerializeField]
    public GameObject Arrow = default;
    [SerializeField]
    private Text OffenceText = default;
    private PunTurnManager turnManager;
    [SerializeField]
    private RectTransform TimerFillImage = default;
    [SerializeField]
    private Image Timebar;
    private int[,] board = new int[5, 5];
    [SerializeField]
    private CanvasGroup canvasbutton = default;
    private int previous = 0;
    private int localgravitydir = 4;
    private int remotegravitydir = 4;
    PhotonView photonview;

    private Sprite usedgravity;

    public BannerView bannerViewbottom;
    public BannerView bannerViewtop;

    private int surrender = 0;//勝ちは1,負けは2

    private bool byname=false;

    private string remoteplayername = "相手";
    private int remoteplayerrating=1500;

    private float[] vectorJ = { -309.4f, -154.9f, 0f, 154f, 310f };
    private float[] vectorI = { 409f, 256f, 101.5f, -53f, -208f };

    private int localSelection;
    private int remoteSelection;

    bool stlocalallow = true;
    bool stremoteallow = true;
    bool resultonce=true;
    bool putenemystone=true;

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
    public AudioClip Winnersound;
    public AudioClip Losersound;
    AudioSource audioSource;

    [SerializeField]
    public GameObject Matchstarterbla;
    [SerializeField]
    public GameObject Matchstarterwhi;
    [SerializeField]
    public Text onename;
    [SerializeField]
    public Text onerating;
    [SerializeField]
    public Text twoname;
    [SerializeField]
    public Text tworating;

    [SerializeField]
    public GameObject canvas;//キャンバス
    [SerializeField]
    public GameObject Resultback;
    [SerializeField]
    public GameObject Returnmenu;
    [SerializeField]
    public GameObject Share;
    [SerializeField]
    public GameObject Nodescontainer;
    [SerializeField]
    public Text Ratingchange;
    [SerializeField]
    public GameObject returnbutton;
    [SerializeField]
    public GameObject retirebutton;
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
    bool moveallow = false;
    int Arrowallow = 3;
    [SerializeField, Range(0, 10)]
    float Starttime = 0;
    private float startTime;

    [SerializeField]
    private AudioSource[] _audios;

    int soundstate = 0;


    public void Awake()// StartをAwakeに変更。
    {
        this.turnManager = this.gameObject.AddComponent<PunTurnManager>();//PunTurnManagerをコンポーネントに追加
        this.turnManager.TurnManagerListener = this;//リスナーを？
        this.turnManager.TurnDuration = 10f;//ターンは10秒にする

        //photonview = GameObject.Find("TurnManager").GetComponent<PhotonView>();//scriptsにphotonviewを付けておくのを忘れずに。
    }
    public void Start()// StartをAwakeにする。
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        audioSource = GetComponent<AudioSource>();
        photonview = GameObject.Find("TurnManager").GetComponent<PhotonView>();//scriptsにphotonviewを付けておくのを忘れずに。
        black = (GameObject)Resources.Load("black");
        blackcircle = (GameObject)Resources.Load("blackcircle");
        white = (GameObject)Resources.Load("white");
        whitecircle = (GameObject)Resources.Load("whitecircle");
        returnbutton.SetActive(true);
        // Create an empty ad request.
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                board[i, j] = 0;
            }
        }
        canvasbutton.interactable = false;
        Arrow.SetActive(false);
        PhotonNetwork.ConnectUsingSettings("-1");
        Returnmenu.SetActive(false);
        Share.SetActive(false);
        Matchstarterbla.SetActive(false);
        Matchstarterwhi.SetActive(false);
        Resultback.SetActive(false);
        retirebutton.SetActive(false);
              
        #if UNITY_ANDROID
                string appId="ca-app-pub-9206182867957926~5965745577";
        #elif UNITY_IPHONE
                string appId = "ca-app-pub-9206182867957926~4122890336";
                //string appId = "ca-app-pub-3940256099942544~3347511713";
        #else
                string appId = "unexpected_platform";
        #endif
        // Initialize the Google Mobile Ads SDK.
        //string appId = "ca-app-pub-9206182867957926~5965745577";
        MobileAds.Initialize(appId);

       RequestBannerbottom();
    }
   private void RequestBannerbottom()
    {
      
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-9206182867957926/8755560442";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-9206182867957926/8740435345";
#else
        string adUnitId = "unexpected_platform";
#endif
      //  string adUnitId = "ca-app-pub-9206182867957926/8755560442";

        // Create a 320x50 banner at the top of the screen.
        bannerViewbottom = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerViewbottom.LoadAd(request);
    }
    private void RequestBannertop()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-9206182867957926/8755560442";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-9206182867957926/8740435345";
#else
        string adUnitId = "unexpected_platform";
#endif

        // Create a 320x50 banner at the top of the screen.
        BannerView bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }
    void Update()
    {
        if (soundstate == 1)
        {
            if (_audios[0].volume > 0.02)
            {
                _audios[0].volume -= 0.02f;
                _audios[1].volume += 0.02f;
            }
            else
            {
                _audios[0].Stop();
                _audios[1].volume = 0.2f;
            }
        }
        else if (soundstate == 2)
        {
            if (_audios[1].volume > 0.02)
            {
                _audios[1].volume -= 0.02f;
                _audios[2].volume += 0.02f;
            }
            else
            {
                _audios[1].Stop();
                _audios[2].volume = 0.2f;
            }
        }
        else if (soundstate == 3)
        {
            if (_audios[2].volume > 0.02)
            {
                _audios[2].volume -= 0.02f;
            }
            else
            {
                _audios[2].Stop();
            }
        }
        if (this.turnManager.Turn > 0&&moveallow)//ターンが0以上、TimeTextがnullでない、結果が見えていない場合。
        {
            TimerFillImage.transform.localPosition = new Vector3(this.turnManager.RemainingSecondsInTurn * 95 + 60, 0, 0);
            float k = this.turnManager.RemainingSecondsInTurn/10;
            if (this.turnManager.RemainingSecondsInTurn <= 1f) Arrow.SetActive(false);
            Color barcolor = new Color(k, k, k, 1.0f);
            Timebar.color = barcolor;
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
        if(byname){
            Matchstarterbla.transform.position+= new Vector3(80, 0, 0);
            Matchstarterwhi.transform.position-= new Vector3(80, 0, 0);
        }
    }
   
    #region TurnManager Callbacks
    public void OnPlayerFinished(PhotonPlayer photonPlayer, int turn, object move)//1
    {
        Debug.Log("OnTurnFinished: " + photonPlayer + " turn: " + turn + " action: " + move);
        if (photonPlayer.IsLocal)
        {
            this.localSelection = (int)move;
        }
        else
        {
            this.remoteSelection = (int)move;
        }
    }
    public void OnPlayerMove(PhotonPlayer photonPlayer, int turn, object move) { Debug.Log("Severe Error Occured"); }

    public void OnTurnBegins(int turn)//攻撃側の表示
    {
        localgravitydir = 4;
        remotegravitydir = 4;
        Color barcolor = new Color(1,1,1,1);
        Timebar.color = barcolor;
        TimerFillImage.transform.localPosition = new Vector3(1010, 0, 0);
        Ratingchange.text ="";
        Resultback.SetActive(false);
        putenemystone=true;
        turnonce = true;
        turnonceEmu = true;
        moveallow = true;
        localSelection = 100;
        remoteSelection = 100;
        if (Arrowallow>0) Arrow.SetActive(true);
        canvasbutton.interactable = true;
        //Debug.Log("remoterating=" + remoteplayerrating);
    }


    public void OnTurnCompleted(int obj)//4
    {
        moveallow = false;
        Debug.Log(localSelection);
        Debug.Log(remoteSelection);
        /* if (turnonce == false) return;
         Debug.Log("Funkingbug");
         putenemystone = false;
         canvasbutton.interactable = false;
         GameObject tempothisturn = default;
         int L = localSelection;
         int R = remoteSelection;
         int number = remoteSelection;
         if (R == 100&&turnonce) Debug.Log("turnskip");
         else if (PhotonNetwork.player.ID == 1 && PhotonNetwork.player.ID % 2 == this.turnManager.Turn % 2&&turnonce) tempothisturn = Instantiate(whitecircle, new Vector3(vectorJ[number % 5], vectorI[number / 5], 0f), Quaternion.identity);
         else if (PhotonNetwork.player.ID == 1 && PhotonNetwork.player.ID % 2 != this.turnManager.Turn % 2&&turnonce) tempothisturn = Instantiate(white, new Vector3(vectorJ[number % 5], vectorI[number / 5], 0f), Quaternion.identity);
         else if (PhotonNetwork.player.ID == 2 && PhotonNetwork.player.ID % 2 == this.turnManager.Turn % 2&&turnonce) tempothisturn = Instantiate(blackcircle, new Vector3(vectorJ[number % 5], vectorI[number / 5], 0f), Quaternion.identity);
         else if(turnonce) tempothisturn = Instantiate(black, new Vector3(vectorJ[number % 5], vectorI[number / 5], 0f), Quaternion.identity);
         if (R != 100)
         {
             audioSource.PlayOneShot(Tap);
             tempothisturn.transform.SetParent(Nodescontainer.transform, false);
             temporaryremotestones.Add(tempothisturn);
         }
         turnonce = false;*/
        if (PhotonNetwork.isMasterClient) photonview.RPC("Invoke", PhotonTargets.All, localSelection, remoteSelection, this.turnManager.Turn);
    }
    public void OnTurnTimeEnds(int turn)
    {
        this.OnTurnCompleted(1);
    }

    #endregion

    [PunRPC]
    public void Invoke(int lo,int re,int nowturn)
    {
        StartCoroutine(Placestone(lo,re,nowturn));
    }

    IEnumerator Placestone(int lo,int re,int nowturn)//1秒間持続させてからdestroy
    {
        if (!turnonceEmu) yield break;
        if (PhotonNetwork.player.ID == 2)//後攻だったら自分の選択用に改造
        {
            int t = lo; lo = re; re = t;
        }
        moveallow = false;
        if (turnonce == true)
        {
            GameObject tempothisturn = default;
            if (re == 100 && turnonce) Debug.Log("turnskip");
            else if (PhotonNetwork.player.ID == 1 && PhotonNetwork.player.ID % 2 == this.turnManager.Turn % 2 && turnonce) tempothisturn = Instantiate(whitecircle, new Vector3(vectorJ[re % 5], vectorI[re / 5], 0f), Quaternion.identity);
            else if (PhotonNetwork.player.ID == 1 && PhotonNetwork.player.ID % 2 != this.turnManager.Turn % 2 && turnonce) tempothisturn = Instantiate(white, new Vector3(vectorJ[re % 5], vectorI[re / 5], 0f), Quaternion.identity);
            else if (PhotonNetwork.player.ID == 2 && PhotonNetwork.player.ID % 2 == this.turnManager.Turn % 2 && turnonce) tempothisturn = Instantiate(blackcircle, new Vector3(vectorJ[re % 5], vectorI[re / 5], 0f), Quaternion.identity);
            else if (turnonce) tempothisturn = Instantiate(black, new Vector3(vectorJ[re % 5], vectorI[re / 5], 0f), Quaternion.identity);
            if (re != 100)
            {
                audioSource.PlayOneShot(Tap);
                tempothisturn.transform.SetParent(Nodescontainer.transform, false);
                temporaryremotestones.Add(tempothisturn);
            }
            turnonce = false;
        }
        yield return new WaitForSeconds(1f);
        Debug.Log("localSelection_second:" + lo + "remoteSelection_second:" + re);
        var size = 0f;
        var speed = 0.12f;
        if (lo != re)//マスが違う
        {
            if ((nowturn % 2) == 1 && PhotonNetwork.player.ID == 1 && turnonceEmu)//奇数ターン+先行
            {
                Debug.Log("攻撃側かつ1p");
                if (re != 100)
                {
                    audioSource.PlayOneShot(Erase);
                    while (size <= 1f)
                    {
                        if(temporaryremotestones.Count>0)temporaryremotestones[temporaryremotestones.Count - 1].transform.localScale = Vector3.Lerp(new Vector3(1,1,1), new Vector3(0, 0, 0), size);
                        size += speed;
                        yield return null;
                    }
                    if (temporaryremotestones.Count > 0) Destroy(temporaryremotestones[temporaryremotestones.Count - 1]);
                }
                if (lo != 100)
                {
                    this.board[lo / 5, lo % 5] = 1;
                    if (temporarylocalstones.Count > 0) this.stones.Add(temporarylocalstones[temporarylocalstones.Count - 1] as GameObject);
                    if(stones.Count>0)this.stoneplace.Add(lo, stones[stones.Count - 1]);
                }
            }
            else if ((nowturn % 2) == 0 && PhotonNetwork.player.ID == 1 && turnonceEmu)//偶数ターン+先行
            {//防御側かつ1p
                Debug.Log("防御側かつ1p");
                if (lo != 100)
                {
                    audioSource.PlayOneShot(Erase);
                    while (size <= 1f)
                    {
                        if (temporarylocalstones.Count > 0) temporarylocalstones[temporarylocalstones.Count - 1].transform.localScale = Vector3.Lerp(new Vector3(1,1,1), new Vector3(0, 0, 0), size);
                        size += speed;
                        yield return null;
                    }
                    if (temporarylocalstones.Count > 0) Destroy(temporarylocalstones[temporarylocalstones.Count - 1]);
                }
                if (re != 100)
                {
                    this.board[re / 5, re % 5] = 2;
                    if (temporaryremotestones.Count > 0) this.stones.Add(temporaryremotestones[temporaryremotestones.Count - 1] as GameObject);
                    if (stones.Count > 0) this.stoneplace.Add(re, stones[stones.Count - 1]);
                }
            }
            else if ((nowturn % 2) == 0 && PhotonNetwork.player.ID == 2 && turnonceEmu)//偶数ターン+後攻
            {
                Debug.Log("攻撃側かつ2p");
                if (re != 100)
                {
                    audioSource.PlayOneShot(Erase);
                    while (size <= 1f)
                    {
                        if (temporaryremotestones.Count > 0) temporaryremotestones[temporaryremotestones.Count - 1].transform.localScale = Vector3.Lerp(new Vector3(1,1,1), new Vector3(0, 0, 0), size);
                        size += speed;
                        yield return null;
                    }
                    if (temporaryremotestones.Count > 0) Destroy(temporaryremotestones[temporaryremotestones.Count - 1]);
                }
                if (lo != 100)
                {
                    this.board[lo / 5, lo % 5] = 1;
                    if (temporarylocalstones.Count > 0) this.stones.Add(temporarylocalstones[temporarylocalstones.Count - 1] as GameObject);
                    if (stones.Count > 0) this.stoneplace.Add(lo, stones[stones.Count - 1]);
                }
            }
            else if ((nowturn % 2) == 1 && PhotonNetwork.player.ID == 2 && turnonceEmu)//奇数ターン+後攻
            {
                //防御側2p
                Debug.Log("防御側かつ2p");
                if (lo != 100)
                {
                    audioSource.PlayOneShot(Erase);
                    while (size <= 1f)
                    {
                        if (temporarylocalstones.Count > 0) temporarylocalstones[temporarylocalstones.Count - 1].transform.localScale = Vector3.Lerp(new Vector3(1,1,1), new Vector3(0, 0, 0), size);
                        size += speed;
                        yield return null;
                    }
                    if (temporarylocalstones.Count > 0) Destroy(temporarylocalstones[temporarylocalstones.Count - 1]);
                }
                if (re != 100)
                {
                    this.board[re / 5, re % 5] = 2;
                    if (temporaryremotestones.Count > 0) this.stones.Add(temporaryremotestones[temporaryremotestones.Count - 1] as GameObject);
                    if (stones.Count > 0) this.stoneplace.Add(re, stones[stones.Count - 1]);
                }
            }
        }
        else if(turnonceEmu)
        {
            if (lo != 100)
            {
                audioSource.PlayOneShot(Erase);
                while (size <= 1f)
                {
                    Debug.Log("Start");
                    if (temporarylocalstones.Count > 0) temporarylocalstones[temporarylocalstones.Count - 1].transform.localScale = Vector3.Lerp(new Vector3(1,1,1), new Vector3(0, 0, 0), size);
                    Debug.Log("End");
                    if (temporaryremotestones.Count > 0) temporaryremotestones[temporaryremotestones.Count - 1].transform.localScale = Vector3.Lerp(new Vector3(1,1,1), new Vector3(0, 0, 0), size);
                    size += speed;
                    yield return null;
                }
                if (temporarylocalstones.Count > 0) Destroy(temporarylocalstones[temporarylocalstones.Count - 1]);
                if (temporaryremotestones.Count > 0) Destroy(temporaryremotestones[temporaryremotestones.Count - 1]);
            }
        }
        GameObject[] defenses = GameObject.FindGameObjectsWithTag("destructive");
        foreach (GameObject defense in defenses)
        {
            Destroy(defense);
        }
        turnonceEmu = false;
        if (localgravitydir < 4 && localgravitydir >= 0) stlocalallow = false;
        if (remotegravitydir < 4 && remotegravitydir >= 0) stremoteallow = false;
        yield return new WaitForSeconds(0.5f);//もう少し長くてもいいかも
        if (PhotonNetwork.player.ID == 1 && localgravitydir < 4 && localgravitydir >= 0)
        {
            audioSource.PlayOneShot(Gravitysound);
            photonview.RPC("gravity",PhotonTargets.All,localgravitydir,1);
            localgravitydir = 4;
        }
        if (PhotonNetwork.player.ID == 1 && remotegravitydir < 4 && remotegravitydir >= 0&&stlocalallow)
        {//先行のgravityが同ターンに発動していない場合はこっち
            audioSource.PlayOneShot(Gravitysound);
            photonview.RPC("gravity", PhotonTargets.All, remotegravitydir,2);
            remotegravitydir = 4;
        }
        if (PhotonNetwork.player.ID == 1 && remotegravitydir < 4 && remotegravitydir >= 0 && !stlocalallow)
        {//1Pがやったあとはこっち
            yield return new WaitForSeconds(2f);
            audioSource.PlayOneShot(Gravitysound);
            photonview.RPC("gravity", PhotonTargets.All, remotegravitydir, 2);
            remotegravitydir = 4;
        }
        int result = JudgeResult();
        Debug.Log("stlocalallow= " + stlocalallow);
        Debug.Log("stremoteallow= " + stremoteallow);
        Debug.Log("res= " + JudgeResult());
        if (result > 0){
            if (result == 1)
            {
                if(surrender==0)changeOffence("あなたの勝ちです");
                else changeOffence("降参されました");
            }
            else if (result == 2)
            {
                if (surrender == 0) changeOffence("あなたの負けです");
                else changeOffence("降参しました");
            }
            else if (result == 3) changeOffence("引き分けです");
            if(resultonce)Endofthegame(result);
        }
        else if(stlocalallow&&stremoteallow&&result==0){
            Debug.Log("NG");
            StartTurn();
        }
    }
    void changeOffence(string result){
        OffenceText.text = result;
    }

    void Endofthegame(int result)
    {
        if(resultonce){
            resultonce=false;
            PlayerPrefs.SetInt("Nowplaying", 0);
            int nowrating = PlayerPrefs.GetInt("Rating");
            int newrating;
            if (result == 1)
            {
                audioSource.PlayOneShot(Winnersound);
                newrating = (int)(nowrating + 16 + (remoteplayerrating - nowrating) * 0.04);
            }
            else if (result == 2)
            {
                audioSource.PlayOneShot(Losersound);
                newrating = (int)(nowrating - 16 + (remoteplayerrating - nowrating) * 0.04);
            }
            else newrating = nowrating;
            stlocalallow = false;stremoteallow = false;
            Ratingchange.text = nowrating + "→" + newrating;
            PlayerPrefs.SetInt("Rating", newrating);
            TimerFillImage.transform.localPosition = new Vector3(60, 0, 0);
            Arrow.SetActive(false);
            retirebutton.SetActive(false);
            Resultback.SetActive(true);
            Share.SetActive(true);
            Returnmenu.SetActive(true);
        }
    }
    public void StartTurn()
    {
        GameObject[] defenses = GameObject.FindGameObjectsWithTag("destructive");
        foreach(GameObject defense in defenses){
            Destroy(defense);
        }
        localgravitydir = 4;
        remotegravitydir = 4;
        Color barcolor = new Color(1,1,1,1);
        Timebar.color = barcolor;
        TimerFillImage.transform.localPosition = new Vector3(1010, 0, 0);
        if ((this.turnManager.Turn % 2) == PhotonNetwork.player.ID % 2)
        {//攻撃側
            Ratingchange.text = "相手の攻撃です";
            OffenceText.text = "相手の攻撃です";
        }
        else
        {
            Ratingchange.text = "自分の攻撃です";
            OffenceText.text = "自分の攻撃です";
        }
        Resultback.SetActive(true);
        //if (PhotonNetwork.isMasterClient) StartCoroutine("PrepareForStartturn");
        if (PhotonNetwork.isMasterClient) photonview.RPC("StartInvoke", PhotonTargets.All);
    }

    [PunRPC]
    public void StartInvoke()
{
    StartCoroutine("Startturn");
}
    IEnumerator Startturn(){
        yield return new WaitForSeconds(0.5f);
        Ratingchange.text ="";
        Resultback.SetActive(false);
        if (PhotonNetwork.isMasterClient)
        {
            this.turnManager.BeginTurn();//turnmanagerに新しいターンを始めさせる
        }
    }
public int JudgeResult()
    {//0は何もなし、1は先行、2は後攻,引き分けは3
        if (surrender != 0)
        {
            return surrender;
        }
        int localwin = 0;
        int remotewin = 0;
        for (int k = 0; k < 2; k++)
        {
            for (int j = 0; j < 5; j++)
            {
                bool t = true;
                for (int i = 0 + k; i < 4 + k; i++)
                {
                    if (board[i, j] != 1) t = false;
                }
                if (t) localwin++;
            }
        }
        for (int k = 0; k < 2; k++)
        {
            for (int j = 0; j < 5; j++)
            {
                bool t = true;
                for (int i = 0 + k; i < 4 + k; i++)
                {
                    if (board[i, j] != 2) t = false;
                }
                if (t) remotewin++;
            }
        }
        for (int k = 0; k < 2; k++)
        {
            for (int i = 0; i < 5; i++)
            {
                bool t = true;
                for (int j = 0 + k; j < 4 + k; j++)
                {
                    if (board[i, j] != 2) t = false;
                }
                if (t) remotewin++;
            }
        }
        for (int k = 0; k < 2; k++)
        {
            for (int i = 0; i < 5; i++)
            {
                bool t = true;
                for (int j = 0 + k; j < 4 + k; j++)
                {
                    if (board[i, j] != 1) t = false;
                }
                if (t) localwin++;
            }
        }
        if (board[0, 3] == 1 && board[1, 2] == 1 && board[2, 1] == 1 && board[3, 0] == 1) localwin++;
        if (board[0, 3] == 2 && board[1, 2] == 2 && board[2, 1] == 2 && board[3, 0] == 2) remotewin++;
        if (board[0, 4] == 1 && board[1, 3] == 1 && board[2, 2] == 1 && board[3, 1] == 1) localwin++;
        if (board[1, 3] == 1 && board[2, 2] == 1 && board[3, 1] == 1 && board[4, 0] == 1) localwin++;
        if (board[1, 4] == 1 && board[2, 3] == 1 && board[3, 2] == 1 && board[4, 1] == 1) localwin++;
        if (board[0, 4] == 2 && board[1, 3] == 2 && board[2, 2] == 2 && board[3, 1] == 2) remotewin++;
        if (board[1, 3] == 2 && board[2, 2] == 2 && board[3, 1] == 2 && board[4, 0] == 2) remotewin++;
        if (board[1, 4] == 2 && board[2, 3] == 2 && board[3, 2] == 2 && board[4, 1] == 2) remotewin++;
        if (board[0, 0] == 1 && board[1, 1] == 1 && board[2, 2] == 1 && board[3, 3] == 1) localwin++;
        if (board[4, 4] == 1 && board[1, 1] == 1 && board[2, 2] == 1 && board[3, 3] == 1) localwin++;
        if (board[0, 1] == 1 && board[1, 2] == 1 && board[2, 3] == 1 && board[3, 4] == 1) localwin++;
        if (board[1, 0] == 1 && board[2, 1] == 1 && board[3, 2] == 1 && board[4, 3] == 1) localwin++;
        if (board[0, 0] == 2 && board[1, 1] == 2 && board[2, 2] == 2 && board[3, 3] == 2) localwin++;
        if (board[4, 4] == 2 && board[1, 1] == 2 && board[2, 2] == 2 && board[3, 3] == 2) localwin++;
        if (board[0, 1] == 2 && board[1, 2] == 2 && board[2, 3] == 2 && board[3, 4] == 2) localwin++;
        if (board[1, 0] == 2 && board[2, 1] == 2 && board[3, 2] == 2 && board[4, 3] == 2) localwin++;
        Debug.Log("localwin:" + localwin + " remotewin:" + remotewin);
        //継続は0,勝利は1,敗北は2,引き分けは3
        if (localwin == 0 && remotewin == 0){
            int Cnt = 0;
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (board[i, j] == 0) Cnt++;
                }
            }
            if (Cnt < 2) return 3;
            else
            {
                if (Cnt < 20) soundstate = 1;
                if (Cnt < 15) soundstate = 2;
                return 0;
            }
        } 
        else if (localwin > remotewin) return 1;
        else if (localwin < remotewin) return 2;
        else return 3;
    }//勝敗の判定
    int Rotate()
    {
        var dir = Vector3.zero;
        dir.y = Input.acceleration.y;
        dir.z = Input.acceleration.z;
        int rotate = 0;
        if (Math.Abs(dir.z) >= Math.Abs(dir.y)*1.5f) rotate= 2;
        else if (dir.y <= 0) rotate = 0;
        else rotate = 2;
        return rotate;
    }
    void arrowpushed()
    {//arrowが押された時に呼ばれる処理
        Debug.Log("Arrowpushed");
        Arrow.SetActive(false);
        if (Arrowallow == 3)
        {
            Color usedcolor = new Color(55/255f, 55/255f, 55/255f, 1f);
            gravity3.color = usedcolor;
        }
        else if (Arrowallow == 2)
        {
            Color usedcolor = new Color(55/255f, 55/255f, 55/255f, 1f);
            gravity2.color = usedcolor;

        }
        else if (Arrowallow == 1)
        {
            Color usedcolor = new Color(55/255f, 55/255f, 55/255f, 1f);
            gravity1.color = usedcolor;
        }
        Arrowallow = Arrowallow - 1;
        localgravitydir = Rotate();
        var properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("Gravitydir", localgravitydir);
        PhotonNetwork.player.SetCustomProperties(properties);
    }

    [PunRPC]
    void gravity(int dir,int granum)
    {//0は下、1は右、2は上、3は左
        int[,] newboard = new int[5, 5];
        if (dir == 0)
        {
            for (int j = 0; j < 5; j++)
            {
                int[] line = { 0, 0, 0, 0, 0 };
                int num = 4;
                for (int i = 4; i >= 0; i--)
                {
                    if (board[i, j] != 0)
                    {
                        line[num] = board[i, j];
                        changestones.Add(i * 5 + j, num * 5 + j);
                        num--;
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    newboard[i, j] = line[i];
                }
            }
        }
        else if (dir == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                int[] line = { 0, 0, 0, 0, 0 };
                int num = 4;
                for (int j = 4; j >= 0; j--)
                {
                    if (board[i, j] != 0)
                    {
                        line[num] = board[i, j];
                        changestones.Add(i * 5 + j, i * 5 + num);
                        num--;
                    }
                }
                for (int j = 0; j < 5; j++)
                {
                    newboard[i, j] = line[j];
                }
            }
        }
        else if (dir == 2)
        {
            for (int j = 0; j < 5; j++)
            {
                int[] line = { 0, 0, 0, 0, 0 };
                int num = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (board[i, j] != 0)
                    {
                        line[num] = board[i, j];
                        changestones.Add(i * 5 + j, num * 5 + j);
                        num++;
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    newboard[i, j] = line[i];
                }
            }
        }
        else if (dir == 3)
        {
            for (int i = 0; i < 5; i++)
            {
                int[] line = { 0, 0, 0, 0, 0 };
                int num = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (board[i, j] != 0)
                    {
                        line[num] = board[i, j];
                        changestones.Add(i * 5 + j, i*5 + num);
                        num++;
                    }
                }
                for (int j = 0; j < 5; j++)
                {
                    newboard[i, j] = line[j];
                }
            }
        }
        Debug.Log(dir);
        int[] transboard = new int[25];
        for (int i = 0; i < 5; i++) for (int j = 0; j < 5; j++) transboard[i*5+j] = newboard[i,j];
        StartCoroutine(Gravitytime(transboard, granum));
    }

    private IEnumerator Gravitytime(int [] transboard,int granum)//3秒開けてgravityに仕事をさせる
    {

        Debug.Log("Stoneplace.size= "+stoneplace.Count);
        Debug.Log("changestones.size= " + changestones.Count);
        gravitytime = true;
        Resultback.SetActive(true);
        if (granum == PhotonNetwork.player.ID) Ratingchange.text = "自分のGravity";
        else Ratingchange.text = "相手のGravity";
        Starttime = Time.timeSinceLevelLoad;
        yield return new WaitForSeconds(1f);
        Resultback.SetActive(false);
        Ratingchange.text = "";
        yield return new WaitForSeconds(0.5f);
        gravitytime =false;
        foreach (KeyValuePair<int, int> pair in changestones)
        {//帳尻合わせ
            Vector3 endposition = new Vector3(vectorJ[pair.Value % 5], vectorI[pair.Value / 5], 0f);
            // stoneplace[pair.Key].transform.position = Vector3.Lerp(stoneplace[pair.Key].transform.position, endposition, rate);
            stoneplace[pair.Key].transform.localPosition = endposition;
        }
        //新たにどこに何があるのかの記憶
        foreach (KeyValuePair<int, GameObject> pair in stoneplace)
        {
            temporary.Add(changestones[pair.Key], pair.Value);
        }
        stoneplace.Clear();//初期化
        foreach (KeyValuePair<int, GameObject> pair in temporary)
        {
            stoneplace.Add(pair.Key, pair.Value);
        }
        temporary.Clear();//置き換え用の配列を消去
        changestones.Clear();//使った配列を消去
        int[,] newboard = new int[5, 5];
        for (int i = 0; i < 5; i++) for (int j = 0; j < 5; j++)  newboard[i, j]= transboard[i * 5 + j];
        board = newboard;

        if (granum == PhotonNetwork.player.ID) stlocalallow = true;
        else stremoteallow = true;
        Debug.Log("stlocalallow= " + stlocalallow);
        Debug.Log("stremoteallow= " + stremoteallow);
        Debug.Log("res= " + JudgeResult());
        int resultlocal = JudgeResult();
        if (resultlocal> 0) {
            if(resultonce)Endofthegame(JudgeResult());
            if (resultlocal == 1) changeOffence("あなたの勝ちです");
            else if (resultlocal == 2) changeOffence("あなたの負けです");
            else if (resultlocal == 3) changeOffence("引き分けです");
        }

        if (stlocalallow&&stremoteallow&&JudgeResult()==0){
            Debug.Log("OK");
            StartTurn();
        }
    }

    public void OnFailedToConnectToPhoton(){
        StartCoroutine("Lobbyfail");
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    private IEnumerator Lobbyfail()
    {
        OffenceText.text = "接続に失敗しました";
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("MenuScene");
    }

    public override void  OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        // ランダムに入室失敗した場合、ルームを作成
        // ルームオプションの作成
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        roomOptions.MaxPlayers = 2;

        /*roomOptions.CustomRoomPropertiesForLobby = { "map", "ai" };
        roomOptions.CustomRoomProperties = new Hashtable() { { "map", 1 } };*/
        // ルームの作成
        PhotonNetwork.CreateRoom(null, roomOptions, null);
    }
    /// <summary>
    /// ルームに入室成功した場合呼ばれるメソッド
    /// </summary>
    public override void OnJoinedRoom()
    {
        var properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("Username", PlayerPrefs.GetString("Name"));
        properties.Add("Rating", PlayerPrefs.GetInt("Rating"));
        PhotonNetwork.player.SetCustomProperties(properties);
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            if (this.turnManager.Turn == 0)
            {// when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                StartCoroutine("Startgame");
            }
        }
        else
        {
            Debug.Log("Waiting for another player");
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Other player arrived");
        if (PhotonNetwork.room.PlayerCount == 2)
        {
            if (this.turnManager.Turn == 0)
            {
                StartCoroutine("Startgame");
            }
        }
    }
    private IEnumerator Startgame()
    {
    
        PhotonNetwork.room.IsOpen = false;            // 部屋を閉じる
        PhotonNetwork.room.IsVisible = false;
        yield return new WaitForSeconds(0.5f);
        returnbutton.SetActive(false);
        Matchstarterbla.SetActive(true);
        Matchstarterwhi.SetActive(true);
        onename.text = PlayerPrefs.GetString("Name");
        onerating.text=PlayerPrefs.GetInt("Rating")+"";
        twoname.text = remoteplayername;
        tworating.text = remoteplayerrating+"";
        if (PhotonNetwork.player.ID==1){
            Arrowallow = 2;
            Color usedcolor = new Color(55/255f,55/255f,55/255f,1f);
            gravity3.color = usedcolor;
            string s = onename.text;
            onename.text = twoname.text;
            twoname.text = s;
            string t = onerating.text;
            onerating.text = tworating.text;
            tworating.text=t;
        }

        if (bannerViewbottom != null)
        {
            bannerViewbottom.Hide();
            bannerViewbottom.Destroy();
        }
        RequestBannertop();
        PlayerPrefs.SetInt("Nowplaying", 1);
        yield return new WaitForSeconds(2.0f);
        byname = true;
        yield return new WaitForSeconds(0.8f);
        byname = false;
        onename.text = "";
        onerating.text = "";
        twoname.text ="";
        tworating.text = "";
        retirebutton.SetActive(true);
        Matchstarterbla.SetActive(false);
        Matchstarterwhi.SetActive(false);
        this.StartTurn();
    }

     public void Onpushretirebutton()
    {
        //降参敗北処理
        var properties = new ExitGames.Client.Photon.Hashtable();
        properties.Add("Surrender", 1);
        PhotonNetwork.player.SetCustomProperties(properties);
        surrender = 2;
        retirebutton.SetActive(false);
    }


    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Arrow.SetActive(false);
        moveallow = false;
        if(JudgeResult()==0){
            Debug.Log("Other player disconnected! " + otherPlayer.ToStringFull());
            changeOffence("通信が切断されました");
            if(resultonce)Endofthegame(1);
        }
    }

    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {//相手の名前、レートを保存,稀に重力の方向を保存
        var player = playerAndUpdatedProps[0] as PhotonPlayer;
        var properties = playerAndUpdatedProps[1] as ExitGames.Client.Photon.Hashtable;
        if(player.ID!= PhotonNetwork.player.ID){
            if (properties.TryGetValue("Username", out object username))
            {
                Debug.Log(username);
                remoteplayername = (string)username;
            }
            if (properties.TryGetValue("Rating", out object rating))
            {
                Debug.Log(rating);
                remoteplayerrating = (int)rating;
            }
            if (properties.TryGetValue("Gravitydir", out object gra))
            {
                Debug.Log(gra);
                remotegravitydir = (int)gra;
            }
            if (properties.TryGetValue("Surrender", out object sur))
            {
                Debug.Log(sur);
                surrender = (int)sur;
            }
        }
    }
    
    public void OnDisconnectedFromServer()
    {
        if(JudgeResult()==0&&resultonce)Endofthegame(2);
   }

    public void returnmenu(){
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        if (bannerViewtop != null)
        {
            bannerViewtop.Hide();
            bannerViewtop.Destroy();
        }
        if (bannerViewbottom != null)
        {
            bannerViewbottom.Hide();
            bannerViewbottom.Destroy();
        }
        SceneManager.LoadScene(0);
    }
    public void onClickShare()
    {
        StartCoroutine("_share");
    }
    private IEnumerator _share()
    {
        UnityEngine.ScreenCapture.CaptureScreenshot("image.png");
        yield return null;

        var shareText = "今すぐNodesをプレイ!!";
        var imagePath = Application.persistentDataPath + "/image.png";

        SocialConnector.SocialConnector.Share(shareText,"",imagePath);
    }
    public void OnClick(int number)
    {
        if (number == 100) arrowpushed();
        else if (board[number / 5, number % 5] != 0) Debug.Log("CannotPush");
        else
        {
            Arrow.SetActive(false);
            audioSource.PlayOneShot(Tap);
            Debug.Log(number);
            canvasbutton.interactable = false;
            GameObject tempothisturn;
            if (PhotonNetwork.player.ID == 1 && PhotonNetwork.player.ID % 2 == this.turnManager.Turn % 2) tempothisturn = Instantiate(black, new Vector3(vectorJ[number % 5], vectorI[number / 5], 0f), Quaternion.identity) as GameObject;
            else if (PhotonNetwork.player.ID == 1 && PhotonNetwork.player.ID % 2 != this.turnManager.Turn % 2) tempothisturn = Instantiate(blackcircle, new Vector3(vectorJ[number % 5], vectorI[number / 5], 0f), Quaternion.identity) as GameObject;
            else if (PhotonNetwork.player.ID == 2 && PhotonNetwork.player.ID % 2 == this.turnManager.Turn % 2) tempothisturn = Instantiate(white, new Vector3(vectorJ[number % 5], vectorI[number / 5], 0f), Quaternion.identity) as GameObject;
            else tempothisturn = Instantiate(whitecircle, new Vector3(vectorJ[number % 5], vectorI[number / 5], 0f), Quaternion.identity) as GameObject;
            tempothisturn.transform.SetParent(Nodescontainer.transform, false);
            temporarylocalstones.Add(tempothisturn as GameObject);
            this.turnManager.SendMove(number, true);
        }
    }
}

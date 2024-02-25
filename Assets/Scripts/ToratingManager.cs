using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
//using NCMB;
using System;
using UnityEngine.UI;

public class ToratingManager : MonoBehaviour
{
    private string usingletters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnprstuwxyz23456789";
    string roomname="";
    [SerializeField]
    public GameObject create;
    [SerializeField]
    public GameObject join;
    [SerializeField]
    public GameObject prep;
    [SerializeField]
    public GameObject howtouse;
    [SerializeField]
    public GameObject rating;
    [SerializeField]
    public GameObject resign;

    public AudioClip Click;
    AudioSource audioSource;

    [SerializeField]
    public Text[] TopRankingname;
    [SerializeField]
    public Text[] TopRankingrating;
    [SerializeField]
    public Text[] MyRankingname;
    [SerializeField]
    public Text[] MyRankingrating;

    [SerializeField]
    public GameObject BillBoard;

    public BannerView bannerView;
    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Debug.Log("Startrating");
        audioSource = GetComponent<AudioSource>();
        BillBoard.SetActive(false);
        join.SetActive(false);
        create.SetActive(false);
        resign.SetActive(false);
        howtouse.SetActive(false);

#if UNITY_ANDROID
        string appId="ca-app-pub-9206182867957926~5965745577";
#elif UNITY_IPHONE
        string appId = "ca-app-pub-9206182867957926~4122890336";
#else
        string appId = "unexpected_platform";
#endif
      //  string appId = "ca-app-pub-9206182867957926~5965745577";
        //submitranking();
        // Initialize the Google Mobile Ads SDK.
       MobileAds.Initialize(appId);
        RequestBanner();
        Debug.Log("Endrating");
    }

   /* public void submitranking()
    {
        NCMBObject obj = new NCMBObject("Nodes_Ranking");
        if (!PlayerPrefs.HasKey("SelfId") && PlayerPrefs.HasKey("Rating") && PlayerPrefs.HasKey("Name"))
        {
            obj["Name"] = PlayerPrefs.GetString("Name");
            obj["Rating"] = PlayerPrefs.GetInt("Rating");
            obj.SaveAsync((NCMBException e) =>
            {

                if (e != null)
                {

                    return;
                }
                else
                {
                    PlayerPrefs.SetString("SelfId", obj.ObjectId);
                }
            });
        }
        else if (PlayerPrefs.HasKey("Rating") && PlayerPrefs.HasKey("Name"))
        {
            Debug.Log("G");
            obj.ObjectId = PlayerPrefs.GetString("SelfId");
            obj.FetchAsync((NCMBException e) =>
            {
                if (e != null)
                {
                    return;
                }
                else
                {
                    obj["Name"] = PlayerPrefs.GetString("Name");
                    obj["Rating"] = PlayerPrefs.GetInt("Rating");
                }
            });
            obj.SaveAsync();
        }
        else return;
    }
    public void getranking()
    {
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("Nodes_Ranking");
        query.FindAsync((List<NCMBObject> objList, NCMBException e) => {
            if (e == null)
            {
                int myPosition = 0;
                for (int i = 0; i < objList.Count; i++)
                {
                    if (objList[i].ObjectId == PlayerPrefs.GetString("SelfId"))
                    {
                        myPosition = i;
                        break;
                    }
                }
                for (int i = 0; i < Math.Min(objList.Count, 5); i++)
                {
                    Debug.Log(objList[i]["Name"].ToString());
                    TopRankingname[i].text =objList[i]["Name"].ToString();
                    TopRankingrating[i].text = objList[i]["Rating"].ToString();

                }
                for (int i = Math.Max(0, myPosition - 2); i < Math.Min(objList.Count, myPosition + 3); i++)
                {
                    int k= Math.Max(0, myPosition - 2);
                    MyRankingname[i-k].text = objList[i]["Name"].ToString();
                    MyRankingrating[i-k].text = objList[i]["Rating"].ToString();
                }

            }

        });
    }*/

    private void RequestBanner()
    {
        
        #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-9206182867957926/8755560442";
        #elif UNITY_IPHONE
                string adUnitId = "ca-app-pub-9206182867957926/8740435345";
        #else
                string adUnitId = "unexpected_platform";
        #endif
        //string adUnitId = "ca-app-pub-9206182867957926/8755560442";
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }
    public void Movettorating(){
        audioSource.PlayOneShot(Click);
        Debug.Log("succeeded");
        if (bannerView != null)
        {
            bannerView.Hide();
            bannerView.Destroy();
        }
        SceneManager.LoadScene("RatingScene");
    }
    public void Movettofriend_prep()
    {
        audioSource.PlayOneShot(Click);
        join.SetActive(true);
        create.SetActive(true);
        resign.SetActive(true);
        prep.SetActive(false);
        howtouse.SetActive(false);
        rating.SetActive(false);
    }
    public void Movettofriend_resign()
    {
        audioSource.PlayOneShot(Click);
        resign.SetActive(false);
        join.SetActive(false);
        create.SetActive(false);
        prep.SetActive(true);
        howtouse.SetActive(false);
        rating.SetActive(true);
    }
    static System.Random random = new System.Random();
    public void Movettofriend_create()
    {
        audioSource.PlayOneShot(Click);
        PlayerPrefs.SetInt("Roomcreate", 1);
        for (int i = 0; i < 6;i++){
            roomname += usingletters[random.Next(usingletters.Length)];
        }
        Debug.Log(roomname);
        PlayerPrefs.SetString("Roomname",roomname);
        if (bannerView != null)
        {
            bannerView.Hide();
            bannerView.Destroy();
        }
        SceneManager.LoadScene("FriendBattleScene");
    }
    public void Movettofriend_join()
    {
        audioSource.PlayOneShot(Click);
        PlayerPrefs.SetInt("Roomcreate",0);
        if (bannerView != null)
        {
            bannerView.Hide();
            bannerView.Destroy();
        }
        SceneManager.LoadScene("FriendBattleScene");
    }

    public void Movettoranking()
    {
        audioSource.PlayOneShot(Click);
        BillBoard.SetActive(true);
        //getranking();
    }
   /* public void Returnfromranking()
    {
        audioSource.PlayOneShot(Click);
        for(int i = 0; i < 5; i++)
        {
            TopRankingname[i].text = "";
            TopRankingrating[i].text = "";
            MyRankingname[i].text = "";
            MyRankingrating[i].text = "";
        }
        BillBoard.SetActive(false);
    }*/
    public void Movettotutorial()
    {
        audioSource.PlayOneShot(Click);
        if (bannerView != null)
        {
            bannerView.Hide();
            bannerView.Destroy();
        }
        SceneManager.LoadScene("TutorialScene");
    }
    
}

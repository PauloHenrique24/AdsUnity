using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    public static AdsInitializer instance { get; private set; }


    [Header("Initialize")]
    [SerializeField] string _androidGameId;
    [SerializeField] bool _testMode = true;

    private string _gameId;

    [Space]

    [Header("Interstitial Ads")]
    [SerializeField] string _interstitialAdUnitId = "Interstitial_Android";

    private string _itUnitId;

    [Space]

    [Header("Banner Ads")]
    [SerializeField] string _bannerAdUnitId = "Banner_Android";
    [SerializeField] BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

    private string _baUnitId = null;

    [Space]

    [Header("Rewarded Ads")]
    [SerializeField] string _rewardedAdUnitId = "Rewarded_Android";

    [HideInInspector] public delegate void RewardedRecompense();
    public static event RewardedRecompense Recompense;

    private string _reUnitId;

    void Awake()
    {
        // Initialized

        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        InitializeAds();

        //Interstitial
        _itUnitId = (Application.platform == RuntimePlatform.Android) ? _interstitialAdUnitId : null;

    }

    void Start()
    {
        #if UNITY_ANDROID
                _baUnitId = _bannerAdUnitId;
                _reUnitId = _rewardedAdUnitId;
        #endif

        Advertisement.Banner.SetPosition(bannerPosition);
    }

    public void InitializeAds()
    {
        #if UNITY_ANDROID
                _gameId = _androidGameId;
        #elif UNITY_EDITOR
                _gameId = _androidGameId; //Only for testing the functionality in the Editor
        #endif


        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

    #region Banner

    public void LoadBanner()
    {
        BannerLoadOptions bannerOptions = new BannerLoadOptions()
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_baUnitId,bannerOptions);
    }

    public void ShowBanner()
    {
        BannerOptions bannerOptions = new BannerOptions()
        {
            clickCallback = OnBannerClicked,
            showCallback = OnBannerShown,
            hideCallback = OnBannerHidden
        };

        Advertisement.Banner.Show(_baUnitId,bannerOptions);
    }


    // Verifica se o banner foi carregado
    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");
    }

    // Avisa se houver algum erro
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
    }

    //Fecha o banner
    public void HideBannerAd() { Advertisement.Banner.Hide(); }

    void OnBannerClicked() { } // Se o banner for clicado
    void OnBannerShown() { } // Se o banner for chamado
    void OnBannerHidden() { } // Se o banner for fechado

    #endregion

    #region Interstitial

    public void InterstitialLoad()
    {
        Debug.Log("Loading Ad: " + _itUnitId);
        Advertisement.Load(_itUnitId, this);
    }

    public void InterstitialShow()
    {
        Debug.Log("Showing Ad: " + _itUnitId);
        Advertisement.Show(_itUnitId, this);
    }

    #endregion

    #region Rewarded

    public void RewardedLoad()
    {
        Debug.Log($"Loading Ad: {_reUnitId}");
        Advertisement.Load(_reUnitId, this);
    }

    public void RewardedShow()
    {
        Debug.Log($"Showing Ad: {_reUnitId}");
        Advertisement.Show(_reUnitId, this);
    }

    #endregion

    #region CallBacks Initilized
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
    #endregion


    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Ad Loaded Sucessfull");
    }
    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
    }
    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
    }


    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) 
    {
        if (adUnitId.Equals(_reUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            Recompense?.Invoke();
        }
    }

    public void OnUnityAdsShowStart(string placementId) { }
    public void OnUnityAdsShowClick(string placementId) { }

}
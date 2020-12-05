using UnityEngine;
using UnityEngine.Advertisements;

public class RewardVideoManager : MonoBehaviour, IUnityAdsListener
{
    string gameId = "3800651";
    string myPlacementId = "rewardedVideo";
    bool testMode = false;
    bool adLoaded = false;
    public GameObject ShowAdButton;
    bool adKeepPlayUsed = false;
    public GameObject MainGameScript;
    


    // Initialize the Ads listener and service:
    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, testMode);
    }

    void FixedUpdate()
    {
        if(adLoaded && !adKeepPlayUsed)
        {
            ShowAdButton.SetActive(true);
            adKeepPlayUsed = true;
        }

    }


    public void ShowRewardedVideo()
    {
        adLoaded = false;
        ShowAdButton.SetActive(false);
        if (Advertisement.IsReady(myPlacementId))
        {
            Advertisement.Show(myPlacementId);
        }
        else
        {
            Debug.Log("Rewarded video is not ready at the moment! Please try again later!");
        }
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            MainGameScript.GetComponent<QuizMainScript>().KeepPlaying();
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, show the ad:
        if (placementId == myPlacementId)
        {
            adLoaded = true;
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }

    // When the object that subscribes to ad events is destroyed, remove the listener:
    public void OnDestroy()
    {
        Advertisement.RemoveListener(this);
    }
}


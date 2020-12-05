using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System;

public class MainMenuScript : MonoBehaviour
{
    public GameObject Menu;
    public GameObject UpgradesShopBox;
    public GameObject BestListBox;
    public GameObject StatisticBox;
    public GameObject ShopPoints;
    public GameObject ShopLifesUpgrade;
    public GameObject ShopJokerUpgrade;
    public GameObject lifeCurrent;
    public GameObject jokerCurrent;
    public int lifesCount;
    public int pointsCount;
    public int jokerCount;
    public GameObject InputName;
    public GameObject InputButton;
    public GameObject InputText;

    public GameObject InputText1;
    public GameObject InputText2;
    public GameObject InputText3;
    public GameObject InputText4;
    public GameObject InputText5;
    public GameObject InputText6;



    void Start()
    {
        lifesCount = PlayerPrefs.GetInt("Lifes", 3);
        pointsCount = PlayerPrefs.GetInt("TotalPoints", 0);
        jokerCount = PlayerPrefs.GetInt("NumOfJokers", 1);
        StartCoroutine(UploadBestResultsGet());
        if(PlayerPrefs.GetString("Mark",null) != "")
        {
            InputName.SetActive(false);
            InputButton.SetActive(false);
            InputText.SetActive(false);
            StartCoroutine(UploadNameAndNewScore());
        }
        else if(PlayerPrefs.GetString("Mark", null) == "")
        {
            InputName.SetActive(true);
            InputButton.SetActive(true);
            InputText.SetActive(true);
        }
        InputText1.GetComponent<Text>().text = PlayerPrefs.GetInt("GamesPlayed", 0).ToString();
        InputText2.GetComponent<Text>().text = PlayerPrefs.GetInt("JokersUsed", 0).ToString();
        InputText3.GetComponent<Text>().text = PlayerPrefs.GetInt("Record", 0).ToString();
        InputText4.GetComponent<Text>().text = PlayerPrefs.GetInt("TotalPoints", 0).ToString();
        InputText5.GetComponent<Text>().text = PlayerPrefs.GetInt("Wrongs", 0).ToString();
        int wrongs = PlayerPrefs.GetInt("Wrongs", 0);
        int total = PlayerPrefs.GetInt("TotalPoints", 0);
        float result = (float)wrongs / ((float)total + (float)wrongs);
        InputText6.GetComponent<Text>().text = (result*100).ToString("0.00") + " %";


    }

    void FixedUpdate()
    {
        ShopPoints.GetComponent<Text>().text = pointsCount.ToString() + " BODOVA";
        lifeCurrent.GetComponent<Text>().text = "X   " + (lifesCount + 1).ToString();
        jokerCurrent.GetComponent<Text>().text = "X   " + (jokerCount + 1).ToString();
        if (lifesCount >= 15)
        {
            Color tmp = ShopLifesUpgrade.GetComponent<Image>().color;
            tmp.a = 0.3f;
            ShopLifesUpgrade.GetComponent<Image>().color = tmp;
            lifeCurrent.GetComponent<Text>().text = "  MAX";

        }
        if (jokerCount >= 5)
        {
            Color tmp = ShopJokerUpgrade.GetComponent<Image>().color;
            tmp.a = 0.3f;
            ShopJokerUpgrade.GetComponent<Image>().color = tmp;
            jokerCurrent.GetComponent<Text>().text = "  MAX";
        }

    }

    public void PlayQuiz()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void OpenUpgrades()
    {
        Menu.SetActive(false);
        UpgradesShopBox.SetActive(true);
    }
    public void OpenBestPlayersList()
    {
        Menu.SetActive(false);
        BestListBox.SetActive(true);
    }
    public void BackToMenu()
    {
        UpgradesShopBox.SetActive(false);
        BestListBox.SetActive(false);
        StatisticBox.SetActive(false);
        Menu.SetActive(true);
    }
    public void OpenRateUs()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.neoblast.sportquiz");
    }
    public void OpenMoreGames()
    {
        Application.OpenURL("https://play.google.com/store/apps/developer?id=Neoblast");
    }
    public void UpgradeLifes()
    {
        if(lifesCount >= 15)
        {
            return;
        }
        if(pointsCount < 50)
        {
            return;
        }
        pointsCount -= 50;
        PlayerPrefs.SetInt("TotalPoints", pointsCount);
        lifesCount += 1;
        PlayerPrefs.SetInt("Lifes", lifesCount);
    }
    public void UpgradeJokers()
    {
        if (jokerCount >= 5)
        {
            return;
        }
        if (pointsCount < 50)
        {
            return;
        }
        pointsCount -= 50;
        PlayerPrefs.SetInt("TotalPoints", pointsCount);
        jokerCount += 1;
        PlayerPrefs.SetInt("NumOfJokers", jokerCount);
    }
    public void GetInputName()
    {
        string name = InputName.GetComponent<InputField>().text;
        int normalLettersCounter = Regex.Matches(name, @"[a-zA-Z]").Count;
        if (name.Length != normalLettersCounter)
        {
            InputName.GetComponent<Image>().color = new Color(255, 0, 0);
            InputText.GetComponent<Text>().text = "Samo slova su dopuštena";
            return;
        }
        if(name.Length > 10)
        {
            InputName.GetComponent<Image>().color = new Color(255, 0, 0);
            InputText.GetComponent<Text>().text = "Unesite ime do 10 slova";
            return;
        }
        if (name.Length < 3)
        {
            InputName.GetComponent<Image>().color = new Color(255, 0, 0);
            InputText.GetComponent<Text>().text = "Unesite minimalno 3 slova";
            return;
        }
        else
        {
            InputName.SetActive(false);
            InputButton.SetActive(false);
        }
        PlayerPrefs.SetString("PlayerName", name);
        StartCoroutine(UploadNameAndScore(name));
    }
    IEnumerator UploadNameAndScore(string name)
    {
        int score = PlayerPrefs.GetInt("TotalPoints", 0);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        string mark = "";
        const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789"; //add the characters you want
        for (int i = 0; i < 30; i++)
        {
            mark += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
        }
        PlayerPrefs.SetString("Mark", mark);

        string url = "https://www.neoblast-official.com/sportquiz/index.php?name=" + name +"&highscore=" + score.ToString() + "&hash=neo&mark=" + mark;

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            StartCoroutine(UploadBestResultsGet());
        }
    }
    IEnumerator UploadNameAndNewScore()
    {
        int score = PlayerPrefs.GetInt("TotalPoints", 0);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        string mark = PlayerPrefs.GetString("Mark", " ");
        string name = PlayerPrefs.GetString("PlayerName", " ");

        string url = "https://www.neoblast-official.com/sportquiz/index.php?name=" + name + "&highscore=" + score.ToString() + "&hash=neoup&mark=" + mark;

        UnityWebRequest www = UnityWebRequest.Post(url, formData);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            StartCoroutine(UploadBestResultsGet());
        }
    }

    [Serializable]
    private class Result
    {
        public string mark;
        public string name;
        public string highscore;
    }
    [Serializable]
    private class ListResults
    {
        public Result[] results;
        public ListResults()
        {
            results = new Result[25];
        }
    }
    public GameObject userNames;
    public GameObject userScores;
    IEnumerator UploadBestResultsGet()
    {
        string url = "https://www.neoblast-official.com/sportquiz/getBestRecord.php?hash=neo";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                string result = webRequest.downloadHandler.text;
                ListResults tmp_objects = JsonUtility.FromJson<ListResults>(result);
                string text = "";
                for(int i = 0; i < tmp_objects.results.Length; i++)
                {
                    text += tmp_objects.results[i].name + "\n";
                }
                userNames.GetComponent<Text>().text = text;
                text = "";
                for (int i = 0; i < tmp_objects.results.Length; i++)
                {
                    text += tmp_objects.results[i].highscore + "\n";
                }
                userScores.GetComponent<Text>().text = text;
            }
        }
    }

    public void OpenStatistic()
    {
        UpgradesShopBox.SetActive(false);
        BestListBox.SetActive(false);
        StatisticBox.SetActive(true);
        Menu.SetActive(false);
    }
}

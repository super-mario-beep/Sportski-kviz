using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class QuizMainScript : MonoBehaviour
{
    //HEADER
    public int lifes;
    public GameObject lifesText;
    private int points;
    public GameObject pointsText;
    public GameObject menuPauseButton;
    public GameObject menuMauseBox;
    //QUESTIONS
    public GameObject mainBox;
    public GameObject mainBoxImage;
    public GameObject mainBoxImageText;
    public GameObject mainBoxText;
    public GameObject mainBoxImageType;
    public GameObject secondBox;
    public GameObject secondBoxImage;
    public GameObject secondBoxImageText;
    public GameObject secondBoxText;
    public GameObject secondBoxImageType;
    //ANSWERS
    public GameObject boolBox;
    public GameObject multyBox;
    public GameObject answerTrue;
    public GameObject answerFalse;
    public GameObject answer1;
    public GameObject answer2;
    public GameObject answer3;
    public GameObject answer4;
    public Sprite WrongSprite;
    public Sprite NormalSprite;
    public Sprite BoolTrueNormal;
    public Sprite BoolFalseNormal;
    public Sprite BoolTrueWrong;
    public Sprite BoolFalseWrong;
    //JOKERS
    public GameObject noLoseCountText;
    public GameObject skipCountText;
    public GameObject halfCountText;
    private int noLoseCount;
    private int skipCount;
    private int halfCount;
    public GameObject alertBox;
    public GameObject alertAboutJoker;
    public GameObject alertWarnText;
    private int jokerClicked = -1;
    private bool canLoseLifes = true;
    //OTHER
    public GameObject gameOverMenu;
    private int correntAnswer;
    private QuestionsHandler questionHanlder;
    private QuestionClass currentQuestionClass;
    private Color normalColorAnswer;
    private Color wrongColorAnswer;
    private Color correctColorAnswer;
    private List<int> answersClicked = new List<int>();
    private bool ClickingAvailable = true;
    public GameObject lifesAnimationObject;

    void Start()
    {
        lifes = PlayerPrefs.GetInt("Lifes", 3);
        points = 0;
        noLoseCount = PlayerPrefs.GetInt("NumOfJokers", 1);
        halfCount = PlayerPrefs.GetInt("NumOfJokers", 1);
        skipCount = PlayerPrefs.GetInt("NumOfJokers", 1);
        questionHanlder = new QuestionsHandler();
        normalColorAnswer = new Color(0, 0, 0);
        normalColorAnswer.a = 0.102f;
        wrongColorAnswer = new Color(255, 0, 0, 200);
        correctColorAnswer = new Color(0, 255, 0, 200);
        int played_games = PlayerPrefs.GetInt("GamesPlayed", 0);
        PlayerPrefs.SetInt("GamesPlayed", played_games + 1);

        SetQuestionToSceen();
    }

    void FixedUpdate()
    {
        UpdateLifesAndPoints();
        UpdateJokers();
    }

    private void UpdateLifesAndPoints()
    {
        lifesText.GetComponent<Text>().text = lifes.ToString();
        pointsText.GetComponent<Text>().text = points.ToString();

    }
    public void AlertJoker(int type)//0 - noLoseLife, 1 - skip question, 2 - Half Half
    {
        if (!ClickingAvailable)
        {
            return;
        }
        if (noLoseCount != 0 && type == 0)
        {
            alertBox.SetActive(true);
            alertAboutJoker.GetComponent<Text>().text = "Što radi? Ukoliko odgovorite netočno nećete izgubiti živote.";
            alertWarnText.GetComponent<Text>().text = "Jeste li sigurni da želite iskoristiti?";
            jokerClicked = 0;
        }
        else if(skipCount != 0 && type == 1)
        {
            alertBox.SetActive(true);
            alertAboutJoker.GetComponent<Text>().text = "Što radi? Preskačete trenutno pitanje i otvara Vam se drugo.";
            alertWarnText.GetComponent<Text>().text = "Jeste li sigurni da želite iskoristiti?";
            jokerClicked = 1;
        }
        else if(halfCount != 0 && type == 2 && currentQuestionClass.type != 0)
        {
            alertBox.SetActive(true);
            alertAboutJoker.GetComponent<Text>().text = "Što radi? Dva netočna odgovora ćemo ukloniti.";
            alertWarnText.GetComponent<Text>().text = "Jeste li sigurni da želite iskoristiti?";
            jokerClicked = 2;
        }
    }
    public void DiscardAlert()
    {
        alertBox.SetActive(false);
        jokerClicked = -1;
    }
    public void UseJoker()
    {
        int used_jokers = PlayerPrefs.GetInt("JokersUsed", 0);
        PlayerPrefs.SetInt("JokersUsed", used_jokers + 1);
        if (jokerClicked == 0)
        {
            noLoseCount--;
            canLoseLifes = false;
        }
        else if (jokerClicked == 1)
        {
            skipCount--;
            ClickingAvailable = false;
            answersClicked.Clear();
            NextQuestionAnimationAndClear();
            SetQuestionToSceen();
        }
        else if (jokerClicked == 2)
        {
            halfCount--;
            if(currentQuestionClass.type != 0)
            {
                List<GameObject> incorrectAnswers = new List<GameObject>();
                for(int i = 0; i < 4; i++)
                {
                    if (i != correntAnswer && i == 0)
                        incorrectAnswers.Add(answer1);
                    else if (i != correntAnswer && i == 1)
                        incorrectAnswers.Add(answer2);
                    else if (i != correntAnswer && i == 2)
                        incorrectAnswers.Add(answer3);
                    else if (i != correntAnswer && i == 3)
                        incorrectAnswers.Add(answer4);
                }
                int random = UnityEngine.Random.Range(0, 3);
                if(random == 0)
                {
                    incorrectAnswers[0].SetActive(false);
                    incorrectAnswers[1].SetActive(false);
                }else if( random == 1)
                {
                    incorrectAnswers[2].SetActive(false);
                    incorrectAnswers[1].SetActive(false);
                }
                else if (random == 2)
                {
                    incorrectAnswers[2].SetActive(false);
                    incorrectAnswers[0].SetActive(false);
                }
            }
        }
        DiscardAlert();
    }
    private void UpdateJokers()
    {
        noLoseCountText.GetComponent<Text>().text = noLoseCount.ToString();
        skipCountText.GetComponent<Text>().text = skipCount.ToString();
        halfCountText.GetComponent<Text>().text = halfCount.ToString();

    }
    public void SelectAnswer(int id)
    {
       
        if (!ClickingAvailable)
        {
            return;
        }
        if (!answersClicked.Contains(id))
        {
            answersClicked.Add(id);
            if (correntAnswer == id)
            {
                ClickingAvailable = false;
                answersClicked.Clear();
                answer1.SetActive(true);
                answer2.SetActive(true);
                answer3.SetActive(true);
                answer4.SetActive(true);
                points += 1;
                NextQuestionAnimationAndClear();
                SetQuestionToSceen();
            }
            else
            {
                IncorectAnswerSelected(id);
            }
        }
    }
    private void IncorectAnswerSelected(int id)
    {
        int tmp = PlayerPrefs.GetInt("Wrongs", 0);
        tmp += 1;
        PlayerPrefs.SetInt("Wrongs", tmp);
        if (canLoseLifes)
        {
            lifes -= 1;
        }
        if(lifes == 0)
        {
            gameOverMenu.SetActive(true);
        }
        else
        {
            if(id == 0 && currentQuestionClass.type == 0)
            {
                answerTrue.GetComponent<Image>().sprite = BoolTrueWrong;
            }else if(id == 1 && currentQuestionClass.type == 0)
            {
               answerFalse.GetComponent<Image>().sprite = BoolFalseWrong;
            }else if(id == 0)
            {
                answer1.GetComponent<Image>().sprite = WrongSprite;
            }
            else if(id == 1)
            {
                answer2.GetComponent<Image>().sprite = WrongSprite;
            }
            else if(id == 2)
            {
                answer3.GetComponent<Image>().sprite = WrongSprite;
            }
            else if(id == 3)
            {
                answer4.GetComponent<Image>().sprite = WrongSprite;
            }
        }

    }
    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }
    public void CountiniuePlay()
    {
        menuMauseBox.SetActive(false);
    }
    public void PauseGame()
    {
        menuMauseBox.SetActive(true);
        //OpenFullScreenAd();
    }
    public void BackToMenu()
    {
        int tmp_points = PlayerPrefs.GetInt("TotalPoints", 0);
        PlayerPrefs.SetInt("TotalPoints", tmp_points + points);
        int tmp_record = PlayerPrefs.GetInt("Record", 0);
        if (tmp_record < points)
            PlayerPrefs.SetInt("Record", points);
        SceneManager.LoadScene(0);
    }
    public void SetQuestionToSceen()
    {
        if(points % 8 == 0 && points >= 8)
        {
            OpenFullScreenAd();
        }
        if(points % 15 == 0 && points >= 15)
        {
            StartCoroutine(AddLifes(1));
        }
        int random = UnityEngine.Random.Range(0,17);
        if(random == 0)
        {
            int tmp = Random.Range(0, 5);
            if (tmp == 1)
                halfCount++;
            else if (tmp == 2)
                noLoseCount++;
            else if(tmp == 0)
                skipCount++;
            else if(tmp == 3 && points%10 != 0)
                StartCoroutine(AddLifes(1));

        }

        currentQuestionClass = questionHanlder.GetRandomQuestion(0);
        if(currentQuestionClass.type == 0)
        {
            mainBoxText.GetComponent<Text>().text = currentQuestionClass.text;
            correntAnswer = currentQuestionClass.correctAnswer;
            boolBox.SetActive(true);
            multyBox.SetActive(false);
            mainBoxText.SetActive(true);
            mainBoxImageType.SetActive(false);
        }
        else if(currentQuestionClass.type == 1)
        {
            correntAnswer = currentQuestionClass.correctAnswer;
            mainBoxImageText.GetComponent<Text>().text = currentQuestionClass.text;
            mainBoxImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(currentQuestionClass.imagePath);
            boolBox.SetActive(false);
            multyBox.SetActive(true);
            mainBoxText.SetActive(false);
            mainBoxImageType.SetActive(true);
            answer1.transform.GetChild(0).GetComponent<Text>().text = currentQuestionClass.answers[0];
            answer2.transform.GetChild(0).GetComponent<Text>().text = currentQuestionClass.answers[1];
            answer3.transform.GetChild(0).GetComponent<Text>().text = currentQuestionClass.answers[2];
            answer4.transform.GetChild(0).GetComponent<Text>().text = currentQuestionClass.answers[3];
        }
        else if(currentQuestionClass.type == 2)
        {
            correntAnswer = currentQuestionClass.correctAnswer;
            mainBoxText.GetComponent<Text>().text = currentQuestionClass.text;
            boolBox.SetActive(false);
            multyBox.SetActive(true);
            mainBoxText.SetActive(true);
            mainBoxImageType.SetActive(false);
            answer1.transform.GetChild(0).GetComponent<Text>().text = currentQuestionClass.answers[0];
            answer2.transform.GetChild(0).GetComponent<Text>().text = currentQuestionClass.answers[1];
            answer3.transform.GetChild(0).GetComponent<Text>().text = currentQuestionClass.answers[2];
            answer4.transform.GetChild(0).GetComponent<Text>().text = currentQuestionClass.answers[3];
        }
        questionHanlder.SetQuestionUsed(currentQuestionClass.id);
    }
    private void NextQuestionAnimationAndClear()
    {
        answer1.GetComponent<Image>().sprite = NormalSprite;
        answer2.GetComponent<Image>().sprite = NormalSprite;
        answer3.GetComponent<Image>().sprite = NormalSprite;
        answer4.GetComponent<Image>().sprite = NormalSprite;
        answerTrue.GetComponent<Image>().sprite = BoolTrueNormal;
        answerFalse.GetComponent<Image>().sprite = BoolFalseNormal;

        if (currentQuestionClass.type != 1)
        {
            secondBoxText.GetComponent<Text>().text = currentQuestionClass.text;
            secondBoxImageText.SetActive(false);
            secondBoxImage.SetActive(false);
            secondBoxText.SetActive(true);
        }
        else
        {
            secondBoxImageText.GetComponent<Text>().text = currentQuestionClass.text;
            secondBoxImage.GetComponent<Image>().sprite = Resources.Load<Sprite>(currentQuestionClass.imagePath);
            secondBoxText.GetComponent<Text>().text = currentQuestionClass.text;
            secondBoxImageText.SetActive(true);
            secondBoxImage.SetActive(true);
            secondBoxText.SetActive(false);
        }
        secondBox.transform.SetSiblingIndex(3);
        StartCoroutine(StartAnimation());

    }
    IEnumerator StartAnimation()
    {
        secondBox.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        secondBox.transform.SetSiblingIndex(0);
        StartCoroutine(FinishAnimation());
    }
    IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(2.15f);
        secondBox.SetActive(false);
        secondBox.transform.SetSiblingIndex(2);
        canLoseLifes = true;
        ClickingAvailable = true;

    }
    IEnumerator AddLifes(int count)
    {
        yield return new WaitForSeconds(1.80f);
        lifesAnimationObject.SetActive(true);
        StartCoroutine(FinishedAddLifes(count));

    }
    IEnumerator FinishedAddLifes(int count)
    {
        yield return new WaitForSeconds(1.85f);
        lifesAnimationObject.SetActive(false);
        lifes += count;
    }

    //ADS
    public GameObject FullScreenManager;
    public void OpenFullScreenAd()
    {
        FullScreenManager.GetComponent<AdManager>().ShowInterstitialAd();
    }
    public void KeepPlaying()
    {
        gameOverMenu.SetActive(false);
        SetQuestionToSceen();
        NextQuestionAnimationAndClear();
        StartCoroutine(AddLifes(1));
        ClickingAvailable = false;
        answersClicked.Clear();
    }
}

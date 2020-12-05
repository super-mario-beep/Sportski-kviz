using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class QuestionsHandler
{
    private static List<QuestionClass> stage1 = new List<QuestionClass>();
    private static List<QuestionClass> stage2 = new List<QuestionClass>();
    private static List<QuestionClass> stage3 = new List<QuestionClass>();
    private static List<int> questionsUsed = new List<int>();

        
    public QuestionsHandler()
    {
        QuestionsFile loadQuestions = new QuestionsFile();
        for(int i = 0; i < loadQuestions.questions.Count; i++)
        {
            QuestionClass tmp_object = JsonUtility.FromJson<QuestionClass>(loadQuestions.questions[i]);
            //if (tmp_object.stage == 1)
            stage1.Add(tmp_object);
            //else if (tmp_object.stage == 2)
                //stage2.Add(tmp_object);
            //else if (tmp_object.stage == 3)
                //stage3.Add(tmp_object);
        }
        LoadQuestionUsed();
    }

    public QuestionClass GetRandomQuestion(int stage)// 0, easy, 1 normal, 2 hard
    {
        //return stage1[234];
        for (int i = 0; i < 10; i++)
        {
            int tmp = UnityEngine.Random.Range(0, stage1.Count);
            if (!questionsUsed.Contains(stage1[tmp].id))
                return stage1[tmp];
        }
        for (int i = 0; i < stage1.Count; i++)
        {
            if (!questionsUsed.Contains(stage1[i].id))
                return stage1[i];
        }
        ClearQuestionsUsed();
        Debug.Log("Sva pitanja su procitata pokazuju se ponovo.");
        return GetRandomQuestion(stage);
    }

    private void ClearQuestionsUsed()
    {
        for (int i = 1; i <= stage1.Count + stage2.Count + stage3.Count; i++)
        {
            PlayerPrefs.SetInt(i.ToString(), 0);
        }
        questionsUsed.Clear();
    }
    private static void LoadQuestionUsed()
    {
        for(int i = 1; i <= stage1.Count + stage2.Count + stage3.Count; i++)
        {
            if(PlayerPrefs.GetInt(i.ToString(), 0) != 0)
            {
                questionsUsed.Add(i);
            }
        }
    }
    public void SetQuestionUsed(int id)
    {
        questionsUsed.Add(id);
        PlayerPrefs.SetInt(id.ToString(), id);
    }

}

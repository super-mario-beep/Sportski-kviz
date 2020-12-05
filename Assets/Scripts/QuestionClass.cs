using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class QuestionClass
{
    public int id;
    public int type; // 0 - bool, 1 - image, 2 - text
    public int stage;
    public string text;
    public string imagePath;
    public List<string> answers = new List<string>();
    public int correctAnswer;

    public QuestionClass(int _type, int _id, string _text, string _imagePath, List<string> _answers, int _stage, int _correctAnswer)
    {
        type = _type;
        id = _id;
        text = _text;
        imagePath = _imagePath;
        answers = _answers;
        stage = _stage;
        correctAnswer = _correctAnswer;
    }
    public QuestionClass() 
    { 
    
    }
}

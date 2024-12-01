using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] float vitesseRemplissageRepBar = 1; // ratio appliquer au delta time, pour gérer la vitesse de remplissage des barres rep lors d'une modif de leur valeur
    [SerializeField] private Image repGouvBar, repPeupleBar, repCorpBar;
    [SerializeField] float maxRep, startingRep;
    public int mailLeft, startingMail;
    [SerializeField] Text mailLeftText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float repGouv, repPeuple, repCorp;
    [SerializeField] Color goodRepColor, midleRepColor, badRepColor;
    public GameObject retryButton, optionPanel;

    public bool gameIsOver = false;

    public static UIManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("careful more than one instance of UIManager");
            return;
        }
        instance = this;
    }

    public void ActualisationUiBar()
    {
        //repGouvBar.fillAmount = repGouv / maxRep;
        //repCorpBar.fillAmount = repCorp / maxRep;
        //repPeupleBar.fillAmount = repPeuple / maxRep;

        StartCoroutine(DynamicRepBar(repGouvBar, repGouv)); 
        StartCoroutine(DynamicRepBar(repCorpBar, repCorp));
        StartCoroutine(DynamicRepBar(repPeupleBar, repPeuple));

        mailLeftText.text = mailLeft.ToString();
    }


    IEnumerator DynamicRepBar(Image fillBar, float targetAmont) // coroutine for get a dynamic Update for the fill bar 
    {
        targetAmont = targetAmont/maxRep; // target need to be between 0-1

        //Debug.Log(targetAmont + " / " + fillBar.fillAmount);
        while (fillBar.fillAmount >= targetAmont +0.02 || fillBar.fillAmount <= targetAmont - 0.02) // verifie si le remplissage de la barre de rep 
        {
            yield return null;
            if (fillBar.fillAmount <= targetAmont)
            {
                fillBar.fillAmount = fillBar.fillAmount + Time.deltaTime * vitesseRemplissageRepBar;
            }
            else if(fillBar.fillAmount >= targetAmont)
            {
                fillBar.fillAmount = fillBar.fillAmount - Time.deltaTime * vitesseRemplissageRepBar;
            }

            float ecartRep = Math.Abs(fillBar.fillAmount - 0.5f);

            if (ecartRep < 0.20f)
            {
                fillBar.color = goodRepColor;
            }
            else if (ecartRep < 0.35f)
            {
                fillBar.color = midleRepColor;
            }
            else if (ecartRep <= 0.5f)
            {
                fillBar.color = badRepColor;
            }

        }
    }

    public void UpdateGouvCorpPeuple(float modGouv, float modCorp, float modPeuple)
    {
        repGouv += modGouv;
        repPeuple += modPeuple;
        repCorp += modCorp;

 
    }

    public void DecreaseMail()
    {

        if (mailLeft > 0)
        {
            mailLeft -= 1;
            ActualisationUiBar();
        }

    }

    public ScriptableLetter CheckIfLose()
    {
        if (repGouv >= maxRep)
        {
            gameIsOver = true;
            return LetterManager.instance.GetSpecialLetter("HightGouv");

        }
        if (repGouv <= 0)
        {
            gameIsOver = true;
            return LetterManager.instance.GetSpecialLetter("LowGouv");
        }

        if (repCorp >= maxRep)
        {
            gameIsOver = true;
            return LetterManager.instance.GetSpecialLetter("HightCorp");
        }
        if (repCorp <= 0)
        {
            gameIsOver = true;
            return LetterManager.instance.GetSpecialLetter("LowCorp");
        }

        if (repPeuple >= maxRep)
        {
            gameIsOver = true;
            return LetterManager.instance.GetSpecialLetter("HightPeuple");
        }
        if (repPeuple <= 0)
        {
            gameIsOver = true;
            return LetterManager.instance.GetSpecialLetter("LowPeuple");
        }

        return null;
    }

    void Start()
    {
        Init();

    }

    private void Init()
    {
        repGouv = startingRep;
        repCorp = startingRep;
        repPeuple = startingRep;

        mailLeft = startingMail;

        ActualisationUiBar();
    }

    public void RetryButton()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenOptionButton()
    {
        optionPanel.SetActive(true);
        AudioScript.instance.LaunchSoundSFX(AudioScript.instance.buttonSFX);
    }

    public void CloseOptionButton()
    { 
        optionPanel.SetActive(false);
        AudioScript.instance.LaunchSoundSFX(AudioScript.instance.buttonSFX);
    }

    }






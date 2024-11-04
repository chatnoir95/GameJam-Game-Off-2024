using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using System.IO;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image repGouvBar, repPeupleBar, repCorpBar;
    [SerializeField] float maxRep, startingRep;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float repGouv, repPeuple, repCorp;

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
        repGouvBar.fillAmount = repGouv / maxRep;
        repCorpBar.fillAmount = repCorp / maxRep;
        repPeupleBar.fillAmount = repPeuple / maxRep;
    }


    public void UpdateGouvCorpPeuple(float modGouv, float modCorp, float modPeuple)
    {
        repGouv += modGouv;
        repPeuple += modPeuple;
        repCorp += modCorp;
    }

    public ScriptableLetter CheckIfLose()
    {
        if (repGouv >= maxRep)
        {
            return LetterManager.instance.GetSpecialLetter("HightGouv");
        }
        if (repGouv <= 0)
        {
            return LetterManager.instance.GetSpecialLetter("LowGouv");
        }

        if (repCorp >= maxRep)
        {
            return LetterManager.instance.GetSpecialLetter("HightCorp");
        }
        if (repCorp <= 0)
        {
            return LetterManager.instance.GetSpecialLetter("LowCorp");
        }

        if (repPeuple >= maxRep)
        {
            return LetterManager.instance.GetSpecialLetter("HightPeuple");
        }
        if (repPeuple <= 0)
        {
            return LetterManager.instance.GetSpecialLetter("LowPeuple");
        }

        return null;
    }

    void Start()
    {
        Init();
        ActualisationUiBar();
    }

    private void Init()
    {
        repGouv = startingRep;
        repCorp = startingRep;
        repPeuple = startingRep;
    }

        // Update is called once per frame
        void Update()
        {
        
        }



    }






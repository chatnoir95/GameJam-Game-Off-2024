using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    [SerializeField] private Image repGouvBar, repPeupleBar, repCorpBar;
    [SerializeField] float maxRep, startingRep;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float repGouv, repPeuple, repCorp;   
    
    
    private void ActualisationUiBar()
    {
        repGouvBar.fillAmount = repGouv / maxRep;
        repCorpBar.fillAmount = repCorp / maxRep;
        repPeupleBar.fillAmount = repPeuple / maxRep;
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

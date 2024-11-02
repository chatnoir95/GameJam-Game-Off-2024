using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterManager : MonoBehaviour
{
    public List<ScriptableLetter> lettreDisponibles = new List<ScriptableLetter>();

    public Text textAcceptButton, textRefuseButton, textContenueLettre, textExpediteur;

    private ScriptableLetter lettreActive;

    [SerializeField] private Image imageLogo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public ScriptableLetter TirageLettre()
    {
        if (lettreDisponibles.Count != 0)
        {
            int a = Random.Range(0, lettreDisponibles.Count);
            return lettreDisponibles[a];
        }
        else
        {
            Debug.LogWarning("attention list de tirage vide");
            return null;
        }

    }

    public void NouvelleLettre(ScriptableLetter scriptableLetter)
    {
        // applique le bon text pour les boutton de choix 
        textAcceptButton.text = scriptableLetter.accepteText;
        textRefuseButton.text = scriptableLetter.refuseText;
        
        // affiche le contenue de la lettre 
        textContenueLettre.text = scriptableLetter.contentLetter; 
        // affiche l'expediteur
        textExpediteur.text = scriptableLetter.expediteur;

        if (scriptableLetter.logoLetter != null)
        {
            imageLogo.sprite = scriptableLetter.logoLetter;
        }

        lettreActive = scriptableLetter;
    }

    // EN COURS ON CLICK 
    public void AcceptButton()
    {
        UIManager.instance.UpdateGouvCorpPeuple(lettreActive.acceptImpactGouv, lettreActive.acceptImpactCorp, lettreActive.acceptImpactPeuple);
        UIManager.instance.ActualisationUiBar();

        NouvelleLettre(TirageLettre());
    }

    public void RefuseButton()
    {
        UIManager.instance.UpdateGouvCorpPeuple(lettreActive.refuseImpactGouv, lettreActive.refuseImpactCorp, lettreActive.refuseImpactPeuple);
        UIManager.instance.ActualisationUiBar();

        NouvelleLettre(TirageLettre());
    }


    void Start()
    {
        NouvelleLettre(TirageLettre());

        //Debug.Log( TirageLettre().expediteur) ;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(TirageLettre().expediteur);
    }
}

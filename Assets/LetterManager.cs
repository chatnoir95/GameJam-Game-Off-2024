using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using System.IO;

public class LetterManager : MonoBehaviour
{
    public List<ScriptableLetter> lettreDisponibles = new List<ScriptableLetter>();
    
    public List<ScriptableLetter> followingLetters = new List<ScriptableLetter>();
    
    public List<ScriptableLetter> specialLetters = new List<ScriptableLetter>();

    public Animator letterAnimator;

    public Text textAcceptButton, textRefuseButton, textContenueLettre, textExpediteur;

    [SerializeField] private ScriptableLetter lettreActive;

    [SerializeField] private Image imageLogo;


    public static LetterManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("careful more than one instance of LetterManager");
            return;
        }
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public ScriptableLetter TirageLettre()
    {

        UIManager.instance.DecreaseMail();

        if (UIManager.instance.mailLeft <= 0 )
        {
            return GetSpecialLetter("WinClearAllLetter");
        }


        // check if value of rep is at a losing point, in that case return a special losing letter 
        if( UIManager.instance.CheckIfLose() != null )
        {
            UIManager.instance.retryButton.SetActive(true);
            return UIManager.instance.CheckIfLose();
        }
        

        if (followingLetters.Count != 0)
        {
            for (int i = followingLetters.Count -1; i >= 0; i--)
            {
                int randomInt = Random.Range(0, 100);
                if (randomInt < followingLetters[i].chanceToSpawn)
                {
                    ScriptableLetter lettre = followingLetters[i];
                    followingLetters.Remove(followingLetters[i]);
                    return lettre;
                }
            } 
        }




        if (lettreDisponibles.Count != 0)
        {
            int a = Random.Range(0, lettreDisponibles.Count);


            ScriptableLetter letter = lettreDisponibles[a];
            lettreDisponibles.Remove(lettreDisponibles[a]);
            return letter;
        }
        else
        {
            Debug.LogWarning("attention list de tirage vide");
            return null;
        }

    }

    public void NouvelleLettre(ScriptableLetter scriptableLetter)
    {
        letterAnimator.SetTrigger("newLetter");

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

    public ScriptableLetter GetSpecialLetter(string letterName)
    {
        foreach (ScriptableLetter letter in specialLetters)
        {
            if (letter.name == letterName)
            { return letter; }
        }

        Debug.LogWarning("special letter not found");
        return null;
    }

    // EN COURS ON CLICK 
    public void AcceptButton()
    {
        if (!UIManager.instance.gameIsOver)
        {
            AudioScript.instance.LaunchSoundSFX(AudioScript.instance.buttonSFX); // launch button SFX
            UIManager.instance.UpdateGouvCorpPeuple(lettreActive.acceptImpactGouv, lettreActive.acceptImpactCorp, lettreActive.acceptImpactPeuple); // apply rep change, 
            UIManager.instance.ActualisationUiBar();

            if (lettreActive.acceptUnlockLetters.Count != 0)
            {
                for (int i = 0; i < lettreActive.acceptUnlockLetters.Count; i++)
                {
                    followingLetters.Add(lettreActive.acceptUnlockLetters[i]);
                }
            }

            InfoManager.instance.NextInfo(lettreActive.ImpactAcceptText);// change info text with the consequence of the choice 
            NouvelleLettre(TirageLettre());

        }
    }

    public void RefuseButton()
    {
        if (!UIManager.instance.gameIsOver)
        {
            AudioScript.instance.LaunchSoundSFX(AudioScript.instance.buttonSFX);
            UIManager.instance.UpdateGouvCorpPeuple(lettreActive.refuseImpactGouv, lettreActive.refuseImpactCorp, lettreActive.refuseImpactPeuple);
            UIManager.instance.ActualisationUiBar();

            if (lettreActive.refuseUnlockLetters.Count != 0)
            {
                for (int i = 0; i < lettreActive.refuseUnlockLetters.Count; i++)
                {
                    followingLetters.Add(lettreActive.refuseUnlockLetters[i]);
                }
            }

            InfoManager.instance.NextInfo(lettreActive.ImpactRefuseText);
            NouvelleLettre(TirageLettre());
        }
    }




    [ContextMenu("Create Scriptable Objects From CSV")]
    public void CreateItemsFromCSV()
    {
        string csvFilePath = "Assets/Data/cards.csv";
        // Check if the file exists
        if (!File.Exists(csvFilePath))
        {
            Debug.LogError($"CSV file not found at path: {csvFilePath}");
            return;
        }

        // Reading the CSV file
        string[] csvLines = File.ReadAllLines(csvFilePath);
        //Debug.Log($"Loaded {csvLines.Length - 1} entries from CSV.");

        // Loop through each line (skip the header if it exists)
        for (int i = 1; i < csvLines.Length; i++)
        {
            string[] values = csvLines[i].Split(',');

            try
            {

                // Debug output of parsed values
                //Debug.Log($"Parsing line {i + 1}: {csvLines[i]}");



                // Création d'un nouvel objet scriptable pour chaque ligne
                ScriptableLetter newItem = ScriptableObject.CreateInstance<ScriptableLetter>();

                newItem.index = int.Parse(values[0]);

                newItem.expediteur = values[3];
               // Debug.Log($"Created ScriptableObject for 3 {values[3]}");
                newItem.contentLetter = values[4];
                newItem.accepteText = values[5];
                newItem.refuseText = values[6];


                //Debug.Log($"Created ScriptableObject for 4 {values[4]}");
                newItem.acceptImpactPeuple = float.Parse(values[7]);
                //Debug.Log($"Created ScriptableObject for 7 {values[7]}");
                newItem.acceptImpactGouv = float.Parse(values[8]);
                //Debug.Log($"Created ScriptableObject for 8 {values[8]}");
                newItem.acceptImpactCorp = float.Parse(values[9]);
                //Debug.Log($"Created ScriptableObject for 9 {values[9]}");
                newItem.refuseImpactPeuple = float.Parse(values[10]);
                //Debug.Log($"Created ScriptableObject for 10 {values[10]}");
                newItem.refuseImpactGouv = float.Parse(values[11]);
                //Debug.Log($"Created ScriptableObject for 11 {values[11]}");
                newItem.refuseImpactCorp = float.Parse(values[12]);
                //Debug.Log($"Created ScriptableObject for 12 {values[12]}");
                if (values[13] != "")
                {
                    newItem.chanceToSpawn = int.Parse(values[13]);
                }
                newItem.ImpactAcceptText = values[14];
                newItem.ImpactRefuseText = values[15];


                // Ajout d'imbrication
                newItem.acceptUnlockLetters = new List<ScriptableLetter>();
                newItem.refuseUnlockLetters = new List<ScriptableLetter>();

                // Saving the ScriptableObject as an asset
                string assetPath = $"Assets/Script/ScriptableChoice/{values[0]}.asset";
                AssetDatabase.CreateAsset(newItem, assetPath);
                //Debug.Log($"Created ScriptableObject for {values[0]} at {assetPath}");

                // add the new letter to the list
                if (values[1] != "")
                {
                    //Debug.Log("index a trouvé : " + values[1]);
                    for (int y = 0; y < lettreDisponibles.Count; y++)
                    {
                      //  Debug.Log("check ID : " + lettreDisponibles[y].index);
                        if (lettreDisponibles[y].index == int.Parse(values[1]))
                        {
                        //    Debug.Log("ID trouvé : " + lettreDisponibles[y].index);
                            if (values[2] != "")
                            {
                                if (int.Parse(values[2]) == 0)
                                {
                                    lettreDisponibles[y].refuseUnlockLetters.Add(newItem);
                                }
                                else if (int.Parse(values[2]) == 1)
                                {
                                    lettreDisponibles[y].acceptUnlockLetters.Add(newItem);
                                }
                            }
                            else
                            {
                                Debug.LogWarning("prerequis pas detaille ligne : " + i);
                            }
                        }

                    }
                }
                else { 

                    lettreDisponibles.Add(newItem); } // si aucun prerequis on ajoute a la liste lettre dispo 
                }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error parsing line {i + 1}: {csvLines[i]}\nException: {ex.Message}");
            }
        }

        // Refresh the asset database to display newly created assets
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("All Scriptable Objects have been created and saved.");
    }


    void Start()
    {

        CreateItemsFromCSV();

        NouvelleLettre(TirageLettre());

        //Debug.Log( TirageLettre().expediteur) ;
    }




    // Update is called once per frame
    void Update()
    {
        //Debug.Log(TirageLettre().expediteur);
    }
}

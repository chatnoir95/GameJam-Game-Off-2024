using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LetterManager : MonoBehaviour
{
    public List<ScriptableLetter> lettreDisponibles = new List<ScriptableLetter>();
    
    public List<ScriptableLetter> followingLetters = new List<ScriptableLetter>();
    
    public List<ScriptableLetter> specialLetters = new List<ScriptableLetter>();

    public List<ScriptableLetter> allLetter = new List<ScriptableLetter>();

    public Animator letterAnimator;

    public Text textAcceptButton, textRefuseButton, textContenueLettre, textExpediteur;

    [SerializeField] private ScriptableLetter lettreActive;

    [SerializeField] private Image imageLogo;

    public string folderPath = "Assets/Script/ScriptableChoice"; // path for all scriptable letter

    public static LetterManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("careful more than one instance of LetterManager");
            return;
        }
        instance = this;
        InitLetterList();
       // LoadScriptablLetter(); // found all letter and put them on good list // this not work when we try to build the project
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
    

        //CreateItemsFromCSV();

        NouvelleLettre(GetSpecialLetter("IntroductionLetter")); // start the game with the introduction Letter
        
        //Debug.Log( TirageLettre().expediteur) ;
    }
    /*
    public void LoadScriptablLetter() // take all letter and put them on allLetter List
    {
        allLetter.Clear(); // Vider la liste

        // Obtenir tous les GUID des fichiers dans le dossier
        string[] assetGUIDs = AssetDatabase.FindAssets($"t:{nameof(ScriptableLetter)}", new[] { folderPath });

        foreach (string guid in assetGUIDs)
        {
            
            // Convertir le GUID en chemin d'accès
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
           // Debug.Log(assetPath);

            // Charger le ScriptableObject
            ScriptableLetter letter = AssetDatabase.LoadAssetAtPath<ScriptableLetter>(assetPath);
            //Debug.Log(asset);

            if (letter != null)
            {
                allLetter.Add(letter);
            }
        }



        Debug.Log($"Loaded {allLetter.Count} ScriptableObjects from {folderPath}");
    }
    */

    public void InitLetterList()
    {
        foreach (ScriptableLetter letter in allLetter) // check if letter have a prerequis, if yes we add it to the good list of the prerequis letter 
        {
            if (letter.idPrerequis == 0 && !letter.isSpecialLetter) // check if letter as no prerequisite and is not a special letter (like tuto or game over letter)
            {
                lettreDisponibles.Add(letter);
            }
            else if (letter.isSpecialLetter)
            {
                specialLetters.Add(letter); // add special letter to the list, this list will be use for special letter (like game over or tuto)
            }

            if (letter.idPrerequis != 0)
            {
                if (letter.decisionPrerequis == 0)
                {
                    GetLetterById(letter.idPrerequis).refuseUnlockLetters.Clear();
                    GetLetterById(letter.idPrerequis).refuseUnlockLetters.Add(letter);
                }
                else if (letter.decisionPrerequis == 1)
                {
                    GetLetterById(letter.idPrerequis).acceptUnlockLetters.Clear();
                    GetLetterById(letter.idPrerequis).acceptUnlockLetters.Add(letter);
                }

            }

        }
    }

    public ScriptableLetter TirageLettre()
    {

        UIManager.instance.DecreaseMail();

        if (UIManager.instance.mailLeft <= 0 )
        {
            UIManager.instance.gameIsOver = true;
            UIManager.instance.retryButton.SetActive(true);
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

    private ScriptableLetter GetLetterById(int id)
    {
        foreach (ScriptableLetter letter in allLetter)
        {
            if (letter.index == id)
            { return letter; }
        }

        Debug.LogWarning("letter not found with id search");
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


    private string ChangeSemicolon(string text) // change the ';' of string by ',' 
    {
        char charToChange = ';';
        char newChar = ',';

        text = text.Replace(charToChange, newChar);
        return text;
    }


    /*
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

                newItem.expediteur = ChangeSemicolon(values[3]);
                // Debug.Log($"Created ScriptableObject for 3 {values[3]}");
                newItem.contentLetter = ChangeSemicolon(values[4]);
                newItem.accepteText = ChangeSemicolon(values[5]);
                newItem.refuseText = ChangeSemicolon(values[6]);


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
                newItem.ImpactAcceptText = ChangeSemicolon(values[14]);
                newItem.ImpactRefuseText = ChangeSemicolon(values[15]);

                if (values[2] != "")
                {
                    newItem.decisionPrerequis = int.Parse(values[2]);
                }
                else
                {
                    newItem.decisionPrerequis = 3;
                }
               

                newItem.idPrerequis = int.Parse(values[1]);
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
                if (values[1] == "") 
                {
                    //newItem.startInPool = true; // this letter will be add on disponibleLetter list at the start // why this is not working with bool value ?
                    //newItem.startPool = 1;
                    Debug.Log(newItem.idPrerequis);
                   lettreDisponibles.Add(newItem);  // si aucun prerequis on ajoute a la liste lettre dispo 

                }
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


    */



    // Update is called once per frame
    void Update()
    {
        //Debug.Log(TirageLettre().expediteur);
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "letter", menuName = "ScriptableObject/letter")]
public class ScriptableLetter : ScriptableObject
{
    public string expediteur, contentLetter, accepteText, refuseText;
    public Sprite logoLetter;
    public float acceptImpactCorp, acceptImpactPeuple, acceptImpactGouv, refuseImpactCorp, refuseImpactPeuple, refuseImpactGouv;

    public List<ScriptableLetter> acceptUnlockLetters, refuseUnlockLetters;

}

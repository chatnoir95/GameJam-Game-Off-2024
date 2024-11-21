using UnityEngine;
using UnityEngine.UI;

public class InfoManager : MonoBehaviour
{
    public Text infoText;
    public string newTextInfo;
    [SerializeField] Animator infoPanelAnimator;

    public static InfoManager instance;
    private void Awake() // singleton
    {
        if (instance != null)
        {
            Debug.LogWarning("careful more than one instance of InfoManager");
            return;
        }
        instance = this;
    }

    public void NextInfo(string newInfo)
    {
        newTextInfo = newInfo; // value use a the midle of Info animation, for change text 
        infoPanelAnimator.SetTrigger("newInfoPanel");
        
    }

    public void changeTextInfo() // this is call at midle off changeInfo animation 
    {
        infoText.text = newTextInfo;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

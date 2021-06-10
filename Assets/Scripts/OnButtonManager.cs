using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonManager : MonoBehaviour
{
    public void OnResetScore() 
    {
        PlayerPrefs.DeleteKey("ScorePlayer1");
        PlayerPrefs.DeleteKey("ScorePlayer2");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.NetworkVariable;
using MLAPI.Messaging;
using UnityEngine.UI;
using UnityEngine.Networking.Types;

public class CharacterBehav : NetworkBehaviour
{
    Text p1ScoreText;
    Text p2ScoreText;
    int p1Score;
    int p2Score;

    Text whoIsWinner;
    GameObject textWinner;
    CharacterControl characterControl;
    private void Awake()
    {
        p1ScoreText = GameObject.Find("ScorePlayer1").GetComponent<Text>();
        p2ScoreText = GameObject.Find("ScorePlayer2").GetComponent<Text>();
        p1Score = PlayerPrefs.GetInt("ScorePlayer1");
        p2Score = PlayerPrefs.GetInt("ScorePlayer2");
        characterControl = gameObject.GetComponent<CharacterControl>();
        textWinner = GameObject.FindGameObjectWithTag("Winner");
        whoIsWinner = GameObject.FindGameObjectWithTag("Winner").GetComponent<Text>();
    }
    [SerializeField]
    private NetworkVariableBool finish = new NetworkVariableBool(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.ServerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    }, false);

    [ServerRpc]
    public void ChangeScoreServerRpc(bool value)
    {
        finish.Value = value;
        Debug.Log("Finish");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLocalPlayer) { return; }
        if (collision.gameObject.tag == "Obstacle")
        {
            gameObject.GetComponent<PlayerSpawner>().Respawn();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsLocalPlayer) { return; }
        if (other.gameObject.tag == "Finish")
        {
            
            ChangeScoreServerRpc(true);
        }
    }
    private void OnEnable()
    {
        finish.OnValueChanged += OnScoreChanged;
    }
    private void OnDisable()
    {
        finish.OnValueChanged -= OnScoreChanged;

    }
    void OnScoreChanged(bool oldValue, bool newValue)
    {
        if (!IsClient) { return; }
        Debug.LogFormat("{0} >> Old Finish : {1} >> new Finish : {2}",
            gameObject.name, oldValue, newValue);
        if (oldValue == newValue) { return; }
        if (IsOwnedByServer)
        {
            Debug.Log("P1 : winner");
            p1Score++;
            PlayerPrefs.SetInt("ScorePlayer1", p1Score);
            whoIsWinner.text = "Player1 Win!!";
            characterControl.enabled = false;
            StartCoroutine(endGame());
        }
        else
        {
            Debug.Log("P2 : winner");
            p2Score++;
            PlayerPrefs.SetInt("ScorePlayer2", p2Score);
            whoIsWinner.text = "Player2 Win!!";
            characterControl.enabled = false;
            StartCoroutine(endGame());
        }
    }
    private void Update()
    {

            p2ScoreText.text = "P2 : " + PlayerPrefs.GetInt("ScorePlayer2");

            p1ScoreText.text = "P1 : " + PlayerPrefs.GetInt("ScorePlayer1");

    }
    IEnumerator endGame() 
    {
        yield return new WaitForSeconds(5);
        NetworkManager.StopServer();
    }
}

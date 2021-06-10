using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class PlayerSpawner : NetworkBehaviour
{
    CharacterControl characterControl;
    Renderer[] renderers;
    public Behaviour[] scripts;
    Transform hostPos, clientPos;

    void Start()
    {

        characterControl = gameObject.GetComponent<CharacterControl>();
        renderers = GetComponentsInChildren<Renderer>();
        hostPos = GameObject.FindGameObjectWithTag("HostPosition").GetComponent<Transform>();
        clientPos = GameObject.FindGameObjectWithTag("ClientPosition").GetComponent<Transform>();

    }

    public void Respawn()
    {
        if (IsOwnedByServer)
        {
            RespawnServerRpc(hostPos.position);
        }
        else
        {
            RespawnServerRpc(clientPos.position);
        }
    }
    [ServerRpc]
    public void RespawnServerRpc(Vector3 spawnPos)
    {
        RespawnClientRpc(spawnPos);
    }
    [ClientRpc]
    void RespawnClientRpc(Vector3 spawnPos)
    {
        
        StartCoroutine(RespawnCoroutine(spawnPos));
    }
    IEnumerator RespawnCoroutine(Vector3 spawnPos)
    {
        characterControl.enabled = false;
        setPlayerStare(false);
        yield return new WaitForSeconds(0.5f);
        this.transform.position = spawnPos;
        characterControl.enabled = true;
        setPlayerStare(true);
    }

    void setPlayerStare(bool state)
    {
        foreach (var script in scripts)
        {
            script.enabled = state;
        }
        foreach (var renderer in renderers)
        {
            renderer.enabled = state;
        }
    }
    private void Update()
    {
        Debug.Log(hostPos);
        Debug.Log(clientPos);
    }
}

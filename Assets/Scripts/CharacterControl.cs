using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using UnityEngine.UI;

public class CharacterControl : NetworkBehaviour
{
    public CharacterController controller;
    [SerializeField] float speed = 6f;
    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;

    [SerializeField] float turnsmoothTime = 0.1f;
    float turnsmoothVelocity;

    public Text namePrefab;
    private Text nameLabel;
    string TextBoxName = "";

    public Transform cam;

    //public NetworkVariableVector3 Position = new NetworkVariableVector3(new NetworkVariableSettings
    //{

    //    WritePermission = NetworkVariablePermission.ServerOnly,
    //    ReadPermission = NetworkVariablePermission.Everyone

    //});
    private NetworkVariable<string> playerName = new NetworkVariable<string>(new NetworkVariableSettings
    { WritePermission = NetworkVariablePermission.OwnerOnly }, "Player");

    private void OnEnable()
    {
        playerName.OnValueChanged += OnPlayerNameChanged;
        if (nameLabel != null)
        {
            nameLabel.enabled = true;
        }

    }
    private void OnDisable()
    {
        playerName.OnValueChanged -= OnPlayerNameChanged;
        if (nameLabel != null)
        {
            nameLabel.enabled = false;
        }
    }
    void OnPlayerNameChanged(string oldValue, string newValue)
    {
        if (!IsClient)
        {
            return;
        }
        gameObject.name = newValue;

    }
    [ServerRpc]
    public void SetPlayerNameServerRpc(string name)
    {
        playerName.Value = name;
    }

    public void SetPlayerName()
    {
        if (!IsOwner) return;
        if (NetworkManager.Singleton.IsServer)
        {
            playerName.Value = "Player1";
        }
        else
        {
            playerName.Value = "Player2";
        }
    }
    public override void NetworkStart()
    {
        GameObject canvas = GameObject.FindWithTag("MainCanvas");
        nameLabel = Instantiate(namePrefab, Vector3.zero, Quaternion.identity) as Text;
        nameLabel.transform.SetParent(canvas.transform);
        if (IsLocalPlayer)
        {
            CameraFollow360.player = this.gameObject.transform;
        }
    }
    private void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    void Update()
    {
        Vector3 nameLabelPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1.3f, 0));
        nameLabel.text = gameObject.name;
        nameLabel.transform.position = nameLabelPosition;
    }
    private void FixedUpdate()
    {
        if (!IsLocalPlayer)
            return;
        moveCharacter();
    }
    void moveCharacter() 
    {
        Vector3 direction;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg +cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnsmoothVelocity, turnsmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0f);

            controller.Move(direction * speed * Time.deltaTime);
        }
    }
    private void OnDestroy()
    {
        if (nameLabel != null)
        {
            Destroy(nameLabel.gameObject);
        }
    }
}

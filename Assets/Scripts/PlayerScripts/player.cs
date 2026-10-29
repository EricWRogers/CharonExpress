using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class player : MonoBehaviour
{
    public float speed;
    public float groundDist;
    public float x = 0;
    public float y = 0;


    public LayerMask ground;
    public Rigidbody rb;
    public SpriteRenderer sr;
    static public bool freeze = false;
    public PlayerThings inputActions;
    public GameObject playerObj;
    public  bool buttonBufferPress = false;
    public  bool buttonBuffer = false;
    public float buttonBufferTimer = 0.1f;
    public float nextAllowedClickTime;


    private void Awake()
    {
        inputActions = new PlayerThings();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        inputActions.Player.Interact.performed += OnInteract;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerObj = gameObject;
    }

    void Update()
    {
        if (!freeze)
        {
            RaycastHit hit;
            Vector3 castPos = transform.position;
            castPos.y += 1;
            if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, ground))
            {
                if (hit.collider != null)
                {
                    Vector3 movePos = transform.position;
                    movePos.y = hit.point.y + groundDist;
                    transform.position = movePos;
                }
            }

            Vector3 moveDir = new Vector3(x, 0, y);
            rb.linearVelocity = moveDir * (inputActions.Player.Sprint.IsPressed() ? speed * 1.5f : speed);

            if (x != 0 && x < 0)
            {
                sr.flipX = true;
            }
            else if (x != 0 && x > 0)
            {
                sr.flipX = false;
            }
        }
        if (buttonBufferPress)
        {
            WaitForButtonBuffer();
        }
    }
    public void OnMove(CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        x = input.x;
    }
    public void OnInteract(CallbackContext ctx)
    {
        Debug.Log("interact clicked");
        if (!buttonBuffer)
        {
            playerObj.GetComponent<Interact>().OnInteract();
        }
        buttonBufferPress = true;
    }

    public void WaitForButtonBuffer()
    {
        if(!buttonBuffer)
            StartCoroutine(Wait1Sec());
    }
    IEnumerator Wait1Sec()
    {
        float elapsedTime = 0f;
        float duration = buttonBufferTimer;



        while (elapsedTime < duration)
        {
            buttonBuffer = true;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonBuffer = false;
        buttonBufferPress = false;
        Debug.Log("button should buffer");
    }
}

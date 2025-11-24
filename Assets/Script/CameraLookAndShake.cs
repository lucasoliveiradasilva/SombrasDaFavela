using UnityEngine;

public class CameraLookAndShake : MonoBehaviour
{
    // ───────────────────────────────────────────────────────────────
    //  ► CONFIG DO LOOK (MOUSE)
    // ───────────────────────────────────────────────────────────────
    [Header("Mouse Look")]
    public float sensitivity = 150f;
    public Transform playerBody;

    private float verticalRotation = 0f;
    private Quaternion mouseBaseRotation;

    // ───────────────────────────────────────────────────────────────
    //  ► CONFIGURAÇÕES DO SHAKE (Amenti)
    // ───────────────────────────────────────────────────────────────
    [Header("Movimento Base (sempre ativo)")]
    public float positionAmount = 0.05f;
    public float rotationAmount = 0.8f;
    public float positionSpeed = 2f;
    public float rotationSpeed = 3f;

    [Header("Ruído Procedural (Amenti)")]
    public float noiseFrequency = 1.5f;
    public float noiseAmount = 0.03f;

    [Header("Movimento de Caminhada")]
    public float walkPositionAmount = 0.05f;
    public float walkRotationAmount = 1.5f;
    public float walkSpeed = 8f;

    [Header("Suavização")]
    public float blendSpeed = 6f;

    [Header("Player")]
    public GameObject player;
    private CharacterController cc;

    private float shakeBlend = 0f;
    private Vector3 baseLocalPos;

    // ───────────────────────────────────────────────────────────────
    //  ► START
    // ───────────────────────────────────────────────────────────────
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        baseLocalPos = transform.localPosition;

        if (player != null)
            cc = player.GetComponent<CharacterController>();
    }

    // ───────────────────────────────────────────────────────────────
    //  ► UPDATE
    // ───────────────────────────────────────────────────────────────
    void Update()
    {
        HandleMouseLook();
        HandleCameraShake();
    }

    // ───────────────────────────────────────────────────────────────
    //  ► FUNÇÃO: LOOK COM MOUSE
    // ───────────────────────────────────────────────────────────────
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);

        // Rotação vertical (só camera)
        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Rotação horizontal (gira player)
        if (playerBody != null)
            playerBody.Rotate(Vector3.up * mouseX);

        // guarda rotação base
        mouseBaseRotation = transform.localRotation;
    }

    // ───────────────────────────────────────────────────────────────
    //  ► FUNÇÃO: SHAKE + HEADBOB (Amenti)
    // ───────────────────────────────────────────────────────────────
    void HandleCameraShake()
    {
        float t = Time.time;

        // detectar se está andando
        bool isMoving = false;

        if (cc != null)
            isMoving = cc.velocity.magnitude > 0.1f;

        float target = isMoving ? 1f : 0f;
        shakeBlend = Mathf.Lerp(shakeBlend, target, Time.deltaTime * blendSpeed);

        // movimento suave
        float px = Mathf.Sin(t * positionSpeed) * positionAmount;
        float py = Mathf.Cos(t * positionSpeed * 0.8f) * positionAmount;

        float rx = Mathf.Sin(t * rotationSpeed) * rotationAmount;
        float ry = Mathf.Cos(t * rotationSpeed * 1.1f) * rotationAmount;

        // ruído procedural
        float nx = (Mathf.PerlinNoise(t * noiseFrequency, 0f) - .5f) * noiseAmount;
        float ny = (Mathf.PerlinNoise(0f, t * noiseFrequency) - .5f) * noiseAmount;

        float nrx = (Mathf.PerlinNoise(t * noiseFrequency, 2f) - .5f) * noiseAmount;
        float nry = (Mathf.PerlinNoise(3f, t * noiseFrequency) - .5f) * noiseAmount;

        // headbob andando
        float wbX = Mathf.Sin(t * walkSpeed) * walkPositionAmount * shakeBlend;
        float wbY = Mathf.Cos(t * walkSpeed * 2f) * walkPositionAmount * shakeBlend;

        float wrX = Mathf.Sin(t * walkSpeed) * walkRotationAmount * shakeBlend;
        float wrY = Mathf.Cos(t * walkSpeed * 1.3f) * walkRotationAmount * shakeBlend;

        // aplica posição (SEM apagar o mouse look)
        transform.localPosition =
            baseLocalPos +
            new Vector3(px + nx + wbX, py + ny + wbY, 0f);

        // aplica rotação somada com mouse look
        transform.localRotation =
            mouseBaseRotation *
            Quaternion.Euler(rx + nrx + wrX, ry + nry + wrY, 0f);
    }
}


using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input from WASD keys
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey(KeyCode.W))
            moveVertical = 1f;
        if (Input.GetKey(KeyCode.S))
            moveVertical = -1f;
        if (Input.GetKey(KeyCode.D))
            moveHorizontal = 1f;
        if (Input.GetKey(KeyCode.A))
            moveHorizontal = -1f;

        // Normalize the movement vector to ensure consistent speed in all directions
        Vector2 movement = new Vector2(moveHorizontal, moveVertical).normalized;

        // Move the player
        rb.velocity = movement * moveSpeed;
    }
}
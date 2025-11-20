using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Interacao : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public GameObject buyButton;
    private GameObject currentDoor;
    private Rigidbody2D rb;
    public float speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Esconde o texto e botão no início 
        messageText.gameObject.SetActive(false);
        buyButton.SetActive(false);  
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        rb.MovePosition(rb.position + movement.normalized * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PortaTrigger"))
        {
            currentDoor = other.transform.parent.gameObject; // Pega o objeto pai (a porta)
            Debug.Log("Player próximo à porta!");

            // Muda a cor da porta para branco
            SpriteRenderer spriteRenderer = currentDoor.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }

            // Mostra o texto e o botão
            messageText.gameObject.SetActive(true);
            messageText.text = "Desbloquear Porta por 500 moedas?";
            buyButton.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PortaTrigger"))
        {
            // Volta a cor original da porta (vermelho)
            SpriteRenderer spriteRenderer = currentDoor.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red; // Mude para a cor original da sua porta
            }

            messageText.gameObject.SetActive(false);
            buyButton.SetActive(false);
            currentDoor = null;
        }
    }

     // Método para ser chamado pelo botão
    public void BuyDoor()
    {
        Debug.Log("BuyDoor chamado!");
        if (currentDoor != null)
        {
            Debug.Log("Comprando a porta!");
            // Desativa a UI primeiro
            messageText.gameObject.SetActive(false);
            buyButton.SetActive(false);
            // Destroi o objeto da porta
            Destroy(currentDoor);
            currentDoor = null;
        }
        else
        {
            Debug.Log("currentDoor é null!");
        }
    }
}

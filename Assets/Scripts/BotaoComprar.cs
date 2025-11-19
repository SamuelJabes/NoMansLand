using UnityEngine;

public class BotaoComprar : MonoBehaviour
{
    public void OnClickComprar()
    {
        Debug.Log("Botão clicado!");
        // Encontra o Player e chama o método BuyDoor
        Interacao player = FindObjectOfType<Interacao>();
        if (player != null)
        {
            player.BuyDoor();
        }
        else
        {
            Debug.LogError("Script Interacao não encontrado!");
        }
    }
}

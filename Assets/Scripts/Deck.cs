using UnityEngine;
using UnityEngine.UI;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;

    public int[] values = new int[52];
    int cardIndex = 0;    
       
    private void Awake()
    {    
        InitCardValues();        

    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        int palo = 0; // Creamos una variable para el valor de las cartas en cada palo.

        // Creamos un for hasta 52 que es el tamaño de values
        for (int i = 0; i < 52; i++)
        {
            palo++; // Sumamos 1

            if (palo <= 10) // Comprobamos si el número de la variable palo es menor o igual a 10
            {
                if (palo == 1) // Comprobamos si es un AS
                {
                    values[i] = 11; // En caso de que si, el valor de esa carta será 11
                }
                else 
                {
                    values[i] = palo; // En caso de que no, el valor de esa carta será su número
                }
                
            }
            else // En caso de que no, sabemos que se trata de J, Q o K
            {
                values[i] = 10; // Entonces les pondremos el valor de 10
                if (palo == 13) palo = 0; // Y por último encaso de que palo sea 13, significa que se ha terminado ese palo y lo volvemos a 0
            }

            //Debug.Log(values[i].ToString());
        }
    }

    private void ShuffleCards()
    {
        int rnd; // Creamos una variable donde va ha ir el número random
        int temp; // Aquí guardaremos el número que quitamos para poner otro aleatorio
        Sprite tempSprite; // Aquí hacemos lo mismo para el Sprite

        for (int i = 51; i >= 0; i--)
        {
            //Debug.Log("INI: " + values[i] + "; " + faces[i]);

            rnd = Random.Range(0, 52); // Generamos el número aleatorio.

            // Hacemos el proceso para el array de valores
            temp = values[i]; // guardamos el número actual en la variable temporal.
            values[i] = values[rnd]; // ponemos en el hueco actual el número que corresponde con el aleatorio.
            values[rnd] = temp; // Y en el sitio del aleatorio ponemos el actual.

            // Hacemos lo mismo para el array de Sprites
            tempSprite = faces[i];
            faces[i] = faces[rnd];
            faces[rnd] = tempSprite;

            //Debug.Log("RND: " + values[i] + "; " + faces[i]);
        }

    }

    void StartGame()
    {
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
        }
        if (player.GetComponent<CardHand>().points == 21) finalMessage.text = "El Jugador ha hecho Blackjack";
        if (dealer.GetComponent<CardHand>().points == 21) finalMessage.text = "El Dealer ha hecho Blackjack";

    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true); // Volteamos la carta del Dealer

       //Repartimos carta al jugador
       PushPlayer();

        if (player.GetComponent<CardHand>().points == 21) // Comprobamos si ha hecho blackjack
        {
            // En caso de que si, mostramos mensaje y anulamos los botones
            finalMessage.text = "El Jugador ha hecho Blackjack";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
        if (player.GetComponent<CardHand>().points > 21) // Comprobamos si ha perdido
        {
            // En caso de que si, mostramos mensaje y anulamos los botones
            finalMessage.text = "El Jugador ha perdido";
            hitButton.interactable = false;
            stickButton.interactable = false;
        }
    }

    public void Stand()
    {
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true); // Volteamos carta del Dealer

        bool fin = false;

        do // Creamos un bluce que se hace hasta que el Dealer tiene una puntuación mayor a 16
        {
            if (dealer.GetComponent<CardHand>().points <= 16) // Comprobamos si tiene menos de 16 puntos
            {
                PushDealer(); // En caso de ser así, repartimos carta
            }
            else
            {
                if (player.GetComponent<CardHand>().points < dealer.GetComponent<CardHand>().points && dealer.GetComponent<CardHand>().points <= 21) // Comprobamos si el juegador ha ganado
                {
                    finalMessage.text = "El Dealer ha GANADO";
                }
                else
                {
                    finalMessage.text = "El Juegador ha GANADO";
                }

                if (player.GetComponent<CardHand>().points == dealer.GetComponent<CardHand>().points) // Comprobamos si han quedado en empate
                {
                    finalMessage.text = "EMPATE";
                }
                fin = true; // Salimos del bucle
                hitButton.interactable = false;
                stickButton.interactable = false;
            }
        } while (!fin);

    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }
    
}

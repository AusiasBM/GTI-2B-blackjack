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
        if (player.GetComponent<CardHand>().points == 21) finalMessage.text = "El Juegador ha hecho Blackjack";
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
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        
        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */      

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */                
         
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

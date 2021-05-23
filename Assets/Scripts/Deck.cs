﻿using UnityEngine;
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
    public Text bancaMessage;
    public Text apuestaMessage;

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


        // - Probabilidad de que el jugador obtenga más de 21 si pide una carta

        string probMayor21 = "";

        if (player.GetComponent<CardHand>().points < 12) // Al no haber ninguna carta mayor a 10 puntos no se puede sobrepasar los 21, si pensamos en el as lo tratamos como 1
        {
            probMayor21 = "\n - Probabilidad de que obtenga más de 21 si pide otra carta: 0.0";
        }else if (player.GetComponent<CardHand>().points >= 12) // Si tiene más de 11 ya hay cartas que sobrepasan los 21
        {
            int puntosParaPasarse = 22 - player.GetComponent<CardHand>().points;
            float cartasQueSobrepasan = 12; // Si es mayor que 11, automáticamente los 3 dieces de cada palo sobrepasan los 21

            // En este for sumamos las cartas posibles que pueden hacer que tenga una puntuación mayor a 21 sin contar los dieces
            for (int i = puntosParaPasarse; i <= 9; i++)
            {
                cartasQueSobrepasan += 4;
            }

            // Hacemos este for para recorrer todas la cartas que tiene el dealer
            for (int i = 0; i < dealer.GetComponent<CardHand>().cards.Count; i++)
            {
                // En este if comprobamos si la carta del delaer que está hacia arriba es una de las cartas que podría obtener el jugador si coge otra carta
                if (dealer.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value >= puntosParaPasarse)
                {
                    cartasQueSobrepasan -= 1; // En caso de que si que la hayamos contado restamos esa carta porque no podría salir
                }
            }


            // Lo mismo pero para el player
            for (int i = 0; i < player.GetComponent<CardHand>().cards.Count; i++)
            {
                // En este if comprobamos si las cartas del player son unas de las cartas que podría obtener el player si coge otra carta
                if (player.GetComponent<CardHand>().cards[i].GetComponent<CardModel>().value >= puntosParaPasarse)
                {
                    cartasQueSobrepasan -= 1; // En caso de que si que la hayamos contado restamos esa carta porque no podría salir
                }
            }

            Debug.Log("puntos para pasarse: " + puntosParaPasarse + ", cartas que sobrepasan: " + cartasQueSobrepasan + ", Cartas en la mesa: " + cardIndex);

            // Calculamos la probabilidad
            probMayor21 = "\n - Probabilidad de que obtenga más de 21 si pide otra carta: " + cartasQueSobrepasan / (52.0 - cardIndex * 0.0);

        }



        probMessage.text = probMayor21;

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
                    player.GetComponent<CardHand>().banca += player.GetComponent<CardHand>().apuesta * 2; // Gana el doble
                }

                if (player.GetComponent<CardHand>().points == dealer.GetComponent<CardHand>().points) // Comprobamos si han quedado en empate
                {
                    finalMessage.text = "EMPATE";
                    player.GetComponent<CardHand>().banca += player.GetComponent<CardHand>().apuesta; // Se queda con lo que ha venido
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
        bancaMessage.text = "Banca: " + player.GetComponent<CardHand>().banca.ToString() + " €";
        apuestaMessage.text = "Apuesta: 0 €";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }

    public void apostar()
    {
        if (player.GetComponent<CardHand>().banca != 0) // Comprobamos que tiene dinero
        {
            player.GetComponent<CardHand>().banca -= 10; // quitamos a la banca
            player.GetComponent<CardHand>().apuesta += 10; // Sumamos a la apuesta
            bancaMessage.text = "Banca: " + player.GetComponent<CardHand>().banca.ToString() + " €"; // Escibimos
            apuestaMessage.text = "Apuesta: " + player.GetComponent<CardHand>().apuesta.ToString() + " €";
        }

    }

    
}

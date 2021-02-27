using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameController gameController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2Int moveDirection = new Vector2Int();
        //TODO
        //w zaleznosci od wcisnietych kalawiszy ustaw moveDirection na jedno z (1, 0), (-1, 0), (0, 1), (0, -1)

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection = new Vector2Int(0, 1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection = new Vector2Int(0, -1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection = new Vector2Int(-1,0);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection = new Vector2Int(1, 0);
        }

            //jeżeli jakis ruch chcemy wykonac to informujemy gameController o probie ruchu
            if (moveDirection != Vector2Int.zero) 
            {
                gameController.TryMovePlayer(moveDirection);
                //udalo sie zrobic ruch  
                moveDirection = Vector2Int.zero; //zeruje i czekam na nastepne ustawienie wartosci przy nacisnieciu przycisku

        }
        

    }
}

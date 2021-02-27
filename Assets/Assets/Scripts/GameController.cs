using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    //odpowiedzialny na ładowanie poziomu
    //przesuwanie gracza i boxow

    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject targetPrefab;
    public GameObject boxPrefab;
    public GameObject playerPrefab;

    static List<Level> levels = new List<Level>(); //lista wczytanych poziomow
    public int currentLevel;

    private GameObject[,] boxes; //dwuwymiarowa tablica boxow
    private Vector2Int playerPos;
    private static Player playerObject;

    private bool[,] walls; //dwuwymiarowa tablica dla scian
    private bool[,] targets;

    private int countSteps; //zlicza kroki

    private int widthSize;
    private int heightSize;
    private int howManyTargets; //zmiena zlicza ile jest targetow na planszy

    public string fileName; //wczytanie pliku

    Level lev;

    // Start is called before the first frame update
    void Start()
    {
        //wczytac poziomy do listy levels i zaladowac pierwszy
        //levels = new List<Level>();

        //levels.Add(new Level());
        //LoadLevel(currentLevel);

        //sprawdzam, warunek na którym poziomie się znajduję
        //jeżeli jest to poziom <0 to wczytuję level z pliku tekstowego
        //ktory podaje jako fileName w inspektorze

        // (CANT - doesnt store arrays) Korzystam z PlayerPrefs - stores and accesses player preferences between game sessions
        //zapisanie naszej listy z lvl
        // PlayerPrefs.
        countSteps = 0;


        if(currentLevel == 0)
        {
            lev = new Level();
            LoadLevel(currentLevel);
           
        }
        else
        {
            lev = new Level(fileName);
            LoadLevel(currentLevel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //resetowanie poziomu do stanu pierwotnego
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }

    private void LoadLevel(int idx) {
        currentLevel = idx;
        //Level lev = levels[currentLevel];
        
        widthSize = lev.width;
        heightSize = lev.height;

        boxes = new GameObject[widthSize, heightSize];
        walls = new bool[widthSize, heightSize]; //tablica z wymiarami poziomu; zapisujemy update skrzynek i gracza, bo moga sie przemieszczac
        targets = new bool[widthSize, heightSize];
        howManyTargets = 0;



        //TODO na podstawie levels[currentLevel] robimy Instantiate odpowiednich prefabów w odp. miejscach
        //Debug.Log("wysokosc" + lev.height);
        //Debug.Log("szerokosco" + lev.width);

        for (int i=0; i < lev.height; i++) {
            for(int j=0; j < lev.width; j++) {
                //instantiate odpowiedni prefub
 
                if (lev.levelLayout[i][j].field == FieldType.Wall)
                {
                    Instantiate(wallPrefab, new Vector3(j, i, 0), Quaternion.identity); //Quaternion.identity - normal rotation its already have
                    walls[j, i] = true;
                    
                }
                if (lev.levelLayout[i][j].field == FieldType.Floor)
                {
                    Instantiate(floorPrefab, new Vector3(j, i, 0), Quaternion.identity);
                    
                }
                if (lev.levelLayout[i][j].field == FieldType.Target)
                {
                    Instantiate(targetPrefab, new Vector3(j, i, 0), Quaternion.identity);
                    targets[j, i] = true;
                    howManyTargets++;
                }

                //jesli na polu jest gracz to wynik Instantiate zapisujemy w playerGameObject i zapamietujemy
                //wspolrzedne gracza w playerPos
                if (lev.levelLayout[i][j].entity == EntityType.Player)
                {
                    playerObject = Instantiate(playerPrefab, new Vector3(j, i, -1), Quaternion.identity).gameObject.GetComponent<Player>(); //-1 bo musi byc z widoczne z przodu
                    playerPos = new Vector2Int(j, i);
                    

                }

                //jezeli na polu [x, y] znajduje się skrzynia to wynik Instantiate zapisujemy do boxes na wsp. [x, y]
                if (lev.levelLayout[i][j].entity == EntityType.Box)
                {
                    boxes[j, i] = Instantiate(boxPrefab, new Vector3(j, i, -1), Quaternion.identity) as GameObject;
                    //boxes[j, i] = boxPrefab;
                   
                }
            }
        }
        // playerObject = FindObjectOfType<Player>();
        playerObject = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerObject.gameController = this;


        //TODO Wycentrowac Camera.main na srodku poziomu i oddalic (ortographicSize) tak, zeby miescil sie cały na ekranie

        Camera.main.orthographicSize = lev.width * 0.5f; //skala

       
        Camera.main.transform.position = new Vector3(lev.width * 0.5f - 0.5f, lev.height* 0.5f - 0.5f, -10f); //camera znajduje się na środku poziomu !inty _nie_ float żeby "zaokrąglić dol"


    }

    public bool TryMovePlayer(Vector2Int direction) {
        //patrzymy na pole playerPos + direction:
        // jezeli jest tam ściana to nie robimy ruchu
        // jezeli jest tam skrzynia to próbujemy ją przesunąć i jeśli się udało to przesuwamy też gracza
        // aktualizując jego playerPos i pozycję obiektu na ekranie


        if (walls[direction.x + playerPos.x, direction.y + playerPos.y] == false) //spr czy nie wystepuje sciana
        {
            if (boxes[direction.x + playerPos.x, direction.y + playerPos.y] == null)
            {
                //spr czy w danym miejscu nie wystepuje pudeleko
                playerPos += direction; //nowa pozycja playera
                playerObject.transform.position = new Vector3(playerPos.x, playerPos.y, -1);
                countSteps++;
                Debug.Log("Ilosc krokow:" + countSteps);
                return true;
            }
            //sprawdzamy warunek jezeli znajduje sie pudelko
            else if (boxes[direction.x + playerPos.x, direction.y + playerPos.y] != null)
            {
                Vector2Int boxPosition = new Vector2Int(playerPos.x + direction.x, playerPos.y + direction.y); // pozycja box 
                                                                                                               //Debug.Log("Box posistion" + boxPosition);
                if (TryMoveBox(boxPosition, direction))
                {
                    //jeżeli uda sie przesunac box - nie stoi za nim ściana ani pudelko --->  true
                    //Debug.Log("Box posistion" + (boxPosition.x + direction.x) + " " + (direction.y + boxPosition.y));
                    //jezeli udalo sie przesunac box to przesuwamy rowniez gracza na miejsce box
                    playerPos = playerPos + direction;
                    playerObject.transform.position = new Vector3(playerPos.x, playerPos.y, -1);
                    countSteps++;
                    Debug.Log("Ilosc krokow:" + countSteps);
                    return true;
                }
            }


        }
        
        return false;
    }

    private bool TryMoveBox(Vector2Int pos, Vector2Int direction) {
        //jezeli na polu pos + direction nie ma sciany ani boxu przesuwamy pudełko aktualizując jego pozycję w tablicy i pozycję na ekranie; transform.position
        if (walls[direction.x + pos.x, direction.y + pos.y] == false && boxes[direction.x + pos.x, direction.y + pos.y] == null)
        { //spr czy w danym miejscu nie wystepuje ściana lub pudelko

            boxes[pos.x, pos.y].transform.position = new Vector3(pos.x + direction.x, pos.y + direction.y, -1);
            boxes[pos.x + direction.x, pos.y + direction.y] = boxes[pos.x, pos.y];
            boxes[pos.x, pos.y] = null;

            UpdateCorrectBoxes();

            return true;

        }
        
        //UpdateCorrectBoxes();
        // w przeciwnym przypadku jezeli na polu pos + direction jest ściana lub pudełko to się nie da przesunąć i zwracamy false;
        return false;
    }

    private void UpdateCorrectBoxes() {
        //TODO przeiterowac po wspolrzednych, na których są targety i sprawdzic na ilu z nich jest skrzynia
        //sprawdzam to dając wynik na konsolę
        int howManyDone = 0;
        for(int i=0; i < heightSize; i++)
        {
            for(int j=0; j < widthSize; j++)
            {
                if(targets[j,i] == true)
                {
                    //spr czy jest na nim box
                    if(boxes[j,i] != null)
                    {
                        //box znajduje sie na targecie
                        //zmienna zliczająca boxy w dobrych miejscach
                        howManyDone++;
                    }
                }
            }
        }
        Debug.Log("Boxy: " + howManyDone + " na " + howManyTargets + " sa w odpowiednim miejscu");
        if(howManyTargets == howManyDone) //jeżli wszystkie boxy są w odpowiednich miejsach
        {
            Debug.Log("Brawo! Wygrał_ś poziom!");
            if (currentLevel == 1) { Debug.Log("Koniec gry"); } //  <----- KONIEC GRY; można edytować 1 - wstępnie koniec gry jest na 1 lvl
            else
            {

                LoadNextLevel(currentLevel);
            }
        }
        
    }

    //załadowanie nastepnego poziomu
    private void LoadNextLevel(int currentLevel)
    {

        //nowa wersja - załadowanie drugiej sceny

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        ///this.currentLevel = currentLevel + 1;
        //restart wszystkich zmiennych
        //playerPos = null;
        // playerObject = null ;
        // //dwuwymiarowa tablica boxow
        // GameObject.FindWithTag("Player").GetComponent<Player>()

        //levels.Clear();
        //Debug.Log("pierwszy" + playerObject.gameController.GetType());
        //Destroy(playerObject.gameController);
        //Destroy(playerObject);
        //Debug.Log("drugi" + playerObject);
        //playerPos = playerPosOrginal;

        /*foreach (GameObject box in boxes)
        {
            Destroy(box);
        }*/

        /*var player = GameObject.FindGameObjectsWithTag("Player");
        foreach(var clone in player)
        {
            Destroy(clone);
        }*/

        //**Tu była próba wczytania nowego lvl na tej samej scenie... restetowanie wszystkich obiektów
        //problem z resetowaniem Vector2Int (nawet po przypisaniu drugiej początkowej wartosci)

        /*
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        
        var all = GameObject.FindGameObjectsWithTag("Other");
        foreach(var clone in all)
        {
            Destroy(clone);
        }
        var box = GameObject.FindGameObjectsWithTag("Box");
        foreach(var clone in box)
        {
            Destroy(clone);
        }

        


        //nowy lvl
        levels.Add(new Level(fileName));
        LoadLevel(currentLevel + 1);*/
    }
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontalInput = 0f;
    public float movementSpeed = 3f;
    //publiczne zmienne b�d� widoczne w inspektorze w edytorze Unity
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    private bool isGrounded = false;
    private bool isJumping = false;
    public float jumpForce = 5f;
    private Collider2D coll;
    private ContactFilter2D contactFilter;
    public LayerMask layerMask;
    private RaycastHit2D[] results = new RaycastHit2D[10];
    public Rigidbody2D rb;
    
    //Metoda wbudowana w silnik Unity. Wywo�ywana tylko RAZ na starcie dzia�ania gry.
    //S�u�y do inicjjalizowania zmiennych
    void Start()
    {
        //Metoda GetComponent s�u�y do pobierania innych komponent�w lub skrypt�w, kt�re s�
        //dodane do tego samego obiektu na scenie do kt�rych zosta� do��czony ten skrypt
        //W tych nawiasach "<>" podajemy typ obiektu kt�ry chcemy pobra�. W naszym wypadku Collider2D
        coll = GetComponent<Collider2D>();
        //zmienna contactFilter typu ContactFilter2D s�u�y do podania ustawie� dla naszego testu wykrywania
        //czy gracz stoi na ziemi.
        //Metoda SetLayerMask ustawia layery (warstwy), na kt�rych musz� znajdowa� si� obiekty aby nasz test wykrywa� je jako ziemia
        //W naszym wypadku zmienn� layerMask ustawili�my na Ground. i wszystkim obiekty (niebieska belka, kt�r� traktujemy jako ziemi�)
        //ustawiamy Layer na Ground.        
        contactFilter.SetLayerMask(layerMask);        
    }

    //Metoda wbudowana w silnik Unity. Jest ona wywo�ywana co klatk� dzia�ania programu.
    //Tutaj w okre�lonej kolejno�ci wywo�ujemy napisane przez nas metody utworzone poni�ej
    void Update()
    {
        CheckGrounded();
        ProcessInput();
        UpdateMovement();
        UpdateAnimator();
        UpdateFacing();
    }

    //Metoda odpowiada za sprawdzenie czy posta� gracza znajduje si� na ziemi
    private void CheckGrounded()
    {
        //Na pocz�tku ustawiamy warto�� na "False" - zak�adamy, �e gracz nie jest w powietrzu
        
        isGrounded = false;
        // Korzystaj�c z BoxCollider2D gracza (prostok�ta odpowiadaj�cego za kolizje)
        // Robimy test przesuwaj�c wirtualnie ten prostok�t w d� (Vector2.down) o warto�� 0.1f (ostatni argument)
        // Do testu u�ywamy ustawie� zapisanych w contactFilter
        // Wszystkie collidery innych obiekt�w na scenie kt�re przecinaj� collider naszego gracza
        // zostan� zapisane w zmiennej results.
        // Metoda zwraca liczb� (int) wszystkich collder�w zapisanych w zmiennej results
        // T� liczb� zapisujemy do zmiennej count.
        int count = coll.Cast(Vector2.down, contactFilter, results, 0.1f);
        //Poniewa� nie interesuj� nas konkretne obiekty, a sam fakt czy przecieli�my naszym testem kt�ry� z nich
        //Sprawdzamy czy count jest wi�ksze od 0 (count > 0)
        //Je�li tak to ustawiamy zmienn� isGrounded na "True"
        if(count > 0)
        {
            isGrounded = true;
        }
        //Metoda kt�ra umo�liwia nam wypisanie warto�ci w konsoli edytora Unity
        //W tym wypadku w konsoli mo�e zosta� wypisana informacja
        //"Is grounded: true" lub "Is grounded: false" w zale�no�ci od warto�ci zmiennej isGrounded
        Debug.Log("Is grounded: " + isGrounded);
    }

    //W tej metodzie sprawdzamy czy gracz wcisn�� przycisk w prawo lub w lewo.
    //Zale�nie od tego czy ustawiamy w komponencie Sprite Renderer warto�� FlipX na true lub false
    //FlipX oznacza czy sprite przedstawiaj�cy posta� powinien zosta� odbity lustrzanie 
    private void UpdateFacing()
    {
        if(horizontalInput > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        if(horizontalInput < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }

    //W tej metodzie aktualizujemy komponent AnimatorController
    //Ustalamy zmienn� "Speed" zgodnie z aktualn� pr�dko�ci� postaci
    private void UpdateAnimator()
    {
        //Poniewa� horizontal input mo�e przyj�� warto�ci z przedzia�u <-1,1>
        //nasz currentSpeed mo�e przyj�� warto�ci minusowe
        float currentSpeed = movementSpeed * horizontalInput;
        //Metoda Mathf.Abs zwraca warto�� bezwzgl�dn� z warto�ci
        //To oznacza, �e currentSpeed zawsze b�dzie warto�ci� dodatni�, niezale�nie od kierunku ruchu
        currentSpeed = Mathf.Abs(currentSpeed);
        animator.SetFloat("Speed", currentSpeed);
    }

    //W tej metodzie aktualizujemy pr�dko�� postaci korzystaj�� z parametru Velocity
    //w komponencie Rigidbody2D (odpowiadj�cego za symulacj� fizyki w grze)
    private void UpdateMovement()
    {
        Vector2 velocity = rb.velocity;
        //Ustalamy pr�dko�� horyzontaln� zgodnie z wci�ni�tym przez gracza klawiszem
        velocity.x = horizontalInput * movementSpeed;
        //Je�li wykryli�my, �e gracz nacisn�� klawisz skoku to nadajemy postaci pr�dko�� wertykaln�
        if(isJumping == true)
        {
            velocity.y = jumpForce;
        }
        //Tak obliczon� pr�dko�� przypisujemy do naszego komponentu Rigidbody2D
        rb.velocity = velocity;
    }

    //W tej metodzie sprawdzamy czy gracz wcisn�� przycisk odpowiadaj�cy za ruch
    //i zapisujemy te warto�ci w zmiennych do dalszego wykorzystania
    private void ProcessInput()
    {
        //GetAxis("Horizontal") odpowiada za pobranie warto�ci w zakresie <-1,1> 
        //w zale�no�ci od wci�nietych klawiszy strza�ek lub A i D
        //W lewo zwraca -1, w prawo zwraca 1, brak wci�ni�cia przycisku zwraca 0
        horizontalInput = Input.GetAxis("Horizontal");
        
        //Zmienna isJumping b�dzie ustawiony na "True" je�li wcisn�li�my przycisk spacji
        //I zmienna isGrounded jest ustawiona na "True"
        //Znak && odpowiada za logiczne s�owo AND (I)
        //Znak || odpowiada za logiczne s�owo OR (LUB)
        //Znak == odpowiada za sprawdzenie czy warto�ci s� r�wne
        //Znak != odpowiada za sprawdzenie czy warto�ci nie s� r�wne
        isJumping = Input.GetKeyDown(KeyCode.Space) && isGrounded;
    }



}

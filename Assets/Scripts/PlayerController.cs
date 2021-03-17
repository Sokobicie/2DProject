using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontalInput = 0f;
    public float movementSpeed = 3f;
    //publiczne zmienne bêd¹ widoczne w inspektorze w edytorze Unity
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
    
    //Metoda wbudowana w silnik Unity. Wywo³ywana tylko RAZ na starcie dzia³ania gry.
    //S³u¿y do inicjjalizowania zmiennych
    void Start()
    {
        //Metoda GetComponent s³u¿y do pobierania innych komponentów lub skryptów, które s¹
        //dodane do tego samego obiektu na scenie do których zosta³ do³¹czony ten skrypt
        //W tych nawiasach "<>" podajemy typ obiektu który chcemy pobraæ. W naszym wypadku Collider2D
        coll = GetComponent<Collider2D>();
        //zmienna contactFilter typu ContactFilter2D s³u¿y do podania ustawieñ dla naszego testu wykrywania
        //czy gracz stoi na ziemi.
        //Metoda SetLayerMask ustawia layery (warstwy), na których musz¹ znajdowaæ siê obiekty aby nasz test wykrywa³ je jako ziemia
        //W naszym wypadku zmienn¹ layerMask ustawiliœmy na Ground. i wszystkim obiekty (niebieska belka, któr¹ traktujemy jako ziemiê)
        //ustawiamy Layer na Ground.        
        contactFilter.SetLayerMask(layerMask);        
    }

    //Metoda wbudowana w silnik Unity. Jest ona wywo³ywana co klatkê dzia³ania programu.
    //Tutaj w okreœlonej kolejnoœci wywo³ujemy napisane przez nas metody utworzone poni¿ej
    void Update()
    {
        CheckGrounded();
        ProcessInput();
        UpdateMovement();
        UpdateAnimator();
        UpdateFacing();
    }

    //Metoda odpowiada za sprawdzenie czy postaæ gracza znajduje siê na ziemi
    private void CheckGrounded()
    {
        //Na pocz¹tku ustawiamy wartoœæ na "False" - zak³adamy, ¿e gracz nie jest w powietrzu
        
        isGrounded = false;
        // Korzystaj¹c z BoxCollider2D gracza (prostok¹ta odpowiadaj¹cego za kolizje)
        // Robimy test przesuwaj¹c wirtualnie ten prostok¹t w dó³ (Vector2.down) o wartoœæ 0.1f (ostatni argument)
        // Do testu u¿ywamy ustawieñ zapisanych w contactFilter
        // Wszystkie collidery innych obiektów na scenie które przecinaj¹ collider naszego gracza
        // zostan¹ zapisane w zmiennej results.
        // Metoda zwraca liczbê (int) wszystkich collderów zapisanych w zmiennej results
        // Tê liczbê zapisujemy do zmiennej count.
        int count = coll.Cast(Vector2.down, contactFilter, results, 0.1f);
        //Poniewa¿ nie interesuj¹ nas konkretne obiekty, a sam fakt czy przecieliœmy naszym testem któryœ z nich
        //Sprawdzamy czy count jest wiêksze od 0 (count > 0)
        //Jeœli tak to ustawiamy zmienn¹ isGrounded na "True"
        if(count > 0)
        {
            isGrounded = true;
        }
        //Metoda która umo¿liwia nam wypisanie wartoœci w konsoli edytora Unity
        //W tym wypadku w konsoli mo¿e zostaæ wypisana informacja
        //"Is grounded: true" lub "Is grounded: false" w zale¿noœci od wartoœci zmiennej isGrounded
        Debug.Log("Is grounded: " + isGrounded);
    }

    //W tej metodzie sprawdzamy czy gracz wcisn¹³ przycisk w prawo lub w lewo.
    //Zale¿nie od tego czy ustawiamy w komponencie Sprite Renderer wartoœæ FlipX na true lub false
    //FlipX oznacza czy sprite przedstawiaj¹cy postaæ powinien zostaæ odbity lustrzanie 
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
    //Ustalamy zmienn¹ "Speed" zgodnie z aktualn¹ prêdkoœci¹ postaci
    private void UpdateAnimator()
    {
        //Poniewa¿ horizontal input mo¿e przyj¹æ wartoœci z przedzia³u <-1,1>
        //nasz currentSpeed mo¿e przyj¹æ wartoœci minusowe
        float currentSpeed = movementSpeed * horizontalInput;
        //Metoda Mathf.Abs zwraca wartoœæ bezwzglêdn¹ z wartoœci
        //To oznacza, ¿e currentSpeed zawsze bêdzie wartoœci¹ dodatni¹, niezale¿nie od kierunku ruchu
        currentSpeed = Mathf.Abs(currentSpeed);
        animator.SetFloat("Speed", currentSpeed);
    }

    //W tej metodzie aktualizujemy prêdkoœæ postaci korzystaj¹æ z parametru Velocity
    //w komponencie Rigidbody2D (odpowiadj¹cego za symulacjê fizyki w grze)
    private void UpdateMovement()
    {
        Vector2 velocity = rb.velocity;
        //Ustalamy prêdkoœæ horyzontaln¹ zgodnie z wciœniêtym przez gracza klawiszem
        velocity.x = horizontalInput * movementSpeed;
        //Jeœli wykryliœmy, ¿e gracz nacisn¹³ klawisz skoku to nadajemy postaci prêdkoœæ wertykaln¹
        if(isJumping == true)
        {
            velocity.y = jumpForce;
        }
        //Tak obliczon¹ prêdkoœæ przypisujemy do naszego komponentu Rigidbody2D
        rb.velocity = velocity;
    }

    //W tej metodzie sprawdzamy czy gracz wcisn¹³ przycisk odpowiadaj¹cy za ruch
    //i zapisujemy te wartoœci w zmiennych do dalszego wykorzystania
    private void ProcessInput()
    {
        //GetAxis("Horizontal") odpowiada za pobranie wartoœci w zakresie <-1,1> 
        //w zale¿noœci od wciœnietych klawiszy strza³ek lub A i D
        //W lewo zwraca -1, w prawo zwraca 1, brak wciœniêcia przycisku zwraca 0
        horizontalInput = Input.GetAxis("Horizontal");
        
        //Zmienna isJumping bêdzie ustawiony na "True" jeœli wcisnêliœmy przycisk spacji
        //I zmienna isGrounded jest ustawiona na "True"
        //Znak && odpowiada za logiczne s³owo AND (I)
        //Znak || odpowiada za logiczne s³owo OR (LUB)
        //Znak == odpowiada za sprawdzenie czy wartoœci s¹ równe
        //Znak != odpowiada za sprawdzenie czy wartoœci nie s¹ równe
        isJumping = Input.GetKeyDown(KeyCode.Space) && isGrounded;
    }



}

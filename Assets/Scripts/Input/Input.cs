using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class Input : MonoBehaviour
{
    public bool isMoving { get; private set; }
    public Vector2 dir { get; private set; }

    private InputAction move;
    private InputAction jump;
    private Player player;
    private void Awake()
    {
        player = GetComponent<Player>(); //Obtem o gameobject do mesmo contexto que Input e que implemente o componente CubeEntity
        if (player.IsUnityNull()) throw new Exception("player is null");


    }

    private void Start()
    {
        move = InputSystem.actions.FindAction("Move");
        move.performed += MoveIn; //event in
        move.canceled += MoveOut; //event out

        jump = InputSystem.actions.FindAction("Jump");
        //move.started += JumpIn;
        jump.performed += JumpIn;
        
    }

    private void Update()
    {
        //Move logic case
        if (isMoving && !dir.IsUnityNull())
        {
            //Debug.Log(dir.ToString());
            player.transform.position += new Vector3(dir.x, dir.y, Vector3.zero.z) * Time.deltaTime;
        }

    }

    //MY METHODS
    //Business logic of movement, can be inserted in another class
    private void MoveIn(InputAction.CallbackContext context)
    {
        dir = context.ReadValue<Vector2>();
        isMoving = true;
    }
    private void MoveOut(InputAction.CallbackContext context)
    {
        isMoving = false;
    }


    private void JumpIn(InputAction.CallbackContext context)
    { 
        var y = player.transform.position.y + 7.0f; //Queremos player 13 frames para cima
        var dir = new Vector3(player.transform.position.x, y, player.transform.position.z);
        //Debug.Log($"Jump performed y: {y}");
        //player.transform.position += new Vector3(0, y, 0);
        StartCoroutine(MoveTo(dir, 1.0f));
    }

    private IEnumerator MoveTo(Vector3 destino, float duracao)
    {
        Vector3 ini = transform.position;
        float t = 0f;

        while (t < duracao)
        {
            t += Time.deltaTime;
            //Debug.Log(t.ToString());
            transform.position = Vector3.Lerp(ini, destino, t/duracao); //t/duracao pode ser modificado por uma função exposta ou AnimationCurve
            //yield entrega um valor e cede o controle para quem o chamou
            //yield com return faz com que a gente 'pause' a execução da função, ela não morre na memória, e no próximo loop de interação ela volta de onde parou
            //Nativamente, se eu fosse usar Yield, tenho que usar no IEnumerator, usando o método MoveNext() para permitir que a execução ocorra a partir do yield
            yield return null; //pausa a corrotina no próximo frame, sem isso o loop roda inteiro num frame
            //Falando em performance
            //O compilador reescreve esse método numa classe escondida, numa versão simplificada de máquina de eestados finita.
            //A classe é alocada em memória na heap, o garbabe collector, ao limpar isso, pode criar Slutter (travadinhas ao rodar o jogo) e isso pode ser chato ;c
            //Por isso, não iniciar muitas corrotinas por frame :)
        }
        //o trecho de código abaxo é o que dá a suavização para a duração.
        //float progresso = t / duracao;
        //float suave = Mathf.SmoothStep(0f, 1f, progresso); // começa e termina devagar
        //transform.position = Vector3.Lerp(inicio, destino, suave);
        transform.position = destino;

    }
    private void OnCollisionEnter(Collision collision) //colision é o gameobject qual o objeto colidiu
    {
        Debug.Log(collision.gameObject.name);
        //Destroy(collision.gameObject); //Para destruir o objeto ao qual estamos colidindo
    }


}

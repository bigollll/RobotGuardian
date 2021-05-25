using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player;        //pega o player.
    public Transform bulletSpawn;   //ponto de spawn da bala.
    public Slider healthBar;        //barra de vida.
    public GameObject bulletPrefab; //prefab da bala.

    NavMeshAgent agent;              //peganndo o navmesh do agente
    public Vector3 destination;      // The movement destination.
    public Vector3 target;           // The position to aim to.
    float health = 100.0f;           //define a vida inicial.
    float rotSpeed = 5.0f;           //velocidade de rotação.

    float visibleRange = 80.0f;      //define a range de visão.
    float shotRange = 40.0f;         //define a range da bala.

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();                                //associando o agent ao navmesh.
        agent.stoppingDistance = shotRange - 5;                                   //for a little buffer.
        InvokeRepeating("UpdateHealth",5,0.5f);                                   //??????????????.
    }

    void Update()
    {
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);    //a barra de vida segue a camera.
        healthBar.value = (int)health;                                                     //associando a barra de vida da UI com o a vida dada ao player.
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);                 //posição da barra de vida.
    }

    void UpdateHealth()
    {
       if(health < 100)              //se a vida n for igual a 100, recupera vida.
        health ++;                   //incrementa a vida.
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "bullet")              //se colidir com a bala, toma dano .
        {
            health -= 10;                               //tira 10 de vida por bala.
        }
    }

    [Task]                                    //faz parte de um comando a ser concluido.
    public void PickRandomDestination()                                                       //metodo que da um destino aleatorio.
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));      //dest é um ponto aleatorio dentro dos parametros.
        agent.SetDestination(dest);                                                           //o destino é o "dest".
        Task.current.Succeed();                                                               //task concluida.
    }

    [Task]                                                                            //faz parte de um comando a ser concluido.
    public void MoveToDestination()                                                   //Metodo que faz mover ao destino.
    {
        if(Task.isInspected)                                                          //se a task foi pega
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);          //mostra o tempo que estao sendo executado
        if(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)   //pega a distancia do andada pelo anget e se for menor que a distancia pra ele parar a task é bem sucedida
        {
            Task.current.Succeed();                                                   //task concluida.
        }

    }
    
    [Task]                                                                            //faz parte de um comando a ser concluido.
    public void PickDestination(int x, int z)                                         //metodo que da o destino que precisa de paremetros.
    {
        Vector3 dest = new Vector3(x, 0, z);                                          //novo destino é o paremetro colocado na BT.
        agent.SetDestination(dest);                                                   //falando que o dest é o destino pro agent.
        Task.current.Succeed();                                                       //task concluida.
    }
   
    [Task]                                                                            //faz parte de um comando a ser concluido.
    public void TargetPlayer()                                                        //player que vai ser targetado.
    {
        target = player.transform.position;                                           //o target é o player.
        Task.current.Succeed();                                                       //task concluida.
    } 
    
   

    [Task]
    bool Turn(float angle)                                                                                      //metodo turn com paremetro de algulo.
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;     //p é a posição para realizar o turn.
        target = p;                                                                                             //target é o p.
        return true;                                                                                            // retorna verdadeiro.
    }


    [Task]                                                                                                                                      //faz parte de um comando a ser concluido.
    public void LookAtTarget()                                                                                                                  //metodo para detectar o target.
    {
        Vector3 direction = target - this.transform.position;                                                                                   //a direção do vertor é o targt.
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);     //rotação do ai para virar para o player.
       
        if (Task.isInspected)                                                                                                                   //se a task foi pega.
            Task.current.debugInfo = string.Format("anfle={0}", Vector3.Angle(this.transform.forward, direction));                              //ativa a task no ai.
       
        if(Vector3.Angle(this.transform.forward, direction) < 5.0f)                                                                             //se o angulo for menor que 5.
        {
            Task.current.Succeed();                                                                                                             //task concluida.
        }
    }

    [Task]                                                                            //faz parte de um comando a ser concluido.
    public bool Fire()                                                                //metodo para atirar.
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);     //Instancia a bala.       
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);                                                   //aplica força na bala.

        return true;                                                                                                                  //se atirou retonar verdadeiro.
    }

    [Task]                                                                         //faz parte de um comando a ser concluido.
    bool SeePlayer()                                                               //detecta o Player.
    {
        Vector3 distance = player.transform.position - this.transform.position;    //distancia do player.
        RaycastHit hit;                                                            //cria um raycast.
        bool seeWall = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);               //desenha um raycast vermelho.
       
        if (Physics.Raycast(this.transform.position, distance, out hit))           //Pega a distacia do raycast.
        {
            if(hit.collider.gameObject.tag == "wall")                              //se o raycast colidir com a tag wall.
            {
                seeWall = true;                                                    //retorna verdadeiro no seeWall.
            }
        }
       
        if (Task.isInspected)                                                      //se a task foi pega.
            Task.current.debugInfo = string.Format("wall={0}", seeWall);           //ativa a task no ai.

        if (distance.magnitude < visibleRange && !seeWall)                         //se o see wall for false o estiver com visão.
            return true;                                                           //retonar verdadeiro.
        else                                                                       //se não.
            return false;                                                          //retorna falso.

    }

    [Task]                                                 //faz parte de um comando a ser concluido.
    public bool IsHealthLessThan(float health)             //parametro float passado para o "se a vida for menor que".
    {
        return this.health < health;                       //retorna que a vida é menor que a propria vida, desencadenado outra função posteriormente.
    }

    [Task]                                 //faz parte de um comando a ser concluido.
    public bool Explode()                  //metodo criado para Destruir o oponente.
    {
        Destroy(healthBar.gameObject);     //destroi a barra de vida.
        Destroy(this.gameObject);          //destroi o proprio player.
        return true;                       //retorna verdadeiro.
    }

}


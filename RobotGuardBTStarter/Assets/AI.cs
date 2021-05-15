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
            health -= 10;                               //tira 10 de vida por bla.
        }
    }

    [Task]
    public void PickRandomDestination()    
    {
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));      //dest é um ponto aleatorio dentro dos parametros.
        agent.SetDestination(dest);                                                           //o destino é o "dest".
        Task.current.Succeed();                                                               //sucesso na task ao termina-la.
    }

    [Task]
    public void MoveToDestination()
    {
        if(Task.isInspected)                                                          //se a task foi pega
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);          //mostra o tempo que estao sendo executado
        if(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)   //pega a distancia do andada pelo anget e se for menor que a distancia pra ele parar a task é bem sucedida
        {
            Task.current.Succeed();
        }

    }
}


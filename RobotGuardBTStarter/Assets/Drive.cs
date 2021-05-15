using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour {

	float speed = 20.0F;              //velocidade do player.
    float rotationSpeed = 120.0F;     //velocidade de rotação.
    public GameObject bulletPrefab;   //prefab da bala.
    public Transform bulletSpawn;     //ponto de spawn da bala.

    void Update() {
        float translation = Input.GetAxis("Vertical") * speed;             //movimentação frontal.
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;      //movimentação rotativa.
        translation *= Time.deltaTime;                                     //movimentao pelo delta time.
        rotation *= Time.deltaTime;                                        //rotação pelo delta time.
        transform.Translate(0, 0, translation);                            //vetor que modifica qnd há movimentação
        transform.Rotate(0, rotation, 0);                                  //vetor que modifica qnd há rotação

        if(Input.GetKeyDown("space"))          //se apertar Espaço
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);  //spawna o prefab da bala no ponto de bulletSpawn
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);                                                  //pegando o rigid body do prefab da bala e adcionando força para frente nela
        }
    }
}

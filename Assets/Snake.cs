using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // libreria importada para el reinicio del juego
using UnityEngine.UI;// libreria para el texto de puntos en interfaz


public class Snake : MonoBehaviour
{
    // Start is called before the first frame update
    public int xSize,ySize;
    public GameObject block;
    GameObject cabeza;
    public Material cabezaMaterial, colaMaterial, paredesMaterial, comidaMaterial;
    List<GameObject> cola;
    Vector2 direccionSnake;
    bool estaVivo;
    public GameObject GameOverGrande;

    public Text puntos;

    


    void Start()
    {


        tiempoEntreMovimiento = 0.10f;
        direccionSnake = Vector2.right;
        crearMundo();// creacion de la cuadricula
        crearJugador();
        crearComida();
        block.SetActive(false);
        estaVivo = true;
        
        
    }

    private Vector2 posicionAleatoria(){

        int posicionX = -xSize/2+1;
        int posicionX2 = xSize/2;
        int posicionY = -ySize/2+1;
        int posicionY2 =  ySize/2;


        return new Vector2(Random.Range(posicionX,posicionX2), Random.Range(posicionY,posicionY2));
    }

    GameObject comida;

    private bool posiciconesSnake(Vector2 crearComida){

        bool comidaCabeza = crearComida.x == cabeza.transform.position.x && crearComida.x == cabeza.transform.position.y;
        bool comidaCola = false;

        foreach (var item in cola)
        {
            if (item.transform.position.x == crearComida.x && item.transform.position.y == crearComida.y){

                comidaCola = true;


            }
        }

        return comidaCabeza || comidaCola;
    }

    private void crearComida(){

        Vector2 crearComida = posicionAleatoria();

        while(posiciconesSnake(crearComida)){
            
            crearComida = posicionAleatoria();
        }

        comida = Instantiate(block);
        comida.transform.position = new Vector3(crearComida.x, crearComida.y,0);
        comida.SetActive(true);
        comida.GetComponent<MeshRenderer>().material = comidaMaterial;


    }


    private void crearJugador(){
        cabeza = Instantiate(block) as GameObject;
        cabeza.GetComponent<MeshRenderer>().material = cabezaMaterial;
        cola = new List<GameObject>();
    }

    private void crearMundo(){

        for(int x = 0; x <= xSize; x++){

            GameObject borderBottom = Instantiate(block) as GameObject;
            borderBottom.GetComponent<Transform>().position = new Vector3(x-xSize/2, -ySize/2,0);// pared de abajo ubicado en 0.0
            borderBottom.GetComponent<MeshRenderer>().material = paredesMaterial;

             GameObject borderTop = Instantiate(block) as GameObject;
             borderTop.GetComponent<MeshRenderer>().material = paredesMaterial;
             borderTop.GetComponent<Transform>().position = new Vector3(x-xSize/2, ySize - ySize/2,0); // pared del lado de arriba

              for(int y = 0; y <= ySize; y++){
            GameObject borderLeft = Instantiate(block) as GameObject;
            borderLeft.GetComponent<MeshRenderer>().material = paredesMaterial;
            borderLeft.GetComponent<Transform>().position = new Vector3(-xSize/2, y-(ySize/2), 0); // lateral izquierdo

            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponent<MeshRenderer>().material = paredesMaterial;
            borderRight.GetComponent<Transform>().position = new Vector3(xSize-(xSize/2), y-(ySize/2), 0); //lateral derecho
        }
            
            

        }
    }

    private void gameOver(){

        estaVivo = false;
        GameOverGrande.SetActive(true);
    }

    public void restart(){

        SceneManager.LoadScene(0);
    }


    float tiempoPasado, tiempoEntreMovimiento;

    // Update is called once per frame
    void Update()
    {


        //secuencia de niveles

        if(cola.Count >= 2){

            tiempoEntreMovimiento = 0.12F;
        }
        else if(cola.Count >= 4){

            tiempoEntreMovimiento = 0.15F;

        }

        else if(cola.Count >= 6){

            tiempoEntreMovimiento = 0.20F;

        }
        //MOVIMIENTO DE LA CULEBRA

        if(Input.GetKey(KeyCode.DownArrow)){
            if(direccionSnake == Vector2.up ){ // pregunto si la direccion a la que se dirige es la contraria de la tecla

                direccionSnake = Vector2.up;// entonces asigno que si la direccion es asi, direccionSnake vuelve al vector en el que estaba
            }
            else{

                direccionSnake = Vector2.down;
            }

            
        }else if(Input.GetKey(KeyCode.UpArrow)){

            if(direccionSnake == Vector2.down){

                direccionSnake = Vector2.down;
            }
            else{

                direccionSnake = Vector2.up;
            }

            
            
        }

        else if(Input.GetKey(KeyCode.LeftArrow))
        {
            if(direccionSnake == Vector2.right){

                direccionSnake = Vector2.right;
            }

            else{

                direccionSnake = Vector2.left;
            }
            
        }

        else if(Input.GetKey(KeyCode.RightArrow))
        {

            if(direccionSnake == Vector2.left){

                direccionSnake = Vector2.left;
            }
            else{

                direccionSnake = Vector2.right;
            }

            
            
        }

        

       
        tiempoPasado = tiempoPasado + Time.deltaTime;

        if(tiempoEntreMovimiento <  tiempoPasado && estaVivo){

            tiempoPasado = 0;

            //movimiento

            Vector3 nuevaPosicion = cabeza.GetComponent<Transform>().position + new Vector3(direccionSnake.x,direccionSnake.y,0);


            //chequeo de colision con bordes

            if(nuevaPosicion.x >= xSize/2 || nuevaPosicion.x <= -xSize/2 || nuevaPosicion.y >= ySize/2 || nuevaPosicion.y <= -ySize/2){

                gameOver();
                
                

            }

            //verificar si colisiona con alguna parte de la cola!

            foreach(var item in cola )
            {

                if(item.transform.position == nuevaPosicion)
                {

                    gameOver();
                    
                }

            }
            if(nuevaPosicion.x == comida.transform.position.x && nuevaPosicion.y == comida.transform.position.y){// verifico si la nueva poscion coincide con la de la comida

                GameObject nuevaCola = Instantiate(block);
                nuevaCola.SetActive(true);
                nuevaCola.transform.position = comida.transform.position;
                DestroyImmediate(comida);
                cabeza.GetComponent<MeshRenderer>().material = colaMaterial;
                cola.Add(cabeza);
                cabeza = nuevaCola;
                cabeza.GetComponent<MeshRenderer>().material = cabezaMaterial;
                crearComida();
                puntos.text = "puntos: "+ cola.Count;
                


            }else
            
            {


                if(cola.Count == 0)
                {

                cabeza.transform.position = nuevaPosicion;
            }
            else 
            {
                    cabeza.GetComponent<MeshRenderer>().material = colaMaterial;
                    cola.Add(cabeza); 
                    cabeza = cola[0];
                    cabeza.GetComponent<MeshRenderer>().material = cabezaMaterial;
                    cola.RemoveAt(0);
                    cabeza.transform.position = nuevaPosicion;
            }

            }

            

            
    }
}}

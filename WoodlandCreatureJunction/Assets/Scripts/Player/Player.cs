using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Transform t;
    private GameObject ptext;
    private float speed = 4f;
    private PlayerState state;
    private GameObject Villager;
    private Task<string> genTask;

    private CameraController cameraController;

    public enum PlayerState
    {
        IDLE,
        MOVING,
        THINKING,
        TALKING
    }


    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        t = GetComponent<Transform>();
        ptext = GameObject.Find("PlayerText");
        ptext.SetActive(false);
        state = PlayerState.IDLE;
    }

    // Update is called once per frame
    async void Update()
    {
        if (state == PlayerState.TALKING)
        {
            //check status of async task
            if(genTask.IsCompleted)
            {
                string output = await genTask;
                state = PlayerState.IDLE;
                Villager.GetComponent<Villager>().EndConversation();
                cameraController.EndConversation();
            }
            else
            {
                Debug.Log("WAIT");
            }
        }
        else if (state == PlayerState.THINKING)
        {

            if(Input.GetKeyDown(KeyCode.Return))
            {
                string inp = GameObject.Find("PlayerTextBox").GetComponent<Text>().text;

                
                ptext.SetActive(false);
                string name = Villager.name;
                genTask = Task.Run( () => GenText.GenerateTextAsync(inp, name));
                //string output = await GenText.GenerateTextAsync(inp, Villager.name);
                //Debug.Log(output);
                state = PlayerState.TALKING;
            }
          
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                state = PlayerState.THINKING;
                InitTalk();
            }

            Movement();
         
        }
        
        
      
    }

  /*  async void async_gentext()
    {

        string inp = GameObject.Find("PlayerTextBox").GetComponent<Text>().text;
        ptext.SetActive(false);
        string response = GenText.GenerateText(inp, Villager.name);
    } */


    void Movement()
    {

        float time = Time.deltaTime;
        if (Input.GetKey(KeyCode.W))
        {
            t.position += new Vector3(0f, 0f, time*speed);
            state = PlayerState.MOVING;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            t.position += new Vector3(time * -speed, 0f, 0f);
            state = PlayerState.MOVING;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            t.position += new Vector3(0f, 0f, time * -speed);
            state = PlayerState.MOVING;

        }
        else if (Input.GetKey(KeyCode.D))
        {
            t.position += new Vector3(time * speed, 0f, 0f);
            state = PlayerState.MOVING;

        }
    }

    void InitTalk()
    {
        GameObject[] VillagerList = GameObject.FindGameObjectsWithTag("Villager");

        int closest_index = -1;
        float minDist = 10f; //Implicit 10f distance cutoff
        for(int i = 0; i <VillagerList.Length; i++)
        {
            float distance = Vector3.Distance(VillagerList[i].GetComponent<Transform>().position, t.position);
            if(distance < minDist)
            {
                minDist = distance;
                closest_index = i;
            }
        }

        if (closest_index == -1)
        {
            state = PlayerState.IDLE;
            return;
        }
            

        Villager = VillagerList[closest_index];
        Villager.GetComponent<Villager>().StartConversation(transform);
        cameraController.StartConversation(Villager.transform);

        ptext.SetActive(true);
        

    }
}

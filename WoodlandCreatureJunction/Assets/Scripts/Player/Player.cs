using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

    [SerializeField] GameObject PlayerText;

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
        ptext = PlayerText;
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
                /* Get the text */
                string output = await genTask;
                output = ProcessOutput(output);
                Villager.GetComponentInChildren<BillboardCanvas>().DisplayMessage(output);

                /* End the conversation */
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

    string ProcessOutput(string input)
    {
        /* just get the first sentence in the AI response */
        try
        {
            input = input.Substring(input.IndexOf("Villager says "));                   //remove the player prompt
            input = input.Substring(0, input.IndexOfAny(new char[] { '.', '!', '?' }) + 1); //stop at 1 sentence
            input = input.Remove(input.IndexOf('\u001B'), 1);   //remove random escape
            input = input.Remove(input.IndexOf("[0m"), 3);      //remove weird chars
            input = input.Substring("Villager says ".Length);   //remove prompt
        }
        catch (Exception)
        {
            return input;
        }
        

        Debug.Log("Villager output: " + input);

        return input;
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
        Vector3 Direction = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            Direction += transform.forward;
            state = PlayerState.MOVING;
        }

        if (Input.GetKey(KeyCode.A))
        {
            Direction -= transform.right;
            state = PlayerState.MOVING;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            Direction -= transform.forward;
            state = PlayerState.MOVING;

        }
        
        if (Input.GetKey(KeyCode.D))
        {
            Direction += transform.right;
            state = PlayerState.MOVING;

        }

        transform.position += Direction.normalized * time * speed;
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

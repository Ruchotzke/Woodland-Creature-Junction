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
    private GameObject Shovel;
    private GameObject Rock;
    private Vector3 orig_pos_shovel;
    private Vector3 rock_pos_shovel;
    private float shov_time;
    private Task<string> genTask;
    private ActionState actionState;
    [SerializeField] GameObject PlayerText;
    [SerializeField] Transform PlayerGraphic;
    [SerializeField] GameObject LanternObject;

    [Header("Prefabs")]
    public GameObject pf_Shovel;

    [Header("Sprites")]
    public Sprite Talk;
    public Sprite RockBreak;
    public Sprite Lantern;

    private CameraController cameraController;
    private float conversation_halt = 3f;
    private float conv_counter = 0f;
    public enum PlayerState
    {
        IDLE,
        MOVING,
        THINKING,
        TALKING,
        READING,
        ROCK_BREAK,
    }

    public enum ActionState
    {
        TALK,
        ROCKBREAK,
        LAMP
    }


    // Start is called before the first frame update
    void Start()
    {
        cameraController = FindObjectOfType<CameraController>();
        t = GetComponent<Transform>();
        ptext = PlayerText;
        ptext.SetActive(false);
        state = PlayerState.IDLE;
        actionState = ActionState.TALK;
    }

    // Update is called once per frame
    async void Update()
    {
        if(state == PlayerState.READING)
        {
            conv_counter += Time.deltaTime;
            if(conv_counter > conversation_halt)
            {
                /* End the conversation */
                state = PlayerState.IDLE;
                Villager.GetComponent<Villager>().EndConversation();
                cameraController.EndConversation();
            }

        }
        else if (state == PlayerState.TALKING)
        {
            //check status of async task
            if(genTask.IsCompleted)
            {
                /* Get the text */
                string output = await genTask;
                output = ProcessOutput(output);
                Villager.GetComponentInChildren<BillboardCanvas>().DisplayMessage(output);
                conv_counter = 0f;
                state = PlayerState.READING;
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
        else if (state == PlayerState.ROCK_BREAK)
        {
            shov_time += Time.deltaTime;
            Shovel.transform.position = Vector3.Lerp(orig_pos_shovel, rock_pos_shovel, shov_time);
            if (shov_time > 1f)
            {
                state = PlayerState.IDLE;
                Shovel.SetActive(false);
                Rock.SetActive(false);
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                GameObject tool = GameObject.Find("Tool");
                if (actionState == ActionState.TALK)
                {
                    tool.GetComponent<Image>().sprite = RockBreak;
                    actionState = ActionState.ROCKBREAK;
                }
                else if (actionState == ActionState.ROCKBREAK)
                {
                    tool.GetComponent<Image>().sprite = Lantern;
                    LanternObject.SetActive(true);
                    actionState = ActionState.LAMP;
                }
                else
                {
                    tool.GetComponent<Image>().sprite = Talk;
                    LanternObject.SetActive(false);
                    actionState = ActionState.TALK;
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(actionState == ActionState.TALK)
                {
                    state = PlayerState.THINKING;
                    InitTalk();
                }
                else
                {
                    InitRock();
                }
            }

            Movement();
         
        }
        
        
      
    }

    string ProcessOutput(string input)
    {
        /* just get the first sentence in the AI response */
        Debug.Log(input);
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
        if (CameraController.transitioning) return; //can't move during a camera transition!

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
        if(Direction.sqrMagnitude != 0) PlayerGraphic.rotation = Quaternion.LookRotation(Direction);
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

    void InitRock()
    {
        GameObject[] RockList = GameObject.FindGameObjectsWithTag("Rock");

        int closest_index = -1;
        float minDist = 5f; //Implicit 5f distance cutoff
        for (int i = 0; i < RockList.Length; i++)
        {
            float distance = Vector3.Distance(RockList[i].GetComponent<Transform>().position, t.position);
            if (distance < minDist)
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


        Rock = RockList[closest_index];
        Shovel = Instantiate(pf_Shovel);
        Shovel.transform.position = Rock.transform.position + new Vector3(0f,3f,0f);
        orig_pos_shovel = Shovel.transform.position;
        rock_pos_shovel = Rock.transform.position + new Vector3(0f,1f,0f);
        shov_time = 0f;
        state = PlayerState.ROCK_BREAK;


    }
}

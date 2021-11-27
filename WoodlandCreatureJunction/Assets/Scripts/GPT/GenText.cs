using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;

public class GenText : MonoBehaviour
{

    public static string GenerateText(string PlayerInput, string CharacterType)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            WorkingDirectory = @"..\GPT2\",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        Process myProcess = new Process
        {
            StartInfo = startInfo
        };
        // myProcess.StartInfo.Arguments = "this is the prompt";
        startInfo.FileName = @"C:\Program Files\Python39\python.exe"; //Not sure how else to do this, could maybe make the game ask for this in gui
        startInfo.Arguments = @"..\GPT2\RunAiTextGen.py " + "\" " + PlayerInput +"\" "+ "\"" + CharacterType + ": \"";
        myProcess.StartInfo.RedirectStandardInput = true;
        myProcess.Start();
        //UnityEngine.Debug.Log("START\n");
        /*myProcess.StandardInput.WriteLine(" \"Howdy partner\"");
        myProcess.StandardInput.Close();*/
        // UnityEngine.Debug.Log("CLOSED\n");
        string output = myProcess.StandardOutput.ReadToEnd();
        UnityEngine.Debug.Log(output);

        myProcess.WaitForExit();
        return output;
    }


    // Start is called before the first frame update
    void Start()
    {
        GenerateText("Krusty Krab Pizza is the pizza for you and me.", "Squidward");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

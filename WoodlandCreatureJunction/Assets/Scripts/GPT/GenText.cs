using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;
using System.Threading.Tasks;

public class GenText : MonoBehaviour
{
    public static string PYTHON_PATH = @"C:\Users\Rucho\AppData\Local\Programs\Python\Python39\python.exe";

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
        startInfo.FileName = PYTHON_PATH; //Not sure how else to do this, could maybe make the game ask for this in gui
        //startInfo.Arguments = @"..\GPT2\RunAiTextGen.py " + "\" " + PlayerInput +"\" "+ "\"" + CharacterType + ": \"";
        startInfo.Arguments = @"..\GPT2\RunAiTextGen.py " + "\" " + PlayerInput + "\"";
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

    public static async Task<string> GenerateTextAsync(string PlayerInput, string CharacterType)
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
        startInfo.FileName = PYTHON_PATH; //Not sure how else to do this, could maybe make the game ask for this in gui
        startInfo.Arguments = @"..\GPT2\RunAiTextGen.py " + "\" " + PlayerInput + "\" " + "\"" + CharacterType + ": \"";
        myProcess.StartInfo.RedirectStandardInput = true;

        try
        {
            myProcess.Start();
        }
        catch (Exception)
        {
            UnityEngine.Debug.LogError("Unable to open python. Erroring out.");
            return "ERROR";
        }
        
        //UnityEngine.Debug.Log("START\n");
        /*myProcess.StandardInput.WriteLine(" \"Howdy partner\"");
        myProcess.StandardInput.Close();*/
        // UnityEngine.Debug.Log("CLOSED\n");
        string output = myProcess.StandardOutput.ReadToEnd();
        UnityEngine.Debug.Log(output);

        myProcess.WaitForExit();
        return output;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.IO;

public class Controller : MonoBehaviour
{
    private InputField input;

    private Text input_message;
    private RawImage image;
    private string MessagesFile = "data.json";

    void Awake()
    {
        input = GameObject.Find("InputField").GetComponent<InputField>();
        input_message = GameObject.Find("Text").GetComponent<Text>();
        image = GameObject.Find("RawImage").GetComponent<RawImage>();
    }
    public void GetInput(string message)
    {
        Debug.Log("You entered " + message);
        bool contains = message.IndexOf("fuck", StringComparison.OrdinalIgnoreCase) >= 0;
        if (contains)
        {
            input_message.text = "Message is rejected!";
            Debug.Log("No way");
            input.text = "";
            StartCoroutine(getImage());
            Message mymessage = new Message();
            mymessage.message = "Last message was rejected!!";
            mymessage.CurrentTime = DateTime.Now.ToString("h:mm:ss tt");

            //serialize it to Json format and store it to file
            string filepath = Path.Combine(Application.persistentDataPath, MessagesFile);
            Debug.Log(Application.persistentDataPath);
            string json = JsonUtility.ToJson(mymessage);
            Debug.Log(json);
            System.IO.File.WriteAllText(filepath, json);
        }
        else
        {
            input.text = "";

            //create a message class
            Message mymessage = new Message();
            mymessage.message = message;
            mymessage.CurrentTime = DateTime.Now.ToString("h:mm:ss tt");

            //serialize it to Json format and store it to file
            string filepath = Path.Combine(Application.persistentDataPath, MessagesFile);
            Debug.Log(Application.persistentDataPath);
            string json = JsonUtility.ToJson(mymessage);
            Debug.Log(json);
            System.IO.File.WriteAllText(filepath, json);

            //print out the message on the console
            string full_message = mymessage.message + "\n" + "Entered at " + mymessage.CurrentTime;
            input_message.text = full_message;
        }
    }

    IEnumerator getImage()
    {
        WWW wwwLoader = new WWW("https://picsum.photos/200/300/?random");
        while (!wwwLoader.isDone)
        {
            Debug.Log("Download image on progress" + wwwLoader.progress);
            yield return null;
        }
        if (!string.IsNullOrEmpty(wwwLoader.error))
        {
            Debug.Log("Download failed");
        }
        else
        {
            Debug.Log("Download success");
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(wwwLoader.bytes);
            texture.Apply();
            image.color = Color.white;
            image.texture = texture;
        }  
    }
    void Start()
    {
        Message pre_message = LoadMessage();
        Debug.Log(pre_message);
        if(pre_message != null)
        {
            string pre_m = pre_message.message + "\n" + "Entered at " + pre_message.CurrentTime;
            input_message.text = pre_m;
        }
    }

    [Serializable]
    public class Message
    {
        public string message;
        public string CurrentTime;
    }

    private Message LoadMessage()
    {
        //get previous message store in json
        Message pre_message = new Message();
        string filepath = Path.Combine(Application.persistentDataPath, MessagesFile);
        if(File.Exists(filepath))
        {
            string json = File.ReadAllText(filepath);
            pre_message = JsonUtility.FromJson<Message>(json);
            return pre_message;
        }
        return null;
    }
}

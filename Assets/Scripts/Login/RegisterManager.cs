using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

using UnityEngine;
using UnityEngine.UI;

public class RegisterManager : MonoBehaviour
{
    public GameObject Title;
    public GameObject PasswordIF;
    public GameObject CPasswordIF;
    public InputField UsernameIF;
    public LoginNetController netcontrol;
    private string password;
    private string cpassword;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnRegister_Click()
    {
        Text mtext = Title.GetComponent<Text>();
        if (string.Equals(password, cpassword))
        {
            netcontrol.CreateUser(UsernameIF.text, password);
            //mtext.text = "You have succeed in register.";
        }
        else
        {
            mtext.text = "The passwords are not equal.";
        }
    }

    public void GetCreateResult(int id)
    {
        Text mtext = Title.GetComponent<Text>();
        if (id < 0)
        {
            mtext.text = "The user name has been used;";
        }
        else
        {
            mtext.text = "Success! Your id is:" + id;
        }
    }

    public void OnIP_End(string str)
    {
        password = PasswordIF.GetComponent<InputField>().text;
        Debug.Log("Password:" + password);
    }

    public void OnCP_End(string str)
    {
        cpassword = CPasswordIF.GetComponent<InputField>().text;
        Debug.Log("Confirm Password:" + cpassword);
    }
}

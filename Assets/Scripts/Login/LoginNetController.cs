﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;

public class LoginNetController : MonoBehaviour
{
    SocketIOComponent m_socket;
    public RegisterManager registerscript;
    public LoginUIManager cameracontrolscript;
    // Start is called before the first frame update
    void Start()
    {
        m_socket = GetComponent<SocketIOComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    # region 创建用户接口
    public void CreateUser(string username, string password)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["PasswordHash"] = GetMd5Hash(password);
        data["Username"] = username;
        m_socket.Emit("CreateUser", new JSONObject(data), OnCreateResBack);
    }

    public void OnCreateResBack(JSONObject json)
    {
        Dictionary<string, string> data = json[0].ToDictionary();
        if (string.Equals(data["res"], "true"))
        {
            registerscript.GetCreateResult(Int32.Parse(data["id"]));
        }
        else
        {
            registerscript.GetCreateResult(-1);
        }
    }
    #endregion
    #region 登陆接口
    public void PasswordAuth(string username, string password)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["PasswordHash"] = GetMd5Hash(password);
        data["Username"] = username;
        m_socket.Emit("PassAuth", new JSONObject(data), OnAuthResBack);
    }


    public void OnAuthResBack(JSONObject json)
    {
        bool temp;

        Dictionary<string, string> data = json[0].ToDictionary();
        Debug.Log("json:" + data["res"]);

        if (string.Equals(data["res"], "true"))
            temp = true;
        else
            temp = false;
        cameracontrolscript.GetAuthResult(temp);
    }

    public class AuthJSON
    {
        public bool res;
    }
    #endregion
    #region 加密函数
    private string GetMd5Hash(string input)
    {
        MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

        byte[] inputBytes = Encoding.Default.GetBytes(input);

        byte[] data = md5Hasher.ComputeHash(inputBytes);

        StringBuilder sBuilder = new StringBuilder();

        //将data中的每个字符都转换为16进制的
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

    private bool VerifyMd5Hash(string input, string hash)
    {
        string hashOfInput = GetMd5Hash(input);

        if (string.Equals(hashOfInput, hash))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
}
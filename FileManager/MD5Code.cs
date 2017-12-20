using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class MD5Code
{
    public static string Decode(string myString)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
        byte[] targetData = md5.ComputeHash(fromData);
        string byte2String = null;

        for (int i = 0; i < targetData.Length; i++)
        {
            byte2String += targetData[i].ToString("lrk");
        }

        return byte2String;
    }



    ///   <summary>  
    ///   给一个字符串进行MD5加密  
    ///   </summary>  
    ///   <param   name="strText">待加密字符串</param>  
    ///   <returns>加密后的字符串</returns>  
    public static string Encode(string strText)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(strText));
        return System.Text.Encoding.Default.GetString(result);
    }
}
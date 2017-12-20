using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class FileTool
{
    private static int BinaryInsertNum = 5;
    public static string DecryptFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            FileStream input = null;
            StreamReader reader = null;
            BinaryReader reader2 = null;
            try
            {
                FileInfo info = new FileInfo(filePath);
                input = new FileStream(filePath, FileMode.Open);
                reader2 = new BinaryReader(input, Encoding.UTF8);
                bool flag = false;
                string source = "";
                try
                {
                    if (reader2.ReadInt32() == BinaryInsertNum)
                    {
                        flag = true;
                        reader2.ReadInt32();
                        reader2.ReadInt32();
                        source = reader2.ReadString();
                        source = MD5Code.Decode(source);
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError(filePath + "_____DecryptFile Error : " + exception.Message);
                }
                finally
                {
                    if (reader2 != null)
                    {
                        reader2.Close();
                    }
                    if (input != null)
                    {
                        input.Close();
                    }
                }
                if (!flag)
                {
                    input = new FileStream(filePath, FileMode.Open);
                    reader = new StreamReader(input);
                    source = reader.ReadToEnd();
                }
                return source;
            }
            catch
            {
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (reader2 != null)
                {
                    reader2.Close();
                }
                if (input != null)
                {
                    input.Close();
                }
            }
        }
        return "";
    }

    public static void EncryptFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            FileStream stream = null;
            StreamReader reader = null;
            BinaryWriter writer = null;
            try
            {
                FileInfo info = new FileInfo(filePath);
                stream = new FileStream(filePath, FileMode.Open);
                string source = "";
                reader = new StreamReader(stream);
                source = reader.ReadToEnd();
                reader.Close();
                stream.Close();
                File.Delete(filePath);
                string str2 = "";
                str2 = source;//new
                str2 = MD5Code.Encode(source);
                stream = File.Open(filePath, FileMode.CreateNew);
                stream.Seek(0L, SeekOrigin.Begin);
                writer = new BinaryWriter(stream);
                writer.Write(BinaryInsertNum);
                writer.Write(UnityEngine.Random.Range(0, 100));
                writer.Write(UnityEngine.Random.Range(0x10, 0x4e));
                writer.Write(str2);
            }
            catch (Exception exception)
            {
                Debug.Log("EncryptFile --- " + exception.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
    }

    public static bool OpenSerializedInfo<T>(string path, ref T result) where T : new()
    {
        FileStream stream = null;
        StreamReader reader = null;
        bool flag;
        try
        {
            string str = path;
            flag = OpenSerializedInfoByContent<T>(DecryptFile(path), ref result);
        }
        catch
        {
            flag = false;
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
            if (stream != null)
            {
                stream.Close();
            }
        }
        return flag;
    }

    public static bool OpenSerializedInfoByContent<T>(string content, ref T result)
    {
        try
        {
            try
            {
                if (content.Contains("!DOCTYPE"))
                {
                    return false;
                }
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                StringReader input = new StringReader(content);
                XmlReader xmlReader = new XmlTextReader(input);
                result = (T)serializer.Deserialize(xmlReader);
            }
            catch (Exception exception)
            {
                Debug.LogError("OpenSerializedInfoByContent error : " + exception.Message);
                return false;
            }
        }
        finally
        {
        }
        return true;
    }

    public static bool SerializeInfo<T>(T info, string path, bool encrypt = true)
    {
        TextWriter textWriter = null;
        bool flag;
        try
        {
            FileUtil.makesureFileExist(path, false);//看路径是否存在
            string str = path;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            textWriter = new StreamWriter(str);
            serializer.Serialize(textWriter, info);
            FileInfo info2 = new FileInfo(str);
            textWriter.Close();
            if (encrypt)
            {
                EncryptFile(str);
            }
            flag = true;
        }
        catch
        {
            flag = false;
        }
        finally
        {
            if (textWriter != null)
            {
                textWriter.Close();
            }
        }
        return flag;
    }
}

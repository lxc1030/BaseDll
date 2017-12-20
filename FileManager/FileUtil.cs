using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class FileUtil : MonoBehaviour
{
    public static bool encrypt = false;
    public bool Encrypt = false;
    private static FileUtil instance;
    private Dictionary<string, ReplaceCfgItem> replaceCfgMap = new Dictionary<string, ReplaceCfgItem>();
    public List<ReplaceCfgItem> replaceCfgNames = new List<ReplaceCfgItem>();
    private static Dictionary<string, CallBack_Void_S> updateCallbacks = new Dictionary<string, CallBack_Void_S>();

    private void Awake()
    {
        UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
        encrypt = this.Encrypt;
        instance = this;
        if (this.replaceCfgNames != null)
        {
            int count = this.replaceCfgNames.Count;
            for (int i = 0; i < count; i++)
            {
                ReplaceCfgItem item = this.replaceCfgNames[i];
                if ((!string.IsNullOrEmpty(item.oriFileName) && !string.IsNullOrEmpty(item.replacedFileName)) && item.enable)
                {
                    this.replaceCfgMap.Add(item.oriFileName, item);
                }
            }
        }
    }

    public static string CheckReplaceFile(string oriFileName)
    {
        if ((instance != null) && ((instance.replaceCfgNames != null) && instance.replaceCfgMap.ContainsKey(oriFileName)))
        {
            ReplaceCfgItem item = instance.replaceCfgMap[oriFileName];
            Debug.Log("replaceFile : " + item.oriFileName + " to " + item.replacedFileName);
            return item.replacedFileName;
        }
        return oriFileName;
    }

    [ContextMenu("清除缓存")]
    public void ClearData()
    {
        DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath);
        if (info.Exists)
        {
            info.Delete();
        }
    }

    public static string DecryptStr(string pToDecrypt, string sKey)
    {
        DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
        byte[] buffer = Convert.FromBase64String(pToDecrypt);
        provider.Key = Encoding.ASCII.GetBytes(sKey);
        provider.IV = Encoding.ASCII.GetBytes(sKey);
        MemoryStream stream = new MemoryStream();
        CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
        stream2.Write(buffer, 0, buffer.Length);
        stream2.FlushFinalBlock();
        return Encoding.Default.GetString(stream.ToArray());
    }

    public static string EncryptStr(string pToEncrypt, string sKey)
    {
        DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
        byte[] bytes = Encoding.UTF8.GetBytes(pToEncrypt);
        provider.Key = Encoding.ASCII.GetBytes(sKey);
        provider.IV = Encoding.ASCII.GetBytes(sKey);
        MemoryStream stream = new MemoryStream();
        CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
        stream2.Write(bytes, 0, bytes.Length);
        stream2.FlushFinalBlock();
        byte[] inArray = stream.ToArray();
        return Convert.ToBase64String(inArray, 0, inArray.Length);
    }

    public static IEnumerator getAsset(string assetName, string fileName)
    {
        GameObject iteratorVariable2;
        WWW iteratorVariable1 = WWW.LoadFromCacheOrDownload("file:///" + PathConfig.getFilePath(ConfigType.PREFAB, assetName, true), 12);
        yield return iteratorVariable1;
        if (string.IsNullOrEmpty(iteratorVariable1.error))
        {
            Debug.Log(string.Concat(new object[] { "getAsset == ", fileName, "__", iteratorVariable1.assetBundle }));
            iteratorVariable2 = iteratorVariable1.assetBundle.LoadAsset(fileName) as GameObject;
        }
        else
        {
            Debug.LogError("error is === " + iteratorVariable1.error);
            iteratorVariable2 = null;
        }
        iteratorVariable1.assetBundle.Unload(false);
    }

    public static void getImg(string fileName, OnLoadResResult<Texture> call)
    {
        Texture2D ret = null;
        string str = PathConfig.getFilePath(ConfigType.IMAGE, fileName, true);
        if (string.IsNullOrEmpty(str) && (call != null))
        {
            call(null, false);
        }
        if (File.Exists(str))
        {
            FileStream stream = null;
            MemoryStream stream2 = null;
            ret = new Texture2D(4, 4);
            try
            {
                stream = File.Open(str, FileMode.Open);
                stream2 = new MemoryStream();
                int count = -1;
                int num2 = 0xc800;
                byte[] buffer = new byte[num2];
                while ((count = stream.Read(buffer, 0, num2)) > 0)
                {
                    stream2.Write(buffer, 0, count);
                }
                stream2.Flush();
                ret.LoadImage(stream2.GetBuffer());
            }
            catch
            {
                ret = null;
                File.Delete(str);
            }
            finally
            {
                if (stream2 != null)
                {
                    stream2.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }
        if (ret == null)
        {
            ret = Resources.Load(fileName, typeof(Texture)) as Texture2D;
        }
        if (call != null)
        {
            call(ret, false);
        }
    }

    public static bool getPackageResource<T>(string fileName, ref T result) where T : new()
    {
        bool flag;
        string path = Application.dataPath + "/Resources/" + fileName + ".xml";
        FileStream stream = null;
        StreamReader reader = null;
        try
        {
            stream = File.Open(path, FileMode.Open);
            reader = new StreamReader(stream);
            flag = FileTool.OpenSerializedInfoByContent<T>(reader.ReadToEnd(), ref result);
        }
        catch (Exception exception)
        {
            Debug.LogError("getPackageResource error : " + exception.Message);
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

    public static bool getResourceAtPath<T>(string path, ref T result) where T : new()
    {
        FileStream stream = null;
        StreamReader reader = null;
        bool flag;
        try
        {
            stream = File.Open(path, FileMode.Open);
            reader = new StreamReader(stream);
            flag = FileTool.OpenSerializedInfoByContent<T>(reader.ReadToEnd(), ref result);
        }
        catch (Exception exception)
        {
            Debug.Log("getPackageResource error : " + exception.Message);
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

    public static string GetRootDir()
    {
        return PathConfig.getDirPath(ConfigType.XML);
    }

    public static bool HasCfg(string cfgName)
    {
        if (File.Exists(PathConfig.getFilePath(ConfigType.XML, cfgName, true)))
        {
            return true;
        }
        TextAsset asset = Resources.Load("data/" + cfgName, typeof(TextAsset)) as TextAsset;
        return (asset != null);
    }

    public static bool loadConfig<T>(string fileName, ref T result, CallBack_Void_S onUpdateCallback, bool isDefault = false) where T : new()
    {
        fileName = CheckReplaceFile(fileName);
        if (onUpdateCallback != null)
        {
            if (!updateCallbacks.ContainsKey(fileName))
            {
                updateCallbacks.Add(fileName, null);
            }
            updateCallbacks[fileName] = onUpdateCallback;
        }
        bool flag = false;
        bool flag2 = false;
        string path = PathConfig.getFilePath(ConfigType.XML, fileName, true);
        if (File.Exists(path) && !isDefault)
        {
            flag = FileTool.OpenSerializedInfo<T>(path, ref result);
            if (!flag)
            {
                Debug.Log(fileName + " 更新配置文件损坏");
                flag2 = true;
                File.Delete(path);
            }
        }
        if ((((T)result) == null) || !flag)
        {
            TextAsset asset = Resources.Load("data/" + fileName, typeof(TextAsset)) as TextAsset;
            if ((asset == null) || string.IsNullOrEmpty(asset.text))
            {
                return flag2;
            }
            string text = asset.text;
            MemoryStream input = new MemoryStream();
            input.Write(asset.bytes, 0, asset.bytes.Length);
            input.Seek(0L, SeekOrigin.Begin);
            BinaryReader reader = new BinaryReader(input, Encoding.UTF8);
            try
            {
                string str3 = "";
                if (reader.ReadInt32() == 5)
                {
                    int num = reader.ReadInt32();
                    num = reader.ReadInt32();
                    str3 = MD5Code.Decode(reader.ReadString());
                    Debug.LogWarning(fileName + " 文件启用加密");
                }
                else
                {
                    Debug.LogWarning(fileName + " 文件没有加密");
                }
                if (!string.IsNullOrEmpty(str3))
                {
                    text = str3;
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(fileName + " 解析出错 : " + exception.Message);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            FileTool.OpenSerializedInfoByContent<T>(text, ref result);
        }
        return flag2;
    }

    public static void makesureFileExist(string path, bool isDir)
    {
        string str = path;
        if (!isDir)
        {
            str = path.Substring(0, path.LastIndexOf("/"));
        }
        if (!Directory.Exists(str))
        {
            Directory.CreateDirectory(str);
        }
    }

    public static void MoveFile(string scrPath, string destPath)
    {
        File.Move(scrPath, destPath);
    }

    public static void parseToConfig<T>(string content, ref T result)
    {
        FileTool.OpenSerializedInfoByContent<T>(content, ref result);
    }

    public static void ReloadConfig(string fileName)
    {
        try
        {
            if (updateCallbacks.ContainsKey(fileName) && (updateCallbacks[fileName] != null))
            {
                updateCallbacks[fileName](fileName);
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
    }

    public static void writeConfigToFile<T>(string fileName, T info, bool encrypt = true)
    {
        string path = PathConfig.getFilePath(ConfigType.XML, fileName, true);
        Debug.Log("Write config to File : " + path);
        FileTool.SerializeInfo<T>(info, path, encrypt);
    }

    public static void writeConfigToTmpFile<T>(string fileName, T info, bool encrypt = true)
    {
        string path = PathConfig.getTmpFilePath(ConfigType.XML, fileName);
        Debug.Log("Write config to temp File : " + path);
        FileTool.SerializeInfo<T>(info, path, encrypt);
    }

    public static void writeToFile(byte[] data, string filePath)
    {
        FileStream output = null;
        BinaryWriter writer = null;
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            output = File.Open(filePath, FileMode.OpenOrCreate);
            if (output != null)
            {
                writer = new BinaryWriter(output, Encoding.UTF8);
                writer.Write(data);
                writer.Close();
                output.Close();
                FileTool.EncryptFile(filePath);
            }
        }
        catch
        {
        }
        finally
        {
            if (writer != null)
            {
                writer.Close();
            }
            if (output != null)
            {
                output.Close();
            }
        }
    }

    public delegate void OnLoadResResult<T>(T ret, bool isNewVerResDirty = false);
}

public class ReplaceCfgItem
{
    public List<string> channels;
    public bool enable = true;
    public string oriFileName;
    public string replacedFileName;
}

public delegate void CallBack_Void_S(string param);
public enum ConfigType
{
    NONE,
    XML,
    PREFAB,
    TEXT,
    IMAGE
}

public class PathConfig
{
    private static string PATH_DIR_EXTERN = Application.persistentDataPath;
    private static string PATH_DIR_LOCAL = Application.streamingAssetsPath;
    private static string PATH_DIR_TEMP = (Application.persistentDataPath + "/temp/");

    public static string getDirPath(ConfigType type)
    {
        switch (type)
        {
            case ConfigType.XML:
                return (PATH_DIR_EXTERN + "/XML/");

            case ConfigType.PREFAB:
                return (PATH_DIR_EXTERN + "/PREFAB/");

            case ConfigType.TEXT:
                return (PATH_DIR_EXTERN + "/TEXT/");

            case ConfigType.IMAGE:
                return (PATH_DIR_EXTERN + "/IMG/");
        }
        return (PATH_DIR_EXTERN + "/");
    }

    public static string getFilePath(ConfigType type, string fileName, bool external)
    {
        string str = external ? PATH_DIR_EXTERN : PATH_DIR_LOCAL;
        switch (type)
        {
            case ConfigType.XML:
                return (str + "/XML/" + fileName + ".xml");

            case ConfigType.PREFAB:
                return (str + "/PREFAB/" + fileName + ".unity3d");

            case ConfigType.TEXT:
                return (str + "/TEXT/" + fileName);

            case ConfigType.IMAGE:
                return (str + "/IMG/" + fileName + ".png");
        }
        return (str + "/" + fileName);
    }

    public static string getTmpDirPath(ConfigType type)
    {
        switch (type)
        {
            case ConfigType.XML:
                return (PATH_DIR_TEMP + "/XML/");

            case ConfigType.PREFAB:
                return (PATH_DIR_TEMP + "/PREFAB/");

            case ConfigType.TEXT:
                return (PATH_DIR_TEMP + "/TEXT/");

            case ConfigType.IMAGE:
                return (PATH_DIR_TEMP + "/IMG/");
        }
        return (PATH_DIR_TEMP + "/");
    }

    public static string getTmpFilePath(ConfigType type, string fileName)
    {
        string str = PATH_DIR_TEMP;
        switch (type)
        {
            case ConfigType.XML:
                return (str + "/XML/" + fileName + ".xml");

            case ConfigType.PREFAB:
                return (str + "/PREFAB/" + fileName + ".unity3d");

            case ConfigType.TEXT:
                return (str + "/TEXT/" + fileName);

            case ConfigType.IMAGE:
                return (str + "/IMG/" + fileName + ".png");
        }
        return (str + "/" + fileName);
    }
}

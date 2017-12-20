using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ReadXML
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static bool loadConfig<T>(string path, ref T result, bool isDefault = false) where T : new()
        {
            bool flag = false;
            bool flag2 = false;
            if ((((T)result) == null) || !flag)
            {
                string str;
                StreamReader sr = new StreamReader(path, false);
                str = sr.ReadLine().ToString();
                sr.Close();

                if (string.IsNullOrEmpty(str))
                {
                    return flag2;
                }
                string text = str;
                MemoryStream input = new MemoryStream();
                byte[] allBytes = File.ReadAllBytes(path);
                input.Write(allBytes, 0, allBytes.Length);
                input.Seek(0L, SeekOrigin.Begin);
                BinaryReader reader = new BinaryReader(input, Encoding.UTF8);
                try
                {
                    string str3 = "";
                    if (reader.ReadInt32() == 5)
                    {
                        int num = reader.ReadInt32();
                        num = reader.ReadInt32();
                        //
                        //str3 = MD5Code.Decode(reader.ReadString());
                        //Debug.LogWarning(fileName + " 文件启用加密");
                    }
                    else
                    {
                        //Debug.LogWarning(fileName + " 文件没有加密");
                    }
                    if (!string.IsNullOrEmpty(str3))
                    {
                        text = str3;
                    }
                }
                catch (Exception exception)
                {
                    //Debug.LogError(fileName + " 解析出错 : " + exception.Message);
                }
                finally
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                }
                //FileTool.OpenSerializedInfoByContent<T>(text, ref result);
            }
            return flag2;
        }



        public static void ReadAll(string path,ref DataTable dt)
        {
            XmlTextReader reader = new XmlTextReader(path);
            //List<BookModel> modelList = new List<BookModel>();
            //BookModel model = new BookModel();
            List<string> NodeName = new List<string>();

            dt = new DataTable();


            string[] ElementName = null;
            string eleName = "";

            while (reader.Read())
            {

                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (ElementName == null)
                    {
                        string sp = "ArrayOf";
                        if (reader.Name.Contains(sp))
                        {
                            ElementName = reader.Name.Split(new string[] { sp }, StringSplitOptions.None);
                            for (int i = 0; i < ElementName.Length; i++)
                            {
                                eleName += ElementName[i];
                            }
                        }
                    }
                    else
                    {
                        string temp = reader.LocalName;
                        if (temp != eleName)
                        {
                            if (!NodeName.Contains(temp))
                            {
                                NodeName.Add(temp);
                            }
                            //dt.Rows.Add("temp", Type.GetType(temp));
                        }
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    //modelList.Add(model);
                    //model = new BookModel();
                }


            }
            reader.Close();
        }



      



    }
}

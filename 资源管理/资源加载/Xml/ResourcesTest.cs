using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

///<summary>
///
///</summary>

public class ResourcesTest : MonoBehaviour
{
    private void Start()
    {
        SerilizeTest();
    }
    void SerilizeTest()
    {
        TestSerilize testSerilize = new TestSerilize();
        testSerilize.id = 1;
        testSerilize.Name = "测试";
        testSerilize.list = new List<int>();
        testSerilize.list.Add(1);
        testSerilize.list.Add(2);

        XmlSerilize(testSerilize);
    }
    void XmlSerilize(TestSerilize testSerilize) 
    {
        FileStream fileStream = new FileStream(Application.dataPath + "/test.xml",FileMode.Create,FileAccess.ReadWrite,FileShare.ReadWrite);
        StreamWriter streamWriter = new StreamWriter(fileStream,System.Text.Encoding.UTF8);
        //创建xml 传入type
        XmlSerializer xml = new XmlSerializer(testSerilize.GetType());
        //xml序列化传入数据流和对象
        xml.Serialize(streamWriter,testSerilize);
        streamWriter.Close();
        fileStream.Close();
        AssetDatabase.Refresh();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

///<summary>
///
///</summary>

//序列化必须有标签，对应属性也有对应标签
[System.Serializable]
public class TestSerilize
{
    [XmlAttribute("Id")]
    public int id { get; set; }
    [XmlAttribute("Name")]
    public string Name { get; set; }
    [XmlElement("list")] //[XmlArray] 容器可以使用这两种标签
    public List<int> list { get; set; }
}

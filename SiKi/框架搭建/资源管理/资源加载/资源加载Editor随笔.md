资源加载Editor随笔

获得选中的文件的路径

```c#
[MenuItem("HSGameEngine/生成当前选择资源的AB文件名称 %H")]
public static void SetAssetsBundleName()
{
	UnityEngine.Object[] selection = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
	foreach (UnityEngine.Object obj in selection)
	{
		string path = AssetDatabase.GetAssetPath(obj);//选中的文件夹
	}
}
```

设置文件ab包名称

```c#
public static void SetAssetsBundleName(string path)
{
    Debug.Log(path);
    //获取资源
    var importer = AssetImporter.GetAtPath(path);
    string[] strs = path.Split('.');
    string[] dictors = strs[0].Split('/');
    string name = "";
    for (int i = 2; i < dictors.Length; i++)
    {
        if (i < dictors.Length - 1)
        {
            name += dictors[i] + "/";
        }
        else
        {
            name += dictors[i];
        }
    }
    name += ".unity3d";
    if (importer != null)
    {
        //设置ab包名称
        importer.assetBundleName = name;
    }
    else
        Debug.Log("importer是空的");
}
```


# FairyGUI解决方案

## 折叠菜单选中栏

![1638780572389](C:\Users\Administrator\AppData\Roaming\Typora\typora-user-images\1638780572389.png)

### GTree继承于GList，GList所有API适用于GTree，但不支持虚拟化

1.需要在FairyGUI内将List设置-树视图-激活，设置项目资源(折叠节点)

2.在代码内加载

```c#
view.m_list.rootNode.RemoveChildren();
view.m_list.treeNodeRender = RenderTreeNode;//设置渲染调用的函数
view.m_list.onClickItem.Add(SwitchServer);//添加点击回调

GTreeNode treeNode = new GTreeNode(true);//true为文件夹节点,false为叶子节点
//给list添加文件夹节点
view.m_list.rootNode.AddChild(treeNode);

//子节点，需要填入url资源路径
GTreeNode treeNode2 = new GTreeNode(false, "ui://cb3r3hhtgdcra");
//设置data给组件提供数据
treeNode2.data=newdata;
//给文件夹节点添加子节点
treeNode.AddChild(treeNode2);
```

3.渲染函数

```c#
private void RenderTreeNode(GTreeNode node, GComponent obj)
{
	GComponent objQ = node.cell;
	//判断当前节点的层级，如果为1为文件夹节点，不为1为子节点
	if (node.level != 1)
	{
		ServerCity serverCity = node.data as ServerCity;
		objQ.GetChild("lab_name").text = serverCity.Desc;
		objQ.GetChild("lab_ID").text = serverCity.Server.ToString();
		objQ.GetChild("lab_time").text = serverCity.Opentime.Split(' ')[0] ;
	}
}
```

4.设置点击回调

同理使用node.level判断层级分别进行点击操作。

文件夹节点，**点击默认操作为打开文件夹**。

加载资源不需要为按钮组件也可以添加点击操作和点击回调。
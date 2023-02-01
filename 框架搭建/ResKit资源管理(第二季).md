# 资源管理

## 资源加载、卸载

`Resources.UnloadAsset()`方法卸载Resources.Load加载的资源

```c#
IEnumerator Start()
{
	var audioClip = Resources.Load<AudioClip>("coin");
	yield return new WaitForSeconds(5.0f);
	Resources.UnloadAsset(audioClip);
}
```

在Profiler(性能分析)-Memory中可以查看加载的资源以及是否被卸载

### Prefab卸载

Prefab不能直接用`Resources.UnLoadAsset`卸载。可以使用`Resources.UnloadUnusedAssets();`卸载

`Resources.UnloadUnusedAssets();`只能卸载没有引用的资源，无法精确卸载。从工作机制上，它会触发`GC.Collect`的调用。复杂项目中会产生很多不可预见的bug。

```c#
IEnumerator Start()
{
	var homePanel = Resources.Load<GameObject>("HomePanel");
	yield return new WaitForSeconds(5.0f);
	homePanel = null;
	Resources.UnloadUnusedAssets();
}
```

### 重复加载

`Resources`的API，重复加载或重复卸载，Unity都做了容错处理。但是AssetBundle就比较危险，重复添加会导致游戏闪退。

对加载逻辑进行封装，在加载、卸载过程中判断是否重复加载、卸载。

```c#
public class ResLoader
{

	private List<Res> mResRecord = new List<Res>();


	public T LoadAsset<T>(string assetName) where T : Object
	{
		// 查询当前的 资源记录
		var res = mResRecord.Find(loadedAsset => loadedAsset.Name == assetName);

		if (res != null)
		{
			return res.Asset as T;
		}

		// 查询全局资源池
		res = ResMgr.Instance.SharedLoadedReses.Find(loadedAsset => loadedAsset.Name == assetName);

		if (res != null)
		{
			mResRecord.Add(res);

			res.Retain();

			return res.Asset as T;
		}


		// 真正加载资源
		var asset = Resources.Load<T>(assetName);

		res = new Res(asset);

		res.Retain();

		ResMgr.Instance.SharedLoadedReses.Add(res);

		mResRecord.Add(res);

		return asset;
	}


	public void ReleaseAll()
	{
		mResRecord.ForEach(loadedAsset => loadedAsset.Release());

		mResRecord.Clear();
	}
}
```

### 加载、卸载的策略复用

资源管理除了加载和卸载资源以外，还需要进行预加载，预加载操作可能是在还没有打开界面的时候，通过操作一些静态或者单例的管理类进行资源的准备；而当退出当前模块时，这个模块的资源需要卸载。

#### 服务类(组件)

服务类创建的对象本身有状态(数据)，也提供了API(服务)，当需要使用时直接new类对象来使用。

### 全局资源池

类似对象池的做法，将加载过的资源缓存到资源池中，当重复加载时直接返回池中资源，避免重复加载。

### 引用计数系统

当一个资源每被引用一次时，计数变量都进行+1的操作，当不用时则进行-1的操作。当计数的变量为0时，则对资源进行卸载。

当加载和从全局池中获取时都需要对资源的计数变量进行+1的操作

```c#
public T LoadAsset<T>(string assetName) where T : Object
{
	// 查询当前的 资源记录
	var res = mResRecord.Find(loadedAsset => loadedAsset.Name == assetName);

	if (res != null)
	{
		return res.Asset as T;
	}

	// 查询全局资源池
	res = ResMgr.Instance.SharedLoadedReses.Find(loadedAsset => loadedAsset.Name == assetName);

	if (res != null)
	{
		mResRecord.Add(res);

		res.Retain();

		return res.Asset as T;
	}


	// 真正加载资源
	var asset = Resources.Load<T>(assetName);

	res = new Res(asset);

	res.Retain();

	ResMgr.Instance.SharedLoadedReses.Add(res);

	mResRecord.Add(res);

	return asset;
}
```

## 简易计数器（引用计数）

引用计数的应用很广泛，很多引擎、框架、插件的底层都离不开引用计数

定义接口

```c#
public interface IRefCounter
{
    int RefCount { get; }

    void Retain(object refOwner = null);

    void Release(object refOwner = null);
}
```

```c#
public class SimpleRC : IRefCounter
{
    public int RefCount { get; private set; }

    public void Retain(object refOwner = null)
    {
        RefCount++;
    }

    public void Release(object refOwner = null)
    {
        RefCount--;

        if (RefCount == 0)
        {
            OnZeroRef();
        }
    }

    protected virtual void OnZeroRef()
    {

    }
}
```

## 享元模式

**享元（Flyweight）模式的定义**：运用共享技术来有效地支持大量细粒度对象的复用。它通过共享已经存在的对象来大幅度减少需要创建的对象数量、避免大量相似类的开销，从而提高系统资源的利用率。

主要用于减少创建对象的数量，以减少内存占用和提高性能。

享元模式尝试重用现有的同类对象，如果未找到匹配的对象，则创建新的对象。

主要是分离好内部状态和外部状态，外部状态具有固优化的性质，不应该随着内部状态而改变。

**优点**是：相同对象只要保存一份 

**缺点**是：

1. 为了使对象可以共享，需要将一些不能共享的状态外部化，这将增加程序的复杂性。
2. 读取享元模式的外部状态会使得运行时间稍微变长。

应用场景：1.系统有大量相似对象。2.需要缓冲池的场景。

享元模式和对象池又不完全相同。对象池更注重于资源的回收和创建。而享元模式更注重于共享资源，例如渲染森林时，树的模型信息基本一致则该部分信息是共享的，位置缩放的信息每颗树都不相同。则可以渲染相同的模型实例，只是位置不相同。

## 资源信息显示

目前比较重要的信息是，资源的名字、资源的引用次数。

可以使用编写editor在游戏运行时查看。

或在IMGUI中显示，注意只在开发中显示

```c#
#if UNITY_EDITOR
  private void OnGUI()
	{
  	if(Input.GetKey(KeyCode.F1))
    {
      GUILayout.BeginVertical("box");
      foreach(var res in ResList)
      {
        GUILayout.Label(string.Format("name:{0} RefCount:{1}",res.name,res.refCount));
			}
      GUILayout.EndVertical();
		}
	}
}
#endif
```



## AssetBundle

### 打包AssetBundle

```c#
#if UNITY_EDITOR
    [MenuItem("Tools/AseetBundle打包")]
    static void MenuItem()
    {
        if (! Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath,BuildAssetBundleOptions.None,BuildTarget.StandaloneWindows);
    }
#endif
```

### 加载AssetBundle

```c#
AssetBundle.LoadFromFile();
```

### 从AssetBundle中加载资源

```c#
AssetBundle.LoadAsset<T>(name);
```

### 卸载AssetBundle

```c#
AssetBundle.UnLoad(true);
```

## 路由设计模式

路由设计模式没有在23种设计模式中，不过很多web框架都实现了这种模式。

简单说路由就是url到函数的映射

[iOS 浅析路由设计模式 - 简书 (jianshu.com)](https://www.jianshu.com/p/e1653e28a37f)

通过url定义表去索引到具体的函数实现，在代码中可以用掩码的方式实现。

```c#
//对应具体函数实现
enum FuncFlag
{
  A = 1,
  B = 2,
  C = 3,
}
//具体功能对应的需要实现的函数
//不同路径根据路由实现不同的功能
enum ResType
{
  Resources = FuncFlag.A|FuncFlag.B,
  AssetBundle = FuncFlag.B|FuncFlag.C,
}
//需要一个处理中心，对掩码进行解析并调用对应的函数
```

## 桥接模式

桥接模式将抽象化与实现化脱耦，使得二者可以独立的变化，把强关联变成弱关联，也就是在抽象化和实现化之间使用组合/聚合关系而不是继承关系。让两者都可以独立变化。

说人话就是把公有行为抽象出来，不同角色进行不同实现。比如各种颜色的玩具鸭子，颜色只是公有属性，具体颜色才是具体角色所持有的。

**缺点**就是增加了复杂度，在多层桥接模式下代码变得复杂很多。当需要多层桥接的时候需要考虑下是否好维护，可以改为组件组合的形式来降低复杂度。

## 第二章 C#技术要点

### 2.1　Unity3D中C#的底层原理

Unity底层在运行C#程序时有两种机制，一种是Mono，另一种是IL2CPP。

IL即Intermediate Language，但很多时候我们看到的CIL（Common Intermediate Language，特指在.NET平台下的IL标准），其实大部分文章中提到的IL和CIL表示的是同一个东西，即中间语言。

将通用语言翻译成IL，然后汇编成字节码，最后运行在虚拟机上；也可以把IL看作一个面向对象的汇编语言，只是它必须运行在虚拟机上，而且是完全基于堆栈的语言。

不同语言会被个字的编译器编译成中间语言(IL)，需要时则实时加载到运行时库中，由虚拟机动态编译(JIT)成汇编代码并执行

#### IL的三种转译模式：

- Just-In-Time(JIT)编译：在程序运行过程中将CIL转移成机器码。
- Ahead-Of-Time(AOT)编译：将IL转译成机器码并存储在文件中，此文件并不能完全独立运行。通常此模式可产生出绝大部分JIT模式所产生的机器码，只是有部分例外，例如trampolines或是控管监督相关的代码仍旧需要JIT来运行。
- 完全静态编译：这个模式只支持少数平台，它基于AOT编译模式更进一步产生所有的机器码。完全静态编译模式可以让程序在运行期完全不需要用到JIT，这个做法适用于iOS操作系统、PlayStation 3以及XBox 360等不允许使用JIT的操作系统。

在打包IOS系统时使用完全静态编译，而在Android和Windows则使用JIT实时编译来运行代码。

C#代码生成的IL编码称为**托管代码**，由虚拟机的JIT编译执行，其中对象无需手动释放，他们由**GC**管理。

C/C++或C#中以不安全类型写的代码我们称为非托管代码，虚拟机无法跟踪到这类代码对象，因此在Unity中有托管代码和非托管代码之分。

一般情况下，我们使用托管代码来编写游戏逻辑，非托管代码通常用于更底层的架构、第三方库或者操作系统相关接口，非托管代码使用这部分的内存必须由程序员自己来管理，否则会造成运行时错误或者内存泄漏。

#### Mono使用垃圾回收机制来管理内存

应用程序向垃圾回收器申请内存，最终由垃圾回收器决定是否回收。当我们向垃圾回收器申请内存时，如果发现内存不足，就会自动触发垃圾回收，或者也可以主动触发垃圾回收，垃圾回收器此时会遍历内存中所有对象的引用关系，如果没有被任务对象引用则会释放内存。如果内存还是不够时，则会向系统申请更大的内存。

#### IL2CPP

由Mono将C#语言翻译成IL，IL2CPP在得到中间语言IL后，将它们重新变回C++代码，再由各个平台的C++编译器直接编译成能执行的机器码。

虽然C#代码被翻译成了C++代码，但IL2CPP也有自己的虚拟机，IL2CPP的虚拟机并不执行JIT或者翻译任何代码，它主要是用于内存管理，其内存管理仍然采用类似Mono的方式，因此程序员在使用IL2CPP时无须关心Mono与IL2CPP之间的内存差异。

Unity在iOS平台中使用基于AOT的完全静态编译绕过了JIT，使得Mono能在这些不支持JIT的操作系统中使用。对于IL2CPP来说，其实就相当于静态编译了C#代码，只是这次编译成了C++代码，最后翻译成二进制机器码绕过了JIT，所以也可以说IL2CPP实现了另一种AOT完全静态编译。

##### C++代码和C#代码的跨平台区别

C#  .Net Java的跨平台性是指编译时编译成中间语言(例如c#的IL语言)，并在虚拟机上运行IL语言再解释称机器语言执行。受限于虚拟机，如果系统不支持虚拟机则不能运行。 属于解释性语言 

C++的跨平台，代码通过编译器直接将代码生成为 运行系统的机器语言，受限于编译器， 属于编译性语言 

### 2.2　List底层源码剖析

Add、Insert、IndexOf、Remove接口都是没有做过任何形式优化的，使用的都是顺序迭代的方式，如果过于频繁使用，效率就会降低，也会造成不少内存的冗余，使得垃圾回收（GC）时要承担更多的压力。

List并不是高效的组件，真实情况是，它比数组的效率还要差，它只是一个兼容性比较强的组件而已，好用但效率并不高。

#### Add接口

```c#
public void Add(T item)
{
    //判断容量够不够，不够列表容量扩容1倍
    if(_size==_items.Length) EnsureCapacity(_size+1);
    _items[_size++]=item;
    _version++;//版本 迭代器判断
}
//如果列表的当前容量小于min，则容量将增加到当前容量的两倍或min，以较大者为准。
private void EnsureCapacity(int min)
{
    if(_items.Length<min)
    {
        int newCapacity = _items.Length == 0? _defaultCapacity : _items.Length * 2;
        //在遇到溢出之前，允许列表增长到最大可能的容量(约2GB元素)
        //即使_items.Length由于(uint)强制转换而溢出，此检查仍然有效。
        if((uint)newCapacity > Array.MaxArrayLength) 
            newCapacity = Array.MaxArrayLength;
        if(newCapacity < min)
            newCapacity = min;
        Capacity = newCapacity;
    }
}
```

Add()函数，每次增加一个元素的数据，Add接口都会首先检查容量是够还是不够，如果不够，则调用EnsureCapacity()函数来增加容量。

_defaultCapacity表示容量的默认值为4。**每次扩容都会针对数组进行new操作都会造成内存垃圾，这给垃圾回收（GC）带来了很大负担。**

#### Remove接口

```c#
// 删除给定索引处的元素。列表的大小减1
// 
public bool Remove(T item) {
    int index = IndexOf(item);
    if (index>= 0) {
        RemoveAt(index);
        return true;
    }

    return false;
}
// 删除给定索引处的元素。列表的大小减1
// 
public void RemoveAt(int index) {
    if ((uint)index>= (uint)_size) {
       ThrowHelper.ThrowArgumentOutOfRangeException();
    }
    Contract.EndContractBlock();
    _size--;
    if (index<_size) {
        Array.Copy(_items, index + 1, _items, index, _size - index);
    }
    _items[_size] = default(T);
    _version++;
}
```

元素删除的原理就是使用Array.Copy对数组进行覆盖。IndexOf()是用Array.IndexOf接口来查找元素的索引位置，这个接口本身的内部实现就是按索引顺序从0到n对每个位置进行比较，复杂度为线性迭代O(n)。

#### Insert接口

```c#
// 在给定索引处将元素插入此列表，列表的大小增加1
// 如果需要，在插入新元素之前，列表的容量会增加一倍
// 
public void Insert(int index, T item) {
    // 请注意，结尾处的插入是合法的
    if ((uint) index>(uint)_size) {
        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, 
            ExceptionResource.ArgumentOutOfRange_ListInsert);
    }
    Contract.EndContractBlock();
    if (_size == _items.Length) EnsureCapacity(_size + 1);
    if (index<_size) {
        Array.Copy(_items, index, _items, index + 1, _size - index);
    }
    _items[index] = item;
    _size++;
    _version++;
}
```

Add接口一样，先检查容量是否足够，不足则扩容。从以上源码中获悉，Insert()函数插入元素时，使用的是复制数组的形式，将数组里指定元素后面的所有元素向后移动一个位置。

#### Clear接口

```c#
// 清除列表的内容
public void Clear() {
    if (_size>0)
    {
        Array.Clear(_items, 0, _size); // 无须对此进行记录，我们清除了元素，以便gc可以回收引用
        _size = 0;
    }
    _version++;
}
```

Clear接口是清除数组的接口，**在调用时并不会删除数组，而只是将数组中的元素设置为0或NULL，并设置_size为0**而已，用于虚拟地表明当前容量为0。

当元素为引用类型时，设为NULL可以让GC识别，当触发GC时可以进行回收。

#### Contains接口

```c#
// 如果指定的元素在List中，则Contains返回true
// 它执行线性O（n）搜索。平等是通过调用item.Equals()来确定的
// 
public bool Contains(T item) {
    if ((Object) item == null) {
        for(int i=0; i<_size; i++)
            if ((Object) _items[i] == null)
                return true;
        return false;
    }
    else {
        EqualityComparer<T>c = EqualityComparer<T>.Default;
        for(int i=0; i<_size; i++) {
            if (c.Equals(_items[i], item)) return true;
        }
        return false;
    }
}
```

Contains接口是使用线性查找方式比较元素，对数组执行循环操作，比较每个元素与参数实例是否一致，如果一致则返回true，全部比较结束后还没有找到，则认为查找失败。

#### Enumerator接口

```c#
// 返回具有给定删除元素权限的此列表的枚举数
// 如果在进行枚举时对列表进行了修改，
// 则枚举器的MoveNext和GetObject方法将引发异常
// 
public Enumerator GetEnumerator() {
    return new Enumerator(this);
}

/// 仅供内部使用
IEnumerator<T>IEnumerable<T>.GetEnumerator() {
    return new Enumerator(this);
}

System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
    return new Enumerator(this);
}

[Serializable]
public struct Enumerator : IEnumerator<T>, System.Collections.IEnumerator
{
    private List<T>list;
    private int index;
    private int version;
    private T current;

    internal Enumerator(List<T>list) {
        this.list = list;
        index = 0;
        version = list._version;
        current = default(T);
    }

    public void Dispose() {
    }

    public bool MoveNext() {

        List<T>localList = list;

        if (version == localList._version && ((uint)index<(uint)localList._size))
        {
            current = localList._items[index];
            index++;
            return true;
        }
        return MoveNextRare();
    }

    private bool MoveNextRare()
    {
        if (version != list._version) {
            ThrowHelper.ThrowInvalidOperationException(
                ExceptionResource.InvalidOperation_EnumFailedVersion);
        }

        index = list._size + 1;
        current = default(T);
        return false;
    }

    public T Current {
        get {
            return current;
        }
    }

    Object System.Collections.IEnumerator.Current {
        get {
            if( index == 0 || index == list._size + 1) {
                ThrowHelper.ThrowInvalidOperationException(
                    ExceptionResource.InvalidOperation_EnumOpCantHappen);
            }
            return Current;
        }
    }

    void System.Collections.IEnumerator.Reset() {
        if (version != list._version) {
            ThrowHelper.ThrowInvalidOperationException(
                ExceptionResource.InvalidOperation_EnumFailedVersion);
        }

        index = 0;
        current = default(T);
    }

}
```

Enumerator接口是枚举迭代部分细节的接口，其中要注意Enumerator这个结构，**每次获取迭代器时，Enumerator都会被创建出来，如果大量使用迭代器，比如foreach，就会产生大量的垃圾对象**，这也是为什么我们常常告诫程序员尽量不要使用foreach，因为**List的foreach会增加新的Enumerator实例，最后由GC单元将垃圾回收掉。**虽然.NET在4.0后已经修复了此问题，但仍然不建议大量使用foreach。

### 2.3　Dictionary底层源码剖析

给定Key值，根据造表时设定的Hash函数求得Hash地址，若表中此位置没有记录，则表示查找不成功

Hash函数是从关键字范围到索引范围的映射，通常关键字范围要远大于索引范围，它的元素包括多个可能的关键字。当key不同时，HashCode可能相同，Hash冲突只能尽可能少，不能完全避免。

**Dictionary使用的解决冲突方法是拉链法，又称链地址法。**

#### 拉链法的原理

将所有关键字为同义词的节点链接在同一个单链表中。若选定的Hash表长度为n，则可将Hash表定义为一个由n个头指针组成的指针数组T[0...n-1]。凡是Hash地址为i的节点，均插入以T[i]为头指针的单链表中。T中各分量的初值均为空指针。

给定Key值，根据造表时设定的Hash函数求得Hash地址，若表中此位置没有记录，则表示查找不成功；否则比较关键字，若给定值相等，则表示查找成功；否则，根据处理冲突的方法寻找“下一地址”，直到Hash表中某个位置为空或者表中所填记录的关键字等于给定值为止。

#### Hash冲突拉链法

Hash冲突拉链法结构中，主要的宿主为数组指针，每个数组元素里存放着指向下一个节点的指针，如果没有元素在单元上，则为空指针。当多个元素都指向同一个单元格时，则以链表的形式依次存放并列的元素。

#### Dictionary接口

```c#
public class Dictionary<TKey,TValue>: IDictionary<TKey,TValue>, IDictionary, 
    IReadOnlyDictionary<TKey, TValue>, ISerializable, IDeserializationCallback
{

    private struct Entry {
        public int hashCode;            // 低31位为Hash值，如果未使用则为-1
        public int next;                // 下一个实例索引，如果是最后一个则为-1
        public TKey key;                // 实例的键值
        public TValue value;            // 实例的值
    }

    private int[] buckets;
    private Entry[] entries;
    private int count;
    private int version;
    private int freeList;
    private int freeCount;
    private IEqualityComparer<TKey>comparer;
    private KeyCollection keys;
    private ValueCollection values;
    private Object _syncRoot;
}
```

Dictionary是以数组为底层数据结构的类。当实例化new Dictionary()后，内部的数组是0个数组的状态。与List组件一样，Dictionary也是需要扩容的，会随着元素数量的增加而不断扩容。
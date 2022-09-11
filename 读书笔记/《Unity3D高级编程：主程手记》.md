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

Dictionary同List一样，并不是线程安全的组件，Hashtable在多线程读/写中是线程安全的，而Dictionary不是。如果要在多个线程中共享Dictionary的读/写操作，就要自己写lock，以保证线程安全。

使用数值方式作为键值比使用类实例的方式更高效，因为类对象实例的Hash值通常都由内存地址再计算得到。

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

#### Remove接口

```c#
public bool Remove(TKey key)
{
    if(key == null) {
        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
    }

    if (buckets != null) {
        int hashCode = comparer.GetHashCode(key) & 0x7FFFFFFF;
        int bucket = hashCode % buckets.Length;
        int last = -1;
        for (int i = buckets[bucket]; i>= 0; last = i, i = entries[i].next) {
            if (entries[i].hashCode == hashCode && comparer.Equals(entries[i].
                key, key)) {
                if (last<0) {
                    buckets[bucket] = entries[i].next;
                }
                else {
                    entries[last].next = entries[i].next;
                }
                entries[i].hashCode = -1;
                entries[i].next = freeList;
                entries[i].key = default(TKey);
                entries[i].value = default(TValue);
                freeList = i;
                freeCount++;
                version++;
                return true;
            }
        }
    }
    return false;
}
```

Remove接口相对于Add接口简单得多，同样使用Hash函数comparer.GetHashCode()获得Hash值，再执行余操作，确定索引值落在数组范围内，从Hash索引地址开始查找链表中的值，查找冲突链表中元素的Key值是否与需要移除的Key值相同，若相同，则进行移除操作并退出。

Remove()函数的移除操作并没有对内存进行删减，而只是将其单元格置空，这是为了减少内存的频繁操作。

### 2.4　浮点数的精度问题

根据IEEE 754标准，任意一个二进制浮点数F均可表示为`F=(-1^s)×(1.M)×(2^e)`

分为3个部分：

符号部分即s部分、尾数部分即M部分、阶码部分即e部分。

- s为符号位0或1；
- M为尾数，是指具体的小数，用二进制表示，它完整地表示为1.xxx中1后面的尾数部分，也是因此才称它为尾数。二进制直译的那种，然后再根据阶码来平移小数点，最后根据小数点的左右部分分别得出整数部分和小数部分的数据。；
- e是比例因子的指数，是浮点数的指数。

#### 浮点数产生的问题

1. 数值比较不相等
  - 浮点数在加减乘除时无法准确定位到某个值，这时我们不得不放弃“==”（等于号）而选择“>”（大于号）或者“<”（小于号）来解决这种问题的出现。
2. 数值计算不确定
  - 经常会遇到这样的情况，在外圈的if判断成立，理论上同样的结果只是公式不同，它们在内圈的if判断却可能不成立，使得程序出现异常行为，因为看起来应该是得到同样的数值，但结果却不一样。
3. 不同设备的计算结果不同

#### 解决方案

1. 由一台机器决定计算结果，只计算一次，且认定这个值为准确值
2. 改用int或long类型来替代浮点数。
  - 把小数乘以10的幂次进行计算，一般为万分比。
3. 用定点数保持一致性并缩小精度问题。
   - 把整数部分和小数部分拆分开来，都用整数的形式表示，这样计算和表达都使用整数的方式。缺点是由于拆分了整数和小数，两个部分都要占用空间，所以受到存储位数的限制。
   - c#的decimal整数类型，是基础类型的补充类型，拥有更高的精度，却比float范围小。内部实现就是定点数。
   - decimal精度比较大，精度范围为28个有效位，另外任何与它一起计算的数值都必须先转化为decimal类型，否则就会编译报错，数值不会隐式地自动转换成decimal。
   - 实际上大部分项目都会自己实现定点数，用两个int表示整数或小数部分，或用long类型（前32位存储整数，后32位存储浮点数）。并自己重载运作算符号。
4. 用字符串代替浮点数。
   - 如果想要精确度非常高，定点数和浮点数无法满足要求，那么就可以用字符串代替浮点数来计算。但它的缺点是CPU和内存的消耗特别大，只能做少量高精度的计算。

### 2.5　委托、事件、装箱、拆箱

#### 委托与事件

在创建委托时，其实就是创建一个delegate类实例，这个delegate委托类继承了System.MulticastDelegate类，类实例里有BeginInvoke()、EndInvoke()、Invoke()这三个函数，分别表示异步开始调用、结束异步调用及直接调用。

PS:异步调用在后续版本可能不让使用

Delegate类中有一个变量是用来存储函数地址的，当变量操作=（等号）时，把函数地址赋值给变量保存起来。不过这个存储函数地址的变量是一个可变数组，你可以认为它是一个链表，每次直接赋值时会换一个链表。

##### Event事件

Event在delegate上又做了一次封装，限制用户直接操作delegate实例中变量的权利。

被event声明的委托不再提供“=”操作符，但仍然有“+=”和“-=”操作符可供注册委托和注销委托操作。

**限制的意义在于：公开的delegate会直接暴露在外，随时会被“=”赋值而清空前面累积起来的委托链表，委托的操作权限范围太大，导致问题会比较严重。**

事件保证了“谁注册就必须谁负责销毁”。

#### 装箱和拆箱

当声明一个类时，只在堆栈（堆或栈）中分配一小片内存用于容纳一个地址，而此时并没有为其分配堆上的内存空间，因此它是空的，为null，直到使用new创建一个类的实例，分配了一个堆上的空间，并把堆上空间的地址保存给这个引用变量，这时这个引用变量才真正指向内存空间。

##### 装箱

```c#
int a = 5;
object obj = a;
```

因为a是值类型，是直接有数据的变量，obj为引用类型，指针与内存拆分开来，把a赋值给obj，实际上就是obj为自己**创建了一个指针，并指向了a的数据空间。**

##### 拆箱

```c#
a = (int)obj;
```

相当于把obj指向的内存空间复制一份交给了a，因为a是值类型，所以它不允许指向某个内存空间，只能靠**复制数据**来传递数据。

##### 栈和堆

**栈**是用来存放对象的一种特殊的容器，它是最基本的数据结构之一，遵循**先进后出**的原则。**它是一段连续的内存**，所以**对栈数据的定位比较快速**；而**堆则是随机分配的空间**，处理的数据比较多，**无论情况如何，都至少要两次才能定位**。堆内存的**创建和删除节点的时间复杂度是O(lgn)。栈创建和删除的时间复杂度则是O(1)，栈速度更快。**

栈中的生命周期必须确定，销毁时必须按次序销毁，即从最后分配的块部分开始销毁，创建后什么时候销毁必须是一个定量，所以在分配和销毁上不灵活，它基本都用于函数调用和递归调用这些生命周期比较确定的地方。

堆内存可以存放生命周期不确定的内存块，满足当需要删除时再删除的需求，所以堆内存相对于全局类型的内存块更适合，分配和销毁更灵活。

###### 内存

栈内存主要为确定性生命周期的内存服务，堆内存则更多的是无序的随时可以释放的内存。因此值类型可以在堆内也可以在栈内，引用类型的指针部分也一样，可以在栈内和堆内，区别在于引用类型指向的内存块都在堆内，一般这些内存块都在委托堆内，这样便于内存回收和控制

##### 装箱步骤

1. 在堆内存中新分配一个内存块（大小为值类型实例大小加上一个方法表指针和一个SyncBlockIndex类）。
2. 将值类型的实例字段复制到新分配的内存块中。
3. 返回内存堆中新分配对象的地址。这个地址就是一个指向对象的引用。

##### 拆箱步骤

先检查对象实例，确保它是给定值类型的一个装箱值，再将该值从实例复制到值类型变量的内存块中。

**由于装箱、拆箱时生成的是全新的对象，不断地分配和销毁内存不但会大量消耗CPU，同时也会增加内存碎片，降低性能**

##### Struct的拆装箱优化

1. Struct通过重载函数来避免拆箱、装箱
   - ToString()、GetType()方法，如果Struct没有写重载ToString()和GetType()的方法，就会在Struct实例调用它们时先装箱再调用，导致内存块重新分配，性能损耗，所以对于那些需要调用的引用方法，必须重载。
2. 通过泛型来避免拆箱、装箱。
   - Struct也是可以继承的，在不同的、相似的、父子关系的Struct之间可以使用泛型来传递参数，这样就不用在装箱后再传递了。
3. 通过继承统一的接口提前拆箱、装箱，避免多次重复拆箱、装箱。
   - 很多时候拆箱、装箱不可避免，这时可以让多种Struct继承某个统一的接口，不同的Struct可以有相同的接口。把Struct传递到其他方法里，就相当于提前进行了装箱操作，在方法中得到的是引用类型的值，并且有它需要的接口，避免了在方法中完成重复多次的拆箱、装箱操作。

### 2.6　排序算法

#### 2.6.1 快速排序

##### 排序步骤

1. 从序列中选一个元素作为基准元素。
2. 把所有比基准元素小的元素移到基准元素的左边，把比基准元素大的移到右边。
3. 对分开来的两个（一大一小）区块依次进行递归、筛选后，再对这两个区块进行前两个步骤的处理。

##### 优化快速排序

1. 随机选择中轴数

   中轴数影响算法效率。虽然随机选是为了减少选到最大值和最小值的概率，但随机也会选到不好的中轴数。实际对效率没有多大帮助。

2. 三数取中

   让选择的中轴数更接近中位数，**可以将头、中、尾3个数字先进行排序，最小的数字放在头部，中间的数字放在中部，最大的数字放在尾部**，然后用3个数字去提高有效接近中位数的中轴元素。

   这样取出来的中轴数保证不是最小的，接近中位数的概率更大。

3. 小区间使用插入排序

   **插入排序的特点是排序序列越长，效率越差。短序列的排序效果很好，高效排序序列长度为8左右。**于是我们可以用这个特点来改善快速排序中的效率，即当切分的区块小于或等于8个时，就采用插入排序来替代快速排序。

4. 缩小分割范围，与中轴数相同的合并在一起

   在每次的分割比较中，**当元素与中轴数相等时，直接将其移动到中轴数身边，移动完毕后划分范围从中轴数变为最边上相同元素的位置**，使用这种方式来缩小范围，后续可减少排序元素。

#### 2.6.2 最大最小堆

最大最小堆，它其实就是堆排序的优先级队列。

普通堆排序比快速排序更低效，但堆排序中的最大最小堆的优先级队列非常有用，即**只关注最大值或最小值，在不断增加和删除根节点元素的情况下仍可获取最大值或最小值**。

寻路系统的A星算法中特别有用，因此最大最小堆排序常用于A星算法。

##### 一维数组表示最大最小堆结构

堆排序是一种完全二叉树结构(每个节点如果有叶子节点都为两个)，所以可以用一维数组表示，数组的内存连续效率更高。

即如果i为节点索引，i2和i2+1就是它的两个子节点，而索引i的父节点位置可以用i/2来表示，数组中的任何节点都应遵循这种规则。

(i2)2和(i2)2+1就是i2这个索引的两个子节点，所有子节点自身的索引直接除以2就是父节点的索引，即i2和i×2+1各除以2后取整就是它们的父节点索引i。

最大最小堆的优先级队列的操作分为**插入元素**、返回最大或最小值、返回并**删除最大最小值**、查找并修改某个元素

##### 操作步骤

其基本思想是，利用完全二叉树的特性，**将新元素放入二叉树的叶子节点上**，然后比较它与父节点的大小，如果它比父节点大（最小堆的情况），则结束，否则就与父节点交换，继续比较，直到没有父节点或者父节点比它小为止。**删除根节点则反过来，把最后一个叶子节点放入根节点**，然后找到这个新根节点的实际位置，即比较它与两个子节点的大小，如果比它们小（最小堆的情况），则结束，否则取最小值（最小堆的情况）替换节点位置，然后再继续向下比较和替换，直到停止或者替换到叶子节点时再没有子节点可比较为止

#### 2.6.3 其他算法排序

##### 桶排序

桶排序是**将所有元素按一定的大小分成N个组，再对每个组进行快速排序**，最终得到有序的数组，并得到N个桶的记录，虽然第一次排序的速度不怎么样，但这N个桶记录下来的信息对后面的程序逻辑有非常大的帮助。

##### 基数排序

针对元素的特性来实施的“分配式排序”，利用数字的特性，按个位数、十位数、百位数的性质将元素放入0～9个桶中，不用排序，几次合并后就有了序数组，利用元素特性排序的速度比任何其他排序方式都要快。

### 2.7　各类搜索算法

#### 2.7.2　二分查找算法

**查找所用的数组必须是有序的数组**

##### 二分查找法步骤

1. 将数组分为三块，即**前半部分区域、中间位元素、后半部分区域。**
2. **将要查找的值与数组的中间位元素进行比较**，若小于中间位，则在前半部分区域查找，若大于中间位，则在后半部分区域查找，如果等于中间位，则直接返回。
3. 继续在选择的查找范围中查找，跳入步骤1），**依次进行递归操作**，将当前选择的范围继续拆分成前半部分、后半部分和中间位元素这三部分，直到范围缩到最小，如果还是没有找到匹配的元素，则说明元素并不在数组里。


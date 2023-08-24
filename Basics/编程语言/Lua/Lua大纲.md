# Lua大纲

## 注意点

- 循环用`do`，if语句用`then`，都使用`end`结尾
- Lua不支持自减和自加操作，只能`i=i+1`，`i=i-1`
- 字符串的第一个位置是1不是0

## 技巧

- Lua的字符串可以存储`0x00`，可以用字符串存储二进制流

## 注释

 用`--`开头，来写一段`单行注释` 

```lua
-- 单行注释
```

 用`--[[`开头，`]]`结尾，写一段多行注释。 

```lua
--[[
	多行
	注释
]]
```

## 变量

对一个变量进行赋值就相当于声明变量，与python相似

### **声明的变量默认为全局变量**

```lua
a=1
```

同时给多个变量赋值，多重赋值

```lua
a,b=1,2
```

### 局部变量

```lua
local b=2
```

### nil类型

**没有被声明过的变量都是nil**

**nil类型只有一个值，就是nil**

nil类似null类型  if(nil)为false

### number类型

```lua
--支持十六进制表示方法
a=0x11--输出为17
--科学计数法
a=2e10
```

### string类型

#### 单引号、双引号

```lua
a="daeqeq"
b='sdarrr'
--转义字符\n
d="sdada\nsdar"
```

#### 多行文本

多行文本无法使用转义字符

```lua
c=[[asdasfffrrqweqq]]
```

#### 字符串连接

字符串连接符号`..`

```lua
a="daeqeq"
b='sdarrr'
c=a..b
print("asdaf".."as232")
```

#### 数值转字符串

```lua
tostring(10)
```

#### 字符串转数值

`tonumber`转换失败为nil

```lua
tonumber("193")
```

#### 获取字符串长度

```lua
a="daeqeq"
print(#a)
--等价于s:len()
```

#### string类型类似char数组

Lua中string类型 类似于C里的字符数组，可以包含任意数值包括0x00，可以存储二进制流因为都是原原本本存储

##### 将ascii码转为字符串

```lua
s=string.char(65)
s=string.char(0x30,0x31,0x32,0x33)
```

##### 取出string中的某一位的ascii码

```lua
n=string.byte(s,2)
print(n)
--语法糖，第一个到最后一个
s:byte(1，-1)
```

### format

调用c的接口

```lua
local f=string.format("%d,%d",1,2)
print(f)
```



## function函数

函数默认返回值为nil，没有任何返回值则为nil

```lua
function function_name()
	--body
end
--函数名放前面
f=function(...)
    --body
end
```

```lua
function f(a,b,c)
    print(a,b,c)
    return a
end
--c没有传值则为nil
print(f(1,2))--1
```

### 函数可以返回多个值

```lua
function f(a,b,c)
    return a,b
end

print(f(1,2))
```

#### 多个返回值可以使用多重赋值语句

```lua
function f(a,b,c)
    return a,b
end

local i,j=f(1,2)
```

## table

### table接口

```lua
table.insert(a,"d")--插入元素，末尾添加
table.insert(a,2,"d")--插入元素，第二个位置，后续元素后移
local s= table.remove(a,2)--移除第2个元素
```

### table数字下标

可以存所有东西(number,string,table,function)

```lua
a={1,"ac",{},function() end}
a[5]=123 --可以直接添加一个元素
print(a[1])
--获取table长度
print(#a)
```

### table字符串下标

以字符为table的下标

```lua
a={
    a=1,
    b="1234",
    c=function()
        end,
    d=123123
}
print(a["a"])
--下标符合变量命名规范时
print(a.a)
```

### 全局表_G

**Lua内的所有全局变量都在_G这个table内**

包括`table.insert`中的table(也是全局变量)也存储在_G中，insert为table的下标

```lua
print(_G["table"]["insert"])
--function: 0000000063be4590
--输出值为一个函数
--多文件调用知识
```

## 布尔型

### 不等于

**在Lua中不等于使用`~=`表示**

```lua
a=true
b=false
print(1>2)
print(1<2)
print(1>=2)
print(1<=2)
print(1==2)
--不等于
print(1~=2)
```

### 与或非

###  只有false和nil表示假

其他都表示真，包括0

```lua
print(a and b)
print(a or b)
print(not a)
```

**`and``or`返回的并不完全是true和false，会直接返回a或者b的值，可以通过短路求值**

只有`not`返回true和false

```lua
a=nil --真
b=0	  --假
print(a and b) --nil
print(a or b)  --0
print(not a)   --true
print(b>10 and "yes" or "no") --no
```

## 分支判断

### if语句

```lua
if 1>10 then
	print("1>10")
elseif 1<10 then
    print("1<10")
else
    print("no")
end
```

## 循环

### for循环

```lua
for i=1,10 do
	print(i)
end
--步长为2
for i=1,10,2 do
	print(i)
end
```

#### 倒序循环

```lua
for i=10,1,-1 do
	print(i)
end
```

`i`在过程途中不能赋值，赋值了也没用

假如对`i`赋值会被认定为新建了局部变量

```lua
for i=10,1,-1 do
	print(i)
	local i=1
end
```

#### break

通过break退出循环

```lua
for i=10,1,-1 do
	print(i)
	if i == 5 then break end
end
```

### while循环

```lua
n = 10
while n>1 do
    print(n)
    n = n - 1
end
```



### repeat循环

和while循环基本一致



## 多文件调用

### require和import的区别

#### require实现

换个目录后就需要对路径进行变更

个人理解：绝对路径

```lua
local MyClassBase = require("app.classes.MyClassBase")
local MyClass = class("MyClass", MyClassBase)

local Data1 = require("app.classes.data.Data1")
local Data2 = require("app.classes.data.Data2")
```

#### import实现

在模块名前添加多个"." ，这样 import() 会从更上层的目录开始查找模块。

个人理解：相对路径

```lua
local MyClassBase = import(".MyClassBase")
local MyClass = class("MyClass", MyClassBase)

local Data1 = import(".data.Data1")
local Data2 = import(".data.Data2")
```



### require

运行指定多文件

末尾不带扩展名

```lua
--.\?lua   把文件名匹配到？内
require("文件名")
```

不同层架文件夹用`.`分隔

```lua
require("文件夹名.文件名")
```

只会运行一次

```lua
--lua文件有可以return返回值
local r = require('hello')
--后面调用的都是前面返回的内容
require('hello')
require('hello')
require('hello')
require('hello')
print(r)
```

### package.path

```lua
package.path=package.path..";./path/?.lua"
require('hello2')
```

### 多次调用

调用lua文件内的函数

```lua 
--hello.lua
local hello={}
function hello.say()
    print("hello world")
end
//通过返回table
return hello

--test.lua
local test=require('hello')
//通过table内的方法调用
test.say()
```

## 迭代table

```lua
t={"a","b","c","d"}
for i=1,#t,do
    print(i,t[i])
end
```

### 迭代器ipairs

纯数字连续下标可以用

```lua
t={"a","b","c","d"}
--下标给i，值给j
for i,j in ipairs(t) do
    print(i,j)
end
```

```lua
t={
	[1]="a",
	[2]="b",
	[3]="c",
	[5]="d"
}
--下标给i，值给j
for i,j in ipairs(t) do
	--只能遍历到1到3，不连续的后面遍历不到
    print(i,j)
end
```

### 迭代器pairs

可以遍历所有下标

pairs内部调用的是`next`函数

```lua
t={
	[1]="a",
	[2]="b",
	[3]="c",
	[5]="d"
}
--下标给i，值给j
for i,j in pairs(t) do
	--只能遍历到1到3，不连续的后面遍历不到
    print(i,j)
end
```

```lua
t={
	apple="a",
	banana="b",
	eraser="c",
	water="d"
}
--下标给i，值给j
for i,j in pairs(t) do
	--只能遍历到1到3，不连续的后面遍历不到
    print(i,j)
end
```

## 元表、元方法



## 语法糖

```lua
t={
	a=0,
	add=function(tab,num)
		tab.a=tab.a+num
	end
}
--类似于面向对象的方法调用
t:add(10)--等价于t.add(t,10)
```

### 面向对象

```lua
--对象名
bag={}

bagmt={
    --装入东西的函数
    put=function(t,item)
        	table.insert(t.items,item)
        end,
    take=function(t)
        	return table.remove(t)
    	 end,
    list=function(t)
        	return table.concat(t.items,",")
         end
}
bagmt["__index"]=bagmt
--构造函数
function bag.new()
    local t={
        items={}
    }
    setmetatable(t,bagmt)
    return t
end
```

## 协程coroutine

一个lua虚拟机里只能有一个线程

### coroutine.create 可创建一个协程

返回值为 `thread `类型

```lua
local co=coroutine.create(
    function()
        print("hello world!")
    end
    )

```


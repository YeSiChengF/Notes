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
```

#### string类型类似char数组

Lua中string类型 类似于C里的字符数组，可以包含任意数值

##### 将ascii码转为字符串

```lua
s=string.char(65)
s=string.char(0x30,0x31,0x32,0x33)
```

##### 取出string中的某一位的ascii码

```lua
n=string.byte(s,2)
print(n)
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

**`and``or``not`返回的并不完全是true和false，会直接返回a或者b的值**

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
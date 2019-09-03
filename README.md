# SuperLiner

SuperLiner is a simple and rough script interpreter like the glue.

# Sample Script 1

```
func test
print somethinghere
endfunc

func setter
set 123450 > aGlobalNumber
endfunc

call setter
call test
call tail
print &aGlobalNumber
print compeleted.

func tail
print `tail is here.`
endfunc

```

# Sample Script 2

```
func test
	print somethinghere
endfunc

func setter
	set 123450 > aGlobalNumber
	Date > currentTime
	HttpGet http://t.weather.sojson.com/api/weather/city/101030100 > testGet
	Print &testGet
endfunc


setslaver 10.1.53.252 9000 12345678
setslaver 10.1.83.73 9000 12345678
remotecall setter 10.1.53.252
call setter
call test
call tail
output on .\test.log
log lolp
pause `Type any key to continue.`

--version1--
if ab lt cd `times 10 test`
#cmd cmd `mkdir C:\kokololo` @10.1.83.73
print hahahahaha @.,10.1.83.73
loadjson `[1,2,3]`
changexml .\test.xml `/xml/item/@src` &currentTime
sethttpHeader ll kk
download https://ww4.sinaimg.cn/bmiddle/62d90090ly1g5thpgmej4j20m80gon0r.jpg .\biubiu.png

#version 2 is going
--version2--
print &aGlobalNumber @10.1.53.252
AppendScript `C:\Users\Administrator\Desktop\sh2.txt`
print compeleted.



func tail
	print `tailhere aaa aaa.`
endfunc



```

# How to add new Action

A sample named SampleExtension has been added into project.
This project is built for win10-x64, it should be packed as nuget package if you want to run it in docker.

# Remote Execute Result

Usually, We use `Set something > param` to put `something` into `param` as `Set` action return its paramater.
But a lucent `set` action will be done after executing a remote line.
After doing `Set something @10.10.10.10`, `something` will be returned. But here's no variable defined to receive it, so it will be returned to `Context`.
And because of the lucent `set` action, the line result will be saved into a temporary variable.
You can use `LastRemoteResult` or `LastRemoteString` action to get the last result, one IP one temporary variable.
For example:
```
Set something @10.10.10.10
set other @10.10.1.10
LastRemoteResult 10.10.10.10 > h1
LastRemoteString 10.10.1.10 > h2
print &h1
print &h2
```
result is:

```
something
other
```


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
FUNC 0                                   
PRINT 123							  
ENDFUNC 0                              

FUNC hello
COPYFILE A B
CALL [0]
ENDFUNC hello

FUNC 2
COPYFILE B A
ENDFUNC 2

{mark0}
MOVEFILE A B                         
CREATEFILE A abcd,ddfdgads            
COPYFILE A B
{mark1}
APPENDFILE A nnn
CHANGEXML A \root\body\div[0] hello
CHANGEYAML B \aaa\bbb\ccc\d op
CHECKXML A \root\body\div[0] hello [1] [2]
TIMES 20 [hello]



DELETE A
NETSEND 127.0.0.1:443 hello > back1
APPENDFILE A &back1
SET back2 12345
PRINT &back2
{mark2}
SETCONTEXT request-header Content-Type json
SETCONTEXT request-header Method GET
DOWNLOAD URL A
RESETCONTEXT



```

# How to add new Action

A sample named SampleExtension has been added into project.
This project is built for win10-x64, it should be packed as nuget package if you want to run it in docker.



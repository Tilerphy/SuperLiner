# SuperLiner

SuperLiner is a simple and rough script interpreter like the glue.

# Sample Script

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

# How to add new Action

A sample named SampleExtension has been added into project.


# SuperLiner

SuperLiner is a simple and rough script interpreter like the glue.

# Sample Script

```
func test
print somethinghere
endfunc

func setter
set 123450 => aGlobalNumber
endfunc

call setter
call test
call tail
print &aGlobalNumber
print compeleted.

func tail
print tailhere.
endfunc

```

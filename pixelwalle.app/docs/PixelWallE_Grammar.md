# PixelWallE Language Grammar

## Literals

- **String**: `"text"` (color names, etc.)
- **Number**: `123`
- **Boolean**: `true`, `false`

## Expressions

### Unary Expressions

- Logical NOT: `!expression`
- Numeric Negation: `-expression`

### Binary Expressions

- **Arithmetic**: `+`, `-`, `*`, `**`, `%`, `/`
- **Logic**: `<`, `<=`, `>`, `>=`, `==`, `!=`, `&&`, `||`

### Grouping

- Parentheses: `( expression )`

## Commands

Commands are special statements that perform actions on the canvas or robot.  
Each command starts with a keyword and may take arguments (expressions).  
**Syntactically, commands are similar to function calls, but are used as statements.**

**Syntax:**
```
command_statement ::= command_name "[" argument_list? "]"
command_name      ::= "DrawCircle" | "DrawLine" | "DrawRectangle" | "Fill" | "GoTo" | "Size" | "Spawn" | "Color"
argument_list     ::= expression ("," expression)*
```

**Examples:**
```plaintext
DrawCircle(1, 2, 5)
Fill()
GoTo(true, "label1")
Size(10)
Spawn(0, 0)
Color("red")
```

## Functions

Functions return values and can be used as literals in expressions.  
**A function call is a kind of literal, not a unary expression.**

**Syntax:**
```
literal         ::= number
                 | string
                 | bool
                 | function_call
                 | "List" "<" baseType ">"

baseType        ::= number
                 | string
                 | bool


declaration     ::= Identifier "=" listInit 

listInit        ::="List""<" type ">" "[" expressionList? "]" 

type            ::="int"
                 | "bool"
                 | "string"

expressionList  ::= expression ("," expression)*

expression      ::= identifier "[" expression "]"
statement       ::= identifier "[" expression "]" "<-" expression

expression      ::= identifier "." methodCall
methodCall      ::= "Add" "(" expression ")" 
                 |  "RemoveAt" "(" expression ")"
                 |  "Clear" "(" ")"
                 |  "Lenght




function_call   ::= function_name "(" argument_list? ")"
function_name   ::= "GetActualX" | "GetActualY" | "GetCanvasSize" | "GetColorCount" 
                 | "IsBrushColor" | "IsBrushSize" | "IsCanvasColor" | "IsColor"
argument_list   ::= expression ("," expression)*
```

**Examples:**
```plaintext
GetActualX()
IsBrushColor("red")
GetColorCount("red", 0, 0, 5, 5)
```

## Grammar Rules 

```
expression      ::= literal
                 | unary
                 | binary
                 | grouping

literal         ::= number
                 | string
                 | bool
                 | function_call

grouping        ::= "(" expression ")"

unary           ::= ("-" | "!") expression

function_call   ::= function_name "(" argument_list? ")"
function_name   ::= "GetActualX" | "GetActualY" | "GetCanvasSize" | "GetColorCount" 
                 | "IsBrushColor" | "IsBrushSize" | "IsCanvasColor" | "IsColor"
argument_list   ::= expression ("," expression)*

binary          ::= expression operator expression

operator        ::= "+" | "-" | "*" | "**" | "%" | "/" 
                 | "<" | "<=" | ">" | ">=" | "==" | "!=" | "&&" | "||"
```

## Example

```plaintext
(5 + 3) * -2
!"red"
true && false
```


## List

```



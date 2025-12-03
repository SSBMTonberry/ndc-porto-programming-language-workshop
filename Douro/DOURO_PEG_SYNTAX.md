# Douro PEG Grammar Syntax Reference

This document explains all the syntax and constructs used in the `douro.peg` file, which defines the grammar for the Douro programming language using the Pegasus PEG parser generator.

## Table of Contents
- [PEG Basics](#peg-basics)
- [Pegasus Directives](#pegasus-directives)
- [Grammar Rules](#grammar-rules)
- [PEG Operators](#peg-operators)
- [Character Classes](#character-classes)
- [Semantic Actions](#semantic-actions)
- [Complete Rule Reference](#complete-rule-reference)

---

## PEG Basics

**PEG** stands for **Parsing Expression Grammar**. It's a way to define the syntax of a language using:
- **Ordered choice**: Try alternatives in order, use the first one that matches
- **Greedy matching**: Consumes as much input as possible
- **No ambiguity**: Unlike CFG (Context-Free Grammars), PEG always has exactly one parse tree

---

## Pegasus Directives

These special directives configure the parser generator:

### `@namespace Douro`
Sets the C# namespace for the generated parser class.

### `@using Douro.Values`
### `@using Douro.Statements`
Adds C# using directives to the generated parser, allowing access to classes from these namespaces.

### `@classname DouroParser`
Specifies the name of the generated parser class.

---

## Grammar Rules

### Rule Definition Syntax

```peg
ruleName<ReturnType>
    = pattern { action }
    / alternative_pattern { alternative_action }
```

- **`ruleName`**: Identifier for the rule
- **`<ReturnType>`**: Optional C# type that the rule returns
- **`=`**: Separates rule name from pattern
- **`/`**: Ordered choice operator (try alternatives in order)
- **`{ action }`**: C# code to execute when pattern matches

### Rule Modifiers

#### `-memoize`
```peg
sum <Expr> -memoize
```
Caches parsing results to improve performance for recursive rules (prevents exponential time complexity).

---

## PEG Operators

### Sequence (Space)
```peg
"function" _ name:identifier
```
Matches patterns in sequence (one after another). Whitespace between patterns means "followed by".

### Ordered Choice (`/`)
```peg
statement<Statement>
    = function
    / print
    / assign
    / return
```
Tries alternatives **in order**. Returns the first one that succeeds. Does NOT backtrack if a later alternative would be better.

### Zero or More (`*`)
```peg
ws = [ \t\n\r]*
```
Matches the pattern zero or more times (greedy).

### One or More (`+`)
```peg
_ = [ \t]+
```
Matches the pattern one or more times (greedy).

### Optional (`?`)
```peg
args:identifier_list?
```
Matches the pattern zero or one time.

### Labeled Capture (`name:pattern`)
```peg
stmt:statement
```
Captures the result of `pattern` and binds it to the variable `name` for use in semantic actions.

### Negation (`!`)
```peg
EOF = !.
```
Succeeds if the pattern does NOT match (lookahead assertion). Does not consume input.

### Any Character (`.`)
```peg
unexpected:.
```
Matches any single character.

---

## Character Classes

### Range Syntax
```peg
[a-zA-Z_]      // Matches: lowercase, uppercase, or underscore
[a-zA-Z0-9_]   // Matches: alphanumeric or underscore
[0-9]          // Matches: digits 0-9
[ \t]          // Matches: space or tab
[\n\r]         // Matches: newline characters
[ \t\n\r]      // Matches: any whitespace
```

### String Capture (`""`)
```peg
identifier = ("" [a-zA-Z_][a-zA-Z0-9_]*)
```
The `""` prefix tells Pegasus to capture the matched text as a string.

---

## Semantic Actions

Semantic actions are C# code blocks `{ }` that execute when a pattern matches. They construct the AST (Abstract Syntax Tree).

### Simple Construction
```peg
print<Statement> =
    "print" _ expr:expression {
        new Print(expr)
    }
```

### List Construction
```peg
arg_list<List<Expr>>
    = head:expression __ ',' __ tail:arg_list {
        (new List<Expr> { head }).Concat(tail).ToList()
    }
    / arg:expression { new List<Expr> { arg } }
    / "" { new List<Expr>() }
```
Builds a list recursively:
1. First alternative: multiple items separated by commas
2. Second alternative: single item
3. Third alternative: empty list

### Null Coalescing
```peg
args?.ToList() ?? new List<string>()
```
Handles optional captures that might be null.

### Error Handling
```peg
EOF = !.
    / unexpected:.
        #error{ $"Unexpected '{unexpected}' at line {state.Line}, col {state.Column - 1}" }
```
The `#error` directive generates a parse error with a custom message.

---

## Complete Rule Reference

### Program Structure

#### `program<DouroProgram>`
The top-level rule that matches the entire program.
- Recursively matches statements separated by whitespace
- Returns a `DouroProgram` containing all statements

```peg
program<DouroProgram>
    = ws stmt:statement ws rest:program {
        rest.Insert(stmt)
    }
    / ws { new DouroProgram() }
```

### Statements

#### `statement<Statement>`
Matches any valid statement type.
```peg
statement<Statement>
    = function      // Function definition
    / print         // Print statement
    / assign        // Variable assignment
    / return        // Return statement
```

#### `function <Statement>`
Matches a function definition.
```peg
"function" _ name:identifier __ '(' __ args:identifier_list __ ')' __ '{' __ EOL+
    __ body:statement_list EOL*
    __ '}'
```
Creates an `Assign` statement that binds a `Function` object to a name.

#### `assign<Statement>`
Matches a variable assignment.
```peg
"let" _ name:identifier __ "=" __ expr:expression
```

#### `print<Statement>`
Matches a print statement.
```peg
"print" _ expr:expression
```

#### `return<Statement>`
Matches a return statement.
```peg
"return" _ expr:expression
```

#### `statement_list <List<Statement>>`
Matches a list of statements (used in function bodies).
- Statements separated by newlines (`EOS+`)
- Can be a single statement or multiple statements

### Expressions

#### `expression <Expr>`
Entry point for expressions. Delegates to `sum`.

#### `sum <Expr>` (marked `-memoize`)
Handles addition and subtraction with left-to-right associativity.
```peg
sum <Expr> -memoize
    = lhs:product __ "+" __ rhs:sum     // Addition
    / lhs:product __ "-" __ rhs:sum     // Subtraction
    / product                            // Fall through to multiplication/division
```

#### `product <Expr>` (marked `-memoize`)
Handles multiplication and division with higher precedence than sum.
```peg
product <Expr> -memoize
    = lhs:product __ "/" __ rhs:primary  // Division
    / lhs:product __ "*" __ rhs:primary  // Multiplication
    / primary                             // Fall through to primary expressions
```

#### `primary`
Base expressions with highest precedence.
```peg
primary
    = number         // Numeric literal
    / print_expr     // Print as expression
    / function_call  // Function invocation
    / lookup         // Variable lookup
```

**Order matters!** `print_expr` must come before `lookup` because both start with an identifier, and `print` is a keyword.

#### `print_expr <Expr>`
Allows `print` to be used in expression context (prints and returns value).
```peg
"print" _ expr:expression { new PrintExpr(expr) }
```

#### `function_call <Expr>`
Matches a function call with arguments.
```peg
name:identifier __ '(' __ args:arg_list __ ')' { new FunctionCall(name, args) }
```

#### `lookup <Expr>`
Matches a variable reference.
```peg
name:identifier { new Lookup(name) }
```

#### `number <Expr>`
Matches a numeric literal (one or more digits).
```peg
digit:("" [0-9]+) { new Number(digit) }
```

### Lists

#### `arg_list<List<Expr>>`
Matches comma-separated expression arguments.
```peg
arg_list<List<Expr>>
    = head:expression __ ',' __ tail:arg_list  // Multiple args
    / arg:expression                            // Single arg
    / ""                                        // Empty (no args)
```

#### `identifier_list<List<string>>`
Matches comma-separated parameter names.
```peg
identifier_list<List<string>>
    = head:identifier __ ',' __ tail:identifier_list  // Multiple params
    / name:identifier                                  // Single param
    / ""                                               // Empty (no params)
```

### Lexical Elements

#### `identifier`
Matches a valid identifier (variable or function name).
```peg
identifier = ("" [a-zA-Z_][a-zA-Z0-9_]*)
```
- Must start with letter or underscore
- Followed by any number of letters, digits, or underscores
- The `""` captures it as a string

### Whitespace

#### `_` (required whitespace)
One or more spaces or tabs.
```peg
_ = [ \t]+
```

#### `__` (optional whitespace)
Zero or more spaces or tabs.
```peg
__ = [ \t]*
```

#### `ws` (any whitespace)
Zero or more whitespace characters including newlines.
```peg
ws = [ \t\n\r]*
```

#### `EOL` (end of line)
One or more newline characters.
```peg
EOL = [\n\r]+
```

#### `EOS` (end of statement)
Alias for `EOL`.
```peg
EOS = EOL
```

### Special Rules

#### `EOF`
Matches end of file or reports an error.
```peg
EOF = !.                              // No more characters
    / unexpected:.                     // Or any character (error)
        #error{ $"Unexpected '{unexpected}' at line {state.Line}, col {state.Column - 1}" }
```

---

## Operator Precedence

From **highest** to **lowest** precedence:

1. **Primary expressions**: numbers, identifiers, function calls, parentheses
2. **Multiplication/Division**: `*`, `/` (left-associative)
3. **Addition/Subtraction**: `+`, `-` (left-associative)

Example:
```
2 + 3 * 4  →  2 + (3 * 4)  →  2 + 12  →  14
```

---

## Example Parse

Given the input:
```douro
function foo(x,y) {
print x
return x + y
}
```

**Parse flow:**
1. `program` matches `ws` (any leading whitespace)
2. `program` matches `statement` → `function`
3. `function` matches literal `"function"`
4. `function` matches required space `_`
5. `function` matches `identifier` → captures `"foo"`
6. `function` matches `(` and `identifier_list` → captures `["x", "y"]`
7. `function` matches `{` and newline
8. `function` matches `statement_list`:
   - Matches `print` statement with `lookup` → `print x`
   - Matches `EOS+` (newline)
   - Matches `return` statement with `sum` expression → `return x + y`
9. `function` matches `}` and creates `new Assign("foo", new Function(["x", "y"], [print_stmt, return_stmt]))`

---

## Common Patterns

### Left-Recursive Lists
PEG doesn't support left recursion directly, so we use right recursion:
```peg
arg_list<List<Expr>>
    = head:expression __ ',' __ tail:arg_list
        { (new List<Expr> { head }).Concat(tail).ToList() }
    / arg:expression { new List<Expr> { arg } }
```

### Keyword vs Identifier
Keywords (like `print`) must come **before** the general `identifier` rule:
```peg
primary
    = number
    / print_expr    // Must come before lookup!
    / function_call
    / lookup        // Would match "print" as identifier
```

### Optional with Default
```peg
args:identifier_list?  // Might be null
{ new Function(args?.ToList() ?? new List<string>(), body) }
```

---

## Tips for Reading PEG Grammars

1. **Read alternatives top-to-bottom**: The first match wins
2. **`*` and `+` are greedy**: They consume as much as possible
3. **Labeled captures** (`:`) bind values for use in actions
4. **Semantic actions** `{ }` build the AST
5. **Whitespace handling** is explicit (must be in the grammar)

---

## References

- [Pegasus Parser Generator](https://github.com/otac0n/Pegasus)
- [PEG Wikipedia](https://en.wikipedia.org/wiki/Parsing_expression_grammar)
- Original PEG paper: Bryan Ford, "Parsing Expression Grammars: A Recognition-Based Syntactic Foundation" (2004)

---

*This grammar defines the Douro programming language, a simple functional language with functions, variables, arithmetic, and print statements.*


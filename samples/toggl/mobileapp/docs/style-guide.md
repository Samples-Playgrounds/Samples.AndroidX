# Toggl Mobile's code formatting guidelines

All types use PascalCase
```csharp
// Do
internal class Foo : IFoo
{
    private struct Baz
    {
    }
}

// Don't
internal class foo : ifoo
{
    private struct baz
    {
    }
}
```

All public and protected members use PascalCase
```csharp
// Do
public void Foo()
{
}
protected int Baz => 0;

// Don't
public void foo()
{
}
protected int baz => 0;
```

All private members use camelCase
```csharp
// Do
private void foo()
{
}
private int baz => 0;
private const int bar = 42;

// Don't
private void Foo()
{
}
private int Baz => 0;
private const int Bar = 42;
```

Indentation is done with four spaces
```csharp
// Do
public int Foo()
{
    return 0;
}

// Don't (use tabs)
public int Foo()
{
	return 0;
}

// Don't (use other than 4 spaces)
public int Foo()
{
  return 0;
}
```

Always specify access modifiers
```csharp
// Do
internal class Foo
{
    private int baz;

    private class Bar
    {
    }
}

// Don't
class Foo
{
    int baz;

    class Bar
    {
    }
}
```

Classes should be sealed by default, unless they are specifically meant to be inherited from.
```csharp
// Do
internal sealed class Foo
{
}

// Don't
internal class Foo
{
}
```

Only use fields privately
```csharp
// Do
private int foo;
private string bar;

// Don't
protected int Foo;
public string Bar;
```

Use auto properties where possible
```csharp
// Do
public int Foo { get; } = 0;
public string Bar { get; private set; }

// Don't
private readonly int foo = 0;
public int Foo => foo;

private string bar;
public string Bar => bar;
```

Open braces on a new line
```csharp
// Do
internal class Foo
{
    public void Baz()
    {
    }
}

// Don't
internal class Foo {
    public void Baz() {
    }
}
```

Prefer `var` over explicit types, unless this causes confusion
```csharp
// Do
var foos = new List<Foo>();
var baz = bar.Baz;

// Don't
List<Foo> foos = new List<Foo>();
Baz baz = bar.Baz;
```

Use `this` only when necessary
```csharp
// Do
private int foo;
public int Foo => foo * 2;
public void Baz(int newFoo)
    => foo = newFoo;

// Don't
private int foo;
public int Foo => this.foo * 2;
public void Baz(int newFoo)
    => this.foo = newFoo;
```

Prefer early returning over nesting
```csharp
// Do
public void Foo(int bar)
{
    if (bar < 0) return;

    var baz = Baz(bar)

    if (baz < 0) return;

    // ..
}

// Don't
public void Foo(int bar)
{
    if (bar >= 0)
    {
        var baz = Baz(bar)

        if (baz >= 0)
        {
            // ..
        }
    }
}
```

Return early in single line, if the condition is short and there is no return value.
```csharp
// Do
if (condition) return;

if (condition && anotherOne || maybeThis || thatOtherThing)
    return;

if (condition)
    return foo;

// Don't
if (condition)
    return;

if (condition && anotherOne || maybeThis || thatOtherThing) return;

if (condition) return foo;
```

Prefer expression bodies for single statement members
```csharp
// Do
private int foo;
public int Foo => foo * 2;

public void Baz(int newFoo)
    => foo = newFoo;

// Don't
private int foo;
public int Foo
{
    get
    {
        return foo * 2;
    }
}

public void Baz(int newFoo)
{
    foo = newFoo;
}
```

Private methods that do not use instance members should be marked as `static`.
```csharp
public sealed class Foo
{
    private int number;
    
    public void Bar(int offset)
    {
    	int rating = twice(number * offset);
	Console.WriteLine(rating);
    }
    
    private static int twice(int value)
    {
    	return value * 2;
    }
}
```

Do not add spaces around parentheses.
```csharp
// Do
public void Foo()
{
    bar(baz());
}

// Don't
public void Foo ( )
{
    bar ( baz ( ) ) ;
}
```

Add spaces after, but not before commas.
```csharp
// Do
foo(bar, baz);

// Don't
foo(bar , baz);
foo(bar ,baz);
```

Add spaces around all operators (except `.` and `?.`, never add spaces here).
```csharp
// Do
(foo + bar) * baz
foo ? bar : baz
foo && bar || baz
foo.Bar?.Baz ?? 0

// Don't
(foo+bar)* baz
foo ? bar:baz
foo&&bar || baz
foo. Bar ?.Baz?? 0
```

When splitting statements into multiple lines, do so by starting new lines with operators, and using an additional level of indentation.
```csharp
// Do
myLongExpression
    .Foo()
    .Bar
    + 5

myLongCondition
    ? foo
    : bar

// Don't
myLongExpression.
    Foo().
    Bar +
    5

myLongCondition ?
    foo :
    bar

```

Do not use unnecessary parentheses or braces for lambda expressions.
```csharp
// Do
Action foo = Foo;

Action<int> bar = Bar;
Action<int> bar = x => Bar(x * 2);

Func<int, int> baz = x => x * 2;

// Don't
Action foo = () => { Foo(); };

Action<int> bar = (x) => { Bar(x); };
Action<int> bar = (x) => { Bar(x * 2); };

Func<int, int> baz = (x) => { return x * 2; };
```

Append platform suffixes to platform specific implementations of services.
```csharp
//In Foundation
public interface IFooService
{
    void Bar();
}

//In iOS
public class FooServiceIos : IFooService
{
    public void Bar() { }
}

//In Toggl.Droid
public class FooServiceAndroid : IFooService
{
    public void Bar() { }
}

```

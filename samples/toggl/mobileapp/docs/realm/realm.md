# :sparkles: The Realm of IQueryables
  
## :electric_plug: Interfaces
  
When dealing with collections, three of the most useful .NET interfaces are `IEnumerable<T>`, `IObservable<T>` and `IQueryable<T>`.  If a descriptive term was to be assigned to each of those based on their mechanics, those would be: **Pull**, **Push** and **Transform**, respectively. 

With `IEnumerable<T>`, there is a source collection (existing or generated on the fly) that we ask the items from (hence the **PULL**).
  
With `IObservable<T>` the collection is implicit in the emission of the items and we can't ask for items ourselves, we can only have those items pushed to us whenever they are ready (hence the **PUSH**).
  
In comparison with the two, `IQueryable<T>` is completely different. The exact behavior and the data returned depend on the implementation of the *query providers*. Usually, the code is written in such a way that we describe what data we need, but that code is not really executed but instead transformed into some other code that will be executed (hence the **TRANSFORM**). The core mechanic behind these transformations are Expression Trees.

### :link: LINQ

For all three interfaces, a set of extension methods exist that utilizes the mechanics behind them. Many of the operators are similar across the three and usually they mean what they say. The most typical of them are `Where()`, `Select()`, `SelectMany()`, `Count()`, etc. 

### :curly_loop: Expression Trees

In short, expression trees are **_code as data_**. They are typically used in `IQueryable` extension methods, but they are not limited to that.

Take a look at a simple example:

```C#
IEnumerable<Item> enumerableCollection = getEnumerableItems();
var validEnumeratedItems = enumerableCollection
    .Where(item => item.Text.StartsWith("Data"))
    .ToList();

IQueryable<Item> queryableCollection = getQueryableItems();
var validQueriedItems = queryableCollection
    .Where(item => item.Text.StartsWith("Data"))
    .ToList();
```

In the case of `IEnumerable`, the getter of the `Text` property of every object in the collection will be accessed. However, in the case of `IQueryable` we actually have no idea what can happen. That depends on the _query provider_ used.

If we look at the `Where` extension methods, we can see the following signatures:

```C#
IEnumerable<T> Where<T>(this IEnumerable<T> collection, Func<T, bool> predicate);
IQueryable<T> Where<T>(this IQueryable<T> collection, Expression<Func<T, bool>> predicate);
```

So instead of the `Func<T, bool>` we have `Expression<Func<T, bool>>`. That means that our `item => item.Text.StartsWith("Data")` lambda is actually data, and not the code to be executed as is.

The query provider will actually get this code as an expression tree and it can use it in any way it sees fit. For example, query providers such as Object-to-SQL query providers (e.g. Entity Framework or similar), could transformed this into a WHERE clause like this `WHERE lower([obj].Text) LIKE 'data%'`. 

> :warning: Expression trees are also great for generating dynamic behaviors as they can be compiled into executable code, even with some transformations applied if needed. This dynamic compilation, however, depends deeply on things from the `Reflection.Emit` namespace that uses JIT compilation, so it is not available within non-jitted environments and we are not able to use it on iOS. 

The benefits for the programmer:

* Able to express the intention of what data is desired, without specifying how to get it
* Able to use autocomplete feature (e.g. intellisense) within IDE
* The query will remain type safe
* Queries that are compelx under the hood can be simplified to simple lambdas


### :small_red_triangle_down: The problem

When we write a query in terms of `IEnumerable`, we can expect the same output, regardless of the underlying data source.

```C#
var data = source
    .Where(item => item.IsValid)
    .Select(item => item.Name)
    .GroupBy(name => name[0])
    .Select(group => new { Letter: g.Key, Count: g.Count() })
    .ToList();
```
The problem with `IQueryable` though is that for the same query, a query provider can't just be switched around with the expectation of the same behavior. And this is because the query provider can support, or not support, certain operators.

And that leads us to Realm Database Engine.

## :package: Realm

To make things clear from the start, Realm is a **pure object database**. No relations, tables, attributes, foreign keys, normal forms and all other typical RDB fuss. 

None of that! 

Just the objects!

Under the hood, there is a very optimized C++ core engine and different platforms simply bind to that engine. In part, this is a source of Realm's speed. 

The other part of its speed lies in the fact that its memory model and the file format model are the same. This memory-mapping of the files allows for less searching thru files and loading less data into memory. :horse_racing: Speed!

It is **ACID**-compliant and uses [MVCC for concurrency control](https://en.wikipedia.org/wiki/Multiversion_concurrency_control).

### :blue_book: Data Model

POCO all the way (almost). 

:warning: A few important considerations about the data model, though:

* Classes that need to be persisted **must** inherit from `RealmObject`
  * This sucks in general case because it prevents us from creating our own inheritance hierarchies, but since we are trying to avoid needless inheritance chains, we don't really care about this.
* Only classes that inherit from `RealmObject` will be persisted. Classes that inherit from classes that inherit from `RealmObject` are ignored and will not work with Realm.
  * Well, we don't care much about this either.
* Every class that needs to be persisted **must** have a public, parameterless constructor (implicit, or explicit).
* Since Realm will weave through our properties, having custom getters/setters requires a different solution:
  * Creating a surrogate private auto-implemented property
  * Creating a public property with custom accessors which use the private property.
  * Realm will weave the auto-implemented property even if private and leave the properties with custom accessors alone.
  * Not the same in terms of intention, but we have used this mechanic with `RealmUser.BeginningOfWeek`. This property is `[Ignored]` because Realm does not support C# enums. However, this property is a wrapper around the `int` property, which will be persisted by Realm. _Note that even though our code marks the property with `[Ignored]`, this is not needed because Realm will (by default) ignore all properties that are not auto-implemented._


### :bird: Fody

One of the key benefits is that Realm does all the to-and-from database (de)serialization by hijacking our getters and setters and instead providing it's own behavior for persisting our data. However, in order to keep things performant (and also to support even non-jitted environments), none of this is being done dynamically at runtime. Instead, Realm has its own Fody weaver that will transform the accessors of our properties to make sure things are persisted as needed.

### :question: IQueryable all over again

So, why are `IQueryable`s important for Realm?

Simply because with them, things can happen faster or even not happen at all.

Take a look at these two snippets!

```C#
var elders1 = people.Where(p => p.Age > 65).ToList();
var elders2 = people.Where(p => 65 < p.Age).ToList();
```

Those are enumerables so the result is the same.
However, if we try to do the same with Realm, things will go :boom:.

```C#
var elders1 = realmInstance.All<Person>().Where(p => p.Age > 65).ToList();
var elders2 = realmInstance.All<Person>().Where(p => 65 < p.Age).ToList();
```

The second query will throw: `System.NotSupportedException: 'The left-hand side of the LessThan operator must be a direct access to a persisted property in Realm.'`

As we can see, Realm decides on its own what kind of expression it will allow. So, for Realm, the comparison is valid only if the property on the left of the operator is a property access, such as `p.Age`.

:heartpulse: Now, the most important thing about `IQueryables` in this context is the fact that Realm will see what we want by looking at the query and then transform this into its own operations under the hood to provide as fast a solution as possible. That means that 

```C#
var eldersFast = realmInstance
    .All<Person>()
    .Where(p => p.Age > 65)
    .ToList();
```
will work much faster than

```C#
var eldersSlow = realmInstance
    .All<Person>()
    .ToList()
    .Where(p => p.Age > 65)
    .ToList();
```

The latter query fetches all the persons from the database file into memory and does the filtering in memory. The former uses our intention written as `IQueryable` and transforms it into operations needed only to fetch those Persons from the disk that are needed.

> Note: This is of course just a simplification, the exact algorithm is much more complicated and depending on the format layout it may be better to fetch more than needed, however, such implementation details are not part of this document and [will be left to a more curious reader](https://github.com/realm/realm-dotnet). The point to take is that it will work faster/better because these operations do not have to be done in memory.

At the moment of writing this, at least these [operators are supported](https://realm.io/docs/dotnet/latest/api/linqsupport.html):

* `Where(predicate)` in which the predicate uses only the persisted elements
  * Also, only one level of indirection can be used. For example, this is not supported: `.Where(p => p.Address.Country.Name == "Estonia")`
* Ordering operators: `OrderBy(propertySelector)`, `OrderByDescending(propertySelector)`, `ThenBy(propertySelector)`, `ThenByDescending(propertySelector)` 
    * This applies exclusively to ordering selectors that target only persisted properties
    * Similar to `Where`, only one level of indirection can be used. Sorting users by Country names with `OrderBy(p => p.Address.Country.Name)` will not work
* `ToArray`, `ToList`, `ToDictionary`, and `ToLookup`.
* `First(predicate)`, `Last(predicate)`, `Single(predicate)` and all their `*OrDefault` versions
* `Any(predicate)`
* `Count(predicate)`
* For strings, these can be used: `Contains`, `StartsWith`, and `EndsWith`, `Equals`, and `Like`
* Operators `!=` and `==` can be used for all types, and `>`, `>=`, `<`, `<=` for numeric types
* `&&` and `||` and parentheses grouping can be used to create more complex logical conditions in predicates, such as `.Where(p => p.Age > 30 && (p.Age < 40 || p.Age > 60))`

These are not supported (and by not supported, that means that these must be done with the in-memory objects, i.e. `Enumerables`):
* `Select` is not supported, so any kind of conversion must be done in-memory
* `OfType` and `Cast`. These will probably not be supported ever because it makes no sense. The resulting type is already specified by a generic type `T` in `All<T>`
* `Reverse` is not yet supported
* `All` and `Contains` are not yet supported
* `LongCount`, `Sum`, `Min`, `Max`, and `Average` are not yet supported
* `GroupBy` 
* Set operators `Distinct`, `Union`, `Intersect` and `Except` are also not supported
* Partitioning with `Take`, `Skip` and similar are not supported. While Realm docs says that it makes no real benefit since the access to data is random access, they could have implemented those and under the hood just fetch the data that would be retrieved as if those were applied. For completeness sake. Ah, well... :disappointed:

### :busts_in_silhouette: A lesson in reference equality

In the following example, both `Person` and `Address` inherit from `RealmObject`. When we use the same reference to specify related property in a Realm object, as is done in this example with the same `address` reference, Realm will actually store only one address in the database, and the persons will point to the same.

```c#
var address = new Address();

realm.Write(() =>
{
    realm.RemoveAll();
    realm.Add(new Person { Address = address });
    realm.Add(new Person { Address = address });
});

var persons = realm.All<Person>().ToList();

var x = persons[0].Address;
var y = persons[1].Address;

Console.WriteLine(x == y);
```

Interestingly enough, the above code will output `False`. The two address objects will not be reference-equal, even though they were equal before. How exactly it points to the same object in the file internally, I am not sure, I don't know and it's a black box.

Even more interesting is the following: had we created two different instances of addresses with all the same property values and assigned those to the two persons, the database would actually hold two addresses with the same values. Fetching them again would also lead to different references. So in any case, Realm does not hash, cache or reuse objects in Realm instance for reference equality purposes. 

:warning: Here comes the scary part. Even though the references are not the same, changing one value will be visible in other instances as well.

```c#
realm.Write(() =>
{
    persons[0].Address.StreetNo = 14; 
});

Console.WriteLine(persons[1].Address.StreetNo == 14); // True
```

At least, there is a safety mechanism there: the change must be done within the `.Write` or an exception will be thrown. This forces us to think about where, when and why we are doing the changes.

All this behavior must be well understood when making choices about the architecture we use.

## :crystal_ball: Prime Radiant

Prime Radiant is a device designed by Hari Seldon and built by Yugo Amaryl, used for storing the psychohistorical equations showing the future development of humanity. It projects the equations onto walls in some unexplained manner, but it does not cast shadows, thus allowing workers easy interaction. Control operates through the power of the mind, allowing the user to zoom in to details of the equations, and to change them. One can make annotations, but by convention all amendments remain anonymous.

It is also the database project of the mobile people.

### :book: Additional topics

Additional information could be given on 
* linked entities
* concurrency issues
* immutability
* performance considerations

but those will be discussed internally.

# iOS Table view sources

This document will show how to create UITableView sources in our app so we do this in a standard way for all our table views.

There are basically two types of table views in the application right now: sectioned and plain. The former have multiple sections with headers and the latter doesn't, but we treat both of those equally, taken plain table views as table views with just one section.

We use the class `CollectionSection<THeader, TItem>` to create sections in our collections for table and recycler views. That means that those collections should be:

`IEnumerable<TItem>` for plain table views or
`IEnumerable<CollectionSection<THeader, TItem>>` for sectioned table views

Then table views can be also classified as those which reload completely every time there's a change in the data and those that animate every change (or group of changes) without reloading the whole thing (`ReloadTableViewSource` and `AnimatedTableViewSource` respectively).

## ReloadTableViewSource

This is the class you'll have to use to create a source for your table view. You can either subclass it or use it directly. It inherits from `BaseTableViewSource`, which has some methods and properties that'll be common to `AnimatedTableViewSource` too.

### Creation

The constructor is generic over two types: The model for the header and the model for each cell. And it'll get as parameters a delegate for the construction and configuration of each cell and optionally an immutable list with the table items (sectioned or not). If you don't specify them in the constructor you'll then need to bind the table view to an Observable of the items (see next section)

```c#
var source = new ReloadTableViewSource<SectionHeaderViewModel, MyCellViewModel>(
    cellConfigurationDelegate,
    ViewModel.TableItems
);

MyTableView.Source = source;
```

`cellConfigurationDelegate` is a method that dequeues the cell, configures it and return it. Its type:
`public delegate UITableViewCell CellConfiguration<TModel>(UITableView tableView, NSIndexPath indexPath, TModel model)`

As most cells in the application behave in a very similar way (they configure themselves when their view model is set), we can make them inherit from `BaseTableViewCell` which is generic over the cell's view model and updates itself whenever that view model changes).

It also adds a static method that dequeues and configures the cell automatically, so, as long as the cell is a subclass of `BaseTableViewCell`, the above example would be:

```c#
var source = new ReloadTableViewSource<SectionHeaderViewModel, MyCellViewModel>(
    MyCustomViewCell.CellConfiguration(MyCustomViewCell.Identifier),
    ViewModel.TableItems
);

MyTableView.Source = source;
```

And we don't need to write that delegate ourselves.

[Real world example](https://github.com/toggl/mobileapp/blob/1ae9f9554d69430dbd7edfe2f277e9990d5dd218/Toggl.Daneel/ViewControllers/SelectDateFormatViewController.cs#L39)

### Binding

If we want the table view to reload every time the observable of the collection of items emits a new event, we can bind it so it reloads automatically. This is done like this:

```c#
var source = new ReloadTableViewSource<SectionHeaderViewModel, MyCellViewModel>(
    cellConfigurationDelegate
);

ViewModel.ItemsObservable
    .Subscribe(MyTableView.Rx().Items(source))
    .DisposedBy(DisposeBag);

MyTableView.Source = source;
```

`ViewModel.ItemsObservable` has to be of one of these types:
* `IObservable<IEnumerable<MyCellViewModel>>` for plain table views
* `IObservable<IEnumerable<CollectionSection<MyHeaderViewModel, MyCellViewModel>>>` for sectioned table views.

[Real world example](https://github.com/toggl/mobileapp/blob/ae874b5aa87eac33a9613f6b2f389d0e97d504ac/Toggl.Daneel/ViewControllers/SelectClientViewController.cs#L41)

### Events

The `BaseTableViewSource` also forwards some table view events, which are transformed into Observables through reactive extensions.

So we can do:

```c#
source.Rx().ModelSelected() // Sends the cell view model
    .Subscribe(ViewModel.SelectModel)
    .DisposedBy(DisposeBag);
```

and

```c#
source.Rx().Scrolled()
   .Subscribe(onTableViewScroll)
   .DisposedBy(disposeBag);
```

[Real world example](https://github.com/toggl/mobileapp/blob/1ae9f9554d69430dbd7edfe2f277e9990d5dd218/Toggl.Daneel/ViewControllers/SelectDateFormatViewController.cs#L46)

### Sections and Headers

For tables with sections the data source has to include the `CollectionSection`, the binding will be similar, but uses a different Rx metho (`Sections` instead of `Items`):

```c#
ViewModel.SectionedItemsObservable
    .Subscribe(TableView.Rx().Sections(source))  // It uses `Sections` instead of `Items`
    .DisposedBy(DisposeBag);
```

And then the data source has to have a delegate which dequeues, configures and returns the header for each section, it's type is:

`delegate UIView HeaderConfiguration<THeader>(UITableView tableView, int section, THeader header)`

[Real world example](https://github.com/toggl/mobileapp/blob/1ae9f9554d69430dbd7edfe2f277e9990d5dd218/Toggl.Daneel/ViewSources/SelectUserCalendarsTableViewSource.cs#L39)

## AnimatedTableViewSource

To be developed...

For now, this document, focuses on the reloading ones, but the animated once will work in a very similar way (and use diffing to find the changes automatically)

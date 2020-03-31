## Translation Guide

Adding a new language to the Toggl mobile app is very simple!
All you have to do is get the list of sentences and words we use inside the app and translate them, keeping the same file format.

## How are the translations handled?
Please take a look at our [Mobile Translation Framework documentation](./localization-framework.md) for details about the translation process.
Before starting to translate, please check if the language you're aiming to translate the apps for isn't already being translated.
You can check that in the [GitHub Translations Project's Preparing Translation(s) column](https://github.com/toggl/mobileapp/projects/74#column-6151592).

## How to contribute

### How can I start a translation request?
If there isn't a translation request for your desired language or translation improvement, you can create a new issue to make your request. 
Take a look at the Mobile Translation Framework documentation section about the translations GitHub Project [section about open requests](./localization-framework.md) and start a [New Language Request issue](https://github.com/toggl/mobileapp/issues/new?template=h-translation-request.md&title=New+Language+Request+-+Examplanese%2C+eg-EG) or [Translation Improvement issue](https://github.com/toggl/mobileapp/issues/new?template=i-translation-fix-request.md&title=Translation+Improvement+Request+-+Issue%2FFeature%2FScreen+-+Language+code).

### Can I contribute to a translation in progress?
Yes, feel free to leave a comment in the translation issue telling that you want to help.
If there's an open translation pull request for the language you want to contribute to, leaving a comment on it is the easiest way to help.
You can make suggestions that result in direct changes in the code base. Take a look in the [GitHub documentation on how to do that](https://help.github.com/en/articles/commenting-on-a-pull-request).

### How do I create a translation pull request?
When submitting a Pull Request for a translation, please select one of the following pull request templates bellow, based on the type of your translation, copy its content and use in the body of the new Pull Request:
  - [PR template for new translations](../../.github/PULL_REQUEST_TEMPLATE/translation-pull-request-template.md)
  - [PR template for fixes/improvements](../../.github/PULL_REQUEST_TEMPLATE/translation-fix-pull-request-template.md)
  - [PR template for new copies](../../.github/PULL_REQUEST_TEMPLATE/new-copy-translation-pull-request-template.md)

If you are not comfortable doing that, feel free to ask us in the translation issue and one of us will do it for you. 

### Where do I get the list of what needs to be translated?

The sentences/words that need to be translated can be found here [Toggl.Shared/Resources.resx](../../Toggl.Shared/Resources.resx).

### The `Resources.resx` file

This file contains all words/sentences used in our app in English. It can look intimidating at first, but it follows a simple pattern and the beginning of the file can be ignored.

It looks like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
    <!-- One huge comment about the Microsoft ResX Schema -->
    <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
        <!-- A lot of extra content irrelevant for the translations -->
    </xsd:schema>
    <resheader>
        <!-- Some header tags, also irrelevant for the purposes of the translations -->
    </resheader>
    
    <!-- Finally, here's where the fun begins -->
    <!-- A very long sequence of tags like this will follow -->
    <!-- name="SomeName" is the identifier of the text entry -->
    <!-- xml:space="preserve" can be ignore, but has to stay as it is -->
    <data name="SampleTitle" xml:space="preserve">
        <!-- Between the <value> tag and it's closing </value> tag, resides the actual text, which you'll be translating -->
        <value>Title</value>
        <!-- The comment tag will provide you with some context about the text entry to be translated -->
        <comment>This is a title that appears on screen X</comment>
    </data>
    <data name="StopButton" xml:space="preserve">
        <value>Press here to stop the time tracker</value>
        <comment>This is a text used for accessibility reasons to describe the button to stop a time entry</comment>
    </data>
</root>

``` 

Let's say you are translating the app to Japanese, the first thing you'll do is to copy the base file [Toggl.Shared/Resources.resx](../../Toggl.Shared/Resources.resx) into a new file named `Resources.ja.resx`, which should be place next to the original.
The `Toggl.Shared` folder should now contain both files, the original `Resources.resx` and the new `Resources.ja.resx` which will contain the Japanese translations. It would look like this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
    <xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
        <!-- don't modify any of this -->
    </xsd:schema>
    <resheader>
        <!-- don't modify any of this -->
    </resheader>
    <!-- Leave above from the original file untouched -->
    <data name="SampleTitle" xml:space="preserve">
        <value>題</value>
        <!-- You don't need to translate the comments -->
        </data>
    <data name="StopButton" xml:space="preserve">
        <value>時辰儀止めるボタン</value>
    </data>
</root>
```

If you are translating into another language, the only difference is the language code in the `.resx` file. For example, a Brazilian Portuguese translation would require a file named `Resources.pt-Br.resx`, for Portugal Portuguese, it would be `Resources.pt-PT.resx`, for Latvian or Lettish, it would be `Resources.lv.resx`.

### Where can I find the language codes for Resources.`language code`.resx files? (pt-BR, pt-PT, lv, ja etc)
More information on where to get the right language codes can be found in the [Microsoft .NET CultureInfo docs](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=netframework-4.8#culture-names-and-identifiers).

TLDR; "The CultureInfo class specifies a unique name for each culture, based on RFC 4646. The name is a combination of an [ISO 639 two-letter lowercase](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes) culture code associated with a language and an [ISO 3166 two-letter uppercase](https://en.wikipedia.org/wiki/List_of_ISO_3166_country_codes) subculture code associated with a country or region".

If you are not sure, feel free to ask us for the right language code as well.
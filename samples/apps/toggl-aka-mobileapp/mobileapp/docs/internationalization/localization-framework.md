## Mobile Translation Framework

### Who is translating the app?

Toggl Mobile apps are translated through crowdsourcing. That means, both our users and translation volunteers are welcome to report erroneous texts, suggest changes or even [directly contribute to our codebase](./how-to-contribute-with-translations.md).

### In which language was the app originally developed?

English. The translations should be based on the original, default language.

### What file format do we use for storing text entries?

We use `RESX` files, the built-in mechanism for localizing .NET applications.
If you are curious, you might want to look at the [Microsoft docs](https://docs.microsoft.com/en-us/dotnet/framework/resources/creating-resource-files-for-desktop-apps#resources-in-resx-files).

### Where can I find current translations?

The text entries resources files can be found in the [Toggl.Shared folder](../../Toggl.Shared).
The base file is [Toggl.Shared/Resources.resx](../../Toggl.Shared/Resources.resx).
Each translation will have its own file which follows the following format: `Toggl.Shared/Resources.language-code.resx`. E.g. Japanese translations should go into `Toggl.Shared/Resources.ja.resx`.

### How to contribute?

You can contribute as much as you want and in the way that feels most comfortable to you.

- We've written a guide on how to translate the [Toggl.Shared/Resources.resx](../../Toggl.Shared/Resources.resx), which can be found at [docs/internationalization/how-to-contribute-with-translations.md](./how-to-contribute-with-translations.md).
- Leave comments with partial translations in the original translation issue. One of our developers will take care of committing the translation you've added from the comment into an open pull request (or create one if it isn't there yet).
- You can ask us to open a pull request for you to bootstrap the translation process.
- Make GitHub change suggestions on open pull requests. [GitHub instructions here](https://help.github.com/en/articles/commenting-on-a-pull-request).
- If you are a developer or want to learn how to use Git/GitHub, feel free to fork the project, create a branch and submit a pull for the translation.
- If you need any help, ask us, we are quite friendly :).

### How do we organize translations?

We are utilizing a [GitHub project](https://github.com/toggl/mobileapp/projects/74) to organize the translation-related work.
The project has columns for each phase of the translation process, indicating the progress of translations in progress.

One of our developers takes care of moving translations issues and pull requests between the project columns.

### The Translations GitHub Project

The project has the following columns:

- Open Requests:

    All new open requests are located in this column. Different types of translation requests are created using different issue templates:

    - New Language Request:
    If you would like the whole app to be translated into a new language, create a [New Language Request issue](https://github.com/toggl/mobileapp/issues/new?template=h-translation-request.md&title=New+Language+Request+-+Examplanese%2C+eg-EG). If you see that the [Translations project](https://github.com/toggl/mobileapp/projects/74) already has a request for the language or improvement you are rooting for, don't hesitate to comment or react to that issue.
    - Translation Improvement:
    If there are any imprecise/incorrect texts that need to be improved in a specific language, create a new [Translation Improvement issue](https://github.com/toggl/mobileapp/issues/new?template=i-translation-fix-request.md&title=Translation+Improvement+Request+-+Issue%2FFeature%2FScreen+-+Language+code).
    - New Copy Translation:
    When introducing new pieces of text in the app, all of them need to be translated to currently supported languages before they can be released. Create a [New Copy Translation issue](https://github.com/toggl/mobileapp/issues/new?template=j-new-copy-translation-request.md&title=New+Copy+Translation+Request+-+Issue%2FFeature%2FScreen) to start the translation process for new copy.
- In Progress
Translation request issues in this column either have been assigned to translators or are currently being worked on in some other way.
- In Review
You are welcome to review the pull requests in this column. Take a look and leave comments and suggestions if you think something can be improved.
- Done
Translation request issues in this column have been sufficiently approved by one of our developers (and ideally by a translator) and are waiting to be released!

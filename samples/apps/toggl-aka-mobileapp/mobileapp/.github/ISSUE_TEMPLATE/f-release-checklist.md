---
name: "📦 Release checklist"
about: A checklist before releasing to the AppStore and Play Store.
---

## During development phase

- [ ] Add this issue to the release project
- [ ] Pick a release name. iOS uses fruits while Android uses desserts. Check the latest release and pick a fruit/dessert with the next letter of the alphabet.
- [ ] Create a release branch from `develop` (or the latest tag for a hotfix) named after the release
- [ ] Bump the version numbers (including app extensions in iOS)
- [ ] Create a release pull request, including the diff and write the user-facing changelog following [the guidelines][2], and add it to the release project
- [ ] Link the release pull request in this issue
- [ ] Have the changelog reviewed
- [ ] When all changes are included in the release branch, create a new tag, name should be `ios-X.Y` or `android-X.Y`
- [ ] Create a GitHub release from the tag and include the user-facing changelog
- [ ] Inform `@mobileteam` and `@support` in the `#mobile-support` channel that a new build is available for testing, include the user-facing changelog


## Testing phase

- [ ] Create a manual testing issue from the template including all release specific features or bug fixes that need to be tested
- [ ] Add the testing issue to the release project
- [ ] Link the release pull request in the testing issue
- [ ] Ensure that the release is tested according to the created testing issue
- [ ] Close the testing issue once all testing is done
- [ ] Check that there are no bugs/crashes, that need to be fixed ASAP. If there are, open an issue and fix them as a release bug fix


## Release phase

- [ ] Create a new version on AppStore Connect or Google Play Console, include the reviewed changelog
- [ ] Ask in the `#marketing-apps` channel if any app metadata needs to be updated
- [ ] Make sure they translated the changelog into the languages that we support (default texts are [here](https://www.notion.so/toggl/Localized-Changelogs-b490351ff90e445eb98335b9ae770268))
- [ ] Release to users with a phased rollout (send for review with automatic release to users selected in case of iOS)
- [ ] Inform `@mobileteam` and `@support` in the `#mobile-support` channel that a phased rollout started


## Post-release phase

- [ ] Once the app goes out to at least 1 user, merge (not squash) the release pull request
- [ ] Monitor the phased rollout progress, observe for significant new crashes and pause the rollout if needed
- [ ] Post updates in the `#mobile-support` channel mentioning adoption and crash-free users


## Hotfix phase (only if needed)

- [ ] When significant problems are found, pause the rollout
- [ ] Create a new branch from the current release tag adding `-hotfix` to it's name
- [ ] Fix any issue in the hotfix branch and go back to the testing phase


## Closing this issue

- [ ] Close this issue and the release project when the release and any hotfix have shipped to all users


### Notes and useful links

1. [Release flow documentation][1]
2. [Changelog guidelines][2]
3. Diff link example for new releases: `https://github.com/toggl/mobileapp/compare/develop...android-A.B-C`
4. Diff link example for hotfix: `https://github.com/toggl/mobileapp/compare/android-A.B-C...android-X.Y-Z`

[1]: https://github.com/toggl/mobile-docs/blob/develop/release-flow.md
[2]: https://github.com/toggl/mobile-docs/blob/develop/release-flow.md#write-a-changelog-our-users-can-understand

# Build process

## Gotchas in `build.cake` for release builds
For 'regular' release builds, that is ones triggered by a tag push, we generate a bundle version/build version based on the tag. Sometimes, we need to trigger such builds from a branch, just to test something in TestFlight or Play Store internal testing. 

In such situations, bitrise will not pull the tags and we can't generate the build version using the tag-based approach. We can't just pull the tags either, because we'd then have a version that would clash with a previously used one. Incrementing such version by 1 would also not work, if we had to then create a revision for a regular release build.

For such builds, we generate the version based on a timestamp. Specifically, we take `yyMMdd` part of the timestamp and append to it a random value between 15 and 99.

### Why not another way?
We can't just use the whole `yyMMddHHmm`, because:
- `2100000000` is the max build version Google Play supports
- for Android, the version we generate gets prepended with a number between 2 and 5 to differentiate between architectures.

Multiple approaches to making the timestamp shorter have been considered:
- map the yy component so that it only takes one digit, so year 19 would become 1, 20 -> 2, etc. This would break in 9 years.
- make the hour component use one digit - that would become messy (if we just used 12-hour clock, we'd have to map hours 10-12 to something else).
- some other approaches which were equally bad.

Why do we append `rnd(15,99)` to yyMMdd? In order to 'make sure' we don't clash with build numbers actually generated from tags. For instance, for 6th November 2019, the date component is `191106` so the whole build number could end up being `19110610`. If, in the future, we have a tag `android-19.11.6-10`, the version number generated from the tag would be identical to the one in the timestamp. As a result, we would be unable to upload to Play Store. In order to prevent that from happening, __it is assumed the revision number will always be lower than 15__.

__We need to be very careful to not release builds with timestamp-based versions to users!__ If Android users were to install such a build, they would not be able to install regular updates without first uninstalling the app. This is due to the timestamp-based build versions having larger value than the tag-based ones and Google Play preventing app downgrades based on these values.


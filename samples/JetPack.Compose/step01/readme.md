# Step 01



1.  Android Gradle plugin version 7.0.2 has an upgrade available. Start the AGP Upgrade Assistant to update this 
    project's AGP version.

    Start AGP Upgrade Assistant

    Satarted

    Updates available
To take advantage of the latest features, improvements and fixes, we recommend that you upgrade this project's Android Gradle Plugin from 7.0.2 to 7.4.2.


    Upgrade Gradle version to 7.5

        Version 7.5 is the minimum version of Gradle compatible with Android Gradle Plugin version 7.4.2.


    Upgrade Gradle plugins
Some Gradle plugins in the project use interfaces which are no longer supported in version 7.5 (or later) of Gradle and version 7.4.2 (or later) of Android Gradle Plugin.
The following Gradle plugin versions will be updated:
Update version of org.jetbrains.kotlin:kotlin-gradle-plugin to 1.6.21

Upgrade AGP dependency from 7.0.2 to 7.4.2
Changing the version of the Android Gradle Plugin dependency effectively upgrades the project. Pre-upgrade steps must be run no later than this version change; post-upgrade steps must be run no earlier, but can be run afterwards by continuing to use this assistant after running the upgrade.


Sync succeeded
The upgraded project successfully synced with the IDE. You should test that the upgraded project builds and passes its tests successfully before making further changes.
The upgrade consisted of the following steps:
Upgrade AGP dependency from 7.0.2 to 8.3.0
Upgrade Gradle version to 8.4
Upgrade project JDK for running Gradle
Move package from Android manifest to build files
Update default R8 processing mode
Enable buildConfig build feature
Preserve transitive R classes
Preserve constant R class values
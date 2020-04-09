# Certificate for staging :scroll: 

When testing the mobile app, one should always use the staging environment. Xamarin has some problems with that due to the certificate staging uses, so you need to do some preparations before testing the application.

The link to the certificate can be found on #mobile-team (it's pinned, but you can always ask if you can't find it).

## Integration tests :computer:

Run the snippet below, replacing `$CERTIFICATE_URL` and `$FILE_NAME` with the appropriate values:

```
curl -O $CERTIFICATE_URL
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain $FILE_NAME
```

## iOS App :iphone:

1. When you open it, you will see [this screen](https://user-images.githubusercontent.com/7688727/29566049-ca4995f4-871e-11e7-938d-41ddbbd38cd0.png).
2. Click install, pretend you read the warning and then click install again. When the prompt appears, click install a third time and question Apple's UX team.
3. If you followed it all correctly, you will see [this screen](https://user-images.githubusercontent.com/7688727/29566109-fb0f63ee-871e-11e7-8a4a-762e6c9a0a0a.png). Click done to close the settings.
4. Navigate back to the home screen and open the settings app.
5. Click General -> About -> Certificate Trust Settings. You shall see [this](https://user-images.githubusercontent.com/7688727/29566227-6c3ae746-871f-11e7-8287-fca4429ed1ac.png).
6. Toggle (yes, with an `e` at the end) the switch to enable the `Toggl Root CA` certificate. Click `Continue` on the prompt that will show up.

The app now works on staging :tada:

## Android App :robot:

1. Open the certificate, give it a name and click install. 
2. There is no step 2. Android is simple.

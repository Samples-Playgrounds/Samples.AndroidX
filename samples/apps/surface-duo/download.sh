#!/bin/bash


rm -fr jetpack-window-manager-alpha/
curl -O -J -L \
    https://github.com/microsoft/surface-duo-sdk-xamarin-samples/archive/refs/heads/crdun/jetpack-window-manager-alpha.zip

unzip jetpack-window-manager-alpha.zip
mv jetpack-window-manager/ jetpack-window-manager/
rm -fr jetpack-window-manager-alpha.zip


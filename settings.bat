::This file contains the settings for your mod, to allow the automation scripts to function properly.
::Before running any scripts ENSURE you have changed these variables correctly

::Change this to match the name of your Mod. It sets the name in Thunderstore and the release zip file.
::Cannot contain spaces or special characters, no ' or " either!
set MOD_NAME=MartialLaw
:: Change this your your anme as the author
set MOD_AUTHOR=boxlegs
::Change this to your mods version, it will be set automatically into the `manifest.json` and the release zip
set MOD_VERSION=1.0.3
:: If you are using the Thunderstore local mod approach to start Mage Arena with your mod
:: And want the build.bat script to automatically update your mod after every build
:: set this to the path of your local mod in the correct profile
:: You can find the path from `Settings > Browse Profile Folder`
:: Example: C:\Users\Walthzer\AppData\Roaming\Thunderstore Mod Manager\DataFolder\MageArena\profiles\DEVELOPMENT
set THUNDERSTORE_LOCAL_MOD_PATH=D:\Games\r2modman\MageArena\profiles\development
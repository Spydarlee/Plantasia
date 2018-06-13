![Plantasia screenshot](https://img.itch.zone/aW1nLzEyNDcyODgucG5n/original/VBJC%2F%2F.png)
# Plantasia
Plantasia is a small game about cultivating your very own plant-covered island paradise! Take a small, barren island floating in space and fill it with wondrous flora! Explore the universe to discover new species and help them grow. The game is available (for free) on Windows, Mac and Linux via [itch.io](https://karnbianco.itch.io/plantasia) and Android devices via the [Google Play Store](https://play.google.com/store/apps/details?id=com.KarnBianco.Plantasia).

This repository contains all the custom source code and assets that make up the Plantasia Unity project. Most of these are in the [Source folder](UnityProjects/Plantasia/Packages/Source) but some other content, such as Blender files and vector graphics, that aren't used in-game directly can be found in the [Content folder](Content).

Unless otherwise noted (in the [Credits section](#credits)) all code and assets are dedicated to the public domain via the [CC0 license](https://creativecommons.org/share-your-work/public-domain/cc0/), which means you're free to use them for any purpose! No warranty is implied, though, so use at your own risk. No need to credit me if you use anything either, although it's always appreciated!

 ## Building and Running

Plantasia uses [Projeny](https://github.com/modesttree/Projeny) to manage its various dependencies, including Unity asset store assets which cannot be uploded to GitHub for licensing reasons. But you don't actually need to use Projeny to get Plantasia running locally (which is lucky, because it only works on Windows at the moment).

### Common First Steps
* Clone or download this repository
* Manually (sorry!) grab all of the following dependices: [ActionGraph](https://github.com/Spydarlee/ActionGraph/), [LiteCSVParser](https://github.com/floatinghotpot/LiteCsvParser), [LeanTween](https://github.com/dentedpixel/LeanTween), [Full Serializer](https://github.com/jacobdufault/fullserializer), [Cinemachine](https://assetstore.unity.com/packages/essentials/cinemachine-79898), [DefaultPlayables](https://assetstore.unity.com/packages/essentials/default-playables-95266), [PostProcessing Stack](https://assetstore.unity.com/packages/essentials/post-processing-stack-83912), [Simple Sky - Cartoon assets](https://assetstore.unity.com/packages/3d/simple-sky-cartoon-assets-42373), [StandardAssets (Projeny version)](https://github.com/modesttree/Projeny/releases/download/v0.3.4/StandardAssets-Unity5.3.4.zip)

### With Projeny (Windows only)

* Install Projeny using [the instructions here](https://github.com/modesttree/Projeny#installation)
* Put the dependcies from above somewhere that Projeny can find them, for example *UnityProjects/Plantasia/Packages*. I keep all my cross-project Projeny packages in a special folder that's referenced from  *C:/Users/username/Projeny.yaml*:
	
![Global Projeny.yaml file](https://i.imgur.com/3Hz7BGG.png)

* Side note: You can find more details about the various Projeny config files and how to set them up on the [project's GitHub page](https://github.com/modesttree/Projeny#projeny-yaml).	
* Ensure that the names of the folders that each dependency lives in match the names defined in *UnityProjects/Plantasia/ProjenyProject.yaml* (as shown below)

![Global Projeny.yaml file](https://i.imgur.com/Fyb0sS2.png)

* In the root Plantasia directory (where *Projeny.yaml*) lives, use PowerShell or the command prompt to run **prj --init** which initialises Projeny and creates a folder for each platform in *UnityProjects/Plantasia/* 
* You should now be able open any of the platform-specific projects (e.g. *UnityProjects/Plantasia/Plantasia-Windows/*) with Unity and open the *Main* scene to play the game!

### Without Projeny (all platforms)

* Create a new Unity project called Plantasia (or whatever you like)
* Overwrite the project settings files using the ones found in *UnityProjects/Plantasia/ProjectSettings* 
* Copy everything from the *UnityProjects/Plantasia/Packages/Source* folder into your Assets folder
* Copy all of the dependencies listed above into the Assets folder
* Open the *Main* scene and run the game!

## Credits

**Creative Commons Assets**

* [Music](UnityProjects/Plantasia/Packages/Source/Audio/Music) (CC0) by [TRG Banks](http://freemusicarchive.org/music/TRG_Banks/) via [freemusicarchive.org](http://freemusicarchive.org)
	* Title Screen - [St. Peters](http://freemusicarchive.org/music/TRG_Banks/Bills_Adventures/St_Peters)
	* Planetoids - [First stop](http://freemusicarchive.org/music/TRG_Banks/This_train_doesnt_stop_at_Rugby/First_stop)
	* In Space - [Above the Earth](http://freemusicarchive.org/music/TRG_Banks/This_train_doesnt_stop_at_Rugby/Above_the_Earth)
* [Icons](UnityProjects/Plantasia/Packages/Source/Sprites/Icons) (CCBy3.0) by Delapouite, Skoll, sbed and Lorc via [game-icons.net](http://game-icons.net)
* [Sounds](UnityProjects/Plantasia/Packages/Source/Audio/Sounds) (CC0) via [freesound.org](http://freesound.org) and [kenney.nl](http://kenney.nl)
* [Font](UnityProjects/Plantasia/Packages/Source/Fonts) (public domain) is [Halogen](https://www.1001freefonts.com/fr/halogen.font) by JLH Fonts via [1001fonts.com](1001fonts.com)
 
**Open Source Software**

* [LiteCSVParser](https://github.com/floatinghotpot/LiteCsvParser) (MIT License)
* [LeanTween](https://github.com/dentedpixel/LeanTween) (MIT License)
* [Full Serializer](https://github.com/jacobdufault/fullserializer) (MIT License)
* [Projeny](https://github.com/modesttree/projeny) (MIT License)

**Unity Asset Store Assets (Free)**

* [Cinemachine](https://assetstore.unity.com/packages/essentials/cinemachine-79898) (Unity Technologies)
* [DefaultPlayables](https://assetstore.unity.com/packages/essentials/default-playables-95266) (Unity Technologies)
* [PostProcessing Stack](https://assetstore.unity.com/packages/essentials/post-processing-stack-83912) (Unity Technologies)
* [Simple Sky - Cartoon assets](https://assetstore.unity.com/packages/3d/simple-sky-cartoon-assets-42373)  (Synty Studios)
* [StandardAssets.ParticleSystems](https://www.assetstore.unity3d.com/en/#!/content/32351) (Unity Technologies)

## Support

If you found Plantasia fun or the source useful, you can help enable me to keep my work free by [buying me a cofee](http://ko-fi.com/L4L6D3JD), [donating via Paypal](https://www.paypal.me/karnb) or becoming a patron on [Patreon](https://www.patreon.com/bePatron?u=3614608&redirect_uri=http%3A%2F%2Fkarnbianco.co.uk%2F&utm_medium=widget). Alternatively, sharing via social media is great, too. Thank you! <3

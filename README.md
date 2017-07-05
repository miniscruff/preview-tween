# Preview Tween
[![license](https://img.shields.io/github/license/mashape/apistatus.svg)](LICENSE)
[![dev-status](https://img.shields.io/badge/dev%20status-early%20development-orange.svg)]()

A tween system built around an intuitive inspector and being able to preview your tween directly in the editor exactly as it will be in the game, all without having to press play.

## Demo
If you would like to see all the sample scenes running in your browser, [click here](https://developer.cloud.unity3d.com/share/WysjrJmz_G/) to demo the project from unity cloud build.

## Installation
1. Download the latest release from the [Github Releases Page](https://github.com/miniscruff/preview-tween/releases).
   * Note: You will be able to download it from the unity asset store once it is published.
1. Import the unity package into your project.
   * If you are using a unity version before 5.6, you will have to remove the playmode tests as they wont compile properly by deleting the folder "PreviewTween / Tests / PlayMode".

### Running Automated Tests
Preview tween uses automated tests to confirm the features of the system are all running properly. If you would like to run the tests locally yourself or if you are making local changes you can use these tests to prevent regressions.
1. Open the Test Runner by going to "Window > Test Runner".
1. You can then run the Play Mode tests and the Edit Mode test by selecting Run All.

## Basic Usage
1. Prepare the object you are going to tween, in our example we are going to use a simple Tween Position with a box.
1. Add the tween component to the object you want to tween. You can put it on any object and link it using the target variable but if you put it on the same object it will auto-find the target for you.
1. Configure the start and end values, I like to use the Record Start and Record End buttons. This allows you to edit the object as it appears in the scene and simply record this value as either the start or end of the tween.
1. Doing a quick preview now is a good way to check the start and end values. This is a key feature of the plugin and makes working with tweens a lot easier.
1. After that we can configure all the tween settings and continue to preview the changes using the preview bar. Specific setting details are below.

### Settings
The Tween Base script file contains code summary comments for all of the following, but details are listed here as well for non-programmers to reference.
* Delay: The amount of time, in seconds, before our tween actually updates after it starts.
* Duration: The amount of time, in seconds, for the tween to complete. This does not include our delay.
* Play Mode: Under what condition should our tween play.
   * None: The tween will not play unless you call Play() from a script or event
   * Start: The tween will automatically play when the objects Start message is received.
   * On Enable: The tween will automatically play when the objects OnEnable message is received.
* Wrap Mode: How the tween handles reaching the end.
   * Once: Plays once and then stays at the end value.
   * Loop: Replays itself starting from the beginning.
   * Ping Pong: Reverses the direction and easing of the tween.
* Easing Mode: What easing the tween should use, reference the display image for how the tween will animate.
* Custom Curve: If the easing mode is set to Custom Curve, you can create your own curve using Unity's built in animation curve tool. 
* On Complete: Event that is called when the tween is complete, if using Loop or Ping Pong the event is called after EVERY completion.

## Contributing
While preview tween is in early development the best way to contribute is by giving feedback through either github issues or by contacting me using the methods below. Additionally sharing the project, starring and forking the project is greatly appreciated.

If you or your team uses Preview Tween, even if its only for a small portion, of your app or game I would love to hear about it and may add it to this readme (with your permission of course).

## Support
Discord: Ronnie#4609, I am on discord very often so if you want a fast response this is the best option  
Email: halfpint1170 at gmail dot com
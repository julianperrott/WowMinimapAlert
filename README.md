<p align="center">
  <img src="https://raw.githubusercontent.com/julianperrott/WowMinimapAlert/master/Images/starme.png" alt="Star this Repo"/>
</p>


# WowMinimapAlert
This is a little utility which alerts you by playing an wav file when a herb/mining node is visible on the World of Warcraft minimap.

When it is running it looks at the portion of the screen where the minimap is and if it sees any yellow (the colour of a node) then it will alert you.

How is this useful:

1. You can run around the map not paying full attention and be alerted when a node is available.
2. You can camp out a node (e.g. Black Lotus) while you read a book / watch a video etc.

## Video of the app in action

https://youtu.be/sbQqBDAjM1I

[![Wow Mini Map Alert](https://img.youtube.com/vi/sbQqBDAjM1I/0.jpg)](https://www.youtube.com/watch?v=sbQqBDAjM1I)

https://www.youtube.com/watch?v=3pKHHgMbiC4
[![Wow Mini Map Alert](https://img.youtube.com/vi/3pKHHgMbiC4/0.jpg)](https://www.youtube.com/watch?v=3pKHHgMbiC4)

## Build

* Requires: Dot net framework 4.7
https://dotnet.microsoft.com/download/dotnet-framework
* The bot: Compile the source and run it from visual studio 2019.

## Configuring the capture window

<p align="center">
  <img src="https://raw.githubusercontent.com/julianperrott/WowMinimapAlert/master/Images/Setup.png" alt="Setup"/>
</p>

1. Click on the yellow configure button.

2. Click on the capture screen button.

3. Now adjust the capture X,Y and the width and height so that only the centre of the minimap is visible. You probably should then change the values hard coded in WowMinimapAlert\Source\MnmimapAlertBot\Platform\WowScreen.cs.

Any yellow in the minimap is shown in red and this is what triggers the wav to be played.

Note: Any addons which display yellow in the minimap need to be turned off so they don't interfere.

## Running

Click the Play Button after configuring the app.


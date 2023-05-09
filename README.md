# tactileAugmentation
controller vibration for visualization augmentation

hanges since friday:
Cleaned up code.
Connected all scenes to a title screen.
Fixed Aspect Ratio issue
Fixed  issue where I couldn’t transform mouse position into the image’s rect transform (with some help from chatGPT) noted in the code. 
Compiled and created an executable


User Guide

To exit be on title screen and press the “Esc” key

Must use the mouse to press the buttons.

The 5 meter DEM is only accessible trough “title screen” -> “black and white” -> “5 meter”

Grayscale and Vector data functionality
  B Low Frequency: Roughness
  A High Frequency: Intensity 
  X Both
  Y Pulse (not available for vector data)


Raster: Color Image Functionality
  left bumper->  encoded color data
    Green = 3 pulses high frequency motor
    Red = 3 pulses low frequency motor

Notes:
 Other buttons may do things, but  are remnants of testing or other tries. Please ignore them. I am too afraid of breaking things again by removing them.
Tested in 16/9 aspect ratio (specifically 4k and 1080p), UI elements may break with other aspect ratios
Tested on windows 11
Tested with xbox controller(no idea if another controller works)

Youtube Links:
DEM functionality: https://youtube.com/shorts/cR7mavG5MW8?feature=share
Red/Green vibration test: https://youtube.com/shorts/K3n66pY2Z_U?feature=share

Github link

Library folder is 1.3 gigs if it needs to be uploaded, Let me know through my university email thanks.

Richard



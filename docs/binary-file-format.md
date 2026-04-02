[**日本語**](./binary-file-format.ja.md) | [**English**](./binary-file-format.md)

# Binary File Format
## Note
This information is outdated, so please do not use this content as a reference.
## Overview
* Packs the specified sspj file and all registered ssae and ssce information into one at the time of conversion.
* Contains only the information necessary for playback from the information held by the sspj, ssae, and ssce files.
* Animation interpolation calculations are performed during conversion. (Runtime calculation is also planned to be supported)
* Coordinate values are kept as local values without being globalized.
## Details
* File Structure
The ssbp file consists of the following information.
Structures for ssbp parsing are defined in `SS5PlayerData.h`.
	+ Project Settings
	- struct ProjectData
	- dataId　　　　　　　　ID for verifying ssbp file data
	- version　　　　　　　ssbp file version
	- flags　　　　　　　　Reserved
	- imageBaseDir　　　　Resource path, set by argument during conversion
	- cells　　　　　　　　Pointer to cell structures (start address of array)
	- animePacks　　　　　Pointer to animation pack structures (start address of array)
	- numCells　　　　　　Number of cells
	- numAnimePacks　　　　Number of animations
	+ Cell
	- struct Cell
	- name　　　　　　　　Cell name
	- cellMap;　　　　　　Pointer to cell map structure (start address of array)
	- indexInCellMap　　　Cell index
	- x　　　　　　　　　　Starting X coordinate within the cell image
	- y　　　　　　　　　　Starting Y coordinate within the cell image
	- width　　　　　　　　Width of the cell
	- height　　　　　　　Height of the cell
	- reserved　　　　　　Reserved
	+ Cell Map
	- struct CellMap
	- name　　　　　　　　Cell map name (ssce name)
	- imagePath　　　　　　Texture file name
	- index　　　　　　　　Cell map number
	- reserved　　　　　　Reserved
	+ Animation Pack
	- struct AnimePackData
	- name　　　　　　　　　　　Motion name
	- parts　　　　　　　　　　　Pointer to parts structure
	- animations; 　　　　Pointer to animation (motion) structure
	- numParts　　　　　　　　　Number of parts
	- numAnimations　　　　　　Number of motions
	+ Animation
	- struct AnimationData
	- name　　　　　　　　　　　Motion name
	- defaultData　　　　　　　Initial values of parts (start address of array)
	- frameData　　　　　　　　Part data for each frame (start address of array)
	- userData　　　　　　　　　User data (start address of array)
	- labelData　　　　　　　　Label data (start address of array)
	- numFrames　　　　　　　　Total frames
	- fps　　　　　　　　　　　Playback speed
	- labelNum　　　　　　　　Number of label data
	+ Initial Part Values
	- struct AnimationInitialData
	- index　　　　　　　　　　　Part index, order of frame control on SS
	- dummy　　　　　　　　　　　Alignment dummy data for binary data output
	- flags　　　　　　　　　　　Hidden, X flip, Y flip flags
	- cellIndex　　　　　　　　　Cell index
	- positionX　　　　　　　　　Initial X coordinate
	- positionY　　　　　　　　　Initial Y coordinate
	- opacity　　　　　　　　　　Initial opacity
	- anchorX　　　　　　　　　　Origin X offset + initial origin X offset set in the cell
	- anchorY　　　　　　　　　　Origin Y offset + initial origin Y offset set in the cell
	- rotationX　　　　　　　　　Initial X rotation
	- rotationY　　　　　　　　　Initial Y rotation
	- rotationZ　　　　　　　　　Initial Z rotation
	- scaleX　　　　　　　　　　　Initial X scale
	- scaleY　　　　　　　　　　　Initial Y scale
	- size_X　　　　　　　　　　　Initial X size
	- size_Y　　　　　　　　　　　Initial Y size
	- uv_move_X　　　　　　　　　Initial UV X move
	- uv_move_Y　　　　　　　　　Initial UV Y move
	- uv_rotation　　　　　　　　Initial UV rotation
	- uv_scale_X　　　　　　　　　Initial UV X scale
	- uv_scale_Y　　　　　　　　　Initial UV Y scale
	- boundingRadius　　　　　　Initial collision radius
	+ Part data for each frame
	- Part data is variable length and only contains data that differs from the initial values.
	- Header section
	* Part index
	* Flags
	+ The flags bitwise indicate which data is included.
	+ Based on the flags, it is determined whether to read data from the file or from the initial values.
	+ For details, refer to `void Player::setFrame(int frameNo)` in `SS5Player.cpp` included in SS5Player cocos2d-x.
	+ User Data
	- struct UserData
	- partName　　　　　　　Name of the part where this user data is set
	- frameNo　　　　　　　Frame where this user data is set
	- flags　　　　　　　　Types of data included
	- integer　　　　　　　Integer
	- rect[4]　　　　　　　Rectangle data
	- point[2]　　　　　　　Coordinate data
	- str　　　　　　　　　String
	- strSize　　　　　　　Size of the string
	+ Label Data
	- struct LabelData
	- str　　　　　　　　　Label name
	- frameNo　　　　　　　Frame where the label was set

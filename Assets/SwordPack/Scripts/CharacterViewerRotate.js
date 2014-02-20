/*
Character Viewer Rotation Only
By Zodiac Alliance Digital Entertainment, Matumit Sombunjaroen
Updated: 18APR2012
*/

var clickPos 	: Vector2;
var offsetPos	: Vector2;
var divider 	= 80;

function Start()
{
	clickPos = Vector2(0,0);
	offsetPos = Vector2(0,0);
}

function Update () {

	offsetPos = Vector2(0,0);
	
	if(Input.GetKeyDown(leftClick()))
	{
		clickPos = mouseXY();
	}
	
	if(Input.GetKey(leftClick()))
	{
		offsetPos = clickPos - mouseXY();
	}
	
	
	transform.Rotate(Vector3(-(offsetPos.y/divider),offsetPos.x/divider,0.0), Space.World);
}

// Prints the current mouse position
function OnGUI ()
{
	/*GUI.Label(Rect(10,350,200,100), "mouse X = " + Input.mousePosition.x);
	GUI.Label(Rect(10,370,200,100), "mouse Y = " + Input.mousePosition.y);
	
	GUI.Label(Rect(120,350,200,100), "click X = " + clickPos.x);
	GUI.Label(Rect(120,370,200,100), "click Y = " + clickPos.y);
	
	GUI.Label(Rect(210,350,200,100), "offset X = " + offsetPos.x);
	GUI.Label(Rect(210,370,200,100), "offset Y = " + offsetPos.y);*/
}



//////////////////////////////////////////////

// Return true when left mouse is clicked or hold
function leftClick()
{
	return KeyCode.Mouse0;
}

//Immediate location of the mouse
function mouseXY()
{
	return Vector2(Input.mousePosition.x, Input.mousePosition.y);
}

//Immediate location of the mouse's X coordinate
function mouseX()
{
	return Input.mousePosition.x;
}

//Immediate location of the mouse's Y coordinate
function mouseY()
{
	return Input.mousePosition.y;
}


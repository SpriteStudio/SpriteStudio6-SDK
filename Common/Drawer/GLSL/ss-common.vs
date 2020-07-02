"\n\
// Copyright 2019 Web Technology Corp.\n\
#version 120\n\
attribute vec4	varg;\n\
\n\
void main()\n\
{\n\
	gl_TexCoord[0] = gl_TextureMatrix[0] * gl_MultiTexCoord0;\n\
\n\
	gl_Position = ftransform();\n\
\n\
	gl_FrontColor = gl_Color;\n\
\n\
	gl_TexCoord[1] = varg;\n\
}\n\
\n",

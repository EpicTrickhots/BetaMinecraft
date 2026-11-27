#version 330 core

layout(location = 0) in vec3 POSITION;
layout(location = 1) in vec2 TEXCOORD0;

out vec2 uv;

uniform mat4 uTransformation;
uniform mat4 uModel;

void main()
{
	uv = TEXCOORD0;

	gl_Position = vec4(POSITION, 1.0) * uModel * uTransformation;
}
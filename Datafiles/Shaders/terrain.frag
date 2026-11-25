#version 330 core

in vec2 uv;

out vec4 FragColor;

uniform sampler2D texture0;

void main()
{
	FragColor = texture(texture0, uv) * vec4(0.569, 1.0, 0.349, 1.0);
}
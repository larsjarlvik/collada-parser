#version 330

precision highp float;

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

layout(location = 0) in vec3 in_position;
layout(location = 1) in vec3 in_normal;
layout(location = 2) in vec3 in_color;

out vec3 normal;
out vec3 color;

void main(void)
{
	normal = (modelview_matrix * vec4(in_normal, 0)).xyz;
	color = in_color;

	gl_Position = projection_matrix * modelview_matrix * vec4(in_position, 1);
}
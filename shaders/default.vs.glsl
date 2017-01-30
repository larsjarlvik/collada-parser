#version 330

precision highp float;

const vec3 lightDirection = vec3(-1.0, -0.8, -1.0);
const vec3 ambientColor = vec3(0.4, 0.4, 0.4);
const vec3 diffuseColor = vec3(0.7, 0.7, 0.7);

uniform mat4 projection_matrix;
uniform mat4 modelview_matrix;

layout(location = 0) in vec3 in_position;
layout(location = 1) in vec3 in_normal;
layout(location = 2) in highp vec2 in_texture;
layout(location = 3) in vec3 in_color;

out vec3 vLightWeighting;
out vec3 color;
out highp vec2 uv;


void main(void)
{
	vec4 position = projection_matrix * modelview_matrix * vec4(in_position, 1.0);
	
	vec3 lightDirection = (vec4(-lightDirection, 1.0) * modelview_matrix).xyz;
	float directionalLightWeighting = max(dot(in_normal, lightDirection), 0.0);
	vLightWeighting = ambientColor + diffuseColor * directionalLightWeighting;

	color = in_color;
	uv = in_texture;

	gl_Position = position;
}
#version 330

precision highp float;

const vec3 ambient = vec3(0.5, 0.5, 0.5);
const vec3 diffuse = vec3(1.0, 1.0, 1.0);
const vec3 direction = normalize(vec3(1.0, 0.5, 1.0));

in vec3 normal;
in vec3 color;

out vec4 out_frag_color;

void main(void)
{
	float lightWeight = clamp(dot(direction, normalize(normal)), 0.0, 1.0);
	out_frag_color = vec4(color * ambient + (diffuse * color * lightWeight), 1.0);
}
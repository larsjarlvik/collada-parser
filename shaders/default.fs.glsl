#version 330

precision highp float;

in vec3 vLightWeighting;

out vec4 out_frag_color;

void main(void)
{
	out_frag_color = vec4(vLightWeighting * vec3(1.0, 0.0, 0.0), 1.0);
}
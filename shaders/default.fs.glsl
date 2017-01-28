#version 330

precision highp float;

uniform sampler2D main_texture;
uniform bool have_texture;

in vec3 vLightWeighting;
in vec3 color;
in highp vec2 uv;

out vec4 out_frag_color;

void main(void)
{
	if(have_texture)
		out_frag_color = vec4(texture(main_texture, uv).rgb * vLightWeighting, 1.0);
	else
		out_frag_color = vec4(color * vLightWeighting, 1.0);
}
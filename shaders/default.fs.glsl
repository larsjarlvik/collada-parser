#version 400
precision highp float;

const vec4 lightPosEye = vec4(350, 350, 350, 1);

const vec3 ambientLight = vec3(0.6, 0.6, 0.6);
const vec3 diffuseLight = vec3(0.8, 0.8, 0.8);
const vec3 specularLight = vec3(0.1, 0.1, 0.1);

// TODO: Load from collada
uniform vec3 uAmbient;
uniform vec3 uDiffuse;
uniform vec3 uSpecular;
uniform float uShininess;

uniform sampler2D uTexture;
uniform bool uHaveTexture;

in vec3 Position;
in vec3 Normal;
in vec2 TexCoord;
in vec3 Color;

out vec4 out_frag_color;

void main(void)
{
	vec3 n = normalize(Normal);
	vec3 s = normalize(vec3(lightPosEye) - Position);
	vec3 v = normalize(vec3(-Position));
	vec3 r = reflect(-s, n);

	vec3 ambientReflection = ambientLight + uAmbient;
	vec3 diffuseReflection = diffuseLight + uDiffuse;
	vec3 specularReflection = max(dot(s, n), 0.0) + specularLight * pow(max(dot(r,v), 0.0), uShininess);

	// Apply texture/color
	if (uHaveTexture)
		out_frag_color = texture(uTexture, TexCoord);
	else
		out_frag_color = vec4(Color, 1.0);

	// Apply lighting
	out_frag_color *= vec4(uSpecular * (ambientReflection + diffuseReflection * specularReflection), 1.0);
}
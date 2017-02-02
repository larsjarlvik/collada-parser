#version 330
precision highp float;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec2 vTexture;
layout(location = 3) in vec3 vColor;

out vec3 Position;
out vec3 Normal;
out vec2 TexCoord;
out vec3 Color;

void main(void)
{
	mat3 normalMatrix = mat3(transpose(inverse(uViewMatrix)));

	Position = vec3(uViewMatrix * vec4(vPosition, 1.0));
	Normal = normalize(normalMatrix * vNormal);
	Color = vColor;
	TexCoord = vTexture;

	gl_Position = uProjectionMatrix * vec4(Position, 1.0);
}
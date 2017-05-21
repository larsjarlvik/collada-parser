#version 400
precision highp float;

#define MAX_JOINTS 50
#define MAX_WEIGHTS 3

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

uniform bool uIsAnimated;
uniform mat4 uJointTransforms[MAX_JOINTS];

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec2 vTexture;
layout(location = 3) in vec3 vColor;
layout(location = 4) in vec3 vJointWeights;
layout(location = 5) in vec3 vJointIds;

out vec3 Position;
out vec3 Normal;
out vec2 TexCoord;
out vec3 Color;

void main(void)
{
	mat3 normalMatrix = mat3(transpose(inverse(uViewMatrix)));
	vec4 totalLocalPos = vec4(vPosition, 1.0);
	vec4 totalNormal = vec4(0.0);

	if (uIsAnimated) {
		mat4 jointTransform;
		vec4 posePosition;
		vec4 worldNormal;

		for (int i = 0; i < 3; i++) {
			jointTransform = uJointTransforms[int(vJointIds[i])];
			posePosition = jointTransform * vec4(vPosition, 1.0);
			totalLocalPos += posePosition * vJointWeights[i];

			worldNormal = jointTransform * vec4(vNormal, 1.0);
			totalNormal += worldNormal * vJointWeights[i];
		}

		// Result
		Position = vec3(uViewMatrix * totalLocalPos);
		Normal = normalize(normalMatrix * vNormal);
	}
	else {
		Position = vec3(uViewMatrix * vec4(vPosition, 1.0));
		Normal = normalize(normalMatrix * vNormal);
	}

	Color = vColor;
	TexCoord = vTexture;

	gl_Position = uProjectionMatrix * vec4(Position, 1.0);
}
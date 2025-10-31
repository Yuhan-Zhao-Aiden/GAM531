#version 330 core
out vec4 FragColor;

in vec3 vWorldPos;
in vec3 vNormal;
in vec2 vUV;

uniform bool uUseTexture;
uniform sampler2D uTex;
uniform vec3 uColor;
uniform vec3 uLightPos; 
uniform vec3 uLightColor;
uniform vec3 uViewPos; 

void main()
{
    vec3 N = normalize(vNormal);
    vec3 L = normalize(uLightPos - vWorldPos);
    vec3 V = normalize(uViewPos  - vWorldPos);
    vec3 R = reflect(-L, N);

    // Phong terms
    float diff = max(dot(N, L), 0.0);
    float spec = pow(max(dot(R, V), 0.0), 32.0);

    vec3 ambient  = 0.12 * uLightColor;
    vec3 diffuse  = diff * uLightColor;
    vec3 specular = 0.25 * spec * uLightColor;

    vec3 base = uUseTexture ? texture(uTex, vUV).rgb : uColor;
    vec3 lighting = (ambient + diffuse + specular) * base;

    FragColor = vec4(lighting, 1.0);
}

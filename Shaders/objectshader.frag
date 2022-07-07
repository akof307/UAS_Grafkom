#version 330

out vec4 outputColor;

uniform vec3 objColor;
//uniform vec3 lightColor;
uniform vec3 LightPos;
uniform vec3 viewPos;

in vec3 Normal;
in vec3 FragPos;

struct DirLight {
    vec3 direction;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
//uniform DirLight dirLight;
struct PointLight {
    vec3 position;

    float constant;
    float linear;
    float quadratic;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
struct SpotLight{
    vec3  position;
    vec3  direction;
    float cutOff;
    float outerCutOff;

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;

    float constant;
    float linear;
    float quadratic;
};
#define NR_POINT_LIGHTS 24
#define NR_SPOT_LIGHTS 4
//uniform SpotLight spotLight[NR_SPOT_LIGHTS];
uniform SpotLight spotLight;
uniform DirLight dirLight;
uniform PointLight pointLight[NR_POINT_LIGHTS];
vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir);
void main()
{
    //outputColor = vec4(lightColor * objColor, 1.0);
    //float ambientStrength = 0.1;
    //vec3 ambient = ambientStrength * lightColor;
    
    //vec3 norm = normalize(Normal);
    //vec3 lightDir = normalize(LightPos - FragPos); 

    //float diff = max(dot(norm, lightDir), 0.0);
    //vec3 diffuse = diff * lightColor;

    //float specularStrength = 0.5;
    //vec3 viewDir = normalize(viewPos - FragPos);
    //vec3 reflectDir = reflect(-lightDir, norm);
    //float spec = pow(max(dot(viewDir, reflectDir), 0.0), 64); //The 32 is the shininess of the material.
    //vec3 specular = specularStrength * spec * lightColor;

    //vec3 result = (ambient+diffuse+specular) * objColor;
    //outputColor = vec4(result, 1.0);

    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);
    //vec3 result = CalcPointLight(pointLight,norm,FragPos,viewDir);
    vec3 result = vec3(0);
    //result += CalcDirLight(dirLight, norm, viewDir);
    result += CalcSpotLight(spotLight, norm, FragPos, viewDir);
    for(int i = 0; i < NR_POINT_LIGHTS; i++)
        result += CalcPointLight(pointLight[i], norm, FragPos, viewDir);
//    for(int i = 0; i < NR_SPOT_LIGHTS; i++)
//        result += CalcSpotLight(spotLight[i], norm, FragPos, viewDir);
    //vec3 result = CalcSpotLight(spotLight,norm,FragPos,viewDir);
    outputColor = vec4(result, 1.0);
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
   //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    //specular shading
    vec3 reflectDir = reflect(lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 64);
    //combine results
    vec3 ambient  = light.ambient  * objColor;
    vec3 diffuse  = light.diffuse  * diff * objColor;
    vec3 specular = light.specular * spec * objColor;
    return (ambient + diffuse + specular);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    //diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0),256);
    //attenuation
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
 light.quadratic * (distance * distance));
    //combine results
    vec3 ambient  = light.ambient  * objColor;
    vec3 diffuse  = light.diffuse  * diff * objColor;
    vec3 specular = light.specular * spec * objColor;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
} 

vec3 CalcSpotLight(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{

    //diffuse shading
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(normal, lightDir), 0.0);

    //specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 256);
    //attenuation
    float distance    = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +
    light.quadratic * (distance * distance));

    //spotlight intensity
    float theta     = dot(lightDir, normalize(-light.direction));
    float epsilon   = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);
 //combine results
    vec3 ambient = light.ambient * objColor;
    vec3 diffuse = light.diffuse * diff * objColor;
    vec3 specular = light.specular * spec * objColor;
    ambient  *= attenuation;
    diffuse  *= attenuation * intensity;
    specular *= attenuation * intensity;
    return (ambient + diffuse + specular);
    }
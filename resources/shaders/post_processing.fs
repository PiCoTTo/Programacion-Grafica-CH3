#version 330 core
in  vec2  TexCoords;
in  vec2  direction;
out vec4  color;
  
uniform sampler2D scene;
uniform vec2      offsets[9];
uniform int       edge_kernel[9];
uniform float     blur_kernel[9];

uniform bool chaos;
uniform bool confuse;
uniform bool shake;
uniform bool poison;

void main()
{
    color = vec4(0.0f);
    vec3 sample[9];
    // sample from texture offsets if using convolution matrix
    if(chaos || shake)
        for(int i = 0; i < 9; i++)
            sample[i] = vec3(texture(scene, TexCoords.st + offsets[i]));

    // process effects
    if (chaos)
    {           
        for(int i = 0; i < 9; i++)
            color += vec4(sample[i] * edge_kernel[i], 0.0f);
        color.a = 1.0f;
    }
    else if (confuse)
    {
        color = vec4(1.0 - texture(scene, TexCoords).rgb, 1.0);
    }
    else if (shake)
    {
        for(int i = 0; i < 9; i++)
            color += vec4(sample[i] * blur_kernel[i], 0.0f);
        color.a = 1.0f;
    }
    else if (poison)
    {
        float redOffset   =  0.018;
        float greenOffset =  0.012;
        float blueOffset  = -0.012;
        
        vec2 texSize  = textureSize(scene, 0).xy;
        vec2 texCoord = gl_FragCoord.xy / texSize;
        
        color = texture(scene, texCoord);
        
        color.r = texture(scene, texCoord + (direction * vec2(redOffset  ))).r;
        color.g = texture(scene, texCoord + (direction * vec2(greenOffset))).g;
        color.b = texture(scene, texCoord + (direction * vec2(blueOffset ))).b;
    }
    else
    {
        color =  texture(scene, TexCoords);
    }
}
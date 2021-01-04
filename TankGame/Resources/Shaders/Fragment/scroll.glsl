uniform sampler2D textureSample;
uniform vec2 resolution;
uniform float intensityX;
uniform float intensityY;
uniform float time;

void main()
{
    vec2 texCoord;
    texCoord.x = fract((gl_FragCoord.x / resolution.x) + (time * intensityX));
    texCoord.y = fract((gl_FragCoord.y / resolution.y) + (time * intensityY));
    gl_FragColor = texture(textureSample, texCoord);
}
float xRandomValue = 0.0f;

// Noise strength (0 = no effect, 1 = full effect)
float xFilmGrainStrength = 0.2f;

// This accentuates the noise in the dark values. Use values greater than 1.
float xAccentuateDarkNoisePower = 1;

// The noise is both, random and static. With this we can accentuate or reduce the random noise.
// 1 is half random and half static, 0 is only static and more than 1 accentuate the random noise.
float xRandomNoiseStrength = 2;

float3 FilmGrain(float3 color, float2 uv)
{		
	//// The noise is a modification of the noise algorithm of Pat 'Hawthorne' Shearon.
	// Static noise
	float x = uv.x * uv.y * 50000;
	x = fmod(x, 13);
	x = x * x;
	float dx = fmod(x, 0.01);
	
	// Random noise
	float y = x * xRandomValue + xRandomValue;
	float dy = fmod(y, 0.01);
	
	// Noise
	float noise = saturate(0.1f + dx * 100) + saturate(0.1f + dy * 100) * xRandomNoiseStrength;
	
	// I want to maintain more or less the same luminance of the original color and right now the noise range is between 0 and 1.
	// If the range is changed to -1 to 1 some values will add luminance and some other will subtract.
	noise = noise * 2 - 1;
	
	// This accentuates the noise in the dark values. A dark color will give a number closer to 1 and a bright one will give a value closer to 0.
	float accentuateDarkNoise = pow(1 - (color. r + color.g + color.b) / 3, xAccentuateDarkNoisePower);
	
	// Color with noise	
	return color + color * noise * accentuateDarkNoise * xFilmGrainStrength;	
} // FilmGrain
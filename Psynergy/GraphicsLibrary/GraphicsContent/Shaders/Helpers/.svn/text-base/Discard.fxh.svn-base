float3 Discard()
{
	// discard' doesn't actually exit the shader (the shader will continue to execute). 
	// It merely instructs the output merger stage not to output the result of the pixel (which must still be returned by the shader).
	// The pair discard-return works!!! And you should be use it. However, the xbox 360 doesn't support it.
		#ifndef XBOX360
			discard;
		#endif
		return float4(0, 0, 0, 0);
} // Discard
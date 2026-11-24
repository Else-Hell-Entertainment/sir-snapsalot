shader_type canvas_item;
render_mode blend_mix;

const float BASE_PERIOD = 2.0 * PI;
const float VERTICAL_SHIFT = 0.5;

/**
 * Frequency of the flashing in flashes/s.
 */
uniform float frequency:hint_range(0.0, 100) = 0.4;

/**
 * Margin for the opacity between peaks and min/max values.
 * For example, if this is 0.25 the maximum opacity will be 1 - 0.25 = 0.75 and similarly the minimum opacity will be 0 + 0.25 = 0.25.
 */
uniform float peak_margin:hint_range(0.0, 0.5) = 0.25;

void vertex() {
	float period_multiplier = frequency * BASE_PERIOD;
	float amplitude_multiplier = VERTICAL_SHIFT - peak_margin;
	COLOR.a = amplitude_multiplier * cos(period_multiplier * TIME) + VERTICAL_SHIFT;
}

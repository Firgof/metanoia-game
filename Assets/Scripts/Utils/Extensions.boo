import UnityEngine


[Extension]
static def Rotate(vector as Vector2, angle as single):
	angle = angle % 360
	vec = Quaternion.AngleAxis(-angle, Vector3.forward) * vector
	return Vector2(vec.x, vec.y)


[Extension]
static def Rotate(vector as Vector3, angle as single, axis as Vector3):
	angle = angle % 360
	return Quaternion.AngleAxis(-angle, axis) * vector


[Extension]	
static def RGBToHSV (RGBColor as Color):
	R = RGBColor.r
	G = RGBColor.g
	B = RGBColor.b

	H as single
	S as single
	V as single
	
	minRGB = Mathf.Min(R, Mathf.Min(G, B))
	maxRGB = Mathf.Max(R, Mathf.Max(G, B));

	if minRGB == maxRGB:
		computedV = minRGB
		return Color(0, 0, computedV, RGBColor.a)

	if R == minRGB:
		d = G - B
	elif B == minRGB:
		d = R - G
	else:
		d = B - R
		
	if R == minRGB:
		h = 3
	elif B == minRGB:
		h = 1
	else:
		h = 5
		
	H = (60 * (h - d / (maxRGB - minRGB))) / 360
	S = (maxRGB - minRGB) / maxRGB
	V = maxRGB
	
	return Color(H, S, V, RGBColor.a)


[Extension]
static def HSVToRGB (HSVColor as Color):
	H = HSVColor.r
	S = HSVColor.g
	V = HSVColor.b
	
	R as single
	G as single
	B as single
	
	maxHSV = 255 * V
	minHSV = maxHSV * (1 - S)
	
	h = H * 360
	z = (maxHSV - minHSV) * (1 - Mathf.Abs((h / 60) % 2 - 1))
	
	if 0 <= h and h < 60:
		R = maxHSV
		G = z + minHSV
		B = minHSV
	elif 60 <= h and h < 120:
		R = z + minHSV
		G = maxHSV
		B = minHSV		
	elif 120 <= h and h < 180:
		R = minHSV
		G = maxHSV
		B = z + minHSV		
	elif 180 <= h and h < 240:
		R = minHSV
		G = z + minHSV
		B = maxHSV		
	elif 240 <= h and h < 300:
		R = z + minHSV
		G = minHSV
		B = maxHSV		
	elif 300 <= h and h < 360:
		R = maxHSV
		G = minHSV
		B = z + minHSV
				
	return Color(R / 255, G / 255, B / 255, HSVColor.a)
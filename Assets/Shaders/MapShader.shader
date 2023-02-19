Shader "GRD/PirateGame/MapShader"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}

		_IslandOutlineColor("Island Outline Color", Color) = (1,1,1,1)
		_IslandOutlineValue("Island Outline Value", Range(0,1)) = 0.1
		_IslandWetSandColor("Island Wet Sand Color", Color) = (1, 1, 1, 1)
		_IslandWetSandValue("Island Wet Sand Value", Range(0, 1)) = 0.12
		_IslandDrySandColor("Island Dry Sand Color", Color) = (1, 1, 1, 1)
		_IslandDrySandValue("Island Dry Sand Value", Range(0, 1)) = 0.4
		_IslandGrassColor("Island Grass Color", Color) = (1, 1, 1, 1)
		_IslandGrassValue("Island Grass Value", Range(0, 1)) = 0.7
		_MinIslandDisplacement("Min Island Displacement", Range(-1, 0)) = -0.1
		_MaxIslandDisplacement("Max Island Displacement", Range(0, 1)) = 0.1
		_IslandGrassDetailColor("Island Grass Detail Color", Color) = (1, 1, 1, 1)
		_GrassDetailSmoothstepMin("Grass Smoothstep A", Range(0,1)) = 0
		_GrassDetailSmoothstepMax("Grass Smoothstep B", Range(0,1)) = 1

		[Space(10)]
		_RockOutlineColor("Rock Outline Color", Color) = (1,1,1,1)
		_RockOutlineValue("Rock Outline Value", Range(0,1)) = 0.1
		_RockLateralColor1("Rock Lateral Color 1", Color) = (1,1,1,1)
		_RockLateralColor2("Rock Lateral Color 2", Color) = (1,1,1,1)
		_RockLateralColor3("Rock Lateral Color 3", Color) = (1,1,1,1)
		_RockLateralValue("Rock Lateral Value", Range(0,1)) = 0.12
		_RockTopColor("Rock Top Color", Color) = (1,1,1,1)
		_RockTopValue("Rock Top Value", Range(0,1)) = 0.7
		_MinRockDisplacement("Min Rock Displacement", Range(-1, 0)) = -0.1
		_MaxRockDisplacement("Max Rock Displacement", Range(0, 1)) = 0.1

		[Space(10)]
		_WaterColor("Water Color", Color) = (1,1,1,1)
		_DeepWaterColor("DeepWater Color", Color) = (1,1,1,1)
		_WaterSecondaryColor("Water Secondary Color", Color) = (1,1,1,1)
		_DeepWaterSecondaryColor("DeepWater Secondary Color", Color) = (1,1,1,1)
		_WaterDetailSize("Water Detail Size", float) = 100
		_WaterDetailSpeed("Water Detail Speed", float) = 1
		_WavesSpeed("Waves Speed", float) = 1
		_WaveSize("Wave Size", float) = 0.05

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			struct MapElement
			{
				fixed2 position;
				fixed radius;
			};

			uniform fixed3 _islandData[256];
			uint _islandCount;
			uniform fixed3 _rockData[256];
			uint _rockCount;
			fixed _mapSize;

			fixed4 _IslandOutlineColor;
			fixed _IslandOutlineValue;
			fixed4 _IslandWetSandColor;
			fixed _IslandWetSandValue;
			fixed4 _IslandDrySandColor;
			fixed _IslandDrySandValue;
			fixed4 _IslandGrassColor;
			fixed _IslandGrassValue;
			sampler2D  _IslandDisplacementTex;
			fixed _MinIslandDisplacement;
			fixed _MaxIslandDisplacement;
			sampler2D _IslandGrassDetailTex;
			fixed4 _IslandGrassDetailColor;
			fixed _GrassDetailSmoothstepMin;
			fixed _GrassDetailSmoothstepMax;

			fixed4 _RockOutlineColor;
			fixed _RockOutlineValue;
			fixed4 _RockLateralColor1;
			fixed4 _RockLateralColor2;
			fixed4 _RockLateralColor3;
			fixed _RockLateralValue;
			fixed4 _RockTopColor;
			fixed _RockTopValue;
			sampler2D _RockDisplacementTex;
			fixed _MinRockDisplacement;
			fixed _MaxRockDisplacement;

			fixed4 _WaterColor;
			fixed4 _DeepWaterColor;
			fixed4 _WaterSecondaryColor;
			fixed4 _DeepWaterSecondaryColor;
			fixed _WaterDetailSize;
			fixed _WaterDetailSpeed;
			fixed _WavesSpeed;
			fixed _WaveSize;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed invLerp(fixed from, fixed to, fixed value)
			{
				return (value - from) / (to - from);
			}

			fixed4 defineIslandColor(
				fixed value, fixed waveValue,
				fixed4 outlineColor = fixed4(0,0,0,0), fixed outlineValue = 1,
				fixed4 wetSandColor = fixed4(0, 0, 0, 0), fixed wetSandValue = 1,
				fixed4 drySandColor = fixed4(0, 0, 0, 0), fixed drySandValue = 1,
				fixed4 grassColor = fixed4(0, 0, 0, 0), fixed grassValue = 1, 
				fixed4 grassDetailColor = fixed4(0, 0, 0, 0), fixed grassDetailValue = 0)
			{
				if (value >= grassValue) 
				{
					return lerp(grassColor, grassDetailColor, grassDetailValue);
				}
				if (value >= drySandValue)
					return drySandColor;
				if (value >= wetSandValue + waveValue)
					return wetSandColor;
				if (value >= outlineValue + waveValue)
					return outlineColor;

				return fixed4(0, 0, 0, 0);
			}

			fixed4 defineRockColor(
				fixed value, fixed waveValue,
				fixed4 outlineColor = fixed4(0, 0, 0, 0), fixed outlineValue = 1,
				fixed4 lateralColor1 = fixed4(0, 0, 0, 0),
				fixed4 lateralColor2 = fixed4(0, 0, 0, 0),
				fixed4 lateralColor3 = fixed4(0, 0, 0, 0),
				fixed2 normal = fixed2(0, 0),
				fixed lateralValue = 1,
				fixed4 topColor = fixed4(0, 0, 0, 0), fixed topValue = 1)
			{
				if (value >= topValue)
					return topColor;
				if (value >= lateralValue + waveValue / 4)
				{
					fixed normalDotProd = dot(fixed2(0, -1), normal);
					if (normalDotProd > 0.5)
					{
						return lateralColor1;
					}
					normalDotProd = dot(fixed2(1, 0), normal);
					if (normalDotProd > 0) 
					{
						return lateralColor2;
					}
					return lateralColor3;
				}
				if (value >= outlineValue + waveValue / 4)
					return outlineColor;

				return fixed4(0, 0, 0, 0);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				fixed islandValue = 0;
				fixed rockValue = 0;
				fixed2 rockNormal = fixed2(0, 0);

				for(uint islandIndex = 0; islandIndex < _islandCount; islandIndex++)
				{
					fixed currentValue = invLerp(
						_islandData[islandIndex].z, 0,
						distance(_islandData[islandIndex].xy, i.uv) * _mapSize);
					currentValue += lerp(_MinIslandDisplacement, _MaxIslandDisplacement, tex2D(_IslandDisplacementTex, i.uv));
					islandValue = max(currentValue, islandValue);
				}
				for (uint rockIndex = 0; rockIndex < _rockCount; rockIndex++)
				{
					fixed currentValue = invLerp(
						_rockData[rockIndex].z, 0,
						distance(_rockData[rockIndex].xy, i.uv) * _mapSize);
					currentValue += lerp(_MinRockDisplacement, _MaxRockDisplacement, tex2D
						(_RockDisplacementTex, i.uv));

					if (currentValue > rockValue)
					{
						rockValue = currentValue, rockValue;
						rockNormal = normalize(i.uv - _rockData[rockIndex].xy);
					}
				}

				fixed waveValue = (_SinTime.w * _WavesSpeed + 1) / 2 * _WaveSize;

				fixed grassDetailValue = smoothstep(
					_GrassDetailSmoothstepMin, 
					_GrassDetailSmoothstepMax, 
					tex2D(_IslandGrassDetailTex, i.uv).r);
				fixed4 islandColor = defineIslandColor(
					islandValue, waveValue,
					_IslandOutlineColor, _IslandOutlineValue,
					_IslandWetSandColor, _IslandWetSandValue,
					_IslandDrySandColor, _IslandDrySandValue,
					_IslandGrassColor, _IslandGrassValue,
					_IslandGrassDetailColor, grassDetailValue);

				fixed4 rockColor = defineRockColor(
					rockValue, waveValue,
					_RockOutlineColor, _RockOutlineValue,
					_RockLateralColor1,
					_RockLateralColor2,
					_RockLateralColor3,
					rockNormal,
					_RockLateralValue,
					_RockTopColor, _RockTopValue);

				fixed uvx = i.uv.x  * _WaterDetailSize;
				fixed uvy = i.uv.y  * _WaterDetailSize;
				fixed angle = -0.6;
				fixed secondaryWaterColorValue = (sin(uvx * cos(angle) - uvy * sin(angle)) - uvx * sin(angle)) / cos(angle) - uvy + _WaterDetailSize + _Time.y * _WaterDetailSpeed;
				secondaryWaterColorValue = step(1, secondaryWaterColorValue % 2);

				fixed4 waterColor = lerp(_WaterColor, _WaterSecondaryColor, secondaryWaterColorValue);
				if (islandValue - waveValue <= 0 && rockValue - waveValue <= 0)
				{
					waterColor = lerp(_DeepWaterColor, _DeepWaterSecondaryColor, secondaryWaterColorValue);
				}
				
				fixed4 col = lerp(islandColor, rockColor, rockColor.a);
				col = lerp(waterColor, col, col.a);

                return col;
            }
            ENDCG
        }
    }
}

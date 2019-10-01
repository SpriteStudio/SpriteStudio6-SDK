/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
#if !_FOR_SS6CONVERTER_
using UnityEngine;
#else
using SS6ConverterVer2_DLL;
using SS6ConverterVer2_DLL.Types;
using UnityEngine = SS6ConverterVer2_DLL.Assets;
#endif

public static partial class Library_SpriteStudio6
{
	public static partial class Utility
	{
		public static partial class Random
		{
			/* ----------------------------------------------- Classes, Structs & Interfaces */
			#region Classes, Structs & Interfaces
			public class XorShift32 : Library_SpriteStudio6.Utility.Random.Generator
			{
				/* ----------------------------------------------- Variables & Properties */
				#region Variables & Properties
				protected uint SeedNow;
				#endregion Variables & Properties

				/* ----------------------------------------------- Functions */
				#region Functions
				public XorShift32()
				{
					SeedNow = 0;
				}

				public void init_genrand(uint seed)
				{
					SeedNow = seed;
				}

				public uint genrand_uint32()
				{
					SeedNow = SeedNow ^ (SeedNow << 13);
					SeedNow = SeedNow ^ (SeedNow >> 17);
					SeedNow = SeedNow ^ (SeedNow << 15);

					return(SeedNow);
				}

				public float genrand_float32()
				{
					return((float)((genrand_uint32() >> 9) & 0x007fffff) * (1.0f / 8388607.0f));
				}

				/* generates a random floating point number on [0,1] */
				public double genrand_real1()
				{
					return(genrand_uint32() * (1.0 / 4294967295.0));	/* divided by 2^32-1 */
				}

				/* generates a random floating point number on [0,1] */
				public double genrand_real2()
				{
					return(genrand_uint32() * (1.0 / 4294967296.0));	/* divided by 2^32 */
				}

				/* generates a random integer number from 0 to N-1 */
				public int genrand_N(int iN)
				{
					return((int)(genrand_uint32() * (iN / 4294967296.0)));
				}

				/* Interfaces */
				public uint[] ListSeed
				{
					get
					{
						return(null);
					}
				}
				public void InitSeed(uint seed)
				{
					init_genrand(seed);
				}

				public uint RandomUint32()
				{
					return(genrand_uint32());
				}

				public double RandomDouble(double valueMax)
				{
					return((genrand_uint32() * (valueMax / 4294967296.0)));
				}

				public float RandomFloat(float valueMax)
				{
//					return((float)(genrand_uint32() * (valueMax / 4294967296.0f)));	/* 8388607.0f */
//					return((float)(genrand_uint32() * (valueMax * (1.0f/ 4294967296.0f))));	/* 8388607.0f */
					SeedNow = SeedNow ^ (SeedNow << 13);
					SeedNow = SeedNow ^ (SeedNow >> 17);
					SeedNow = SeedNow ^ (SeedNow << 15);
					return((float)SeedNow * (valueMax * (1.0f/ 4294967296.0f)));
				}

				public int RandomN(int valueMax)
				{
					return(genrand_N(valueMax));
				}
				#endregion Functions
			}
			#endregion Classes, Structs & Interfaces
		}
	}
}

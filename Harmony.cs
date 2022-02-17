namespace MoreBirds
{
	public class Harmony_Main
	{

		[HarmonyLib.HarmonyPatch(typeof(Placemaker.Life.BirdFlock), "OnUpdate")]
		public class TextureBirds
		{

			public static void Postfix(ref Placemaker.Life.BirdFlock __instance)
			{
                if (MoreBirdsMain.NPressed)
                {
                    MoreBirdsMain.ColorBirds();
                    MoreBirdsMain.NPressed = false;
                }
                if (MoreBirdsMain.CreateMultiBirds())
                {
                    MoreBirdsMain.UpdateBirds();
                }

            }
		}

        [HarmonyLib.HarmonyPatch(typeof(Placemaker.Life.BirdFlock), "IterateBirdCreation")]
        public class ColliderCheck
        {

            public static void Postfix(ref Placemaker.Life.BirdFlock __instance)
            {
                MoreBirdsMain.colliderUpToDate = false;
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Placemaker.Life.BirdFlock), "OnReset")]
		public class ClearBirds
		{

			public static void Postfix(ref Placemaker.Life.BirdFlock __instance)
			{
                if (MoreBirdsMain.CreateMultiBirds())
                {
                    MoreBirdsMain.ResetBirds();
                }
            }
		}
	}
}

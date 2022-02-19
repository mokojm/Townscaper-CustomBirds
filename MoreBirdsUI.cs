using MelonLoader;
using UnityEngine;
using ModUI;
using System;
using TMPro;
using UnityEngine.UI;

namespace MoreBirds
{
	public static class BirdsUI
	{
		public static MelonMod myMod;
		public static ModSettings myModSettings;
		public static Slider referencedSlider;

		public static Slider refBirdSlider1;
		public static Slider refBirdSlider2;
		public static Slider refBirdSlider3;
		public static Slider refBirdSlider4;
		public static Slider refBirdSlider5;

		public static bool isInitialized;

		public static void Initialize(MelonMod thisMod)
		{

			myModSettings = UIManager.Register(thisMod, new Color32(199, 243, 182, 255));

			myModSettings.AddButton("Paint", "General", new Color32(199, 243, 182, 255), new Action(delegate { MyPaintBirdButton(); }));
			myModSettings.AddButton("Reset", "General", new Color32(255, 179, 174, 255), new Action(delegate { MyResetButton(); }));
			

			myModSettings.AddSlider("Bird 1", "General", new Color32(119, 206, 224, 255), 0f, 1f, false, (float)MoreBirdsMain.percentBirds[0], new Action<float>(delegate (float value) { Populate(0, value); }));
			myModSettings.AddSlider("Bird 2", "General", new Color32(119, 206, 224, 255), 0f, 1f, false, (float)MoreBirdsMain.percentBirds[1], new Action<float>(delegate (float value) { Populate(1, value); }));
			myModSettings.AddSlider("Bird 3", "General", new Color32(119, 206, 224, 255), 0f, 1f, false, (float)MoreBirdsMain.percentBirds[2], new Action<float>(delegate (float value) { Populate(2, value); }));
			myModSettings.AddSlider("Bird 4", "General", new Color32(119, 206, 224, 255), 0f, 1f, false, (float)MoreBirdsMain.percentBirds[3], new Action<float>(delegate (float value) { Populate(3, value); }));
			myModSettings.AddSlider("Bird 5", "General", new Color32(119, 206, 224, 255), 0f, 1f, false, (float)MoreBirdsMain.percentBirds[4], new Action<float>(delegate (float value) { Populate(4, value); }));
			
			refBirdSlider1 = myModSettings.controlSliders["Bird 1"].GetComponent<Slider>();
			refBirdSlider2 = myModSettings.controlSliders["Bird 2"].GetComponent<Slider>();
			refBirdSlider3 = myModSettings.controlSliders["Bird 3"].GetComponent<Slider>();
			refBirdSlider4 = myModSettings.controlSliders["Bird 4"].GetComponent<Slider>();
			refBirdSlider5 = myModSettings.controlSliders["Bird 5"].GetComponent<Slider>();

			myModSettings.AddToggle("Fixed Nb Birds", "General", new Color32(243, 227, 182, 255), false, new Action<bool>(delegate (bool value) { MyToggleAction(value); }));
			myModSettings.AddSlider("Amount of Birds", "General", new Color32(243, 227, 182, 255), 1f, 254f, true, 10f, new Action<float>(delegate (float value) { AddBirds(value); }));
			referencedSlider = myModSettings.controlSliders["Amount of Birds"].GetComponent<Slider>();
			
			myModSettings.AddKeybind("Color bird", "Input", KeyCode.N, new Color32(10, 190, 124, 255));
            myModSettings.AddKeybind("Tease bird", "Input", KeyCode.B, new Color32(10, 190, 124, 255));
            myModSettings.AddKeybind("Tease ALL", "Input", KeyCode.V, new Color32(10, 190, 124, 255));


            //Apply button
            myModSettings.AddButton("Apply", "General", new Color32(243, 227, 182, 255), new Action(delegate { Apply(); }));

            isInitialized = true;
		}

		public static void Apply()
		{
			myModSettings.GetValueKeyCode("Color bird", "Input", out MoreBirdsMain.ColorBirdKey);
			myModSettings.GetValueKeyCode("Tease bird", "Input", out MoreBirdsMain.UpRootBirdKey);
			myModSettings.GetValueKeyCode("Tease ALL", "Input", out MoreBirdsMain.UpRootAllBirdsKey);
		}

		public static void MyPaintBirdButton()
		{
			MoreBirdsMain.ResetBirds();
			MoreBirdsMain.GetBirds();
		}

		public static void Populate(int index, float value)
        {
			MoreBirdsMain.Populate(index, value);
			UpdateBirdPopSliders();
        }
		public static void UpdateBirdPopSliders()
		{
			if (isInitialized)
            {
				refBirdSlider1.value = (float)MoreBirdsMain.percentBirds[0];
				refBirdSlider2.value = (float)MoreBirdsMain.percentBirds[1];
				refBirdSlider3.value = (float)MoreBirdsMain.percentBirds[2];
				refBirdSlider4.value = (float)MoreBirdsMain.percentBirds[3];
				refBirdSlider5.value = (float)MoreBirdsMain.percentBirds[4];
			}
		}

		public static void MyBoxBirdButton()
		{
			MoreBirdsMain.BoxBirdCreate();
		}

		public static void MyResetButton()
		{
			MoreBirdsMain.ResetBirds();
		}

		public static void AddBirds(float value)
        {
			//MelonLogger.Msg(value);
			if (isInitialized)
            {
				MoreBirdsMain.AddBirds((int)value);
				referencedSlider.value = (float)MoreBirdsMain.myAmountBirds;
			}	
		}

		public static void MyToggleAction(bool choice)
        {
			MoreBirdsMain.ControlBirds(choice);

		}
	}
}

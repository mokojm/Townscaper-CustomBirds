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
			// Create new ModSettings instance
			//
			// [Argument]			[Type]			[Description]
			// [thisMod]			MelonMod		Reference to your mod instance
			// [ButtonColor]		Color32			Color of main mod button in the UI
			//
			// Return:
			// [Type]				[Description]
			// ModSettings			Newly created ModSettings instance. Used to access most methods.
			//
			// Comment:
			// ModSettings instance should not be created before the main scene "Placemaker" is loaded. 
			// Race conditions need to be observed. When in doubt, check if bool "UIManager.isInitialized" is true
			//
			// Example:
			ModSettings myModSettings = UIManager.Register(thisMod, new Color32(199, 243, 182, 255));


			// Create new button
			//
			// [Argument]			[Type]			[Description]
			// [name]				String			Text on button; also used as identifier; must be unique!!!
			// [section]			String			Menu subsection / Settings section; also used as identifier
			// [buttonColor]		Color32			Color of button
			// [newAction]			Action			Action to call on button press

			// Comment:
			// Action delegate is allowed to be empty, eg. "new Action(delegate {})"				
			//
			// Example:
			myModSettings.AddButton("Paint", "General", new Color32(199, 243, 182, 255), new Action(delegate { MyPaintBirdButton(); }));
			myModSettings.AddButton("Reset", "General", new Color32(255, 179, 174, 255), new Action(delegate { MyResetButton(); }));
			//myModSettings.AddButton("Box", "General", new Color32(255, 179, 174, 255), new Action(delegate { MyBoxBirdButton(); }));


			// Create new number slider
			//
			// [Argument]		[Type]			[Description]
			// [name]			String			Name of slider; also used as identifier; must be unique!!!
			// [section]		String			Menu subsection / Settings section; also used as identifier
			// [sliderColor]	Color32			Color of slider
			// [minValue]		Float			Minimum value of slider
			// [maxValue]		Float			Maximum value of slider
			// [wholeNumbers]	Bool			If true, only integer numbers are selectable
			// [defaultValue]	Float			Default value of slider if not present in settings file
			// [newAction]		Action			Action to call value change; usually "new Action<float>(delegate (float value) { MethodToCallOnSliderChange(value); })"
			//
			// Example:
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
			
			// Create new inputfield
			//
			// [Argument]		[Type]								[Description]
			// [name]			String								Name of inputfield; also used as identifier; must be unique!!!
			// [section]		String								Menu subsection / Settings section; also used as identifier
			// [fieldColor]		Color32								Color of inputfield
			// [contentType]	TMP_InputField.ContentType			Content type enum of inputfield; allows restricted input, eg. only numbers
			// [defaultValue]	String								Default value of inputfield if not present in settings file
			// [newAction]		Action								Action to call on edit end; usually "new Action<string>(delegate (string value) { MethodToCallOnSliderChange(value); })"
			//
			// Example:
			//myModSettings.AddInputField("Birds 1", "Population", new Color32(255, 179, 174, 255), TMP_InputField.ContentType.IntegerNumber, "20", new Action<string>(delegate (string value) { InputBirdPop(1, value); }));
			//myModSettings.AddInputField("Birds 2", "Population", new Color32(255, 179, 174, 255), TMP_InputField.ContentType.IntegerNumber, "20", new Action<string>(delegate (string value) { InputBirdPop(2, value); }));

			// Create new toggle
			//
			// [Argument]		[Type]								[Description]
			// [name]			String								Name of toggle; also used as identifier; must be unique!!!
			// [section]		String								Menu subsection / Settings section; also used as identifier
			// [toggleColor]	Color32								Color of toggle
			// [defaultValue]	Bool								Default value of toggle if not present in settings file
			// [newAction]		Action								Action to call on edit end; usually "new Action<bool>(delegate (bool value) { MethodToCallOnSliderChange(value); })"
			//
			// Example:


			// Create new keybind field
			//
			// [Argument]		[Type]					[Description]
			// [name]			String					Name of keybind; also used as identifier; must be unique!!!
			// [section]		String					Menu subsection / Settings section; also used as identifier
			// [defaultValue]	UniyEngine.KeyCode		Default value of keybind if not present in settings file
			// [keybindColor]	Color32					Color of keybind
			//
			// Example:
			myModSettings.AddKeybind("Color bird", "Input", KeyCode.N, new Color32(10, 190, 124, 255));

			myModSettings.GetValueKeyCode("Color bird", "Input", out KeyCode myKeyCode);
			MoreBirdsMain.ColorBirdKey = myKeyCode;

			myModSettings.AddKeybind("Tease bird", "Input", KeyCode.B, new Color32(10, 190, 124, 255));

			myModSettings.GetValueKeyCode("Tease bird", "Input", out KeyCode myKeyCode2);
			MoreBirdsMain.UpRootBirdKey = myKeyCode2;

			myModSettings.AddKeybind("Tease ALL", "Input", KeyCode.V, new Color32(10, 190, 124, 255));

			myModSettings.GetValueKeyCode("Tease ALL", "Input", out KeyCode myKeyCode3);
			MoreBirdsMain.UpRootAllBirdsKey = myKeyCode3;

			isInitialized = true;
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
			refBirdSlider1.value = (float)MoreBirdsMain.percentBirds[0];
			refBirdSlider2.value = (float)MoreBirdsMain.percentBirds[1];
			refBirdSlider3.value = (float)MoreBirdsMain.percentBirds[2];
			refBirdSlider4.value = (float)MoreBirdsMain.percentBirds[3];
			refBirdSlider5.value = (float)MoreBirdsMain.percentBirds[4];

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
			MelonLogger.Msg(value);
			if (UIManager.isInitialized)
            {
				MoreBirdsMain.AddBirds((int)value);
				referencedSlider.value = (float)MoreBirdsMain.myAmountBirds;
			}	
		}

		public static void MyToggleAction(bool choice)
        {
			MoreBirdsMain.ControlBirds(choice);

		}

		public static void InputBirdPop(int index, string value)
		{
			
		}

		/*public static void MySliderActionExample(float value)
		{
			MelonLogger.Msg("Slider value changed to: " + value);
		}

		public static void MyInputActionExample(string value)
		{
			MelonLogger.Msg("Input value changed to: " + value);
		}
		public static void MyToggleActionExample(bool value)
		{
			MelonLogger.Msg("Toggle value changed to: " + value);
		}*/
	}
}

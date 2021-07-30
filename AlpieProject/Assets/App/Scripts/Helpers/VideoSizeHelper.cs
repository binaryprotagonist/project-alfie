// using System;
using UnityEngine;
// using UnityEngine.iOS;
// using UnityEngine.UI;

public class VideoSizeHelper : MonoBehaviour
{
	// [Header("Parameters")]
	// [SerializeField] private float iPhoneWidth;
	// [SerializeField] private float iPhoneHeight;
	//
	// [Space]
	// [SerializeField] private float iPadWidth;
	// [SerializeField] private float iPadHeight;
	//
	// [Header("Links")]
	// [SerializeField] private RawImage videoRawImage;
	//
	// void Start ()
	// {
	// 	string deviceModel = SystemInfo.deviceModel.ToLower ();
	//
	// 	DeviceGeneration deviceGeneration = Device.generation;
	// 	Debug.Log ("deviceGeneration = " + deviceGeneration);
	// 	
	// 	switch (deviceGeneration)
	// 	{
	// 		case DeviceGeneration.Unknown:
	// 			break;
	// 		case DeviceGeneration.iPhone:
	// 			break;
	// 		// case DeviceGeneration.iPhone3G:
	// 		// 	break;
	// 		// case DeviceGeneration.iPhone3GS:
	// 		// 	break;
	// 		// case DeviceGeneration.iPad1Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPhone4:
	// 		// 	break;
	// 		// case DeviceGeneration.iPad2Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPhone4S:
	// 		// 	break;
	// 		// case DeviceGeneration.iPad3Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPhone5:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadMini1Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPad4Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPhone5C:
	// 		// 	break;
	// 		// case DeviceGeneration.iPhone5S:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadAir1:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadMini2Gen:
	// 		// 	break;
	// 		case DeviceGeneration.iPhone6:
	// 			break;
	// 		case DeviceGeneration.iPhone6Plus:
	// 			break;
	// 		// case DeviceGeneration.iPadMini3Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadAir2:
	// 		// 	break;
	// 		case DeviceGeneration.iPhone6S:
	// 			break;
	// 		case DeviceGeneration.iPhone6SPlus:
	// 			break;
	// 		// case DeviceGeneration.iPadMini4Gen:
	// 		// 	break;
	// 		case DeviceGeneration.iPhoneSE1Gen:
	// 			break;
	// 		// case DeviceGeneration.iPadPro10Inch1Gen:
	// 		// 	break;
	// 		case DeviceGeneration.iPhone7:
	// 			break;
	// 		case DeviceGeneration.iPhone7Plus:
	// 			break;
	// 		// case DeviceGeneration.iPad5Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadPro2Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadPro10Inch2Gen:
	// 		// 	break;
	// 		case DeviceGeneration.iPhone8:
	// 			break;
	// 		case DeviceGeneration.iPhone8Plus:
	// 			break;
	// 		case DeviceGeneration.iPhoneX:
	// 			break;
	// 		case DeviceGeneration.iPhoneXS:
	// 			break;
	// 		case DeviceGeneration.iPhoneXSMax:
	// 			break;
	// 		case DeviceGeneration.iPhoneXR:
	// 			break;
	// 		// case DeviceGeneration.iPadPro11Inch:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadPro3Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPad6Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadAir3Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadMini5Gen:
	// 			break;
	// 		case DeviceGeneration.iPhone11:
	// 			break;
	// 		case DeviceGeneration.iPhone11Pro:
	// 			break;
	// 		case DeviceGeneration.iPhone11ProMax:
	// 			break;
	// 		// case DeviceGeneration.iPad7Gen:
	// 		// 	break;
	// 		case DeviceGeneration.iPhoneSE2Gen:
	// 			break;
	// 		// case DeviceGeneration.iPadPro11Inch2Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadPro4Gen:
	// 		// 	break;
	// 		case DeviceGeneration.iPhone12Mini:
	// 			break;
	// 		case DeviceGeneration.iPhone12:
	// 			break;
	// 		case DeviceGeneration.iPhone12Pro:
	// 			break;
	// 		case DeviceGeneration.iPhone12ProMax:
	// 			break;
	// 		// case DeviceGeneration.iPad8Gen:
	// 		// 	break;
	// 		// case DeviceGeneration.iPadAir4Gen:
	// 		// 	break;
	// 		case DeviceGeneration.iPhoneUnknown:
	// 			break;
	// 		// case DeviceGeneration.iPadUnknown:
	// 		// 	break;
	// 		default:
	// 			throw new ArgumentOutOfRangeException ();
	// 	}
	// 	
	// 	if (deviceModel.Contains ("iphone"))
	// 	{
	// 		Debug.Log ("This is iPhone");
	// 		// videoRawImage.rectTransform.rect.width = iPhoneWidth;
	// 		// videoRawImage.rectTransform.rect.width = iPhoneHeight;
	//
	// 		videoRawImage.rectTransform.sizeDelta = new Vector2 (iPhoneWidth, iPhoneHeight);
	// 	}
	// 	else if (deviceModel.Contains ("ipad"))
	// 	{
	// 		Debug.Log ("This is iPad");
	// 		
	// 		videoRawImage.rectTransform.sizeDelta = new Vector2 (iPadWidth, iPadHeight);
	// 	}
	// }
}
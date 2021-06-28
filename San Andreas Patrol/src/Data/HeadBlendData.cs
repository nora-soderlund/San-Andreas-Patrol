using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Rage;
using Rage.Attributes;
using Rage.Native;

namespace SanAndreasPatrol {
    [StructLayout(LayoutKind.Explicit, Size = 80)]
    internal struct HeadBlendData {
        [FieldOffset(0)]
        public int shapeFirstID;

        [FieldOffset(8)]
        public int shapeSecondID;

        [FieldOffset(16)]
        public int shapeThirdID;

        [FieldOffset(24)]
        public int skinFirstID;

        [FieldOffset(32)]
        public int skinSecondID;

        [FieldOffset(40)]
        public int skinThirdID;

        [FieldOffset(48)]
        public float shapeMix;

        [FieldOffset(56)]
        public float skinMix;

        [FieldOffset(64)]
        public float thirdMix;

        [FieldOffset(75)]
        public bool isParent;
    }

    internal enum OverlayId {
        Blemishes,
        FacialHair,
        Eyebrows,
        Ageing,
        Makeup,
        Blush,
        Complexion,
        SunDamage,
        Lipstick,
        Freckles,
        ChestHair,
        BodyBlemishes,
        AddBodyBlemishes
    }

    internal enum ColorType {
        Other,
        EyebrowBeardChestHair,
        BlushLipstick
    }

    public enum FaceFeature {
        NoseWidth,
        NosePeakHeight,
        NosePeakLenght,
        NoseBoneHigh,
        NosePeakLowering,
        NoseBoneTwist,
        EyeBrownHigh,
        EyeBrownForward,
        CheeksBoneHigh,
        CheeksBoneWidth,
        CheeksWidth,
        EyesOpening,
        LipsThickness,
        JawBoneWidth,
        JawBoneLenght,
        ChimpBoneLowering,
        ChimpBoneLenght,
        ChimpBoneWidth,
        ChimpHole,
        NeckThickness,
    }

    internal static class HeadBlend {
        public static Dictionary<int, string> Fathers = new Dictionary<int, string>() {
            { 0, "Benjamin" },
            { 1, "Daniel" },
            { 2, "Joshua" },
            { 3, "Noah" },
            { 4, "Andrew" },
            { 5, "Juan" },
            { 6, "Alex" },
            { 7, "Isaac" },
            { 8, "Evan" },
            { 9, "Ethan" },
            { 10, "Vincent" },
            { 11, "Angel" },
            { 12, "Diego" },
            { 13, "Adrian" },
            { 14, "Gabriel" },
            { 15, "Michael" },
            { 16, "Santiago" },
            { 17, "Kevin" },
            { 18, "Louis" },
            { 19, "Samuel" },
            { 20, "Anthony" },
            { 42, "Claude" },
            { 43, "Niko" },
            { 44, "John" }
        };

        public static Dictionary<int, string> Mothers = new Dictionary<int, string>() {
            { 21, "Hannah" },
            { 22, "Aubrey" },
            { 23, "Jasmine" },
            { 24, "Gisele" },
            { 25, "Amelia" },
            { 26, "Isabella" },
            { 27, "Zoe" },
            { 28, "Ava" },
            { 29, "Camila" },
            { 30, "Violet" },
            { 31, "Sophia" },
            { 32, "Evelyn" },
            { 33, "Nicole" },
            { 34, "Ashley" },
            { 35, "Gracie" },
            { 36, "Brianna" },
            { 37, "Natalie" },
            { 38, "Olivia" },
            { 39, "Elizabeth" },
            { 40, "Charlotte" },
            { 41, "Emma" },
            { 45, "Misty" }
        };

        public static bool HasFinished(Ped ped) => NativeFunction.Natives.x654CD0A825161131<bool>(ped); //HAS_PED_HEAD_BLEND_FINISHED

        public static bool IsLipstickColorValid(int colorId) => NativeFunction.Natives.x0525A2C2562F3CD4(colorId); //_IS_PED_LIPSTICK_COLOR_VALID

        public static bool IsHairColorValid(int colorId) => NativeFunction.Natives.xE0D36E5D9E99CC21(colorId); //_IS_PED_HAIR_COLOR_VALID

        public static bool IsBlushColorValid(int colorId) => NativeFunction.Natives.x604E810189EE3A59(colorId); //_IS_PED_BLUSH_COLOR_VALID

        public static int MaxHairColors => NativeFunction.Natives.xE5C0CF872C2AD150(); //_GET_NUM_HAIR_COLORS

        public static int MaxMakeupColors => NativeFunction.Natives.xD1F7CA1535D22818(); //_GET_NUM_MAKEUP_COLORS

        public static HeadBlendData GetDataFromPed(Ped ped) {
            NativeFunction.Natives.x2746BD9D88C5C5D0(ped, out HeadBlendData result); //GET_PED_HEAD_BLEND_DATA
            return result;
        }

        public static void SetDataForPed(Ped ped, HeadBlendData headBlendData) {
            NativeFunction.Natives.x9414E18B9434C2FE(
                ped,
                headBlendData.shapeFirstID, headBlendData.shapeSecondID, headBlendData.shapeThirdID,
                headBlendData.skinFirstID, headBlendData.skinSecondID, headBlendData.skinThirdID,
                headBlendData.shapeMix, headBlendData.skinMix, headBlendData.thirdMix,
                headBlendData.isParent); //SET_PED_HEAD_BLEND_DATA
        }

        public static void UpdateDataForPed(Ped ped, float shapeMix, float skinMix, float thirdMix) =>
            NativeFunction.Natives.x723538F61C647C5A(ped, shapeMix, skinMix, thirdMix); //UPDATE_PED_HEAD_BLEND_DATA

        public static void SetEyeColor(Ped ped, int eyeColor) =>
            NativeFunction.Natives.x50B56988B170AFDF(ped, eyeColor); //_SET_PED_EYE_COLOR

        public static void SetHairColor(Ped ped, int colorId, int highlightColorId) =>
            NativeFunction.Natives.x4CFFC65454C93A49(ped, colorId, highlightColorId); //_SET_PED_HAIR_COLOR

        public static void SetFaceFeature(Ped ped, FaceFeature faceFeature, float scale) =>
            NativeFunction.Natives.x71A5C1DBA060049E(ped, (int)faceFeature, scale); //_SET_PED_FACE_FEATURE

        public static void SetHeadOverlay(Ped ped, OverlayId overlayId, int index, float opacity) =>
            NativeFunction.Natives.x48F44967FA05CC1E(ped, (int)overlayId, index, opacity); //SET_PED_HEAD_OVERLAY

        public static void SetHeadOverlayColor(Ped ped, OverlayId overlayId, ColorType colorType, int colorId, int highlightColorId) =>
            NativeFunction.Natives.x497BF74A7B9CB952(ped, (int)overlayId, (int)colorType, colorId, highlightColorId); //_SET_PED_HEAD_OVERLAY_COLOR
    }
}

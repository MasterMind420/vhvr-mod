using UnityEngine;
using ValheimVRMod.Scripts;
using ValheimVRMod.VRCore;
using ValheimVRMod.VRCore.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

namespace ValheimVRMod.Utilities {
    public static class Pose {
        
        public static bool toggleShowLeftHand = true;
        public static bool toggleShowRightHand = true;
        public static bool justUnsheathed;

        public static void checkInteractions()
        {
            
            if (Player.m_localPlayer == null || !VRControls.mainControlsActive)
            {
                return;
            }

            checkHandOverShoulder(true, VRPlayer.rightHand, SteamVR_Input_Sources.RightHand, ref toggleShowLeftHand);
            checkHandOverShoulder(false, VRPlayer.leftHand, SteamVR_Input_Sources.LeftHand, ref toggleShowRightHand);

        }
        
        private static void checkHandOverShoulder(bool isRightHand, Hand hand, SteamVR_Input_Sources inputSource, ref bool toggleShowHand)
        {
            var camera = CameraUtils.getCamera(CameraUtils.VR_CAMERA).transform;
            var action = SteamVR_Actions.valheim_Grab;
            if (camera.InverseTransformPoint(hand.transform.position).y > -0.4f &&
                camera.InverseTransformPoint(hand.transform.position).z < 0)
            {

                if (action.GetStateDown(inputSource))
                {

                    if(isRightHand && isHoldingItem(isRightHand) && isUnpressSheath()) {
                        return;
                    } 
                 
                    toggleShowHand = false;
                    hand.hapticAction.Execute(0, 0.2f, 100, 0.3f, inputSource);
                    
                    if (isRightHand && EquipScript.getLeft() == EquipType.Bow) {
                        BowLocalManager.instance.toggleArrow();
                    } else if (isHoldingItem(isRightHand)) {
                        Player.m_localPlayer.HideHandItems();
                    } else {
                        Player.m_localPlayer.ShowHandItems();
                        justUnsheathed = true;
                    }
                }
                else if (!justUnsheathed && isRightHand && action.GetStateUp(inputSource)) {
                    if (isHoldingItem(isRightHand) && isUnpressSheath()) {
                        Player.m_localPlayer.HideHandItems();
                    }
                }
            }
            if (justUnsheathed && isRightHand && action.GetStateUp(inputSource)&& isUnpressSheath()) {
                justUnsheathed = false;
            }
        }
        
        private static bool isHoldingItem(bool isRightHand) {
            return isRightHand && Player.m_localPlayer.GetRightItem() != null
                   || !isRightHand && Player.m_localPlayer.GetLeftItem() != null;
        }
        
        private static bool isUnpressSheath() {
            return isBuildingTool() 
                   || isHoldingThrowable();
        }
        private static bool isHoldingThrowable() {
            return EquipScript.getRight() == EquipType.Spear 
                   || EquipScript.getRight() == EquipType.SpearChitin 
                   || EquipScript.getRight() == EquipType.Fishing
                   || EquipScript.getRight() == EquipType.Tankard;
        }
        private static bool isBuildingTool() {
            return EquipScript.getRight() == EquipType.Hammer 
                   || EquipScript.getRight() == EquipType.Hoe 
                   || EquipScript.getRight() == EquipType.Cultivator;
        }
    }
}
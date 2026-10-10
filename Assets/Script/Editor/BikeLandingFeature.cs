using UnityEngine;
using UnityEngine.Events;

namespace ReVR.Vehicles.Bike
{
    public enum EBikeLanding
    {
        Perfect,
        Good,
        Hard,
        Crash
    }

    [AddComponentMenu("ReVR/Vehicles/Bike/Landing Feature")]
    public class BikeLandingFeature : BikeFeature
    {
        public float PerfectGraceTime = 0.25f;

        public float ExcessiveVelocity = 100f, SteepAngleDown = 60f;

        public Transform BikeRotatorTransform;

        private BikeBlackboard m_Blackboard;

        public bool bLandingFound, bRearFirst, bFrontFirst, bRearWheelGrounded, bFrontWheelGrounded;

        public float WheelTouchTime;

        public UnityEvent<EBikeLanding> LandingEvent;

        public override void OnAwake(BikeBlackboard blackboard)
        {
            m_Blackboard = blackboard;
            Debug.Log("Bike Landed in Awake");
        }

        public override void OnUpdate()
        {
            var wasGrounded = bRearWheelGrounded && bFrontWheelGrounded;

            bRearWheelGrounded = m_Blackboard.isRearWheelGrounded;
            bFrontWheelGrounded = m_Blackboard.isFrontWheelGrounded;

            var isGrounded = bRearWheelGrounded && bFrontWheelGrounded;

            // was on Ground
            if (wasGrounded)
            {
                bRearFirst = false;
                bFrontFirst = false;

                if (isGrounded) bLandingFound = false;
            }

            if (bLandingFound) return;

            if (!wasGrounded && (bRearWheelGrounded || bFrontWheelGrounded))
            {
                Debug.Log("Bike landed on ground");
                if (bRearWheelGrounded && bFrontWheelGrounded) // both wheels are grounded
                {
                    bRearFirst = true;
                    bFrontFirst = true;
                    LandingFound(EBikeLanding.Perfect);
                    return;
                }
                else if (bRearWheelGrounded && !bFrontWheelGrounded) bRearFirst = true;
                else if (bFrontWheelGrounded && !bRearWheelGrounded) bFrontFirst = true;

                if (bRearFirst || bFrontFirst) WheelTouchTime = Time.time;
            }

            var GraceTime = Time.time - WheelTouchTime;
            var bInGrace = GraceTime < PerfectGraceTime;

            if (bRearFirst && bFrontWheelGrounded)
            {
                var LandingEventFound = bInGrace ? EBikeLanding.Perfect : EBikeLanding.Good;
                LandingFound(LandingEventFound);
                return;
            }
            if (bFrontFirst && bRearWheelGrounded)
            {
                var CrashLandCond1 = m_Blackboard.localVelocity.magnitude > ExcessiveVelocity;
                var CrashLandCond2 = Vector3.Angle(m_Blackboard.frontWheelGroundNormal, BikeRotatorTransform.up) > SteepAngleDown;

                EBikeLanding LandingEventFound;
                if (CrashLandCond1 || CrashLandCond2)
                    LandingEventFound = EBikeLanding.Crash;
                else
                    LandingEventFound = bInGrace ? EBikeLanding.Good : EBikeLanding.Hard;

                LandingFound(LandingEventFound);
                return;
            }
        }

        private void LandingFound(EBikeLanding eBikeLanding)
        {
            bLandingFound = true;
            Debug.Log("Bike Landed in " + eBikeLanding + " Landing");
            LandingEvent.Invoke(eBikeLanding);
        }

        public override void OnStart()
        {
        }

        public override void OnPhysicsUpdate()
        {
        }
    }
}
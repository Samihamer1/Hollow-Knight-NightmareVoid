using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Configuration;
using System.EnterpriseServices;
using Vasi;
using static UnityEngine.UI.Selectable;

namespace NightmareVoid
{
    internal class Boss : MonoBehaviour
    {
        private bool duoAttack = false;
        private FsmOwnerDefault GrimmOwnerDefault;
        private FsmOwnerDefault HkOwnerDefault;
        private PlayMakerFSM GrimmControl;
        private PlayMakerFSM HkControl;


        private FsmEvent DuoSpikeHkTeleEvent = new FsmEvent("DUO SPIKE HK TELE");
        private FsmEvent DuoShotHkTeleLeftEvent = new FsmEvent("DUO SHOT HK TELE LEFT");
        private FsmEvent DuoShotHkTeleRightEvent = new FsmEvent("DUO SHOT HK TELE RIGHT");
        private FsmEvent DuoShotHkCounterTeleLeftEvent = new FsmEvent("DUO SHOT COUNTER HK TELE LEFT");
        private FsmEvent DuoShotHkCounterTeleRightEvent = new FsmEvent("DUO SHOT COUNTER HK TELE RIGHT");
        private FsmEvent DuoSpikeGrimmContinueEvent = new FsmEvent("DUO SPIKE GRIMM CONTINUE");
        private FsmEvent DuoSlashHkFeintEvent = new FsmEvent("DUO SLASH HK FEINT");
        private FsmEvent DuoSlashHkEvent = new FsmEvent("DUO SLASH HK");
        private FsmEvent DuoTendrilHkEvent = new FsmEvent("DUO TENDRIL HK");
        private FsmEvent DuoSlashGrimmReplaceEvent = new FsmEvent("DUO SLASH GRIMM REPLACE");
        private FsmEvent DuoHkDashEvent = new FsmEvent("DUO HK DASH");
        private FsmEvent DuoHkFocusEvent = new FsmEvent("DUO HK FOCUS");
        private FsmEvent DuoBalloon1Event = new FsmEvent("DUO BALLOON 1");
        private FsmEvent DuoBalloon2Event = new FsmEvent("DUO BALLOON 2");
        private FsmEvent DuoBalloon3Event = new FsmEvent("DUO BALLOON 3");
        private FsmEvent StopSoloEvent = new FsmEvent("STOP SOLO");
        private FsmEvent TeleOutHkEvent = new FsmEvent("TELE OUT HK");

        private void Start()
        {
            On.HutongGames.PlayMaker.FsmStateAction.ctor += Hotfix;
            On.HealthManager.TakeDamage += DamageDelegate;
            CreateBossDuo();
        }

        private void DamageDelegate(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
        {
            orig.Invoke(self, hitInstance);
            if (HkControl != null && GrimmControl != null)
            {
                if (self.gameObject == HkControl.gameObject)
                {
                    GrimmControl.gameObject.GetComponent<HealthManager>().hp -= (int)(hitInstance.DamageDealt);

                    PlayMakerFSM Stun = GrimmControl.gameObject.LocateMyFSM("Stun");
                    Stun.SendEvent("STUN DAMAGE");
                }
            }
        }

        private void Hotfix(On.HutongGames.PlayMaker.FsmStateAction.orig_ctor orig, FsmStateAction self)
        {
            orig(self);
            self.Reset();
        }

        private void OnDisable()
        { 
            On.HutongGames.PlayMaker.FsmStateAction.ctor -= Hotfix;
            On.HealthManager.TakeDamage -= DamageDelegate;
        }

        private void CreateBossDuo()
        {
            //you know i dont look at other boss mods and man i hope there isnt a better way to make these
            GrimmControl = CreateGrimm();
            HkControl = CreatePureVessel();

            SetupGrimm(GrimmControl);
            SetupPureVessel(HkControl);
        }

        private PlayMakerFSM CreateGrimm()
        {
            GameObject origGrimm = this.gameObject;

            //lobotomise the grimm
            PlayMakerFSM grimmControl = origGrimm.LocateMyFSM("Control");
            GrimmOwnerDefault = grimmControl.GetState("Tele Out").GetAction<SetVelocity2d>().gameObject;

            foreach (FsmState state in grimmControl.FsmStates)
            {
                state.Transitions = new FsmTransition[0];
            }

            origGrimm.GetComponent<HealthManager>().hp = 1600;
            grimmControl.SetState("Set Balloon HP");

            return grimmControl;
        }

        private void SetupGrimm(PlayMakerFSM grimmControl)
        {


            //events
            FsmEvent finishedevent = grimmControl.GetFsmEvent("FINISHED");
            FsmEvent cancelevent = grimmControl.GetFsmEvent("CANCEL");
            FsmEvent wakeevent = grimmControl.GetFsmEvent("WAKE");
            FsmEvent spikecontinueevent = grimmControl.CreateFsmEvent("SPIKECONTINUE");
            FsmEvent duospikeevent = grimmControl.CreateFsmEvent("DUO SPIKE");
            FsmEvent duoshotevent = grimmControl.CreateFsmEvent("DUO SHOT");
            FsmEvent duoslashevent = grimmControl.CreateFsmEvent("DUO SLASH");
            FsmEvent duoslashfeintevent = grimmControl.CreateFsmEvent("DUO SLASH FEINT");
            FsmEvent duodashpillarevent = grimmControl.CreateFsmEvent("DUO DASH PILLAR");
            FsmEvent duofocuspillarevent = grimmControl.CreateFsmEvent("DUO FOCUS PILLAR");
            FsmEvent duotendrilspikeevent = grimmControl.CreateFsmEvent("DUO TENDRIL SPIKE");
            FsmEvent duoslashshotcheckevent = grimmControl.CreateFsmEvent("DUO SLASH SHOT CHECK");
            FsmEvent leftevent = grimmControl.CreateFsmEvent("LEFT");
            FsmEvent rightevent = grimmControl.CreateFsmEvent("RIGHT");
            FsmEvent heroleftevent = grimmControl.GetFsmEvent("HERO L");
            FsmEvent herorightevent = grimmControl.GetFsmEvent("HERO R");
            FsmEvent behindevent = grimmControl.GetFsmEvent("BEHIND");
            FsmEvent repeatevent = grimmControl.GetFsmEvent("REPEAT");
            FsmEvent tookdamageevent = grimmControl.GetFsmEvent("TOOK DAMAGE");
            FsmEvent uppercutevent = grimmControl.GetFsmEvent("UPPERCUT");
            FsmEvent airdashevent = grimmControl.GetFsmEvent("AIR DASH");
            FsmEvent pillarsevent = grimmControl.GetFsmEvent("PILLARS");
            FsmEvent edgeevent = grimmControl.GetFsmEvent("EDGE");
            FsmEvent landevent = grimmControl.GetFsmEvent("LAND");
            FsmEvent nextevent = grimmControl.GetFsmEvent("NEXT");
            FsmEvent altballevent = grimmControl.GetFsmEvent("ALT BALL");
            FsmEvent endevent = grimmControl.GetFsmEvent("END");
            FsmEvent balloon1event = grimmControl.GetFsmEvent("BALLOON 1");
            FsmEvent balloon2event = grimmControl.GetFsmEvent("BALLOON 2");
            FsmEvent balloon3event = grimmControl.GetFsmEvent("BALLOON 3");
            FsmEvent stunevent = grimmControl.GetFsmEvent("STUN");
            FsmEvent zerohpevent = grimmControl.GetFsmEvent("ZERO HP");

            //variables
            FsmInt DuoSpikeCount = grimmControl.GetOrCreateInt("Duo Spike Count");
            FsmFloat HeroX = grimmControl.Fsm.GetFsmFloat("Hero X");
            FsmBool Uppercut = grimmControl.CreateBool("Uppercut");
            FsmFloat ADSpeed = grimmControl.Fsm.GetFsmFloat("AD Speed");
            ADSpeed.Value = 80f; //buff airdash speed to 80 from 55
            FsmFloat GDashSpeed = grimmControl.Fsm.GetFsmFloat("G Dash Speed");
            GDashSpeed.Value = -85f; //buff ground dash speed to 85 from 58
            FsmBool DoneBalloon1 = grimmControl.Fsm.GetFsmBool("Done Balloon 1");
            FsmBool DoneBalloon2 = grimmControl.Fsm.GetFsmBool("Done Balloon 2");
            FsmBool DoneBalloon3 = grimmControl.Fsm.GetFsmBool("Done Balloon 3");
            FsmBool WillRepeat = grimmControl.Fsm.GetFsmBool("Will Repeat");
            FsmBool Tendrils = grimmControl.CreateBool("Tendrils");

            FsmOwnerDefault HeroOwnerDefault = grimmControl.GetState("FB Hero Pos").GetAction<GetPosition>().gameObject;

            FsmInt CtDuoSpike = grimmControl.GetOrCreateInt("Ct Duo Spike");
            FsmInt MsDuoSpike = grimmControl.GetOrCreateInt("Ms Duo Spike");
            FsmInt CtDuoShot = grimmControl.GetOrCreateInt("Ct Duo Shot");
            FsmInt MsDuoShot = grimmControl.GetOrCreateInt("Ms Duo Shot");
            FsmInt MsDuoSlash = grimmControl.GetOrCreateInt("Ms Duo Slash");
            FsmInt CtDuoSlash = grimmControl.GetOrCreateInt("Ct Duo Slash");
            FsmInt MsDuoSlashFeint = grimmControl.GetOrCreateInt("Ms Duo Slash Feint");
            FsmInt CtDuoSlashFeint = grimmControl.GetOrCreateInt("Ct Duo Slash Feint");
            FsmInt MsDuoDashPillar = grimmControl.GetOrCreateInt("Ms Duo Dash Pillar");
            FsmInt CtDuoDashPillar = grimmControl.GetOrCreateInt("Ct Duo Dash Pillar");
            FsmInt MsDuoFocusPillar = grimmControl.GetOrCreateInt("Ms Duo Focus Pillar");
            FsmInt CtDuoFocusPillar = grimmControl.GetOrCreateInt("Ct Duo Focus Pillar");
            FsmInt MsDuoSlashShot = grimmControl.GetOrCreateInt("Ms Duo Slash Shot");
            FsmInt CtDuoSlashShot = grimmControl.GetOrCreateInt("Ct Duo Slash Shot");
            FsmInt MsDuoTendrilSpike = grimmControl.GetOrCreateInt("Ms Duo Tendril Spike");
            FsmInt CtDuoTendrilSpike = grimmControl.GetOrCreateInt("Ct Duo Tendril Spike");

            FsmEvent[] DuoEventArray4 = new FsmEvent[] { duospikeevent, duoshotevent, duoslashevent, duoslashfeintevent, duofocuspillarevent, duoslashshotcheckevent, duotendrilspikeevent };
            FsmFloat[] DuoWeightArray4 = new FsmFloat[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f };
            FsmInt[] DuoEventMaxArary4 = new FsmInt[] { 1, 2, 2, 2, 2, 2, 2 };
            FsmInt[] DuoMissedMaxArray4 = new FsmInt[] { 6, 4, 4, 6, 4, 4, 4 };
            FsmInt[] DuoTrackingIntsArray4 = new FsmInt[] { CtDuoSpike, CtDuoShot, CtDuoSlash, CtDuoSlashFeint, CtDuoFocusPillar, CtDuoSlashShot, CtDuoTendrilSpike };
            FsmInt[] DuoTrackingIntsMissedArray4 = new FsmInt[] { MsDuoSpike, MsDuoShot, MsDuoSlash, MsDuoSlashFeint, MsDuoFocusPillar, MsDuoSlashShot, MsDuoTendrilSpike };

            //phase 1
            FsmEvent[] DuoEventArray1 = new FsmEvent[] { duospikeevent, duoshotevent, duoslashevent, duodashpillarevent };
            FsmFloat[] DuoWeightArray1 = new FsmFloat[] { 1f, 1f, 1f, 1f };
            FsmInt[] DuoEventMaxArary1 = new FsmInt[] { 1, 2, 2, 1 };
            FsmInt[] DuoMissedMaxArray1 = new FsmInt[] { 6, 4, 4, 6 };
            FsmInt[] DuoTrackingIntsArray1 = new FsmInt[] { CtDuoSpike, CtDuoShot, CtDuoSlash, CtDuoDashPillar };
            FsmInt[] DuoTrackingIntsMissedArray1 = new FsmInt[] { MsDuoSpike, MsDuoShot, MsDuoSlash, MsDuoDashPillar };

            //phase 2
            FsmEvent[] DuoEventArray3 = new FsmEvent[] { duospikeevent, duoshotevent, duoslashevent, duofocuspillarevent, duoslashshotcheckevent, duoslashfeintevent };
            FsmFloat[] DuoWeightArray3 = new FsmFloat[] { 1f, 1f, 1f, 1f, 1f, 1f };
            FsmInt[] DuoEventMaxArary3 = new FsmInt[] { 1, 2, 2, 1, 2, 1 };
            FsmInt[] DuoMissedMaxArray3 = new FsmInt[] { 6, 4, 4, 6, 4, 4 };
            FsmInt[] DuoTrackingIntsArray3 = new FsmInt[] { CtDuoSpike, CtDuoShot, CtDuoSlash, CtDuoFocusPillar, CtDuoSlashShot, CtDuoSlashFeint };
            FsmInt[] DuoTrackingIntsMissedArray3 = new FsmInt[] { MsDuoSpike, MsDuoShot, MsDuoSlash, CtDuoFocusPillar, MsDuoSlashShot, MsDuoSlashFeint };

            //phase 2
            FsmEvent[] DuoEventArray2 = new FsmEvent[] { duospikeevent, duoshotevent, duoslashevent, duofocuspillarevent };
            FsmFloat[] DuoWeightArray2 = new FsmFloat[] { 1f, 1f, 1f, 1f };
            FsmInt[] DuoEventMaxArary2 = new FsmInt[] { 1, 2, 2, 1 };
            FsmInt[] DuoMissedMaxArray2 = new FsmInt[] { 6, 4, 4, 6 };
            FsmInt[] DuoTrackingIntsArray2 = new FsmInt[] { CtDuoSpike, CtDuoShot, CtDuoSlash, CtDuoFocusPillar };
            FsmInt[] DuoTrackingIntsMissedArray2 = new FsmInt[] { MsDuoSpike, MsDuoShot, MsDuoSlash, MsDuoFocusPillar };


            //solo choice stuff
            FsmInt CtAirDash = grimmControl.GetOrCreateInt("CT AirDash");
            FsmInt MsAirDash = grimmControl.GetOrCreateInt("Ms AirDash");
            FsmInt CtPillar = grimmControl.GetOrCreateInt("CT Pillar");
            FsmInt MsPillar = grimmControl.GetOrCreateInt("Ms Pillar");

            FsmEvent[] SoloEventArray = new FsmEvent[] { airdashevent };
            FsmFloat[] SoloWeightArray = new FsmFloat[] { 1f };
            FsmInt[] SoloEventMaxArary = new FsmInt[] { 999 };
            FsmInt[] SoloMissedMaxArray = new FsmInt[] { 999 };
            FsmInt[] SoloTrackingIntsArray = new FsmInt[] { CtAirDash };
            FsmInt[] SoloTrackingIntsMissedArray = new FsmInt[] { MsAirDash };



            FsmFloat[] HalfHalfWeightArray = new FsmFloat[] { 1f, 1f };
            FsmEvent[] LeftRightEventArray = new FsmEvent[] { leftevent, rightevent };

            //states
            FsmState TeleOutState = grimmControl.GetState("Tele Out");
            FsmState OutPauseState = grimmControl.GetState("Out Pause");
            FsmState MoveChoiceState = grimmControl.GetState("Move Choice");
            FsmState DormantState = grimmControl.GetState("Dormant");
            FsmState SpikeAttackState = grimmControl.GetState("Spike Attack");
            FsmState SpikeReturnState = grimmControl.GetState("Spike Return");
            FsmState SpikeAnticState = grimmControl.GetState("Spike Antic");
            FsmState SpikeTeleInState = grimmControl.GetState("Spike Tele In");
            FsmState SpikePosState = grimmControl.GetState("Spike Pos");
            FsmState SpikeRetryState = grimmControl.GetState("Spike Retry");
            FsmState FBTeleRState = grimmControl.GetState("FB Tele R");
            FsmState FBTeleLState = grimmControl.GetState("FB Tele L");
            FsmState FBTeleInState = grimmControl.GetState("FB Tele In");
            FsmState FBAnticState = grimmControl.GetState("FB Antic");
            FsmState FBCastState = grimmControl.GetState("FB Cast");
            FsmState Firebat1State = grimmControl.GetState("Firebat 1");
            FsmState Firebat2State = grimmControl.GetState("Firebat 2");
            FsmState Firebat3State = grimmControl.GetState("Firebat 3");
            FsmState Firebat4State = grimmControl.GetState("Firebat 4");
            FsmState ExtraBatState = grimmControl.GetState("Extra Bat");
            FsmState FBCastEndState = grimmControl.GetState("FB Cast End");
            FsmState FBBehindState = grimmControl.GetState("FB Behind");
            FsmState FBReTeleState = grimmControl.GetState("FB Re Tele");
            FsmState FBHeroPosState = grimmControl.GetState("FB Hero Pos");
            FsmState SlashPosState = grimmControl.GetState("Slash Pos");
            FsmState SlashTeleInState = grimmControl.GetState("Slash Tele In");
            FsmState SlashAnticState = grimmControl.GetState("Slash Antic");
            FsmState Slash1State = grimmControl.GetState("Slash 1");
            FsmState Slash2State = grimmControl.GetState("Slash 2");
            FsmState Slash3State = grimmControl.GetState("Slash 3");
            FsmState SlashRecoverState = grimmControl.GetState("Slash Recover");
            FsmState SlashEndState = grimmControl.GetState("Slash End");
            FsmState UppercutCheckState = grimmControl.GetState("Uppercut?");
            FsmState UppercutAnticState = grimmControl.GetState("Uppercut Antic");
            FsmState UppercutUpState = grimmControl.GetState("Uppercut Up");
            FsmState UppercutExplodeState = grimmControl.GetState("UP Explode");
            FsmState ExplodePauseState = grimmControl.GetState("Explode Pause");
            FsmState ADRetryState = grimmControl.GetState("AD Retry");
            FsmState ADPosState = grimmControl.GetState("AD Pos");
            FsmState ADTeleInState = grimmControl.GetState("AD Tele In");
            FsmState ADAnticState = grimmControl.GetState("AD Antic");
            FsmState ADFireState = grimmControl.GetState("AD Fire");
            FsmState ADEdgeState = grimmControl.GetState("AD Edge");
            FsmState GDAnticState = grimmControl.GetState("GD Antic");
            FsmState GDashState = grimmControl.GetState("G Dash");
            FsmState GDashRecoverState = grimmControl.GetState("G Dash Recover");
            FsmState PillarRetryState = grimmControl.GetState("Pillar Retry");
            FsmState PillarPosState = grimmControl.GetState("Pillar Pos");
            FsmState PillarTeleInState = grimmControl.GetState("Pillar Tele In");
            FsmState PillarAnticState = grimmControl.GetState("Pillar Antic");
            FsmState PillarState = grimmControl.GetState("Pillar");
            FsmState PillarRepeatState = grimmControl.GetState("Repeat?");
            FsmState PillarEndState = grimmControl.GetState("Pillar End");
            FsmState SlashRetryState = grimmControl.GetState("Slash Retry");
            FsmState BalloonPosState = grimmControl.GetState("Balloon Pos");
            FsmState BalloonRetryState = grimmControl.GetState("Balloon Retry");
            FsmState BalloonCheckState = grimmControl.GetState("Balloon Check");
            FsmState BalloonTeleInState = grimmControl.GetState("Balloon Tele In");
            FsmState BalloonAnticState = grimmControl.GetState("Balloon Antic");
            FsmState InflateState = grimmControl.GetState("Inflate");
            FsmState FirePauseState = grimmControl.GetState("Fire Pause");
            FsmState FireRepeatState = grimmControl.GetState("Fire Repeat");
            FsmState EndPauseState = grimmControl.GetState("End Pause");
            FsmState DeflateState = grimmControl.GetState("Deflate");
            FsmState HalfwayState = grimmControl.GetState("Halfway?");
            FsmState FireLowRState = grimmControl.GetState("Fire Low R");
            FsmState FireLowLState = grimmControl.GetState("Fire Low L");
            FsmState AltState = grimmControl.GetState("Alt");
            FsmState FireMidRState = grimmControl.GetState("Fire Mid R");
            FsmState FireMidLState = grimmControl.GetState("Fire Mid L");
            FsmState FireHighRState = grimmControl.GetState("Fire High R");
            FsmState FireHighLState = grimmControl.GetState("Fire High L");
            FsmState DownState = grimmControl.GetState("Down");
            FsmState SetBalloon1State = grimmControl.GetState("Set Balloon 1");
            FsmState SetBalloon2State = grimmControl.GetState("Set Balloon 2");
            FsmState SetBalloon3State = grimmControl.GetState("Set Balloon 3");
            FsmState BalloonState = grimmControl.GetState("Balloon?");
            FsmState StunResetState = grimmControl.GetState("Stun Reset");
            FsmState StunState = grimmControl.GetState("Stun");
            FsmState ExplodeState = grimmControl.GetState("Explode");
            FsmState ExplodedState = grimmControl.GetState("Exploded");
            FsmState CallBatsState = grimmControl.GetState("Call Bats");
            FsmState ReformingState = grimmControl.GetState("Reforming");
            FsmState ReformedState = grimmControl.GetState("Reformed");
            FsmState AdjustHPState = grimmControl.GetState("Adjust HP");
            FsmState DeathStartState = grimmControl.GetState("Death Start");
            FsmState HudCanvasOUTState = grimmControl.GetState("HUD Canvas OUT");
            FsmState SteamState = grimmControl.GetState("Steam");
            FsmState DeathExplodeState = grimmControl.GetState("Death Explode");
            FsmState SendNPCEventState = grimmControl.GetState("Send NPC Event");

            //custom states
            FsmState DuoSpikeCounterState = grimmControl.CreateState("Duo Spike Counter");
            DuoSpikeCounterState.AddAction(new IntAdd { everyFrame = false, intVariable = DuoSpikeCount, add = 1 });
            DuoSpikeCounterState.AddAction(new IntCompare { everyFrame = false, integer1 = DuoSpikeCount, integer2 = 4, equal = finishedevent, lessThan = spikecontinueevent });

            FsmState DuoSpikeAnticState = grimmControl.CreateState("Duo Spike Antic");
            DuoSpikeAnticState.AddAction(new SetIntValue { everyFrame = false, intVariable = DuoSpikeCount, intValue = 0 });
            DuoSpikeAnticState.AddAction(new SetBoolValue { everyFrame = false, boolVariable = Tendrils, boolValue = false });

            FsmState DuoSpikeWaitState = grimmControl.CreateState("Duo Spike Wait");
            DuoSpikeWaitState.AddAction(new Wait { finishEvent = nextevent, realTime = false, time = 2.5f });

            FsmState DuoMoveChoice1 = grimmControl.CreateState("Duo Move Choice 1");
            DuoMoveChoice1.AddAction(new SetVelocity2d { everyFrame = false, gameObject = GrimmOwnerDefault, vector = new Vector2(0, 0), x = 0, y = 0 });
            DuoMoveChoice1.AddAction(new SendRandomEventV3 { events = DuoEventArray1, weights = DuoWeightArray1, trackingInts = DuoTrackingIntsArray1, eventMax = DuoEventMaxArary1, trackingIntsMissed = DuoTrackingIntsMissedArray1, missedMax = DuoMissedMaxArray1 });

            FsmState DuoMoveChoice2 = grimmControl.CreateState("Duo Move Choice 2");
            DuoMoveChoice2.AddAction(new SetVelocity2d { everyFrame = false, gameObject = GrimmOwnerDefault, vector = new Vector2(0, 0), x = 0, y = 0 });
            DuoMoveChoice2.AddAction(new SendRandomEventV3 { events = DuoEventArray2, weights = DuoWeightArray2, trackingInts = DuoTrackingIntsArray2, eventMax = DuoEventMaxArary2, trackingIntsMissed = DuoTrackingIntsMissedArray2, missedMax = DuoMissedMaxArray2 });

            FsmState DuoMoveChoice3 = grimmControl.CreateState("Duo Move Choice 3");
            DuoMoveChoice3.AddAction(new SetVelocity2d { everyFrame = false, gameObject = GrimmOwnerDefault, vector = new Vector2(0, 0), x = 0, y = 0 });
            DuoMoveChoice3.AddAction(new SendRandomEventV3 { events = DuoEventArray3, weights = DuoWeightArray3, trackingInts = DuoTrackingIntsArray3, eventMax = DuoEventMaxArary3, trackingIntsMissed = DuoTrackingIntsMissedArray3, missedMax = DuoMissedMaxArray3 });

            FsmState DuoMoveChoice4 = grimmControl.CreateState("Duo Move Choice 4");
            DuoMoveChoice4.AddAction(new SetVelocity2d { everyFrame = false, gameObject = GrimmOwnerDefault, vector = new Vector2(0, 0), x = 0, y = 0 });
            DuoMoveChoice4.AddAction(new SendRandomEventV3 { events = DuoEventArray4, weights = DuoWeightArray4, trackingInts = DuoTrackingIntsArray4, eventMax = DuoEventMaxArary4, trackingIntsMissed = DuoTrackingIntsMissedArray4, missedMax = DuoMissedMaxArray4 });

            FsmState PhaseCheckState = grimmControl.CreateState("Phase Check");
            PhaseCheckState.AddAction(new BoolTest { boolVariable = DoneBalloon3, everyFrame = false, isTrue = balloon3event });
            PhaseCheckState.AddAction(new BoolTest { boolVariable = DoneBalloon2, everyFrame = false, isTrue = balloon2event });
            PhaseCheckState.AddAction(new BoolTest { boolVariable = DoneBalloon1, everyFrame = false, isTrue = balloon1event });

            FsmState DuoShotCheckState = grimmControl.CreateState("Duo Shot Check");
            DuoShotCheckState.AddAction(new GetPosition { gameObject = HeroOwnerDefault, vector = new Vector3(0, 0, 0), x = HeroX, space = 0, everyFrame = false, y = 0, z = 0 });
            DuoShotCheckState.AddAction(new FloatInRange { floatVariable = HeroX, lowerValue = 75, upperValue = 96, falseEvent = cancelevent, everyFrame = false, boolVariable = false });
            DuoShotCheckState.AddAction(new SendRandomEvent { weights = HalfHalfWeightArray, events = LeftRightEventArray, delay = 0 });

            FsmState DuoSlashHkFeintState = grimmControl.CreateState("Duo Slash Feint");
            DuoSlashHkFeintState.AddMethod(() => { SendEventToFSM(DuoSlashHkFeintEvent, HkControl, 0f); });

            FsmState DuoUppercutPosState = grimmControl.CreateState("Duo Uppercut Pos");
            DuoUppercutPosState.CopyActionData(SlashPosState);
            DuoUppercutPosState.GetAction<RandomFloat>().min = 3f;
            DuoUppercutPosState.GetAction<RandomFloat>().max = 4f;
            DuoUppercutPosState.AddAction(new SetBoolValue { everyFrame = false, boolVariable = Uppercut, boolValue = true });

            FsmState DuoSlashHkState = grimmControl.CreateState("Duo Slash");
            DuoSlashHkState.AddMethod(() => { SendEventToFSM(DuoSlashHkEvent, HkControl, 0f); });
            DuoSlashHkState.AddAction(new Wait { realTime = false, time = 0.75f, finishEvent = nextevent });

            FsmState SoloMoveChoice = grimmControl.CreateState("Solo Move Choice");
            SoloMoveChoice.AddAction(new SetVelocity2d { everyFrame = false, gameObject = GrimmOwnerDefault, vector = new Vector2(0, 0), x = 0, y = 0 });
            SoloMoveChoice.AddAction(new SendRandomEventV3 { events = SoloEventArray, weights = SoloWeightArray, trackingInts = SoloTrackingIntsArray, eventMax = SoloEventMaxArary, trackingIntsMissed = SoloTrackingIntsMissedArray, missedMax = SoloMissedMaxArray });

            FsmState DuoDashPillarState = grimmControl.CreateState("Duo Dash Pillar");
            DuoDashPillarState.AddMethod(() => { SendEventToFSM(DuoSlashHkEvent, HkControl, 0f); });

            FsmState DuoFocusPillarState = grimmControl.CreateState("Duo Focus Pillar");
            DuoFocusPillarState.AddMethod(() => { SendEventToFSM(DuoHkFocusEvent, HkControl, 0f); });

            FsmState DuoSlashShotCheckState = grimmControl.CreateState("Duo Slash Shot Check");
            DuoSlashShotCheckState.AddAction(new GetPosition { gameObject = HeroOwnerDefault, everyFrame = false, x = HeroX, y = 0, z = 0, vector = new Vector3(0, 0), space = 0 });
            DuoSlashShotCheckState.AddAction(new SetBoolValue { boolVariable = Uppercut, boolValue = false, everyFrame = false });
            DuoSlashShotCheckState.AddAction(new FloatCompare { float1 = HeroX, float2 = 87f, equal = leftevent, greaterThan = rightevent, lessThan = leftevent, tolerance = 0f, everyFrame = false });


            FsmState DuoSlashShotHkLeftState = grimmControl.CreateState("Duo Slash Shot Left");
            DuoSlashShotHkLeftState.AddMethod(() => { SendEventToFSM(DuoShotHkTeleLeftEvent, HkControl, 0f); });

            FsmState DuoSlashShotHkRightState = grimmControl.CreateState("Duo Slash Shot Right");
            DuoSlashShotHkRightState.AddMethod(() => { SendEventToFSM(DuoShotHkTeleRightEvent, HkControl, 0f); });

            FsmState DuoTendrilSpikeState = grimmControl.CreateState("Duo Tendril Spike");
            DuoTendrilSpikeState.AddMethod(() => { SendEventToFSM(DuoTendrilHkEvent, HkControl, 0f); });
            DuoTendrilSpikeState.AddAction(new SetIntValue { everyFrame = false, intVariable = DuoSpikeCount, intValue = 2 });
            DuoTendrilSpikeState.AddAction(new SetBoolValue { everyFrame = false, boolVariable = Tendrils, boolValue = true });

            //state modifications
            SpikeReturnState.AddMethod(() => { SendEventToFSM(DuoSpikeHkTeleEvent, HkControl, 0); });
            SpikeAttackState.GetAction<Wait>().time = 0.85f; //reduces delay between spike retreat and attack end, originally 1.35f
            SpikeAttackState.AddAction(new SetPosition { gameObject = GrimmOwnerDefault, everyFrame = false, lateUpdate = false, space = 0, vector = new Vector3(0, 0, 0), y = 100 });
            SpikeAttackState.AddAction(new BoolTest { boolVariable = Tendrils, everyFrame = false, isTrue = nextevent });

            FBTeleLState.GetAction<RandomFloat>().min = 70f;
            FBTeleLState.GetAction<RandomFloat>().max = 70f;
            FBTeleRState.GetAction<RandomFloat>().min = 100f;
            FBTeleRState.GetAction<RandomFloat>().max = 100f;

            FBTeleLState.AddMethod(() => { if (DoneBalloon3.Value == true || WillRepeat.Value == false) { SendEventToFSM(DuoShotHkTeleRightEvent, HkControl, 0f); } else { SendEventToFSM(DuoShotHkCounterTeleRightEvent, HkControl, 0f); } });
            FBTeleRState.AddMethod(() => { if (DoneBalloon3.Value == true || WillRepeat.Value == false) { SendEventToFSM(DuoShotHkTeleLeftEvent, HkControl, 0f); } else { SendEventToFSM(DuoShotHkCounterTeleLeftEvent, HkControl, 0f); } });

            FBCastState.GetAction<ActivateGameObject>(3).activate = DoneBalloon2; //behind range only activates if balloon 2 is done

            UppercutCheckState.AddAction(new BoolTest { boolVariable = Uppercut, everyFrame = false, isTrue = uppercutevent }); //adds a second check for guaranteed uppercut

            UppercutAnticState.GetAction<FaceObject>().Enabled = true; //faces player in antic reactivated

            GDAnticState.GetAction<Wait>().time = 0.35f; //time between hitting ground and dashing lowered

            PillarState.GetAction<Wait>().time = 0.55f; //pillars spawn quicker

            SetBalloon1State.AddMethod(() => { SendEventToFSM(DuoBalloon1Event, HkControl, 1f); });
            SetBalloon2State.AddMethod(() => { SendEventToFSM(DuoBalloon2Event, HkControl, 1f); });
            SetBalloon3State.AddMethod(() => { SendEventToFSM(DuoBalloon3Event, HkControl, 1f); });

            DeflateState.AddMethod(() => { if (DoneBalloon3.Value) { SendEventToFSM(StopSoloEvent, HkControl, 0f); } });

            StunState.AddMethod(() => { SendEventToFSM(TeleOutHkEvent, HkControl, 0f); if (grimmControl.gameObject.transform.GetPositionX() > 102 || grimmControl.gameObject.transform.GetPositionX() < 72) { grimmControl.gameObject.transform.SetPositionX(87f); grimmControl.gameObject.transform.SetPositionY(8f); }; });      
            DeathStartState.AddMethod(() => { SendEventToFSM(TeleOutHkEvent, HkControl, 0f); if (grimmControl.gameObject.transform.GetPositionX() > 102 || grimmControl.gameObject.transform.GetPositionX() < 72) { grimmControl.gameObject.transform.SetPositionX(87f); grimmControl.gameObject.transform.SetPositionY(8f); } });

            //transitions
            SpikeAttackState.AddTransition(finishedevent, "Spike Return");
            SpikeAttackState.AddTransition(nextevent, "Duo Spike Wait");
            SpikeReturnState.AddTransition(finishedevent, "Duo Spike Wait");
            DuoSpikeWaitState.AddTransition(DuoSpikeGrimmContinueEvent, "Duo Spike Counter");
            SpikeAnticState.AddTransition(finishedevent, "Spike Attack");
            SpikeTeleInState.AddTransition(finishedevent, "Spike Antic");
            SpikePosState.AddTransition(finishedevent, "Spike Tele In");
            SpikePosState.AddTransition(cancelevent, "Spike Retry");
            SpikeRetryState.AddTransition(finishedevent, "Spike Pos");
            DormantState.AddTransition(wakeevent, "Tele Out");
            PhaseCheckState.AddTransition(finishedevent, "Duo Move Choice 1");
            PhaseCheckState.AddTransition(balloon1event, "Duo Move Choice 2");
            PhaseCheckState.AddTransition(balloon2event, "Duo Move Choice 3");
            PhaseCheckState.AddTransition(balloon3event, "Duo Move Choice 4");
            DuoMoveChoice4.AddTransition(duospikeevent, "Duo Spike Antic");
            DuoMoveChoice4.AddTransition(duoshotevent, "Duo Shot Check");
            DuoMoveChoice4.AddTransition(duoslashfeintevent, "Duo Slash Feint");
            DuoMoveChoice4.AddTransition(duoslashevent, "Duo Slash");
            DuoMoveChoice4.AddTransition(duodashpillarevent, "Duo Dash Pillar");
            DuoMoveChoice4.AddTransition(duofocuspillarevent, "Duo Focus Pillar");
            DuoMoveChoice4.AddTransition(duoslashshotcheckevent, "Duo Slash Shot Check");
            DuoMoveChoice4.AddTransition(duotendrilspikeevent, "Duo Tendril Spike");
            DuoMoveChoice1.Transitions = DuoMoveChoice4.Transitions;
            DuoMoveChoice2.Transitions = DuoMoveChoice4.Transitions;
            DuoMoveChoice3.Transitions = DuoMoveChoice4.Transitions;
            DuoSpikeAnticState.AddTransition(finishedevent, "Duo Spike Counter");
            DuoSpikeCounterState.AddTransition(finishedevent, "Balloon?");
            DuoSpikeCounterState.AddTransition(spikecontinueevent, "Spike Attack");
            TeleOutState.AddTransition(finishedevent, "Out Pause");
            OutPauseState.AddTransition(finishedevent, "Balloon?");

            DuoShotCheckState.AddTransition(leftevent, "FB Tele L");
            DuoShotCheckState.AddTransition(rightevent, "FB Tele R");
            DuoShotCheckState.AddTransition(cancelevent, "Balloon?");

            FBTeleRState.AddTransition(finishedevent, "FB Tele In"); //bats
            FBTeleLState.AddTransition(finishedevent, "FB Tele In");
            FBTeleInState.AddTransition(finishedevent, "FB Antic");
            FBAnticState.AddTransition(finishedevent, "FB Cast");
            FBCastState.AddTransition(finishedevent, "Firebat 1");
            FBCastState.AddTransition(behindevent, "FB Behind");
            FBCastState.AddTransition(repeatevent, "Extra Bat");
            Firebat1State.AddTransition(finishedevent, "Firebat 2");
            Firebat1State.AddTransition(behindevent, "FB Behind");
            Firebat2State.AddTransition(finishedevent, "Firebat 3");
            Firebat2State.AddTransition(behindevent, "FB Behind");
            Firebat3State.AddTransition(finishedevent, "Firebat 4");
            Firebat3State.AddTransition(behindevent, "FB Behind");
            Firebat4State.AddTransition(finishedevent, "FB Cast End");
            FBCastEndState.AddTransition(finishedevent, "Tele Out");
            FBCastEndState.AddTransition(tookdamageevent, "Tele Out");
            FBBehindState.AddTransition(finishedevent, "FB Re Tele");
            FBReTeleState.AddTransition(finishedevent, "FB Hero Pos");
            FBHeroPosState.AddTransition(heroleftevent, "FB Tele R");
            FBHeroPosState.AddTransition(herorightevent, "FB Tele L");
            ExtraBatState.AddTransition(finishedevent, "Firebat 4");

            DuoSlashHkFeintState.AddTransition(DuoSlashGrimmReplaceEvent, "Duo Uppercut Pos"); //slash feint stuff           
            DuoUppercutPosState.AddTransition(finishedevent, "Slash Tele In");
            SlashTeleInState.AddTransition(finishedevent, "Uppercut?");
            UppercutCheckState.AddTransition(uppercutevent, "Uppercut Antic");
            UppercutCheckState.AddTransition(finishedevent, "Slash Antic");
            UppercutAnticState.AddTransition(finishedevent, "Uppercut Up");
            UppercutUpState.AddTransition(finishedevent, "UP Explode");
            UppercutExplodeState.AddTransition(finishedevent, "Explode Pause");
            ExplodePauseState.AddTransition(finishedevent, "Balloon?");
            DuoSlashHkState.AddTransition(nextevent, "AD Pos");

            SoloMoveChoice.AddTransition(airdashevent, "AD Pos"); //solo move choice

            ADPosState.AddTransition(finishedevent, "AD Tele In"); //airdash stuff
            ADPosState.AddTransition(cancelevent, "AD Retry");
            ADRetryState.AddTransition(finishedevent, "AD Pos");
            ADTeleInState.AddTransition(finishedevent, "AD Antic");
            ADAnticState.AddTransition(finishedevent, "AD Fire");
            ADFireState.AddTransition(landevent, "GD Antic");
            ADFireState.AddTransition(edgeevent, "AD Edge");
            ADEdgeState.AddTransition(landevent, "GD Antic");
            GDAnticState.AddTransition(nextevent, "G Dash");
            GDashState.AddTransition(finishedevent, "G Dash Recover");
            GDashRecoverState.AddTransition(finishedevent, "Tele Out");

            DuoDashPillarState.AddTransition(finishedevent, "Pillar Pos"); //pillars
            PillarPosState.AddTransition(finishedevent, "Pillar Tele In");
            PillarPosState.AddTransition(cancelevent, "Pillar Retry");
            PillarRetryState.AddTransition(finishedevent, "Pillar Pos");
            PillarTeleInState.AddTransition(finishedevent, "Pillar Antic");
            PillarAnticState.AddTransition(finishedevent, "Pillar");
            PillarState.AddTransition(finishedevent, "Repeat?");
            PillarRepeatState.AddTransition(repeatevent, "Pillar");
            PillarRepeatState.AddTransition(finishedevent, "Pillar End");
            PillarEndState.AddTransition(finishedevent, "Tele Out");
            DuoFocusPillarState.AddTransition(finishedevent, "Pillar Pos");

            DuoSlashShotCheckState.AddTransition(leftevent, "Duo Slash Shot Left"); //slash shot
            DuoSlashShotCheckState.AddTransition(rightevent, "Duo Slash Shot Right");
            DuoSlashShotHkLeftState.AddTransition(finishedevent, "Slash Pos");
            DuoSlashShotHkRightState.AddTransition(finishedevent, "Slash Pos");
            SlashPosState.AddTransition(finishedevent, "Slash Tele In");
            SlashPosState.AddTransition(cancelevent, "Slash Retry");
            SlashRetryState.AddTransition(finishedevent, "Slash Pos");
            SlashAnticState.AddTransition(finishedevent, "Slash 1");
            Slash1State.AddTransition(finishedevent, "Slash 2");
            Slash2State.AddTransition(finishedevent, "Slash 3");
            Slash3State.AddTransition(finishedevent, "Slash Recover");
            SlashRecoverState.AddTransition(finishedevent, "Slash End");
            SlashEndState.AddTransition(finishedevent, "Tele Out");

            DuoTendrilSpikeState.AddTransition(finishedevent, "Duo Spike Counter"); //tendril and spike
            DuoSpikeWaitState.AddTransition(nextevent, "Spike Return");

            BalloonPosState.AddTransition(finishedevent, "Balloon Retry"); //man im sorry
            BalloonRetryState.AddTransition(finishedevent, "Balloon Check");
            BalloonCheckState.AddTransition(finishedevent, "Balloon Tele In");
            BalloonCheckState.AddTransition(cancelevent, "Balloon Retry");
            BalloonTeleInState.AddTransition(finishedevent, "Balloon Antic");
            BalloonAnticState.AddTransition(finishedevent, "Halfway?");
            HalfwayState.AddTransition(finishedevent, "Inflate");
            InflateState.AddTransition(finishedevent, "Fire Pause");
            FirePauseState.AddTransition(finishedevent, "Fire Repeat");
            FireRepeatState.AddTransition(finishedevent, "Fire Low R");
            FireRepeatState.AddTransition(altballevent, "Alt");
            FireRepeatState.AddTransition(endevent, "End Pause");
            EndPauseState.AddTransition(finishedevent, "Deflate");
            DeflateState.AddTransition(finishedevent, "Tele Out");
            FireLowRState.AddTransition(finishedevent, "Fire Low L");
            FireLowLState.AddTransition(finishedevent, "Fire Mid R");
            FireMidRState.AddTransition(finishedevent, "Fire Mid L");
            FireMidLState.AddTransition(finishedevent, "Fire High R");
            FireHighRState.AddTransition(finishedevent, "Fire High L");
            FireHighLState.AddTransition(finishedevent, "Down");
            AltState.AddTransition(finishedevent, "Fire High R");
            DownState.AddTransition(finishedevent, "Fire Pause");
            SetBalloon1State.AddTransition(finishedevent, "Balloon Pos");
            SetBalloon2State.AddTransition(finishedevent, "Balloon Pos");
            SetBalloon3State.AddTransition(finishedevent, "Balloon Pos");
            BalloonState.AddTransition(finishedevent, "Phase Check");
            BalloonState.AddTransition(balloon1event, "Set Balloon 1");
            BalloonState.AddTransition(balloon2event, "Set Balloon 2");
            BalloonState.AddTransition(balloon3event, "Set Balloon 3");

            StunResetState.AddTransition(finishedevent, "Stun"); //stun
            StunState.AddTransition(finishedevent, "Explode");
            ExplodeState.AddTransition(finishedevent, "Exploded");
            ExplodedState.AddTransition(finishedevent, "Call Bats");
            CallBatsState.AddTransition(finishedevent, "Reforming");
            ReformingState.AddTransition(finishedevent, "Reformed");
            ReformedState.AddTransition(finishedevent, "Adjust HP");
            AdjustHPState.AddTransition(finishedevent, "Balloon?");

            DeathStartState.AddTransition(finishedevent, "HUD Canvas OUT"); //death
            HudCanvasOUTState.AddTransition(finishedevent, "Steam");
            SteamState.AddTransition(finishedevent, "Death Explode");
            DeathExplodeState.AddTransition(finishedevent, "Send NPC Event");


            //globals
            grimmControl.AddGlobalTransition(stunevent, "Stun Reset");
            grimmControl.AddGlobalTransition(zerohpevent, "Death Start");



            StartCoroutine(SetGrimmState(grimmControl));

        }

        private PlayMakerFSM CreatePureVessel()
        {
            GameObject hkClone = Instantiate(ResourceLoader.hkprime);
            Destroy(hkClone.GetComponent<ConstrainPosition>());
            hkClone.transform.position = new Vector3(88.948f, 9.2f, 0.004f);

            //lobotomise the pv
            PlayMakerFSM hkControl = hkClone.LocateMyFSM("Control");
            HkOwnerDefault = hkControl.GetState("Stomp Down").GetAction<SetVelocity2d>().gameObject;

            FsmTemplate template = hkControl.FsmTemplate;
            FsmState[] OldStates = hkControl.Fsm.States;
            FsmEvent[] OldEvents = hkControl.Fsm.Events;
            FsmVariables OldVariables = hkControl.Fsm.Variables;

            hkControl.SetFsmTemplate(null);

            hkControl.Fsm.States = OldStates;
            hkControl.Fsm.Events = OldEvents;
            hkControl.Fsm.Variables = OldVariables;

            foreach (FsmState state in hkControl.Fsm.States)
            {
                state.Transitions = new FsmTransition[0].ToArray();
            }

            //anim fps changes
            tk2dSpriteAnimator animator = hkClone.GetComponent<tk2dSpriteAnimator>();
            //animator.GetClipByName("Slash1 Antic").fps = 12;

            ConstrainPosition newconstrain = hkClone.AddComponent<ConstrainPosition>();
            newconstrain.xMin = 72;
            newconstrain.xMax = 102;
            newconstrain.yMin = 8.8f;
            newconstrain.yMax = 100f;
            newconstrain.constrainX = true;
            newconstrain.constrainY = true;

            hkClone.GetComponent<HealthManager>().hp = 99999; //damage delegated to grimm           

            return hkControl;
        }

        private void SetupPureVessel(PlayMakerFSM hkControl)
        {
            FsmOwnerDefault heroownerdefault = hkControl.GetState("TelePos Counter").GetAction<GetPosition>().gameObject;

            //events
            FsmEvent finishedevent = hkControl.GetFsmEvent("FINISHED");
            FsmEvent endevent = hkControl.GetFsmEvent("END");
            FsmEvent landevent = hkControl.GetFsmEvent("LAND");
            FsmEvent dstabevent = hkControl.GetFsmEvent("DSTAB");
            FsmEvent levent = hkControl.GetFsmEvent("L");
            FsmEvent revent = hkControl.GetFsmEvent("R");
            FsmEvent lowhighevent = hkControl.GetFsmEvent("LOWHIGH");
            FsmEvent highlowevent = hkControl.GetFsmEvent("HIGHLOW");
            FsmEvent cancelevent = hkControl.GetFsmEvent("CANCEL");
            FsmEvent feintevent = hkControl.CreateFsmEvent("FEINT");
            FsmEvent wallevent = hkControl.GetFsmEvent("WALL");
            FsmEvent blockedhitevent = hkControl.GetFsmEvent("BLOCKED HIT");
            FsmEvent nextevent = hkControl.CreateFsmEvent("NEXT");
            FsmEvent slashevent = hkControl.CreateFsmEvent("SLASH");
            FsmEvent dashevent = hkControl.CreateFsmEvent("DASH");

            //new events
            FsmEvent duospiketeleoutevent = hkControl.CreateFsmEvent(DuoSpikeHkTeleEvent);
            FsmEvent duoshottelerightevent = hkControl.CreateFsmEvent(DuoShotHkTeleRightEvent);
            FsmEvent duoshotteleleftevent = hkControl.CreateFsmEvent(DuoShotHkTeleLeftEvent);
            FsmEvent duoslashfeintevent = hkControl.CreateFsmEvent(DuoSlashHkFeintEvent);
            FsmEvent duoslashevent = hkControl.CreateFsmEvent(DuoSlashHkEvent);

            //variables
            FsmFloat selfY = hkControl.Fsm.GetFsmFloat("Self Y");
            FsmFloat SelfX = hkControl.Fsm.GetFsmFloat("Self X");
            FsmFloat HeroX = hkControl.Fsm.GetFsmFloat("Hero X");
            FsmFloat HeroY = hkControl.CreateFsmFloat("Hero Y", 0);
            FsmFloat collideLower = hkControl.CreateFsmFloat("Collide Lower", 0);
            FsmFloat collideUpper = hkControl.CreateFsmFloat("Collide Upper", 0);
            FsmGameObject plumestore = hkControl.Fsm.GetFsmGameObject("Plume");
            FsmVector3 ShotVector3 = hkControl.CreateFsmVector3("Shot Vector");
            FsmGameObject shotobject = hkControl.GetState("SmallShot LowHigh").GetAction<FlingObjectsFromGlobalPoolTime>().gameObject;
            FsmGameObject shotspawnpoint = new GameObject("");
            FsmGameObject plumeobject = hkControl.GetState("Plume Gen").GetAction<SpawnObjectFromGlobalPool>(0).gameObject;
            FsmOwnerDefault plumeownerdefault = hkControl.GetState("Plume Gen").GetAction<SetPosition>(4).gameObject;
            FsmFloat TeleAdder = hkControl.Fsm.GetFsmFloat("Tele Adder");
            FsmFloat SelfRangeMin = hkControl.Fsm.GetFsmFloat("SelfRange Min");
            FsmFloat SelfRangeMax = hkControl.Fsm.GetFsmFloat("SelfRangeMax"); //SERIOUSLY??
            FsmFloat TeleRangeMax = hkControl.Fsm.GetFsmFloat("TeleRange Max");
            FsmFloat TeleRangeMin = hkControl.Fsm.GetFsmFloat("TeleRange Min");
            TeleRangeMax.Value = 102;
            TeleRangeMin.Value = 70;
            FsmFloat TeleX = hkControl.Fsm.GetFsmFloat("Tele X");
            FsmFloat TeleY = hkControl.CreateFsmFloat("Tele Y", 0);
            FsmString TeleEvent = hkControl.Fsm.GetFsmString("Tele Event");
            FsmString NextEvent = hkControl.Fsm.GetFsmString("Next Event");
            FsmBool AlternateAttack = hkControl.CreateBool("Alternate Attack");
            FsmFloat PlumeCount = hkControl.CreateFsmFloat("Plume Count", 0);
            FsmBool SoloVersion = hkControl.CreateBool("Solo");

            //solo choice stuff
            FsmInt CtSlash = hkControl.GetOrCreateInt("CT Slash");
            FsmInt MsSlash = hkControl.GetOrCreateInt("Ms Slash");
            FsmInt CtDash = hkControl.GetOrCreateInt("CT Dash");
            FsmInt MsDash = hkControl.GetOrCreateInt("Ms Dash");

            FsmEvent[] SoloEventArray = new FsmEvent[] { slashevent, dashevent };
            FsmFloat[] SoloWeightArray = new FsmFloat[] { 1f, 1f };
            FsmInt[] SoloEventMaxArary = new FsmInt[] { 2, 2 };
            FsmInt[] SoloMissedMaxArray = new FsmInt[] { 2, 2 };
            FsmInt[] SoloTrackingIntsArray = new FsmInt[] { CtSlash, CtDash };
            FsmInt[] SoloTrackingIntsMissedArray = new FsmInt[] { MsSlash, MsDash };



            //begin States!!!1
            FsmState StompDownState = hkControl.GetState("Stomp Down");
            FsmState StompLandState = hkControl.GetState("Stomp Land");
            FsmState PlumeGenState = hkControl.GetState("Plume Gen");
            FsmState BurstPauseState = hkControl.GetState("Burst Pause");
            FsmState PlumeUpState = hkControl.GetState("Plume Up");
            FsmState StompRecoverState = hkControl.GetState("Stomp Recover");
            FsmState PlumePauseState = hkControl.GetState("Plume Pause");
            FsmState DstabAnticState = hkControl.GetState("Dstab Antic");
            FsmState DstabAimState = hkControl.GetState("Dstab Aim");
            FsmState DstabJumpState = hkControl.GetState("Dstab Jump");
            FsmState DstabAirState = hkControl.GetState("Dstab Air");
            FsmState InitState = hkControl.GetState("Init");
            FsmState IdleState = hkControl.GetState("Idle");
            FsmState StompAnticState = hkControl.GetState("Stomp Antic");
            FsmState TeleOutState = hkControl.GetState("Tele Out");
            FsmState TelePosDStabState = hkControl.GetState("TelePos Dstab");
            FsmState TeleInState = hkControl.GetState("Tele In");
            FsmState IntroIdleState = hkControl.GetState("Intro Idle");
            FsmState AfterTeleState = hkControl.GetState("After Tele");
            FsmState TeleChoiceState = hkControl.GetState("Tele Choice P2 3");
            FsmState SmallShotAnticState = hkControl.GetState("SmallShot Antic");
            FsmState SmallShotStartState = hkControl.GetState("SmallShot Start");
            FsmState SmallShotDirState = hkControl.GetState("SmallShot Dir");
            FsmState LWaveState = hkControl.GetState("L Wave");
            FsmState RWaveState = hkControl.GetState("R Wave");
            FsmState SmallShotHighLowState = hkControl.GetState("SmallShot HighLow");
            FsmState SmallShotLowHighState = hkControl.GetState("SmallShot LowHigh");
            FsmState SmallShotRecoverState = hkControl.GetState("SmallShot Recover");
            FsmState TelePosSlashState = hkControl.GetState("TelePos Slash");
            FsmState Slash1AnticState = hkControl.GetState("Slash1 Antic");
            FsmState Slash1State = hkControl.GetState("Slash1");
            FsmState Slash1RecoverState = hkControl.GetState("Slash1 Recover");
            FsmState Slash2State = hkControl.GetState("Slash 2");
            FsmState Slash2RecoverState = hkControl.GetState("Slash2 Recover");
            FsmState Slash3State = hkControl.GetState("Slash 3");
            FsmState Slash3RecoverState = hkControl.GetState("Slash3 Recover");
            FsmState TelePosDashState = hkControl.GetState("TelePos Dash");
            FsmState DashAnticState = hkControl.GetState("Dash Antic");
            FsmState DashState = hkControl.GetState("Dash");
            FsmState DashRecoverState = hkControl.GetState("Dash Recover");
            FsmState DashEndState = hkControl.GetState("Dash End");
            FsmState CounterAnticState = hkControl.GetState("Counter Antic");
            FsmState CounterDirState = hkControl.GetState("Counter Dir");
            FsmState CounterStanceState = hkControl.GetState("Counter Stance");
            FsmState CounterEndState = hkControl.GetState("Counter End");
            FsmState CounterRecolliderState = hkControl.GetState("Recollider");
            FsmState CSDirState = hkControl.GetState("CS Dir");
            FsmState CSAnticState = hkControl.GetState("CS Antic");
            FsmState CSlashState = hkControl.GetState("CSlash");
            FsmState CSlashRecoverState = hkControl.GetState("CSlash Recover");
            FsmState RecoverState = hkControl.GetState("Recover");
            FsmState TelePosFocus = hkControl.GetState("TelePos Focus");
            FsmState FocusChargeState = hkControl.GetState("Focus Charge");
            FsmState BallUpState = hkControl.GetState("Ball Up");
            FsmState FocusBurstState = hkControl.GetState("Focus Burst");
            FsmState HitEndState = hkControl.GetState("Hit End");
            FsmState FocusRecoverState = hkControl.GetState("Focus Recover");
            FsmState TelePosTendrilState = hkControl.GetState("TelePos Tendril");
            FsmState TendrilAnticState = hkControl.GetState("Tendril Antic");
            FsmState TendrilBurstState = hkControl.GetState("Tendril Burst");
            FsmState TendrilStartState = hkControl.GetState("Tendril Start");
            FsmState TendrilHitState = hkControl.GetState("Tendril Hit");
            FsmState T2State = hkControl.GetState("T2");
            FsmState T3State = hkControl.GetState("T3");
            FsmState TendrilHitEndState = hkControl.GetState("Tendril Hit End");
            FsmState TendrilRecoverState = hkControl.GetState("Tendril Recover");

            //custom states
            FsmState TeleOutIndefiniteState = hkControl.CreateState("Tele Out Indefinite");
            TeleOutIndefiniteState.CopyActionData(TeleOutState);

            FsmState SetTeleDstabState = hkControl.CreateState("Set Tele Dstab");
            SetTeleDstabState.AddAction(new SetStringValue { everyFrame = false, stringVariable = NextEvent, stringValue = "DSTAB" });
            SetTeleDstabState.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "DSTAB" });

            FsmState SetTeleSlashState = hkControl.CreateState("Set Tele Slash");
            SetTeleSlashState.AddAction(new SetStringValue { everyFrame = false, stringVariable = NextEvent, stringValue = "SLASH" });
            SetTeleSlashState.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "SLASH" });
            SetTeleSlashState.AddAction(new SetBoolValue { everyFrame = false, boolVariable = AlternateAttack, boolValue = false });
            SetTeleSlashState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f });

            FsmState SetTeleSlashFeintState = hkControl.CreateState("Set Tele Slash Feint");
            SetTeleSlashFeintState.AddAction(new SetStringValue { everyFrame = false, stringVariable = NextEvent, stringValue = "SLASH" });
            SetTeleSlashFeintState.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "SLASH" });
            SetTeleSlashFeintState.AddAction(new SetBoolValue { everyFrame = false, boolVariable = AlternateAttack, boolValue = true });
            SetTeleSlashFeintState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f });

            FsmState SetTeleShotLeftState = hkControl.CreateState("Set Tele Shot Left");
            SetTeleShotLeftState.AddAction(new SetStringValue { everyFrame = false, stringVariable = NextEvent, stringValue = "SMALL SHOT" });
            SetTeleShotLeftState.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "SMALL SHOT" });
            SetTeleShotLeftState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleX, floatValue = 73 });
            SetTeleShotLeftState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f });
            

            FsmState SetTeleShotRightState = hkControl.CreateState("Set Tele Shot Right");
            SetTeleShotRightState.AddAction(new SetStringValue { everyFrame = false, stringVariable = NextEvent, stringValue = "SMALL SHOT" });
            SetTeleShotRightState.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "SMALL SHOT" });
            SetTeleShotRightState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleX, floatValue = 100 });
            SetTeleShotRightState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f });

            FsmState SlashFeintState = hkControl.CreateState("Slash Feint");
            SlashFeintState.AddMethod(() => { SendEventToFSM(DuoSlashGrimmReplaceEvent, GrimmControl, 0.2f); });

            FsmState SetTeleCounterLeftState = hkControl.CreateState("Set Tele Counter Left");
            SetTeleCounterLeftState.AddAction(new SetStringValue { everyFrame = false, stringVariable = NextEvent, stringValue = "COUNTER" });
            SetTeleCounterLeftState.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "COUNTER" });
            SetTeleCounterLeftState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleX, floatValue = 73 });
            SetTeleCounterLeftState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f });

            FsmState SetTeleCounterRightState = hkControl.CreateState("Set Tele Counter Right");
            SetTeleCounterRightState.AddAction(new SetStringValue { everyFrame = false, stringVariable = NextEvent, stringValue = "COUNTER" });
            SetTeleCounterRightState.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "COUNTER" });
            SetTeleCounterRightState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleX, floatValue = 100 });
            SetTeleCounterRightState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f });

            FsmState TelePosMidState = hkControl.CreateState("TelePos Mid");
            TelePosMidState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleX, floatValue = 87f });
            TelePosMidState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f });

            FsmState Balloon1AttackState = hkControl.CreateState("Balloon 1 Attack");
            Balloon1AttackState.CopyActionData(TeleOutState);

            FsmState Balloon1ShotState = hkControl.CreateState("Balloon 1 Shot");
            Balloon1ShotState.AddMethod(() => { shotspawnpoint.Value.transform.position = new Vector3(85, 30); });
            Balloon1ShotState.AddAction(new FlingObjectsFromGlobalPoolTime { gameObject = shotobject, position = new Vector3(-12,0), spawnMax = 1, spawnMin = 1, speedMax = 30f, speedMin = 30f, angleMax = 270f, angleMin = 270f, originVariationX = 3f, originVariationY = 0.4f, spawnPoint = shotspawnpoint, frequency = 0.75f});
            Balloon1ShotState.AddAction(new FlingObjectsFromGlobalPoolTime { gameObject = shotobject, position = new Vector3(14, 0), spawnMax = 1, spawnMin = 1, speedMax = 30f, speedMin = 30f, angleMax = 270f, angleMin = 270f, originVariationX = 3f, originVariationY = 0.4f, spawnPoint = shotspawnpoint, frequency = 0.75f });

            FsmState Balloon2AttackState = hkControl.CreateState("Balloon 2 Attack");
            Balloon2AttackState.CopyActionData(TeleOutState);
            Balloon2AttackState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = PlumeCount, floatValue = 0 });

            FsmState Balloon2SetPlumeState = hkControl.CreateState("Balloon 2 Set Plume");
            Balloon2SetPlumeState.AddAction(new SpawnObjectFromGlobalPool { gameObject = plumeobject, position = new Vector3(0, 0, -0.05f), rotation = new Vector3(0, 0), storeObject = plumestore, spawnPoint = new GameObject() });
            Balloon2SetPlumeState.AddAction(new GetPosition { gameObject = heroownerdefault, space = 0, everyFrame = false, vector = new Vector3(0, 0), x = HeroX, y = 0, z = 0 });
            Balloon2SetPlumeState.AddAction(new SetPosition { gameObject = plumeownerdefault, vector = new Vector3(0, 0), x = HeroX, y = 4.2f, z = 0, space = 0, everyFrame = false, lateUpdate = false });
            Balloon2SetPlumeState.AddAction(PlumeGenState.GetAction<AudioPlayerOneShotSingle>(1));
            Balloon2SetPlumeState.AddAction(new Wait { realTime = false, time = 0.75f, finishEvent = nextevent });

            FsmState Balloon2PlumeActiveState = hkControl.CreateState("Balloon 2 Plume Active");
            Balloon2PlumeActiveState.AddAction(PlumeUpState.GetAction<SendEventByName>(5));
            Balloon2PlumeActiveState.AddAction(PlumeUpState.GetAction<AudioPlayerOneShotSingle>(1));
            Balloon2PlumeActiveState.AddAction(PlumeUpState.GetAction<SendEventByName>(6));
            Balloon2PlumeActiveState.AddAction(new FloatAdd { everyFrame = false, floatVariable = PlumeCount, add = 1 , perSecond = false});
            Balloon2PlumeActiveState.AddAction(new FloatCompare { float1 = PlumeCount, float2 = 10, lessThan = nextevent, tolerance = 0, everyFrame = false });

            FsmState Balloon3AttackState = hkControl.CreateState("Balloon 3 Attack");
            Balloon3AttackState.CopyActionData(TeleOutState);
            Balloon3AttackState.AddAction(new SetBoolValue { boolVariable = SoloVersion, boolValue = true, everyFrame = false });

            FsmState SoloMoveChoice = hkControl.CreateState("Solo Move Choice");
            SoloMoveChoice.AddAction(new SetVelocity2d { everyFrame = false, gameObject = HkOwnerDefault, vector = new Vector2(0, 0), x = 0, y = 0 });
            SoloMoveChoice.AddAction(new SendRandomEventV3 { events = SoloEventArray, weights = SoloWeightArray, trackingInts = SoloTrackingIntsArray, eventMax = SoloEventMaxArary, trackingIntsMissed = SoloTrackingIntsMissedArray, missedMax = SoloMissedMaxArray });

            FsmState ContinueTeleOutState = hkControl.CreateState("Continue Tele Out");
            ContinueTeleOutState.AddAction(new SetPosition { everyFrame = true, gameObject = HkOwnerDefault, vector = new Vector3(0, 0), lateUpdate = false, space = 0, y = 95, x = 87 });
            ContinueTeleOutState.AddAction(new SetIsKinematic { gameObject = HkOwnerDefault, isKinematic = true });
            ContinueTeleOutState.AddAction(new SetVelocity2d { everyFrame = true, gameObject = HkOwnerDefault, vector = new Vector2(0, 0), x = 0, y = 0 });                  
            ContinueTeleOutState.AddAction(new BoolTest { boolVariable = SoloVersion, everyFrame = false, isTrue = nextevent });

            FsmState StopSoloState = hkControl.CreateState("Stop Solo");
            StopSoloState.AddAction(new SetBoolValue { boolVariable = SoloVersion, boolValue = false, everyFrame = false });

            FsmState CheckShotCollideState = hkControl.CreateState("Check Shot Collide");
            CheckShotCollideState.AddAction(new GetPosition { everyFrame = false, gameObject = heroownerdefault, space = 0, vector = new Vector3(0, 0), x = HeroX });
            CheckShotCollideState.AddAction(new FloatCompare { everyFrame = false, float1 = HeroX, float2 = TeleX, tolerance = 1.7f, equal = cancelevent });                       

            //state modifications.
            TelePosDStabState.GetAction<FloatClamp>().minValue = 73;
            TelePosDStabState.GetAction<FloatClamp>().maxValue = 101;
            StompDownState.GetAction<CheckCollisionSideEnter>().bottomHitEvent = finishedevent;
            StompDownState.GetAction<CheckCollisionSide>().bottomHitEvent = finishedevent;

            StompRecoverState.AddMethod(() => { SendEventToFSM(DuoSpikeGrimmContinueEvent,GrimmControl,0.25f); });

            PlumePauseState.GetAction<Wait>().time = 0.025f; //reduces time between plume spawn and activation, originally 0.05f;

            TeleOutState.RemoveAction(8);
            TeleOutState.AddAction(new SetInvincible { Invincible = SoloVersion, InvincibleFromDirection = 0, target = HkOwnerDefault });

            TeleInState.GetAction<SetPosition>().y = TeleY;

            TelePosDStabState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 15.38f });

            Slash1State.InsertAction(0, new BoolTest { boolVariable = AlternateAttack, everyFrame = false, isTrue = feintevent }); //adds feint

            TelePosSlashState.GetAction<RandomFloat>().min = 3.5f; //shortens distance from player for teleport to slash
            TelePosSlashState.GetAction<RandomFloat>().max = 4f;

            DashState.AddMethod(() => { SendEventToFSM(DuoSlashGrimmReplaceEvent, GrimmControl, 0); }); //in case the slash cancels

            TelePosDashState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f }); //set y for dash
            TelePosDashState.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "DASH" });

            TelePosFocus.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "FOCUS" });
            TelePosFocus.AddAction(new GetPosition { everyFrame = false, gameObject = heroownerdefault, space = 0, vector = new Vector3(0, 0), x = HeroX, y = 0, z = 0 });
            TelePosFocus.AddAction(new RandomFloat { min = 6f, max = 10f, storeResult = TeleAdder });
            TelePosFocus.AddAction(new FloatOperator { float1 = HeroX, float2 = TeleAdder, operation = 0, storeResult = TeleX, everyFrame = false });
            TelePosFocus.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f });
            TelePosFocus.AddAction(new FloatInRange { floatVariable = TeleX, lowerValue = TeleRangeMin, upperValue = TeleRangeMax, boolVariable = false, falseEvent = cancelevent, everyFrame = false });
            TelePosFocus.AddAction(new FloatInRange { floatVariable = TeleX, lowerValue = SelfRangeMin, upperValue = SelfRangeMax, boolVariable = false, trueEvent = cancelevent, everyFrame = false });

            BallUpState.GetAction<Wait>().time = 0.5f; //reduces time between focus ball and burst

            TelePosTendrilState.AddAction(new SetFloatValue { everyFrame = false, floatVariable = TeleY, floatValue = 9.4f }); //set y for dash
            TelePosTendrilState.AddAction(new SetStringValue { everyFrame = false, stringVariable = TeleEvent, stringValue = "TENDRIL" });
           

            //transitions
            StompDownState.AddTransition(landevent, "Stomp Land"); //dstab states
            StompAnticState.AddTransition(finishedevent, "Stomp Down");
            StompLandState.AddTransition(finishedevent, "Plume Gen");
            PlumeGenState.AddTransition(finishedevent, "Plume Pause");
            PlumePauseState.AddTransition(finishedevent, "Plume Gen");
            PlumeGenState.AddTransition(endevent, "Burst Pause");
            BurstPauseState.AddTransition(finishedevent, "Plume Up");
            PlumeUpState.AddTransition(finishedevent, "Stomp Recover");
            DstabAnticState.AddTransition(finishedevent, "Dstab Aim");
            DstabAimState.AddTransition(finishedevent, "Dstab Jump");
            DstabJumpState.AddTransition(finishedevent, "Dstab Air");
            DstabAirState.AddTransition(finishedevent, "Stomp Antic");
            DstabAirState.AddTransition(dstabevent, "Stomp Antic");
            TeleOutState.AddTransition(finishedevent, "Tele Choice P2 3");
            TeleChoiceState.AddTransition("DSTAB", "TelePos Dstab");
            TeleChoiceState.AddTransition("SMALL SHOT", "Tele In"); //telepossmallshot is not used because of L and R
            TeleChoiceState.AddTransition("SLASH", "TelePos Slash");
            TeleChoiceState.AddTransition("COUNTER", "Tele In");
            TeleChoiceState.AddTransition("FOCUS", "Tele In");
            TeleChoiceState.AddTransition("TENDRIL", "Tele In");
            TelePosDStabState.AddTransition(finishedevent, "Tele In");
            TeleInState.AddTransition(finishedevent, "After Tele");
            AfterTeleState.AddTransition("DSTAB", "Stomp Antic"); //NextEvent is sent in aftertele
            AfterTeleState.AddTransition("SMALL SHOT", "SmallShot Antic");
            AfterTeleState.AddTransition("SLASH", "Slash1 Antic");
            AfterTeleState.AddTransition("DASH", "Dash Antic");
            AfterTeleState.AddTransition("COUNTER", "Counter Antic");
            AfterTeleState.AddTransition("FOCUS", "Focus Charge");
            AfterTeleState.AddTransition("TENDRIL", "Tendril Antic");
            StompDownState.AddTransition(finishedevent, "Stomp Land");
            StompRecoverState.AddTransition(finishedevent, "Tele Out Indefinite");
            SetTeleDstabState.AddTransition(finishedevent, "Tele Out");
            SmallShotAnticState.AddTransition(finishedevent, "SmallShot Start"); //smallshot states
            SmallShotStartState.AddTransition(finishedevent, "SmallShot Dir");
            SetTeleShotLeftState.AddTransition(finishedevent, "Check Shot Collide");
            SetTeleShotRightState.AddTransition(finishedevent, "Check Shot Collide");
            CheckShotCollideState.AddTransition(finishedevent, "Tele Out");
            CheckShotCollideState.AddTransition(cancelevent, "TelePos Mid");
            SmallShotDirState.AddTransition(levent, "L Wave");
            SmallShotDirState.AddTransition(revent, "R Wave");
            LWaveState.AddTransition(lowhighevent, "SmallShot LowHigh");
            LWaveState.AddTransition(highlowevent, "SmallShot HighLow");
            RWaveState.AddTransition(lowhighevent, "SmallShot LowHigh");
            RWaveState.AddTransition(highlowevent, "SmallShot HighLow");
            SmallShotHighLowState.AddTransition(finishedevent, "SmallShot Recover");
            SmallShotLowHighState.AddTransition(finishedevent, "SmallShot Recover");
            SmallShotRecoverState.AddTransition(finishedevent, "Tele Out Indefinite");
            
            TelePosSlashState.AddTransition(finishedevent, "Tele In"); //slash states
            TelePosSlashState.AddTransition(cancelevent, "TelePos Dash");
            SetTeleSlashFeintState.AddTransition(finishedevent, "Tele Out");
            SetTeleSlashState.AddTransition(finishedevent, "Tele Out");
            Slash1AnticState.AddTransition(finishedevent, "Slash1");
            Slash1State.AddTransition(finishedevent, "Slash1 Recover");
            Slash1State.AddTransition(feintevent, "Slash Feint");
            Slash1RecoverState.AddTransition(finishedevent, "Slash 2");
            SlashFeintState.AddTransition(finishedevent, "Tele Out Indefinite");
            Slash2State.AddTransition(finishedevent, "Slash2 Recover");
            Slash2RecoverState.AddTransition(finishedevent, "Slash 3");
            Slash3State.AddTransition(finishedevent, "Slash3 Recover");
            Slash3RecoverState.AddTransition(finishedevent, "Tele Out Indefinite");

            TelePosDashState.AddTransition(finishedevent, "Tele In"); //dash stuff
            TelePosDashState.AddTransition(cancelevent, "TelePos Mid");
            DashAnticState.AddTransition(finishedevent, "Dash");
            DashState.AddTransition(endevent, "Dash Recover");
            DashState.AddTransition(wallevent, "Dash Recover");
            DashRecoverState.AddTransition(finishedevent, "Dash End");
            DashEndState.AddTransition(finishedevent, "Tele Out Indefinite");

            SetTeleCounterLeftState.AddTransition(finishedevent, "Tele Out"); //counter stuff
            SetTeleCounterRightState.AddTransition(finishedevent, "Tele Out");
            CounterAnticState.AddTransition(finishedevent, "Counter Dir");
            CounterDirState.AddTransition(finishedevent, "Counter Stance");
            CounterStanceState.AddTransition(endevent, "Counter End");
            CounterStanceState.AddTransition(blockedhitevent, "CS Dir");
            CounterEndState.AddTransition(finishedevent, "Recollider");
            CounterRecolliderState.AddTransition(finishedevent, "Tele Out Indefinite");
            CSDirState.AddTransition(finishedevent, "CS Antic");
            CSAnticState.AddTransition(finishedevent, "CSlash");
            CSlashState.AddTransition(finishedevent, "CSlash Recover");
            CSlashRecoverState.AddTransition(finishedevent, "Recover");
            RecoverState.AddTransition(finishedevent, "Tele Out Indefinite");
            TelePosMidState.AddTransition(finishedevent, "Tele Out");

            TelePosFocus.AddTransition(finishedevent, "Tele Out"); //focus
            TelePosFocus.AddTransition(cancelevent, "TelePos Mid");
            FocusChargeState.AddTransition(finishedevent, "Ball Up");
            BallUpState.AddTransition(finishedevent, "Focus Burst");
            FocusBurstState.AddTransition(finishedevent, "Hit End");
            HitEndState.AddTransition(finishedevent, "Focus Recover");
            FocusRecoverState.AddTransition(finishedevent, "Tele Out Indefinite");

            TelePosTendrilState.AddTransition(finishedevent, "Tele Out"); //tendrils
            TelePosTendrilState.AddTransition(cancelevent, "TelePos Mid");
            TendrilAnticState.AddTransition(finishedevent, "Tendril Burst");
            TendrilBurstState.AddTransition(finishedevent, "Tendril Start");
            TendrilStartState.AddTransition(finishedevent, "Tendril Hit");
            TendrilHitState.AddTransition(finishedevent, "T2");
            T2State.AddTransition(finishedevent, "T3");
            T3State.AddTransition(finishedevent, "Tendril Hit End");
            TendrilHitEndState.AddTransition(finishedevent, "Tendril Recover");
            TendrilRecoverState.AddTransition(finishedevent, "Tele Out Indefinite");

            Balloon1AttackState.AddTransition(finishedevent, "Balloon 1 Shot");
            Balloon2AttackState.AddTransition(finishedevent, "Balloon 2 Set Plume");
            Balloon2SetPlumeState.AddTransition(nextevent, "Balloon 2 Plume Active");
            Balloon2PlumeActiveState.AddTransition(nextevent, "Balloon 2 Set Plume");
            Balloon3AttackState.AddTransition(finishedevent, "Solo Move Choice");

            SoloMoveChoice.AddTransition(dashevent, "TelePos Dash");
            SoloMoveChoice.AddTransition(slashevent, "Set Tele Slash");
            ContinueTeleOutState.AddTransition(nextevent, "Solo Move Choice");
            TeleOutIndefiniteState.AddTransition(finishedevent, "Continue Tele Out");
            StopSoloState.AddTransition(finishedevent, "Tele Out Indefinite");



            //global transitions
            hkControl.AddGlobalTransition(duospiketeleoutevent, "Set Tele Dstab");
            hkControl.AddGlobalTransition(duoshotteleleftevent, "Set Tele Shot Left");
            hkControl.AddGlobalTransition(duoshottelerightevent, "Set Tele Shot Right");
            hkControl.AddGlobalTransition(DuoShotHkCounterTeleLeftEvent, "Set Tele Counter Left");
            hkControl.AddGlobalTransition(DuoShotHkCounterTeleRightEvent, "Set Tele Counter Right");
            hkControl.AddGlobalTransition(duoslashfeintevent, "Set Tele Slash Feint");
            hkControl.AddGlobalTransition(duoslashevent, "Set Tele Slash");
            hkControl.AddGlobalTransition(DuoHkDashEvent, "TelePos Dash");
            hkControl.AddGlobalTransition(DuoHkFocusEvent, "TelePos Focus");
            hkControl.AddGlobalTransition(DuoTendrilHkEvent, "TelePos Tendril");
            hkControl.AddGlobalTransition(DuoBalloon1Event, "Balloon 1 Attack");
            hkControl.AddGlobalTransition(DuoBalloon2Event, "Balloon 2 Attack");
            hkControl.AddGlobalTransition(DuoBalloon3Event, "Balloon 3 Attack");
            hkControl.AddGlobalTransition(StopSoloEvent, "Stop Solo");

            //StompRecoverState.AddTransition("FINISHED", );
            //temporary
            InitState.AddTransition(finishedevent, "Intro Idle");



            //test
            hkControl.gameObject.SetActive(true);
            StartCoroutine(SetHkState(hkControl));
        }

        private IEnumerator SetGrimmState(PlayMakerFSM grimmcontrol)
        {
            yield return new WaitForFinishedEnteringScene();
            grimmcontrol.SetState("Dormant");
        }

        private IEnumerator SetHkState(PlayMakerFSM hkcontrol)
        {
            yield return new WaitForFinishedEnteringScene();
            hkcontrol.SetState("Init");
        }

        private void SendEventToFSM(FsmEvent fsmEvent, PlayMakerFSM fsm, float delay)
        {
            StartCoroutine(SendEventToFSMDelayed(fsmEvent, fsm, delay));
        }

        private IEnumerator SendEventToFSMDelayed(FsmEvent fsmEvent, PlayMakerFSM fsm, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (fsm != null)
            {
                fsm.ChangeState(fsmEvent);
            }
        }
    }
}

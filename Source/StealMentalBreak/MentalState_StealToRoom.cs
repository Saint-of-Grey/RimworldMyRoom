using System.Collections.Generic;
using System.Linq;
using MyRoom.Common;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MyRoom
{
    public class MentalState_StealToRoom : MentalState
    {
        public Thing target;
        public int lastInsultTicks = -999999;
        public bool insultedTargetAtLeastOnce;
        private int targetFoundTicks;

        // Token: 0x04002674 RID: 9844
        private const int CheckChooseNewTargetIntervalTicks = 250;

        // Token: 0x04002675 RID: 9845
        private const int MaxSameTargetChaseTicks = 1250;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref targetFoundTicks, "targetFoundTicks");
            Scribe_References.Look(ref target, "target");
            Scribe_Values.Look(ref insultedTargetAtLeastOnce, "insultedTargetAtLeastOnce");
            Scribe_Values.Look(ref lastInsultTicks, "lastInsultTicks");
        }

        public override RandomSocialMode SocialModeMax()
        {
            return RandomSocialMode.Quiet;
        }

        // Token: 0x06003C52 RID: 15442 RVA: 0x001C608D File Offset: 0x001C448D
        public override void PostStart(string reason)
        {
            base.PostStart(reason);
            ChooseNextTarget();
        }

        // Token: 0x06003C53 RID: 15443 RVA: 0x001C609C File Offset: 0x001C449C
        public override void MentalStateTick()
        {
            if (target != null && pawn.CanReach(target.Position, PathEndMode.Touch, Danger.Some))
            {
                ChooseNextTarget();
            }

            if (pawn.IsHashIntervalTick(250) && (target == null || insultedTargetAtLeastOnce))
            {
                ChooseNextTarget();
            }

            base.MentalStateTick();
        }

        // Token: 0x06003C54 RID: 15444 RVA: 0x001C610C File Offset: 0x001C450C
        private void ChooseNextTarget()
        {
            List<Thing> candidates = Candidates();
            if (!candidates.Any())
            {
                target = null;
                insultedTargetAtLeastOnce = false;
                targetFoundTicks = -1;
            }
            else
            {
                Thing thing;
                if (target != null && Find.TickManager.TicksGame - targetFoundTicks > 1250 &&
                    candidates.Any(x => x != target))
                {
                    thing = candidates.Where(x=>x != target).RandomElementByWeight(GetCandidateWeight);
                }
                else
                {
                    thing = candidates.RandomElementByWeight(GetCandidateWeight);
                }

                if (thing != target)
                {
                    target = thing;
                    insultedTargetAtLeastOnce = false;
                    targetFoundTicks = Find.TickManager.TicksGame;
                }
            }
        }

        private List<Thing> Candidates()
        {
            var movable = pawn.Map.listerThings.AllThings.Where(x =>
                    pawn.CanReserve(x) && x.def.Minifiable && InstallBlueprintUtility.ExistingBlueprintFor(x) == null)
                .ToList();
            var myRoom = RoomUtilities.MyRoom(pawn.MyBeds());
            foreach (var room in myRoom)
            foreach (var thing in room.ContainedAndAdjacentThings)
            {
                movable.Remove(thing);
            }

            return movable;
        }

        // Token: 0x06003C55 RID: 15445 RVA: 0x001C6204 File Offset: 0x001C4604
        private float GetCandidateWeight(Thing candidate)
        {
            float num = pawn.Position.DistanceTo(candidate.Position);
            float num2 = Mathf.Min(num / 40f, 1f);
            return (1f - num2 + 0.01f) * candidate.GetBeautifulValue();
        }
    }
}
//#define DEBUG

using UnityEngine;
using Verse;

namespace MyRoom
{
    public class MyRoom : Mod
    {
        public static MyModSettings latest;

        public MyRoom(ModContentPack content) : base(content)
        {
            this.Settings = GetSettings<MyModSettings>();
            latest = Settings;
        }

        public MyModSettings Settings { get; set; }
        
        public override string SettingsCategory() => "MyRoom - I want that!";
        
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.TimerMultipier = Widgets.HorizontalSlider(
                inRect.TopHalf().TopHalf().TopHalf().ContractedBy(4f),
                Settings.TimerMultipier, .75f, 100f, true,
                "Frequency of Install"
                , "Common", "Rare");

            this.Settings.Write();
        }
    }

    public class MyModSettings : ModSettings
    {
        public float TimerMultipier = 1.0f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.TimerMultipier, "TimerMultipier", 0.01f);
        }
    }
}
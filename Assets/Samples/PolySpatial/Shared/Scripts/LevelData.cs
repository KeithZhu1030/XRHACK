using UnityEngine;

namespace PolySpatial.Samples
{
    public class LevelData : MonoBehaviour
    {
        public enum LevelTypes
        {
            Bison,
            Giraffe,
            Rhino,
            Antelope,
            RedPanda,
            Tiger,
            Empty
        }

        const string BisonTitle = "European Bison";
        const string GiraffeTitle = "Kordofan Giraffe";
        const string RhinoTitle = "Javan Rhinoceros";
        const string AntelopeTitle = "Saiga Antelope";
        const string RedPandaTitle = "Red Panda";
        const string TigerTitle = "Siberian Tiger";
        const string EmptyTitle = "What's Coming?";

        const string CharacterNavigationDescription = "Blah blah.";

        const string TargetedInputDescription =
            "Blah blah 2.";

        const string BisonDescription = "2026: Degradation of plateau grasslands\n2029: Infectious diseases from live stocks\n2033: Militarization of habitats";
        const string GiraffeDescription = "2027: Massive habitat loss\n2030: Insufficient food supply\n2033: New diseases";
        const string RhinoDescription = "2026: Poaching rebounds\n2029: International controls weak\n2032: Loss of genetic diversity";
        const string AntelopeDescription = "2025: Overgrazing and drought\n2031: Illegal poaching\n2033: Rising road fatalities";
        const string RedPandaDescription = "2027: bamboo forest decline and climate change\n2031: habitat disturbance\n2034: poaching and pet trade threats";
        const string TigerDescription = "2028: Habitat fragmentation\n2030: Prey decline\n2034: Poaching and illegal trade rebound";
        const string EmptyDescription = "In 2040,\nanother 38 species are at risk of extinction.\nTake action now to protect their future.";

        public string GetLevelTitle(LevelTypes levelType)
        {
            switch (levelType)
            {
                case LevelTypes.Bison:
                    return BisonTitle;
                case LevelTypes.Giraffe:
                    return GiraffeTitle;
                case LevelTypes.Rhino:
                    return RhinoTitle;
                case LevelTypes.Antelope:
                    return AntelopeTitle;
                case LevelTypes.RedPanda:
                    return RedPandaTitle;
                case LevelTypes.Tiger:
                    return TigerTitle;
                case LevelTypes.Empty:
                    return EmptyTitle;
                default:
                    return "";
            }
        }

        public string GetLevelDescription(LevelTypes levelType)
        {
            switch (levelType)
            {
                case LevelTypes.Bison:
                    return BisonDescription;
                case LevelTypes.Giraffe:
                    return GiraffeDescription;
                case LevelTypes.Rhino:
                    return RhinoDescription;
                case LevelTypes.Antelope:
                    return AntelopeDescription;
                case LevelTypes.RedPanda:
                    return RedPandaDescription;
                case LevelTypes.Tiger:
                    return TigerDescription;
                case LevelTypes.Empty:
                    return EmptyDescription;
                default:
                    return "";
            }
        }
    }
}

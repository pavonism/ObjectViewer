
namespace SurfaceFiller
{
    public static class Defaults
    {
        public const float ThreadsCount = 0.5f;
        public const float KDParameter = 0.8f;
        public const float KSParameter = 0.3f;
        public const float MParameter = 0.5f;

        public const float AnimationSpeed = 0.1f;
        public const float LightLocationZ = 0.2f;
    }

    public static class Resources
    {
        public const string ProgramTitle = "SurfaceFiller  \u25B2";

        public const string TextureAssets = @"..\..\..\..\Assets\Textures";
        public const string NormalMapsAssets = @"..\..\..\..\Assets\NormalMaps";
        public const string ObjectAssets = @"..\..\..\..\Assets\Objects";
        public const string DefaultObject = "Sphere";
    }

    public static class FormConstants
    {
        public const int MinimumWindowSizeX = 700;
        public const int MinimumWindowSizeY = 750;

        public const int InitialWindowSizeX = 900;
        public const int InitialWindowSizeY = 750;

        public const int MainFormColumnCount = 2;
        public const int MinimumControlSize = 32;
        public const int ToolbarWidth = 7 * MinimumControlSize;
        public const int SliderWidth = 3 * MinimumControlSize;
        public const int LabelWidth = 2 * MinimumControlSize;
    }

    public static class Labels
    {
        public const string ThreadSlider = "Threads";
        public const string ShowLinesOption = "Show lines";
        public const string VectorsInterpolationOption = "Vectors Interpolation";
        public const string ColorInterpolationOption = "Color Interpolation";
        public const string ModelParameters = "Model Parameters";
        public const string KDParameter = "KD =";
        public const string KSParameter = "KS =";
        public const string MParameter = "M =";
        public const string LightSection = "Sun Parameters";
        public const string Speed = "Speed";
        public const string XLocation = "X";
        public const string YLocation = "Y";
        public const string ZLocation = "Z";
        public const string ObjectSurface = "Object Surface";
        public const string NormalMapOption = "NormalMap";
    }

    public static class Glyphs
    {
        public const string Palette = "\U0001F3A8";
        public const string Spiral = "\U0001F300";
        public const string Reset = "\U0001F504";
        public const string File = "\U0001F4C2";
        public const string Bucket = "\U0001FAA3";
        public const string Rewind = "\u23EA";
        public const string Forward = "\u23E9";
        public const string Play = "\u25B6";
        public const string Pause = "\u23F8";
    }

    public static class Hints
    {
        public const string OpenOBJ = "Załaduj powierzchnię z pliku *.obj";
        public const string Fill = "Włącz / wyłącz wypełenienie powierzchni";
        public const string ShowLines = "Pokaż linie triangulacji";
        public const string ColorInterpolation = "Włącz tryb interpolacji kolorów";
        public const string VectorInterpolation = "Włącz tryb interpolacji wektorów normalnych";
        public const string ChangeLightColor = "Zmień kolor źródła światła";
        public const string ShowTrack = "Pokaż / ukryj trajektorię poruszania się światła";
        public const string ResetPosition = "Resetuj położenie źródła światła do punktu początkowego";
        public const string ChangeObjectColor = "Zmień kolor powierzchni";
        public const string LoadObjectPattern = "Wczytaj z pliku teksturę powierzchni";
        public const string LoadNormalMap = "Wczytaj z pliku mapę wektorów normalnych";
    }
}

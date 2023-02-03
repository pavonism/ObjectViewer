
using System.CodeDom;

namespace SurfaceFiller
{
    public static class Defaults
    {
        public const float KDParameter = 0.8f;
        public const float KSParameter = 0.3f;
        public const float MParameter = 0.5f;

        public const float ViewDistance = 0.5f;
        public const float FOV = 0.75f;
        public const float CameraXLocation = 0.9f;
        public const float CameraYLocation = 0.9f;
        public const float CameraZLocation = 0.7f;
    }

    public static class Resources
    {
        public const string ProgramTitle = "ObjectViewer  \u25B2";

        public const string TextureAssets = @"..\..\..\..\Assets\Textures";
        public const string NormalMapsAssets = @"..\..\..\..\Assets\NormalMaps";
        public const string ObjectAssets = @"..\..\..\..\Assets\Objects";
        public const string TorusFile = @"..\..\..\..\Assets\Objects\FullDonut.obj";
        public const string SphereFile = @"..\..\..\..\Assets\Objects\Sphere.obj";

        public static Color ThemeColor = Color.FromArgb(255, 0, 120, 215);
    }

    public static class FormConstants
    {
        public const int MinimumWindowSizeX = 700;
        public const int MinimumWindowSizeY = 800;

        public const int InitialWindowSizeX = 900;
        public const int InitialWindowSizeY = 700;

        public const int MainFormColumnCount = 2;
        public const int MinimumControlSize = 32;
        public const int ToolbarWidth = 7 * MinimumControlSize;
        public const int SliderWidth = 3 * MinimumControlSize;
        public const int LabelWidth = 2 * MinimumControlSize;
    }

    public static class Labels
    {
        public const string ApplyButtonLabel = "Add";
        public const string ThreadSlider = "Threads";
        public const string ShowLinesOption = "Pokaż linie";
        public const string FillObjectsOption = "Wypełnianie obiektów";
        public const string Shaders = "Cieniowania";
        public const string PhongShader = "Cieniowanie Phonga";
        public const string GouraudShader = "Cieniowanie Gourauda";
        public const string ConstShader = "Cieniowanie płaskie";
        public const string Camera = "Kamera";
        public const string Fog = "Mgła";
        public const string CameraX = "X";
        public const string CameraY = "Y";
        public const string CameraZ = "Z";
        public const string FOV = "FOV";
        public const string ShaderParameters = "Parametry cieniowania";
        public const string KDParameter = "KD =";
        public const string KSParameter = "KS =";
        public const string MParameter = "M =";
        public const string ObjectSurface = "Object Surface";
        public const string NormalMapOption = "NormalMap";
        public const string StaticCamera = "Kamera statyczna";
        public const string FollowingCamera = "Kamera śledząca";
        public const string FirstPersonCamera = "Kamera pierwszoosobowa";
        public const string ThirdPersonCamera = "Kamera trzecioosobowa";
        public const string FreeCamera = "Kamera swobodna";
        public const string SolidShader = "Bez cieniowania";
        public const string Day = "Dzień";
        public const string Night = "Noc";
        public const string Vibrations = "Drgania";
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
        public const string FillObjects = "Włącz / wyłącz wypełnianie obiektów";
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

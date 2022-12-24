using SketcherControl;
using SketcherControl.Filling;
using SurfaceFiller.Components;
using SurfaceFiller.Samples;
using System.Drawing.Imaging;
using System.Numerics;
using ToolbarControls;

namespace SurfaceFiller
{
    public partial class MainForm : Form
    {
        private TableLayoutPanel mainTableLayout = new();
        private Toolbar toolbar = new() { Width = FormConstants.ToolbarWidth };
        private FlowLayoutPanel fillSection;
        private SceneViewer sceneViewer;
        private Scene scene;
        private ICombo<BasicSample> objectCombo;
        private ComboPickerWithImage<Sample> objectSurfaceCombo;
        private ComboPickerWithImage<Sample> normalMapCombo;
        private ColorSample colorSample;

        public MainForm()
        {
            InitializeScene();
            InitializeToolbar();
            ArrangeComponents();
            InitializeForm();
            LoadTextureSamples();
            LoadNormalMapSamples();
            LoadObjectSamples();
        }

        private void InitializeScene()
        {
            this.scene = new();
            this.sceneViewer = new(this.scene);
        }

        #region Initialization
        private void InitializeToolbar()
        {
            this.toolbar.AddLabel(Resources.ProgramTitle);
            this.toolbar.AddDivider();
            this.objectCombo = this.toolbar.AddComboApply<BasicSample>(ObjectPickedHandler, Labels.ApplyButtonLabel);
            this.toolbar.AddButton(OpenFileHandler, Glyphs.File, Hints.OpenOBJ);
            this.toolbar.AddButton(ClearHandler, Glyphs.Reset);
            //this.toolbar.AddSlider(ThreadsSlidrerHandler, Labels.ThreadSlider, Defaults.ThreadsCount);
            this.toolbar.AddDivider();
            this.toolbar.AddOption(ShowLinesHandler, Labels.ShowLinesOption, Hints.ShowLines, false);
            this.toolbar.AddOption(FillObjectsHandler, Labels.FillObjectsOption, Hints.FillObjects, true);
            this.toolbar.StartSection();
            this.toolbar.AddRadioOption(ColorInterpolationOptionHandler, Labels.ColorInterpolationOption, Hints.ColorInterpolation, true);
            this.toolbar.AddRadioOption(VectorInterpolationOptionHandler, Labels.VectorsInterpolationOption, Hints.VectorInterpolation);
            this.toolbar.EndSection();
            this.toolbar.AddDivider();
            this.toolbar.AddSlider(FovSliderHandler, "FOV", 0.35f);
            this.toolbar.AddSlider(CameraXHandler, "CameraX");
            this.toolbar.AddSlider(CameraYHandler, "CameraY");
            this.toolbar.AddSlider(CameraZHandler, "CameraZ");
            this.toolbar.AddDivider();
            this.fillSection = this.toolbar.StartSection();
            this.toolbar.AddLabel(Labels.ModelParameters);
            this.toolbar.AddFractSlider(KDParameterHandler, Labels.KDParameter, Defaults.KDParameter);
            this.toolbar.AddFractSlider(KSParameterHandler, Labels.KSParameter, Defaults.KSParameter);
            this.toolbar.AddSlider(MParameterHandler, Labels.MParameter, Defaults.MParameter);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.LightSection);
            this.toolbar.AddPlayPouse(SunHandler, false);
            this.toolbar.AddProcessButton(RewindHandler, Glyphs.Rewind);
            this.toolbar.AddProcessButton(MoveForwardHandler, Glyphs.Forward);
            this.toolbar.AddButton(LightColorButtonHandler, Glyphs.Palette, Hints.ChangeLightColor);
            this.toolbar.AddTool(ShowTrackHandler, Glyphs.Spiral, Hints.ShowTrack);
            this.toolbar.AddButton(ResetPositionButtonHandler, Glyphs.Reset, Hints.ResetPosition);
            this.toolbar.AddSlider(SunSpeedHanlder, Labels.Speed, Defaults.AnimationSpeed);
            this.toolbar.AddSlider(SunZLocationHandler, Labels.ZLocation, Defaults.LightLocationZ);
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.ObjectSurface);
            this.objectSurfaceCombo = this.toolbar.AddComboImagePicker<Sample>(TexturePickedHandler);
            this.toolbar.AddButton(ObjectColorButtonHandler, Glyphs.Palette, Hints.ChangeObjectColor);
            this.toolbar.AddButton(LoadTextureHandlar, Glyphs.File, Hints.LoadObjectPattern);
            this.normalMapCombo = this.toolbar.AddComboImagePicker<Sample>(NormalMapPickedHandler);
            this.toolbar.AddButton(VectorMapHandler, Glyphs.File, Hints.LoadNormalMap);
            this.toolbar.EndSection();
        }

        private void CameraZHandler(float obj)
        {
            this.sceneViewer.CameraVector = new Vector3()
            {
                X = this.sceneViewer.CameraVector.X,
                Y = this.sceneViewer.CameraVector.Y,
                Z = 1 + 5 * obj,
            };
        }

        private void CameraYHandler(float obj)
        {
            this.sceneViewer.CameraVector = new Vector3()
            {
                X = this.sceneViewer.CameraVector.X,
                Y = 1 + 5 * obj,
                Z = this.sceneViewer.CameraVector.Z,
            };
        }

        private void CameraXHandler(float obj)
        {
            this.sceneViewer.CameraVector = new Vector3()
            {
                X = 1 + 5 * obj,
                Y = this.sceneViewer.CameraVector.Y,
                Z = this.sceneViewer.CameraVector.Z,
            };
        }

        private void FovSliderHandler(float obj)
        {
            this.sceneViewer.FOV = (float)(Math.PI / 6 + obj * Math.PI / 2);
        }

        private void ClearHandler(object? sender, EventArgs e)
        {
            this.scene.Clear();
        }

        private void InitializeForm()
        {
            this.Text = Resources.ProgramTitle;
            this.MinimumSize = new Size(FormConstants.MinimumWindowSizeX, FormConstants.MinimumWindowSizeY);
            this.Size = new Size(FormConstants.InitialWindowSizeX, FormConstants.InitialWindowSizeY);
        }

        private void ArrangeComponents()
        {
            this.mainTableLayout.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            this.mainTableLayout.ColumnCount = FormConstants.MainFormColumnCount;

            this.mainTableLayout.Controls.Add(this.sceneViewer, 1, 0);
            this.mainTableLayout.Controls.Add(this.toolbar, 0, 0);
            this.mainTableLayout.Dock = DockStyle.Fill;
            this.Controls.Add(mainTableLayout);
        }
        #endregion

        #region Handlers 
        private void FillObjectsHandler(object? sender, EventArgs e)
        {
            this.sceneViewer.Fill = !this.sceneViewer.Fill;
            //this.fillSection.Visible = this.sceneViewer.Fill;
            //this.sceneViewer.LightSource.Show = this.sceneViewer.Fill;
        }

        private void VectorInterpolationOptionHandler(object? sender, EventArgs e)
        {
            this.scene.ColorPicker.InterpolationMode = Interpolation.NormalVector;
        }

        private void ColorInterpolationOptionHandler(object? sender, EventArgs e)
        {
            this.scene.ColorPicker.InterpolationMode = Interpolation.Color;
        }

        private void ObjectPickedHandler(BasicSample newValue)
        {
            if (newValue is ObjectSample objectSample)
            {
                this.scene.LoadObjectFromFile(objectSample.Path);
            }
        }

        private void NormalMapPickedHandler(Sample newValue)
        {
            if (newValue is PictureSample pictureSample)
            {
                this.scene.ColorPicker.NormalMap = pictureSample.Image;
            }
        }

        private void TexturePickedHandler(Sample newValue)
        {
            if(newValue is PictureSample pictureSample)
            {
                this.scene.ColorPicker.Pattern = pictureSample.Image;
            }
            else if(newValue is ColorSample colorSample)
            {
                this.scene.ColorPicker.Pattern = null;
                this.scene.ColorPicker.TargetColor = colorSample.Color;
            }
        }

        private void VectorMapHandler(object? sender, EventArgs e)
        {
            var normalMapSample = OpenLoadImageDialog();

            if (normalMapSample != null)
            {
                this.scene.ColorPicker.NormalMap = normalMapSample.Image;
                this.normalMapCombo.AddAndSelect(normalMapSample);
            }
        }

        private void MoveForwardHandler()
        {
            //this.scene.LightSource.MoveLight();
            //this.sceneViewer.RefreshView();
        }

        private void RewindHandler()
        {
            //this.sceneViewer.LightSource.MoveLight(true);
            //this.sceneViewer.Render();
        }

        private void LoadTextureHandlar(object? sender, EventArgs e)
        {
            var textureSample = OpenLoadImageDialog();

            if (textureSample != null)
            {
                this.scene.ColorPicker.Pattern = textureSample.Image;
                this.objectSurfaceCombo.AddAndSelect(textureSample);
            }
        }

        private void NormalVectorsHandler(object? sender, EventArgs e)
        {
            this.scene.ColorPicker.InterpolationMode = this.scene.ColorPicker.InterpolationMode == Interpolation.Color ? Interpolation.NormalVector : Interpolation.Color;
        }

        private void ThreadsSlidrerHandler(float value)
        {
            this.sceneViewer.RenderThreads = (int)(value * 100);
        }

        private void ShowTrackHandler(bool obj)
        {
            this.scene.LightSource.ShowTrack = !this.scene.LightSource.ShowTrack;
        }

        private void MParameterHandler(float newValue)
        {
            this.scene.ColorPicker.M = (int)(100 * newValue);
        }

        private void KSParameterHandler(float newValue)
        {
            this.scene.ColorPicker.KS = newValue;
        }

        private void KDParameterHandler(float newValue)
        {
            this.scene.ColorPicker.KD = newValue;
        }

        private void SunZLocationHandler(float newValue)
        {
            this.scene.LightSource.LightLocationZ = newValue;
        }

        private void SunSpeedHanlder(float newValue)
        {
            this.scene.LightSource.LightSpeed = newValue;
        }

        private void SunHandler(bool newValue)
        {
            this.scene.LightSource.LightAnimation = !this.scene.LightSource.LightAnimation;
        }

        private void ShowLinesHandler(object? sender, EventArgs e)
        {
            this.sceneViewer.ShowLines = !this.sceneViewer.ShowLines;
            this.sceneViewer.RefreshView();
        }

        private void OpenFileHandler(object? sender, EventArgs e)
        {
            var fileName = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Object files (*.obj)|*.obj|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;
                }
            }

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                this.scene.LoadObjectFromFile(fileName);
                this.objectCombo.AddAndSelect(SampleGenerator.CreateObjectSample(fileName));
            }
        }

        private void ResetPositionButtonHandler(object? sender, EventArgs e)
        {
            //this.scene.LightSource.Reset();
        }

        private void LightColorButtonHandler(object? sender, EventArgs e)
        {

            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = this.scene.LightSource.LightSourceColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
                this.scene.LightSource.LightSourceColor = MyDialog.Color;
        }

        private void ObjectColorButtonHandler(object? sender, EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            MyDialog.AllowFullOpen = true;
            MyDialog.ShowHelp = true;
            MyDialog.Color = this.scene.LightSource.LightSourceColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                this.scene.ColorPicker.TargetColor = MyDialog.Color;
                this.colorSample.Color = MyDialog.Color;
                this.objectSurfaceCombo.Select(colorSample);
                this.objectSurfaceCombo.Refresh();
            }
        }
        #endregion

        #region Loading Samples
        private void LoadTextureSamples()
        {
            this.objectSurfaceCombo.AddOptions(SampleGenerator.GetTextures(Resources.TextureAssets, out this.colorSample));
            this.objectSurfaceCombo.DefaultValue = this.colorSample;
        }

        private void LoadNormalMapSamples()
        {
            var samples = SampleGenerator.GetNormalMaps(Resources.NormalMapsAssets);
            this.normalMapCombo.AddOptions(samples);
            this.normalMapCombo.DefaultValue = samples.FirstOrDefault();
        }

        private void LoadObjectSamples()
        {
            var samples = SampleGenerator.GetObjectSamples(Resources.ObjectAssets);
            this.objectCombo.AddOptions(samples);
            this.objectCombo.DefaultValue = samples.FirstOrDefault();
        }

        private PictureSample? OpenLoadImageDialog()
        {
            var fileContent = string.Empty;
            string filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Object files (*.obj)|*.obj|All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                var codecs = ImageCodecInfo.GetImageEncoders();
                var codecFilter = "Image Files|";
                foreach (var codec in codecs)
                {
                    codecFilter += codec.FilenameExtension + ";";
                }
                openFileDialog.Filter = codecFilter;


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                }
            }

            if (!string.IsNullOrWhiteSpace(filePath))
                return SampleGenerator.GetSample(filePath);

            return null;
        }
        #endregion
    }
}
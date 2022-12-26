using ObjectViewer.Samples;
using ObjectViewer.Scenes;
using SketcherControl;
using SketcherControl.Filling;
using SketcherControl.SceneManipulation;
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
        private FlowLayoutPanel freeCameraSection;
        private SceneViewer sceneViewer;
        private Scene scene;
        private ComboPickerWithImage<Sample> objectSurfaceCombo;
        private ComboPickerWithImage<Sample> normalMapCombo;
        private ColorSample colorSample;

        private CameraSample[] cameras =
        {
            new CameraSample(Labels.StaticCamera, new BaseCamera()),
            new CameraSample(Labels.FollowingCamera, new FollowingCamera()),
            new CameraSample(Labels.FirstPersonCamera, new FirstPersonCamera()), 
            new CameraSample(Labels.ThirdPersonCamera, new ThirdPersonCamera()), 
            new CameraSample(Labels.FreeCamera, new FreeCamera()),
        };

        public MainForm()
        {
            InitializeScene();
            InitializeToolbar();
            ArrangeComponents();
            InitializeForm();
            //LoadTextureSamples();
            //LoadNormalMapSamples();
        }

        private void InitializeScene()
        {
            var builder = new SampleSceneBuilder();
            this.scene = builder.Build();
            this.sceneViewer = new(this.scene);
        }

        #region Initialization
        private void InitializeToolbar()
        {
            this.toolbar.AddLabel(Resources.ProgramTitle);
            this.toolbar.AddDivider();
            this.toolbar.AddOption(ShowLinesHandler, Labels.ShowLinesOption, Hints.ShowLines, false);
            this.toolbar.AddOption(FillObjectsHandler, Labels.FillObjectsOption, Hints.FillObjects, true);
            this.toolbar.AddSpacing();
            this.toolbar.AddDivider();
            this.toolbar.StartSection();
            this.toolbar.AddRadioOption(ConstShadersOptionHandler, Labels.ConstShaders);
            this.toolbar.AddRadioOption(ColorInterpolationOptionHandler, Labels.GouraudShaders, Hints.ColorInterpolation, true);
            this.toolbar.AddRadioOption(VectorInterpolationOptionHandler, Labels.PhongShaders, Hints.VectorInterpolation);
            this.toolbar.EndSection();
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.Camera);
            this.freeCameraSection = this.toolbar.StartSection();
            this.toolbar.AddSlider(FovSliderHandler, Labels.FOV, Defaults.FOV);
            this.toolbar.AddSlider(CameraXHandler, Labels.CameraX, Defaults.CameraXLocation);
            this.toolbar.AddSlider(CameraYHandler, Labels.CameraY, Defaults.CameraYLocation);
            this.toolbar.AddSlider(CameraZHandler, Labels.CameraZ, Defaults.CameraZLocation);
            this.toolbar.EndSection();
            this.toolbar.AddComboPicker(CameraChangedHandler, this.cameras, this.cameras.First());
            this.toolbar.AddSpacing();
            this.toolbar.AddDivider();
            this.toolbar.StartSection();
            this.toolbar.AddLabel(Labels.ShaderParameters);
            this.toolbar.AddFractSlider(KDParameterHandler, Labels.KDParameter, Defaults.KDParameter);
            this.toolbar.AddFractSlider(KSParameterHandler, Labels.KSParameter, Defaults.KSParameter);
            this.toolbar.AddSlider(MParameterHandler, Labels.MParameter, Defaults.MParameter);
            this.toolbar.AddDivider();
            //this.toolbar.AddLabel(Labels.ObjectSurface);
            //this.objectSurfaceCombo = this.toolbar.AddComboImagePicker<Sample>(TexturePickedHandler);
            //this.toolbar.AddButton(ObjectColorButtonHandler, Glyphs.Palette, Hints.ChangeObjectColor);
            //this.toolbar.AddButton(LoadTextureHandlar, Glyphs.File, Hints.LoadObjectPattern);
            //this.normalMapCombo = this.toolbar.AddComboImagePicker<Sample>(NormalMapPickedHandler);
            //this.toolbar.AddButton(VectorMapHandler, Glyphs.File, Hints.LoadNormalMap);
            this.toolbar.EndSection();
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
        private void ConstShadersOptionHandler(object? sender, EventArgs e)
        {
            this.scene.ColorPicker.ShadersType = Shaders.Const;
        }

        private void CameraChangedHandler(CameraSample cameraSample)
        {
            sceneViewer.Freeze();
            cameraSample.Camera.Apply(this.sceneViewer);

            this.freeCameraSection.Visible = cameraSample.Name == Labels.FreeCamera;
            sceneViewer.Thaw();
        }

        private void CameraZHandler(float obj)
        {
            this.sceneViewer.CameraVector = new Vector3()
            {
                X = this.sceneViewer.CameraVector.X,
                Y = this.sceneViewer.CameraVector.Y,
                Z = -5 + 10 * obj,
            };
        }

        private void CameraYHandler(float obj)
        {
            this.sceneViewer.CameraVector = new Vector3()
            {
                X = this.sceneViewer.CameraVector.X,
                Y = -5 + 10 * obj,
                Z = this.sceneViewer.CameraVector.Z,
            };
        }

        private void CameraXHandler(float obj)
        {
            this.sceneViewer.CameraVector = new Vector3()
            {
                X = -5 + 10 * obj,
                Y = this.sceneViewer.CameraVector.Y,
                Z = this.sceneViewer.CameraVector.Z,
            };
        }

        private void FovSliderHandler(float obj)
        {
            this.sceneViewer.FOV = (float)(Math.PI / 6 + obj * Math.PI / 2);
        }

        private void FillObjectsHandler(object? sender, EventArgs e)
        {
            this.sceneViewer.Fill = !this.sceneViewer.Fill;
        }

        private void VectorInterpolationOptionHandler(object? sender, EventArgs e)
        {
            this.scene.ColorPicker.ShadersType = Shaders.Phong;
        }

        private void ColorInterpolationOptionHandler(object? sender, EventArgs e)
        {
            this.scene.ColorPicker.ShadersType = Shaders.Gouraud;
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

        private void LoadTextureHandlar(object? sender, EventArgs e)
        {
            var textureSample = OpenLoadImageDialog();

            if (textureSample != null)
            {
                this.scene.ColorPicker.Pattern = textureSample.Image;
                this.objectSurfaceCombo.AddAndSelect(textureSample);
            }
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

        private void ShowLinesHandler(object? sender, EventArgs e)
        {
            this.sceneViewer.ShowLines = !this.sceneViewer.ShowLines;
            this.sceneViewer.RefreshView();
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
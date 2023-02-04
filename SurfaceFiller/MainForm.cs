using ObjectViewer.Samples;
using ObjectViewer.Scenes;
using SketcherControl;
using SketcherControl.Filling;
using SketcherControl.SceneManipulation;
using SurfaceFiller.Components;
using System.Numerics;

namespace SurfaceFiller
{
    public partial class MainForm : Form
    {
        private TableLayoutPanel mainTableLayout = new();
        private Toolbar toolbar = new() { Width = FormConstants.ToolbarWidth };
        private FlowLayoutPanel freeCameraSection;
        private SceneViewer sceneViewer;
        private Scene scene;

        private readonly CameraSample[] cameras =
        {
            new CameraSample(Labels.StaticCamera, new BaseCamera()),
            new CameraSample(Labels.FollowingCamera, new FollowingCamera()),
            new CameraSample(Labels.FirstPersonCamera, new FirstPersonCamera()), 
            new CameraSample(Labels.ThirdPersonCamera, new ThirdPersonCamera()), 
            new CameraSample(Labels.FreeCamera, new FreeCamera()),
        };

        private readonly ShaderSample[] shaders =
        {
            new ShaderSample(Labels.PhongShader, new PhongShader()),
            new ShaderSample(Labels.GouraudShader, new GouraudShader()),
            new ShaderSample(Labels.ConstShader, new ConstShader()),
            new ShaderSample(Labels.SolidShader, new SolidShader()),
        };

        private ShaderParameters shaderParameters;

        public MainForm()
        {
            LoadShaders();
            InitializeScene();
            InitializeToolbar();
            ArrangeComponents();
            InitializeForm();
        }

        private void LoadShaders()
        {
            this.shaderParameters = new();

            foreach (var shaderSample in this.shaders)
            {
                shaderSample.Shader.Parameters = shaderParameters;
            }
        }

        private void InitializeScene()
        {
            var builder = new SampleSceneBuilder();
            this.scene = builder.Build();
            this.scene.Shader = shaders.First().Shader;
            this.sceneViewer = new(this.scene);
        }

        #region Initialization
        private void InitializeToolbar()
        {
            this.toolbar.AddLabel(Resources.ProgramTitle);
            this.toolbar.AddDivider();
            this.toolbar.AddOption(ShowLinesHandler, Labels.ShowLinesOption, Hints.ShowLines, false);
            this.toolbar.AddOption(FillObjectsHandler, Labels.FillObjectsOption, Hints.FillObjects, true);
            this.toolbar.AddOption(FogHandler, Labels.Fog);
            this.toolbar.AddOption(VibrationsHandler, Labels.Vibrations);
            this.toolbar.AddSpacing();
            this.toolbar.StartSection();
            this.toolbar.AddRadioOption(DayRadioHandler, Labels.Day, null, true);
            this.toolbar.AddRadioOption(NightRadioHandler, Labels.Night);
            this.toolbar.EndSection();
            this.toolbar.AddSpacing();
            this.toolbar.AddDivider();
            this.toolbar.AddLabel(Labels.Shaders);
            this.toolbar.AddComboPicker(ShaderChangedHandler, this.shaders, this.shaders.First());
            this.toolbar.AddSpacing();
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
        private void NightRadioHandler(object? sender, EventArgs e)
        {
            this.sceneViewer.NightMode = true;
        }

        private void DayRadioHandler(object? sender, EventArgs e)
        {
            this.sceneViewer.NightMode = false;
        }

        private void FogHandler(bool value)
        {
            this.sceneViewer.Fog = value;
        }

        private void ShaderChangedHandler(ShaderSample shaderSample)
        {
            this.scene.Shader = shaderSample.Shader;
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

        private void VibrationsHandler(bool newValue)
        {
            if(this.scene.MovingObject?.Animation != null)
                this.scene.MovingObject.Animation.IsStopped = !newValue;
        }

        private void FovSliderHandler(float obj)
        {
            this.sceneViewer.FOV = (float)(Math.PI / 6 + obj * Math.PI / 2);
        }

        private void FillObjectsHandler(bool value)
        {
            this.sceneViewer.Fill = value;
        }

        private void MParameterHandler(float newValue)
        {
            this.shaderParameters.M = (int)(100 * newValue);
        }

        private void KSParameterHandler(float newValue)
        {
            this.shaderParameters.KS = newValue;
        }

        private void KDParameterHandler(float newValue)
        {
            this.shaderParameters.KD = newValue;
        }

        private void ShowLinesHandler(bool newValue)
        {
            this.sceneViewer.ShowLines = newValue;
            this.sceneViewer.RefreshView();
        }
        #endregion
    }
}